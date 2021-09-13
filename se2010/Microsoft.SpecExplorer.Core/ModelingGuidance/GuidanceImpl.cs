// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ModelingGuidance.GuidanceImpl
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
  [XmlRoot("Guidance")]
  [Serializable]
  public class GuidanceImpl : IGuidance
  {
    private ActivityReferenceImpl[] structure;

    [XmlAttribute("Id")]
    public string Id { get; set; }

    [XmlAttribute("Description")]
    public string Description { get; set; }

    [XmlElement("Explanation")]
    public string Explanation { get; set; }

    [XmlIgnore]
    public IActivity[] Activities => (IActivity[]) this.ActivitiesField;

    [XmlIgnore]
    public IActivityReference[] Structure => (IActivityReference[]) this.structure;

    [XmlArrayItem("Activity")]
    [XmlArray("Activities")]
    public ActivityImpl[] ActivitiesField { get; set; }

    [XmlArray("Structure")]
    [XmlArrayItem("Activity")]
    public ActivityReferenceImpl[] StructureField
    {
      get => this.structure;
      set
      {
        this.structure = value;
        this.LoadReferences();
      }
    }

    private void LoadReferences()
    {
      if (this.structure == null || this.Activities == null)
        return;
      Array.ForEach<ActivityReferenceImpl>(this.structure, (Action<ActivityReferenceImpl>) (actRef =>
      {
        actRef.Activity = ((IEnumerable<IActivity>) this.Activities).First<IActivity>((Func<IActivity, bool>) (act => act.Id == actRef.RefId));
        actRef.Index = Array.IndexOf<ActivityReferenceImpl>(this.structure, actRef) + 1;
      }));
    }
  }
}
