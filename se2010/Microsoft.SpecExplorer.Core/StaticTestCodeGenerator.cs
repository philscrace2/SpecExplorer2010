// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.StaticTestCodeGenerator
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer
{
  public class StaticTestCodeGenerator : TestCodeGenerateBase
  {
    private const string ReRunsSwitchName = "ReRuns";
    private int rerunTimes;
    private Dictionary<string, CodeMemberMethod> sharedStateMethods = new Dictionary<string, CodeMemberMethod>();

    public StaticTestCodeGenerator(IHost host, TransitionSystem transitionSystem)
      : base(host, transitionSystem)
    {
    }

    public override string GenerateTestCode(string machineName)
    {
      this.MachineName = machineName;
      this.logProbesHelper.CheckLogProbesSwitchValue(this.transitionSystem, this.host, this.MachineName);
      string s = this.transitionSystem.GetSwitch("ReRuns");
      this.rerunTimes = string.IsNullOrEmpty(s) ? 1 : int.Parse(s);
      return this.InternalGenerateTestCode();
    }

    public override CodeTypeDeclaration GenerateTestClass()
    {
      string generatedTestClassName;
      string generateStaticTestMethods;
      this.PreprocessInnerTestClass(out generatedTestClassName, out generateStaticTestMethods);
      foreach (Node<State> startNode in this.graph.StartNodes)
        this.GenerateTestMethod(startNode, generatedTestClassName);
      return this.GenerateInnerTestClass(generatedTestClassName, generateStaticTestMethods, this.GenerateConstructor(), new CodeTypeMemberCollection(), (CodeStatement) null);
    }

    private void GenerateTestMethod(Node<State> startNode, string generatedTestClassName)
    {
      this.instanceEventRemovalStatements.Clear();
      State label = startNode.Label;
      if (!this.testCaseName.IsNoneOrEmptyValue())
        this.currentTestMethodName = this.MakeUnique(this.variableResolver.Resolve("testcasename", label.Label));
      else
        this.currentTestMethodName = this.MakeUnique(generatedTestClassName + label.Label);
      CodeMemberMethod testMethod = new CodeMemberMethod();
      testMethod.Name = this.currentTestMethodName;
      testMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
      testMethod.StartDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.Start, string.Format("Test Starting in {0}", (object) label.Label)));
      this.testMethodCodeCollection.Add((CodeTypeMember) testMethod);
      this.currentLastTestHelperMethod = testMethod;
      CodeStatementCollection statementCollection = new CodeStatementCollection();
      this.AddState(statementCollection, startNode);
      string methodReturnValue = this.GenerateTestMethodReturnValue(testMethod, this.WrapTestMethodStatementsWithRerunLoop(this.rerunTimes, statementCollection), false);
      this.GenerateStaticTestMethod(this.testAttributeProvider.CreateTestCaseAttributes(label.Label).ToArray<CodeAttributeDeclaration>(), testMethod, methodReturnValue);
      this.currentLastTestHelperMethod.EndDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.End, ""));
      this.currentLastTestHelperMethod = (CodeMemberMethod) null;
      this.currentTestMethodName = (string) null;
    }

    private CodeStatementCollection WrapTestMethodStatementsWithRerunLoop(
      int rerunTimes,
      CodeStatementCollection currentStatements)
    {
      if (rerunTimes <= 1)
        return currentStatements;
      CodeVariableReferenceExpression referenceExpression = new CodeVariableReferenceExpression(nameof (rerunTimes));
      CodeIterationStatement iterationStatement = new CodeIterationStatement((CodeStatement) new CodeVariableDeclarationStatement(typeof (int), nameof (rerunTimes), (CodeExpression) new CodePrimitiveExpression((object) 1)), (CodeExpression) new CodeBinaryOperatorExpression((CodeExpression) referenceExpression, CodeBinaryOperatorType.LessThanOrEqual, (CodeExpression) new CodePrimitiveExpression((object) rerunTimes)), (CodeStatement) new CodeAssignStatement((CodeExpression) referenceExpression, (CodeExpression) new CodeBinaryOperatorExpression((CodeExpression) referenceExpression, CodeBinaryOperatorType.Add, (CodeExpression) new CodePrimitiveExpression((object) 1))), new CodeStatement[0]);
      iterationStatement.Statements.Add((CodeStatement) new CodeConditionStatement((CodeExpression) new CodeBinaryOperatorExpression((CodeExpression) referenceExpression, CodeBinaryOperatorType.IdentityInequality, (CodeExpression) new CodePrimitiveExpression((object) 1)), new CodeStatement[1]
      {
        (CodeStatement) new CodeExpressionStatement((CodeExpression) new CodeMethodInvokeExpression(TestCodeGenerateBase.MakeThis(), "TestInitialize", new CodeExpression[0]))
      }));
      if (string.Compare("true", this.suppressGeneratedTestLogging, true) != 0)
        this.AddComment(iterationStatement.Statements, (CodeExpression) new CodeMethodInvokeExpression((CodeExpression) null, "String.Format", new CodeExpression[2]
        {
          (CodeExpression) new CodePrimitiveExpression((object) "Rerun time: {0}"),
          (CodeExpression) referenceExpression
        }));
      iterationStatement.Statements.AddRange(currentStatements);
      iterationStatement.Statements.Add((CodeStatement) new CodeConditionStatement((CodeExpression) new CodeBinaryOperatorExpression((CodeExpression) referenceExpression, CodeBinaryOperatorType.IdentityInequality, (CodeExpression) new CodePrimitiveExpression((object) rerunTimes)), new CodeStatement[1]
      {
        (CodeStatement) new CodeExpressionStatement((CodeExpression) new CodeMethodInvokeExpression(TestCodeGenerateBase.MakeThis(), "TestCleanup", new CodeExpression[0]))
      }));
      return new CodeStatementCollection()
      {
        (CodeStatement) iterationStatement
      };
    }

    private void AddState(CodeStatementCollection statements, Node<State> node)
    {
      State label1 = node.Label;
      CodeMemberMethod codeMemberMethod;
      if (this.sharedStateMethods.TryGetValue(label1.Label, out codeMemberMethod))
      {
        statements.Add((CodeStatement) new CodeExpressionStatement((CodeExpression) new CodeMethodInvokeExpression((CodeExpression) null, codeMemberMethod.Name, new CodeExpression[0])));
      }
      else
      {
        List<CodeExpression> expects1;
        List<CodeStatementCollection> expectContinuations1;
        List<CodeExpression> expects2;
        List<CodeExpression> expects3;
        List<CodeStatementCollection> expectContinuations2;
        List<CodeStatementCollection> expectContinuations3;
        CodeStatementCollection statements1;
        Node<State> contNode;
        while (true)
        {
          int num = this.graph.IncomingCount(node);
          if (num > 1 || num == 1 && ((IEnumerable<string>) this.transitionSystem.InitialStates).Contains<string>(label1.Label))
          {
            if (!this.sharedStateMethods.TryGetValue(label1.Label, out codeMemberMethod))
            {
              codeMemberMethod = new CodeMemberMethod();
              if (((IEnumerable<string>) this.transitionSystem.InitialStates).Contains<string>(label1.Label))
                codeMemberMethod.Name = this.MakeUnique(this.testLogicClassName + label1.Label + "Shared");
              else
                codeMemberMethod.Name = this.MakeUnique(this.testLogicClassName + label1.Label);
              this.testMethodCodeCollection.Add((CodeTypeMember) codeMemberMethod);
              this.currentLastTestHelperMethod = codeMemberMethod;
              this.sharedStateMethods[label1.Label] = codeMemberMethod;
              statements.Add((CodeStatement) new CodeExpressionStatement((CodeExpression) new CodeMethodInvokeExpression((CodeExpression) null, codeMemberMethod.Name, new CodeExpression[0])));
              statements = codeMemberMethod.Statements;
            }
            else
              break;
          }
          this.AddComment(statements, string.Format("reaching state '{0}'", (object) label1.Label));
          foreach (string commentsForLogProbe in this.logProbesHelper.GetCommentsForLogProbes(label1, this.host))
            this.AddComment(statements, commentsForLogProbe);
          List<Edge<State, Transition>> outEdges;
          if (this.graph.TryGetOutGoingEdges(node, out outEdges) && outEdges.Count != 0)
          {
            expects1 = new List<CodeExpression>();
            expectContinuations1 = new List<CodeStatementCollection>();
            expects2 = new List<CodeExpression>();
            expects3 = new List<CodeExpression>();
            expectContinuations2 = new List<CodeStatementCollection>();
            expectContinuations3 = new List<CodeStatementCollection>();
            statements1 = (CodeStatementCollection) null;
            contNode = (Node<State>) null;
            foreach (Edge<State, Transition> edge in outEdges)
            {
              Transition label2 = edge.Label;
              CodeMemberMethod checkerMethod;
              switch (label2.Action.Symbol.Kind - 1)
              {
                case 0:
                  if (statements1 != null)
                    throw new TestCodeGenerationException(string.Format("Undetermined control not handled by test code generation method (occurred in state {0}). Did you forget to construct test cases in the machine you are trying to generate test code from?", (object) label1.Label));
                  statements1 = this.MakeControl(label2, edge.Target, out contNode);
                  continue;
                case ActionSymbolKind.Call:
                  expects1.Add(this.MakeExpect(edge.Label, out checkerMethod));
                  if (checkerMethod != null)
                  {
                    this.testMethodCodeCollection.Add((CodeTypeMember) checkerMethod);
                    this.currentLastTestHelperMethod = checkerMethod;
                  }
                  CodeStatementCollection statements2 = new CodeStatementCollection();
                  this.AddState(statements2, edge.Target);
                  expectContinuations1.Add(statements2);
                  continue;
                case ActionSymbolKind.Throw:
                  expects2.Add(this.MakeExpect(edge.Label, out checkerMethod));
                  if (checkerMethod != null)
                  {
                    this.testMethodCodeCollection.Add((CodeTypeMember) checkerMethod);
                    this.currentLastTestHelperMethod = checkerMethod;
                  }
                  CodeStatementCollection statements3 = new CodeStatementCollection();
                  this.AddState(statements3, edge.Target);
                  expectContinuations3.Add(statements3);
                  continue;
                case ActionSymbolKind.Event:
                  expects3.Add(this.MakeExpect(edge.Label, out checkerMethod));
                  if (checkerMethod != null)
                  {
                    this.testMethodCodeCollection.Add((CodeTypeMember) checkerMethod);
                    this.currentLastTestHelperMethod = checkerMethod;
                  }
                  CodeStatementCollection statements4 = new CodeStatementCollection();
                  this.AddState(statements4, edge.Target);
                  expectContinuations2.Add(statements4);
                  continue;
                default:
                  throw new TestCodeGenerationException(string.Format("Action '{0}' not supported by test code generation method", (object) label2.Action.Symbol.Member.Header));
              }
            }
            if (contNode != null)
            {
              if (expects2.Count == 0 && expects1.Count == 0)
              {
                statements.AddRange(statements1);
                node = contNode;
                label1 = contNode.Label;
              }
              else
                goto label_37;
            }
            else
              goto label_38;
          }
          else
            goto label_15;
        }
        statements.Add((CodeStatement) new CodeExpressionStatement((CodeExpression) new CodeMethodInvokeExpression((CodeExpression) null, codeMemberMethod.Name, new CodeExpression[0])));
        return;
label_15:
        this.AddEndStateCheck(statements, label1, true);
        return;
label_37:
        this.AddState(statements1, contNode);
label_38:
        CodeStatementCollection defaultContinuation = statements1;
        CodeExpression timeout = statements1 == null ? TestCodeGenerateBase.MakeThisReference("QuiescenceTimeout") : TestCodeGenerateBase.MakeThisReference("ProceedControlTimeout");
        if (expects2.Count > 0)
          defaultContinuation = this.MakeExpectStatement(label1, TestCodeGenerateBase.ExpectKind.Event, timeout, expects2, expectContinuations3, defaultContinuation);
        if (expects1.Count > 0)
          defaultContinuation = this.MakeExpectStatement(label1, TestCodeGenerateBase.ExpectKind.Return, timeout, expects1, expectContinuations1, defaultContinuation);
        if (expectContinuations2.Count > 0)
        {
          CodeStatementCollection statementCollection = new CodeStatementCollection();
          this.AddEndStateCheck(statementCollection, label1, false);
          defaultContinuation = this.MakeExpectPreConstraintStatement(expects3, expectContinuations2, statementCollection);
        }
        statements.AddRange(defaultContinuation);
      }
    }

    private void AddEndStateCheck(
      CodeStatementCollection stms,
      State state,
      bool shallReportWarning)
    {
      if ((state.Flags & StateFlags.Error) != null)
      {
        stms.Add((CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeManagerInvoke("Assert", TestCodeGenerateBase.MakeValue((object) false), TestCodeGenerateBase.MakeValue((object) string.Format("reached error state '{0}'", (object) state.Label)))));
      }
      else
      {
        if ((state.Flags & StateFlags.Accepting) != null)
          return;
        if ((state.Flags & StateFlags.BoundStopped) != null)
        {
          string str1 = "";
          string str2 = "";
          StateFlags stateFlags = (StateFlags) (state.Flags & StateFlags.BoundStopped);
          if (stateFlags <= StateFlags.StateBoundStopped)
          {
            if (stateFlags != StateFlags.StepBoundStopped)
            {
              if (stateFlags == StateFlags.StateBoundStopped)
              {
                str2 = nameof (state);
                str1 = "StateBound";
              }
            }
            else
            {
              str2 = "step";
              str1 = "StepBound";
            }
          }
          else if (stateFlags != StateFlags.PathDepthBoundStopped)
          {
            if (stateFlags != StateFlags.StepsPerStateBoundStopped)
            {
              if (stateFlags == StateFlags.ExplorationErrorBoundStopped)
              {
                str2 = "exploration error";
                str1 = "ExplorationErrorBound";
              }
            }
            else
            {
              str2 = "steps per state";
              str1 = "StepsPerStateBound";
            }
          }
          else
          {
            str2 = "path depth";
            str1 = "PathDepthBound";
          }
          if (shallReportWarning)
            this.Warning("[{0}]:Exploration of test code generation hit a {1} bound at state '{2}', this will result in a test failure if this path is ever executed.You might increase the value of {3} to solve this issue.", (object) this.MachineName, (object) str2, (object) state.Label, (object) str1);
          stms.Add((CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeManagerInvoke("Assert", TestCodeGenerateBase.MakeValue((object) false), TestCodeGenerateBase.MakeValue((object) string.Format("exploration of test code generation hit a {0} bound at state '{1}'.", (object) str2, (object) state.Label)))));
        }
        if (shallReportWarning && (state.Flags & ObjectModel.StateFlags.NonAcceptingEnd) != null)
          this.Warning("[{0}]:Exploration of test code generation ended in a non-accepting end state '{1}', this will result in test failure if this path is ever executed.", (object) this.MachineName, (object) state.Label);
        stms.Add((CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeManagerInvoke("Assert", TestCodeGenerateBase.MakeValue((object) false), TestCodeGenerateBase.MakeValue((object) string.Format("reached non-accepting end state '{0}'.", (object) state.Label)))));
      }
    }

    private CodeStatementCollection MakeControl(
      Transition transition,
      Node<State> targetNode,
      out Node<State> contNode)
    {
      List<CodeVariableDeclarationStatement> outputs;
      SerializableMethodBase mbase;
      CodeExpression receiver;
      List<CodeExpression> codeArgs;
      bool isCtor;
      CodeStatementCollection statements;
      this.GenerateCallActionStep(transition, out outputs, out mbase, out receiver, out codeArgs, out isCtor, out statements);
      this.AddCheckpoints(statements, transition);
      this.AddVariableUnbinding(statements, transition);
      SerializableExpression[] arguments1 = transition.Action.Arguments;
      bool arguments2 = this.CalculateArguments(outputs, (SerializableMemberInfo) mbase, receiver, codeArgs, isCtor);
      List<Edge<State, Transition>> outEdges;
      if (!this.graph.TryGetOutGoingEdges(targetNode, out outEdges))
        outEdges = new List<Edge<State, Transition>>();
      if (outEdges.Count == 1 && this.graph.IncomingCount(targetNode) == 1 && (outEdges[0].Label.Action.Symbol.Kind == ActionSymbolKind.Return && outEdges[0].Label.Action.Symbol.Member == transition.Action.Symbol.Member))
      {
        this.AddComment(statements, string.Format("reaching state '{0}'", (object) targetNode.Label.Label));
        foreach (string commentsForLogProbe in this.logProbesHelper.GetCommentsForLogProbes(targetNode.Label, this.host))
          this.AddComment(statements, commentsForLogProbe);
        this.GenerateReturnStatement(transition, codeArgs, isCtor, statements, arguments1, outEdges[0].Label);
        contNode = outEdges[0].Target;
      }
      else
      {
        contNode = targetNode;
        this.GenerateAddReturn(transition, (SerializableMemberInfo) mbase, codeArgs, isCtor, statements, arguments2);
      }
      return statements;
    }
  }
}
