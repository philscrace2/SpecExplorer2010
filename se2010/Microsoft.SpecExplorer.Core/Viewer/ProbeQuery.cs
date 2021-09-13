// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.ProbeQuery
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.SpecExplorer.Viewer
{
  public class ProbeQuery : IViewQuery
  {
    private string probeName;
    private string resolvedProbeName;

    public ProbeQuery(string probeName) => this.probeName = probeName;

    public string GetLabel(State state)
    {
      foreach (Probe probe in state.Probes)
      {
        if (this.resolvedProbeName.Equals(probe.Name))
        {
          string str = probe.Value;
          if (probe.Kind != null)
            return (string) null;
          return string.Compare(probe.Type.FullName, "System.String", true) == 0 || string.Compare(probe.Type.FullName, "System.Char", true) == 0 && str.StartsWith("'") && str.EndsWith("'") ? str.Substring(1, str.Length - 2) : str;
        }
      }
      throw new QueryException(string.Format("Can not find the probe: '{0}'.", (object) this.resolvedProbeName));
    }

    public IEnumerable<DisplayNode> GetHyperNodes(
      ICollection<DisplayNode> nodes)
    {
      Dictionary<string, DisplayNode> dictionary = new Dictionary<string, DisplayNode>();
      DisplayNode displayNode1 = new DisplayNode(DisplayNodeKind.Hyper, new State(), "<<Error>>", false, (StateFlags) 0, NodeKind.Regular);
      DisplayNode displayNode2 = new DisplayNode(DisplayNodeKind.Hyper, new State(), "<<Exception>>", false, (StateFlags) 0, NodeKind.Regular);
      foreach (DisplayNode node in (IEnumerable<DisplayNode>) nodes)
      {
        if ((node.Label.Flags & 4) != null)
        {
          displayNode1.AddSubNode(node);
        }
        else
        {
          string label = this.GetLabel(node.Label);
          if (label == null)
          {
            displayNode2.AddSubNode(node);
          }
          else
          {
            DisplayNode displayNode3;
            if (!dictionary.TryGetValue(label, out displayNode3))
            {
              displayNode3 = new DisplayNode(DisplayNodeKind.Hyper, new State(), label, false, (StateFlags) 0, NodeKind.Regular);
              dictionary[label] = displayNode3;
              displayNode3.Text = label;
            }
            displayNode3.AddSubNode(node);
            node.ParentNode = displayNode3;
          }
        }
      }
      List<DisplayNode> displayNodeList = new List<DisplayNode>((IEnumerable<DisplayNode>) dictionary.Values);
      if (displayNode1.SubNodes.Count > 0)
        displayNodeList.Add(displayNode1);
      if (displayNode2.SubNodes.Count > 0)
        displayNodeList.Add(displayNode2);
      return (IEnumerable<DisplayNode>) displayNodeList;
    }

    public void DivideHyperNodes(DisplayNode parentNode)
    {
      if (parentNode.DisplayNodeKind != DisplayNodeKind.Hyper)
        return;
      foreach (DisplayNode hyperNode in this.GetHyperNodes((ICollection<DisplayNode>) parentNode.SubNodes))
        hyperNode.ParentNode = parentNode;
    }

    public bool ValidateViewQuery(TransitionSystem transitionSystem, out string errorMessage)
    {
      errorMessage = "";
      if (((IEnumerable<State>) transitionSystem.States).Count<State>() <= 0)
        return true;
      State state = transitionSystem.States[0];
      List<string> stringList = new List<string>();
      foreach (Probe probe in state.Probes)
      {
        if (probe.Name.EndsWith(this.probeName))
          stringList.Add(probe.Name);
      }
      if (stringList.Count > 1)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat("The probe '{0}' is ambiguous for ", (object) this.probeName);
        foreach (string str in stringList)
          stringBuilder.AppendFormat("'{0}',", (object) str);
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
        stringBuilder.Append(". Please input full probe name.");
        errorMessage = stringBuilder.ToString();
        return false;
      }
      if (stringList.Count == 0)
      {
        errorMessage = string.Format("Can not find the probe: '{0}'.", (object) this.probeName);
        return false;
      }
      this.resolvedProbeName = stringList[0];
      return true;
    }
  }
}
