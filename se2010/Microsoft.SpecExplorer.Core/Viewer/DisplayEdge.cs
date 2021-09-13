// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.DisplayEdge
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SpecExplorer.Viewer
{
  internal class DisplayEdge : Edge<State, Transition>
  {
    private StringBuilder textBuilder = new StringBuilder();
    private List<string> capturedRequirements = new List<string>();
    private List<string> assumeCapturedRequirements = new List<string>();

    private DisplayEdge parentEdge { get; set; }

    internal List<DisplayEdge> subEdges { get; set; }

    internal DisplayEdgeKind displayEdgeKind { get; set; }

    internal string Id { get; set; }

    internal ActionSymbolKind Kind { get; set; }

    internal IList<string> CapturedRequirements => (IList<string>) this.capturedRequirements;

    internal IList<string> AssumeCapturedRequirements => (IList<string>) this.assumeCapturedRequirements;

    internal string Text
    {
      get => this.textBuilder.ToString();
      set
      {
        this.textBuilder = new StringBuilder();
        this.textBuilder.Append(value);
      }
    }

    internal string ActionText { get; private set; }

    internal DisplayEdge(DisplayNode source, DisplayNode target, DisplayEdgeKind kind)
      : base((Node<State>) source, (Node<State>) target, new Transition(), false, (IEnumerable<string>) null)
    {
      this.subEdges = new List<DisplayEdge>();
      this.displayEdgeKind = kind;
    }

    internal DisplayEdge(
      DisplayNode source,
      DisplayNode target,
      Transition trans,
      bool isDisplayRequirements)
      : base((Node<State>) source, (Node<State>) target, trans, trans.Action.IsObservable(), (IEnumerable<string>) null)
    {
      this.displayEdgeKind = DisplayEdgeKind.Normal;
      if (trans.Action == null || trans.Action.Symbol == null)
      {
        this.Kind = (ActionSymbolKind) 0;
      }
      else
      {
        this.Kind = trans.Action.Symbol.Kind;
        this.Text = this.GetEdgeDisplayText(trans);
        this.ActionText = trans.Action.Symbol.ToDisplayText();
      }
      this.capturedRequirements.AddRange((IEnumerable<string>) trans.CapturedRequirements);
      this.assumeCapturedRequirements.AddRange((IEnumerable<string>) trans.AssumeCapturedRequirements);
      if (!isDisplayRequirements)
        return;
      this.AppendRequirements((IEnumerable<string>) trans.CapturedRequirements, (IEnumerable<string>) trans.AssumeCapturedRequirements, this.textBuilder.Length);
    }

    internal DisplayEdge(DisplayEdge inEdge, DisplayEdge outEdge, bool isDisplayRequirements)
      : base(inEdge.Source, outEdge.Target, new Transition(), false, (IEnumerable<string>) null)
    {
      this.displayEdgeKind = DisplayEdgeKind.Collapsed;
      this.Kind = (ActionSymbolKind) 1;
      this.subEdges = new List<DisplayEdge>();
      this.subEdges.Add(inEdge);
      this.subEdges.Add(outEdge);
      this.BuildCollapseEdgeText(inEdge.Text, outEdge.Text);
      this.ActionText = inEdge.ActionText;
      this.capturedRequirements.AddRange((IEnumerable<string>) inEdge.Label.CapturedRequirements);
      this.assumeCapturedRequirements.AddRange((IEnumerable<string>) inEdge.Label.AssumeCapturedRequirements);
      this.capturedRequirements.AddRange((IEnumerable<string>) outEdge.Label.CapturedRequirements);
      this.assumeCapturedRequirements.AddRange((IEnumerable<string>) outEdge.Label.AssumeCapturedRequirements);
      if (!isDisplayRequirements)
        return;
      this.AppendRequirements((IEnumerable<string>) this.capturedRequirements, (IEnumerable<string>) this.assumeCapturedRequirements, this.textBuilder.Length);
    }

    internal DisplayEdge(DisplayNode source, DisplayNode target, DisplayEdge displayEdge)
      : base((Node<State>) source, (Node<State>) target, displayEdge.Label, displayEdge.IsObservable, displayEdge.Requirements)
    {
      this.subEdges = displayEdge.subEdges;
      this.parentEdge = displayEdge.parentEdge;
      this.textBuilder = displayEdge.textBuilder;
      this.ActionText = displayEdge.ActionText;
      this.displayEdgeKind = displayEdge.displayEdgeKind;
      this.Kind = displayEdge.Kind;
    }

    internal void AddSubEdge(DisplayEdge edge)
    {
      if (this.displayEdgeKind != DisplayEdgeKind.Hyper)
        throw new InvalidOperationException("Can not add sub edge to non hyper edge");
      this.subEdges.Add(edge);
      this.Kind = this.subEdges.Count != 1 ? this.Kind | edge.Kind : edge.Kind;
      edge.parentEdge = this;
      if (edge.Label.CapturedRequirements != null)
        this.capturedRequirements.AddRange((IEnumerable<string>) edge.Label.CapturedRequirements);
      if (edge.Label.AssumeCapturedRequirements != null)
        this.assumeCapturedRequirements.AddRange((IEnumerable<string>) edge.Label.AssumeCapturedRequirements);
      if (this.textBuilder.Length > 0)
        this.textBuilder.Append("\r\n");
      this.textBuilder.Append(edge.Text);
    }

    private void BuildCollapseEdgeText(string inText, string outText)
    {
      int length1 = inText.IndexOf('\r');
      if (length1 > 0)
        inText = inText.Substring(0, length1);
      int length2 = outText.IndexOf('\r');
      if (length2 > 0)
        outText = outText.Substring(0, length2);
      this.textBuilder.Append(inText.Substring(5));
      int startIndex = outText.IndexOf('/');
      if (startIndex <= 0)
        return;
      this.textBuilder.Append(outText.Substring(startIndex));
    }

    private void AppendRequirements(
      IEnumerable<string> capturedRequirements,
      IEnumerable<string> assumeCapturedRequirements,
      int maxWidth)
    {
      string text = new RequirementSequence(capturedRequirements, assumeCapturedRequirements).ToString();
      if (string.IsNullOrEmpty(text))
        return;
      this.textBuilder.AppendLine();
      this.textBuilder.Append(AnnotationFormatter.Format(text, maxWidth));
    }

    private string GetEdgeDisplayText(Transition trans)
    {
      if (trans.Action.Symbol.Kind != ActionSymbolKind.PreConstraintCheck)
        return trans.Action.Text;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("check:");
      if (trans.PreConstraints.Length > 0)
      {
        foreach (Constraint preConstraint in trans.PreConstraints)
          stringBuilder.AppendLine(preConstraint.Text);
      }
      return stringBuilder.ToString();
    }
  }
}
