using System.Collections.Generic;
using System.Linq;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer.Viewer
{
	internal class DisplayGraphBuilder
	{
		private DisplayGraph displayGraph;

		private IViewQuery selectQuery;

		private HashSet<string> selectLabel;

		private List<IViewQuery> groupQueries;

		private IViewQuery descriptionQuery;

		private IViewQuery hideQuery;

		private Dictionary<string, DisplayNode> labelToNodeDict;

		private Dictionary<string, DisplayNode> equivalentDict;

		private List<DisplayNode> topNodes;

		private bool isDisplayRequirements;

		internal TransitionSystem TransitionSystem { get; set; }

		internal DisplayGraphBuilder(TransitionSystem transitionSystem)
		{
			TransitionSystem = transitionSystem;
			selectLabel = new HashSet<string>();
			groupQueries = new List<IViewQuery>();
			labelToNodeDict = new Dictionary<string, DisplayNode>();
			equivalentDict = new Dictionary<string, DisplayNode>();
			topNodes = new List<DisplayNode>();
		}

		private void Reset()
		{
			selectQuery = null;
			selectLabel.Clear();
			groupQueries.Clear();
			labelToNodeDict.Clear();
			equivalentDict.Clear();
			topNodes.Clear();
		}

		private void SetViewDefinition(ViewDefinition viewDefinition)
		{
			selectQuery = QueryFactory.GetViewQuery(viewDefinition.SelectQuery.Query);
			selectLabel.Clear();
			selectLabel.Add(true.ToString());
			groupQueries.Clear();
			Query[] groupQuery = viewDefinition.GroupQuery;
			foreach (Query query in groupQuery)
			{
				if (!string.IsNullOrEmpty(query.Param))
				{
					groupQueries.Add(QueryFactory.GetViewQuery(query));
				}
			}
			descriptionQuery = QueryFactory.GetViewQuery(viewDefinition.StateDescription);
			hideQuery = QueryFactory.GetViewQuery(viewDefinition.HideQuery.Query);
			isDisplayRequirements = viewDefinition.DisplayRequirements;
		}

		private void ValidateViewDefinition()
		{
			string errorMessage = "";
			if (selectQuery != null && !selectQuery.ValidateViewQuery(TransitionSystem, out errorMessage))
			{
				throw new QueryException(errorMessage);
			}
			foreach (IViewQuery groupQuery in groupQueries)
			{
				if (!groupQuery.ValidateViewQuery(TransitionSystem, out errorMessage))
				{
					throw new QueryException(errorMessage);
				}
			}
			if (descriptionQuery != null && !descriptionQuery.ValidateViewQuery(TransitionSystem, out errorMessage))
			{
				throw new QueryException(errorMessage);
			}
			if (hideQuery != null && !hideQuery.ValidateViewQuery(TransitionSystem, out errorMessage))
			{
				throw new QueryException(errorMessage);
			}
		}

		internal DisplayGraph BuildDisplayGraph(ViewDefinition viewDefinition)
		{
			Reset();
			displayGraph = new DisplayGraph();
			displayGraph.NodeFillColor = viewDefinition.NodeFillColor;
			displayGraph.EdgeColor = viewDefinition.EdgeColor;
			SetViewDefinition(viewDefinition);
			ValidateViewDefinition();
			BuildAndAddNodes();
			BuildAndAddEdges(viewDefinition.ShowParameters);
			if (viewDefinition.ShowErrorPathsOnly)
			{
				ShowErrorPathsOnly();
			}
			if (viewDefinition.ViewCollapseSteps)
			{
				CollapseSteps();
			}
			if (hideQuery != null)
			{
				ProcessHiddenQuery();
			}
			if (groupQueries.Count > 0 && groupQueries[0] != null)
			{
				ProcessGroupQuery();
			}
			if (viewDefinition.ViewCollapseLabels)
			{
				CollapseLabels();
			}
			return displayGraph;
		}

		private void ProcessRepresentativeStates()
		{
			State[] states = TransitionSystem.States;
			foreach (State state in states)
			{
				if (!string.IsNullOrEmpty(state.RepresentativeState))
				{
					if (state.RelationKind == StateRelationKind.Equivalent)
					{
						DisplayNode displayNode = labelToNodeDict[state.RepresentativeState];
						equivalentDict[state.Label] = displayNode;
						displayGraph.AddEquivalentState(displayNode, state);
					}
					else if (state.RelationKind == StateRelationKind.Subsumed)
					{
						displayGraph.AddEdge(new DisplayEdge(labelToNodeDict[state.Label], labelToNodeDict[state.RepresentativeState], DisplayEdgeKind.Subsume));
					}
				}
			}
		}

		private void BuildAndAddNodes()
		{
			State[] states = TransitionSystem.States;
			foreach (State state in states)
			{
				DisplayNode displayNode = new DisplayNode(DisplayNodeKind.Normal, state, state.Label, TransitionSystem.InitialStates.Contains(state.Label), state.Flags, state.Flags.ToNodeKind());
				labelToNodeDict[state.Label] = displayNode;
				if (descriptionQuery != null)
				{
					string label = descriptionQuery.GetLabel(displayNode.Label);
					displayNode.Text = ((label != null) ? label : "<<Exception>>");
				}
				if (string.IsNullOrEmpty(state.RepresentativeState) || state.RelationKind != StateRelationKind.Equivalent)
				{
					displayGraph.AddNode(displayNode, displayNode.IsStart);
					topNodes.Add(displayNode);
				}
			}
			ProcessRepresentativeStates();
		}

		private void BuildAndAddEdges(bool showParameters)
		{
			Transition[] transitions = TransitionSystem.Transitions;
			foreach (Transition transition in transitions)
			{
				DisplayNode value;
				if (!labelToNodeDict.TryGetValue(transition.Source, out value))
				{
					continue;
				}
				DisplayNode value2;
				if (equivalentDict.TryGetValue(transition.Target, out value2))
				{
					if (!labelToNodeDict.TryGetValue(value2.Label.Label, out value2))
					{
						continue;
					}
				}
				else if (!labelToNodeDict.TryGetValue(transition.Target, out value2))
				{
					continue;
				}
				DisplayEdge displayEdge = new DisplayEdge(value, value2, transition, isDisplayRequirements);
				if (!showParameters)
				{
					switch (displayEdge.Kind)
					{
					case ActionSymbolKind.Call:
					case ActionSymbolKind.Event:
						displayEdge.Text = displayEdge.Text.Remove(displayEdge.Text.IndexOf("("));
						break;
					case ActionSymbolKind.Return:
						if (displayEdge.Text.Contains('/'))
						{
							displayEdge.Text = displayEdge.Text.Remove(displayEdge.Text.IndexOf("/"));
						}
						break;
					}
				}
				displayGraph.AddEdge(displayEdge);
			}
		}

		private void ShowErrorPathsOnly()
		{
			HashSet<Node<State>> hashSet = new HashSet<Node<State>>();
			foreach (DisplayNode node2 in displayGraph.Nodes)
			{
				if ((node2.StateFlags & StateFlags.Error) != 0)
				{
					hashSet.Add(node2);
				}
			}
			HashSet<Edge<State, Transition>> hashSet2 = new HashSet<Edge<State, Transition>>();
			foreach (DisplayNode startNode2 in displayGraph.StartNodes)
			{
				DisplayNode startNode = startNode2;
				if (hashSet.Count == 0)
				{
					break;
				}
				ShortestPathAlgorithm<State, Transition> shortestPathAlgorithm = new ShortestPathAlgorithm<State, Transition>(displayGraph, startNode, hashSet);
				shortestPathAlgorithm.Run();
				Dictionary<Node<State>, Path<State, Transition>> resultDict = shortestPathAlgorithm.ResultDict;
				foreach (Node<State> key in resultDict.Keys)
				{
					foreach (Edge<State, Transition> edge2 in resultDict[key].Edges)
					{
						hashSet2.Add(edge2);
					}
					hashSet.Remove(key);
				}
			}
			Edge<State, Transition>[] array = displayGraph.Edges.ToArray();
			foreach (Edge<State, Transition> edge in array)
			{
				if (!hashSet2.Contains(edge))
				{
					displayGraph.DeleteEdge(edge);
				}
			}
			Node<State>[] array2 = displayGraph.Nodes.ToArray();
			foreach (Node<State> node in array2)
			{
				if (displayGraph.IncomingCount(node) == 0 && displayGraph.OutgoingCount(node) == 0)
				{
					displayGraph.DeleteNode(node);
				}
			}
		}

		private void ProcessHiddenQuery()
		{
			Dictionary<Node<State>, HashSet<Node<State>>> dictionary = new Dictionary<Node<State>, HashSet<Node<State>>>();
			Node<State>[] array = displayGraph.Nodes.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				DisplayNode displayNode = (DisplayNode)array[i];
				if ((displayNode.StateFlags & StateFlags.Error) != 0 || string.Compare("true", hideQuery.GetLabel(displayNode.Label), true) != 0)
				{
					continue;
				}
				List<Edge<State, Transition>> edges;
				if (!displayGraph.TryGetInComingEdges(displayNode, out edges))
				{
					edges = new List<Edge<State, Transition>>();
				}
				List<Edge<State, Transition>> outEdges;
				if (!displayGraph.TryGetOutGoingEdges(displayNode, out outEdges))
				{
					outEdges = new List<Edge<State, Transition>>();
				}
				foreach (Edge<State, Transition> item in edges)
				{
					if (item.Source == item.Target)
					{
						continue;
					}
					Node<State> source = item.Source;
					HashSet<Node<State>> value;
					if (!dictionary.TryGetValue(source, out value))
					{
						value = (dictionary[source] = new HashSet<Node<State>>());
					}
					foreach (Edge<State, Transition> item2 in outEdges)
					{
						if (item2.Source != item2.Target)
						{
							Node<State> target = item2.Target;
							if (value.Add(target))
							{
								DisplayEdge edge = new DisplayEdge(source as DisplayNode, target as DisplayNode, DisplayEdgeKind.Hidden);
								displayGraph.AddEdge(edge);
							}
						}
					}
				}
				displayGraph.DeleteNode(displayNode);
				topNodes.Remove(displayNode);
			}
		}

		private void CollapseSteps()
		{
			Node<State>[] array = displayGraph.ChoiceNodes.ToArray();
			foreach (Node<State> node in array)
			{
				List<Edge<State, Transition>> outEdges;
				if ((node.Label.Flags & StateFlags.BoundStopped) != 0 || !displayGraph.TryGetOutGoingEdges(node, out outEdges) || outEdges.Count != 1)
				{
					continue;
				}
				DisplayEdge displayEdge = outEdges[0] as DisplayEdge;
				List<Edge<State, Transition>> edges;
				if (displayEdge.Kind != ActionSymbolKind.Return || displayEdge.Source == displayEdge.Target || !displayGraph.TryGetInComingEdges(node, out edges))
				{
					continue;
				}
				bool flag = true;
				List<DisplayEdge> list = new List<DisplayEdge>();
				foreach (Edge<State, Transition> item in edges)
				{
					DisplayEdge displayEdge2 = item as DisplayEdge;
					if (displayEdge2.Kind != ActionSymbolKind.Call)
					{
						flag = false;
						break;
					}
					list.Add(new DisplayEdge(displayEdge2, displayEdge, isDisplayRequirements));
					DisplayNode displayNode = displayEdge.Target as DisplayNode;
					displayNode.CollapsedNode = node as DisplayNode;
				}
				if (!flag)
				{
					continue;
				}
				foreach (DisplayEdge item2 in list)
				{
					displayGraph.AddEdge(item2);
				}
				displayGraph.DeleteNode(node);
				topNodes.Remove(node as DisplayNode);
			}
		}

		private void CollapseLabels()
		{
			Dictionary<Node<State>, List<Edge<State, Transition>>> dictionary = new Dictionary<Node<State>, List<Edge<State, Transition>>>();
			Node<State>[] array = displayGraph.Nodes.ToArray();
			foreach (Node<State> node in array)
			{
				dictionary.Clear();
				List<Edge<State, Transition>> outEdges;
				if (!displayGraph.TryGetOutGoingEdges(node, out outEdges))
				{
					continue;
				}
				foreach (Edge<State, Transition> item in outEdges)
				{
					List<Edge<State, Transition>> value;
					if (!dictionary.TryGetValue(item.Target, out value))
					{
						value = new List<Edge<State, Transition>>();
						dictionary[item.Target] = value;
					}
					value.Add(item);
				}
				foreach (Node<State> key in dictionary.Keys)
				{
					List<Edge<State, Transition>> list = dictionary[key];
					if (list.Count <= 1)
					{
						continue;
					}
					DisplayEdge displayEdge = new DisplayEdge(node as DisplayNode, key as DisplayNode, DisplayEdgeKind.Hyper);
					foreach (Edge<State, Transition> item2 in list)
					{
						displayGraph.DeleteEdge(item2);
						displayEdge.AddSubEdge(item2 as DisplayEdge);
					}
					displayGraph.AddEdge(displayEdge);
				}
			}
		}

		private void ProcessSelectQuery()
		{
			if (selectQuery == null)
			{
				return;
			}
			IEnumerable<DisplayNode> hyperNodes = selectQuery.GetHyperNodes(topNodes);
			topNodes.Clear();
			foreach (DisplayNode item in hyperNodes)
			{
				if (!selectLabel.Contains(item.Label.Label))
				{
					continue;
				}
				foreach (DisplayNode subNode in item.SubNodes)
				{
					subNode.ResetParent();
					topNodes.Add(subNode);
					labelToNodeDict[subNode.Label.Label] = subNode;
				}
			}
		}

		private void ProcessGroupQuery()
		{
			List<DisplayNode> list = new List<DisplayNode>();
			list.AddRange(groupQueries[0].GetHyperNodes(topNodes));
			foreach (DisplayNode item in list)
			{
				displayGraph.CollapseNode(item);
			}
			List<DisplayNode> list2 = new List<DisplayNode>();
			bool flag = true;
			foreach (IViewQuery groupQuery in groupQueries)
			{
				if (flag)
				{
					flag = false;
					continue;
				}
				foreach (DisplayNode item2 in list)
				{
					groupQuery.DivideHyperNodes(item2);
					list2.AddRange(item2.SubNodes);
				}
				List<DisplayNode> list3 = list;
				list = list2;
				list2 = list3;
				list2.Clear();
			}
		}
	}
}
