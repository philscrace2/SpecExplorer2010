using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.Runtime.Testing;

namespace Microsoft.SpecExplorer
{
	public class DynamicTraversalTestCodeGenerator : TestCodeGenerateBase
	{
		private const string TraversalFieldName = "traversal";

		private const string TransitionSystemFieldName = "transitionSystem";

		private const string SerializedTransitionSystemFieldName = "splitSerializedTransitionSystem";

		private const string CallDelegatesFieldName = "callDelegates";

		private const string ReturnDelegatesFieldName = "returnDelegates";

		private const string EventDelegatesFieldName = "eventDelegates";

		private const string TestHousekeepingHandlerName = "TestHousekeepingHandler";

		private const string LogToFileFieldName = "LogToFile";

		private const int MaxStringLength = 10240;

		private Dictionary<int, CodeExpression> callDelegates = new Dictionary<int, CodeExpression>();

		private Dictionary<int, CodeExpression> eventDelegates = new Dictionary<int, CodeExpression>();

		private Dictionary<int, CodeExpression> returnDelegates = new Dictionary<int, CodeExpression>();

		private Dictionary<Transition, CodeMemberMethod> transitionMethods = new Dictionary<Transition, CodeMemberMethod>();

		public DynamicTraversalTestCodeGenerator(IHost host, TransitionSystem transitionSystem)
			: base(host, transitionSystem)
		{
		}

		public override string GenerateTestCode(string machineName)
		{
			base.MachineName = machineName;
			logProbesHelper.CheckLogProbesSwitchValue(transitionSystem, host, base.MachineName);
			InitializeInstanceFieldMaps();
			methodMap = transitionSystem.InitializeActionMethodMap();
			InitializeCodeGenerator();
			RetrieveTestAttributes();
			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			string text = transitionSystem.GetSwitch("GeneratedTestNamespace");
			if (string.IsNullOrEmpty(text))
			{
				text = "GeneratedTests";
			}
			CodeNamespace codeNamespace = new CodeNamespace(text);
			codeCompileUnit.Namespaces.Add(codeNamespace);
			codeNamespace.Imports.AddRange(GenerateImportNamespaces().ToArray());
			codeNamespace.Imports.AddRange(GenerateDynamicTraversalNamespaces().ToArray());
			codeCompileUnit.AssemblyCustomAttributes.AddRange(testAttributeProvider.CreateTestAssemblyAttributes().ToArray());
			codeNamespace.Types.Add(GenerateTestClass());
			StringWriter stringWriter = new StringWriter();
			generator.GenerateCodeFromCompileUnit(codeCompileUnit, stringWriter, generatorOptions);
			return stringWriter.ToString();
		}

		private IEnumerable<CodeNamespaceImport> GenerateDynamicTraversalNamespaces()
		{
			yield return new CodeNamespaceImport("Microsoft.SpecExplorer.DynamicTraversal");
			yield return new CodeNamespaceImport("Microsoft.SpecExplorer.ObjectModel");
		}

		public override CodeTypeDeclaration GenerateTestClass()
		{
			string generatedTestClassName;
			string generateStaticTestMethods;
			PreprocessInnerTestClass(out generatedTestClassName, out generateStaticTestMethods);
			GenerateTestCase();
			GenerateTransitionDelegates();
			CodeConstructor constructor = GenerateConstructor();
			GenerateAdditionalConstructorStatements(constructor);
			CodeAssignStatement testManagerSetStatement = new CodeAssignStatement(new CodeFieldReferenceExpression(TestCodeGenerateBase.MakeThisReference("traversal"), "Manager"), TestCodeGenerateBase.MakeThisReference("Manager"));
			CodeTypeDeclaration codeTypeDeclaration = GenerateInnerTestClass(generatedTestClassName, generateStaticTestMethods, constructor, GenerateDynamicTraversalFields(generatedTestClassName), testManagerSetStatement);
			codeTypeDeclaration.Comments.Add(new CodeCommentStatement("Dynamic traversal test only has one test case, which will traverse the transition system dynamically according to the customized strategy. Your test harness might have default timeout for a single test case, please configure this timeout when you execute Spec Explorer dynamic test."));
			return codeTypeDeclaration;
		}

		private void GenerateAdditionalConstructorStatements(CodeConstructor constructor)
		{
			CodeAssignStatement value = new CodeAssignStatement(TestCodeGenerateBase.MakeThisReference("LogToFile"), new CodePrimitiveExpression(true));
			constructor.Statements.Add(value);
			CodeAssignStatement value2 = new CodeAssignStatement(TestCodeGenerateBase.MakeThisReference("transitionSystem"), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("DynamicTraversalHelper"), "GetTransitionSystem"), TestCodeGenerateBase.MakeThisReference("splitSerializedTransitionSystem")));
			constructor.Statements.Add(value2);
			CodeFieldReferenceExpression targetObject = new CodeFieldReferenceExpression(TestCodeGenerateBase.MakeThisReference("transitionSystem"), "Transitions");
			foreach (KeyValuePair<int, CodeExpression> callDelegate in callDelegates)
			{
				CodeAssignStatement codeAssignStatement = new CodeAssignStatement();
				codeAssignStatement.Left = new CodeArrayIndexerExpression(TestCodeGenerateBase.MakeThisReference("callDelegates"), new CodeArrayIndexerExpression(targetObject, new CodePrimitiveExpression(callDelegate.Key)));
				codeAssignStatement.Right = callDelegate.Value;
				constructor.Statements.Add(codeAssignStatement);
			}
			foreach (KeyValuePair<int, CodeExpression> returnDelegate in returnDelegates)
			{
				CodeAssignStatement codeAssignStatement2 = new CodeAssignStatement();
				codeAssignStatement2.Left = new CodeArrayIndexerExpression(TestCodeGenerateBase.MakeThisReference("returnDelegates"), new CodeArrayIndexerExpression(targetObject, new CodePrimitiveExpression(returnDelegate.Key)));
				codeAssignStatement2.Right = returnDelegate.Value;
				constructor.Statements.Add(codeAssignStatement2);
			}
			foreach (KeyValuePair<int, CodeExpression> eventDelegate in eventDelegates)
			{
				CodeAssignStatement codeAssignStatement3 = new CodeAssignStatement();
				codeAssignStatement3.Left = new CodeArrayIndexerExpression(TestCodeGenerateBase.MakeThisReference("eventDelegates"), new CodeArrayIndexerExpression(targetObject, new CodePrimitiveExpression(eventDelegate.Key)));
				codeAssignStatement3.Right = eventDelegate.Value;
				constructor.Statements.Add(codeAssignStatement3);
			}
		}

		private CodeTypeMemberCollection GenerateDynamicTraversalFields(string generatedTestClassName)
		{
			CodeTypeMemberCollection codeTypeMemberCollection = new CodeTypeMemberCollection();
			CodeTypeReference type = new CodeTypeReference(generatedTestClassName);
			CodeMemberField codeMemberField = new CodeMemberField(type, "transitionSystem");
			codeMemberField.Type = new CodeTypeReference(typeof(TransitionSystem));
			codeMemberField.Attributes = MemberAttributes.Private;
			codeTypeMemberCollection.Add(codeMemberField);
			codeMemberField.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Transition System and Transition Delegate Dictionary Fields"));
			CodeMemberField codeMemberField2 = new CodeMemberField(new CodeTypeReference("IDynamicTraversal"), "traversal");
			string @switch = transitionSystem.GetSwitch("DynamicTestStrategy");
			if (string.IsNullOrEmpty(@switch))
			{
				throw new TestCodeGenerationException("Invalid switch: DynamicTestStrategy must be set if GenerateDynamicTest is true.");
			}
			codeMemberField2.InitExpression = new CodeObjectCreateExpression(new CodeTypeReference(@switch));
			codeTypeMemberCollection.Add(codeMemberField2);
			CodeMemberField codeMemberField3 = new CodeMemberField(type, "callDelegates");
			codeMemberField3.Attributes = MemberAttributes.Private;
			CodeTypeReference createType = (codeMemberField3.Type = new CodeTypeReference("Dictionary", new CodeTypeReference("Transition"), new CodeTypeReference("CallTransitionDelegate")));
			codeMemberField3.InitExpression = new CodeObjectCreateExpression(createType);
			codeTypeMemberCollection.Add(codeMemberField3);
			CodeMemberField codeMemberField4 = new CodeMemberField(type, "returnDelegates");
			codeMemberField4.Attributes = MemberAttributes.Private;
			CodeTypeReference createType2 = (codeMemberField4.Type = new CodeTypeReference("Dictionary", new CodeTypeReference("Transition"), new CodeTypeReference("ReturnTransitionDelegate")));
			codeMemberField4.InitExpression = new CodeObjectCreateExpression(createType2);
			codeTypeMemberCollection.Add(codeMemberField4);
			CodeMemberField codeMemberField5 = new CodeMemberField(type, "eventDelegates");
			codeMemberField5.Attributes = MemberAttributes.Private;
			CodeTypeReference createType3 = (codeMemberField5.Type = new CodeTypeReference("Dictionary", new CodeTypeReference("Transition"), new CodeTypeReference("EventTransitionDelegate")));
			codeMemberField5.InitExpression = new CodeObjectCreateExpression(createType3);
			codeTypeMemberCollection.Add(codeMemberField5);
			CodeMemberField codeMemberField6 = new CodeMemberField(type, "splitSerializedTransitionSystem");
			codeMemberField6.Type = new CodeTypeReference(new CodeTypeReference(typeof(string)), 1);
			codeMemberField6.Attributes = MemberAttributes.Private;
			string text;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(TransitionSystem));
				StringWriter stringWriter = new StringWriter();
				xmlSerializer.Serialize(stringWriter, transitionSystem);
				text = stringWriter.ToString();
				stringWriter.Close();
			}
			catch (InvalidOperationException innerException)
			{
				throw new TestCodeGenerationException("Failed to serialize transition system during dynamic test code generation.", innerException);
			}
			List<string> list = new List<string>();
			while (text.Length > 10240)
			{
				string item = text.Substring(0, 10240);
				text = text.Substring(10240);
				list.Add(item);
			}
			list.Add(text);
			IEnumerable<CodePrimitiveExpression> source = list.Select((string i) => new CodePrimitiveExpression(i));
			codeMemberField6.InitExpression = new CodeArrayCreateExpression(new CodeTypeReference(new CodeTypeReference(typeof(string)), 1), source.ToArray());
			codeTypeMemberCollection.Add(codeMemberField6);
			codeMemberField6.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
			return codeTypeMemberCollection;
		}

		private void GenerateTransitionDelegates()
		{
			int num = 0;
			Transition[] transitions = transitionSystem.Transitions;
			foreach (Transition transition in transitions)
			{
				GenerateTransitionDelegate(transition, num);
				num++;
			}
		}

		private void GenerateTransitionDelegate(Transition transition, int counter)
		{
			CodeMemberMethod value;
			if (transitionMethods.TryGetValue(transition, out value))
			{
				return;
			}
			value = new CodeMemberMethod();
			value.Attributes = MemberAttributes.Public;
			value.Name = MakeDynamicTraversalDelegateName(transition.Action.Symbol.Member);
			CodeRegionDirective value2 = new CodeRegionDirective(CodeRegionMode.Start, string.Format("Delegates for transition \"{0}\" from state {1} to state {2}", transition.Action.Text, transition.Source, transition.Target));
			CodeRegionDirective value3 = new CodeRegionDirective(CodeRegionMode.End, "");
			if (transitionSystem.IsEventTransition(transition))
			{
				value.ReturnType = new CodeTypeReference(typeof(ExpectedEvent));
				SerializableMemberInfo member = transition.Action.Symbol.Member;
				if (!delegateInstanceFieldMap.ContainsKey(member))
				{
					delegateInstanceFieldMap[member] = GenerateDelegateFieldInstance(member);
				}
				CodeMemberMethod checkerMethod;
				CodeMethodReturnStatement value4 = new CodeMethodReturnStatement(MakeExpect(transition, out checkerMethod));
				value.Statements.Add(value4);
				value.StartDirectives.Add(value2);
				testMethodCodeCollection.Add(value);
				if (checkerMethod != null)
				{
					testMethodCodeCollection.Add(checkerMethod);
					checkerMethod.EndDirectives.Add(value3);
				}
				else
				{
					value.EndDirectives.Add(value3);
				}
				CodeDelegateCreateExpression value5 = new CodeDelegateCreateExpression(new CodeTypeReference("EventTransitionDelegate"), TestCodeGenerateBase.MakeThis(), value.Name);
				eventDelegates[counter] = value5;
			}
			else if (transitionSystem.IsCallTransition(transition))
			{
				value.ReturnType = new CodeTypeReference(typeof(void));
				value.Statements.AddRange(GenerateCallActionStatements(transition));
				testMethodCodeCollection.Add(value);
				value.StartDirectives.Add(value2);
				value.EndDirectives.Add(value3);
				CodeDelegateCreateExpression value6 = new CodeDelegateCreateExpression(new CodeTypeReference("CallTransitionDelegate"), TestCodeGenerateBase.MakeThis(), value.Name);
				callDelegates[counter] = value6;
			}
			else if (transitionSystem.IsReturnTransition(transition))
			{
				SerializableMemberInfo member2 = transition.Action.Symbol.Member;
				Transition callTransition;
				if (transitionSystem.TryGetUniqueCallTransition(transition, out callTransition))
				{
					return;
				}
				if (!delegateInstanceFieldMap.ContainsKey(member2))
				{
					delegateInstanceFieldMap[member2] = GenerateDelegateFieldInstance(member2);
				}
				value.ReturnType = new CodeTypeReference(typeof(ExpectedReturn));
				CodeMemberMethod checkerMethod2;
				CodeMethodReturnStatement value7 = new CodeMethodReturnStatement(MakeExpect(transition, out checkerMethod2));
				value.Statements.Add(value7);
				value.StartDirectives.Add(value2);
				testMethodCodeCollection.Add(value);
				if (checkerMethod2 != null)
				{
					testMethodCodeCollection.Add(checkerMethod2);
					checkerMethod2.EndDirectives.Add(value3);
				}
				else
				{
					value.EndDirectives.Add(value3);
				}
				CodeDelegateCreateExpression value8 = new CodeDelegateCreateExpression(new CodeTypeReference("ReturnTransitionDelegate"), TestCodeGenerateBase.MakeThis(), value.Name);
				returnDelegates[counter] = value8;
			}
			transitionMethods[transition] = value;
		}

		private CodeStatementCollection GenerateCallActionStatements(Transition transition)
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
			Transition returnTransition;
			if (transitionSystem.TryGetUniqueReturnTransition(transition, out returnTransition))
			{
				GenerateReturnStatement(transition, codeArgs, isCtor, statements, arguments, returnTransition);
			}
			else
			{
				GenerateAddReturn(transition, mbase, codeArgs, isCtor, statements, hasReceiver);
			}
			return statements;
		}

		private void GenerateTestCase()
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Attributes = MemberAttributes.Public;
			if (!testCaseName.IsNoneOrEmptyValue())
			{
				codeMemberMethod.Name = MakeUnique(variableResolver.Resolve("testcasename"));
			}
			else
			{
				codeMemberMethod.Name = MakeUnique(transitionSystem.Name + "Test");
			}
			CodeStatementCollection codeStatementCollection = new CodeStatementCollection();
			CodePropertyReferenceExpression left = new CodePropertyReferenceExpression(TestCodeGenerateBase.MakeThisReference("traversal"), "QuiescenceTimeout");
			CodeAssignStatement value = new CodeAssignStatement(left, TestCodeGenerateBase.MakeThisReference("QuiescenceTimeout"));
			codeStatementCollection.Add(value);
			CodePropertyReferenceExpression left2 = new CodePropertyReferenceExpression(TestCodeGenerateBase.MakeThisReference("traversal"), "ProceedControlTimeout");
			CodeAssignStatement value2 = new CodeAssignStatement(left2, TestCodeGenerateBase.MakeThisReference("ProceedControlTimeout"));
			codeStatementCollection.Add(value2);
			CodeMethodInvokeExpression value3 = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(TestCodeGenerateBase.MakeThisReference("traversal"), "RunTestSuite"), TestCodeGenerateBase.MakeThisReference("transitionSystem"), TestCodeGenerateBase.MakeThisReference("callDelegates"), TestCodeGenerateBase.MakeThisReference("returnDelegates"), TestCodeGenerateBase.MakeThisReference("eventDelegates"), new CodeDelegateCreateExpression(new CodeTypeReference("TestHousekeepingHandler"), TestCodeGenerateBase.MakeThis(), "TestInitialize"), new CodeDelegateCreateExpression(new CodeTypeReference("TestHousekeepingHandler"), TestCodeGenerateBase.MakeThis(), "TestCleanup"), TestCodeGenerateBase.MakeThisReference("TestProperties"));
			codeStatementCollection.Add(value3);
			string testMethodReturnType = GenerateTestMethodReturnValue(codeMemberMethod, codeStatementCollection, true);
			GenerateStaticTestMethod(testAttributeProvider.CreateDynamicTraversalTestCaseAttributes().ToArray(), codeMemberMethod, testMethodReturnType);
			testMethodCodeCollection.Add(codeMemberMethod);
			codeMemberMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Unique Dynamic Traversal Test Case"));
			codeMemberMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
		}

		private string MakeDynamicTraversalDelegateName(SerializableMemberInfo method)
		{
			return MakeUnique(string.Format("{0}DynamicTraversalDelegate", GetMethodName(method)));
		}
	}
}
