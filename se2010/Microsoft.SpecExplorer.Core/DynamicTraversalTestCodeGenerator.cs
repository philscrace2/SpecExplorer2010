// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.DynamicTraversalTestCodeGenerator
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.Runtime.Testing;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

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
      this.MachineName = machineName;
      this.logProbesHelper.CheckLogProbesSwitchValue(this.transitionSystem, this.host, this.MachineName);
      this.InitializeInstanceFieldMaps();
      this.methodMap = this.transitionSystem.InitializeActionMethodMap();
      this.InitializeCodeGenerator();
      this.RetrieveTestAttributes();
      CodeCompileUnit e = new CodeCompileUnit();
      string name = this.transitionSystem.GetSwitch("GeneratedTestNamespace");
      if (string.IsNullOrEmpty(name))
        name = "GeneratedTests";
      CodeNamespace codeNamespace = new CodeNamespace(name);
      e.Namespaces.Add(codeNamespace);
      codeNamespace.Imports.AddRange(this.GenerateImportNamespaces().ToArray<CodeNamespaceImport>());
      codeNamespace.Imports.AddRange(this.GenerateDynamicTraversalNamespaces().ToArray<CodeNamespaceImport>());
      e.AssemblyCustomAttributes.AddRange(this.testAttributeProvider.CreateTestAssemblyAttributes().ToArray<CodeAttributeDeclaration>());
      codeNamespace.Types.Add(this.GenerateTestClass());
      StringWriter stringWriter = new StringWriter();
      this.generator.GenerateCodeFromCompileUnit(e, (TextWriter) stringWriter, this.generatorOptions);
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
      this.PreprocessInnerTestClass(out generatedTestClassName, out generateStaticTestMethods);
      this.GenerateTestCase();
      this.GenerateTransitionDelegates();
      CodeConstructor constructor = this.GenerateConstructor();
      this.GenerateAdditionalConstructorStatements(constructor);
      CodeAssignStatement codeAssignStatement = new CodeAssignStatement((CodeExpression) new CodeFieldReferenceExpression(TestCodeGenerateBase.MakeThisReference("traversal"), "Manager"), TestCodeGenerateBase.MakeThisReference("Manager"));
      CodeTypeDeclaration innerTestClass = this.GenerateInnerTestClass(generatedTestClassName, generateStaticTestMethods, constructor, this.GenerateDynamicTraversalFields(generatedTestClassName), (CodeStatement) codeAssignStatement);
      innerTestClass.Comments.Add(new CodeCommentStatement("Dynamic traversal test only has one test case, which will traverse the transition system dynamically according to the customized strategy. Your test harness might have default timeout for a single test case, please configure this timeout when you execute Spec Explorer dynamic test."));
      return innerTestClass;
    }

    private void GenerateAdditionalConstructorStatements(CodeConstructor constructor)
    {
      CodeAssignStatement codeAssignStatement1 = new CodeAssignStatement(TestCodeGenerateBase.MakeThisReference("LogToFile"), (CodeExpression) new CodePrimitiveExpression((object) true));
      constructor.Statements.Add((CodeStatement) codeAssignStatement1);
      CodeAssignStatement codeAssignStatement2 = new CodeAssignStatement(TestCodeGenerateBase.MakeThisReference("transitionSystem"), (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) new CodeTypeReferenceExpression("DynamicTraversalHelper"), "GetTransitionSystem"), new CodeExpression[1]
      {
        TestCodeGenerateBase.MakeThisReference("splitSerializedTransitionSystem")
      }));
      constructor.Statements.Add((CodeStatement) codeAssignStatement2);
      CodeFieldReferenceExpression referenceExpression = new CodeFieldReferenceExpression(TestCodeGenerateBase.MakeThisReference("transitionSystem"), "Transitions");
      foreach (KeyValuePair<int, CodeExpression> callDelegate in this.callDelegates)
        constructor.Statements.Add((CodeStatement) new CodeAssignStatement()
        {
          Left = (CodeExpression) new CodeArrayIndexerExpression(TestCodeGenerateBase.MakeThisReference("callDelegates"), new CodeExpression[1]
          {
            (CodeExpression) new CodeArrayIndexerExpression((CodeExpression) referenceExpression, new CodeExpression[1]
            {
              (CodeExpression) new CodePrimitiveExpression((object) callDelegate.Key)
            })
          }),
          Right = callDelegate.Value
        });
      foreach (KeyValuePair<int, CodeExpression> returnDelegate in this.returnDelegates)
        constructor.Statements.Add((CodeStatement) new CodeAssignStatement()
        {
          Left = (CodeExpression) new CodeArrayIndexerExpression(TestCodeGenerateBase.MakeThisReference("returnDelegates"), new CodeExpression[1]
          {
            (CodeExpression) new CodeArrayIndexerExpression((CodeExpression) referenceExpression, new CodeExpression[1]
            {
              (CodeExpression) new CodePrimitiveExpression((object) returnDelegate.Key)
            })
          }),
          Right = returnDelegate.Value
        });
      foreach (KeyValuePair<int, CodeExpression> eventDelegate in this.eventDelegates)
        constructor.Statements.Add((CodeStatement) new CodeAssignStatement()
        {
          Left = (CodeExpression) new CodeArrayIndexerExpression(TestCodeGenerateBase.MakeThisReference("eventDelegates"), new CodeExpression[1]
          {
            (CodeExpression) new CodeArrayIndexerExpression((CodeExpression) referenceExpression, new CodeExpression[1]
            {
              (CodeExpression) new CodePrimitiveExpression((object) eventDelegate.Key)
            })
          }),
          Right = eventDelegate.Value
        });
    }

    private CodeTypeMemberCollection GenerateDynamicTraversalFields(
      string generatedTestClassName)
    {
      CodeTypeMemberCollection memberCollection = new CodeTypeMemberCollection();
      CodeTypeReference type = new CodeTypeReference(generatedTestClassName);
      CodeMemberField codeMemberField1 = new CodeMemberField(type, "transitionSystem");
      codeMemberField1.Type = new CodeTypeReference(typeof (TransitionSystem));
      codeMemberField1.Attributes = MemberAttributes.Private;
      memberCollection.Add((CodeTypeMember) codeMemberField1);
      codeMemberField1.StartDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.Start, "Transition System and Transition Delegate Dictionary Fields"));
      CodeMemberField codeMemberField2 = new CodeMemberField(new CodeTypeReference("IDynamicTraversal"), "traversal");
      string typeName = this.transitionSystem.GetSwitch("DynamicTestStrategy");
      codeMemberField2.InitExpression = !string.IsNullOrEmpty(typeName) ? (CodeExpression) new CodeObjectCreateExpression(new CodeTypeReference(typeName), new CodeExpression[0]) : throw new TestCodeGenerationException("Invalid switch: DynamicTestStrategy must be set if GenerateDynamicTest is true.");
      memberCollection.Add((CodeTypeMember) codeMemberField2);
      CodeMemberField codeMemberField3 = new CodeMemberField(type, "callDelegates");
      codeMemberField3.Attributes = MemberAttributes.Private;
      CodeTypeReference createType1 = new CodeTypeReference("Dictionary", new CodeTypeReference[2]
      {
        new CodeTypeReference("Transition"),
        new CodeTypeReference("CallTransitionDelegate")
      });
      codeMemberField3.Type = createType1;
      codeMemberField3.InitExpression = (CodeExpression) new CodeObjectCreateExpression(createType1, new CodeExpression[0]);
      memberCollection.Add((CodeTypeMember) codeMemberField3);
      CodeMemberField codeMemberField4 = new CodeMemberField(type, "returnDelegates");
      codeMemberField4.Attributes = MemberAttributes.Private;
      CodeTypeReference createType2 = new CodeTypeReference("Dictionary", new CodeTypeReference[2]
      {
        new CodeTypeReference("Transition"),
        new CodeTypeReference("ReturnTransitionDelegate")
      });
      codeMemberField4.Type = createType2;
      codeMemberField4.InitExpression = (CodeExpression) new CodeObjectCreateExpression(createType2, new CodeExpression[0]);
      memberCollection.Add((CodeTypeMember) codeMemberField4);
      CodeMemberField codeMemberField5 = new CodeMemberField(type, "eventDelegates");
      codeMemberField5.Attributes = MemberAttributes.Private;
      CodeTypeReference createType3 = new CodeTypeReference("Dictionary", new CodeTypeReference[2]
      {
        new CodeTypeReference("Transition"),
        new CodeTypeReference("EventTransitionDelegate")
      });
      codeMemberField5.Type = createType3;
      codeMemberField5.InitExpression = (CodeExpression) new CodeObjectCreateExpression(createType3, new CodeExpression[0]);
      memberCollection.Add((CodeTypeMember) codeMemberField5);
      CodeMemberField codeMemberField6 = new CodeMemberField(type, "splitSerializedTransitionSystem");
      codeMemberField6.Type = new CodeTypeReference(new CodeTypeReference(typeof (string)), 1);
      codeMemberField6.Attributes = MemberAttributes.Private;
      string str1;
      try
      {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (TransitionSystem));
        StringWriter stringWriter = new StringWriter();
        xmlSerializer.Serialize((TextWriter) stringWriter, (object) this.transitionSystem);
        str1 = stringWriter.ToString();
        stringWriter.Close();
      }
      catch (InvalidOperationException ex)
      {
        throw new TestCodeGenerationException("Failed to serialize transition system during dynamic test code generation.", (Exception) ex);
      }
      List<string> source1 = new List<string>();
      while (str1.Length > 10240)
      {
        string str2 = str1.Substring(0, 10240);
        str1 = str1.Substring(10240);
        source1.Add(str2);
      }
      source1.Add(str1);
      IEnumerable<CodePrimitiveExpression> source2 = source1.Select<string, CodePrimitiveExpression>((Func<string, CodePrimitiveExpression>) (i => new CodePrimitiveExpression((object) i)));
      codeMemberField6.InitExpression = (CodeExpression) new CodeArrayCreateExpression(new CodeTypeReference(new CodeTypeReference(typeof (string)), 1), (CodeExpression[]) source2.ToArray<CodePrimitiveExpression>());
      memberCollection.Add((CodeTypeMember) codeMemberField6);
      codeMemberField6.EndDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.End, ""));
      return memberCollection;
    }

    private void GenerateTransitionDelegates()
    {
      int counter = 0;
      foreach (Transition transition in this.transitionSystem.Transitions)
      {
        this.GenerateTransitionDelegate(transition, counter);
        ++counter;
      }
    }

    private void GenerateTransitionDelegate(Transition transition, int counter)
    {
      if (this.transitionMethods.TryGetValue(transition, out CodeMemberMethod _))
        return;
      CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
      codeMemberMethod.Attributes = MemberAttributes.Public;
      codeMemberMethod.Name = this.MakeDynamicTraversalDelegateName(transition.Action.Symbol.Member);
      CodeRegionDirective codeRegionDirective1 = new CodeRegionDirective(CodeRegionMode.Start, string.Format("Delegates for transition \"{0}\" from state {1} to state {2}", (object) transition.Action.Text, (object) transition.Source, (object) transition.Target));
      CodeRegionDirective codeRegionDirective2 = new CodeRegionDirective(CodeRegionMode.End, "");
      if (this.transitionSystem.IsEventTransition(transition))
      {
        codeMemberMethod.ReturnType = new CodeTypeReference(typeof (ExpectedEvent));
        SerializableMemberInfo member = transition.Action.Symbol.Member;
        if (!this.delegateInstanceFieldMap.ContainsKey(member))
          this.delegateInstanceFieldMap[member] = this.GenerateDelegateFieldInstance(member);
        CodeMemberMethod checkerMethod;
        CodeMethodReturnStatement methodReturnStatement = new CodeMethodReturnStatement(this.MakeExpect(transition, out checkerMethod));
        codeMemberMethod.Statements.Add((CodeStatement) methodReturnStatement);
        codeMemberMethod.StartDirectives.Add((CodeDirective) codeRegionDirective1);
        this.testMethodCodeCollection.Add((CodeTypeMember) codeMemberMethod);
        if (checkerMethod != null)
        {
          this.testMethodCodeCollection.Add((CodeTypeMember) checkerMethod);
          checkerMethod.EndDirectives.Add((CodeDirective) codeRegionDirective2);
        }
        else
          codeMemberMethod.EndDirectives.Add((CodeDirective) codeRegionDirective2);
        CodeDelegateCreateExpression createExpression = new CodeDelegateCreateExpression(new CodeTypeReference("EventTransitionDelegate"), TestCodeGenerateBase.MakeThis(), codeMemberMethod.Name);
        this.eventDelegates[counter] = (CodeExpression) createExpression;
      }
      else if (this.transitionSystem.IsCallTransition(transition))
      {
        codeMemberMethod.ReturnType = new CodeTypeReference(typeof (void));
        codeMemberMethod.Statements.AddRange(this.GenerateCallActionStatements(transition));
        this.testMethodCodeCollection.Add((CodeTypeMember) codeMemberMethod);
        codeMemberMethod.StartDirectives.Add((CodeDirective) codeRegionDirective1);
        codeMemberMethod.EndDirectives.Add((CodeDirective) codeRegionDirective2);
        CodeDelegateCreateExpression createExpression = new CodeDelegateCreateExpression(new CodeTypeReference("CallTransitionDelegate"), TestCodeGenerateBase.MakeThis(), codeMemberMethod.Name);
        this.callDelegates[counter] = (CodeExpression) createExpression;
      }
      else if (this.transitionSystem.IsReturnTransition(transition))
      {
        SerializableMemberInfo member = transition.Action.Symbol.Member;
        Transition transition1;
        if (this.transitionSystem.TryGetUniqueCallTransition(transition, out transition1))
          return;
        if (!this.delegateInstanceFieldMap.ContainsKey(member))
          this.delegateInstanceFieldMap[member] = this.GenerateDelegateFieldInstance(member);
        codeMemberMethod.ReturnType = new CodeTypeReference(typeof (ExpectedReturn));
        CodeMemberMethod checkerMethod;
        CodeMethodReturnStatement methodReturnStatement = new CodeMethodReturnStatement(this.MakeExpect(transition, out checkerMethod));
        codeMemberMethod.Statements.Add((CodeStatement) methodReturnStatement);
        codeMemberMethod.StartDirectives.Add((CodeDirective) codeRegionDirective1);
        this.testMethodCodeCollection.Add((CodeTypeMember) codeMemberMethod);
        if (checkerMethod != null)
        {
          this.testMethodCodeCollection.Add((CodeTypeMember) checkerMethod);
          checkerMethod.EndDirectives.Add((CodeDirective) codeRegionDirective2);
        }
        else
          codeMemberMethod.EndDirectives.Add((CodeDirective) codeRegionDirective2);
        CodeDelegateCreateExpression createExpression = new CodeDelegateCreateExpression(new CodeTypeReference("ReturnTransitionDelegate"), TestCodeGenerateBase.MakeThis(), codeMemberMethod.Name);
        this.returnDelegates[counter] = (CodeExpression) createExpression;
      }
      this.transitionMethods[transition] = codeMemberMethod;
    }

    private CodeStatementCollection GenerateCallActionStatements(
      Transition transition)
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
      Transition successor;
      if (this.transitionSystem.TryGetUniqueReturnTransition(transition, out successor))
        this.GenerateReturnStatement(transition, codeArgs, isCtor, statements, arguments1, successor);
      else
        this.GenerateAddReturn(transition, (SerializableMemberInfo) mbase, codeArgs, isCtor, statements, arguments2);
      return statements;
    }

    private void GenerateTestCase()
    {
      CodeMemberMethod testMethod = new CodeMemberMethod();
      testMethod.Attributes = MemberAttributes.Public;
      if (!this.testCaseName.IsNoneOrEmptyValue())
        testMethod.Name = this.MakeUnique(this.variableResolver.Resolve("testcasename"));
      else
        testMethod.Name = this.MakeUnique(this.transitionSystem.Name + "Test");
      string methodReturnValue = this.GenerateTestMethodReturnValue(testMethod, new CodeStatementCollection()
      {
        (CodeStatement) new CodeAssignStatement((CodeExpression) new CodePropertyReferenceExpression(TestCodeGenerateBase.MakeThisReference("traversal"), "QuiescenceTimeout"), TestCodeGenerateBase.MakeThisReference("QuiescenceTimeout")),
        (CodeStatement) new CodeAssignStatement((CodeExpression) new CodePropertyReferenceExpression(TestCodeGenerateBase.MakeThisReference("traversal"), "ProceedControlTimeout"), TestCodeGenerateBase.MakeThisReference("ProceedControlTimeout")),
        (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(TestCodeGenerateBase.MakeThisReference("traversal"), "RunTestSuite"), new CodeExpression[7]
        {
          TestCodeGenerateBase.MakeThisReference("transitionSystem"),
          TestCodeGenerateBase.MakeThisReference("callDelegates"),
          TestCodeGenerateBase.MakeThisReference("returnDelegates"),
          TestCodeGenerateBase.MakeThisReference("eventDelegates"),
          (CodeExpression) new CodeDelegateCreateExpression(new CodeTypeReference("TestHousekeepingHandler"), TestCodeGenerateBase.MakeThis(), "TestInitialize"),
          (CodeExpression) new CodeDelegateCreateExpression(new CodeTypeReference("TestHousekeepingHandler"), TestCodeGenerateBase.MakeThis(), "TestCleanup"),
          TestCodeGenerateBase.MakeThisReference("TestProperties")
        })
      }, true);
      this.GenerateStaticTestMethod(this.testAttributeProvider.CreateDynamicTraversalTestCaseAttributes().ToArray<CodeAttributeDeclaration>(), testMethod, methodReturnValue);
      this.testMethodCodeCollection.Add((CodeTypeMember) testMethod);
      testMethod.StartDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.Start, "Unique Dynamic Traversal Test Case"));
      testMethod.EndDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.End, ""));
    }

    private string MakeDynamicTraversalDelegateName(SerializableMemberInfo method) => this.MakeUnique(string.Format("{0}DynamicTraversalDelegate", (object) this.GetMethodName(method)));
  }
}
