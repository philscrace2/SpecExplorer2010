using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	public class TransitionSystemGraphBuilder
	{
		private Dictionary<string, Node<State>> labelToNodeDict = new Dictionary<string, Node<State>>();

		private ReferGraph<State, Transition> graph;

		private TransitionSystem transitionSystem;

		public TransitionSystemGraphBuilder(TransitionSystem transitionSystem)
		{
			if (transitionSystem == null)
			{
				throw new ArgumentNullException("transitionSystem");
			}
			this.transitionSystem = transitionSystem;
		}

		public ReferGraph<State, Transition> BuildGraph()
		{
			if (graph != null)
			{
				return graph;
			}
			graph = new ReferGraph<State, Transition>();
			AddNode();
			AddEdge();
			return graph;
		}

		private void AddNode()
		{
			State[] states = transitionSystem.States;
			foreach (State state in states)
			{
				Node<State> node = new Node<State>(state, state.Flags.ToNodeKind());
				graph.AddNode(node, transitionSystem.InitialStates.Contains(state.Label));
				labelToNodeDict[state.Label] = node;
			}
			State[] states2 = transitionSystem.States;
			foreach (State state2 in states2)
			{
				if (!string.IsNullOrEmpty(state2.RepresentativeState) && state2.RelationKind == StateRelationKind.Equivalent)
				{
					labelToNodeDict[state2.Label] = labelToNodeDict[state2.RepresentativeState];
				}
			}
		}

		private void AddEdge()
		{
			Transition[] transitions = transitionSystem.Transitions;
			foreach (Transition transition in transitions)
			{
				Node<State> source = labelToNodeDict[transition.Source];
				Node<State> target = labelToNodeDict[transition.Target];
				List<string> list = new List<string>();
				list.AddRange(transition.CapturedRequirements);
				list.AddRange(transition.AssumeCapturedRequirements);
				graph.AddEdge(new Edge<State, Transition>(source, target, transition, transition.Action.IsObservable(), list));
			}
		}
	}
}
