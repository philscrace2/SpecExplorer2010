using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer.Viewer
{
	public class ProbeQuery : IViewQuery
	{
		private string probeName;

		private string resolvedProbeName;

		public ProbeQuery(string probeName)
		{
			this.probeName = probeName;
		}

		public string GetLabel(State state)
		{
			Probe[] probes = state.Probes;
			foreach (Probe probe in probes)
			{
				if (!resolvedProbeName.Equals(probe.Name))
				{
					continue;
				}
				string value = probe.Value;
				if (probe.Kind == ProbeValueKind.Normal)
				{
					if (string.Compare(probe.Type.FullName, "System.String", true) == 0 || (string.Compare(probe.Type.FullName, "System.Char", true) == 0 && value.StartsWith("'") && value.EndsWith("'")))
					{
						return value.Substring(1, value.Length - 2);
					}
					return value;
				}
				return null;
			}
			throw new QueryException(string.Format("Can not find the probe: '{0}'.", resolvedProbeName));
		}

		public IEnumerable<DisplayNode> GetHyperNodes(ICollection<DisplayNode> nodes)
		{
			Dictionary<string, DisplayNode> dictionary = new Dictionary<string, DisplayNode>();
			DisplayNode displayNode = new DisplayNode(DisplayNodeKind.Hyper, new State(), "<<Error>>", false, StateFlags.None, Microsoft.GraphTraversal.NodeKind.Regular);
			DisplayNode displayNode2 = new DisplayNode(DisplayNodeKind.Hyper, new State(), "<<Exception>>", false, StateFlags.None, Microsoft.GraphTraversal.NodeKind.Regular);
			foreach (DisplayNode node in nodes)
			{
				if ((node.Label.Flags & StateFlags.Error) != 0)
				{
					displayNode.AddSubNode(node);
					continue;
				}
				string label = GetLabel(node.Label);
				if (label == null)
				{
					displayNode2.AddSubNode(node);
					continue;
				}
				DisplayNode value;
				if (!dictionary.TryGetValue(label, out value))
				{
					value = (dictionary[label] = new DisplayNode(DisplayNodeKind.Hyper, new State(), label, false, StateFlags.None, Microsoft.GraphTraversal.NodeKind.Regular));
					value.Text = label;
				}
				value.AddSubNode(node);
				node.ParentNode = value;
			}
			List<DisplayNode> list = new List<DisplayNode>(dictionary.Values);
			if (displayNode.SubNodes.Count > 0)
			{
				list.Add(displayNode);
			}
			if (displayNode2.SubNodes.Count > 0)
			{
				list.Add(displayNode2);
			}
			return list;
		}

		public void DivideHyperNodes(DisplayNode parentNode)
		{
			if (parentNode.DisplayNodeKind != DisplayNodeKind.Hyper)
			{
				return;
			}
			IEnumerable<DisplayNode> hyperNodes = GetHyperNodes(parentNode.SubNodes);
			foreach (DisplayNode item in hyperNodes)
			{
				item.ParentNode = parentNode;
			}
		}

		public bool ValidateViewQuery(TransitionSystem transitionSystem, out string errorMessage)
		{
			errorMessage = "";
			if (transitionSystem.States.Count() > 0)
			{
				State state = transitionSystem.States[0];
				List<string> list = new List<string>();
				Probe[] probes = state.Probes;
				foreach (Probe probe in probes)
				{
					if (probe.Name.EndsWith(probeName))
					{
						list.Add(probe.Name);
					}
				}
				if (list.Count > 1)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat("The probe '{0}' is ambiguous for ", probeName);
					foreach (string item in list)
					{
						stringBuilder.AppendFormat("'{0}',", item);
					}
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
					stringBuilder.Append(". Please input full probe name.");
					errorMessage = stringBuilder.ToString();
					return false;
				}
				if (list.Count == 0)
				{
					errorMessage = string.Format("Can not find the probe: '{0}'.", probeName);
					return false;
				}
				resolvedProbeName = list[0];
				return true;
			}
			return true;
		}
	}
}
