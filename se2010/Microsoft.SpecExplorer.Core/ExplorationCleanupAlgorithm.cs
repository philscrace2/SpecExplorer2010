using System.Collections.Generic;
using System.Linq;
using Microsoft.ActionMachines;
using Microsoft.GraphTraversal;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	internal class ExplorationCleanupAlgorithm : IAlgorithm<MachineState, ExplorationStep>
	{
		private class Cousin
		{
			private int? hashCode = null;

			internal Node<MachineState> Key { get; private set; }

			internal Node<MachineState> Value { get; private set; }

			internal Cousin(Node<MachineState> key, Node<MachineState> value)
			{
				Key = key;
				Value = value;
			}

			public override bool Equals(object obj)
			{
				if (obj is Cousin)
				{
					Cousin cousin = obj as Cousin;
					if (Key == cousin.Key && Value == cousin.Value)
					{
						return true;
					}
					if (Key == cousin.Value && Value == cousin.Key)
					{
						return true;
					}
					return false;
				}
				return false;
			}

			public override int GetHashCode()
			{
				if (!hashCode.HasValue)
				{
					hashCode = Key.GetHashCode() + Value.GetHashCode();
				}
				return hashCode.Value;
			}
		}

		private Dictionary<ICompressedState, HashSet<Node<MachineState>>> equivalentDataStates;

		private HashSet<Cousin> candidateCousins;

		private IGraph<MachineState, ExplorationStep> visitGraph;

		private object abortLock;

		private EventAdapter eventAdapter;

		public Dictionary<Node<MachineState>, Node<MachineState>> Cousins { get; private set; }

		public ExplorationCleanupAlgorithm(IGraph<MachineState, ExplorationStep> visitGraph, object abortLock, EventAdapter eventAdapter)
		{
			this.visitGraph = visitGraph;
			this.abortLock = abortLock;
			this.eventAdapter = eventAdapter;
		}

		public void Run()
		{
			PrepareCandidateCousins();
			while (candidateCousins.Count > 0)
			{
				Cousin cousin = candidateCousins.First();
				candidateCousins.Remove(cousin);
				if (!Cousins.ContainsKey(cousin.Key) && !Cousins.ContainsKey(cousin.Value) && IsMergeAble(cousin.Key, cousin.Value, true))
				{
					Merge(cousin.Key, cousin.Value, true);
				}
			}
			foreach (Node<MachineState> node in visitGraph.Nodes)
			{
				List<Edge<MachineState, ExplorationStep>> list = new List<Edge<MachineState, ExplorationStep>>();
				List<Edge<MachineState, ExplorationStep>> outEdges;
				if (visitGraph.TryGetOutGoingEdges(node, out outEdges))
				{
					Dictionary<Action, HashSet<Node<MachineState>>> dictionary = new Dictionary<Action, HashSet<Node<MachineState>>>();
					foreach (Edge<MachineState, ExplorationStep> item in outEdges)
					{
						HashSet<Node<MachineState>> value;
						if (!dictionary.TryGetValue(item.Label.Action, out value))
						{
							value = new HashSet<Node<MachineState>>();
							dictionary[item.Label.Action] = value;
						}
						if (!value.Add(item.Target))
						{
							list.Add(item);
						}
					}
				}
				RemoveDuplicateEdges(list);
			}
			eventAdapter.ShowStatistics(new ExplorationStatistics(ExplorationStatus.Cleanup, 0, visitGraph.Nodes.Count(), visitGraph.Edges.Count(), 0, 0, 0, true));
		}

		public void Initialize()
		{
			equivalentDataStates = new Dictionary<ICompressedState, HashSet<Node<MachineState>>>();
			candidateCousins = new HashSet<Cousin>();
			Cousins = new Dictionary<Node<MachineState>, Node<MachineState>>();
		}

		private void PrepareCandidateCousins()
		{
			foreach (Node<MachineState> node in visitGraph.Nodes)
			{
				HashSet<Node<MachineState>> value = null;
				if (!equivalentDataStates.TryGetValue(node.Label.Data, out value))
				{
					value = new HashSet<Node<MachineState>>();
					equivalentDataStates[node.Label.Data] = value;
				}
				else
				{
					foreach (Node<MachineState> item in value)
					{
						candidateCousins.Add(new Cousin(node, item));
					}
				}
				value.Add(node);
			}
		}

		private bool IsMergeAble(Node<MachineState> node1, Node<MachineState> node2, bool isForFuture)
		{
			if (node1.Label.Control.Kind != node2.Label.Control.Kind)
			{
				return false;
			}
			Dictionary<Action, HashSet<Node<MachineState>>> dictionary = new Dictionary<Action, HashSet<Node<MachineState>>>();
			List<Edge<MachineState, ExplorationStep>> outEdges = null;
			List<Edge<MachineState, ExplorationStep>> list = new List<Edge<MachineState, ExplorationStep>>();
			if (isForFuture ? visitGraph.TryGetOutGoingEdges(node1, out outEdges) : visitGraph.TryGetInComingEdges(node1, out outEdges))
			{
				foreach (Edge<MachineState, ExplorationStep> item in outEdges)
				{
					HashSet<Node<MachineState>> value;
					if (!dictionary.TryGetValue(item.Label.Action, out value))
					{
						value = new HashSet<Node<MachineState>>();
						dictionary[item.Label.Action] = value;
					}
					if (!value.Add(isForFuture ? item.Target : item.Source))
					{
						list.Add(item);
					}
				}
				RemoveDuplicateEdges(list);
				list.Clear();
			}
			if (isForFuture ? visitGraph.TryGetOutGoingEdges(node2, out outEdges) : visitGraph.TryGetInComingEdges(node2, out outEdges))
			{
				Dictionary<Action, HashSet<Node<MachineState>>> dictionary2 = new Dictionary<Action, HashSet<Node<MachineState>>>();
				foreach (Edge<MachineState, ExplorationStep> item2 in outEdges)
				{
					HashSet<Node<MachineState>> value2;
					if (!dictionary2.TryGetValue(item2.Label.Action, out value2))
					{
						value2 = new HashSet<Node<MachineState>>();
						dictionary2[item2.Label.Action] = value2;
					}
					if (!value2.Add(isForFuture ? item2.Target : item2.Source))
					{
						list.Add(item2);
						continue;
					}
					HashSet<Node<MachineState>> value3;
					if (dictionary.TryGetValue(item2.Label.Action, out value3))
					{
						if (value3.Remove(isForFuture ? item2.Target : item2.Source))
						{
							if (value3.Count == 0)
							{
								dictionary.Remove(item2.Label.Action);
							}
							continue;
						}
						RemoveDuplicateEdges(list);
						return false;
					}
					RemoveDuplicateEdges(list);
					return false;
				}
				RemoveDuplicateEdges(list);
			}
			if (dictionary.Count == 0)
			{
				return true;
			}
			return false;
		}

		private void Merge(Node<MachineState> node1, Node<MachineState> node2, bool isForFuture)
		{
			List<Edge<MachineState, ExplorationStep>> outEdges = null;
			foreach (Node<MachineState> item in equivalentDataStates[node1.Label.Data])
			{
				if (item != node1 && item != node2)
				{
					candidateCousins.Add(new Cousin(item, node1));
				}
			}
			List<Edge<MachineState, ExplorationStep>> outEdges2;
			if (isForFuture ? visitGraph.TryGetInComingEdges(node1, out outEdges2) : visitGraph.TryGetOutGoingEdges(node1, out outEdges2))
			{
				foreach (Edge<MachineState, ExplorationStep> item2 in outEdges2)
				{
					HashSet<Node<MachineState>> hashSet = equivalentDataStates[isForFuture ? item2.Source.Label.Data : item2.Target.Label.Data];
					if (!(isForFuture ? visitGraph.TryGetInComingEdges(node2, out outEdges) : visitGraph.TryGetOutGoingEdges(node2, out outEdges)))
					{
						continue;
					}
					foreach (Edge<MachineState, ExplorationStep> item3 in outEdges)
					{
						Node<MachineState> node3 = (isForFuture ? item2.Source : item2.Target);
						Node<MachineState> node4 = (isForFuture ? item3.Source : item3.Target);
						if (node3 != node4 && hashSet.Contains(item3.Source))
						{
							candidateCousins.Add(new Cousin(node3, node4));
						}
					}
				}
			}
			lock (abortLock)
			{
				if (isForFuture ? visitGraph.TryGetInComingEdges(node2, out outEdges) : visitGraph.TryGetOutGoingEdges(node2, out outEdges))
				{
					List<Edge<MachineState, ExplorationStep>> list = new List<Edge<MachineState, ExplorationStep>>(outEdges);
					foreach (Edge<MachineState, ExplorationStep> item4 in list)
					{
						if (isForFuture)
						{
							if (item4.Target != node1)
							{
								visitGraph.RelinkEdge(item4, item4.Source, false, node1);
							}
						}
						else if (item4.Source != node1)
						{
							visitGraph.RelinkEdge(item4, node1, visitGraph.IsStart(node2), item4.Target);
						}
					}
				}
				Cousins[node2] = node1;
				if (visitGraph.IsStart(node2))
				{
					visitGraph.AddNode(node1, true);
				}
				visitGraph.DeleteNode(node2);
				equivalentDataStates[node2.Label.Data].Remove(node2);
			}
			eventAdapter.ShowStatistics(new ExplorationStatistics(ExplorationStatus.Cleanup, 0, visitGraph.Nodes.Count(), visitGraph.Edges.Count(), 0, 0, 0, false));
		}

		private void RemoveDuplicateEdges(List<Edge<MachineState, ExplorationStep>> edges)
		{
			lock (abortLock)
			{
				foreach (Edge<MachineState, ExplorationStep> edge in edges)
				{
					visitGraph.DeleteEdge(edge);
				}
			}
		}
	}
}
