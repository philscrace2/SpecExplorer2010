// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.PostProcessorHelper
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Threading;

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

    public string CurrentPostProcesser => this.marshalPostProcesser != null ? this.marshalPostProcesser.Description : string.Empty;

    public PostProcessorHelper(
      IDictionary<string, object> environment,
      ProgressMessageDisplayer progressMessageDisplayer)
    {
      this.environment = environment;
      this.progressMessageDisplayer = progressMessageDisplayer;
      AppDomainSetup info = new AppDomainSetup();
      info.LoaderOptimization = LoaderOptimization.MultiDomain;
      string localPath = new Uri(typeof (MarshalPostProcesser).Assembly.CodeBase).LocalPath;
      info.ApplicationBase = Path.GetDirectoryName(localPath);
      this.postAppdomain = AppDomain.CreateDomain(string.Format("Post AppDomain"), (Evidence) null, info);
    }

    public static bool LoadCustomizedPostProcessingTypes(
      string postProcessorsPath,
      IHost host,
      out Dictionary<string, Type> postProcessorTypeMap,
      out Dictionary<string, string> postProcessorDisplayNameMap)
    {
      if (postProcessorsPath == null)
        throw new ArgumentNullException(nameof (postProcessorsPath));
      if (host == null)
        throw new ArgumentNullException(nameof (host));
      postProcessorTypeMap = (Dictionary<string, Type>) null;
      postProcessorDisplayNameMap = (Dictionary<string, string>) null;
      string[] strArray = new string[0];
      try
      {
        if (Directory.Exists(postProcessorsPath))
          strArray = Directory.GetFiles(postProcessorsPath, "*.dll", SearchOption.TopDirectoryOnly);
      }
      catch (Exception ex)
      {
        host.DiagMessage(DiagnosisKind.Error, string.Format("Cannot retrieve post processor files from directory \"{0}\".\r\n Exception: {1}.\r\n Stack Trace: {2}", (object) postProcessorsPath, (object) ex.Message, (object) ex.StackTrace), (object) null);
        return false;
      }
      Dictionary<string, Type> dictionary1 = new Dictionary<string, Type>();
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      bool flag = true;
      int num = 0;
      foreach (string assemblyFile in ((IEnumerable<string>) strArray).Where<string>((Func<string, bool>) (f => f.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase))))
      {
        Assembly assembly = (Assembly) null;
        try
        {
          assembly = Assembly.LoadFrom(assemblyFile);
          foreach (Type type in assembly.GetTypes())
          {
            if (((IEnumerable<Type>) type.GetInterfaces()).Contains<Type>(typeof (IPostProcessor)))
            {
              string key = "PostProcossor" + num.ToString();
              ++num;
              ConstructorInfo constructor = type.GetConstructor(new Type[0]);
              IPostProcessor ipostProcessor = !(constructor == (ConstructorInfo) null) ? constructor.Invoke(new object[0]) as IPostProcessor : throw new PostProcessorException(string.Format("'{0}' does not contain a constructor that takes 0 arguments", (object) type.FullName));
              string description = ipostProcessor.Description;
              if (description == null)
                throw new PostProcessorException("Description of processor must be specified");
              if (dictionary2.ContainsValue(description))
                throw new PostProcessorException(string.Format("Duplicate processors '{0}' are found, please check assemblies under $InstallationPath\\Extensions.", (object) description));
              dictionary1[key] = type;
              dictionary2[key] = description;
              ((IDisposable) ipostProcessor).Dispose();
            }
          }
        }
        catch (PostProcessorException ex)
        {
          flag = false;
          host.DiagMessage(DiagnosisKind.Error, string.Format("Processor Exception: {0} See output window for detailed stack trace.", (object) ((Exception) ex).Message), (object) null);
          host.Log(string.Format("Error: Processor Exception: {0} \r\n Stack Trace: \r\n", (object) ((Exception) ex).Message));
          host.Log(((Exception) ex).StackTrace);
        }
        catch (Exception ex)
        {
          Exception exception = ex;
          flag = false;
          if (exception is TargetInvocationException)
            exception = exception.InnerException;
          string str = assemblyFile;
          if (assembly != (Assembly) null)
            str = assembly.FullName;
          host.DiagMessage(DiagnosisKind.Error, string.Format("Exception in loading user customized processor: '{0}': {1} See output window for detailed stack trace.", (object) str, (object) exception.Message), (object) null);
          host.Log(string.Format("Exception in loading user customized processor: '{0}': {1} \r\n Stack Trace: \r\n", (object) str, (object) exception.Message));
          host.Log(exception.StackTrace);
        }
      }
      if (!flag)
        return false;
      postProcessorTypeMap = dictionary1;
      postProcessorDisplayNameMap = dictionary2;
      return true;
    }

    public bool ExecutePostProcessing(string fileName, Type type, IHost host)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentException("File name cannot be null or empty.", nameof (fileName));
      if (type == (Type) null)
        throw new ArgumentNullException(nameof (type));
      if (host == null)
        throw new ArgumentNullException(nameof (host));
      bool flag = true;
      Timer timer = (Timer) null;
      try
      {
        this.marshalPostProcesser = this.postAppdomain.CreateInstanceAndUnwrap(typeof (MarshalPostProcesser).Assembly.FullName, typeof (MarshalPostProcesser).FullName, new object[0]) as MarshalPostProcesser;
        this.marshalPostProcesser.Initial(new string[1]
        {
          fileName
        }, type.Assembly.CodeBase, type.FullName);
        this.DisplayProgressMessage(string.Format("Post processing '{0}' is started.", (object) this.marshalPostProcesser.Description));
        DateTime startTime = DateTime.Now;
        timer = new Timer((TimerCallback) (obj => this.DisplayProgressMessage(string.Format("Post processing '{0}' is in progress, {1} seconds.", (object) this.marshalPostProcesser.Description, (object) (DateTime.Now - startTime).ToProgressDurationFormat()))), (object) null, 1000, 1000);
        this.marshalPostProcesser.Invoke(this.environment);
        string message = string.Format("'{0}' finished.", (object) this.marshalPostProcesser.Description);
        this.DisplayProgressMessage(message);
        host.ProgressMessage(VerbosityLevel.Minimal, message);
        if (this.marshalPostProcesser.Exception != null)
        {
          flag = false;
          if (this.marshalPostProcesser.Exception is PostProcessorException)
          {
            host.DiagMessage(DiagnosisKind.Error, string.Format("Exception in user processor '{0}': {1} See output window for detailed stack trace. ", (object) this.marshalPostProcesser.Description, (object) this.marshalPostProcesser.Exception.Message), (object) null);
            host.Log(string.Format("Error: Exception in user processor '{0}': {1} \r\n Stack Trace: \r\n", (object) this.marshalPostProcesser.Description, (object) this.marshalPostProcesser.Exception.Message));
            host.Log(this.marshalPostProcesser.Exception.StackTrace);
          }
          else
          {
            host.DiagMessage(DiagnosisKind.Error, string.Format("Unhandled exception in user processor '{0}': {1} See output window for detailed stack trace. ", (object) this.marshalPostProcesser.Description, (object) this.marshalPostProcesser.Exception.Message), (object) null);
            host.Log(string.Format("Error: Unhandled exception in user processor '{0}': {1} \r\n Stack Trace: \r\n", (object) this.marshalPostProcesser.Description, (object) this.marshalPostProcesser.Exception.Message));
            host.Log(this.marshalPostProcesser.Exception.StackTrace);
          }
        }
      }
      finally
      {
        if (this.marshalPostProcesser != null)
        {
          this.marshalPostProcesser.Dispose();
          this.marshalPostProcesser = (MarshalPostProcesser) null;
        }
        timer?.Dispose();
      }
      return flag;
    }

    public void ExecutePostProcessing(string fileName, IEnumerable<Type> types, IHost host)
    {
      if (string.IsNullOrEmpty(fileName))
        throw new ArgumentException("File name cannot be null or empty.", nameof (fileName));
      if (types == null)
        throw new ArgumentNullException(nameof (types));
      if (host == null)
        throw new ArgumentNullException(nameof (host));
      foreach (Type type in types)
        this.ExecutePostProcessing(fileName, type, host);
    }

    public void Dispose()
    {
      if (this.marshalPostProcesser != null)
        this.marshalPostProcesser.Dispose();
      AppDomain.Unload(this.postAppdomain);
    }

    private void DisplayProgressMessage(string message)
    {
      if (this.progressMessageDisplayer == null)
        return;
      this.progressMessageDisplayer(message);
    }
  }
}
