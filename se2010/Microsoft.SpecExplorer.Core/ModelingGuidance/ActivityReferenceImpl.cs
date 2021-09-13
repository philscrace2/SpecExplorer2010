// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ModelingGuidance.ActivityReferenceImpl
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
  [Serializable]
  public class ActivityReferenceImpl : IActivityReference, INotifyPropertyChanged
  {
    private bool isSelected;
    private bool isCompleted;

    [XmlAttribute("Ref")]
    public string RefId { get; set; }

    [XmlIgnore]
    public IActivity Activity { get; set; }

    [XmlIgnore]
    public bool IsCompleted
    {
      get => this.isCompleted;
      set
      {
        this.isCompleted = value;
        this.SendNotification(nameof (IsCompleted));
      }
    }

    [XmlIgnore]
    public bool IsSelected
    {
      get => this.isSelected;
      set
      {
        this.isSelected = value;
        this.SendNotification(nameof (IsSelected));
      }
    }

    [XmlIgnore]
    public int Index { get; set; }

    [XmlAttribute("Optional")]
    public bool IsOptional { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    private void SendNotification(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
