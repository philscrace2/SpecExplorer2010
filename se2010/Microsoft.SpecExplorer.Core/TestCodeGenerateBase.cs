// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.TestCodeGenerateBase
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.CSharp;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.Runtime.Testing;
using Microsoft.Xrt;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using ExpressionType = Microsoft.SpecExplorer.ObjectModel.ExpressionType;
using Microsoft.SpecExplorer.Extensions;

namespace Microsoft.SpecExplorer
{
  public abstract class TestCodeGenerateBase : ITestCodeGenerator
  {
    protected const string DefaultGeneratedTestNamespace = "GeneratedTests";
    protected const string GeneratedTestNamespaceSwitchName = "GeneratedTestNamespace";
    protected const string GenerateStaticTestMethodsSwitchName = "GenerateStaticTestMethods";
    protected const string TestInitializeMethodName = "TestInitialize";
    protected const string TestCleanupMethodName = "TestCleanup";
    protected const string PtfTestClassBaseName = "PtfTestClassBase";
    protected const string VsTestClassBaseName = "VsTestClassBase";
    protected const string DefaultTestInnerClassSuffixName = "Inner";
    protected const string TestMethodReturnTypeSwitchName = "TestMethodReturnType";
    protected const string TestPassedReturnValueSwitchName = "TestPassedReturnValue";
    protected const string TestFailedReturnValueSwitchName = "TestFailedReturnValue";
    protected const string TestFailedExceptionTypeSwitchName = "TestFailedExceptionType";
    protected string testLogicClassName;
    protected IHost host;
    protected TransitionSystem transitionSystem;
    private OptionSetManager optionSetManager;
    protected VariableResolver variableResolver;
    protected IGraph<State, Transition> graph;
    private HashSet<SerializableType> adapterTypes;
    protected ICodeGenerator generator;
    protected CodeGeneratorOptions generatorOptions;
    private string testClassBase;
    protected string testCaseName;
    private string generateEventHandlers;
    protected string suppressGeneratedTestLogging;
    protected ITestAttributeProvider testAttributeProvider;
    protected string currentTestMethodName;
    protected CodeMemberMethod currentLastTestHelperMethod;
    private Dictionary<string, CodeMemberField> usedMetadataFields = new Dictionary<string, CodeMemberField>();
    protected CodeTypeMemberCollection testMethodCodeCollection = new CodeTypeMemberCollection();
    private CodeMemberMethod getFieldMethod;
    private Dictionary<SerializableEventInfo, CodeMemberMethod> eventHandlerMethods = new Dictionary<SerializableEventInfo, CodeMemberMethod>();
    protected Dictionary<string, SerializableMemberInfo> methodMap;
    private MethodInfo helperEqualityMethod;
    private CodeTypeMemberCollection testMethodWrapperCollection = new CodeTypeMemberCollection();
    protected LogProbesHelper logProbesHelper = new LogProbesHelper();
    private Dictionary<string, int> testClassMemberNames = new Dictionary<string, int>();
    protected Dictionary<SerializableMemberInfo, CodeTypeDelegate> delegateInstanceFieldMap = new Dictionary<SerializableMemberInfo, CodeTypeDelegate>();
    private Dictionary<SerializableType, CodeMemberField> adapterInstanceFieldMap = new Dictionary<SerializableType, CodeMemberField>();
    protected CodeStatementCollection instanceEventRemovalStatements = new CodeStatementCollection();
    private int tempCounter;
    private int labelCounter;

    protected string MachineName { get; set; }

    protected TestCodeGenerateBase(IHost host, TransitionSystem transitionSystem)
    {
      this.host = host;
      this.transitionSystem = transitionSystem;
      this.graph = (IGraph<State, Transition>) new TransitionSystemGraphBuilder(transitionSystem).BuildGraph();
      this.adapterTypes = new HashSet<SerializableType>((IEnumerable<SerializableType>) transitionSystem.AdapterTypes);
      this.optionSetManager = new OptionSetManagerBuilder(transitionSystem).CreateOptionSetManager();
      this.variableResolver = new VariableResolver(transitionSystem);
    }

    public string Generate(string machineName)
    {
      this.MachineName = machineName;
      string str = string.Empty;
      CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
      try
      {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        str = this.GenerateTestCode(machineName);
      }
      catch (TestCodeGenerationException ex)
      {
        this.host.DiagMessage(DiagnosisKind.Error, string.Format("[{0}]:Test Code Generation failed with errors (see below errors)", (object) machineName), (object) null);
        this.host.DiagMessage(DiagnosisKind.Error, ex.Message, (object) null);
      }
      finally
      {
        Thread.CurrentThread.CurrentCulture = currentCulture;
      }
      return str;
    }

    public abstract string GenerateTestCode(string machineName);

    protected void AddTestClassMembers(
      CodeTypeDeclaration testClass,
      CodeConstructor constructor,
      CodeTypeMemberCollection additionalMembers,
      CodeStatement testManagerSetStatement)
    {
      testClass.Members.AddRange(additionalMembers);
      testClass.Members.Add((CodeTypeMember) constructor);
      testClass.BaseTypes.Add(this.testClassBase);
      CodeTypeMemberCollection memberCollection = new CodeTypeMemberCollection();
      if (this.testClassBase == "PtfTestClassBase")
        memberCollection.AddRange(this.GenerateClassInitializationAndCleanup());
      memberCollection.AddRange(this.GenerateTestInitializationAndCleanup(testManagerSetStatement));
      testClass.Members.AddRange(this.GenerateExpectDelegateInstanceFieldDeclarations());
      testClass.Members.AddRange(this.GenerateMetadataDeclarationsAndInitialization());
      testClass.Members.AddRange(this.GenerateAdapterInstanceFieldDeclarations());
      testClass.Members.AddRange(this.GenerateVariableDeclarations());
      testClass.Members.AddRange(memberCollection);
      testClass.Members.AddRange(this.testMethodCodeCollection);
      if (this.getFieldMethod != null)
      {
        this.getFieldMethod.StartDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.Start, "Helpers"));
        testClass.Members.Add((CodeTypeMember) this.getFieldMethod);
        this.getFieldMethod.EndDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.End, ""));
      }
      int num = 0;
      foreach (CodeMemberMethod codeMemberMethod in this.eventHandlerMethods.Values)
      {
        if (num == 0)
          codeMemberMethod.StartDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.Start, "Event Handlers"));
        if (num == this.eventHandlerMethods.Count - 1)
          codeMemberMethod.EndDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.End, ""));
        testClass.Members.Add((CodeTypeMember) codeMemberMethod);
        ++num;
      }
    }

    protected CodeConstructor GenerateConstructor()
    {
      CodeConstructor codeConstructor = new CodeConstructor();
      codeConstructor.Attributes = MemberAttributes.Public;
      HashSet<string> stringSet = new HashSet<string>(this.optionSetManager.GetProperties(Visibility.TestCodeGeneration).Select<PropertyDescriptor, string>((Func<PropertyDescriptor, string>) (p => p.Name.ToLower())));
      foreach (ConfigSwitch configSwitch in this.transitionSystem.ConfigSwitches)
      {
        if (stringSet.Contains(configSwitch.Name.ToLower()))
          codeConstructor.Statements.Add((CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeThisInvoke("SetSwitch", TestCodeGenerateBase.MakeValue((object) configSwitch.Name), TestCodeGenerateBase.MakeValue((object) configSwitch.Value))));
      }
      return codeConstructor;
    }

    protected string InternalGenerateTestCode()
    {
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
      e.AssemblyCustomAttributes.AddRange(this.testAttributeProvider.CreateTestAssemblyAttributes().ToArray<CodeAttributeDeclaration>());
      codeNamespace.Types.Add(this.GenerateTestClass());
      StringWriter stringWriter = new StringWriter();
      this.generator.GenerateCodeFromCompileUnit(e, (TextWriter) stringWriter, this.generatorOptions);
      return stringWriter.ToString();
    }

    public abstract CodeTypeDeclaration GenerateTestClass();

    protected void InitializeCodeGenerator()
    {
      this.generator = new CSharpCodeProvider().CreateGenerator("testcode");
      this.generatorOptions = new CodeGeneratorOptions();
      this.generatorOptions.VerbatimOrder = true;
      this.generatorOptions.BlankLinesBetweenMembers = true;
    }

    protected void RetrieveTestAttributes()
    {
      this.testClassBase = this.variableResolver.Resolve("testclassbase");
      this.testCaseName = this.transitionSystem.GetSwitch("testcasename");
      this.generateEventHandlers = this.transitionSystem.GetSwitch("generateeventhandlers");
      this.suppressGeneratedTestLogging = this.transitionSystem.GetSwitch("suppressgeneratedtestlogging");
      if (string.Compare(this.testClassBase, "vs", true) == 0)
      {
        this.testClassBase = "VsTestClassBase";
        this.testAttributeProvider = (ITestAttributeProvider) new VsTestAttributeProvider(this.transitionSystem);
      }
      else if (string.Compare(this.testClassBase, "ptf", true) == 0)
      {
        this.testClassBase = "PtfTestClassBase";
        this.testAttributeProvider = (ITestAttributeProvider) new PtfTestAttributeProvider(this.transitionSystem);
      }
      else
        this.testAttributeProvider = (ITestAttributeProvider) new VsTestAttributeProvider(this.transitionSystem);
    }

    protected IEnumerable<CodeNamespaceImport> GenerateImportNamespaces()
    {
      yield return new CodeNamespaceImport("System");
      yield return new CodeNamespaceImport("System.Collections.Generic");
      yield return new CodeNamespaceImport("System.Text");
      yield return new CodeNamespaceImport("System.Reflection");
      yield return new CodeNamespaceImport("Microsoft.SpecExplorer.Runtime.Testing");
      if (this.testClassBase == "PtfTestClassBase")
        yield return new CodeNamespaceImport("Microsoft.Protocols.TestTools");
      else if (this.testClassBase == "VsTestClassBase")
        yield return new CodeNamespaceImport("Microsoft.VisualStudio.TestTools.UnitTesting");
    }

    protected void InitializeInstanceFieldMaps()
    {
      foreach (Edge<State, Transition> edge in this.graph.Edges)
      {
        Transition label = edge.Label;
        SerializableMemberInfo member = label.Action.Symbol.Member;
        if (this.adapterTypes.Contains(member.DeclaringType) && !this.adapterInstanceFieldMap.ContainsKey(member.DeclaringType))
          this.adapterInstanceFieldMap[member.DeclaringType] = new CodeMemberField(TestCodeGenerateBase.MakeTypeReference(member.DeclaringType), this.MakeAdapterInstanceName(member.DeclaringType.FullName));
        switch (label.Action.Symbol.Kind - 2)
        {
          case 0:
            if (((IEnumerable<SerializableMemberInfo>) this.transitionSystem.ActionMembers).Contains<SerializableMemberInfo>(member) && !this.delegateInstanceFieldMap.ContainsKey(member) && (this.graph.OutgoingCount(edge.Source) > 1 || this.graph.IncomingCount(edge.Source) > 1))
            {
              this.delegateInstanceFieldMap[member] = this.GenerateDelegateFieldInstance(member);
              continue;
            }
            continue;
          case ActionSymbolKind.Return:
          case ActionSymbolKind.Throw:
            if (((IEnumerable<SerializableMemberInfo>) this.transitionSystem.ActionMembers).Contains<SerializableMemberInfo>(member) && !this.delegateInstanceFieldMap.ContainsKey(member))
            {
              this.delegateInstanceFieldMap[member] = this.GenerateDelegateFieldInstance(member);
              continue;
            }
            continue;
          default:
            continue;
        }
      }
    }

    protected static CodeExpression MakeValue(object value) => (CodeExpression) new CodePrimitiveExpression(value);

    protected static CodeExpression MakeThis() => (CodeExpression) new CodeThisReferenceExpression();

    protected static CodeExpression MakeThisInvoke(
      string name,
      params CodeExpression[] parameters)
    {
      return (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(TestCodeGenerateBase.MakeThis(), name), parameters);
    }

    protected static CodeExpression MakeBase() => (CodeExpression) new CodeBaseReferenceExpression();

    protected static CodeExpression MakeThisReference(string name) => (CodeExpression) new CodeFieldReferenceExpression(TestCodeGenerateBase.MakeThis(), name);

    protected static CodeTypeReference MakeTypeReference(SerializableType type) => new CodeTypeReference(type.FullName);

    protected static CodeExpression MakeManagerInvoke(
      string name,
      params CodeExpression[] parameters)
    {
      return (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) new CodeFieldReferenceExpression(TestCodeGenerateBase.MakeThis(), "Manager"), name), parameters);
    }

    protected CodeMemberField GetMetadataField(SerializableMemberInfo member)
    {
      CodeMemberField orInitialization;
      if (!this.usedMetadataFields.TryGetValue(member.Header, out orInitialization))
      {
        orInitialization = this.GenerateMetadataDeclarationOrInitialization(member);
        if (orInitialization == null)
          this.host.DiagMessage(DiagnosisKind.Error, "Cannot find the metadata", (object) null);
        this.usedMetadataFields[member.Header] = orInitialization;
      }
      return orInitialization;
    }

    protected static CodeExpression MakeBaseInvoke(
      string name,
      params CodeExpression[] parameters)
    {
      return (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(TestCodeGenerateBase.MakeBase(), name), parameters);
    }

    protected string MakeUnique(string name)
    {
      int num;
      if (!this.testClassMemberNames.TryGetValue(name, out num))
        num = 0;
      string str = num > 0 ? name + (object) num : name;
      this.testClassMemberNames[name] = num + 1;
      return str;
    }

    private string MakeMetadataFieldName(SerializableMemberInfo method) => this.MakeUnique(string.Format("{0}Info", (object) this.GetMethodName(method)));

    protected string GetMethodName(SerializableMemberInfo method)
    {
      string str;
      if (method is SerializableConstructorInfo)
      {
        str = method.DeclaringType.FullName;
        int num = str.LastIndexOf(".");
        if (num >= 0)
          str = str.Substring(num + 1);
      }
      else
        str = method.Name;
      return str;
    }

    protected static CodeExpression MakeHelperInvoke(
      string name,
      params CodeExpression[] parameters)
    {
      return (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) new CodeTypeReferenceExpression("TestManagerHelpers"), name), parameters);
    }

    protected string MakeCheckerDelegateName(SerializableMemberInfo method) => this.MakeUnique(string.Format("{0}Delegate", (object) this.GetMethodName(method)));

    protected string MakeCheckerMethodName(string testName, SerializableMemberInfo method) => this.MakeUnique(string.Format("{0}{1}Checker", (object) testName, (object) this.GetMethodName(method)));

    protected string CodeExpressionToString(CodeExpression expression)
    {
      using (StringWriter stringWriter = new StringWriter())
      {
        this.generator.GenerateCodeFromExpression(expression, (TextWriter) stringWriter, this.generatorOptions);
        return stringWriter.ToString();
      }
    }

    private CodeTypeMemberCollection GenerateClassInitializationAndCleanup()
    {
      CodeTypeMemberCollection memberCollection = new CodeTypeMemberCollection();
      CodeMemberMethod codeMemberMethod1 = new CodeMemberMethod();
      memberCollection.Add((CodeTypeMember) codeMemberMethod1);
      codeMemberMethod1.StartDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.Start, "Class Initialization and Cleanup"));
      codeMemberMethod1.Name = "ClassInitialize";
      codeMemberMethod1.Attributes = MemberAttributes.Static | MemberAttributes.Public;
      codeMemberMethod1.CustomAttributes.AddRange(this.testAttributeProvider.CreateTestClassInitializeAttributes().ToArray<CodeAttributeDeclaration>());
      codeMemberMethod1.Parameters.Add(new CodeParameterDeclarationExpression("Microsoft.VisualStudio.TestTools.UnitTesting.TestContext", "context"));
      codeMemberMethod1.Statements.Add((CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(this.testClassBase), "Initialize"), new CodeExpression[1]
      {
        (CodeExpression) new CodeVariableReferenceExpression("context")
      }));
      CodeMemberMethod codeMemberMethod2 = new CodeMemberMethod();
      memberCollection.Add((CodeTypeMember) codeMemberMethod2);
      codeMemberMethod2.EndDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.End, ""));
      codeMemberMethod2.Name = "ClassCleanup";
      codeMemberMethod2.Attributes = MemberAttributes.Static | MemberAttributes.Public;
      codeMemberMethod2.CustomAttributes.AddRange(this.testAttributeProvider.CreateTestClassCleanupAttributes().ToArray<CodeAttributeDeclaration>());
      codeMemberMethod2.Statements.Add((CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) new CodeTypeReferenceExpression(this.testClassBase), "Cleanup"), new CodeExpression[0]));
      return memberCollection;
    }

    private CodeTypeMemberCollection GenerateTestInitializationAndCleanup(
      CodeStatement testManagerSetStatement)
    {
      CodeTypeMemberCollection memberCollection = new CodeTypeMemberCollection();
      CodeMemberMethod codeMemberMethod1 = new CodeMemberMethod();
      memberCollection.Add((CodeTypeMember) codeMemberMethod1);
      codeMemberMethod1.StartDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.Start, "Test Initialization and Cleanup"));
      codeMemberMethod1.Name = "TestInitialize";
      CodeAttributeDeclaration[] array1 = this.testAttributeProvider.CreateTestCaseInitializeAttributes().ToArray<CodeAttributeDeclaration>();
      if (array1.Length > 0)
      {
        codeMemberMethod1.Attributes = MemberAttributes.Public;
        codeMemberMethod1.CustomAttributes.AddRange(array1);
      }
      else if (this.testClassBase == "PtfTestClassBase")
        codeMemberMethod1.Attributes = MemberAttributes.Family | MemberAttributes.Override;
      else
        codeMemberMethod1.Attributes = MemberAttributes.Public;
      codeMemberMethod1.Statements.Add((CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeThisInvoke("InitializeTestManager")));
      if (testManagerSetStatement != null)
        codeMemberMethod1.Statements.Add(testManagerSetStatement);
      CodeStatementCollection eventAttachStatements;
      CodeStatementCollection eventRemovalStatements;
      this.GenerateEventAttachAndRemovalStatements(out eventAttachStatements, out eventRemovalStatements);
      foreach (CodeMemberField codeMemberField in this.adapterInstanceFieldMap.Values)
        codeMemberMethod1.Statements.Add((CodeStatement) new CodeAssignStatement(TestCodeGenerateBase.MakeThisReference(codeMemberField.Name), (CodeExpression) new CodeCastExpression(codeMemberField.Type, TestCodeGenerateBase.MakeManagerInvoke("GetAdapter", (CodeExpression) new CodeTypeOfExpression(codeMemberField.Type)))));
      codeMemberMethod1.Statements.AddRange(eventAttachStatements);
      foreach (SerializableParameterExpression variable in this.transitionSystem.Variables)
      {
        if (!this.transitionSystem.IsPlaceholder(variable))
          codeMemberMethod1.Statements.Add((CodeStatement) new CodeAssignStatement(TestCodeGenerateBase.MakeThisReference(variable.Name), (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) new CodeFieldReferenceExpression(TestCodeGenerateBase.MakeThis(), "Manager"), "CreateVariable", new CodeTypeReference[1]
          {
            TestCodeGenerateBase.MakeTypeReference(variable.ParameterType)
          }), new CodeExpression[1]
          {
            TestCodeGenerateBase.MakeValue((object) variable.Name)
          })));
      }
      CodeMemberMethod codeMemberMethod2 = new CodeMemberMethod();
      memberCollection.Add((CodeTypeMember) codeMemberMethod2);
      codeMemberMethod2.EndDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.End, ""));
      codeMemberMethod2.Name = "TestCleanup";
      CodeAttributeDeclaration[] array2 = this.testAttributeProvider.CreateTestCaseCleanupAttributes().ToArray<CodeAttributeDeclaration>();
      if (array2.Length > 0)
      {
        codeMemberMethod2.Attributes = MemberAttributes.Public;
        codeMemberMethod2.CustomAttributes.AddRange(array2);
      }
      else if (this.testClassBase == "PtfTestClassBase")
      {
        codeMemberMethod2.Attributes = MemberAttributes.Family | MemberAttributes.Override;
        codeMemberMethod2.Statements.Add((CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeBaseInvoke("TestCleanup")));
      }
      else
        codeMemberMethod2.Attributes = MemberAttributes.Public;
      codeMemberMethod2.Statements.AddRange(eventRemovalStatements);
      codeMemberMethod2.Statements.Add((CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeThisInvoke("CleanupTestManager")));
      return memberCollection;
    }

    private void GenerateEventAttachAndRemovalStatements(
      out CodeStatementCollection eventAttachStatements,
      out CodeStatementCollection eventRemovalStatements)
    {
      eventAttachStatements = new CodeStatementCollection();
      eventRemovalStatements = new CodeStatementCollection();
      foreach (SerializableEventInfo eventInfo in ((IEnumerable) this.transitionSystem.ActionMembers).OfType<SerializableEventInfo>())
      {
        if (this.usedMetadataFields.ContainsKey(((SerializableMemberInfo) eventInfo).Header) && !eventInfo.IsPreConstraintCheck)
        {
          bool isStatic = false;
          CodeExpression target;
          if (this.adapterTypes.Contains(((SerializableMemberInfo) eventInfo).DeclaringType))
            target = TestCodeGenerateBase.MakeThisReference(this.adapterInstanceFieldMap[((SerializableMemberInfo) eventInfo).DeclaringType].Name);
          else if (((SerializableMemberInfo) eventInfo).IsStatic)
          {
            isStatic = true;
            target = (CodeExpression) new CodeSnippetExpression("null");
          }
          else
            continue;
          if (string.Compare(this.generateEventHandlers, "true", true) == 0)
          {
            CodeStatement eventAttachStatement;
            CodeStatement eventRemovalStatement;
            this.GenerateEventAttachAndRemovalStatement(eventInfo, isStatic, target, out eventAttachStatement, out eventRemovalStatement);
            eventAttachStatements.Add(eventAttachStatement);
            eventRemovalStatements.Add(eventRemovalStatement);
          }
          else
            eventAttachStatements.Add((CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeManagerInvoke("Subscribe", new List<CodeExpression>()
            {
              (CodeExpression) new CodeSnippetExpression(this.GetMetadataField((SerializableMemberInfo) eventInfo).Name),
              target
            }.ToArray())));
        }
      }
    }

    private void GenerateEventAttachAndRemovalStatement(
      SerializableEventInfo eventInfo,
      bool isStatic,
      CodeExpression target,
      out CodeStatement eventAttachStatement,
      out CodeStatement eventRemovalStatement)
    {
      CodeMethodReferenceExpression referenceExpression = new CodeMethodReferenceExpression((CodeExpression) new CodeThisReferenceExpression(), this.GenerateEventHandlersMethod(eventInfo, target).Name);
      if (isStatic)
      {
        eventAttachStatement = (CodeStatement) new CodeAttachEventStatement((CodeExpression) new CodeTypeReferenceExpression(((SerializableMemberInfo) eventInfo).DeclaringType.FullName), ((SerializableMemberInfo) eventInfo).Name, (CodeExpression) referenceExpression);
        eventRemovalStatement = (CodeStatement) new CodeRemoveEventStatement((CodeExpression) new CodeTypeReferenceExpression(((SerializableMemberInfo) eventInfo).DeclaringType.FullName), ((SerializableMemberInfo) eventInfo).Name, (CodeExpression) referenceExpression);
      }
      else
      {
        eventAttachStatement = (CodeStatement) new CodeAttachEventStatement(target, ((SerializableMemberInfo) eventInfo).Name, (CodeExpression) referenceExpression);
        eventRemovalStatement = (CodeStatement) new CodeRemoveEventStatement(target, ((SerializableMemberInfo) eventInfo).Name, (CodeExpression) referenceExpression);
      }
    }

    private CodeMemberMethod GenerateEventHandlersMethod(
      SerializableEventInfo eventInfo,
      CodeExpression target)
    {
      CodeMemberMethod codeMemberMethod;
      if (!this.eventHandlerMethods.TryGetValue(eventInfo, out codeMemberMethod))
      {
        codeMemberMethod = new CodeMemberMethod();
        codeMemberMethod.Name = this.MakeUnique(((SerializableMemberInfo) eventInfo).Name + "Handler");
        if (eventInfo.ReturnType != null)
          codeMemberMethod.ReturnType = new CodeTypeReference(eventInfo.ReturnType.FullName);
        CodeSnippetExpression snippetExpression = new CodeSnippetExpression(this.GetMetadataField((SerializableMemberInfo) eventInfo).Name);
        foreach (SerializableParameterInfo parameter in eventInfo.Parameters)
          codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression(TestCodeGenerateBase.MakeTypeReference(parameter.Type), parameter.Name));
        List<CodeExpression> codeExpressionList1 = new List<CodeExpression>();
        codeExpressionList1.Add((CodeExpression) snippetExpression);
        codeExpressionList1.Add(target);
        List<CodeExpression> codeExpressionList2 = new List<CodeExpression>();
        foreach (CodeParameterDeclarationExpression parameter in (CollectionBase) codeMemberMethod.Parameters)
          codeExpressionList2.Add((CodeExpression) new CodeArgumentReferenceExpression(parameter.Name));
        codeExpressionList1.Add((CodeExpression) new CodeArrayCreateExpression(typeof (object), codeExpressionList2.ToArray()));
        codeMemberMethod.Statements.Add((CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeManagerInvoke("AddEvent", codeExpressionList1.ToArray())));
        this.eventHandlerMethods[eventInfo] = codeMemberMethod;
      }
      return codeMemberMethod;
    }

    private CodeTypeMemberCollection GenerateAdapterInstanceFieldDeclarations()
    {
      if (this.adapterInstanceFieldMap.Count > 0)
      {
        ((IEnumerable<CodeMemberField>) this.adapterInstanceFieldMap.Values).First<CodeMemberField>().StartDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.Start, "Adapter Instances"));
        ((IEnumerable<CodeMemberField>) this.adapterInstanceFieldMap.Values).Last<CodeMemberField>().EndDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.End, ""));
      }
      return new CodeTypeMemberCollection((CodeTypeMember[]) ((IEnumerable<CodeMemberField>) this.adapterInstanceFieldMap.Values).ToArray<CodeMemberField>());
    }

    private CodeTypeMemberCollection GenerateExpectDelegateInstanceFieldDeclarations()
    {
      if (this.delegateInstanceFieldMap.Count > 0)
      {
        ((IEnumerable<CodeTypeDelegate>) this.delegateInstanceFieldMap.Values).First<CodeTypeDelegate>().StartDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.Start, "Expect Delegates"));
        ((IEnumerable<CodeTypeDelegate>) this.delegateInstanceFieldMap.Values).Last<CodeTypeDelegate>().EndDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.End, ""));
      }
      return new CodeTypeMemberCollection((CodeTypeMember[]) ((IEnumerable<CodeTypeDelegate>) this.delegateInstanceFieldMap.Values).ToArray<CodeTypeDelegate>());
    }

    private CodeTypeMemberCollection GenerateVariableDeclarations()
    {
      CodeTypeMemberCollection memberCollection = new CodeTypeMemberCollection();
      CodeMemberField codeMemberField1 = (CodeMemberField) null;
      CodeMemberField codeMemberField2 = (CodeMemberField) null;
      foreach (SerializableParameterExpression variable in this.transitionSystem.Variables)
      {
        if (!this.transitionSystem.IsPlaceholder(variable))
        {
          CodeMemberField codeMemberField3 = new CodeMemberField(new CodeTypeReference("IVariable", new CodeTypeReference[1]
          {
            TestCodeGenerateBase.MakeTypeReference(variable.ParameterType)
          }), variable.Name);
          memberCollection.Add((CodeTypeMember) codeMemberField3);
          if (codeMemberField1 == null)
            codeMemberField1 = codeMemberField3;
          codeMemberField2 = codeMemberField3;
        }
      }
      if (codeMemberField1 != null)
      {
        codeMemberField1.StartDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.Start, "Variables"));
        codeMemberField2.EndDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.End, ""));
      }
      return memberCollection;
    }

    private CodeTypeMemberCollection GenerateMetadataDeclarationsAndInitialization()
    {
      CodeTypeMemberCollection memberCollection = new CodeTypeMemberCollection();
      CodeMemberField codeMemberField1 = (CodeMemberField) null;
      CodeMemberField codeMemberField2 = (CodeMemberField) null;
      foreach (CodeMemberField codeMemberField3 in this.usedMetadataFields.Values)
      {
        memberCollection.Add((CodeTypeMember) codeMemberField3);
        if (codeMemberField1 == null)
          codeMemberField1 = codeMemberField3;
        codeMemberField2 = codeMemberField3;
      }
      if (codeMemberField1 != null)
      {
        codeMemberField1.StartDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.Start, "Event Metadata"));
        codeMemberField2.EndDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.End, ""));
      }
      return memberCollection;
    }

    private CodeMemberField GenerateMetadataDeclarationOrInitialization(
      SerializableMemberInfo member)
    {
      CodeTypeReference type1;
      switch (member)
      {
        case SerializableEventInfo _:
          type1 = new CodeTypeReference(typeof (EventInfo));
          break;
        case SerializableMethodInfo _:
        case SerializableConstructorInfo _:
          type1 = new CodeTypeReference(typeof (MethodBase));
          break;
        default:
          return (CodeMemberField) null;
      }
      CodeMemberField codeMemberField = new CodeMemberField(type1, this.MakeMetadataFieldName(member));
      codeMemberField.Attributes = MemberAttributes.Static;
      if (member is SerializableEventInfo)
      {
        codeMemberField.InitExpression = TestCodeGenerateBase.MakeHelperInvoke("GetEventInfo", (CodeExpression) new CodeTypeOfExpression(TestCodeGenerateBase.MakeTypeReference(member.DeclaringType)), TestCodeGenerateBase.MakeValue((object) member.Name));
      }
      else
      {
        SerializableMethodBase serializableMethodBase = member as SerializableMethodBase;
        List<CodeExpression> codeExpressionList = new List<CodeExpression>();
        codeExpressionList.Add((CodeExpression) new CodeTypeOfExpression(TestCodeGenerateBase.MakeTypeReference(member.DeclaringType)));
        if (member is SerializableMethodInfo)
          codeExpressionList.Add(TestCodeGenerateBase.MakeValue((object) member.Name));
        foreach (SerializableParameterInfo parameter in serializableMethodBase.Parameters)
        {
          SerializableType type2 = parameter.Type;
          bool isByRef = type2.IsByRef;
          if (isByRef)
            type2 = type2.ElementType;
          CodeExpression targetObject = (CodeExpression) new CodeTypeOfExpression(TestCodeGenerateBase.MakeTypeReference(type2));
          if (isByRef)
            targetObject = (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject, "MakeByRefType"), new CodeExpression[0]);
          codeExpressionList.Add(targetObject);
        }
        codeMemberField.InitExpression = TestCodeGenerateBase.MakeHelperInvoke(member is SerializableConstructorInfo ? "GetConstructorInfo" : "GetMethodInfo", codeExpressionList.ToArray());
      }
      return codeMemberField;
    }

    protected CodeTypeDelegate GenerateDelegateFieldInstance(
      SerializableMemberInfo member)
    {
      CodeTypeDelegate codeTypeDelegate = new CodeTypeDelegate(this.MakeUnique(this.MakeCheckerDelegateName(member)));
      codeTypeDelegate.Attributes |= MemberAttributes.Public;
      codeTypeDelegate.Parameters.AddRange(this.GenerateParameterDeclarationExpressions(member));
      return codeTypeDelegate;
    }

    private CodeParameterDeclarationExpressionCollection GenerateParameterDeclarationExpressions(
      SerializableMemberInfo member)
    {
      CodeParameterDeclarationExpressionCollection expressionCollection = new CodeParameterDeclarationExpressionCollection();
      if (!member.IsStatic && !this.adapterTypes.Contains(member.DeclaringType))
        expressionCollection.Add(new CodeParameterDeclarationExpression(member.DeclaringType.FullName, "this"));
      SerializableMethodBase serializableMethodBase = member as SerializableMethodBase;
      SerializableMethodInfo serializableMethodInfo = member as SerializableMethodInfo;
      foreach (SerializableParameterInfo serializableParameterInfo in serializableMethodBase != null ? serializableMethodBase.Parameters : ((SerializableEventInfo) member).Parameters)
      {
        if (serializableMethodBase == null)
          expressionCollection.Add(new CodeParameterDeclarationExpression(TestCodeGenerateBase.MakeTypeReference(serializableParameterInfo.Type), serializableParameterInfo.Name));
        else if (serializableParameterInfo.Type.IsByRef)
          expressionCollection.Add(new CodeParameterDeclarationExpression(TestCodeGenerateBase.MakeTypeReference(serializableParameterInfo.Type.ElementType), serializableParameterInfo.Name));
      }
      if (serializableMethodInfo != null && serializableMethodInfo.ReturnType != null)
        expressionCollection.Add(new CodeParameterDeclarationExpression(serializableMethodInfo.ReturnType.FullName, "return"));
      return expressionCollection;
    }

    private string MakeCheckerMethod(
      Transition transition,
      CodeTypeDelegate checkerDelegate,
      out CodeMemberMethod checkerMethod)
    {
      SerializableMemberInfo method = this.methodMap[transition.Action.Symbol.Member.Header];
      checkerMethod = new CodeMemberMethod();
      checkerMethod.Name = this.MakeCheckerMethodName(this.currentTestMethodName, method);
      CodeParameterDeclarationExpressionCollection parameters = checkerDelegate.Parameters;
      checkerMethod.Parameters.AddRange(parameters);
      this.AddChecker(checkerMethod.Statements, transition, parameters, (List<CodeExpression>) null);
      return checkerMethod.Name;
    }

    private void AddChecker(
      CodeStatementCollection stms,
      Transition transition,
      CodeParameterDeclarationExpressionCollection parameters,
      List<CodeExpression> actuals)
    {
      this.AddChecker(stms, transition, parameters, actuals, new List<SerializableExpression>((IEnumerable<SerializableExpression>) transition.Action.Arguments));
    }

    private void AddChecker(
      CodeStatementCollection stms,
      Transition transition,
      CodeParameterDeclarationExpressionCollection parameters,
      List<CodeExpression> actuals,
      List<SerializableExpression> args)
    {
      this.AddComment(stms, string.Format("checking step '{0}'", (object) this.ToString(transition.Action)));
      ActionSymbol symbol = transition.Action.Symbol;
      SerializableMemberInfo method = this.methodMap[symbol.Member.Header];
      SerializableMethodInfo serializableMethodInfo = method as SerializableMethodInfo;
      bool flag1 = method is SerializableConstructorInfo;
      Dictionary<CodeParameterDeclarationExpression, SerializableExpression> dictionary = new Dictionary<CodeParameterDeclarationExpression, SerializableExpression>();
      int index1 = 0;
      int index2 = 0;
      if (symbol.Kind != ActionSymbolKind.Event && symbol.Kind != ActionSymbolKind.Return)
        throw new TestCodeGenerationException(string.Format("unexpected action '{0}'", (object) ((object) transition.Action).ToString()));
      if (!method.IsStatic && !this.adapterTypes.Contains(method.DeclaringType))
      {
        if (flag1)
          dictionary[parameters[index1]] = args[index2];
        ++index1;
        ++index2;
      }
      else if (!method.IsStatic)
        ++index2;
      bool flag2 = serializableMethodInfo != null && serializableMethodInfo.ReturnType != null;
      int num1 = flag2 ? parameters.Count - 1 : parameters.Count;
      while (index1 < num1)
      {
        dictionary[parameters[index1]] = args[index2];
        ++index1;
        ++index2;
      }
      if (flag2)
      {
        dictionary[parameters[index1]] = args[index2];
        int num2 = index1 + 1;
        int num3 = index2 + 1;
      }
      int index3 = 0;
      CodeStatementCollection statementCollection1 = new CodeStatementCollection();
      foreach (CodeParameterDeclarationExpression parameter in (CollectionBase) parameters)
      {
        CodeExpression actual = actuals != null ? actuals[index3] : (CodeExpression) new CodeVariableReferenceExpression(parameter.Name);
        SerializableExpression expected;
        if (dictionary.TryGetValue(parameter, out expected) && expected != null)
        {
          string str = !(method is SerializableConstructorInfo) ? method.Name : method.DeclaringType.FullName;
          this.AddAssertEquality(statementCollection1, parameter.Type, actual, expected, string.Format("{0} of {1}, state {2}", (object) parameter.Name, (object) str, (object) transition.Source));
        }
        ++index3;
      }
      if (statementCollection1.Count > 0)
      {
        CodeStatementCollection statementCollection2 = new CodeStatementCollection();
        if (transition.CapturedRequirements != null && transition.CapturedRequirements.Length > 0)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append("This step would have covered ");
          bool flag3 = true;
          foreach (string capturedRequirement in transition.CapturedRequirements)
          {
            if (flag3)
            {
              flag3 = false;
              stringBuilder.Append(capturedRequirement);
            }
            else
              stringBuilder.AppendFormat(", {0}", (object) capturedRequirement);
          }
          this.AddComment(statementCollection2, stringBuilder.ToString());
        }
        if (statementCollection2.Count > 0)
        {
          statementCollection2.Add((CodeStatement) new CodeThrowExceptionStatement());
          CodeTryCatchFinallyStatement finallyStatement = new CodeTryCatchFinallyStatement(TestCodeGenerateBase.MakeArray(statementCollection1), new CodeCatchClause[1]
          {
            new CodeCatchClause((string) null, new CodeTypeReference("TransactionFailedException"), TestCodeGenerateBase.MakeArray(statementCollection2))
          });
          stms.Add((CodeStatement) finallyStatement);
        }
        else
          stms.AddRange(statementCollection1);
      }
      this.AddAssumptions(stms, transition);
      this.AddCheckpoints(stms, transition);
      this.AddVariableUnbinding(stms, transition);
    }

    private void AddAssertEquality(
      CodeStatementCollection statements,
      CodeTypeReference type,
      CodeExpression actual,
      SerializableExpression expected,
      string context)
    {
      this.AddAssertEquality(statements, type, actual, expected, context, "");
    }

    private void AddAssertEquality(
      CodeStatementCollection statements,
      CodeTypeReference type,
      CodeExpression actual,
      SerializableExpression expected,
      string context,
      string selector)
    {
      if (expected is SerializableParameterExpression && this.transitionSystem.IsPlaceholder((SerializableParameterExpression) expected))
        return;
      if (expected.NodeType == ExpressionType.MemberInit)
      {
        SerializableMemberInitExpression memberInitExpression = (SerializableMemberInitExpression) expected;
        this.AddAssertEquality(statements, actual, (IEnumerable<SerializableMemberBinding>) memberInitExpression.Bindings, context, selector);
      }
      else
      {
        string contextAndSelector = TestCodeGenerateBase.MakeContextAndSelector(context, selector);
        switch (expected)
        {
          case SerializableBinaryExpression _:
            SerializableBinaryExpression expected1 = expected as SerializableBinaryExpression;
            this.AddBindingOrAssertionStatementForBinaryExpression(statements, type, actual, expected1, contextAndSelector);
            break;
          case SerializableUnaryExpression _:
            SerializableUnaryExpression serializableUnaryExpression = expected as SerializableUnaryExpression;
            if (((SerializableExpression) serializableUnaryExpression).NodeType == ExpressionType.Convert && serializableUnaryExpression.Operand is SerializableParameterExpression)
            {
              SerializableParameterExpression operand = (SerializableParameterExpression) serializableUnaryExpression.Operand;
              CodeSnippetExpression snippetExpression = new CodeSnippetExpression(string.Format("({0}){1}", (object) operand.ParameterType.FullName, (object) this.CodeExpressionToString(actual)));
              this.AddBindingOrAssertionStatement(statements, TestCodeGenerateBase.MakeTypeReference(operand.ParameterType), (CodeExpression) snippetExpression, (SerializableExpression) operand, contextAndSelector);
              break;
            }
            this.AddBindingOrAssertionStatement(statements, type, actual, expected, contextAndSelector);
            break;
          default:
            this.AddBindingOrAssertionStatement(statements, type, actual, expected, contextAndSelector);
            break;
        }
      }
    }

    private void AddBindingOrAssertionStatement(
      CodeStatementCollection statements,
      CodeTypeReference type,
      CodeExpression actual,
      SerializableExpression expected,
      string contextAndSelector)
    {
      if (expected is SerializableParameterExpression)
      {
        SerializableParameterExpression parameterExpression = (SerializableParameterExpression) expected;
        statements.Add(this.MakeAssertBind(type, TestCodeGenerateBase.MakeThisReference(parameterExpression.Name), actual, TestCodeGenerateBase.MakeValue((object) contextAndSelector)));
        this.InstallEventHandlerToInstance(statements, this.ExpressionToCode(expected), parameterExpression.ParameterType, true);
      }
      else
        statements.Add(this.MakeAssertAreEqual(type, this.ExpressionToCode(expected), actual, TestCodeGenerateBase.MakeValue((object) contextAndSelector)));
    }

    private void AddAssertEquality(
      CodeStatementCollection statements,
      CodeExpression actual,
      IEnumerable<SerializableMemberBinding> bindings,
      string context,
      string selector)
    {
      bool flag = false;
      foreach (SerializableMemberBinding binding in bindings)
      {
        SerializableMemberAssignment memberAssignment = binding.BindingType == MemberBindingType.Assignment ? (SerializableMemberAssignment) binding : throw new TestCodeGenerationException(string.Format("{0}: {1}", (object) "Cannot process member bindings of this kind", (object) binding));
        SerializableFieldInfo member = (SerializableFieldInfo) ((SerializableMemberBinding) memberAssignment).Member;
        if (((SerializableMemberInfo) member).DeclaringType.TypeCode == TypeCode.Object && !flag)
        {
          flag = true;
          statements.Add(this.MakeAssertNotNull(actual, TestCodeGenerateBase.MakeValue((object) TestCodeGenerateBase.MakeContextAndSelector(context, selector))));
        }
        CodeExpression codeExpression = this.MakeFieldSelection(actual, member);
        if (memberAssignment.Expression.NodeType == ExpressionType.MemberInit)
        {
          string str = this.NewTemporary();
          statements.Add((CodeStatement) new CodeVariableDeclarationStatement(TestCodeGenerateBase.MakeTypeReference(member.Type), str, codeExpression));
          codeExpression = (CodeExpression) new CodeVariableReferenceExpression(str);
        }
        this.AddAssertEquality(statements, TestCodeGenerateBase.MakeTypeReference(member.Type), codeExpression, memberAssignment.Expression, context, string.IsNullOrEmpty(selector) ? ((SerializableMemberInfo) member).Name : selector + "." + ((SerializableMemberInfo) member).Name);
      }
    }

    private void AddBindingOrAssertionStatementForBinaryExpression(
      CodeStatementCollection statements,
      CodeTypeReference type,
      CodeExpression actual,
      SerializableBinaryExpression expected,
      string contextAndSelector)
    {
      if (((SerializableExpression) expected).Type.IsBoolean() && ((SerializableExpression) expected).NodeType == ExpressionType.NotEqual)
      {
        if (expected.Left is SerializableParameterExpression && expected.Right is SerializableConstantExpression && ((SerializableConstantExpression) expected.Right).Value == "0")
        {
          SerializableParameterExpression left = (SerializableParameterExpression) expected.Left;
          actual = (CodeExpression) new CodeSnippetExpression(string.Format("System.Convert.ToInt32({0})", (object) this.CodeExpressionToString(actual)));
          statements.Add(this.MakeAssertBind(new CodeTypeReference("System.Int32"), TestCodeGenerateBase.MakeThisReference(left.Name), actual, TestCodeGenerateBase.MakeValue((object) contextAndSelector)));
        }
        else if (expected.Right is SerializableParameterExpression && expected.Left is SerializableConstantExpression && ((SerializableConstantExpression) expected.Left).Value == "0")
        {
          SerializableParameterExpression right = (SerializableParameterExpression) expected.Right;
          actual = (CodeExpression) new CodeSnippetExpression(string.Format("System.Convert.ToInt32({0})", (object) this.CodeExpressionToString(actual)));
          statements.Add(this.MakeAssertBind(new CodeTypeReference("System.Int32"), TestCodeGenerateBase.MakeThisReference(right.Name), actual, TestCodeGenerateBase.MakeValue((object) contextAndSelector)));
        }
        else
          this.AddBindingOrAssertionStatement(statements, type, actual, (SerializableExpression) expected, contextAndSelector);
      }
      else
        this.AddBindingOrAssertionStatement(statements, type, actual, (SerializableExpression) expected, contextAndSelector);
    }

    private static string MakeContextAndSelector(string context, string selector) => !string.IsNullOrEmpty(selector) ? string.Format("{0}, field selection {1}", (object) context, (object) selector) : context;

    private void InstallEventHandlerToInstance(
      CodeStatementCollection statements,
      CodeExpression target,
      SerializableType targetType,
      bool checkNull)
    {
      foreach (SerializableEventInfo eventInfo in ((IEnumerable) this.transitionSystem.ActionMembers).OfType<SerializableEventInfo>().Where<SerializableEventInfo>((Func<SerializableEventInfo, bool>) (m => !((SerializableMemberInfo) m).IsStatic && TestCodeGenerateBase.IsEventFeasibleToType((SerializableMemberInfo) m, targetType))))
      {
        if (string.Compare(this.generateEventHandlers, "true", true) == 0)
        {
          CodeStatement eventAttachStatement;
          CodeStatement eventRemovalStatement;
          this.GenerateEventAttachAndRemovalStatement(eventInfo, false, target, out eventAttachStatement, out eventRemovalStatement);
          if (!checkNull)
          {
            statements.Add(eventAttachStatement);
            this.instanceEventRemovalStatements.Add(eventRemovalStatement);
          }
          else
          {
            CodeStatement codeStatement = (CodeStatement) new CodeConditionStatement((CodeExpression) new CodeBinaryOperatorExpression(target, CodeBinaryOperatorType.IdentityInequality, (CodeExpression) new CodeSnippetExpression("null")), new CodeStatement[1]
            {
              eventAttachStatement
            });
            statements.Add(codeStatement);
            this.instanceEventRemovalStatements.Add((CodeStatement) new CodeConditionStatement((CodeExpression) new CodeBinaryOperatorExpression(target, CodeBinaryOperatorType.IdentityInequality, (CodeExpression) new CodeSnippetExpression("null")), new CodeStatement[1]
            {
              eventRemovalStatement
            }));
          }
        }
        else
        {
          CodeStatement codeStatement1 = (CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeManagerInvoke("Subscribe", new List<CodeExpression>()
          {
            (CodeExpression) new CodeSnippetExpression(this.GetMetadataField((SerializableMemberInfo) eventInfo).Name),
            target
          }.ToArray()));
          if (!checkNull)
          {
            statements.Add(codeStatement1);
          }
          else
          {
            CodeStatement codeStatement2 = (CodeStatement) new CodeConditionStatement((CodeExpression) new CodeBinaryOperatorExpression(target, CodeBinaryOperatorType.IdentityInequality, (CodeExpression) new CodeSnippetExpression("null")), new CodeStatement[1]
            {
              codeStatement1
            });
            statements.Add(codeStatement2);
          }
        }
      }
    }

    private static bool IsEventFeasibleToType(SerializableMemberInfo @event, SerializableType type)
    {
      if (@event.DeclaringType.FullName == type.FullName)
        return true;
      return type.BaseType != null && TestCodeGenerateBase.IsEventFeasibleToType(@event, type.BaseType);
    }

    protected void AddVariableUnbinding(CodeStatementCollection statements, Transition trans)
    {
      if (trans == null || trans.VariablesToUnbind == null)
        return;
      foreach (SerializableParameterExpression parameterExpression in trans.VariablesToUnbind)
      {
        if (parameterExpression != null)
        {
          this.AddComment(statements, string.Format("Unbinding variable '{0}'", (object) parameterExpression.Name));
          statements.Add((CodeExpression) new CodeMethodInvokeExpression(TestCodeGenerateBase.MakeThisReference(parameterExpression.Name), "Unbind", new CodeExpression[0]));
        }
      }
    }

    private CodeExpression ExpressionToCode(SerializableExpression expression)
    {
      if (expression == null)
        throw new ArgumentNullException(nameof (expression));
      switch ((int) expression.NodeType)
      {
        case 0:
        case 1:
        case 2:
        case 3:
        case 5:
        case 7:
        case 12:
        case 13:
        case 14:
        case 15:
        case 16:
        case 19:
        case 20:
        case 21:
        case 25:
        case 26:
        case 27:
        case 35:
        case 36:
        case 37:
        case 41:
        case 42:
        case 43:
          return this.ExpressionToCode((SerializableBinaryExpression) expression);
        case 4:
        case 10:
        case 11:
        case 28:
        case 30:
        case 34:
        case 40:
        case 44:
          return this.ExpressionToCode((SerializableUnaryExpression) expression);
        case 6:
          return this.ExpressionToCode((SerializableMethodCallExpression) expression);
        case 8:
          return this.ExpressionToCode((SerializableConditionalExpression) expression);
        case 9:
          return expression is SerializableConstantExpression ? this.ExpressionToCode((SerializableConstantExpression) expression) : this.ExpressionToCode((SerializableEnumExpression) expression);
        case 23:
          return this.ExpressionToCode((SerializableMemberExpression) expression);
        case 24:
          return this.ExpressionToCode((SerializableMemberInitExpression) expression);
        case 31:
          return this.ExpressionToCode((SerializableNewExpression) expression);
        case 32:
        case 33:
          return this.ExpressionToCode((SerializableNewArrayExpression) expression);
        case 38:
          return this.ExpressionToCode((SerializableParameterExpression) expression);
        case 45:
          return this.ExpressionToCode((SerializableTypeBinaryExpression) expression);
        default:
          throw new TestCodeGenerationException(string.Format("{0}: {1}", (object) "Unsupported expression", (object) expression));
      }
    }

    private CodeExpression ExpressionToCode(SerializableBinaryExpression b)
    {
      CodeExpression code1 = this.ExpressionToCode(b.Left);
      CodeExpression code2 = this.ExpressionToCode(b.Right);
      switch ((int) ((SerializableExpression) b).NodeType)
      {
        case 0:
        case 1:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.Add, code2);
        case 2:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.BitwiseAnd, code2);
        case 3:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.BooleanAnd, code2);
        case 5:
          return (CodeExpression) new CodeArrayIndexerExpression(code1, new CodeExpression[1]
          {
            code2
          });
        case 12:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.Divide, code2);
        case 13:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.ValueEquality, code2);
        case 14:
          return TestCodeGenerateBase.CodeExpressionFromString(string.Format("({0}) ^ ({1})", (object) this.CodeExpressionToString(code1), (object) this.CodeExpressionToString(code2)));
        case 15:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.GreaterThan, code2);
        case 16:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.GreaterThanOrEqual, code2);
        case 19:
          return TestCodeGenerateBase.CodeExpressionFromString(string.Format("({0}) << ({1})", (object) this.CodeExpressionToString(code1), (object) this.CodeExpressionToString(code2)));
        case 20:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.LessThan, code2);
        case 21:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.LessThanOrEqual, code2);
        case 25:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.Modulus, code2);
        case 26:
        case 27:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.Multiply, code2);
        case 35:
          return (CodeExpression) new CodeBinaryOperatorExpression((CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.ValueEquality, code2), CodeBinaryOperatorType.ValueEquality, (CodeExpression) new CodePrimitiveExpression((object) false));
        case 36:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.BitwiseOr, code2);
        case 37:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.BooleanOr, code2);
        case 41:
          return TestCodeGenerateBase.CodeExpressionFromString(string.Format("({0}) >> ({1})", (object) this.CodeExpressionToString(code1), (object) this.CodeExpressionToString(code2)));
        case 42:
        case 43:
          return (CodeExpression) new CodeBinaryOperatorExpression(code1, CodeBinaryOperatorType.Subtract, code2);
        default:
          throw new TestCodeGenerationException(string.Format("{0}: {1}", (object) "Unsupported binary expression", (object) b));
      }
    }

    private CodeExpression ExpressionToCode(SerializableUnaryExpression u)
    {
      CodeExpression code = this.ExpressionToCode(u.Operand);
      ExpressionType nodeType = ((SerializableExpression) u).NodeType;
      if ((int)nodeType <= 30)
      {
        if (nodeType == ExpressionType.ArrayLength)
          return TestCodeGenerateBase.CodeExpressionFromString(string.Format("({0}).Length", (object) this.CodeExpressionToString(code)));
        switch (nodeType - 10)
        {
          case 0:
          case ExpressionType.AddAssign:
            return (CodeExpression) new CodeCastExpression(TestCodeGenerateBase.MakeTypeReference(((SerializableExpression) u).Type), code);
          default:
            switch (nodeType - 28)
            {
              case 0:
              case ExpressionType.AddAssignChecked:
                return (CodeExpression) new CodeBinaryOperatorExpression((CodeExpression) new CodeDefaultValueExpression(TestCodeGenerateBase.MakeTypeReference(((SerializableExpression) u).Type)), CodeBinaryOperatorType.Subtract, code);
            }
            break;
        }
      }
      else if (nodeType != ExpressionType.Not)
      {
        if (nodeType != ExpressionType.Quote && nodeType == ExpressionType.TypeAs)
          return (CodeExpression) new CodeCastExpression(TestCodeGenerateBase.MakeTypeReference(((SerializableExpression) u).Type), code);
      }
      else
        return u.Operand.Type.FullName == "System.Int32" ? TestCodeGenerateBase.CodeExpressionFromString(string.Format("~({0})", (object) this.CodeExpressionToString(code))) : TestCodeGenerateBase.CodeExpressionFromString(string.Format("!({0})", (object) this.CodeExpressionToString(code)));
      throw new TestCodeGenerationException(string.Format("{0}: {1}", (object) "Unsupported unary expression", (object) u));
    }

    private CodeExpression ExpressionToCode(SerializableConstantExpression c)
    {
      if (System.Type.GetType(c.ValueType.FullName) == (System.Type) null)
        throw new TestCodeGenerationException(string.Format("{0}: {1}", (object) "Cannot discover type of constant expression", (object) c));
      return (CodeExpression) new CodePrimitiveExpression(((ConstantExpression) ((SerializableExpression) c).ToExpression((Func<string, System.Type>) (t => System.Type.GetType(t)))).Value);
    }

    private CodeExpression ExpressionToCode(SerializableEnumExpression e) => TestCodeGenerateBase.CodeExpressionFromString(e.Value.Replace("+", "."));

    private CodeExpression ExpressionToCode(SerializableTypeBinaryExpression b) => TestCodeGenerateBase.CodeExpressionFromString(string.Format("({0}) is {1}", (object) this.CodeExpressionToString(this.ExpressionToCode(b.Expression)), (object) this.TypeToCode(b.TypeOperand)));

    private string TypeToCode(SerializableType serializableType)
    {
      StringWriter stringWriter = new StringWriter();
      this.generator.GenerateCodeFromExpression((CodeExpression) new CodeTypeReferenceExpression(serializableType.FullName), (TextWriter) stringWriter, this.generatorOptions);
      return stringWriter.ToString();
    }

    private CodeExpression ExpressionToCode(SerializableConditionalExpression c) => TestCodeGenerateBase.CodeExpressionFromString(string.Format("({0}) ? ({1}) : ({2})", (object) this.CodeExpressionToString(this.ExpressionToCode(c.Test)), (object) this.CodeExpressionToString(this.ExpressionToCode(c.IfTrue)), (object) this.CodeExpressionToString(this.ExpressionToCode(c.IfFalse))));

    private CodeExpression ExpressionToCode(SerializableParameterExpression p) => !this.transitionSystem.IsPlaceholder(p) ? (CodeExpression) new CodePropertyReferenceExpression(TestCodeGenerateBase.MakeThisReference(p.Name), "Value") : throw new TestCodeGenerationException(string.Format("{0}: {1}", (object) "Placeholder is not supported in test code generation", (object) p));

    private CodeExpression ExpressionToCode(SerializableMemberExpression m)
    {
      CodeExpression code = this.ExpressionToCode(m.Expression);
      return m.Member is SerializableFieldInfo ? this.MakeFieldSelection(code, (SerializableFieldInfo) m.Member) : (CodeExpression) new CodePropertyReferenceExpression(code, m.Member.Name);
    }

    private CodeExpression ExpressionToCode(SerializableMethodCallExpression c)
    {
      CodeExpression targetObject1 = c.Object != null ? this.ExpressionToCode(c.Object) : (CodeExpression) null;
      string fullMemberName = this.GetFullMemberName((SerializableMemberInfo) c.Method);
      if (targetObject1 == null && fullMemberName == "Microsoft.Xrt.Runtime.RuntimeSupport.Create" && (c.Arguments != null && c.Arguments.Length == 2) && (c.Arguments[0].NodeType == ExpressionType.NewArrayInit && c.Arguments[1].NodeType == ExpressionType.NewArrayInit))
      {
        CodeExpression targetObject2 = (CodeExpression) new CodeThisReferenceExpression();
        SerializableExpression[] expressions1 = ((SerializableNewArrayExpression) c.Arguments[0]).Expressions;
        SerializableExpression[] expressions2 = ((SerializableNewArrayExpression) c.Arguments[1]).Expressions;
        return (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject2, "Make", ((IEnumerable<SerializableType>) c.Method.TypeParameters).Select<SerializableType, CodeTypeReference>((Func<SerializableType, CodeTypeReference>) (t => TestCodeGenerateBase.MakeTypeReference(t))).ToArray<CodeTypeReference>()), new CodeExpression[2]
        {
          (CodeExpression) new CodeArrayCreateExpression(new CodeTypeReference(typeof (string)), ((IEnumerable<SerializableExpression>) expressions1).Select<SerializableExpression, CodeExpression>((Func<SerializableExpression, CodeExpression>) (e => this.ExpressionToCode(this.RemoveObjectCast(e)))).ToArray<CodeExpression>()),
          (CodeExpression) new CodeArrayCreateExpression(new CodeTypeReference(typeof (object)), ((IEnumerable<SerializableExpression>) expressions2).Select<SerializableExpression, CodeExpression>((Func<SerializableExpression, CodeExpression>) (e => this.ExpressionToCode(this.RemoveObjectCast(e)))).ToArray<CodeExpression>())
        });
      }
      CodeExpression[] codeExpressionArray = c.Arguments != null ? ((IEnumerable<SerializableExpression>) c.Arguments).Select<SerializableExpression, CodeExpression>((Func<SerializableExpression, CodeExpression>) (e => this.ExpressionToCode(e))).ToArray<CodeExpression>() : new CodeExpression[0];
      return (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject1, fullMemberName, ((IEnumerable<SerializableType>) c.Method.TypeParameters).Select<SerializableType, CodeTypeReference>((Func<SerializableType, CodeTypeReference>) (t => TestCodeGenerateBase.MakeTypeReference(t))).ToArray<CodeTypeReference>()), codeExpressionArray);
    }

    private SerializableExpression RemoveObjectCast(SerializableExpression e)
    {
      if (e.NodeType == ExpressionType.Convert)
      {
        SerializableUnaryExpression serializableUnaryExpression = (SerializableUnaryExpression) e;
        if (((SerializableExpression) serializableUnaryExpression).Type != null && ((SerializableExpression) serializableUnaryExpression).Type.FullName == "System.Object")
          return serializableUnaryExpression.Operand;
      }
      return e;
    }

    private CodeExpression ExpressionToCode(SerializableNewExpression n) => (CodeExpression) new CodeObjectCreateExpression(TestCodeGenerateBase.MakeTypeReference(((SerializableMemberInfo) n.Constructor).DeclaringType), n.Arguments != null ? ((IEnumerable<SerializableExpression>) n.Arguments).Select<SerializableExpression, CodeExpression>((Func<SerializableExpression, CodeExpression>) (e => this.ExpressionToCode(e))).ToArray<CodeExpression>() : new CodeExpression[0]);

    private CodeExpression ExpressionToCode(SerializableNewArrayExpression n)
    {
      if (((SerializableExpression) n).NodeType != ExpressionType.NewArrayBounds)
        return (CodeExpression) new CodeArrayCreateExpression(TestCodeGenerateBase.MakeTypeReference(n.ArrayType), ((IEnumerable<SerializableExpression>) n.Expressions).Select<SerializableExpression, CodeExpression>((Func<SerializableExpression, CodeExpression>) (e => this.ExpressionToCode(e))).ToArray<CodeExpression>());
      if (n.Expressions.Length > 1 || n.Expressions.Length == 0)
        throw new TestCodeGenerationException(string.Format("{0}: {1}", (object) "Unsupported multi-dimensional array creation", (object) n));
      return (CodeExpression) new CodeArrayCreateExpression(TestCodeGenerateBase.MakeTypeReference(n.ArrayType), this.ExpressionToCode(n.Expressions[0]));
    }

    private CodeExpression ExpressionToCode(SerializableMemberInitExpression m)
    {
      List<CodeExpression> codeExpressionList1 = new List<CodeExpression>();
      List<CodeExpression> codeExpressionList2 = new List<CodeExpression>();
      foreach (SerializableMemberBinding binding in m.Bindings)
      {
        if (binding.BindingType != MemberBindingType.Assignment)
          throw new TestCodeGenerationException(string.Format("{0}: {1}", (object) "Unsupported member binding", (object) binding));
        codeExpressionList2.Add(TestCodeGenerateBase.MakeValue((object) binding.Member.Name));
        codeExpressionList1.Add(this.ExpressionToCode(((SerializableMemberAssignment) binding).Expression));
      }
      return (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(TestCodeGenerateBase.MakeThis(), "Make", new CodeTypeReference[1]
      {
        TestCodeGenerateBase.MakeTypeReference(((SerializableMemberInfo) m.NewExpression.Constructor).DeclaringType)
      }), new CodeExpression[2]
      {
        (CodeExpression) new CodeArrayCreateExpression(new CodeTypeReference(typeof (string)), codeExpressionList2.ToArray()),
        (CodeExpression) new CodeArrayCreateExpression(new CodeTypeReference(typeof (object)), codeExpressionList1.ToArray())
      });
    }

    private static CodeExpression CodeExpressionFromString(string s) => (CodeExpression) new CodeSnippetExpression(s);

    private string ExpressionToString(SerializableExpression expr) => this.CodeExpressionToString(this.ExpressionToCode(expr));

    protected void AddComment(CodeStatementCollection statements, string comment) => this.AddComment(statements, TestCodeGenerateBase.MakeValue((object) comment));

    protected void AddComment(CodeStatementCollection statements, CodeExpression parameter)
    {
      if (string.Compare("true", this.suppressGeneratedTestLogging, true) == 0)
        return;
      statements.Add((CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeManagerInvoke("Comment", parameter)));
    }

    protected void AddCheckpoints(CodeStatementCollection stms, Transition trans)
    {
      foreach (string capturedRequirement in trans.CapturedRequirements)
        stms.Add((CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeManagerInvoke("Checkpoint", TestCodeGenerateBase.MakeValue((object) capturedRequirement))));
    }

    private CodeExpression MakeFieldSelection(
      CodeExpression value,
      SerializableFieldInfo field)
    {
      if (((SerializableMemberInfo) field).IsPublic)
        return (CodeExpression) new CodeFieldReferenceExpression(value, ((SerializableMemberInfo) field).Name);
      this.AddGetFieldMethod();
      return (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) null, this.getFieldMethod.Name, new CodeTypeReference[1]
      {
        TestCodeGenerateBase.MakeTypeReference(field.Type)
      }), new CodeExpression[2]
      {
        value,
        (CodeExpression) new CodePrimitiveExpression((object) ((SerializableMemberInfo) field).Name)
      });
    }

    private void AddGetFieldMethod()
    {
      if (this.getFieldMethod != null)
        return;
      CodeTypeParameter typeParameter = new CodeTypeParameter("T");
      this.getFieldMethod = new CodeMemberMethod();
      this.getFieldMethod.Name = "GetField";
      this.getFieldMethod.Attributes = MemberAttributes.Static | MemberAttributes.Private;
      this.getFieldMethod.ReturnType = new CodeTypeReference(typeParameter);
      this.getFieldMethod.TypeParameters.Add(typeParameter);
      this.getFieldMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof (object)), "obj"));
      this.getFieldMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof (string)), "fieldName"));
      CodeStatementCollection statements = this.getFieldMethod.Statements;
      CodeExpression[] codeExpressionArray = new CodeExpression[2]
      {
        (CodeExpression) new CodeArgumentReferenceExpression("obj"),
        (CodeExpression) new CodeArgumentReferenceExpression("fieldName")
      };
      statements.Add((CodeStatement) new CodeMethodReturnStatement((CodeExpression) new CodeCastExpression(new CodeTypeReference(typeParameter), TestCodeGenerateBase.MakeHelperInvoke("GetFieldValueByName", codeExpressionArray))));
    }

    private CodeExpression MakeAssertAreEqual(
      CodeTypeReference type,
      CodeExpression expected,
      CodeExpression actual,
      CodeExpression context)
    {
      return TestCodeGenerateBase.MakeHelperInvoke("AssertAreEqual", new CodeTypeReference[1]
      {
        type
      }, TestCodeGenerateBase.MakeThisReference("Manager"), expected, actual, context);
    }

    private CodeExpression MakeAssertNotNull(
      CodeExpression actual,
      CodeExpression context)
    {
      return TestCodeGenerateBase.MakeHelperInvoke("AssertNotNull", TestCodeGenerateBase.MakeThisReference("Manager"), actual, context);
    }

    private CodeExpression MakeAssertBind(
      CodeTypeReference type,
      CodeExpression var,
      CodeExpression actual,
      CodeExpression context)
    {
      return TestCodeGenerateBase.MakeHelperInvoke("AssertBind", new CodeTypeReference[1]
      {
        type
      }, TestCodeGenerateBase.MakeThisReference("Manager"), var, actual, context);
    }

    private CodeExpression MakeManagerReference(string name) => (CodeExpression) new CodeFieldReferenceExpression(TestCodeGenerateBase.MakeThisReference("Manager"), name);

    private static CodeExpression MakeHelperInvoke(
      string name,
      CodeTypeReference[] typeArgs,
      params CodeExpression[] parameters)
    {
      return (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) new CodeTypeReferenceExpression("TestManagerHelpers"), name, typeArgs), parameters);
    }

    private string NewTemporary() => string.Format("temp{0}", (object) this.tempCounter++);

    private string NewLabel() => string.Format("label{0}", (object) this.labelCounter++);

    private void ResetTemporaryCounters()
    {
      this.tempCounter = 0;
      this.labelCounter = 0;
    }

    private string ToString(ActionInvocation action) => action.Text;

    protected void Warning(string message, params object[] parameters) => this.host.DiagMessage(DiagnosisKind.Warning, string.Format(message, parameters), (object) null);

    private string GetFullMemberName(SerializableMemberInfo member) => member.IsStatic && !this.adapterTypes.Contains(member.DeclaringType) ? string.Format("{0}.{1}", (object) member.DeclaringType.FullName, (object) member.Name) : member.Name;

    private static CodeExpression MakeEquals(
      TypeCode typeCode,
      CodeExpression left,
      CodeExpression right)
    {
      if (typeCode != TypeCode.Object)
        return (CodeExpression) new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.IdentityEquality, right);
      return (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) null, "Object.Equals"), new CodeExpression[2]
      {
        left,
        right
      });
    }

    private CodeExpression MakeNot(CodeExpression operand) => (CodeExpression) new CodeSnippetExpression(string.Format("!({0})", (object) this.CodeExpressionToString(operand)));

    private CodeStatementCollection MakeSwitch(
      CodeExpression selector,
      List<CodeExpression> caseLabels,
      List<CodeStatementCollection> caseStms,
      CodeStatementCollection defaultStms)
    {
      if (defaultStms == null)
        defaultStms = TestCodeGenerateBase.MakeStms((CodeStatement) new CodeThrowExceptionStatement((CodeExpression) new CodeObjectCreateExpression("InvalidOperationException", new CodeExpression[1]
        {
          TestCodeGenerateBase.MakeValue((object) "never reached")
        })));
      CodeStatementCollection statementCollection = new CodeStatementCollection();
      string label = this.NewLabel();
      for (int index = 0; index < caseLabels.Count; ++index)
        statementCollection.Add((CodeStatement) new CodeConditionStatement(TestCodeGenerateBase.MakeEquals(TypeCode.Int32, selector, caseLabels[index]), TestCodeGenerateBase.MakeArray(new CodeStatementCollection(caseStms[index])
        {
          (CodeStatement) new CodeGotoStatement(label)
        })));
      statementCollection.AddRange(defaultStms);
      statementCollection.Add((CodeStatement) new CodeLabeledStatement(label, (CodeStatement) new CodeSnippetStatement(";")));
      return statementCollection;
    }

    private static CodeStatementCollection MakeStms(CodeStatement stm) => new CodeStatementCollection(new CodeStatement[1]
    {
      stm
    });

    private static CodeStatement[] MakeArray(CodeStatementCollection stms)
    {
      CodeStatement[] array = new CodeStatement[stms.Count];
      stms.CopyTo(array, 0);
      return array;
    }

    private string MakeAdapterInstanceName(SerializableType type)
    {
      string name = ((SerializableMemberInfo) type).Name;
      StringBuilder stringBuilder = new StringBuilder("Instance");
      bool flag = false;
      int num = 0;
      for (int index = name.Length - 1; index >= 0 && !flag; --index)
      {
        switch (name[index])
        {
          case '+':
          case '.':
            if (num == 0)
            {
              flag = true;
              break;
            }
            break;
          case '<':
            --num;
            break;
          case '>':
            ++num;
            break;
          default:
            if (num == 0)
            {
              stringBuilder.Insert(0, name[index]);
              break;
            }
            break;
        }
      }
      return this.MakeUnique(stringBuilder.ToString());
    }

    private string MakeAdapterInstanceName(string name)
    {
      StringBuilder stringBuilder = new StringBuilder("Instance");
      bool flag = false;
      int num = 0;
      for (int index = name.Length - 1; index >= 0 && !flag; --index)
      {
        switch (name[index])
        {
          case '+':
          case '.':
            if (num == 0)
            {
              flag = true;
              break;
            }
            break;
          case '<':
            --num;
            break;
          case '>':
            ++num;
            break;
          default:
            if (num == 0)
            {
              stringBuilder.Insert(0, name[index]);
              break;
            }
            break;
        }
      }
      return this.MakeUnique(stringBuilder.ToString());
    }

    private static string MakeNameFromType(SerializableType type) => type.FullName.Replace('.', '_').Replace('+', '_').Replace('<', '_').Replace('>', '_');

    private void AddPreConstraints(CodeStatementCollection stms, Transition trans)
    {
      if (TestCodeGenerateBase.IsPreConstraintCheckAction(trans) && trans.PreConstraints != null)
      {
        foreach (Constraint preConstraint in trans.PreConstraints)
          this.AddConstraintCheckOrBinding(stms, preConstraint, "Fail to check preconstraint");
      }
      this.AddVariableUnbinding(stms, trans);
    }

    private void AddAssumptions(CodeStatementCollection stms, Transition trans)
    {
      if (trans.Action.Symbol.Kind != ActionSymbolKind.Event && trans.Action.Symbol.Kind != ActionSymbolKind.Return || trans.PostConstraints == null)
        return;
      foreach (Constraint postConstraint in trans.PostConstraints)
        this.AddConstraintCheckOrBinding(stms, postConstraint, "Fail to check the assumption");
    }

    private void AddConstraintCheckOrBinding(
      CodeStatementCollection stms,
      Constraint constraint,
      string failureContext)
    {
      if (constraint.Expression.NodeType == ExpressionType.Call)
      {
        SerializableMethodCallExpression expression1 = (SerializableMethodCallExpression) constraint.Expression;
        if (this.helperEqualityMethod == (MethodInfo) null)
        {
          this.helperEqualityMethod = typeof (TestManagerHelpers).GetMethod("Equality", new System.Type[2]
          {
            typeof (object),
            typeof (object)
          });
          if (this.helperEqualityMethod == (MethodInfo) null)
            this.host.FatalError("Unable to retrieve method TestManagerHelpers.Equality(System.Object, System.Object)");
        }
        if (expression1.Method.ToMethodInfo(new Func<string, System.Type>(ObjectModelHelpers.DefaultTypeResolver)) == this.helperEqualityMethod)
        {
          if (expression1.Arguments.Length != 2)
            this.host.FatalError("Equality call has to take two arguments");
          SerializableExpression expression2 = expression1.Arguments[0].NodeType == ExpressionType.Convert ? ((SerializableUnaryExpression) expression1.Arguments[0]).Operand : expression1.Arguments[0];
          SerializableExpression expression3 = expression1.Arguments[1].NodeType == ExpressionType.Convert ? ((SerializableUnaryExpression) expression1.Arguments[1]).Operand : expression1.Arguments[1];
          SerializableParameterExpression parameterExpression1 = expression2 as SerializableParameterExpression;
          SerializableParameterExpression parameterExpression2 = expression3 as SerializableParameterExpression;
          if (parameterExpression1 != null && parameterExpression2 != null)
          {
            stms.Add(this.MakeAssertBind(new CodeTypeReference(((SerializableExpression) parameterExpression1).Type.FullName), TestCodeGenerateBase.MakeThisReference(parameterExpression1.Name), TestCodeGenerateBase.MakeThisReference(parameterExpression2.Name), TestCodeGenerateBase.MakeValue((object) constraint.Text)));
            return;
          }
          if (parameterExpression1 != null)
          {
            stms.Add(this.MakeAssertBind(new CodeTypeReference(((SerializableExpression) parameterExpression1).Type.FullName), TestCodeGenerateBase.MakeThisReference(parameterExpression1.Name), this.ExpressionToCode(expression3), TestCodeGenerateBase.MakeValue((object) constraint.Text)));
            return;
          }
          if (parameterExpression2 != null)
          {
            stms.Add(this.MakeAssertBind(new CodeTypeReference(((SerializableExpression) parameterExpression2).Type.FullName), TestCodeGenerateBase.MakeThisReference(parameterExpression2.Name), this.ExpressionToCode(expression2), TestCodeGenerateBase.MakeValue((object) constraint.Text)));
            return;
          }
        }
      }
      CodeExpression codeExpression = TestCodeGenerateBase.MakeManagerInvoke("Assert", this.ExpressionToCode(constraint.Expression), TestCodeGenerateBase.MakeValue((object) string.Format("{0} : {1}", (object) failureContext, (object) constraint.Text)));
      stms.Add(codeExpression);
    }

    protected CodeStatementCollection MakeExpectPreConstraintStatement(
      List<CodeExpression> expects,
      List<CodeStatementCollection> expectContinuations,
      CodeStatementCollection defaultContinuation)
    {
      List<CodeExpression> codeExpressionList = new List<CodeExpression>();
      if (defaultContinuation.Count > 0)
        codeExpressionList.Add(TestCodeGenerateBase.MakeValue((object) true));
      else
        codeExpressionList.Add(TestCodeGenerateBase.MakeValue((object) false));
      codeExpressionList.AddRange((IEnumerable<CodeExpression>) expects);
      CodeStatementCollection statements = new CodeStatementCollection();
      CodeExpression codeExpression = TestCodeGenerateBase.MakeManagerInvoke("SelectSatisfiedPreConstraint", codeExpressionList.ToArray());
      if (defaultContinuation == null && expectContinuations.Count == 1)
      {
        statements.Add((CodeStatement) new CodeExpressionStatement(codeExpression));
        statements.AddRange(expectContinuations[0]);
      }
      else
        this.MakeLabelSwitch(expects, expectContinuations, defaultContinuation, statements, codeExpression);
      return statements;
    }

    private void MakeLabelSwitch(
      List<CodeExpression> expects,
      List<CodeStatementCollection> expectContinuations,
      CodeStatementCollection defaultContinuation,
      CodeStatementCollection statements,
      CodeExpression expectInvoke)
    {
      string name = this.NewTemporary();
      statements.Add((CodeStatement) new CodeVariableDeclarationStatement(new CodeTypeReference(typeof (int)), name, expectInvoke));
      List<CodeExpression> caseLabels = new List<CodeExpression>();
      for (int index = 0; index < expects.Count; ++index)
        caseLabels.Add(TestCodeGenerateBase.MakeValue((object) index));
      statements.AddRange(this.MakeSwitch((CodeExpression) new CodeSnippetExpression(name), caseLabels, expectContinuations, defaultContinuation));
    }

    protected CodeStatementCollection MakeExpectStatement(
      State state,
      TestCodeGenerateBase.ExpectKind expectKind,
      CodeExpression timeout,
      List<CodeExpression> expects,
      List<CodeStatementCollection> expectContinuations,
      CodeStatementCollection defaultContinuation)
    {
      List<CodeExpression> codeExpressionList = new List<CodeExpression>();
      codeExpressionList.Add(timeout);
      bool isAccepting = (state.Flags & ObjectModel.StateFlags.Accepting) != 0;
      if (defaultContinuation != null || isAccepting)
        codeExpressionList.Add(TestCodeGenerateBase.MakeValue((object) false));
      else
        codeExpressionList.Add(TestCodeGenerateBase.MakeValue((object) true));
      codeExpressionList.AddRange((IEnumerable<CodeExpression>) expects);
      CodeStatementCollection statements = new CodeStatementCollection();
      CodeExpression codeExpression = TestCodeGenerateBase.MakeManagerInvoke(expectKind == TestCodeGenerateBase.ExpectKind.Event ? "ExpectEvent" : "ExpectReturn", codeExpressionList.ToArray());
      if (defaultContinuation == null && expectContinuations.Count == 1)
      {
        if (expectKind == TestCodeGenerateBase.ExpectKind.Event)
        {
          List<CodeStatement> codeStatementList = new List<CodeStatement>();
          foreach (CodeStatement codeStatement in (CollectionBase) expectContinuations[0])
            codeStatementList.Add(codeStatement);
          statements.Add((CodeStatement) new CodeConditionStatement((CodeExpression) new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.IdentityInequality, TestCodeGenerateBase.MakeValue((object) -1)), codeStatementList.ToArray(), this.MakeObservationTimeoutCheck(isAccepting, expects.ToArray())));
        }
        else
        {
          statements.Add((CodeStatement) new CodeExpressionStatement(codeExpression));
          statements.AddRange(expectContinuations[0]);
        }
      }
      else
      {
        if (defaultContinuation == null && expectKind == TestCodeGenerateBase.ExpectKind.Event)
          defaultContinuation = new CodeStatementCollection(this.MakeObservationTimeoutCheck(isAccepting, expects.ToArray()));
        this.MakeLabelSwitch(expects, expectContinuations, defaultContinuation, statements, codeExpression);
      }
      return statements;
    }

    private CodeStatement[] MakeObservationTimeoutCheck(
      bool isAccepting,
      CodeExpression[] expected)
    {
      List<CodeExpression> codeExpressionList = new List<CodeExpression>();
      codeExpressionList.Add(TestCodeGenerateBase.MakeValue((object) isAccepting));
      codeExpressionList.AddRange((IEnumerable<CodeExpression>) expected);
      return new CodeStatement[1]
      {
        (CodeStatement) new CodeExpressionStatement(TestCodeGenerateBase.MakeManagerInvoke("CheckObservationTimeout", codeExpressionList.ToArray()))
      };
    }

    protected CodeExpression MakeExpect(
      Transition transition,
      out CodeMemberMethod checkerMethod)
    {
      bool flag = TestCodeGenerateBase.IsPreConstraintCheckAction(transition);
      checkerMethod = (CodeMemberMethod) null;
      CodeTypeDelegate checkerDelegate;
      CodeExpression codeExpression1;
      if (this.delegateInstanceFieldMap.TryGetValue(transition.Action.Symbol.Member, out checkerDelegate))
      {
        string methodName = !flag ? this.MakeCheckerMethod(transition, checkerDelegate, out checkerMethod) : this.MakePreConstraintCheckerMethod(transition, out checkerMethod);
        codeExpression1 = (CodeExpression) new CodeDelegateCreateExpression(new CodeTypeReference(checkerDelegate.Name), TestCodeGenerateBase.MakeThis(), methodName);
      }
      else
        codeExpression1 = TestCodeGenerateBase.MakeValue((object) null);
      ActionInvocation action = transition.Action;
      if (flag)
        return (CodeExpression) new CodeObjectCreateExpression(new CodeTypeReference("ExpectedPreConstraint"), new CodeExpression[1]
        {
          codeExpression1
        });
      CodeTypeReference createType = action.Symbol.Kind != ActionSymbolKind.Event ? new CodeTypeReference("ExpectedReturn") : new CodeTypeReference("ExpectedEvent");
      SerializableMemberInfo method = this.methodMap[action.Symbol.Member.Header];
      CodeExpression codeExpression2 = TestCodeGenerateBase.MakeValue((object) null);
      if (!method.IsStatic && !this.adapterTypes.Contains(method.DeclaringType))
      {
        if (action.Arguments.Length == 0)
        {
          this.host.FatalError("cannot get target of non-static non-adapter event or action " + method.Name);
          return (CodeExpression) new CodeSnippetExpression("Error");
        }
        codeExpression2 = this.ExpressionToCode(action.Arguments[0]);
      }
      return (CodeExpression) new CodeObjectCreateExpression(createType, new CodeExpression[3]
      {
        (CodeExpression) new CodeSnippetExpression(string.Format("{0}.{1}", (object) this.testLogicClassName, (object) this.GetMetadataField(action.Symbol.Member).Name)),
        codeExpression2,
        codeExpression1
      });
    }

    private string MakePreConstraintCheckerMethod(
      Transition trans,
      out CodeMemberMethod checkerMethod)
    {
      checkerMethod = new CodeMemberMethod();
      checkerMethod.Name = this.MakeUnique("PreConstraintChecker");
      this.AddPreConstraints(checkerMethod.Statements, trans);
      return checkerMethod.Name;
    }

    private static bool IsPreConstraintCheckAction(Transition trans) => trans.Action.Symbol.Kind.Equals(5);

    protected void GenerateCallActionStep(
      Transition transition,
      out List<CodeVariableDeclarationStatement> outputs,
      out SerializableMethodBase mbase,
      out CodeExpression receiver,
      out List<CodeExpression> codeArgs,
      out bool isCtor,
      out CodeStatementCollection statements)
    {
      outputs = new List<CodeVariableDeclarationStatement>();
      mbase = (SerializableMethodBase) this.methodMap[transition.Action.Symbol.Member.Header];
      SerializableMethodInfo serializableMethodInfo = mbase as SerializableMethodInfo;
      receiver = (CodeExpression) null;
      codeArgs = new List<CodeExpression>();
      SerializableExpression[] arguments = transition.Action.Arguments;
      SerializableParameterInfo[] parameters = mbase.Parameters;
      int num = 0;
      string name1 = (string) null;
      isCtor = serializableMethodInfo == null;
      if (isCtor)
      {
        name1 = this.NewTemporary();
        outputs.Add(new CodeVariableDeclarationStatement(TestCodeGenerateBase.MakeTypeReference(((SerializableMemberInfo) mbase).DeclaringType), name1));
        ++num;
      }
      if (this.adapterTypes.Contains(((SerializableMemberInfo) mbase).DeclaringType))
        receiver = TestCodeGenerateBase.MakeThisReference(this.adapterInstanceFieldMap[((SerializableMemberInfo) mbase).DeclaringType].Name);
      else if (!((SerializableMemberInfo) mbase).IsStatic && !isCtor)
      {
        if (num >= arguments.Length)
          throw new TestCodeGenerationException(string.Format("{0}: {1}", (object) "Placeholder is not supported in test code generation", (object) transition.Action.Text));
        receiver = this.ExpressionToCode(arguments[num++]);
      }
      for (int index = 0; index < parameters.Length; ++index)
      {
        SerializableParameterInfo serializableParameterInfo = parameters[index];
        if (serializableParameterInfo.Type.IsByRef)
        {
          string name2 = this.NewTemporary();
          CodeVariableDeclarationStatement declarationStatement;
          if (!serializableParameterInfo.IsOut)
          {
            if (num >= arguments.Length)
              throw new TestCodeGenerationException(string.Format("{0}: {1}", (object) "Placeholder is not supported in test code generation", (object) transition.Action.Text));
            declarationStatement = new CodeVariableDeclarationStatement(TestCodeGenerateBase.MakeTypeReference(serializableParameterInfo.Type.ElementType), name2, this.ExpressionToCode(arguments[num++]));
            codeArgs.Add((CodeExpression) new CodeSnippetExpression("ref " + name2));
          }
          else
          {
            declarationStatement = new CodeVariableDeclarationStatement(TestCodeGenerateBase.MakeTypeReference(serializableParameterInfo.Type.ElementType), name2);
            codeArgs.Add((CodeExpression) new CodeSnippetExpression("out " + name2));
          }
          outputs.Add(declarationStatement);
        }
        else
        {
          if (num >= arguments.Length)
            throw new TestCodeGenerationException(string.Format("{0}: {1}", (object) "Placeholder is not supported in test code generation", (object) transition.Action.Text));
          codeArgs.Add(this.ExpressionToCode(arguments[num++]));
        }
      }
      if (serializableMethodInfo != null && serializableMethodInfo.ReturnType != null)
      {
        name1 = this.NewTemporary();
        outputs.Add(new CodeVariableDeclarationStatement(TestCodeGenerateBase.MakeTypeReference(serializableMethodInfo.ReturnType), name1));
      }
      statements = new CodeStatementCollection();
      statements.AddRange((CodeStatement[]) outputs.ToArray());
      this.AddComment(statements, string.Format("executing step '{0}'", (object) this.ToString(transition.Action)));
      if (isCtor)
      {
        CodeExpression right = (CodeExpression) new CodeObjectCreateExpression(TestCodeGenerateBase.MakeTypeReference(((SerializableMemberInfo) mbase).DeclaringType), codeArgs.ToArray());
        if (name1 != null)
          statements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) new CodeSnippetExpression(name1), right));
        else
          statements.Add(right);
      }
      else if (serializableMethodInfo.AssociationReference != null)
      {
        if (serializableMethodInfo.AssociationReference.Kind == null)
        {
          if (codeArgs.Count != 0)
            throw new TestCodeGenerationException(string.Format("get method of property '{0}' must not take any argument.", (object) this.GetFullMemberName(serializableMethodInfo.AssociationReference.Association)));
          CodeExpression right = receiver == null ? (CodeExpression) new CodePropertyReferenceExpression(receiver, this.GetFullMemberName(serializableMethodInfo.AssociationReference.Association)) : (CodeExpression) new CodePropertyReferenceExpression(receiver, serializableMethodInfo.AssociationReference.Association.Name);
          if (name1 == null)
            throw new TestCodeGenerationException(string.Format("get method of property '{0}' must have non-void return type.", (object) this.GetFullMemberName(serializableMethodInfo.AssociationReference.Association)));
          statements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) new CodeSnippetExpression(name1), right));
        }
        else
        {
          if (codeArgs.Count != 1)
            throw new TestCodeGenerationException(string.Format("set method of property '{0}' must take exact one argument.", (object) this.GetFullMemberName(serializableMethodInfo.AssociationReference.Association)));
          if (receiver != null)
            statements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) new CodePropertyReferenceExpression(receiver, serializableMethodInfo.AssociationReference.Association.Name), codeArgs.First<CodeExpression>()));
          else
            statements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) new CodePropertyReferenceExpression(receiver, this.GetFullMemberName(serializableMethodInfo.AssociationReference.Association)), codeArgs.First<CodeExpression>()));
        }
      }
      else
      {
        CodeExpression right = (CodeExpression) new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(receiver, this.GetFullMemberName((SerializableMemberInfo) mbase)), codeArgs.ToArray());
        if (name1 != null)
          statements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) new CodeSnippetExpression(name1), right));
        else
          statements.Add(right);
      }
    }

    protected void GenerateReturnStatement(
      Transition transition,
      List<CodeExpression> codeArgs,
      bool isCtor,
      CodeStatementCollection statements,
      SerializableExpression[] actionArgs,
      Transition successor)
    {
      List<SerializableExpression> args = new List<SerializableExpression>((IEnumerable<SerializableExpression>) successor.Action.Arguments);
      if (isCtor)
      {
        if (actionArgs.Length <= 0)
          throw new TestCodeGenerationException(string.Format("{0}: {1}", (object) "Placeholder is not supported in test code generation", (object) transition.Action.Text));
        args.Insert(0, actionArgs[0]);
      }
      if (((IEnumerable<SerializableMemberInfo>) this.transitionSystem.ActionMembers).Contains<SerializableMemberInfo>(successor.Action.Symbol.Member))
        this.AddChecker(statements, successor, this.GenerateParameterDeclarationExpressions(successor.Action.Symbol.Member), codeArgs, args);
      else
        this.AddChecker(statements, successor, new CodeParameterDeclarationExpressionCollection(), codeArgs, args);
    }

    protected bool CalculateArguments(
      List<CodeVariableDeclarationStatement> outputs,
      SerializableMemberInfo member,
      CodeExpression receiver,
      List<CodeExpression> codeArgs,
      bool isCtor)
    {
      codeArgs.Clear();
      bool flag = !member.IsStatic && !isCtor && !this.adapterTypes.Contains(member.DeclaringType);
      if (flag)
        codeArgs.Add(receiver);
      foreach (CodeVariableDeclarationStatement output in outputs)
        codeArgs.Add((CodeExpression) new CodeVariableReferenceExpression(output.Name));
      return flag;
    }

    protected void GenerateAddReturn(
      Transition transition,
      SerializableMemberInfo memberInfo,
      List<CodeExpression> codeArgs,
      bool isCtor,
      CodeStatementCollection statements,
      bool hasReceiver)
    {
      if (isCtor)
        throw new TestCodeGenerationException(string.Format("constructor call followed by non-deterministic return not supported by code generation method: {0}", (object) ((object) transition.Action).ToString()));
      if (!hasReceiver)
        codeArgs.Insert(0, (CodeExpression) new CodeSnippetExpression("null"));
      codeArgs.Insert(0, (CodeExpression) new CodeFieldReferenceExpression((CodeExpression) null, this.GetMetadataField(memberInfo).Name));
      statements.Add(TestCodeGenerateBase.MakeManagerInvoke("AddReturn", codeArgs.ToArray()));
    }

    protected string GenerateTestMethodReturnValue(
      CodeMemberMethod testMethod,
      CodeStatementCollection currentStatements,
      bool IsDynamicTraversal)
    {
      string str = this.transitionSystem.GetSwitch("TestMethodReturnType").ExpandPrimitiveType();
      if (str.IsNoneOrEmptyValue() || string.Compare(str, "System.Void", false) == 0)
      {
        if (this.instanceEventRemovalStatements.Count > 0)
        {
          CodeTryCatchFinallyStatement finallyStatement = new CodeTryCatchFinallyStatement();
          if (!IsDynamicTraversal)
            finallyStatement.TryStatements.Add(TestCodeGenerateBase.MakeManagerInvoke("BeginTest", TestCodeGenerateBase.MakeValue((object) testMethod.Name)));
          finallyStatement.TryStatements.AddRange(currentStatements);
          finallyStatement.FinallyStatements.AddRange(this.instanceEventRemovalStatements);
          if (!IsDynamicTraversal)
            finallyStatement.FinallyStatements.Add(TestCodeGenerateBase.MakeManagerInvoke("EndTest"));
          testMethod.Statements.Add((CodeStatement) finallyStatement);
        }
        else
        {
          if (!IsDynamicTraversal)
            testMethod.Statements.Add(TestCodeGenerateBase.MakeManagerInvoke("BeginTest", TestCodeGenerateBase.MakeValue((object) testMethod.Name)));
          testMethod.Statements.AddRange(currentStatements);
          if (!IsDynamicTraversal)
            testMethod.Statements.Add(TestCodeGenerateBase.MakeManagerInvoke("EndTest"));
        }
      }
      else
      {
        testMethod.ReturnType = new CodeTypeReference(str.ExpandPrimitiveType());
        CodeTryCatchFinallyStatement finallyStatement = new CodeTryCatchFinallyStatement();
        if (!IsDynamicTraversal)
          finallyStatement.TryStatements.Add(TestCodeGenerateBase.MakeManagerInvoke("BeginTest", TestCodeGenerateBase.MakeValue((object) testMethod.Name)));
        finallyStatement.TryStatements.AddRange(currentStatements);
        string s1 = this.transitionSystem.GetSwitch("TestPassedReturnValue");
        if (s1.IsNoneOrEmptyValue())
          this.host.DiagMessage(DiagnosisKind.Error, "Return type of test method has been configured with switch 'TestMethodReturnType', please set switch 'TestPassedReturnValue' accordingly.", (object) null);
        else
          finallyStatement.TryStatements.Add((CodeStatement) new CodeMethodReturnStatement(TestCodeGenerateBase.CodeExpressionFromString(s1)));
        string typeName = this.transitionSystem.GetSwitch("TestFailedExceptionType");
        CodeCatchClause codeCatchClause = new CodeCatchClause(this.NewTemporary(), new CodeTypeReference(typeName));
        this.AddComment(codeCatchClause.Statements, (CodeExpression) new CodeMethodInvokeExpression(TestCodeGenerateBase.CodeExpressionFromString(codeCatchClause.LocalName), "ToString", new CodeExpression[0]));
        string s2 = this.transitionSystem.GetSwitch("TestFailedReturnValue");
        if (s2.IsNoneOrEmptyValue())
          this.host.DiagMessage(DiagnosisKind.Error, "Return type of test method has been configured with switch 'TestMethodReturnType', please set switch 'TestFailedReturnValue' accordingly.", (object) null);
        else
          codeCatchClause.Statements.Add((CodeStatement) new CodeMethodReturnStatement(TestCodeGenerateBase.CodeExpressionFromString(s2)));
        finallyStatement.CatchClauses.Add(codeCatchClause);
        finallyStatement.FinallyStatements.AddRange(this.instanceEventRemovalStatements);
        if (!IsDynamicTraversal)
          finallyStatement.FinallyStatements.Add(TestCodeGenerateBase.MakeManagerInvoke("EndTest"));
        testMethod.Statements.Add((CodeStatement) finallyStatement);
      }
      return str;
    }

    protected void GenerateStaticTestMethod(
      CodeAttributeDeclaration[] attributes,
      CodeMemberMethod testMethod,
      string testMethodReturnType)
    {
      if (string.Compare(this.transitionSystem.GetSwitch("GenerateStaticTestMethods"), "true", true) == 0)
        this.WrapTestMethod(testMethod, testMethodReturnType, attributes, MemberAttributes.Static | MemberAttributes.Public);
      else
        testMethod.CustomAttributes.AddRange(attributes);
    }

    private void WrapTestMethod(
      CodeMemberMethod testMethod,
      string testMethodReturnType,
      CodeAttributeDeclaration[] customAttributes,
      MemberAttributes memberAttributes)
    {
      CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
      codeMemberMethod.Name = testMethod.Name;
      codeMemberMethod.Attributes = memberAttributes;
      codeMemberMethod.CustomAttributes.AddRange(customAttributes);
      string str = this.NewTemporary();
      CodeVariableDeclarationStatement declarationStatement = new CodeVariableDeclarationStatement(this.testLogicClassName, str, (CodeExpression) new CodeObjectCreateExpression(this.testLogicClassName, new CodeExpression[0]));
      CodeVariableReferenceExpression referenceExpression = new CodeVariableReferenceExpression(str);
      CodeMethodInvokeExpression invokeExpression1 = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) referenceExpression, "TestInitialize"), new CodeExpression[0]);
      CodeMethodInvokeExpression invokeExpression2 = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) referenceExpression, testMethod.Name), new CodeExpression[0]);
      CodeMethodInvokeExpression invokeExpression3 = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression((CodeExpression) referenceExpression, "TestCleanup"), new CodeExpression[0]);
      codeMemberMethod.Statements.Add((CodeStatement) declarationStatement);
      CodeTryCatchFinallyStatement finallyStatement = new CodeTryCatchFinallyStatement();
      finallyStatement.TryStatements.Add((CodeExpression) invokeExpression1);
      if (testMethodReturnType.IsNoneOrEmptyValue() || string.Compare(testMethodReturnType, "System.Void", false) == 0)
      {
        finallyStatement.TryStatements.Add((CodeExpression) invokeExpression2);
      }
      else
      {
        codeMemberMethod.ReturnType = new CodeTypeReference(testMethodReturnType);
        finallyStatement.TryStatements.Add((CodeStatement) new CodeMethodReturnStatement((CodeExpression) invokeExpression2));
      }
      finallyStatement.FinallyStatements.Add((CodeExpression) invokeExpression3);
      codeMemberMethod.Statements.Add((CodeStatement) finallyStatement);
      this.testMethodWrapperCollection.Add((CodeTypeMember) codeMemberMethod);
    }

    protected void PreprocessInnerTestClass(
      out string generatedTestClassName,
      out string generateStaticTestMethods)
    {
      generatedTestClassName = this.variableResolver.Resolve("generatedtestclass");
      if (string.IsNullOrEmpty(generatedTestClassName))
        generatedTestClassName = this.transitionSystem.Name;
      generateStaticTestMethods = this.transitionSystem.GetSwitch("GenerateStaticTestMethods");
      if (string.Compare(generateStaticTestMethods, "true", true) == 0)
        this.testLogicClassName = generatedTestClassName + "Inner";
      else
        this.testLogicClassName = generatedTestClassName;
    }

    protected CodeTypeDeclaration GenerateInnerTestClass(
      string generatedTestClassName,
      string generateStaticTestMethods,
      CodeConstructor constructor,
      CodeTypeMemberCollection additionalMembers,
      CodeStatement testManagerSetStatement)
    {
      CodeTypeDeclaration testClass1 = new CodeTypeDeclaration(generatedTestClassName);
      testClass1.IsClass = true;
      testClass1.IsPartial = true;
      testClass1.Attributes = MemberAttributes.Public;
      testClass1.CustomAttributes.AddRange(this.testAttributeProvider.CreateTestClassAttributes().ToArray<CodeAttributeDeclaration>());
      if (string.Compare(generateStaticTestMethods, "true", true) == 0)
      {
        CodeTypeDeclaration testClass2 = new CodeTypeDeclaration(this.testLogicClassName);
        testClass2.IsClass = true;
        testClass2.IsPartial = true;
        testClass2.Attributes = MemberAttributes.Public;
        this.AddTestClassMembers(testClass2, constructor, additionalMembers, testManagerSetStatement);
        int num = 0;
        foreach (CodeMemberMethod testMethodWrapper in (CollectionBase) this.testMethodWrapperCollection)
        {
          if (num == 0)
            testMethodWrapper.StartDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.Start, "Test Methods"));
          if (num == this.testMethodWrapperCollection.Count - 1)
            testMethodWrapper.EndDirectives.Add((CodeDirective) new CodeRegionDirective(CodeRegionMode.End, ""));
          testClass1.Members.Add((CodeTypeMember) testMethodWrapper);
          ++num;
        }
        testClass1.Members.Add((CodeTypeMember) testClass2);
      }
      else
        this.AddTestClassMembers(testClass1, constructor, additionalMembers, testManagerSetStatement);
      return testClass1;
    }

    protected enum ExpectKind
    {
      Event,
      Return,
    }
  }
}
