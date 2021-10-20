using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	public class VariableResolver
	{
		private TestCaseHashBuilder testCaseHashBuilder;

		private HashSet<string> initialStateSet = new HashSet<string>();

		private Dictionary<string, List<string>> methodRequirementsMap = new Dictionary<string, List<string>>();

		private Dictionary<string, string> methodHashCodeMap = new Dictionary<string, string>();

		private Dictionary<string, State> methodEndStateMap = new Dictionary<string, State>();

		private string transitionSystemName;

		private TransitionSystem transitionSystem;

		private IGraph<State, Transition> graph;

		internal VariableResolver(TransitionSystem transitionSystem)
		{
			this.transitionSystem = transitionSystem;
			transitionSystemName = transitionSystem.Name;
			graph = new TransitionSystemGraphBuilder(transitionSystem).BuildGraph();
			testCaseHashBuilder = new TestCaseHashBuilder(transitionSystem);
		}

		internal string Resolve(string switchName)
		{
			return Resolve(switchName, string.Empty);
		}

		internal string Resolve(string switchName, string initialState)
		{
			string @switch = transitionSystem.GetSwitch(switchName);
			IEnumerable<SubstitutionPattern> enumerable = null;
			try
			{
				enumerable = SubstitutionPattern.GetPatterns(@switch, '$');
			}
			catch (SubstitutionPatternException ex)
			{
				throw new TestCodeGenerationException(ex.Message);
			}
			string text = @switch;
			foreach (SubstitutionPattern item in enumerable)
			{
				text = text.Remove(item.StartIndex, item.EndIndex - item.StartIndex + 1);
				text = (string.IsNullOrEmpty(initialState) ? text.Insert(item.StartIndex, GetNonMethodLevelVariable(item.VariableText, switchName)) : text.Insert(item.StartIndex, GetMethodLevelVariable(item.VariableText, switchName, initialState)));
			}
			return text.Replace("$$", "$");
		}

		internal string GetNonMethodLevelVariable(string variable, string switchName)
		{
			switch (variable.ToLower())
			{
			case "testcoveredrequirementset":
			case "testcoveredrequirementsequence":
			case "testcasehashcode":
				throw new TestCodeGenerationException(string.Format("Invalid value for switch {0}: cannot take Spec Explorer built-in variable ‘{1}'", switchName, variable));
			case "machinename":
				return transitionSystemName;
			default:
				if (variable.ToLower().StartsWith("endstateprobe("))
				{
					throw new TestCodeGenerationException(string.Format("Invalid value for switch {0}: cannot take Spec Explorer built-in variable '{1}'", switchName, variable));
				}
				throw new TestCodeGenerationException(string.Format("'{0}' is not a valid Spec Explorer built-in variable.", variable));
			}
		}

		internal string GetMethodLevelVariable(string variable, string switchName, string initialState)
		{
			InitializeInitialState(initialState);
			switch (variable.ToLower())
			{
			case "testcoveredrequirementset":
			{
				HashSet<string> strings = new HashSet<string>(GetMethodRequirements(initialState));
				return MakeString(strings);
			}
			case "testcoveredrequirementsequence":
				return MakeString(GetMethodRequirements(initialState));
			case "testcasehashcode":
				return GetMethodHashCode(initialState);
			case "machinename":
				return transitionSystemName;
			default:
				if (variable.ToLower().StartsWith("endstateprobe(") && variable.EndsWith(")"))
				{
					State methodEndState = GetMethodEndState(initialState);
					string text = variable.Substring("endstateprobe(".Length, variable.Length - "endstateprobe(".Length - 1);
					Probe[] outProbes;
					if (methodEndState.TryGetProbesByName(text, out outProbes))
					{
						if (outProbes.Count() == 1)
						{
							string value = outProbes[0].Value;
							if (outProbes[0].Kind == ProbeValueKind.Normal)
							{
								if (string.Compare(outProbes[0].Type.FullName, "System.String", true) == 0 || (string.Compare(outProbes[0].Type.FullName, "System.Char", true) == 0 && value.Length >= 2 && value.StartsWith("'") && value.EndsWith("'")))
								{
									return value.Substring(1, value.Length - 2);
								}
								return value;
							}
							throw new TestCodeGenerationException(string.Format("Probe '{0}' in state '{1}' is not available.", text, methodEndState.Label));
						}
						throw new TestCodeGenerationException(string.Format("Probe '{0}' in switch '{1}' is ambiguous. Please use full name instead.", text, switchName));
					}
					throw new TestCodeGenerationException(string.Format("Probe '{0}' in switch '{1}' does not exist.", text, switchName));
				}
				throw new TestCodeGenerationException(string.Format("'{0}' in switch '{1}' is not a valid Spec Explorer built-in variables", variable, switchName));
			}
		}

		internal string GetMethodHashCode(string initialState)
		{
			InitializeInitialState(initialState);
			return methodHashCodeMap[initialState];
		}

		internal IEnumerable<string> GetMethodRequirements(string initialState)
		{
			InitializeInitialState(initialState);
			return methodRequirementsMap[initialState];
		}

		internal State GetMethodEndState(string initialState)
		{
			InitializeInitialState(initialState);
			return methodEndStateMap[initialState];
		}

		private void InitializeInitialState(string initialStateLabel)
		{
			Node<State> startNode;
			if (!graph.GetInitialNodeByLabel(initialStateLabel, out startNode))
			{
				throw new InvalidOperationException("No matched initial node.");
			}
			if (!initialStateSet.Add(initialStateLabel))
			{
				return;
			}
			methodHashCodeMap[initialStateLabel] = testCaseHashBuilder.GetHashCode(initialStateLabel);
			DepthFirstSearchAlgorithm<State, Transition> depthFirstSearchAlgorithm = new DepthFirstSearchAlgorithm<State, Transition>(graph);
			List<string> requirements = (methodRequirementsMap[initialStateLabel] = new List<string>());
			depthFirstSearchAlgorithm.VisitEdge += delegate(object sender, EdgeEventArgs<State, Transition> e)
			{
				requirements.AddRange(e.Edge.Requirements);
			};
			depthFirstSearchAlgorithm.BackEdge += delegate(object sender, EdgeEventArgs<State, Transition> e)
			{
				if (!methodEndStateMap.ContainsKey(initialStateLabel))
				{
					methodEndStateMap[initialStateLabel] = e.Edge.Target.Label;
				}
			};
			depthFirstSearchAlgorithm.TreeEdge += delegate(object sender, EdgeEventArgs<State, Transition> e)
			{
				methodEndStateMap[initialStateLabel] = e.Edge.Target.Label;
			};
			depthFirstSearchAlgorithm.Visit(startNode);
		}

		private string MakeString(IEnumerable<string> strings)
		{
			if (strings.Count() > 0)
			{
				return strings.Aggregate((string a, string b) => a + ", " + b);
			}
			return string.Empty;
		}
	}
}
