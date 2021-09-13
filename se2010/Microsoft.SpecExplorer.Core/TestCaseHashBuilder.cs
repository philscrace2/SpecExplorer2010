// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.TestCaseHashBuilder
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.SpecExplorer
{
  public class TestCaseHashBuilder
  {
    private const string versionNum = "0x01";
    private IGraph<State, Transition> graph;
    private Dictionary<Transition, string> stepsInformation;
    private Dictionary<string, string> methodsInformation;
    private VariableRenamingVisitor visitor;
    private Dictionary<string, SerializableMemberInfo> methodMap;

    public TestCaseHashBuilder(TransitionSystem transitionSystem)
    {
      this.graph = (IGraph<State, Transition>) new TransitionSystemGraphBuilder(transitionSystem).BuildGraph();
      this.methodMap = transitionSystem.InitializeActionMethodMap();
      this.stepsInformation = new Dictionary<Transition, string>();
      this.visitor = new VariableRenamingVisitor();
      this.methodsInformation = new Dictionary<string, string>();
    }

    public string GetHashCode(string start) => "0x01" + TestCaseHashBuilder.ComputeMd5Hash(this.TraversalFromOneNode(start));

    private string TraversalFromOneNode(string start)
    {
      StringBuilder sb = new StringBuilder();
      int nodeSequence = 0;
      Dictionary<string, int> nodeSequenceDict = new Dictionary<string, int>();
      DepthFirstSearchAlgorithm<State, Transition> firstSearchAlgorithm = new DepthFirstSearchAlgorithm<State, Transition>(this.graph);
      firstSearchAlgorithm.FinishNode += (EventHandler<NodeEventArgs<State>>) ((sender, nodeArg) =>
      {
        Node<State> node = nodeArg.Node;
        if (this.graph.OutgoingCount(node) != 0)
          return;
        sb.AppendLine(string.Format("Accepting: {0}", (object) ((node.Label.Flags & 2) != 0)));
        sb.AppendLine(string.Format("Error: {0}", (object) ((node.Label.Flags & 4) != 0)));
      });
      firstSearchAlgorithm.VisitEdge += (EventHandler<EdgeEventArgs<State, Transition>>) ((sender, edgeArg) =>
      {
        Edge<State, Transition> edge = edgeArg.Edge;
        string label1 = edge.Source.Label.Label;
        if (!nodeSequenceDict.ContainsKey(label1))
          nodeSequenceDict[label1] = nodeSequence++;
        string label2 = edge.Target.Label.Label;
        if (!nodeSequenceDict.ContainsKey(label2))
          nodeSequenceDict[label2] = nodeSequence++;
        sb.AppendLine(string.Format("{0} {1} {2}", (object) nodeSequenceDict[label1], (object) this.TransitionToString(edge.Label), (object) nodeSequenceDict[label2]));
      });
      Node<State> startNode;
      if (!this.graph.GetInitialNodeByLabel(start, out startNode))
        throw new InvalidOperationException("No matched initial node.");
      firstSearchAlgorithm.Visit(startNode);
      return this.BuildMethodsInformation() + sb.ToString();
    }

    private string TransitionToString(Transition step)
    {
      string str1;
      if (this.stepsInformation.TryGetValue(step, out str1))
        return str1;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format("ActionKind: {0}", (object) step.Action.Symbol.Kind));
      string header = step.Action.Symbol.Member.Header;
      stringBuilder.AppendLine(string.Format("ActionSymbolName: {0}", (object) header));
      if (!this.methodsInformation.ContainsKey(header))
        this.methodsInformation[header] = this.MethodToString(header);
      if (step.Action.Arguments.Length > 0)
        stringBuilder.AppendLine(string.Format("Arguments:{0}", (object) string.Join(",", ((IEnumerable<SerializableExpression>) step.Action.Arguments).Select<SerializableExpression, string>((Func<SerializableExpression, string>) (argument => ((object) this.visitor.Visit(argument)).ToString())).ToArray<string>())));
      if (step.PreConstraints.Length > 0)
      {
        List<string> array = new List<string>();
        foreach (Constraint preConstraint in step.PreConstraints)
          array.Add(((object) this.visitor.Visit(preConstraint.Expression)).ToString());
        stringBuilder.AppendLine(string.Format("StateConstraints:{0}", (object) TestCaseHashBuilder.ListSortedToString(array)));
      }
      if (step.PostConstraints.Length > 0)
      {
        List<string> array = new List<string>();
        foreach (Constraint postConstraint in step.PostConstraints)
          array.Add(((object) this.visitor.Visit(postConstraint.Expression)).ToString());
        stringBuilder.AppendLine(string.Format("ParameterConstraints:{0}", (object) TestCaseHashBuilder.ListSortedToString(array)));
      }
      if (step.CapturedRequirements.Length > 0)
        stringBuilder.AppendLine(string.Format("CapturedRequirements:{0}", (object) TestCaseHashBuilder.ListSortedToString(new List<string>((IEnumerable<string>) step.CapturedRequirements))));
      if (step.AssumeCapturedRequirements.Length > 0)
        stringBuilder.AppendLine(string.Format("AssumeRequirements:{0}", (object) TestCaseHashBuilder.ListSortedToString(new List<string>((IEnumerable<string>) step.AssumeCapturedRequirements))));
      string str2 = stringBuilder.ToString();
      this.stepsInformation[step] = str2;
      return str2;
    }

    private string MethodToString(string methodName)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(methodName);
      SerializableMemberInfo method = this.methodMap[methodName];
      SerializableParameterInfo[] serializableParameterInfoArray = (SerializableParameterInfo[]) null;
      switch (method)
      {
        case SerializableConstructorInfo _:
          SerializableConstructorInfo serializableConstructorInfo = method as SerializableConstructorInfo;
          stringBuilder.Append("ctor");
          serializableParameterInfoArray = ((SerializableMethodBase) serializableConstructorInfo).Parameters;
          break;
        case SerializableMethodInfo _:
          SerializableMethodInfo serializableMethodInfo = method as SerializableMethodInfo;
          if (serializableMethodInfo.ReturnType != null)
            stringBuilder.AppendLine(string.Format("ReturnType: {0}", (object) serializableMethodInfo.ReturnType.FullName));
          serializableParameterInfoArray = ((SerializableMethodBase) serializableMethodInfo).Parameters;
          break;
        case SerializableEventInfo _:
          serializableParameterInfoArray = (method as SerializableEventInfo).Parameters;
          break;
      }
      if (serializableParameterInfoArray.Length > 0)
      {
        stringBuilder.Append("ParameterType:");
        bool flag = true;
        foreach (SerializableParameterInfo serializableParameterInfo in serializableParameterInfoArray)
        {
          if (flag)
            flag = false;
          else
            stringBuilder.Append(",");
          if (serializableParameterInfo.IsOut)
            stringBuilder.Append(" out");
          stringBuilder.AppendFormat(" {0}", (object) serializableParameterInfo.Type.FullName);
        }
      }
      return stringBuilder.ToString();
    }

    private string BuildMethodsInformation()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string str in this.methodsInformation.Values)
        stringBuilder.AppendLine(str);
      return stringBuilder.ToString();
    }

    private static string ListSortedToString(List<string> array)
    {
      array.Sort();
      return string.Join(",", array.ToArray());
    }

    private static string ComputeMd5Hash(string input)
    {
      byte[] hash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in hash)
        stringBuilder.Append(num.ToString("x2"));
      return stringBuilder.ToString();
    }
  }
}
