// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.BrowserEdge
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer.Viewer
{
  public struct BrowserEdge
  {
    public string Text { get; private set; }

    public string ActionText { get; private set; }

    public State Source { get; private set; }

    public State Target { get; private set; }

    public string[] PreConstraints { get; private set; }

    public string[] PostConstraints { get; private set; }

    public string[] unboundVariables { get; private set; }

    public string[] CapturedRequirements { get; private set; }

    public string[] AssumeCapturedRequirements { get; private set; }

    public BrowserEdge(
      string text,
      string actionText,
      State source,
      State target,
      string[] preConstraints,
      string[] postConstraints,
      string[] unboundVariables,
      string[] capturedRequirements,
      string[] assumeCapturedRequirements)
      : this()
    {
      this.Text = text;
      this.ActionText = actionText;
      this.Source = source;
      this.Target = target;
      this.PreConstraints = preConstraints;
      this.PostConstraints = postConstraints;
      this.unboundVariables = unboundVariables;
      this.CapturedRequirements = capturedRequirements;
      this.AssumeCapturedRequirements = assumeCapturedRequirements;
    }
  }
}
