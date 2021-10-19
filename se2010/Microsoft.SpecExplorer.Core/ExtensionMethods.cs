// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ExtensionMethods
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodeKind = Microsoft.GraphTraversal.NodeKind;

namespace Microsoft.SpecExplorer
{
  public static class ExtensionMethods
  {
    public static bool IsBoolean(this SerializableType type) => type != null && type.FullName == "System.Boolean";

    public static bool IsSystemObject(this SerializableType type) => type != null && type.FullName == "System.Object";

    public static string ToProgressDurationFormat(this TimeSpan duration)
    {
      if (duration.Days > 0)
        return string.Format("{0}:{1}:{2}:{3}.{4}", (object) duration.Days, (object) duration.Hours, (object) duration.Minutes, (object) duration.Seconds, (object) duration.Milliseconds);
      if (duration.Hours > 0)
        return string.Format("{0}:{1}:{2}.{3}", (object) duration.Hours, (object) duration.Minutes, (object) duration.Seconds, (object) duration.Milliseconds);
      if (duration.Minutes > 0)
        return string.Format("{0}:{1}.{2}", (object) duration.Minutes, (object) duration.Seconds, (object) duration.Milliseconds);
      return duration.Seconds > 0 ? string.Format("{0}.{1}", (object) duration.Seconds, (object) duration.Milliseconds) : string.Format("0.{0}", (object) duration.Milliseconds);
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

    public static Dictionary<string, SerializableMemberInfo> InitializeActionMethodMap(
      this TransitionSystem transitionSystem)
    {
      if (transitionSystem == null)
        throw new ArgumentNullException(nameof (transitionSystem));
      Dictionary<string, SerializableMemberInfo> dictionary = new Dictionary<string, SerializableMemberInfo>();
      foreach (SerializableMemberInfo actionMember in transitionSystem.ActionMembers)
        dictionary[actionMember.Header] = actionMember;
      return dictionary;
    }

    public static bool GetInitialNodeByLabel(
      this IGraph<State, Transition> graph,
      string label,
      out Node<State> startNode)
    {
      startNode = graph.Nodes.FirstOrDefault<Node<State>>((Func<Node<State>, bool>) (n => n.Label.Label == label));
      return startNode != null;
    }

    public static Microsoft.GraphTraversal.NodeKind ToNodeKind(this StateFlags stateFlag)
    {
        if (StateFlags.Accepting == stateFlag)
        {
            return Microsoft.GraphTraversal.NodeKind.Accepting;
        }
        if (StateFlags.Error == stateFlag)
        {
            return Microsoft.GraphTraversal.NodeKind.Error;
        }
        return Microsoft.GraphTraversal.NodeKind.Regular;
    }

        public static bool IsObservable(this ActionInvocation action) => action != null && action.Symbol != null && action.Symbol.Kind.Equals(1);

    public static bool ShouldSaveTestResult(
      this TestResult result,
      ExperimentTracePreservationOption option)
    {
      switch (result)
      {
        case TestResult.Passed:
          return (option & ExperimentTracePreservationOption.Passed) != ExperimentTracePreservationOption.None;
        case TestResult.Failed:
          return (option & ExperimentTracePreservationOption.Failed) != ExperimentTracePreservationOption.None;
        case TestResult.Inconclusive:
          return (option & ExperimentTracePreservationOption.Inconclusive) != ExperimentTracePreservationOption.None;
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
      for (int index = 0; index < input.Length; ++index)
      {
        switch (input[index])
        {
          case '"':
            if (!inSingleQuotes)
            {
              inDoubleQuotes = !inDoubleQuotes;
              break;
            }
            break;
          case '\'':
            if (!inDoubleQuotes)
            {
              inSingleQuotes = !inSingleQuotes;
              break;
            }
            break;
          case '(':
            if (!inSingleQuotes && !inDoubleQuotes)
            {
              ++roundBracketDepth;
              break;
            }
            break;
          case ')':
            if (!inSingleQuotes && !inDoubleQuotes)
            {
              --roundBracketDepth;
              break;
            }
            break;
          case ',':
            if (roundBracketDepth == 0 && squareBracketDepth == 0 && (braceBracketDepth == 0 && angleBracketDepth == 0) && (!inSingleQuotes && !inDoubleQuotes))
            {
              spliterIndices.Add(index);
              break;
            }
            break;
          case '<':
            if (!inSingleQuotes && !inDoubleQuotes)
            {
              ++angleBracketDepth;
              break;
            }
            break;
          case '>':
            if (!inSingleQuotes && !inDoubleQuotes)
            {
              --angleBracketDepth;
              break;
            }
            break;
          case '[':
            if (!inSingleQuotes && !inDoubleQuotes)
            {
              ++squareBracketDepth;
              break;
            }
            break;
          case '\\':
            ++index;
            break;
          case ']':
            if (!inSingleQuotes && !inDoubleQuotes)
            {
              --squareBracketDepth;
              break;
            }
            break;
          case '{':
            if (!inSingleQuotes && !inDoubleQuotes)
            {
              ++braceBracketDepth;
              break;
            }
            break;
          case '}':
            if (!inSingleQuotes && !inDoubleQuotes)
            {
              --braceBracketDepth;
              break;
            }
            break;
        }
      }
      int startIndex = 0;
      foreach (int num in spliterIndices)
      {
        yield return input.Substring(startIndex, num - startIndex).Trim();
        startIndex = num + 1;
      }
      yield return input.Substring(startIndex).Trim();
    }

    public static IEnumerable<CodeAttributeDeclaration> SplitAttributes(
      this string input)
    {
      foreach (string name in input.TopLevelCommaSplitter())
      {
        int startRoundBracketIndex = name.IndexOf('(');
        if (startRoundBracketIndex > 0 && name.EndsWith(")"))
        {
          CodeAttributeDeclaration codeAttributeDeclarition = new CodeAttributeDeclaration(name.Substring(0, startRoundBracketIndex));
          string argumentStrings = name.Substring(startRoundBracketIndex + 1, name.Length - startRoundBracketIndex - 2);
          foreach (string str in argumentStrings.TopLevelCommaSplitter())
            codeAttributeDeclarition.Arguments.Add(new CodeAttributeArgument((CodeExpression) new CodeSnippetExpression(str)));
          yield return codeAttributeDeclarition;
        }
        else
          yield return new CodeAttributeDeclaration(name);
      }
    }

    public static string ToDisplayText(this ActionSymbol symbol)
    {
      if (symbol == null)
        throw new ArgumentNullException(nameof (symbol));
      StringBuilder builder = new StringBuilder();
      if (symbol.Member.IsStatic)
        builder.Append("static ");
      if (symbol.Member is SerializableEventInfo)
      {
        SerializableEventInfo member = symbol.Member as SerializableEventInfo;
        builder.Append("event ");
        ExtensionMethods.BuildMemberText(builder, member.ReturnType, ((SerializableMemberInfo) member).Name, ((SerializableMemberInfo) member).DeclaringType, member.Parameters);
      }
      else
      {
        if (!(symbol.Member is SerializableMethodBase))
          throw new InvalidOperationException("unsupported serializable member.");
        if (symbol.Member is SerializableMethodInfo)
        {
          SerializableMethodInfo member = symbol.Member as SerializableMethodInfo;
          if (member.AssociationReference != null)
          {
            if (!(member.AssociationReference.Association is SerializablePropertyInfo association6))
              throw new InvalidOperationException("unsupported serializable member.");
            builder.Append(association6.PropertyType.FullName.CollapsePrimitiveType() + " " + ((SerializableMemberInfo) association6).Name);
          }
          else
            ExtensionMethods.BuildMemberText(builder, member.ReturnType, ((SerializableMemberInfo) member).Name, ((SerializableMemberInfo) member).DeclaringType, ((SerializableMethodBase) member).Parameters);
        }
        else
        {
          SerializableConstructorInfo serializableConstructorInfo = symbol.Member is SerializableConstructorInfo ? symbol.Member as SerializableConstructorInfo : throw new InvalidOperationException("unsupported serializable member.");
          builder.Append(((SerializableMemberInfo) serializableConstructorInfo).DeclaringType.FullName.CollapsePrimitiveType());
          ExtensionMethods.BuildParameters(builder, ((SerializableMethodBase) serializableConstructorInfo).Parameters);
        }
      }
      return builder.ToString();
    }

    public static void BuildMemberText(
      StringBuilder builder,
      SerializableType returnType,
      string name,
      SerializableType declaringType,
      SerializableParameterInfo[] parameters)
    {
      if (returnType != null)
        builder.Append(returnType.FullName.CollapsePrimitiveType() + " ");
      else
        builder.Append("void ");
      builder.Append(declaringType.FullName.CollapsePrimitiveType() + "." + name);
      ExtensionMethods.BuildParameters(builder, parameters);
    }

    private static void BuildParameters(
      StringBuilder builder,
      SerializableParameterInfo[] parameters)
    {
      builder.Append("(");
      for (int index = 0; index < parameters.Length; ++index)
      {
        SerializableParameterInfo parameter = parameters[index];
        if (index == 0)
          builder.Append(ExtensionMethods.BuildParameterText(parameter));
        else
          builder.Append(" ," + ExtensionMethods.BuildParameterText(parameter));
      }
      builder.Append(")");
    }

    private static string BuildParameterText(SerializableParameterInfo parameter)
    {
      if (!parameter.Type.IsByRef)
        return parameter.Type.FullName.CollapsePrimitiveType();
      return parameter.IsOut ? "out " + parameter.Type.FullName.CollapsePrimitiveType() : "ref " + parameter.Type.FullName.CollapsePrimitiveType();
    }
  }
}
