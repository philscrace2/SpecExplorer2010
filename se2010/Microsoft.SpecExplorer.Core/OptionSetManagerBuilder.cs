// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.OptionSetManagerBuilder
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;
using System;
using System.ComponentModel;

namespace Microsoft.SpecExplorer
{
  internal class OptionSetManagerBuilder
  {
    private TransitionSystem transitionSystem;

    internal OptionSetManagerBuilder(TransitionSystem transitionSystem) => this.transitionSystem = transitionSystem != null ? transitionSystem : throw new ArgumentNullException(nameof (transitionSystem));

    internal OptionSetManager CreateOptionSetManager()
    {
      OptionSetManager optionSetManager = new OptionSetManager();
      optionSetManager.Initialize();
      IOptionSet defaultOptionSet = optionSetManager.CreateDefaultOptionSet();
      foreach (ConfigSwitch configSwitch in this.transitionSystem.ConfigSwitches)
      {
        PropertyDescriptor property = optionSetManager.FindProperty(configSwitch.Name, Visibility.Configurable);
        defaultOptionSet.SetValue(property, (object) configSwitch.Value);
      }
      optionSetManager.CurrentOptionSet = defaultOptionSet;
      return optionSetManager;
    }
  }
}
