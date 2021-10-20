using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer.Viewer
{
	internal class DisplayGraph : Graph<State, Transition>
	{
		internal const string ExceptionNodeLabel = "<<Exception>>";

		internal const string ErrorNodeLabel = "<<Error>>";

		private Dictionary<string, DisplayNode> nodeIdDict;

		private Dictionary<string, DisplayEdge> edgeIdDict;

		private Dictionary<DisplayNode, State> equivalentStateDict;

		private int nodeId;

		private int edgeId;

		internal Color NodeFillColor { get; set; }

		internal Color EdgeColor { get; set; }

		internal DisplayGraph()
		{
			nodeIdDict = new Dictionary<string, DisplayNode>();
			edgeIdDict = new Dictionary<string, DisplayEdge>();
			equivalentStateDict = new Dictionary<DisplayNode, State>();
		}

		internal void CollapseNode(DisplayNode parentNode)
		{
			if (ContainsNode(parentNode))
			{
				throw new InvalidOperationException("Can not collapse node to existing node");
			}
			if (parentNode.DisplayNodeKind != DisplayNodeKind.Hyper)
			{
				throw new InvalidOperationException("Can not collapse non-Hyper node");
			}
			AddNode(parentNode, parentNode.IsStart);
			HashSet<Node<State>> hashSet = new HashSet<Node<State>>();
			HashSet<Node<State>> hashSet2 = new HashSet<Node<State>>();
			hashSet2.Add(parentNode);
			foreach (DisplayNode subNode in parentNode.SubNodes)
			{
				if (!ContainsNode(subNode))
				{
					throw new InvalidOperationException("Can not collapse non-existing node");
				}
				List<Edge<State, Transition>> edges;
				if (TryGetInComingEdges(subNode, out edges))
				{
					Edge<State, Transition>[] array = edges.ToArray();
					foreach (Edge<State, Transition> edge in array)
					{
						DeleteEdge(edge);
						DisplayEdge displayEdge = new DisplayEdge(edge.Source as DisplayNode, parentNode, edge as DisplayEdge);
						if (displayEdge.displayEdgeKind != DisplayEdgeKind.Hidden || hashSet.Add(edge.Source))
						{
							AddEdge(displayEdge);
						}
					}
				}
				List<Edge<State, Transition>> outEdges;
				if (TryGetOutGoingEdges(subNode, out outEdges))
				{
					Edge<State, Transition>[] array2 = outEdges.ToArray();
					foreach (Edge<State, Transition> edge2 in array2)
					{
						DeleteEdge(edge2);
						DisplayEdge displayEdge2 = new DisplayEdge(parentNode, edge2.Target as DisplayNode, edge2 as DisplayEdge);
						if (displayEdge2.displayEdgeKind != DisplayEdgeKind.Hidden || hashSet2.Add(edge2.Target))
						{
							AddEdge(displayEdge2);
						}
					}
				}
				DeleteNode(subNode);
			}
			if (parentNode.SubNodes.Count == 0)
			{
				DeleteNode(parentNode);
			}
		}

		internal DisplayNode GetNodeById(string id)
		{
			DisplayNode value;
			if (!nodeIdDict.TryGetValue(id, out value))
			{
				throw new InvalidOperationException(string.Format("The display graph does not contains node: '{0}'", id));
			}
			return value;
		}

		internal DisplayEdge GetEdgeById(string id)
		{
			DisplayEdge value;
			if (!edgeIdDict.TryGetValue(id, out value))
			{
				throw new InvalidOperationException(string.Format("The display graph does not contains node: '{0}'", id));
			}
			return value;
		}

		internal void AddNode(DisplayNode node, bool isStart)
		{
			base.AddNode(node, isStart);
			if (node.Id == null)
			{
				node.Id = nodeId.ToString();
				nodeId++;
			}
			nodeIdDict[node.Id] = node;
		}

		internal void AddEdge(DisplayEdge edge)
		{
			AddEdge((Edge<State, Transition>)edge);
			if (edge.Id == null)
			{
				edge.Id = edgeId.ToString();
				edgeId++;
			}
			edgeIdDict[edge.Id] = edge;
		}

		internal void AddEquivalentState(DisplayNode displayNode, State state)
		{
			equivalentStateDict[displayNode] = state;
		}

		internal State GetEquivalentState(DisplayNode displayNode)
		{
			State value;
			if (equivalentStateDict.TryGetValue(displayNode, out value))
			{
				return value;
			}
			return null;
		}
	}
}
