using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;

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
			graph = new TransitionSystemGraphBuilder(transitionSystem).BuildGraph();
			methodMap = transitionSystem.InitializeActionMethodMap();
			stepsInformation = new Dictionary<Transition, string>();
			visitor = new VariableRenamingVisitor();
			methodsInformation = new Dictionary<string, string>();
		}

		public string GetHashCode(string start)
		{
			return "0x01" + ComputeMd5Hash(TraversalFromOneNode(start));
		}

		private string TraversalFromOneNode(string start)
		{
			StringBuilder sb = new StringBuilder();
			int nodeSequence = 0;
			Dictionary<string, int> nodeSequenceDict = new Dictionary<string, int>();
			DepthFirstSearchAlgorithm<State, Transition> depthFirstSearchAlgorithm = new DepthFirstSearchAlgorithm<State, Transition>(graph);
			depthFirstSearchAlgorithm.FinishNode += delegate(object sender, NodeEventArgs<State> nodeArg)
			{
				Node<State> node = nodeArg.Node;
				if (graph.OutgoingCount(node) == 0)
				{
					sb.AppendLine(string.Format("Accepting: {0}", (node.Label.Flags & StateFlags.Accepting) != 0));
					sb.AppendLine(string.Format("Error: {0}", (node.Label.Flags & StateFlags.Error) != 0));
				}
			};
			depthFirstSearchAlgorithm.VisitEdge += delegate(object sender, EdgeEventArgs<State, Transition> edgeArg)
			{
				Edge<State, Transition> edge = edgeArg.Edge;
				string label = edge.Source.Label.Label;
				if (!nodeSequenceDict.ContainsKey(label))
				{
					nodeSequenceDict[label] = nodeSequence++;
				}
				string label2 = edge.Target.Label.Label;
				if (!nodeSequenceDict.ContainsKey(label2))
				{
					nodeSequenceDict[label2] = nodeSequence++;
				}
				sb.AppendLine(string.Format("{0} {1} {2}", nodeSequenceDict[label], TransitionToString(edge.Label), nodeSequenceDict[label2]));
			};
			Node<State> startNode;
			if (!graph.GetInitialNodeByLabel(start, out startNode))
			{
				throw new InvalidOperationException("No matched initial node.");
			}
			depthFirstSearchAlgorithm.Visit(startNode);
			return BuildMethodsInformation() + sb.ToString();
		}

		private string TransitionToString(Transition step)
		{
			string value;
			if (stepsInformation.TryGetValue(step, out value))
			{
				return value;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("ActionKind: {0}", step.Action.Symbol.Kind));
			string header = step.Action.Symbol.Member.Header;
			stringBuilder.AppendLine(string.Format("ActionSymbolName: {0}", header));
			if (!methodsInformation.ContainsKey(header))
			{
				methodsInformation[header] = MethodToString(header);
			}
			if (step.Action.Arguments.Length > 0)
			{
				stringBuilder.AppendLine(string.Format("Arguments:{0}", string.Join(",", step.Action.Arguments.Select((SerializableExpression argument) => visitor.Visit(argument).ToString()).ToArray())));
			}
			if (step.PreConstraints.Length > 0)
			{
				List<string> list = new List<string>();
				Constraint[] preConstraints = step.PreConstraints;
				foreach (Constraint constraint in preConstraints)
				{
					list.Add(visitor.Visit(constraint.Expression).ToString());
				}
				stringBuilder.AppendLine(string.Format("StateConstraints:{0}", ListSortedToString(list)));
			}
			if (step.PostConstraints.Length > 0)
			{
				List<string> list2 = new List<string>();
				Constraint[] postConstraints = step.PostConstraints;
				foreach (Constraint constraint2 in postConstraints)
				{
					list2.Add(visitor.Visit(constraint2.Expression).ToString());
				}
				stringBuilder.AppendLine(string.Format("ParameterConstraints:{0}", ListSortedToString(list2)));
			}
			if (step.CapturedRequirements.Length > 0)
			{
				stringBuilder.AppendLine(string.Format("CapturedRequirements:{0}", ListSortedToString(new List<string>(step.CapturedRequirements))));
			}
			if (step.AssumeCapturedRequirements.Length > 0)
			{
				stringBuilder.AppendLine(string.Format("AssumeRequirements:{0}", ListSortedToString(new List<string>(step.AssumeCapturedRequirements))));
			}
			value = stringBuilder.ToString();
			stepsInformation[step] = value;
			return value;
		}

		private string MethodToString(string methodName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(methodName);
			SerializableMemberInfo serializableMemberInfo = methodMap[methodName];
			SerializableParameterInfo[] array = null;
			if (serializableMemberInfo is SerializableConstructorInfo)
			{
				SerializableConstructorInfo serializableConstructorInfo = serializableMemberInfo as SerializableConstructorInfo;
				stringBuilder.Append("ctor");
				array = serializableConstructorInfo.Parameters;
			}
			else if (serializableMemberInfo is SerializableMethodInfo)
			{
				SerializableMethodInfo serializableMethodInfo = serializableMemberInfo as SerializableMethodInfo;
				if (serializableMethodInfo.ReturnType != null)
				{
					stringBuilder.AppendLine(string.Format("ReturnType: {0}", serializableMethodInfo.ReturnType.FullName));
				}
				array = serializableMethodInfo.Parameters;
			}
			else if (serializableMemberInfo is SerializableEventInfo)
			{
				SerializableEventInfo serializableEventInfo = serializableMemberInfo as SerializableEventInfo;
				array = serializableEventInfo.Parameters;
			}
			if (array.Length > 0)
			{
				stringBuilder.Append("ParameterType:");
				bool flag = true;
				SerializableParameterInfo[] array2 = array;
				foreach (SerializableParameterInfo serializableParameterInfo in array2)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						stringBuilder.Append(",");
					}
					if (serializableParameterInfo.IsOut)
					{
						stringBuilder.Append(" out");
					}
					stringBuilder.AppendFormat(" {0}", serializableParameterInfo.Type.FullName);
				}
			}
			return stringBuilder.ToString();
		}

		private string BuildMethodsInformation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in methodsInformation.Values)
			{
				stringBuilder.AppendLine(value);
			}
			return stringBuilder.ToString();
		}

		private static string ListSortedToString(List<string> array)
		{
			array.Sort();
			return string.Join(",", array.ToArray());
		}

		private static string ComputeMd5Hash(string input)
		{
			MD5 mD = MD5.Create();
			byte[] array = mD.ComputeHash(Encoding.Default.GetBytes(input));
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}
	}
}
