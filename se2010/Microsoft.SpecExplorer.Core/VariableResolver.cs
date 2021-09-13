// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VariableResolver
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.Linq;

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
      this.transitionSystemName = transitionSystem.Name;
      this.graph = (IGraph<State, Transition>) new TransitionSystemGraphBuilder(transitionSystem).BuildGraph();
      this.testCaseHashBuilder = new TestCaseHashBuilder(transitionSystem);
    }

    internal string Resolve(string switchName) => this.Resolve(switchName, string.Empty);

    internal string Resolve(string switchName, string initialState)
    {
      string str1 = this.transitionSystem.GetSwitch(switchName);
      IEnumerable<SubstitutionPattern> patterns;
      try
      {
        patterns = SubstitutionPattern.GetPatterns(str1, '$');
      }
      catch (SubstitutionPatternException ex)
      {
        throw new TestCodeGenerationException(ex.Message);
      }
      string str2 = str1;
      foreach (SubstitutionPattern substitutionPattern in patterns)
      {
        str2 = str2.Remove(substitutionPattern.StartIndex, substitutionPattern.EndIndex - substitutionPattern.StartIndex + 1);
        str2 = string.IsNullOrEmpty(initialState) ? str2.Insert(substitutionPattern.StartIndex, this.GetNonMethodLevelVariable(substitutionPattern.VariableText, switchName)) : str2.Insert(substitutionPattern.StartIndex, this.GetMethodLevelVariable(substitutionPattern.VariableText, switchName, initialState));
      }
      return str2.Replace("$$", "$");
    }

    internal string GetNonMethodLevelVariable(string variable, string switchName)
    {
      switch (variable.ToLower())
      {
        case "testcoveredrequirementset":
        case "testcoveredrequirementsequence":
        case "testcasehashcode":
          throw new TestCodeGenerationException(string.Format("Invalid value for switch {0}: cannot take Spec Explorer built-in variable ‘{1}'", (object) switchName, (object) variable));
        case "machinename":
          return this.transitionSystemName;
        default:
          if (variable.ToLower().StartsWith("endstateprobe("))
            throw new TestCodeGenerationException(string.Format("Invalid value for switch {0}: cannot take Spec Explorer built-in variable '{1}'", (object) switchName, (object) variable));
          throw new TestCodeGenerationException(string.Format("'{0}' is not a valid Spec Explorer built-in variable.", (object) variable));
      }
    }

    internal string GetMethodLevelVariable(string variable, string switchName, string initialState)
    {
      this.InitializeInitialState(initialState);
      switch (variable.ToLower())
      {
        case "testcoveredrequirementset":
          return this.MakeString((IEnumerable<string>) new HashSet<string>(this.GetMethodRequirements(initialState)));
        case "testcoveredrequirementsequence":
          return this.MakeString(this.GetMethodRequirements(initialState));
        case "testcasehashcode":
          return this.GetMethodHashCode(initialState);
        case "machinename":
          return this.transitionSystemName;
        default:
          if (!variable.ToLower().StartsWith("endstateprobe(") || !variable.EndsWith(")"))
            throw new TestCodeGenerationException(string.Format("'{0}' in switch '{1}' is not a valid Spec Explorer built-in variables", (object) variable, (object) switchName));
          State methodEndState = this.GetMethodEndState(initialState);
          string str1 = variable.Substring("endstateprobe(".Length, variable.Length - "endstateprobe(".Length - 1);
          Probe[] probeArray;
          if (!methodEndState.TryGetProbesByName(str1, out probeArray))
            throw new TestCodeGenerationException(string.Format("Probe '{0}' in switch '{1}' does not exist.", (object) str1, (object) switchName));
          string str2 = ((IEnumerable<Probe>) probeArray).Count<Probe>() == 1 ? probeArray[0].Value : throw new TestCodeGenerationException(string.Format("Probe '{0}' in switch '{1}' is ambiguous. Please use full name instead.", (object) str1, (object) switchName));
          if (probeArray[0].Kind != null)
            throw new TestCodeGenerationException(string.Format("Probe '{0}' in state '{1}' is not available.", (object) str1, (object) methodEndState.Label));
          return string.Compare(probeArray[0].Type.FullName, "System.String", true) == 0 || string.Compare(probeArray[0].Type.FullName, "System.Char", true) == 0 && str2.Length >= 2 && (str2.StartsWith("'") && str2.EndsWith("'")) ? str2.Substring(1, str2.Length - 2) : str2;
      }
    }

    internal string GetMethodHashCode(string initialState)
    {
      this.InitializeInitialState(initialState);
      return this.methodHashCodeMap[initialState];
    }

    internal IEnumerable<string> GetMethodRequirements(string initialState)
    {
      this.InitializeInitialState(initialState);
      return (IEnumerable<string>) this.methodRequirementsMap[initialState];
    }

    internal State GetMethodEndState(string initialState)
    {
      this.InitializeInitialState(initialState);
      return this.methodEndStateMap[initialState];
    }

    private void InitializeInitialState(string initialStateLabel)
    {
      Node<State> startNode;
      if (!this.graph.GetInitialNodeByLabel(initialStateLabel, out startNode))
        throw new InvalidOperationException("No matched initial node.");
      if (!this.initialStateSet.Add(initialStateLabel))
        return;
      this.methodHashCodeMap[initialStateLabel] = this.testCaseHashBuilder.GetHashCode(initialStateLabel);
      DepthFirstSearchAlgorithm<State, Transition> firstSearchAlgorithm = new DepthFirstSearchAlgorithm<State, Transition>(this.graph);
      List<string> requirements = this.methodRequirementsMap[initialStateLabel] = new List<string>();
      firstSearchAlgorithm.VisitEdge += (EventHandler<EdgeEventArgs<State, Transition>>) ((sender, e) => requirements.AddRange(e.Edge.Requirements));
      firstSearchAlgorithm.BackEdge += (EventHandler<EdgeEventArgs<State, Transition>>) ((sender, e) =>
      {
        if (this.methodEndStateMap.ContainsKey(initialStateLabel))
          return;
        this.methodEndStateMap[initialStateLabel] = e.Edge.Target.Label;
      });
      firstSearchAlgorithm.TreeEdge += (EventHandler<EdgeEventArgs<State, Transition>>) ((sender, e) => this.methodEndStateMap[initialStateLabel] = e.Edge.Target.Label);
      firstSearchAlgorithm.Visit(startNode);
    }

    private string MakeString(IEnumerable<string> strings) => strings.Count<string>() > 0 ? strings.Aggregate<string>((Func<string, string, string>) ((a, b) => a + ", " + b)) : string.Empty;
  }
}
