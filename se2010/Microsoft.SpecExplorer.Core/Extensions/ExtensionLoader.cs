using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.SpecExplorer.Properties;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer.Extensions
{
	public class ExtensionLoader
	{
		private ComponentSetup setup;

		private ExplorerMediator explorerMediator;

		public ExtensionLoader(ComponentSetup setup, ExplorerMediator explorerMediator)
		{
			this.setup = setup;
			this.explorerMediator = explorerMediator;
		}

		public void LoadExtension(string baseDir, bool needPreLoadDependentAssemblies)
		{
			string[] array = null;
			if (string.IsNullOrEmpty(baseDir))
			{
				throw new ArgumentException("baseDir cannot be null or empty", "baseDir");
			}
			if (needPreLoadDependentAssemblies)
			{
				string text = Path.Combine(baseDir, Resources.GraphTraversalAssemblyName);
				if (File.Exists(text))
				{
					Assembly.LoadFrom(text);
				}
			}
			string text2 = Path.Combine(baseDir, Resources.ExtensionDirectoryName);
			try
			{
				if (Directory.Exists(text2))
				{
					array = Directory.GetFiles(text2, "*.dll", SearchOption.AllDirectories);
				}
			}
			catch (Exception ex)
			{
				explorerMediator.DiagMessage(DiagnosisKind.Error, string.Format("Extension(s) Registration failure: Cannot read extension files from directory \"{0}\".\r\n Exception: {1}.\r\n Stack Trace: {2}", text2, ex.Message, ex.StackTrace), null);
			}
			try
			{
				if (array == null)
				{
					return;
				}
				foreach (string item in array.Where((string f) => f.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase)))
				{
					Assembly assembly = Assembly.LoadFrom(item);
					Type typeFromHandle = typeof(ComponentBase);
					Type[] types = assembly.GetTypes();
					foreach (Type type in types)
					{
						Attribute[] customAttributes = Attribute.GetCustomAttributes(type);
						foreach (Attribute attribute in customAttributes)
						{
							if (!(attribute is SpecExplorerExtensionAttribute) || !typeFromHandle.IsAssignableFrom(type))
							{
								continue;
							}
							ConstructorInfo constructor = type.GetConstructor(new Type[0]);
							if (constructor != null)
							{
								ComponentBase componentBase = (ComponentBase)constructor.Invoke(new object[0]);
								if (componentBase != null)
								{
									setup.Add(componentBase);
									componentBase.Initialize();
								}
							}
							else
							{
								explorerMediator.DiagMessage(DiagnosisKind.Error, string.Format("Extension(s) Registration failure: Cannot get constructor for type {0}.", type.FullName), null);
							}
						}
					}
				}
			}
			catch (ReflectionTypeLoadException ex2)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (ex2.LoaderExceptions != null)
				{
					Exception[] loaderExceptions = ex2.LoaderExceptions;
					foreach (Exception ex3 in loaderExceptions)
					{
						stringBuilder.AppendLine(string.Format("Extension(s) Registration failure: {0}.\r\n StackTrace: {1}. ", ex3.Message, ex3.StackTrace));
					}
				}
				else
				{
					stringBuilder.AppendLine(string.Format("Extension(s) Registration failure: {0}.\r\n StackTrace: {1}. ", ex2.Message, ex2.StackTrace));
				}
				explorerMediator.DiagMessage(DiagnosisKind.Error, stringBuilder.ToString(), null);
			}
			catch (Exception ex4)
			{
				explorerMediator.DiagMessage(DiagnosisKind.Error, string.Format("Extension(s) Registration failure: {0}.\r\n Stack Trace: {1}", ex4.Message, ex4.StackTrace), null);
			}
		}
	}
}
