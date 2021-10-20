using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	public class PostProcessorHelper : IDisposable
	{
		private const int TimerPeriod = 1000;

		private const string processorNamePrefix = "PostProcossor";

		private IDictionary<string, object> environment;

		private ProgressMessageDisplayer progressMessageDisplayer;

		private AppDomain postAppdomain;

		private MarshalPostProcesser marshalPostProcesser;

		public string CurrentPostProcesser
		{
			get
			{
				if (marshalPostProcesser != null)
				{
					return marshalPostProcesser.Description;
				}
				return string.Empty;
			}
		}

		public PostProcessorHelper(IDictionary<string, object> environment, ProgressMessageDisplayer progressMessageDisplayer)
		{
			this.environment = environment;
			this.progressMessageDisplayer = progressMessageDisplayer;
			AppDomainSetup appDomainSetup = new AppDomainSetup
			{
				LoaderOptimization = LoaderOptimization.MultiDomain
			};
			string localPath = new Uri(typeof(MarshalPostProcesser).Assembly.CodeBase).LocalPath;
			appDomainSetup.ApplicationBase = Path.GetDirectoryName(localPath);
			postAppdomain = AppDomain.CreateDomain(string.Format("Post AppDomain"), null, appDomainSetup);
		}

		public static bool LoadCustomizedPostProcessingTypes(string postProcessorsPath, IHost host, out Dictionary<string, Type> postProcessorTypeMap, out Dictionary<string, string> postProcessorDisplayNameMap)
		{
			if (postProcessorsPath == null)
			{
				throw new ArgumentNullException("postProcessorsPath");
			}
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			postProcessorTypeMap = null;
			postProcessorDisplayNameMap = null;
			string[] source = new string[0];
			try
			{
				if (Directory.Exists(postProcessorsPath))
				{
					source = Directory.GetFiles(postProcessorsPath, "*.dll", SearchOption.TopDirectoryOnly);
				}
			}
			catch (Exception ex)
			{
				host.DiagMessage(DiagnosisKind.Error, string.Format("Cannot retrieve post processor files from directory \"{0}\".\r\n Exception: {1}.\r\n Stack Trace: {2}", postProcessorsPath, ex.Message, ex.StackTrace), null);
				return false;
			}
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			bool flag = true;
			int num = 0;
			foreach (string item in source.Where((string f) => f.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase)))
			{
				Assembly assembly = null;
				try
				{
					assembly = Assembly.LoadFrom(item);
					Type[] types = assembly.GetTypes();
					foreach (Type type in types)
					{
						if (type.GetInterfaces().Contains(typeof(IPostProcessor)))
						{
							string key = "PostProcossor" + num;
							num++;
							ConstructorInfo constructor = type.GetConstructor(new Type[0]);
							if (constructor == null)
							{
								throw new PostProcessorException(string.Format("'{0}' does not contain a constructor that takes 0 arguments", type.FullName));
							}
							IPostProcessor postProcessor = constructor.Invoke(new object[0]) as IPostProcessor;
							string description = postProcessor.Description;
							if (description == null)
							{
								throw new PostProcessorException("Description of processor must be specified");
							}
							if (dictionary2.ContainsValue(description))
							{
								throw new PostProcessorException(string.Format("Duplicate processors '{0}' are found, please check assemblies under $InstallationPath\\Extensions.", description));
							}
							dictionary[key] = type;
							dictionary2[key] = description;
							postProcessor.Dispose();
						}
					}
				}
				catch (PostProcessorException ex2)
				{
					flag = false;
					host.DiagMessage(DiagnosisKind.Error, string.Format("Processor Exception: {0} See output window for detailed stack trace.", ex2.Message), null);
					host.Log(string.Format("Error: Processor Exception: {0} \r\n Stack Trace: \r\n", ex2.Message));
					host.Log(ex2.StackTrace);
				}
				catch (Exception innerException)
				{
					flag = false;
					if (innerException is TargetInvocationException)
					{
						innerException = innerException.InnerException;
					}
					string arg = item;
					if (assembly != null)
					{
						arg = assembly.FullName;
					}
					host.DiagMessage(DiagnosisKind.Error, string.Format("Exception in loading user customized processor: '{0}': {1} See output window for detailed stack trace.", arg, innerException.Message), null);
					host.Log(string.Format("Exception in loading user customized processor: '{0}': {1} \r\n Stack Trace: \r\n", arg, innerException.Message));
					host.Log(innerException.StackTrace);
				}
			}
			if (flag)
			{
				postProcessorTypeMap = dictionary;
				postProcessorDisplayNameMap = dictionary2;
				return true;
			}
			return false;
		}

		public bool ExecutePostProcessing(string fileName, Type type, IHost host)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentException("File name cannot be null or empty.", "fileName");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			bool result = true;
			Timer timer = null;
			try
			{
				marshalPostProcesser = postAppdomain.CreateInstanceAndUnwrap(typeof(MarshalPostProcesser).Assembly.FullName, typeof(MarshalPostProcesser).FullName, new object[0]) as MarshalPostProcesser;
				marshalPostProcesser.Initial(new string[1] { fileName }, type.Assembly.CodeBase, type.FullName);
				DisplayProgressMessage(string.Format("Post processing '{0}' is started.", marshalPostProcesser.Description));
				DateTime startTime = DateTime.Now;
				timer = new Timer(delegate
				{
					string arg = (DateTime.Now - startTime).ToProgressDurationFormat();
					DisplayProgressMessage(string.Format("Post processing '{0}' is in progress, {1} seconds.", marshalPostProcesser.Description, arg));
				}, null, 1000, 1000);
				marshalPostProcesser.Invoke(environment);
				string message = string.Format("'{0}' finished.", marshalPostProcesser.Description);
				DisplayProgressMessage(message);
				host.ProgressMessage(VerbosityLevel.Minimal, message);
				if (marshalPostProcesser.Exception != null)
				{
					result = false;
					if (marshalPostProcesser.Exception is PostProcessorException)
					{
						host.DiagMessage(DiagnosisKind.Error, string.Format("Exception in user processor '{0}': {1} See output window for detailed stack trace. ", marshalPostProcesser.Description, marshalPostProcesser.Exception.Message), null);
						host.Log(string.Format("Error: Exception in user processor '{0}': {1} \r\n Stack Trace: \r\n", marshalPostProcesser.Description, marshalPostProcesser.Exception.Message));
						host.Log(marshalPostProcesser.Exception.StackTrace);
						return result;
					}
					host.DiagMessage(DiagnosisKind.Error, string.Format("Unhandled exception in user processor '{0}': {1} See output window for detailed stack trace. ", marshalPostProcesser.Description, marshalPostProcesser.Exception.Message), null);
					host.Log(string.Format("Error: Unhandled exception in user processor '{0}': {1} \r\n Stack Trace: \r\n", marshalPostProcesser.Description, marshalPostProcesser.Exception.Message));
					host.Log(marshalPostProcesser.Exception.StackTrace);
					return result;
				}
				return result;
			}
			finally
			{
				if (marshalPostProcesser != null)
				{
					marshalPostProcesser.Dispose();
					marshalPostProcesser = null;
				}
				if (timer != null)
				{
					timer.Dispose();
					timer = null;
				}
			}
		}

		public void ExecutePostProcessing(string fileName, IEnumerable<Type> types, IHost host)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentException("File name cannot be null or empty.", "fileName");
			}
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			foreach (Type type in types)
			{
				ExecutePostProcessing(fileName, type, host);
			}
		}

		public void Dispose()
		{
			if (marshalPostProcesser != null)
			{
				marshalPostProcesser.Dispose();
			}
			AppDomain.Unload(postAppdomain);
		}

		private void DisplayProgressMessage(string message)
		{
			if (progressMessageDisplayer != null)
			{
				progressMessageDisplayer(message);
			}
		}
	}
}
