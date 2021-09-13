// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.Views
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.Viewer
{
  [Serializable]
  public class Views
  {
    [XmlAttribute("Version")]
    public string Version { get; set; }

    [XmlArray("ViewList")]
    public ViewDefinition[] ViewList { get; set; }

    public Views()
    {
      this.Version = "1.0";
      this.ViewList = (ViewDefinition[]) null;
    }
  }
}
