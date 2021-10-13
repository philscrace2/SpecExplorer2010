using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;
using Microsoft.Xrt.Persistence;

namespace Microsoft.SpecExplorer
{
	internal class TransitionSystemBuilder
	{
		private struct ExpressionCacheKey
		{
			internal Term term;

			internal IType type;
		}

		private string name;

		private IConfiguration config;

		private StateLabelBuilder labelBuilder;

		private ComponentBase host;

		private List<string> initialStates = new List<string>();

		private List<State> allStates = new List<State>();

		private Dictionary<MachineState, State> machineStates = new Dictionary<MachineState, State>();

		private List<Transition> transitions = new List<Transition>();

		private HashSet<SerializableMemberInfo> actionMembers = new HashSet<SerializableMemberInfo>();

		private HashSet<SerializableType> adapterTypes = new HashSet<SerializableType>();

		private List<ConfigSwitch> configSwitches = new List<ConfigSwitch>();

		private Dictionary<string, State> statesMap = new Dictionary<string, State>();

		private Dictionary<Term, ParameterExpression> variableMap = new Dictionary<Term, ParameterExpression>();

		private Dictionary<Term, SerializableParameterExpression> variableSerializableMap = new Dictionary<Term, SerializableParameterExpression>();

		private List<SerializableParameterExpression> variables = new List<SerializableParameterExpression>();

		private HashSet<string> usedVariableNames = new HashSet<string>();

		private Dictionary<ExpressionCacheKey, SerializableExpression> serializableExpressionCache = new Dictionary<ExpressionCacheKey, SerializableExpression>();

		private HashSet<string> usedExpressionKeys = new HashSet<string>();

		private IBackground bg;

		private IProgram program;

		private IStatePersistenceBuilder stateBuilder;

		private ITermDescriptionContextFactory contextFactory;

		private EventAdapter eventAdapter;

		private IActionSymbol preConstraintActionSymbol;

		internal TransitionSystemBuilder(string name, IConfiguration config, ComponentBase host, EventAdapter eventAdapter)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			if (eventAdapter == null)
			{
				throw new ArgumentNullException("eventAdapter");
			}
			this.host = host;
			this.name = name;
			this.config = config;
			this.eventAdapter = eventAdapter;
			IOptionSetManager requiredService = host.GetRequiredService<IOptionSetManager>();
			IOptionSet optionSet = this.config.OptionSet;
			foreach (PropertyDescriptor property in requiredService.GetProperties(Visibility.Public))
			{
				string switchValue = GetSwitchValue(optionSet, property);
				configSwitches.Add(TransitionSystem.ConfigSwitch(property.Name, switchValue));
			}
			bg = host.GetRequiredService<IBackground>();
			contextFactory = host.GetRequiredService<ITermDescriptionContextFactory>();
			program = host.GetRequiredService<IProgram>();
			stateBuilder = host.GetRequiredService<IStatePersistenceBuilderProvider>().CreateStatePersistencyBuilder();
			labelBuilder = new StateLabelBuilder();
			preConstraintActionSymbol = host.GetRequiredService<IConfigurationProvider>().PreConstraintActionSymbol;
		}

		private static string GetSwitchValue(IOptionSet optionSet, PropertyDescriptor p)
		{
			if (p.Converter != null)
			{
				try
				{
					return p.Converter.ConvertToString(optionSet.GetValue(p));
				}
				catch (Exception)
				{
					return optionSet.GetValue(p).ToString();
				}
			}
			return optionSet.GetValue(p).ToString();
		}

		internal ExplorationResult BuildTransitionSystem(bool withStateContent = true)
		{
			foreach (IActionSymbol action in config.Actions)
			{
				actionMembers.Add(BuildMethod(action));
			}
			TransitionSystem system = TransitionSystem.Create(name, configSwitches.OrderBy((ConfigSwitch sw) => sw.Name), actionMembers.OrderBy((SerializableMemberInfo m) => m.Header), adapterTypes.OrderBy((SerializableType t) => t.Header), variables.OrderBy((SerializableParameterExpression v) => v.Name), initialStates.OrderBy((string s) => s), allStates.OrderBy((State s) => s.Label), from t in transitions
				orderby t.Source, t.Action.Symbol.Member.Header, t.Target
				select t);
			if (withStateContent)
			{
				stateBuilder.BuildStateObjectModel();
				List<StateEntity> list = new List<StateEntity>();
				foreach (KeyValuePair<MachineState, State> machineState in machineStates)
				{
					StateEntity item = new StateEntity(machineState.Value.Label, machineState.Value.Flags, machineState.Value.Description, stateBuilder.GetStateContent(machineState.Key.Data));
					list.Add(item);
				}
				SharedEntitySet sharedEntitySet = stateBuilder.SharedEntitySet;
				return new ExplorationResult(system, sharedEntitySet, list, new ExplorationResultExtensions());
			}
			return new ExplorationResult(system, new SharedEntitySet(), Enumerable.Empty<StateEntity>(), new ExplorationResultExtensions());
		}

		internal void AddStep(ExplorationStep step, MachineState? subsumptionRelatedState, SubsumptionResult relatedStateRelationKind)
		{
			State state = AddState(step.SourceState, step.SourceFlags);
			State state2 = AddState(step.TargetState, step.TargetFlags);
			if (subsumptionRelatedState.HasValue)
			{
				State state3 = state2;
				State state4 = AddState(subsumptionRelatedState.Value, step.TargetFlags);
				if (relatedStateRelationKind == SubsumptionResult.ThisSubsumesOther)
				{
					State state5 = state3;
					state3 = state4;
					state4 = state5;
				}
				state3.RepresentativeState = state4.Label;
				state3.RelationKind = StateRelationKind.Subsumed;
			}
			ITermDescriptionContext context = contextFactory.CreateContext(TermDescriptionFlags.DefaultUserDescription | TermDescriptionFlags.ReEscapingInString, step.IntermediateTargetState.Data);
			ActionInvocation action = BuildAction(step.Action, context, step.TargetState.Data);
			string[] array = null;
			string[] array2 = null;
			if (step.Requirements == null)
			{
				array = new string[0];
				array2 = new string[0];
			}
			else
			{
				array = step.Requirements.CapturedRequirements.ToArray();
				array2 = step.Requirements.AssumeCapturedRequirements.ToArray();
			}
			TermSetBuilder constraints = CalculatePostConstraints(step);
			IOrderedEnumerable<Constraint> postConstraints = from t in FilterAndConvertConstraints(constraints)
				select TransitionSystem.Constraint(BuildData(t, program.SystemTypes.Boolean, step.TargetState.Data), context.ToString(program.SystemTypes.Boolean, t)) into c
				orderby c.Text
				select c;
			IOrderedEnumerable<Constraint> preConstraints = from t in FilterAndConvertConstraints(step.PreConstraints)
				select TransitionSystem.Constraint(BuildData(t, program.SystemTypes.Boolean, step.TargetState.Data), context.ToString(program.SystemTypes.Boolean, t)) into c
				orderby c.Text
				select c;
			TermSetBuilder termSetBuilder = new TermSetBuilder(step.FreedVariables);
			termSetBuilder.IntersectWith(variableMap.Keys);
			IOrderedEnumerable<SerializableParameterExpression> variablesToUnbind = from v in termSetBuilder
				select variableSerializableMap[v] into v
				orderby v.Name
				select v;
			Transition item = TransitionSystem.Transition(state.Label, action, state2.Label, preConstraints, postConstraints, variablesToUnbind, array, array2);
			transitions.Add(item);
		}

		private TermSetBuilder CalculatePostConstraints(ExplorationStep step)
		{
			TermSet context = step.SourceState.Data.Context;
			TermSet context2 = step.IntermediateTargetState.Data.Context;
			TermSetBuilder termSetBuilder = bg.MakeTermSetBuilder();
			if (step.Action.Symbol.Kind == ActionKind.Return || (step.Action.Symbol.Kind == ActionKind.Event && step.Action.Symbol != preConstraintActionSymbol))
			{
				termSetBuilder.UnionWith(context2);
				termSetBuilder.ExceptWith(context);
				ICompressedState csource = step.SourceState.Data;
				ICompressedState ctarget = step.IntermediateTargetState.Data;
				IActiveState asource = csource.Uncompress();
				IActiveState atarget = ctarget.Uncompress();
				foreach (Term item in context)
				{
					if (bg.FindInTerms((Term t) => bg.IsVariable(t) && !csource.IsExistential(t) && !asource.IsVariableBound(t) && !ctarget.IsExistential(t) && atarget.IsVariableBound(t), item))
					{
						termSetBuilder.Add(item);
					}
				}
				if (termSetBuilder.Count > 0)
				{
					NarrowReachableConstraints(step.Action, termSetBuilder, atarget.IsVariableBound);
				}
			}
			return termSetBuilder;
		}

		private List<Term> FilterAndConvertConstraints(IEnumerable<Term> constraints)
		{
			List<Term> list = new List<Term>();
			foreach (Term constraint in constraints)
			{
				if (bg.IsDomainPredicate(constraint))
				{
					Term term = bg.ConvertDomainPredicateToExplicitConstraint(constraint);
					if (term != bg.True)
					{
						list.Add(term);
					}
				}
				else if (!bg.IsFunctionApplication(constraint))
				{
					list.Add(constraint);
				}
			}
			return list;
		}

		private bool HasCycle(State thisState, State otherState)
		{
			State value = otherState;
			while (value.RepresentativeState != null && statesMap.TryGetValue(value.RepresentativeState, out value))
			{
				if (value.Label == thisState.Label)
				{
					return true;
				}
			}
			return false;
		}

		private void NarrowReachableConstraints(Microsoft.ActionMachines.Action action, TermSetBuilder constraints, Predicate<Term> validConstraintVariableChecker)
		{
			TermSetBuilder termSetBuilder = bg.MakeTermSetBuilder();
			if (action.Arguments != null)
			{
				int num = 0;
				Term[] arguments = action.Arguments;
				foreach (Term term in arguments)
				{
					if (action.Symbol.Parameters[num].Kind != ParameterKind.Receiver)
					{
						CollectVariablesAndTranslations(termSetBuilder, term);
					}
					num++;
				}
			}
			HashSet<Term> hashSet = new HashSet<Term>(constraints);
			constraints.Clear();
			Dictionary<Term, TermSetBuilder> dictionary = new Dictionary<Term, TermSetBuilder>();
			bool flag = true;
			while (flag)
			{
				flag = false;
				foreach (Term item in hashSet.ToList())
				{
					TermSetBuilder value;
					if (!dictionary.TryGetValue(item, out value))
					{
						value = bg.MakeTermSetBuilder();
						CollectVariablesAndTranslations(value, item);
						if (value.Count == 0)
						{
							hashSet.Remove(item);
							continue;
						}
						dictionary[item] = value;
					}
					bool flag2 = false;
					foreach (Term item2 in value)
					{
						if (termSetBuilder.Contains(item2) || (bg.IsVariable(item2) && validConstraintVariableChecker(item2)))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						constraints.Add(item);
						termSetBuilder.UnionWith(value);
						hashSet.Remove(item);
						flag = true;
					}
				}
			}
		}

		private void CollectVariablesAndTranslations(TermSetBuilder tsb, Term term)
		{
			IType baseType;
			uint index;
			string description;
			string baseName;
			if (bg.TryGetVariable(term, out baseType, out index, out description, out baseName))
			{
				tsb.Add(term);
				return;
			}
			Term value;
			if (bg.TryGetTranslation(term, out baseType, out value))
			{
				tsb.Add(term);
				return;
			}
			bg.DoOnAllDirectSubTerms(term, delegate(Term t)
			{
				CollectVariablesAndTranslations(tsb, t);
			});
		}

		internal State AddState(MachineState machineState, ExplorationStateFlags explorationFlags)
		{
			State value;
			if (machineStates.TryGetValue(machineState, out value))
			{
				UpdateStoppedReasonFlags(value, explorationFlags);
				return value;
			}
			stateBuilder.AddState(machineState.Data);
			bool flag = (explorationFlags & ExplorationStateFlags.IsStart) != 0;
			StateFlags flags = StateFlags.None;
			flags = flags.SetStateKindFlag(machineState.Control.Kind);
			flags = flags.SetStoppedReasonFlags(explorationFlags);
			List<Probe> list = new List<Probe>();
			foreach (ProbeEntry item in stateBuilder.BuildProbeEntries(machineState.Data))
			{
				ProbeValueKind kind = (item.IsException ? ProbeValueKind.Exception : ProbeValueKind.Normal);
				SerializableType type = item.Type.RuntimeType.ToSerializable();
				list.Add(TransitionSystem.Probe(item.Name, item.Value, type, kind));
			}
			string label = labelBuilder.AddState(machineState);
			value = TransitionSystem.State(label, false, machineState.Description, flags, list);
			if (flag)
			{
				initialStates.Add(value.Label);
			}
			allStates.Add(value);
			machineStates[machineState] = value;
			statesMap[value.Label] = value;
			return value;
		}

		private void UpdateStoppedReasonFlags(State state, ExplorationStateFlags explorationFlags)
		{
			if (explorationFlags != 0)
			{
				StateFlags flags = StateFlags.None;
				flags = flags.SetStoppedReasonFlags(explorationFlags);
				state.Flags |= flags;
			}
		}

		private ActionInvocation BuildAction(Microsoft.ActionMachines.Action action, ITermDescriptionContext context, ICompressedState compressedState)
		{
			ActionSymbolKind kind = ((action.Symbol.Kind == ActionKind.Call) ? ActionSymbolKind.Call : ((action.Symbol.Kind == ActionKind.Return) ? ActionSymbolKind.Return : ((action.Symbol.Kind == ActionKind.Throw) ? ActionSymbolKind.Throw : ((action.Symbol.Kind == ActionKind.Event && action.Symbol == preConstraintActionSymbol) ? ActionSymbolKind.PreConstraintCheck : ((action.Symbol.Kind == ActionKind.Event) ? ActionSymbolKind.Event : ActionSymbolKind.Invocation)))));
			SerializableMemberInfo serializableMemberInfo = BuildMetadata(action);
			actionMembers.Add(serializableMemberInfo);
			ActionSymbol symbol = TransitionSystem.ActionSymbol(kind, serializableMemberInfo);
			List<SerializableExpression> list = new List<SerializableExpression>();
			if (action.Arguments != null)
			{
				for (int i = 0; i < action.Arguments.Length; i++)
				{
					Term term = action.Arguments[i];
					IType type = action.Symbol.Parameters[i].Type;
					list.Add(BuildData(term, type, compressedState));
				}
			}
			return TransitionSystem.ActionInvocation(symbol, list, action.ToString(context));
		}

		private SerializableMemberInfo BuildMetadata(Microsoft.ActionMachines.Action action)
		{
			if ((action.Symbol.Kind & ActionKind.Call) != 0 && (action.Symbol.Kind & ActionKind.AllObserved) != 0)
			{
				eventAdapter.ProgressMessage(VerbosityLevel.Medium, string.Format("Action {0} is not a method invocation or an event.", action.Symbol.ToString()));
				return null;
			}
			return BuildMethod(action.Symbol);
		}

		private SerializableMemberInfo BuildMethod(IActionSymbol actionSymbol)
		{
			if (actionSymbol.CompoundActionSymbol != null)
			{
				actionSymbol = actionSymbol.CompoundActionSymbol;
			}
			bool isPublic = actionSymbol.AssociatedMember == null || actionSymbol.AssociatedMember.IsPublic;
			Type baseTypeOfDeclaringType = GetBaseTypeOfDeclaringType(actionSymbol);
			SerializableType serializableType = SerializableMemberInfo.Type(actionSymbol.ContainerName, isPublic, TypeCode.Object, (baseTypeOfDeclaringType == null) ? null : baseTypeOfDeclaringType.ToSerializable());
			if (IsAdapter(actionSymbol))
			{
				adapterTypes.Add(serializableType);
			}
			bool isStatic = actionSymbol.IsStatic;
			SerializableType serializableType2 = null;
			List<SerializableParameterInfo> list = new List<SerializableParameterInfo>();
			ActionParameter[] parameters = actionSymbol.Parameters;
			for (int i = 0; i < parameters.Length; i++)
			{
				ActionParameter actionParameter = parameters[i];
				switch (actionParameter.Kind)
				{
				case ParameterKind.In:
					list.Add(SerializableMemberInfo.Parameter(actionParameter.Type.RuntimeType.ToSerializable(), actionParameter.Name, false));
					break;
				case ParameterKind.Out:
				case ParameterKind.Ref:
					list.Add(SerializableMemberInfo.Parameter(actionParameter.Type.RuntimeType.MakeByRefType().ToSerializable(), actionParameter.Name, actionParameter.Kind != ParameterKind.Ref));
					break;
				case ParameterKind.Return:
					serializableType2 = actionParameter.Type.RuntimeType.ToSerializable();
					break;
				}
			}
			switch (actionSymbol.Kind)
			{
			case ActionKind.Call:
			case ActionKind.MethodCompound:
			{
				if (actionSymbol.IsConstructor)
				{
					return SerializableMemberInfo.Constructor(serializableType, isPublic, isStatic, list.ToArray());
				}
				Microsoft.SpecExplorer.ObjectModel.AssociationReference associationReference = null;
				IMethod method = (IMethod)actionSymbol.AssociatedMember;
				if (method != null && method.AssociationReferences != null && method.AssociationReferences.Length > 0)
				{
					SerializablePropertyInfo serializablePropertyInfo = SerializableMemberInfo.Property(serializableType, isPublic, isStatic, method.AssociationReferences[0].Association.ShortName, serializableType2);
					actionMembers.Add(serializablePropertyInfo);
					if (method.AssociationReferences[0].Kind == Microsoft.Xrt.AssociationReferenceKind.SetMethod)
					{
						Microsoft.SpecExplorer.ObjectModel.AssociationReference associationReference2 = new Microsoft.SpecExplorer.ObjectModel.AssociationReference();
						associationReference2.Association = serializablePropertyInfo;
						associationReference2.Kind = Microsoft.SpecExplorer.ObjectModel.AssociationReferenceKind.SetMethod;
						associationReference = associationReference2;
					}
					else
					{
						Microsoft.SpecExplorer.ObjectModel.AssociationReference associationReference3 = new Microsoft.SpecExplorer.ObjectModel.AssociationReference();
						associationReference3.Association = serializablePropertyInfo;
						associationReference3.Kind = Microsoft.SpecExplorer.ObjectModel.AssociationReferenceKind.GetMethod;
						associationReference = associationReference3;
					}
				}
				return SerializableMemberInfo.Method(serializableType, isPublic, isStatic, actionSymbol.Name, null, list.ToArray(), serializableType2, associationReference);
			}
			case ActionKind.Event:
				return SerializableMemberInfo.Event(serializableType, isPublic, isStatic, actionSymbol.Name, list.ToArray(), serializableType2, (actionSymbol == preConstraintActionSymbol) ? true : false);
			default:
				throw host.FatalError("Unexpected action symbol");
			}
		}

		private bool IsAdapter(IActionSymbol symbol)
		{
			IMember associatedMember = symbol.AssociatedMember;
			if (associatedMember == null)
			{
				return false;
			}
			if (associatedMember is IMethod)
			{
				IMethod method = (IMethod)associatedMember;
				return method.DeclaringType.IsAdapter;
			}
			IAssociation association = associatedMember as IAssociation;
			return association.FireMethod.DeclaringType.IsAdapter;
		}

		private Type GetBaseTypeOfDeclaringType(IActionSymbol symbol)
		{
			IMember associatedMember = symbol.AssociatedMember;
			if (associatedMember == null)
			{
				return null;
			}
			if (associatedMember is IMethod)
			{
				IMethod method = (IMethod)associatedMember;
				if (method.DeclaringType.BaseType != null)
				{
					return method.DeclaringType.BaseType.RuntimeType;
				}
				return null;
			}
			IAssociation association = associatedMember as IAssociation;
			if (association.FireMethod.DeclaringType.BaseType != null)
			{
				return association.FireMethod.DeclaringType.BaseType.RuntimeType;
			}
			return null;
		}

		private SerializableExpression BuildData(Term term, IType contextType, ICompressedState compressedState)
		{
			if (term == Term.Undef)
			{
				return BuildPlaceholder(contextType);
			}
			ExpressionCacheKey expressionCacheKey = default(ExpressionCacheKey);
			expressionCacheKey.term = term;
			expressionCacheKey.type = contextType;
			ExpressionCacheKey expressionCacheKey2 = expressionCacheKey;
			SerializableExpression value;
			if (term != Term.Undef && serializableExpressionCache.TryGetValue(expressionCacheKey2, out value))
			{
				return value;
			}
			CollectVariables(term);
			ParameterExpression value2;
			Expression expression = bg.ToExpression((Term t) => variableMap.TryGetValue(t, out value2) ? value2 : null, contextType, term, compressedState);
			if (!serializableExpressionCache.TryGetValue(expressionCacheKey2, out value))
			{
				return AddToplevelExpression(expressionCacheKey2, expression);
			}
			return value;
		}

		private void CollectVariables(Term term)
		{
			if (variableMap.ContainsKey(term))
			{
				return;
			}
			IType baseType;
			uint index;
			string description;
			string baseName;
			if (bg.TryGetVariable(term, out baseType, out index, out description, out baseName))
			{
				BuildVariable(term, baseType, description);
				return;
			}
			if (bg.TryGetObjectReference(term, out baseType, out index))
			{
				if (!bg.GetType(term).IsBoxType)
				{
					BuildVariable(term, baseType, "o" + index);
				}
				return;
			}
			Term value;
			if (bg.TryGetTranslation(term, out baseType, out value))
			{
				BuildVariable(term, bg.GetType(term), "o");
				return;
			}
			Term[] directSubTerms = bg.GetDirectSubTerms(term);
			foreach (Term term2 in directSubTerms)
			{
				CollectVariables(term2);
			}
		}

		private void BuildVariable(Term term, IType type, string description)
		{
			string text = MakeUniqueVarName(description);
			ParameterExpression parameterExpression = Expression.Parameter(type.RepresentationType.RuntimeType, text);
			SerializableParameterExpression serializableParameterExpression = (SerializableParameterExpression)AddToplevelExpression(new ExpressionCacheKey
			{
				term = term,
				type = type.RepresentationType
			}, parameterExpression);
			variables.Add(serializableParameterExpression);
			variableMap[term] = parameterExpression;
			variableSerializableMap[term] = serializableParameterExpression;
		}

		private SerializableParameterExpression BuildPlaceholder(IType type)
		{
			string text = MakeUniqueVarName("_");
			ParameterExpression expression = Expression.Parameter(type.RuntimeType, text);
			SerializableParameterExpression serializableParameterExpression = (SerializableParameterExpression)AddToplevelExpression(new ExpressionCacheKey
			{
				term = Term.Undef,
				type = type
			}, expression);
			variables.Add(serializableParameterExpression);
			return serializableParameterExpression;
		}

		private string MakeUniqueVarName(string prefix)
		{
			string text = prefix;
			int num = 1;
			while (usedVariableNames.Contains(text))
			{
				text = prefix + num;
				num++;
			}
			usedVariableNames.Add(text);
			return text;
		}

		private SerializableExpression AddToplevelExpression(ExpressionCacheKey ckey, Expression expression)
		{
			SerializableExpression serializableExpression = expression.ToSerializable();
			string text = serializableExpression.ToString();
			if (usedExpressionKeys.Contains(text))
			{
				text = string.Format("{1}(#{0})", serializableExpressionCache.Count, text);
			}
			usedExpressionKeys.Add(text);
			serializableExpression.Key = text;
			if (ckey.term != Term.Undef)
			{
				serializableExpressionCache[ckey] = serializableExpression;
			}
			return serializableExpression;
		}
	}
}
