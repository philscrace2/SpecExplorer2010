using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	public static class ExtensionMethods
	{
		public static bool IsBoolean(this SerializableType type)
		{
			if (type != null)
			{
				return type.FullName == "System.Boolean";
			}
			return false;
		}

		public static bool IsSystemObject(this SerializableType type)
		{
			if (type != null)
			{
				return type.FullName == "System.Object";
			}
			return false;
		}

		public static string ToProgressDurationFormat(this TimeSpan duration)
		{
			if (duration.Days > 0)
			{
				return string.Format("{0}:{1}:{2}:{3}.{4}", duration.Days, duration.Hours, duration.Minutes, duration.Seconds, duration.Milliseconds);
			}
			if (duration.Hours > 0)
			{
				return string.Format("{0}:{1}:{2}.{3}", duration.Hours, duration.Minutes, duration.Seconds, duration.Milliseconds);
			}
			if (duration.Minutes > 0)
			{
				return string.Format("{0}:{1}.{2}", duration.Minutes, duration.Seconds, duration.Milliseconds);
			}
			if (duration.Seconds > 0)
			{
				return string.Format("{0}.{1}", duration.Seconds, duration.Milliseconds);
			}
			return string.Format("0.{0}", duration.Milliseconds);
		}

		public static DiagnosisKind ToDiagnosisKind(this ErrorKind kind)
		{
			switch (kind)
			{
			case ErrorKind.Warning:
				return DiagnosisKind.Warning;
			case ErrorKind.Hint:
				return DiagnosisKind.Hint;
			default:
				return DiagnosisKind.Error;
			}
		}

		public static Dictionary<string, SerializableMemberInfo> InitializeActionMethodMap(this TransitionSystem transitionSystem)
		{
			if (transitionSystem == null)
			{
				throw new ArgumentNullException("transitionSystem");
			}
			Dictionary<string, SerializableMemberInfo> dictionary = new Dictionary<string, SerializableMemberInfo>();
			SerializableMemberInfo[] actionMembers = transitionSystem.ActionMembers;
			foreach (SerializableMemberInfo serializableMemberInfo in actionMembers)
			{
				dictionary[serializableMemberInfo.Header] = serializableMemberInfo;
			}
			return dictionary;
		}

		public static bool GetInitialNodeByLabel(this IGraph<State, Transition> graph, string label, out Node<State> startNode)
		{
			startNode = graph.Nodes.FirstOrDefault((Node<State> n) => n.Label.Label == label);
			if (startNode == null)
			{
				return false;
			}
			return true;
		}

		public static Microsoft.GraphTraversal.NodeKind ToNodeKind(this StateFlags stateFlag)
		{
			if ((StateFlags.Accepting & stateFlag) != 0)
			{
				return Microsoft.GraphTraversal.NodeKind.Accepting;
			}
			if ((StateFlags.Error & stateFlag) != 0)
			{
				return Microsoft.GraphTraversal.NodeKind.Error;
			}
			return Microsoft.GraphTraversal.NodeKind.Regular;
		}

		public static bool IsObservable(this ActionInvocation action)
		{
			if (action != null && action.Symbol != null)
			{
				return action.Symbol.Kind != ActionSymbolKind.Call;
			}
			return false;
		}

		public static bool ShouldSaveTestResult(this TestResult result, ExperimentTracePreservationOption option)
		{
			switch (result)
			{
			case TestResult.Passed:
				return (option & ExperimentTracePreservationOption.Passed) != 0;
			case TestResult.Failed:
				return (option & ExperimentTracePreservationOption.Failed) != 0;
			case TestResult.Inconclusive:
				return (option & ExperimentTracePreservationOption.Inconclusive) != 0;
			default:
				return false;
			}
		}

		internal static IEnumerable<string> TopLevelCommaSplitter(this string input)
		{
			List<int> spliterIndices = new List<int>();
			int roundBracketDepth = 0;
			int squareBracketDepth = 0;
			int braceBracketDepth = 0;
			int angleBracketDepth = 0;
			bool inSingleQuotes = false;
			bool inDoubleQuotes = false;
			for (int i = 0; i < input.Length; i++)
			{
				switch (input[i])
				{
				case '(':
					if (!inSingleQuotes && !inDoubleQuotes)
					{
						roundBracketDepth++;
					}
					break;
				case ')':
					if (!inSingleQuotes && !inDoubleQuotes)
					{
						roundBracketDepth--;
					}
					break;
				case '[':
					if (!inSingleQuotes && !inDoubleQuotes)
					{
						squareBracketDepth++;
					}
					break;
				case ']':
					if (!inSingleQuotes && !inDoubleQuotes)
					{
						squareBracketDepth--;
					}
					break;
				case '{':
					if (!inSingleQuotes && !inDoubleQuotes)
					{
						braceBracketDepth++;
					}
					break;
				case '}':
					if (!inSingleQuotes && !inDoubleQuotes)
					{
						braceBracketDepth--;
					}
					break;
				case '<':
					if (!inSingleQuotes && !inDoubleQuotes)
					{
						angleBracketDepth++;
					}
					break;
				case '>':
					if (!inSingleQuotes && !inDoubleQuotes)
					{
						angleBracketDepth--;
					}
					break;
				case '"':
					if (!inSingleQuotes)
					{
						inDoubleQuotes = !inDoubleQuotes;
					}
					break;
				case '\'':
					if (!inDoubleQuotes)
					{
						inSingleQuotes = !inSingleQuotes;
					}
					break;
				case '\\':
					i++;
					break;
				case ',':
					if (roundBracketDepth == 0 && squareBracketDepth == 0 && braceBracketDepth == 0 && angleBracketDepth == 0 && !inSingleQuotes && !inDoubleQuotes)
					{
						spliterIndices.Add(i);
					}
					break;
				}
			}
			int startIndex = 0;
			foreach (int index in spliterIndices)
			{
				yield return input.Substring(startIndex, index - startIndex).Trim();
				startIndex = index + 1;
			}
			yield return input.Substring(startIndex).Trim();
		}

		public static IEnumerable<CodeAttributeDeclaration> SplitAttributes(this string input)
		{
			foreach (string attributeString in input.TopLevelCommaSplitter())
			{
				int startRoundBracketIndex = attributeString.IndexOf('(');
				if (startRoundBracketIndex > 0 && attributeString.EndsWith(")"))
				{
					CodeAttributeDeclaration codeAttributeDeclarition = new CodeAttributeDeclaration(attributeString.Substring(0, startRoundBracketIndex));
					string argumentStrings = attributeString.Substring(startRoundBracketIndex + 1, attributeString.Length - startRoundBracketIndex - 2);
					foreach (string item in argumentStrings.TopLevelCommaSplitter())
					{
						codeAttributeDeclarition.Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression(item)));
					}
					yield return codeAttributeDeclarition;
				}
				else
				{
					yield return new CodeAttributeDeclaration(attributeString);
				}
			}
		}

		public static string ToDisplayText(this ActionSymbol symbol)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException("symbol");
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (symbol.Member.IsStatic)
			{
				stringBuilder.Append("static ");
			}
			if (symbol.Member is SerializableEventInfo)
			{
				SerializableEventInfo serializableEventInfo = symbol.Member as SerializableEventInfo;
				stringBuilder.Append("event ");
				BuildMemberText(stringBuilder, serializableEventInfo.ReturnType, serializableEventInfo.Name, serializableEventInfo.DeclaringType, serializableEventInfo.Parameters);
			}
			else
			{
				if (!(symbol.Member is SerializableMethodBase))
				{
					throw new InvalidOperationException("unsupported serializable member.");
				}
				if (symbol.Member is SerializableMethodInfo)
				{
					SerializableMethodInfo serializableMethodInfo = symbol.Member as SerializableMethodInfo;
					if (serializableMethodInfo.AssociationReference != null)
					{
						SerializablePropertyInfo serializablePropertyInfo = serializableMethodInfo.AssociationReference.Association as SerializablePropertyInfo;
						if (serializablePropertyInfo == null)
						{
							throw new InvalidOperationException("unsupported serializable member.");
						}
						stringBuilder.Append(serializablePropertyInfo.PropertyType.FullName.CollapsePrimitiveType() + " " + serializablePropertyInfo.Name);
					}
					else
					{
						BuildMemberText(stringBuilder, serializableMethodInfo.ReturnType, serializableMethodInfo.Name, serializableMethodInfo.DeclaringType, serializableMethodInfo.Parameters);
					}
				}
				else
				{
					if (!(symbol.Member is SerializableConstructorInfo))
					{
						throw new InvalidOperationException("unsupported serializable member.");
					}
					SerializableConstructorInfo serializableConstructorInfo = symbol.Member as SerializableConstructorInfo;
					stringBuilder.Append(serializableConstructorInfo.DeclaringType.FullName.CollapsePrimitiveType());
					BuildParameters(stringBuilder, serializableConstructorInfo.Parameters);
				}
			}
			return stringBuilder.ToString();
		}

		public static void BuildMemberText(StringBuilder builder, SerializableType returnType, string name, SerializableType declaringType, SerializableParameterInfo[] parameters)
		{
			if (returnType != null)
			{
				builder.Append(returnType.FullName.CollapsePrimitiveType() + " ");
			}
			else
			{
				builder.Append("void ");
			}
			builder.Append(declaringType.FullName.CollapsePrimitiveType() + "." + name);
			BuildParameters(builder, parameters);
		}

		private static void BuildParameters(StringBuilder builder, SerializableParameterInfo[] parameters)
		{
			builder.Append("(");
			for (int i = 0; i < parameters.Length; i++)
			{
				SerializableParameterInfo parameter = parameters[i];
				if (i == 0)
				{
					builder.Append(BuildParameterText(parameter));
				}
				else
				{
					builder.Append(" ," + BuildParameterText(parameter));
				}
			}
			builder.Append(")");
		}

		private static string BuildParameterText(SerializableParameterInfo parameter)
		{
			if (parameter.Type.IsByRef)
			{
				if (parameter.IsOut)
				{
					return "out " + parameter.Type.FullName.CollapsePrimitiveType();
				}
				return "ref " + parameter.Type.FullName.CollapsePrimitiveType();
			}
			return parameter.Type.FullName.CollapsePrimitiveType();
		}
	}
}
