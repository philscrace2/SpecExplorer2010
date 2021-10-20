using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;

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
			base.MachineName = machineName;
			logProbesHelper.CheckLogProbesSwitchValue(transitionSystem, host, base.MachineName);
			string @switch = transitionSystem.GetSwitch("ReRuns");
			if (!string.IsNullOrEmpty(@switch))
			{
				rerunTimes = int.Parse(@switch);
			}
			else
			{
				rerunTimes = 1;
			}
			return InternalGenerateTestCode();
		}

		public override CodeTypeDeclaration GenerateTestClass()
		{
			string generatedTestClassName;
			string generateStaticTestMethods;
			PreprocessInnerTestClass(out generatedTestClassName, out generateStaticTestMethods);
			foreach (Node<State> startNode in graph.StartNodes)
			{
				GenerateTestMethod(startNode, generatedTestClassName);
			}
			return GenerateInnerTestClass(generatedTestClassName, generateStaticTestMethods, GenerateConstructor(), new CodeTypeMemberCollection(), null);
		}

		private void GenerateTestMethod(Node<State> startNode, string generatedTestClassName)
		{
			instanceEventRemovalStatements.Clear();
			State label = startNode.Label;
			if (!testCaseName.IsNoneOrEmptyValue())
			{
				currentTestMethodName = MakeUnique(variableResolver.Resolve("testcasename", label.Label));
			}
			else
			{
				currentTestMethodName = MakeUnique(generatedTestClassName + label.Label);
			}
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = currentTestMethodName;
			codeMemberMethod.Attributes = (MemberAttributes)24578;
			codeMemberMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("Test Starting in {0}", label.Label)));
			testMethodCodeCollection.Add(codeMemberMethod);
			currentLastTestHelperMethod = codeMemberMethod;
			CodeStatementCollection codeStatementCollection = new CodeStatementCollection();
			AddState(codeStatementCollection, startNode);
			string testMethodReturnType = GenerateTestMethodReturnValue(codeMemberMethod, WrapTestMethodStatementsWithRerunLoop(rerunTimes, codeStatementCollection), false);
			GenerateStaticTestMethod(testAttributeProvider.CreateTestCaseAttributes(label.Label).ToArray(), codeMemberMethod, testMethodReturnType);
			currentLastTestHelperMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
			currentLastTestHelperMethod = null;
			currentTestMethodName = null;
		}

		private CodeStatementCollection WrapTestMethodStatementsWithRerunLoop(int rerunTimes, CodeStatementCollection currentStatements)
		{
			if (rerunTimes > 1)
			{
				CodeVariableReferenceExpression codeVariableReferenceExpression = new CodeVariableReferenceExpression("rerunTimes");
				CodeIterationStatement codeIterationStatement = new CodeIterationStatement(new CodeVariableDeclarationStatement(typeof(int), "rerunTimes", new CodePrimitiveExpression(1)), new CodeBinaryOperatorExpression(codeVariableReferenceExpression, CodeBinaryOperatorType.LessThanOrEqual, new CodePrimitiveExpression(rerunTimes)), new CodeAssignStatement(codeVariableReferenceExpression, new CodeBinaryOperatorExpression(codeVariableReferenceExpression, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
				codeIterationStatement.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(codeVariableReferenceExpression, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(1)), new CodeExpressionStatement(new CodeMethodInvokeExpression(TestCodeGenerateBase.MakeThis(), "TestInitialize"))));
				if (string.Compare("true", suppressGeneratedTestLogging, true) != 0)
				{
					AddComment(codeIterationStatement.Statements, new CodeMethodInvokeExpression(null, "String.Format", new CodePrimitiveExpression("Rerun time: {0}"), codeVariableReferenceExpression));
				}
				codeIterationStatement.Statements.AddRange(currentStatements);
				codeIterationStatement.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(codeVariableReferenceExpression, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(rerunTimes)), new CodeExpressionStatement(new CodeMethodInvokeExpression(TestCodeGenerateBase.MakeThis(), "TestCleanup"))));
				CodeStatementCollection codeStatementCollection = new CodeStatementCollection();
				codeStatementCollection.Add(codeIterationStatement);
				return codeStatementCollection;
			}
			return currentStatements;
		}

		private void AddState(CodeStatementCollection statements, Node<State> node)
		{
			State label = node.Label;
			CodeMemberMethod value;
			if (sharedStateMethods.TryGetValue(label.Label, out value))
			{
				statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(null, value.Name)));
				return;
			}
			List<CodeExpression> list;
			List<CodeStatementCollection> list2;
			List<CodeExpression> list3;
			List<CodeExpression> list4;
			List<CodeStatementCollection> list5;
			List<CodeStatementCollection> list6;
			CodeStatementCollection codeStatementCollection;
			while (true)
			{
				int num = graph.IncomingCount(node);
				if (num > 1 || (num == 1 && transitionSystem.InitialStates.Contains(label.Label)))
				{
					if (sharedStateMethods.TryGetValue(label.Label, out value))
					{
						statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(null, value.Name)));
						return;
					}
					value = new CodeMemberMethod();
					if (transitionSystem.InitialStates.Contains(label.Label))
					{
						value.Name = MakeUnique(testLogicClassName + label.Label + "Shared");
					}
					else
					{
						value.Name = MakeUnique(testLogicClassName + label.Label);
					}
					testMethodCodeCollection.Add(value);
					currentLastTestHelperMethod = value;
					sharedStateMethods[label.Label] = value;
					statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(null, value.Name)));
					statements = value.Statements;
				}
				AddComment(statements, string.Format("reaching state '{0}'", label.Label));
				foreach (string commentsForLogProbe in logProbesHelper.GetCommentsForLogProbes(label, host))
				{
					AddComment(statements, commentsForLogProbe);
				}
				List<Edge<State, Transition>> outEdges;
				if (!graph.TryGetOutGoingEdges(node, out outEdges) || outEdges.Count == 0)
				{
					AddEndStateCheck(statements, label, true);
					return;
				}
				list = new List<CodeExpression>();
				list2 = new List<CodeStatementCollection>();
				list3 = new List<CodeExpression>();
				list4 = new List<CodeExpression>();
				list5 = new List<CodeStatementCollection>();
				list6 = new List<CodeStatementCollection>();
				codeStatementCollection = null;
				Node<State> contNode = null;
				foreach (Edge<State, Transition> item in outEdges)
				{
					Transition label2 = item.Label;
					CodeMemberMethod checkerMethod;
					switch (label2.Action.Symbol.Kind)
					{
					case ActionSymbolKind.Return:
					{
						list.Add(MakeExpect(item.Label, out checkerMethod));
						if (checkerMethod != null)
						{
							testMethodCodeCollection.Add(checkerMethod);
							currentLastTestHelperMethod = checkerMethod;
						}
						CodeStatementCollection codeStatementCollection2 = new CodeStatementCollection();
						AddState(codeStatementCollection2, item.Target);
						list2.Add(codeStatementCollection2);
						break;
					}
					case ActionSymbolKind.PreConstraintCheck:
					{
						list4.Add(MakeExpect(item.Label, out checkerMethod));
						if (checkerMethod != null)
						{
							testMethodCodeCollection.Add(checkerMethod);
							currentLastTestHelperMethod = checkerMethod;
						}
						CodeStatementCollection codeStatementCollection2 = new CodeStatementCollection();
						AddState(codeStatementCollection2, item.Target);
						list5.Add(codeStatementCollection2);
						break;
					}
					case ActionSymbolKind.Event:
					{
						list3.Add(MakeExpect(item.Label, out checkerMethod));
						if (checkerMethod != null)
						{
							testMethodCodeCollection.Add(checkerMethod);
							currentLastTestHelperMethod = checkerMethod;
						}
						CodeStatementCollection codeStatementCollection2 = new CodeStatementCollection();
						AddState(codeStatementCollection2, item.Target);
						list6.Add(codeStatementCollection2);
						break;
					}
					case ActionSymbolKind.Call:
						if (codeStatementCollection != null)
						{
							throw new TestCodeGenerationException(string.Format("Undetermined control not handled by test code generation method (occurred in state {0}). Did you forget to construct test cases in the machine you are trying to generate test code from?", label.Label));
						}
						codeStatementCollection = MakeControl(label2, item.Target, out contNode);
						break;
					default:
						throw new TestCodeGenerationException(string.Format("Action '{0}' not supported by test code generation method", label2.Action.Symbol.Member.Header));
					}
				}
				if (contNode == null)
				{
					break;
				}
				if (list3.Count == 0 && list.Count == 0)
				{
					statements.AddRange(codeStatementCollection);
					node = contNode;
					label = contNode.Label;
					continue;
				}
				AddState(codeStatementCollection, contNode);
				break;
			}
			CodeStatementCollection codeStatementCollection3 = codeStatementCollection;
			CodeExpression timeout = ((codeStatementCollection == null) ? TestCodeGenerateBase.MakeThisReference("QuiescenceTimeout") : TestCodeGenerateBase.MakeThisReference("ProceedControlTimeout"));
			if (list3.Count > 0)
			{
				codeStatementCollection3 = MakeExpectStatement(label, ExpectKind.Event, timeout, list3, list6, codeStatementCollection3);
			}
			if (list.Count > 0)
			{
				codeStatementCollection3 = MakeExpectStatement(label, ExpectKind.Return, timeout, list, list2, codeStatementCollection3);
			}
			if (list5.Count > 0)
			{
				CodeStatementCollection codeStatementCollection4 = new CodeStatementCollection();
				AddEndStateCheck(codeStatementCollection4, label, false);
				codeStatementCollection3 = MakeExpectPreConstraintStatement(list4, list5, codeStatementCollection4);
			}
			statements.AddRange(codeStatementCollection3);
		}

		private void AddEndStateCheck(CodeStatementCollection stms, State state, bool shallReportWarning)
		{
			if ((state.Flags & StateFlags.Error) != 0)
			{
				stms.Add(new CodeExpressionStatement(TestCodeGenerateBase.MakeManagerInvoke("Assert", TestCodeGenerateBase.MakeValue(false), TestCodeGenerateBase.MakeValue(string.Format("reached error state '{0}'", state.Label)))));
			}
			else
			{
				if ((state.Flags & StateFlags.Accepting) != 0)
				{
					return;
				}
				if ((state.Flags & StateFlags.BoundStopped) != 0)
				{
					string text = "";
					string text2 = "";
					switch (state.Flags & StateFlags.BoundStopped)
					{
					case StateFlags.ExplorationErrorBoundStopped:
						text2 = "exploration error";
						text = "ExplorationErrorBound";
						break;
					case StateFlags.PathDepthBoundStopped:
						text2 = "path depth";
						text = "PathDepthBound";
						break;
					case StateFlags.StateBoundStopped:
						text2 = "state";
						text = "StateBound";
						break;
					case StateFlags.StepBoundStopped:
						text2 = "step";
						text = "StepBound";
						break;
					case StateFlags.StepsPerStateBoundStopped:
						text2 = "steps per state";
						text = "StepsPerStateBound";
						break;
					}
					if (shallReportWarning)
					{
						Warning("[{0}]:Exploration of test code generation hit a {1} bound at state '{2}', this will result in a test failure if this path is ever executed.You might increase the value of {3} to solve this issue.", base.MachineName, text2, state.Label, text);
					}
					stms.Add(new CodeExpressionStatement(TestCodeGenerateBase.MakeManagerInvoke("Assert", TestCodeGenerateBase.MakeValue(false), TestCodeGenerateBase.MakeValue(string.Format("exploration of test code generation hit a {0} bound at state '{1}'.", text2, state.Label)))));
				}
				if (shallReportWarning && (state.Flags & StateFlags.NonAcceptingEnd) != 0)
				{
					Warning("[{0}]:Exploration of test code generation ended in a non-accepting end state '{1}', this will result in test failure if this path is ever executed.", base.MachineName, state.Label);
				}
				stms.Add(new CodeExpressionStatement(TestCodeGenerateBase.MakeManagerInvoke("Assert", TestCodeGenerateBase.MakeValue(false), TestCodeGenerateBase.MakeValue(string.Format("reached non-accepting end state '{0}'.", state.Label)))));
			}
		}

		private CodeStatementCollection MakeControl(Transition transition, Node<State> targetNode, out Node<State> contNode)
		{
			List<CodeVariableDeclarationStatement> outputs;
			SerializableMethodBase mbase;
			CodeExpression receiver;
			List<CodeExpression> codeArgs;
			bool isCtor;
			CodeStatementCollection statements;
			GenerateCallActionStep(transition, out outputs, out mbase, out receiver, out codeArgs, out isCtor, out statements);
			AddCheckpoints(statements, transition);
			AddVariableUnbinding(statements, transition);
			SerializableExpression[] arguments = transition.Action.Arguments;
			bool hasReceiver = CalculateArguments(outputs, mbase, receiver, codeArgs, isCtor);
			List<Edge<State, Transition>> outEdges;
			if (!graph.TryGetOutGoingEdges(targetNode, out outEdges))
			{
				outEdges = new List<Edge<State, Transition>>();
			}
			if (outEdges.Count == 1 && graph.IncomingCount(targetNode) == 1 && outEdges[0].Label.Action.Symbol.Kind == ActionSymbolKind.Return && outEdges[0].Label.Action.Symbol.Member == transition.Action.Symbol.Member)
			{
				AddComment(statements, string.Format("reaching state '{0}'", targetNode.Label.Label));
				foreach (string commentsForLogProbe in logProbesHelper.GetCommentsForLogProbes(targetNode.Label, host))
				{
					AddComment(statements, commentsForLogProbe);
				}
				GenerateReturnStatement(transition, codeArgs, isCtor, statements, arguments, outEdges[0].Label);
				contNode = outEdges[0].Target;
			}
			else
			{
				contNode = targetNode;
				GenerateAddReturn(transition, mbase, codeArgs, isCtor, statements, hasReceiver);
			}
			return statements;
		}
	}
}
