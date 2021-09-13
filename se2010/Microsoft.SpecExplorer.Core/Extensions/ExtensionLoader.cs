// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Extensions.ExtensionLoader
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.Properties;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
      string[] strArray = (string[]) null;
      if (string.IsNullOrEmpty(baseDir))
        throw new ArgumentException("baseDir cannot be null or empty", nameof (baseDir));
      if (needPreLoadDependentAssemblies)
      {
        string str = Path.Combine(baseDir, Resources.GraphTraversalAssemblyName);
        if (File.Exists(str))
          System.Reflection.Assembly.LoadFrom(str);
      }
      string path = Path.Combine(baseDir, Resources.ExtensionDirectoryName);
      try
      {
        if (Directory.Exists(path))
          strArray = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
      }
      catch (Exception ex)
      {
        this.explorerMediator.DiagMessage(DiagnosisKind.Error, string.Format("Extension(s) Registration failure: Cannot read extension files from directory \"{0}\".\r\n Exception: {1}.\r\n Stack Trace: {2}", (object) path, (object) ex.Message, (object) ex.StackTrace), (object) null);
      }
      try
      {
        if (strArray == null)
          return;
        foreach (string assemblyFile in ((IEnumerable<string>) strArray).Where<string>((Func<string, bool>) (f => f.EndsWith(".dll", StringComparison.CurrentCultureIgnoreCase))))
        {
          System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(assemblyFile);
          System.Type type1 = typeof (ComponentBase);
          foreach (System.Type type2 in assembly.GetTypes())
          {
            foreach (Attribute customAttribute in Attribute.GetCustomAttributes((System.Reflection.MemberInfo) type2))
            {
              if (customAttribute is SpecExplorerExtensionAttribute && type1.IsAssignableFrom(type2))
              {
                System.Reflection.ConstructorInfo constructor = type2.GetConstructor(new System.Type[0]);
                if (constructor != (System.Reflection.ConstructorInfo) null)
                {
                  ComponentBase componentBase = (ComponentBase) constructor.Invoke(new object[0]);
                  if (componentBase != null)
                  {
                    this.setup.Add((IComponent) componentBase);
                    componentBase.Initialize();
                  }
                }
                else
                  this.explorerMediator.DiagMessage(DiagnosisKind.Error, string.Format("Extension(s) Registration failure: Cannot get constructor for type {0}.", (object) type2.FullName), (object) null);
              }
            }
          }
        }
      }
      catch (ReflectionTypeLoadException ex)
      {
        StringBuilder stringBuilder = new StringBuilder();
        if (ex.LoaderExceptions != null)
        {
          foreach (Exception loaderException in ex.LoaderExceptions)
            stringBuilder.AppendLine(string.Format("Extension(s) Registration failure: {0}.\r\n StackTrace: {1}. ", (object) loaderException.Message, (object) loaderException.StackTrace));
        }
        else
          stringBuilder.AppendLine(string.Format("Extension(s) Registration failure: {0}.\r\n StackTrace: {1}. ", (object) ex.Message, (object) ex.StackTrace));
        this.explorerMediator.DiagMessage(DiagnosisKind.Error, stringBuilder.ToString(), (object) null);
      }
      catch (Exception ex)
      {
        this.explorerMediator.DiagMessage(DiagnosisKind.Error, string.Format("Extension(s) Registration failure: {0}.\r\n Stack Trace: {1}", (object) ex.Message, (object) ex.StackTrace), (object) null);
      }
    }
  }
}
