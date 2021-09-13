// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ModelingGuidance.ActivityImpl
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
  [Serializable]
  public class ActivityImpl : IActivity
  {
    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlAttribute("Description")]
    public string Description { get; set; }

    [XmlElement("Explanation")]
    public string Explanation { get; set; }

    [XmlIgnore]
    public IInstructions Instructions => (IInstructions) this.IntructionsField;

    [XmlElement("Instructions")]
    public InstructionsImpl IntructionsField { get; set; }
  }
}
