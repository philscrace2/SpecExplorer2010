// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ModelingGuidance.InstructionStepImpl
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
  [XmlType("InstructionStep")]
  [Serializable]
  public class InstructionStepImpl : IInstructionStep
  {
    [XmlAttribute("Header")]
    public string Title { get; set; }

    [XmlElement("StepDetails")]
    public string StepDetails { get; set; }

    [XmlAttribute("Optional")]
    public bool IsOptional { get; set; }

    [XmlIgnore]
    public ICodeBlock Code => (ICodeBlock) this.CodeField;

    [XmlIgnore]
    public bool HasContent => this.StepDetails != null || this.Code != null;

    [XmlIgnore]
    public int Index { get; set; }

    [XmlAttribute("Instructive")]
    public bool IsInstructive { get; set; }

    [XmlElement("Code")]
    public CodeBlockImpl CodeField { get; set; }
  }
}
