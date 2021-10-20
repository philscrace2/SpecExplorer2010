using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ActionMachines;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.Properties;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	internal class ReplayStepsBuilder
	{
		private ComponentBase host;

		private Dictionary<string, Type> resolvedTypes = new Dictionary<string, Type>();

		internal ReplayStepsBuilder(ComponentBase host)
		{
			this.host = host;
		}

		private Type ResolveType(SerializableType type)
		{
			Type value;
			if (!resolvedTypes.TryGetValue(type.FullName, out value))
			{
				value = type.ToType();
				if (value == null)
				{
					throw new ExplorationRuntimeException(string.Format(Resources.CannotResolveTypeFormat, type.FullName));
				}
				resolvedTypes[type.FullName] = value;
			}
			return value;
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
				throw new ExplorationRuntimeException(string.Format("Failed to load file {0}:\n{1}", replayResultPath, ex.Message), ex);
			}
			IGraph<State, Transition> graph = new TransitionSystemGraphBuilder(transitionSystem).BuildGraph();
			return CreateReplaySteps(graph);
		}

		internal IList<ReplayStep> CreateReplaySteps(IGraph<State, Transition> graph)
		{
			IProgram requiredService = host.GetRequiredService<IProgram>();
			IConfigurationProvider requiredService2 = host.GetRequiredService<IConfigurationProvider>();
			List<Node<State>> list = new List<Node<State>>(graph.StartNodes);
			if (list.Count != 1)
			{
				throw new ExplorationRuntimeException("Replay exploration result could and only could contains 1 initial state.");
			}
			Node<State> node = list[0];
			HashSet<Node<State>> hashSet = new HashSet<Node<State>>();
			List<ReplayStep> list2 = new List<ReplayStep>();
			while (hashSet.Add(node))
			{
				List<Edge<State, Transition>> outEdges = null;
				if (!graph.TryGetOutGoingEdges(node, out outEdges) || outEdges.Count == 0)
				{
					break;
				}
				if (outEdges.Count > 1)
				{
					throw new ExplorationRuntimeException("Each state in replay exploration result cannot contain more than 1 outgoing transition.");
				}
				Edge<State, Transition> edge = outEdges[0];
				node = edge.Target;
				switch (edge.Label.Action.Symbol.Kind)
				{
				case ActionSymbolKind.Call:
				case ActionSymbolKind.Return:
				case ActionSymbolKind.Event:
					if (string.Compare(edge.Label.Action.Symbol.Member.Name, "<error>", true) == 0)
					{
						if (graph.OutgoingCount(node) > 0)
						{
							throw new ExplorationRuntimeException("Cannot replay the exploration result who contains error action but it's not the last action.");
						}
					}
					else
					{
						list2.Add(ToReplayStep(edge.Label, requiredService, requiredService2));
					}
					break;
				default:
					throw new ExplorationRuntimeException(string.Format("Invalid step {0}", edge.Label.Action.Symbol.Member.ToString()));
				}
			}
			return list2;
		}

		private ReplayStep ToReplayStep(Transition transition, IProgram program, IConfigurationProvider configurationProvider)
		{
			List<string> list = new List<string>();
			List<Term> list2 = new List<Term>();
			ActionInvocation action = transition.Action;
			IType type = program.LoadType(ResolveType(action.Symbol.Member.DeclaringType));
			SerializableExpression[] arguments = action.Arguments;
			foreach (SerializableExpression serializableExpression in arguments)
			{
				list2.Add(Term.Undef);
				list.Add(serializableExpression.ToString());
			}
			SerializableMemberInfo member = action.Symbol.Member;
			string text;
			if (member is SerializableMethodInfo)
			{
				SerializableMethodInfo serializableMethodInfo = member as SerializableMethodInfo;
				text = ((serializableMethodInfo.Parameters.Length <= 0) ? string.Format("{0}.{1}", serializableMethodInfo.DeclaringType.FullName, serializableMethodInfo.Name) : string.Format("{0}.{1}({2})", serializableMethodInfo.DeclaringType.FullName, serializableMethodInfo.Name, string.Join(",", serializableMethodInfo.Parameters.Select((SerializableParameterInfo s) => MakeTypeName(ResolveType(s.Type))))));
			}
			else if (member is SerializableConstructorInfo)
			{
				SerializableConstructorInfo serializableConstructorInfo = member as SerializableConstructorInfo;
				text = ((serializableConstructorInfo.Parameters.Length <= 0) ? string.Format("{0}.#ctor", serializableConstructorInfo.DeclaringType.FullName) : string.Format("{0}.#ctor({1})", serializableConstructorInfo.DeclaringType.FullName, string.Join(",", serializableConstructorInfo.Parameters.Select((SerializableParameterInfo s) => MakeTypeName(ResolveType(s.Type))))));
			}
			else
			{
				if (!(member is SerializableEventInfo))
				{
					throw new ExplorationRuntimeException("Un-supported member type.");
				}
				text = "";
			}
			IActionSymbol actionSymbol = null;
			if (action.Symbol.Kind != ActionSymbolKind.Event)
			{
				IMethod member2 = null;
				if (!type.TryGetMember<IMethod>(text, out member2))
				{
					throw new ExplorationRuntimeException(string.Format("No matched rule method {0}", text));
				}
				actionSymbol = configurationProvider.GetConcreteActionSymbol(member2);
			}
			else
			{
				IAssociation association = null;
				if (!TryGetAssociation(member as SerializableEventInfo, type, out association))
				{
					throw new ExplorationRuntimeException(string.Format("No matched event {0}", text));
				}
				actionSymbol = configurationProvider.GetConcreteActionSymbol(association);
			}
			IActionSymbol actionSymbol2 = null;
			switch (action.Symbol.Kind)
			{
			case ActionSymbolKind.Call:
				foreach (IActionSymbol basicActionSymbol in actionSymbol.BasicActionSymbols)
				{
					if (basicActionSymbol.Kind == ActionKind.Call)
					{
						actionSymbol2 = basicActionSymbol;
					}
				}
				break;
			case ActionSymbolKind.Event:
				actionSymbol2 = actionSymbol.BasicActionSymbols.First();
				break;
			case ActionSymbolKind.Return:
				foreach (IActionSymbol basicActionSymbol2 in actionSymbol.BasicActionSymbols)
				{
					if (basicActionSymbol2.Kind == ActionKind.Return)
					{
						actionSymbol2 = basicActionSymbol2;
					}
				}
				break;
			default:
				throw new InvalidOperationException(string.Format("Unsupported action symbol kind '{0}'", action.Symbol.Kind.ToString()));
			}
			if (actionSymbol2 == null)
			{
				throw new ExplorationRuntimeException("No matched action symbol.");
			}
			return new ReplayStep(new Microsoft.ActionMachines.Action(actionSymbol2, list2.ToArray()), list.ToArray());
		}

		private bool TryGetAssociation(SerializableEventInfo eventInfo, IType type, out IAssociation association)
		{
			association = null;
			IMethod method = null;
			if (eventInfo == null || eventInfo.Name == null)
			{
				return false;
			}
			IAssociation[] associations = type.Associations;
			foreach (IAssociation association2 in associations)
			{
				if (!(association2.ShortName == eventInfo.Name) || eventInfo.IsStatic != association2.IsStatic)
				{
					continue;
				}
				for (int j = 0; j < association2.Type.DeclaredMethods.Length; j++)
				{
					string shortName = association2.Type.DeclaredMethods[j].ShortName;
					if (shortName.Equals("Invoke"))
					{
						method = association2.Type.DeclaredMethods[j];
						break;
					}
				}
				if (method == null)
				{
					continue;
				}
				int num = 1;
				int num2 = method.Parameters.Length - num;
				if (eventInfo.Parameters == null)
				{
					if (num2 != 0)
					{
						continue;
					}
				}
				else
				{
					if (eventInfo.Parameters.Length != num2)
					{
						return false;
					}
					for (int k = 0; k < eventInfo.Parameters.Length; k++)
					{
						List<Type> list = new List<Type>();
						SerializableParameterInfo serializableParameterInfo = eventInfo.Parameters[k];
						if (serializableParameterInfo.Type != null)
						{
							list.Add(ResolveType(serializableParameterInfo.Type));
						}
						List<IType> list2 = new List<IType>();
						if (eventInfo.Parameters[k].IsOut)
						{
							foreach (IType item in list)
							{
								if (!item.IsAddressType)
								{
									list2.Add(item.AddressType);
								}
							}
						}
						if (list == null || (!list.Contains(method.Parameters[k + num].Type.RuntimeType) && !list2.Contains(method.Parameters[k + num].Type)))
						{
							return false;
						}
					}
				}
				if (eventInfo.ReturnType == null && method.ResultIsVoid)
				{
					association = association2;
					return true;
				}
				Type type3 = ResolveType(eventInfo.ReturnType);
				if (type3 == method.ResultType.RuntimeType || IsSubType(method.ResultType.RuntimeType, type3))
				{
					association = association2;
					return true;
				}
			}
			return false;
		}

		private static bool IsSubType(Type type1, Type type2)
		{
			if (type2.BaseType == null)
			{
				return false;
			}
			if (type2.BaseType == type1)
			{
				return true;
			}
			return IsSubType(type1, type2.BaseType);
		}

		private static string MakeTypeName(Type type)
		{
			if (type.IsByRef)
			{
				return MakeTypeName(type.GetElementType()) + "@";
			}
			if (type.IsGenericType)
			{
				return string.Format("{0}.{1}<{2}>", type.Namespace, type.Name.Substring(0, type.Name.Length - 2), string.Join(",", from s in type.GetGenericArguments()
					select MakeTypeName(s)));
			}
			return type.FullName;
		}
	}
}
