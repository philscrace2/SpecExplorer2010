// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.TransitionSystemBuilder
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;
using Microsoft.Xrt.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.SpecExplorer
{
  internal class TransitionSystemBuilder
  {
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
    private Dictionary<TransitionSystemBuilder.ExpressionCacheKey, SerializableExpression> serializableExpressionCache = new Dictionary<TransitionSystemBuilder.ExpressionCacheKey, SerializableExpression>();
    private HashSet<string> usedExpressionKeys = new HashSet<string>();
    private IBackground bg;
    private IProgram program;
    private IStatePersistenceBuilder stateBuilder;
    private ITermDescriptionContextFactory contextFactory;
    private EventAdapter eventAdapter;
    private IActionSymbol preConstraintActionSymbol;

    internal TransitionSystemBuilder(
      string name,
      IConfiguration config,
      ComponentBase host,
      EventAdapter eventAdapter)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (config == null)
        throw new ArgumentNullException(nameof (config));
      if (host == null)
        throw new ArgumentNullException(nameof (host));
      if (eventAdapter == null)
        throw new ArgumentNullException(nameof (eventAdapter));
      this.host = host;
      this.name = name;
      this.config = config;
      this.eventAdapter = eventAdapter;
      IOptionSetManager requiredService = host.GetRequiredService<IOptionSetManager>();
      IOptionSet optionSet = this.config.OptionSet;
      foreach (PropertyDescriptor property in requiredService.GetProperties(Visibility.Public))
      {
        string switchValue = TransitionSystemBuilder.GetSwitchValue(optionSet, property);
        this.configSwitches.Add(TransitionSystem.ConfigSwitch(property.Name, switchValue));
      }
      this.bg = host.GetRequiredService<IBackground>();
      this.contextFactory = host.GetRequiredService<ITermDescriptionContextFactory>();
      this.program = host.GetRequiredService<IProgram>();
      this.stateBuilder = host.GetRequiredService<IStatePersistenceBuilderProvider>().CreateStatePersistencyBuilder();
      this.labelBuilder = new StateLabelBuilder();
      this.preConstraintActionSymbol = host.GetRequiredService<IConfigurationProvider>().PreConstraintActionSymbol;
    }

    private static string GetSwitchValue(IOptionSet optionSet, PropertyDescriptor p)
    {
      string str;
      if (p.Converter != null)
      {
        try
        {
          str = p.Converter.ConvertToString(optionSet.GetValue(p));
        }
        catch (Exception ex)
        {
          str = optionSet.GetValue(p).ToString();
        }
      }
      else
        str = optionSet.GetValue(p).ToString();
      return str;
    }

    internal ExplorationResult BuildTransitionSystem(bool withStateContent = true)
    {
      foreach (IActionSymbol action in (IEnumerable<IActionSymbol>) this.config.Actions)
        this.actionMembers.Add(this.BuildMethod(action));
      TransitionSystem transitionSystem = TransitionSystem.Create(this.name, (IEnumerable<ConfigSwitch>) ((IEnumerable<ConfigSwitch>) this.configSwitches).OrderBy<ConfigSwitch, string>((Func<ConfigSwitch, string>) (sw => sw.Name)), (IEnumerable<SerializableMemberInfo>) ((IEnumerable<SerializableMemberInfo>) this.actionMembers).OrderBy<SerializableMemberInfo, string>((Func<SerializableMemberInfo, string>) (m => m.Header)), (IEnumerable<SerializableType>) ((IEnumerable<SerializableType>) this.adapterTypes).OrderBy<SerializableType, string>((Func<SerializableType, string>) (t => ((SerializableMemberInfo) t).Header)), (IEnumerable<SerializableParameterExpression>) ((IEnumerable<SerializableParameterExpression>) this.variables).OrderBy<SerializableParameterExpression, string>((Func<SerializableParameterExpression, string>) (v => v.Name)), (IEnumerable<string>) this.initialStates.OrderBy<string, string>((Func<string, string>) (s => s)), (IEnumerable<State>) ((IEnumerable<State>) this.allStates).OrderBy<State, string>((Func<State, string>) (s => s.Label)), (IEnumerable<Transition>) ((IEnumerable<Transition>) this.transitions).OrderBy<Transition, string>((Func<Transition, string>) (t => t.Source)).ThenBy<Transition, string>((Func<Transition, string>) (t => t.Action.Symbol.Member.Header)).ThenBy<Transition, string>((Func<Transition, string>) (t => t.Target)));
      if (!withStateContent)
        return new ExplorationResult(transitionSystem, new SharedEntitySet(), Enumerable.Empty<StateEntity>(), new ExplorationResultExtensions());
      this.stateBuilder.BuildStateObjectModel();
      List<StateEntity> stateEntityList = new List<StateEntity>();
      foreach (KeyValuePair<MachineState, State> machineState in this.machineStates)
      {
        StateEntity stateEntity = new StateEntity(machineState.Value.Label, machineState.Value.Flags, machineState.Value.Description, this.stateBuilder.GetStateContent(machineState.Key.Data));
        stateEntityList.Add(stateEntity);
      }
      SharedEntitySet sharedEntitySet = this.stateBuilder.SharedEntitySet;
      return new ExplorationResult(transitionSystem, sharedEntitySet, (IEnumerable<StateEntity>) stateEntityList, new ExplorationResultExtensions());
    }

    internal void AddStep(
      ExplorationStep step,
      MachineState? subsumptionRelatedState,
      SubsumptionResult relatedStateRelationKind)
    {
      State state1 = this.AddState(step.SourceState, step.SourceFlags);
      State state2 = this.AddState(step.TargetState, step.TargetFlags);
      if (subsumptionRelatedState.HasValue)
      {
        State state3 = state2;
        State state4 = this.AddState(subsumptionRelatedState.Value, step.TargetFlags);
        if (relatedStateRelationKind == SubsumptionResult.ThisSubsumesOther)
        {
          State state5 = state3;
          state3 = state4;
          state4 = state5;
        }
        state3.RepresentativeState = state4.Label;
        state3.RelationKind = (StateRelationKind) 1;
      }
      ITermDescriptionContext context = this.contextFactory.CreateContext(TermDescriptionFlags.DefaultUserDescription | TermDescriptionFlags.ReEscapingInString, step.IntermediateTargetState.Data);
      ActionInvocation actionInvocation = this.BuildAction(step.Action, context, step.TargetState.Data);
      string[] strArray1;
      string[] strArray2;
      if (step.Requirements == null)
      {
        strArray1 = new string[0];
        strArray2 = new string[0];
      }
      else
      {
        strArray1 = step.Requirements.CapturedRequirements.ToArray<string>();
        strArray2 = step.Requirements.AssumeCapturedRequirements.ToArray<string>();
      }
      IOrderedEnumerable<Constraint> orderedEnumerable1 = this.FilterAndConvertConstraints((IEnumerable<Term>) this.CalculatePostConstraints(step)).Select<Term, Constraint>((Func<Term, Constraint>) (t => TransitionSystem.Constraint(this.BuildData(t, this.program.SystemTypes.Boolean, step.TargetState.Data), context.ToString(this.program.SystemTypes.Boolean, t)))).OrderBy<Constraint, string>((Func<Constraint, string>) (c => c.Text));
      IOrderedEnumerable<Constraint> orderedEnumerable2 = this.FilterAndConvertConstraints((IEnumerable<Term>) step.PreConstraints).Select<Term, Constraint>((Func<Term, Constraint>) (t => TransitionSystem.Constraint(this.BuildData(t, this.program.SystemTypes.Boolean, step.TargetState.Data), context.ToString(this.program.SystemTypes.Boolean, t)))).OrderBy<Constraint, string>((Func<Constraint, string>) (c => c.Text));
      TermSetBuilder source = new TermSetBuilder(step.FreedVariables);
      source.IntersectWith((IEnumerable<Term>) this.variableMap.Keys);
      IOrderedEnumerable<SerializableParameterExpression> orderedEnumerable3 = source.Select<Term, SerializableParameterExpression>((Func<Term, SerializableParameterExpression>) (v => this.variableSerializableMap[v])).OrderBy<SerializableParameterExpression, string>((Func<SerializableParameterExpression, string>) (v => v.Name));
      this.transitions.Add(TransitionSystem.Transition(state1.Label, actionInvocation, state2.Label, (IEnumerable<Constraint>) orderedEnumerable2, (IEnumerable<Constraint>) orderedEnumerable1, (IEnumerable<SerializableParameterExpression>) orderedEnumerable3, (IEnumerable<string>) strArray1, (IEnumerable<string>) strArray2));
    }

    private TermSetBuilder CalculatePostConstraints(ExplorationStep step)
    {
      TermSet context1 = step.SourceState.Data.Context;
      TermSet context2 = step.IntermediateTargetState.Data.Context;
      TermSetBuilder constraints = this.bg.MakeTermSetBuilder();
      if (step.Action.Symbol.Kind == ActionKind.Return || step.Action.Symbol.Kind == ActionKind.Event && step.Action.Symbol != this.preConstraintActionSymbol)
      {
        constraints.UnionWith((IEnumerable<Term>) context2);
        constraints.ExceptWith((IEnumerable<Term>) context1);
        ICompressedState csource = step.SourceState.Data;
        ICompressedState ctarget = step.IntermediateTargetState.Data;
        IActiveState asource = csource.Uncompress();
        IActiveState atarget = ctarget.Uncompress();
        foreach (Term term in context1)
        {
          if (this.bg.FindInTerms((Predicate<Term>) (t => this.bg.IsVariable(t) && !csource.IsExistential(t) && (!asource.IsVariableBound(t) && !ctarget.IsExistential(t)) && atarget.IsVariableBound(t)), term))
            constraints.Add(term);
        }
        if (constraints.Count > 0)
          this.NarrowReachableConstraints(step.Action, constraints, new Predicate<Term>(atarget.IsVariableBound));
      }
      return constraints;
    }

    private List<Term> FilterAndConvertConstraints(IEnumerable<Term> constraints)
    {
      List<Term> termList = new List<Term>();
      foreach (Term constraint in constraints)
      {
        if (this.bg.IsDomainPredicate(constraint))
        {
          Term explicitConstraint = this.bg.ConvertDomainPredicateToExplicitConstraint(constraint);
          if (explicitConstraint != this.bg.True)
            termList.Add(explicitConstraint);
        }
        else if (!this.bg.IsFunctionApplication(constraint))
          termList.Add(constraint);
      }
      return termList;
    }

    private bool HasCycle(State thisState, State otherState)
    {
      State state = otherState;
      while (state.RepresentativeState != null && this.statesMap.TryGetValue(state.RepresentativeState, out state))
      {
        if (state.Label == thisState.Label)
          return true;
      }
      return false;
    }

    private void NarrowReachableConstraints(
      Microsoft.ActionMachines.Action action,
      TermSetBuilder constraints,
      Predicate<Term> validConstraintVariableChecker)
    {
      TermSetBuilder tsb1 = this.bg.MakeTermSetBuilder();
      if (action.Arguments != null)
      {
        int index = 0;
        foreach (Term term in action.Arguments)
        {
          if (action.Symbol.Parameters[index].Kind != ParameterKind.Receiver)
            this.CollectVariablesAndTranslations(tsb1, term);
          ++index;
        }
      }
      HashSet<Term> source = new HashSet<Term>((IEnumerable<Term>) constraints);
      constraints.Clear();
      Dictionary<Term, TermSetBuilder> dictionary = new Dictionary<Term, TermSetBuilder>();
      bool flag1 = true;
      while (flag1)
      {
        flag1 = false;
        foreach (Term term1 in source.ToList<Term>())
        {
          TermSetBuilder tsb2;
          if (!dictionary.TryGetValue(term1, out tsb2))
          {
            tsb2 = this.bg.MakeTermSetBuilder();
            this.CollectVariablesAndTranslations(tsb2, term1);
            if (tsb2.Count == 0)
            {
              source.Remove(term1);
              continue;
            }
            dictionary[term1] = tsb2;
          }
          bool flag2 = false;
          foreach (Term term2 in (HashSet<Term>) tsb2)
          {
            if (tsb1.Contains(term2) || this.bg.IsVariable(term2) && validConstraintVariableChecker(term2))
            {
              flag2 = true;
              break;
            }
          }
          if (flag2)
          {
            constraints.Add(term1);
            tsb1.UnionWith((IEnumerable<Term>) tsb2);
            source.Remove(term1);
            flag1 = true;
          }
        }
      }
    }

    private void CollectVariablesAndTranslations(TermSetBuilder tsb, Term term)
    {
      IType type;
      if (this.bg.TryGetVariable(term, out type, out uint _, out string _, out string _))
        tsb.Add(term);
      else if (this.bg.TryGetTranslation(term, out type, out Term _))
        tsb.Add(term);
      else
        this.bg.DoOnAllDirectSubTerms(term, (System.Action<Term>) (t => this.CollectVariablesAndTranslations(tsb, t)));
    }

    internal State AddState(MachineState machineState, ExplorationStateFlags explorationFlags)
    {
      State state1;
      if (this.machineStates.TryGetValue(machineState, out state1))
      {
        this.UpdateStoppedReasonFlags(state1, explorationFlags);
        return state1;
      }
      this.stateBuilder.AddState(machineState.Data);
      bool flag = (explorationFlags & ExplorationStateFlags.IsStart) != ExplorationStateFlags.None;
      StateFlags stateFlags = ((StateFlags) 0).SetStateKindFlag(machineState.Control.Kind).SetStoppedReasonFlags(explorationFlags);
      List<Probe> probeList = new List<Probe>();
      foreach (ProbeEntry buildProbeEntry in this.stateBuilder.BuildProbeEntries(machineState.Data))
      {
        ProbeValueKind probeValueKind = buildProbeEntry.IsException ? (ProbeValueKind) 1 : (ProbeValueKind) 0;
        SerializableType serializable = ObjectModelHelpers.ToSerializable(buildProbeEntry.Type.RuntimeType);
        probeList.Add(TransitionSystem.Probe(buildProbeEntry.Name, buildProbeEntry.Value, serializable, probeValueKind));
      }
      State state2 = TransitionSystem.State(this.labelBuilder.AddState(machineState), false, machineState.Description, stateFlags, (IEnumerable<Probe>) probeList);
      if (flag)
        this.initialStates.Add(state2.Label);
      this.allStates.Add(state2);
      this.machineStates[machineState] = state2;
      this.statesMap[state2.Label] = state2;
      return state2;
    }

    private void UpdateStoppedReasonFlags(State state, ExplorationStateFlags explorationFlags)
    {
      if (explorationFlags == ExplorationStateFlags.None)
        return;
      StateFlags stateFlags = ((StateFlags) 0).SetStoppedReasonFlags(explorationFlags);
      State state1 = state;
      state1.Flags = state1.Flags | stateFlags;
    }

    private ActionInvocation BuildAction(
      Microsoft.ActionMachines.Action action,
      ITermDescriptionContext context,
      ICompressedState compressedState)
    {
      ActionSymbolKind actionSymbolKind = action.Symbol.Kind != ActionKind.Call ? (action.Symbol.Kind != ActionKind.Return ? (action.Symbol.Kind != ActionKind.Throw ? (action.Symbol.Kind != ActionKind.Event || action.Symbol != this.preConstraintActionSymbol ? (action.Symbol.Kind != ActionKind.Event ? (ActionSymbolKind) 0 : (ActionSymbolKind) 4) : (ActionSymbolKind) 5) : (ActionSymbolKind) 3) : (ActionSymbolKind) 2) : (ActionSymbolKind) 1;
      SerializableMemberInfo serializableMemberInfo = this.BuildMetadata(action);
      this.actionMembers.Add(serializableMemberInfo);
      ActionSymbol actionSymbol = TransitionSystem.ActionSymbol(actionSymbolKind, serializableMemberInfo);
      List<SerializableExpression> serializableExpressionList = new List<SerializableExpression>();
      if (action.Arguments != null)
      {
        for (int index = 0; index < action.Arguments.Length; ++index)
        {
          Term term = action.Arguments[index];
          IType type = action.Symbol.Parameters[index].Type;
          serializableExpressionList.Add(this.BuildData(term, type, compressedState));
        }
      }
      return TransitionSystem.ActionInvocation(actionSymbol, (IEnumerable<SerializableExpression>) serializableExpressionList, action.ToString(context));
    }

    private SerializableMemberInfo BuildMetadata(Microsoft.ActionMachines.Action action)
    {
      if ((action.Symbol.Kind & ActionKind.Call) == (ActionKind) 0 || (action.Symbol.Kind & ActionKind.AllObserved) == (ActionKind) 0)
        return this.BuildMethod(action.Symbol);
      this.eventAdapter.ProgressMessage(VerbosityLevel.Medium, string.Format("Action {0} is not a method invocation or an event.", (object) action.Symbol.ToString()));
      return (SerializableMemberInfo) null;
    }

    private SerializableMemberInfo BuildMethod(IActionSymbol actionSymbol)
    {
      if (actionSymbol.CompoundActionSymbol != null)
        actionSymbol = actionSymbol.CompoundActionSymbol;
      bool flag = actionSymbol.AssociatedMember == null || actionSymbol.AssociatedMember.IsPublic;
      System.Type typeOfDeclaringType = this.GetBaseTypeOfDeclaringType(actionSymbol);
      SerializableType serializableType1 = SerializableMemberInfo.Type(actionSymbol.ContainerName, flag, TypeCode.Object, typeOfDeclaringType == (System.Type) null ? (SerializableType) null : ObjectModelHelpers.ToSerializable(typeOfDeclaringType));
      if (this.IsAdapter(actionSymbol))
        this.adapterTypes.Add(serializableType1);
      bool isStatic = actionSymbol.IsStatic;
      SerializableType serializableType2 = (SerializableType) null;
      List<SerializableParameterInfo> serializableParameterInfoList = new List<SerializableParameterInfo>();
      foreach (ActionParameter parameter in actionSymbol.Parameters)
      {
        switch (parameter.Kind)
        {
          case ParameterKind.In:
            serializableParameterInfoList.Add(SerializableMemberInfo.Parameter(ObjectModelHelpers.ToSerializable(parameter.Type.RuntimeType), parameter.Name, false));
            break;
          case ParameterKind.Out:
          case ParameterKind.Ref:
            serializableParameterInfoList.Add(SerializableMemberInfo.Parameter(ObjectModelHelpers.ToSerializable(parameter.Type.RuntimeType.MakeByRefType()), parameter.Name, parameter.Kind != ParameterKind.Ref));
            break;
          case ParameterKind.Return:
            serializableType2 = ObjectModelHelpers.ToSerializable(parameter.Type.RuntimeType);
            break;
        }
      }
      switch (actionSymbol.Kind)
      {
        case ActionKind.Call:
        case ActionKind.MethodCompound:
          if (actionSymbol.IsConstructor)
            return (SerializableMemberInfo) SerializableMemberInfo.Constructor(serializableType1, flag, isStatic, serializableParameterInfoList.ToArray());
          AssociationReference associationReference = (AssociationReference) null;
          IMethod associatedMember = (IMethod) actionSymbol.AssociatedMember;
          if (associatedMember != null && associatedMember.AssociationReferences != null && associatedMember.AssociationReferences.Length > 0)
          {
            SerializablePropertyInfo serializablePropertyInfo = SerializableMemberInfo.Property(serializableType1, flag, isStatic, associatedMember.AssociationReferences[0].Association.ShortName, serializableType2);
            this.actionMembers.Add((SerializableMemberInfo) serializablePropertyInfo);
            if (associatedMember.AssociationReferences[0].Kind == AssociationReferenceKind.SetMethod)
              associationReference = new AssociationReference()
              {
                Association = (SerializableMemberInfo) serializablePropertyInfo,
                Kind = (AssociationReferenceKind) 1
              };
            else
              associationReference = new AssociationReference()
              {
                Association = (SerializableMemberInfo) serializablePropertyInfo,
                Kind = (AssociationReferenceKind) 0
              };
          }
          return (SerializableMemberInfo) SerializableMemberInfo.Method(serializableType1, flag, isStatic, actionSymbol.Name, (SerializableType[]) null, serializableParameterInfoList.ToArray(), serializableType2, associationReference);
        case ActionKind.Event:
          return (SerializableMemberInfo) SerializableMemberInfo.Event(serializableType1, flag, isStatic, actionSymbol.Name, serializableParameterInfoList.ToArray(), serializableType2, actionSymbol == this.preConstraintActionSymbol);
        default:
          throw this.host.FatalError("Unexpected action symbol");
      }
    }

    private bool IsAdapter(IActionSymbol symbol)
    {
      IMember associatedMember = symbol.AssociatedMember;
      if (associatedMember == null)
        return false;
      return associatedMember is IMethod ? ((IMethod) associatedMember).DeclaringType.IsAdapter : (associatedMember as IAssociation).FireMethod.DeclaringType.IsAdapter;
    }

    private System.Type GetBaseTypeOfDeclaringType(IActionSymbol symbol)
    {
      IMember associatedMember = symbol.AssociatedMember;
      if (associatedMember == null)
        return (System.Type) null;
      if (associatedMember is IMethod)
      {
        IMethod method = (IMethod) associatedMember;
        return method.DeclaringType.BaseType != null ? method.DeclaringType.BaseType.RuntimeType : (System.Type) null;
      }
      IAssociation association = associatedMember as IAssociation;
      return association.FireMethod.DeclaringType.BaseType != null ? association.FireMethod.DeclaringType.BaseType.RuntimeType : (System.Type) null;
    }

    private SerializableExpression BuildData(
      Term term,
      IType contextType,
      ICompressedState compressedState)
    {
      if (term == Term.Undef)
        return (SerializableExpression) this.BuildPlaceholder(contextType);
      TransitionSystemBuilder.ExpressionCacheKey expressionCacheKey = new TransitionSystemBuilder.ExpressionCacheKey()
      {
        term = term,
        type = contextType
      };
      SerializableExpression serializableExpression;
      if (term != Term.Undef && this.serializableExpressionCache.TryGetValue(expressionCacheKey, out serializableExpression))
        return serializableExpression;
      this.CollectVariables(term);
      ParameterExpression parameterExpression;
      System.Linq.Expressions.Expression expression = this.bg.ToExpression((Func<Term, System.Linq.Expressions.Expression>) (t => this.variableMap.TryGetValue(t, out parameterExpression) ? (System.Linq.Expressions.Expression) parameterExpression : (System.Linq.Expressions.Expression) null), contextType, term, compressedState);
      if (!this.serializableExpressionCache.TryGetValue(expressionCacheKey, out serializableExpression))
        serializableExpression = this.AddToplevelExpression(expressionCacheKey, expression);
      return serializableExpression;
    }

    private void CollectVariables(Term term)
    {
      if (this.variableMap.ContainsKey(term))
        return;
      IType type;
      uint index;
      string description;
      if (this.bg.TryGetVariable(term, out type, out index, out description, out string _))
        this.BuildVariable(term, type, description);
      else if (this.bg.TryGetObjectReference(term, out type, out index))
      {
        if (this.bg.GetType(term).IsBoxType)
          return;
        this.BuildVariable(term, type, "o" + index.ToString());
      }
      else if (this.bg.TryGetTranslation(term, out type, out Term _))
      {
        this.BuildVariable(term, this.bg.GetType(term), "o");
      }
      else
      {
        foreach (Term directSubTerm in this.bg.GetDirectSubTerms(term))
          this.CollectVariables(directSubTerm);
      }
    }

    private void BuildVariable(Term term, IType type, string description)
    {
      string name = this.MakeUniqueVarName(description);
      ParameterExpression parameterExpression1 = System.Linq.Expressions.Expression.Parameter(type.RepresentationType.RuntimeType, name);
      SerializableParameterExpression parameterExpression2 = (SerializableParameterExpression) this.AddToplevelExpression(new TransitionSystemBuilder.ExpressionCacheKey()
      {
        term = term,
        type = type.RepresentationType
      }, (System.Linq.Expressions.Expression) parameterExpression1);
      this.variables.Add(parameterExpression2);
      this.variableMap[term] = parameterExpression1;
      this.variableSerializableMap[term] = parameterExpression2;
    }

    private SerializableParameterExpression BuildPlaceholder(
      IType type)
    {
      string name = this.MakeUniqueVarName("_");
      SerializableParameterExpression parameterExpression = (SerializableParameterExpression) this.AddToplevelExpression(new TransitionSystemBuilder.ExpressionCacheKey()
      {
        term = Term.Undef,
        type = type
      }, (System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Parameter(type.RuntimeType, name));
      this.variables.Add(parameterExpression);
      return parameterExpression;
    }

    private string MakeUniqueVarName(string prefix)
    {
      string str = prefix;
      int num = 1;
      while (this.usedVariableNames.Contains(str))
      {
        str = prefix + (object) num;
        ++num;
      }
      this.usedVariableNames.Add(str);
      return str;
    }

    private SerializableExpression AddToplevelExpression(
      TransitionSystemBuilder.ExpressionCacheKey ckey,
      System.Linq.Expressions.Expression expression)
    {
      SerializableExpression serializable = ObjectModelHelpers.ToSerializable(expression);
      string str = ((object) serializable).ToString();
      if (this.usedExpressionKeys.Contains(str))
        str = string.Format("{1}(#{0})", (object) this.serializableExpressionCache.Count, (object) str);
      this.usedExpressionKeys.Add(str);
      serializable.Key = str;
      if (ckey.term != Term.Undef)
        this.serializableExpressionCache[ckey] = serializable;
      return serializable;
    }

    private struct ExpressionCacheKey
    {
      internal Term term;
      internal IType type;
    }
  }
}
