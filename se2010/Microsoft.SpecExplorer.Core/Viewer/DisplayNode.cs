// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.DisplayNode
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.Collections.Generic;
using NodeKind = Microsoft.GraphTraversal.NodeKind;

namespace Microsoft.SpecExplorer.Viewer
{
  public class DisplayNode : Microsoft.GraphTraversal.Node<State>
  {
    internal List<DisplayNode> SubNodes { get; set; }

    internal List<State> LeafNodeStates { get; set; }

    internal DisplayNode ParentNode { get; set; }

    internal DisplayNodeKind DisplayNodeKind { get; set; }

    internal string Text { get; set; }

    internal StateFlags StateFlags { get; set; }

    internal bool IsStart { get; set; }

    internal DisplayNode CollapsedNode { get; set; }

    internal string Id { get; set; }

    internal Microsoft.Msagl.Drawing.Node DrawingNode { get; set; }

    internal DisplayNode(
      DisplayNodeKind displayNodeKind,
      State state,
      string text,
      bool isStart,
      StateFlags flags,
      NodeKind nodeKind)
      : base(state, nodeKind)
    {
      this.DisplayNodeKind = displayNodeKind;
      this.Text = text;
      this.IsStart = isStart;
      this.StateFlags = flags;
      if (this.DisplayNodeKind != DisplayNodeKind.Hyper)
        return;
      this.SubNodes = new List<DisplayNode>();
      this.LeafNodeStates = new List<State>();
    }

    internal void AddSubNode(DisplayNode node)
    {
      if (this.DisplayNodeKind != DisplayNodeKind.Hyper)
        throw new InvalidOperationException("Can not add sub node to non hyper node");
      this.SubNodes.Add(node);
      if (node.DisplayNodeKind == DisplayNodeKind.Hyper)
        this.LeafNodeStates.AddRange((IEnumerable<State>) node.LeafNodeStates);
      else
        this.LeafNodeStates.Add(node.Label);
      this.StateFlags = this.StateFlags | node.StateFlags;
      this.IsStart = this.IsStart || node.IsStart;
    }

    internal void ResetParent() => this.ParentNode = (DisplayNode) null;
  }
}
