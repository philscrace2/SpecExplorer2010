// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.FindStateSettings
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer.Viewer
{
  internal class FindStateSettings
  {
    internal FindStateSettings(GViewer viewer)
    {
      this.CurrentNodeIndex = -1;
      this.CurrentGViewer = viewer;
      this.SearchString = string.Empty;
      this.CurrentHighlightedNodes = new Dictionary<Microsoft.Msagl.Drawing.Node, NodeAttr>();
    }

    public int CurrentNodeIndex { get; set; }

    public GViewer CurrentGViewer { get; set; }

    public Dictionary<Microsoft.Msagl.Drawing.Node, NodeAttr> CurrentHighlightedNodes { get; set; }

    public List<DisplayNode> OrderedDisplayNode { get; set; }

    public int FindScopeIndex { get; set; }

    public string SearchString { get; set; }

    public bool MatchCaseChecked { get; set; }

    public bool MatchWholeWordChecked { get; set; }

    public bool SearchUpChecked { get; set; }

    public bool UseOptionChecked { get; set; }
  }
}
