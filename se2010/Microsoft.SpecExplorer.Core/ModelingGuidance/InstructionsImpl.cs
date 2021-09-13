// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ModelingGuidance.InstructionsImpl
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
  [Serializable]
  public class InstructionsImpl : IInstructions
  {
    private InstructionStepImpl[] steps;

    [XmlElement("Prerequisites")]
    public string Prerequisites { get; set; }

    [XmlIgnore]
    public IInstructionStep[] Steps => (IInstructionStep[]) this.steps;

    [XmlElement("Step")]
    public InstructionStepImpl[] StepsField
    {
      get => this.steps;
      set
      {
        this.steps = value;
        this.IndexSteps();
      }
    }

    private void IndexSteps()
    {
      int index = 0;
      Array.ForEach<InstructionStepImpl>(this.steps, (Action<InstructionStepImpl>) (step => step.Index = step.IsInstructive ? ++index : 0));
    }
  }
}
