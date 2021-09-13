// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.MarshalPostProcesser
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer
{
  internal class MarshalPostProcesser : DisposableMarshalByRefObject
  {
    private IPostProcessor processor;
    private string[] fileNames;

    internal string Description => this.processor.Description;

    internal Exception Exception { get; private set; }

    internal void Initial(string[] fileNames, string assemblyPath, string typeName)
    {
      foreach (System.Type type in System.Reflection.Assembly.LoadFrom(assemblyPath).GetTypes())
      {
        if (type.FullName.Equals(typeName))
          this.processor = type.GetConstructor(new System.Type[0]).Invoke(new object[0]) as IPostProcessor;
      }
      this.fileNames = fileNames;
    }

    internal void Invoke(IDictionary<string, object> environment)
    {
      try
      {
        List<TransitionSystem> transitionSystemList = new List<TransitionSystem>();
        foreach (string fileName in this.fileNames)
        {
          ExplorationResultLoader explorationResultLoader = new ExplorationResultLoader(fileName);
          transitionSystemList.Add(explorationResultLoader.LoadTransitionSystem());
        }
        this.processor.PostProcess((IEnumerable<TransitionSystem>) transitionSystemList, environment);
      }
      catch (Exception ex)
      {
        this.Exception = ex;
      }
    }

    public override void Dispose()
    {
      base.Dispose();
      ((IDisposable) this.processor).Dispose();
    }
  }
}
