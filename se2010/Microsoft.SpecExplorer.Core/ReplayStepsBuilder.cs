// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ReplayStepsBuilder
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.Properties;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer
{
  internal class ReplayStepsBuilder
  {
    private ComponentBase host;
    private Dictionary<string, System.Type> resolvedTypes = new Dictionary<string, System.Type>();

    internal ReplayStepsBuilder(ComponentBase host) => this.host = host;

    private System.Type ResolveType(SerializableType type)
    {
      System.Type type1;
      if (!this.resolvedTypes.TryGetValue(type.FullName, out type1))
      {
        type1 = type.ToType();
        this.resolvedTypes[type.FullName] = !(type1 == (System.Type) null) ? type1 : throw new ExplorationRuntimeException(string.Format(Resources.CannotResolveTypeFormat, (object) type.FullName));
      }
      return type1;
    }

    internal IList<ReplayStep> CreateReplaySteps(string replayResultPath)
    {
      TransitionSystem transitionSystem;
      try
      {
        transitionSystem = new ExplorationResultLoader(replayResultPath).LoadTransitionSystem();
      }
      catch (ExplorationResultLoadingException ex)
      {
        throw new ExplorationRuntimeException(string.Format("Failed to load file {0}:\n{1}", (object) replayResultPath, (object) ((Exception) ex).Message), (Exception) ex);
      }
      return this.CreateReplaySteps((IGraph<State, Transition>) new TransitionSystemGraphBuilder(transitionSystem).BuildGraph());
    }

    internal IList<ReplayStep> CreateReplaySteps(IGraph<State, Transition> graph)
    {
      IProgram requiredService1 = this.host.GetRequiredService<IProgram>();
      IConfigurationProvider requiredService2 = this.host.GetRequiredService<IConfigurationProvider>();
      List<Node<State>> nodeList = new List<Node<State>>(graph.StartNodes);
      Node<State> node = nodeList.Count == 1 ? nodeList[0] : throw new ExplorationRuntimeException("Replay exploration result could and only could contains 1 initial state.");
      HashSet<Node<State>> nodeSet = new HashSet<Node<State>>();
      List<ReplayStep> replayStepList = new List<ReplayStep>();
      while (nodeSet.Add(node))
      {
        List<Edge<State, Transition>> outEdges = (List<Edge<State, Transition>>) null;
        if (graph.TryGetOutGoingEdges(node, out outEdges) && outEdges.Count != 0)
        {
          Edge<State, Transition> edge = outEdges.Count <= 1 ? outEdges[0] : throw new ExplorationRuntimeException("Each state in replay exploration result cannot contain more than 1 outgoing transition.");
          node = edge.Target;
          switch (edge.Label.Action.Symbol.Kind - 1)
          {
            case 0:
            case 1:
            case 3:
              if (string.Compare(edge.Label.Action.Symbol.Member.Name, "<error>", true) == 0)
              {
                if (graph.OutgoingCount(node) > 0)
                  throw new ExplorationRuntimeException("Cannot replay the exploration result who contains error action but it's not the last action.");
                continue;
              }
              replayStepList.Add(this.ToReplayStep(edge.Label, requiredService1, requiredService2));
              continue;
            default:
              throw new ExplorationRuntimeException(string.Format("Invalid step {0}", (object) ((object) edge.Label.Action.Symbol.Member).ToString()));
          }
        }
        else
          break;
      }
      return (IList<ReplayStep>) replayStepList;
    }

    private ReplayStep ToReplayStep(
      Transition transition,
      IProgram program,
      IConfigurationProvider configurationProvider)
    {
      List<string> stringList = new List<string>();
      List<Term> termList = new List<Term>();
      ActionInvocation action = transition.Action;
      IType type = program.LoadType(this.ResolveType(action.Symbol.Member.DeclaringType));
      foreach (SerializableExpression serializableExpression in action.Arguments)
      {
        termList.Add(Term.Undef);
        stringList.Add(((object) serializableExpression).ToString());
      }
      SerializableMemberInfo member1 = action.Symbol.Member;
      string name;
      switch (member1)
      {
        case SerializableMethodInfo _:
          SerializableMethodInfo serializableMethodInfo = member1 as SerializableMethodInfo;
          name = ((SerializableMethodBase) serializableMethodInfo).Parameters.Length <= 0 ? string.Format("{0}.{1}", (object) ((SerializableMemberInfo) serializableMethodInfo).DeclaringType.FullName, (object) ((SerializableMemberInfo) serializableMethodInfo).Name) : string.Format("{0}.{1}({2})", (object) ((SerializableMemberInfo) serializableMethodInfo).DeclaringType.FullName, (object) ((SerializableMemberInfo) serializableMethodInfo).Name, (object) string.Join(",", ((IEnumerable<SerializableParameterInfo>) ((SerializableMethodBase) serializableMethodInfo).Parameters).Select<SerializableParameterInfo, string>((Func<SerializableParameterInfo, string>) (s => ReplayStepsBuilder.MakeTypeName(this.ResolveType(s.Type))))));
          break;
        case SerializableConstructorInfo _:
          SerializableConstructorInfo serializableConstructorInfo = member1 as SerializableConstructorInfo;
          name = ((SerializableMethodBase) serializableConstructorInfo).Parameters.Length <= 0 ? string.Format("{0}.#ctor", (object) ((SerializableMemberInfo) serializableConstructorInfo).DeclaringType.FullName) : string.Format("{0}.#ctor({1})", (object) ((SerializableMemberInfo) serializableConstructorInfo).DeclaringType.FullName, (object) string.Join(",", ((IEnumerable<SerializableParameterInfo>) ((SerializableMethodBase) serializableConstructorInfo).Parameters).Select<SerializableParameterInfo, string>((Func<SerializableParameterInfo, string>) (s => ReplayStepsBuilder.MakeTypeName(this.ResolveType(s.Type))))));
          break;
        case SerializableEventInfo _:
          name = "";
          break;
        default:
          throw new ExplorationRuntimeException("Un-supported member type.");
      }
      IActionSymbol concreteActionSymbol;
      if (action.Symbol.Kind != 4)
      {
        IMethod member2 = (IMethod) null;
        if (!type.TryGetMember<IMethod>(name, out member2))
          throw new ExplorationRuntimeException(string.Format("No matched rule method {0}", (object) name));
        concreteActionSymbol = configurationProvider.GetConcreteActionSymbol((IMember) member2);
      }
      else
      {
        IAssociation association = (IAssociation) null;
        if (!this.TryGetAssociation(member1 as SerializableEventInfo, type, out association))
          throw new ExplorationRuntimeException(string.Format("No matched event {0}", (object) name));
        concreteActionSymbol = configurationProvider.GetConcreteActionSymbol((IMember) association);
      }
      IActionSymbol symbol = (IActionSymbol) null;
      switch (action.Symbol.Kind - 1)
      {
        case 0:
          using (IEnumerator<IActionSymbol> enumerator = concreteActionSymbol.BasicActionSymbols.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IActionSymbol current = enumerator.Current;
              if (current.Kind == ActionKind.Call)
                symbol = current;
            }
            break;
          }
        case 1:
          using (IEnumerator<IActionSymbol> enumerator = concreteActionSymbol.BasicActionSymbols.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IActionSymbol current = enumerator.Current;
              if (current.Kind == ActionKind.Return)
                symbol = current;
            }
            break;
          }
        case 3:
          symbol = concreteActionSymbol.BasicActionSymbols.First<IActionSymbol>();
          break;
        default:
          throw new InvalidOperationException(string.Format("Unsupported action symbol kind '{0}'", (object) ((object) action.Symbol.Kind).ToString()));
      }
      if (symbol == null)
        throw new ExplorationRuntimeException("No matched action symbol.");
      return new ReplayStep(new Microsoft.ActionMachines.Action(symbol, termList.ToArray()), stringList.ToArray());
    }

    private bool TryGetAssociation(
      SerializableEventInfo eventInfo,
      IType type,
      out IAssociation association)
    {
      association = (IAssociation) null;
      IMethod method = (IMethod) null;
      if (eventInfo == null || ((SerializableMemberInfo) eventInfo).Name == null)
        return false;
      foreach (IAssociation association1 in type.Associations)
      {
        if (association1.ShortName == ((SerializableMemberInfo) eventInfo).Name && ((SerializableMemberInfo) eventInfo).IsStatic == association1.IsStatic)
        {
          for (int index = 0; index < association1.Type.DeclaredMethods.Length; ++index)
          {
            if (association1.Type.DeclaredMethods[index].ShortName.Equals("Invoke"))
            {
              method = association1.Type.DeclaredMethods[index];
              break;
            }
          }
          if (method != null)
          {
            int num1 = 1;
            int num2 = method.Parameters.Length - num1;
            if (eventInfo.Parameters == null)
            {
              if (num2 != 0)
                continue;
            }
            else
            {
              if (eventInfo.Parameters.Length != num2)
                return false;
              for (int index = 0; index < eventInfo.Parameters.Length; ++index)
              {
                List<System.Type> typeList1 = new List<System.Type>();
                SerializableParameterInfo parameter = eventInfo.Parameters[index];
                if (parameter.Type != null)
                  typeList1.Add(this.ResolveType(parameter.Type));
                List<IType> typeList2 = new List<IType>();
                if (eventInfo.Parameters[index].IsOut)
                {
                  foreach (IType type1 in typeList1)
                  {
                    if (!type1.IsAddressType)
                      typeList2.Add(type1.AddressType);
                  }
                }
                if (typeList1 == null || !typeList1.Contains(method.Parameters[index + num1].Type.RuntimeType) && !typeList2.Contains(method.Parameters[index + num1].Type))
                  return false;
              }
            }
            if (eventInfo.ReturnType == null && method.ResultIsVoid)
            {
              association = association1;
              return true;
            }
            System.Type type2 = this.ResolveType(eventInfo.ReturnType);
            if (type2 == method.ResultType.RuntimeType || ReplayStepsBuilder.IsSubType(method.ResultType.RuntimeType, type2))
            {
              association = association1;
              return true;
            }
          }
        }
      }
      return false;
    }

    private static bool IsSubType(System.Type type1, System.Type type2)
    {
      if (type2.BaseType == (System.Type) null)
        return false;
      return type2.BaseType == type1 || ReplayStepsBuilder.IsSubType(type1, type2.BaseType);
    }

    private static string MakeTypeName(System.Type type)
    {
      if (type.IsByRef)
        return ReplayStepsBuilder.MakeTypeName(type.GetElementType()) + "@";
      return type.IsGenericType ? string.Format("{0}.{1}<{2}>", (object) type.Namespace, (object) type.Name.Substring(0, type.Name.Length - 2), (object) string.Join(",", ((IEnumerable<System.Type>) type.GetGenericArguments()).Select<System.Type, string>((Func<System.Type, string>) (s => ReplayStepsBuilder.MakeTypeName(s))))) : type.FullName;
    }
  }
}
