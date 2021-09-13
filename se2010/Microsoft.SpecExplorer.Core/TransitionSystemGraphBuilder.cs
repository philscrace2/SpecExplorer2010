// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.TransitionSystemGraphBuilder
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer
{
  public class TransitionSystemGraphBuilder
  {
    private Dictionary<string, Node<State>> labelToNodeDict = new Dictionary<string, Node<State>>();
    private ReferGraph<State, Transition> graph;
    private TransitionSystem transitionSystem;

    public TransitionSystemGraphBuilder(TransitionSystem transitionSystem) => this.transitionSystem = transitionSystem != null ? transitionSystem : throw new ArgumentNullException(nameof (transitionSystem));

    public ReferGraph<State, Transition> BuildGraph()
    {
      if (this.graph != null)
        return this.graph;
      this.graph = new ReferGraph<State, Transition>();
      this.AddNode();
      this.AddEdge();
      return this.graph;
    }

    private void AddNode()
    {
      foreach (State state in this.transitionSystem.States)
      {
        Node<State> node = new Node<State>(state, state.Flags.ToNodeKind());
        this.graph.AddNode(node, ((IEnumerable<string>) this.transitionSystem.InitialStates).Contains<string>(state.Label));
        this.labelToNodeDict[state.Label] = node;
      }
      foreach (State state in this.transitionSystem.States)
      {
        if (!string.IsNullOrEmpty(state.RepresentativeState) && state.RelationKind == StateRelationKind.Equivalent)
          this.labelToNodeDict[state.Label] = this.labelToNodeDict[state.RepresentativeState];
      }
    }

    private void AddEdge()
    {
      foreach (Transition transition in this.transitionSystem.Transitions)
      {
        Node<State> source = this.labelToNodeDict[transition.Source];
        Node<State> target = this.labelToNodeDict[transition.Target];
        List<string> stringList = new List<string>();
        stringList.AddRange((IEnumerable<string>) transition.CapturedRequirements);
        stringList.AddRange((IEnumerable<string>) transition.AssumeCapturedRequirements);
        this.graph.AddEdge(new Edge<State, Transition>(source, target, transition, transition.Action.IsObservable(), (IEnumerable<string>) stringList));
      }
    }
  }
}
