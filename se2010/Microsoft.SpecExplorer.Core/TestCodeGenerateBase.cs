using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.CSharp;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.Runtime.Testing;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	public abstract class TestCodeGenerateBase : ITestCodeGenerator
	{
		protected enum ExpectKind
		{
			Event,
			Return
		}

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
			graph = new TransitionSystemGraphBuilder(transitionSystem).BuildGraph();
			adapterTypes = new HashSet<SerializableType>(transitionSystem.AdapterTypes);
			optionSetManager = new OptionSetManagerBuilder(transitionSystem).CreateOptionSetManager();
			variableResolver = new VariableResolver(transitionSystem);
		}

		public string Generate(string machineName)
		{
			MachineName = machineName;
			string result = string.Empty;
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			try
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
				result = GenerateTestCode(machineName);
				return result;
			}
			catch (TestCodeGenerationException ex)
			{
				host.DiagMessage(DiagnosisKind.Error, string.Format("[{0}]:Test Code Generation failed with errors (see below errors)", machineName), null);
				host.DiagMessage(DiagnosisKind.Error, ex.Message, null);
				return result;
			}
			finally
			{
				Thread.CurrentThread.CurrentCulture = currentCulture;
			}
		}

		public abstract string GenerateTestCode(string machineName);

		protected void AddTestClassMembers(CodeTypeDeclaration testClass, CodeConstructor constructor, CodeTypeMemberCollection additionalMembers, CodeStatement testManagerSetStatement)
		{
			testClass.Members.AddRange(additionalMembers);
			testClass.Members.Add(constructor);
			testClass.BaseTypes.Add(testClassBase);
			CodeTypeMemberCollection codeTypeMemberCollection = new CodeTypeMemberCollection();
			if (testClassBase == "PtfTestClassBase")
			{
				codeTypeMemberCollection.AddRange(GenerateClassInitializationAndCleanup());
			}
			codeTypeMemberCollection.AddRange(GenerateTestInitializationAndCleanup(testManagerSetStatement));
			testClass.Members.AddRange(GenerateExpectDelegateInstanceFieldDeclarations());
			testClass.Members.AddRange(GenerateMetadataDeclarationsAndInitialization());
			testClass.Members.AddRange(GenerateAdapterInstanceFieldDeclarations());
			testClass.Members.AddRange(GenerateVariableDeclarations());
			testClass.Members.AddRange(codeTypeMemberCollection);
			testClass.Members.AddRange(testMethodCodeCollection);
			if (getFieldMethod != null)
			{
				getFieldMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Helpers"));
				testClass.Members.Add(getFieldMethod);
				getFieldMethod.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
			}
			int num = 0;
			foreach (CodeMemberMethod value in eventHandlerMethods.Values)
			{
				if (num == 0)
				{
					value.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Event Handlers"));
				}
				if (num == eventHandlerMethods.Count - 1)
				{
					value.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
				}
				testClass.Members.Add(value);
				num++;
			}
		}

		protected CodeConstructor GenerateConstructor()
		{
			CodeConstructor codeConstructor = new CodeConstructor();
			codeConstructor.Attributes = MemberAttributes.Public;
			HashSet<string> hashSet = new HashSet<string>(from p in optionSetManager.GetProperties(Visibility.TestCodeGeneration)
				select p.Name.ToLower());
			ConfigSwitch[] configSwitches = transitionSystem.ConfigSwitches;
			foreach (ConfigSwitch configSwitch in configSwitches)
			{
				if (hashSet.Contains(configSwitch.Name.ToLower()))
				{
					codeConstructor.Statements.Add(new CodeExpressionStatement(MakeThisInvoke("SetSwitch", MakeValue(configSwitch.Name), MakeValue(configSwitch.Value))));
				}
			}
			return codeConstructor;
		}

		protected string InternalGenerateTestCode()
		{
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
			codeCompileUnit.AssemblyCustomAttributes.AddRange(testAttributeProvider.CreateTestAssemblyAttributes().ToArray());
			codeNamespace.Types.Add(GenerateTestClass());
			StringWriter stringWriter = new StringWriter();
			generator.GenerateCodeFromCompileUnit(codeCompileUnit, stringWriter, generatorOptions);
			return stringWriter.ToString();
		}

		public abstract CodeTypeDeclaration GenerateTestClass();

		protected void InitializeCodeGenerator()
		{
			CodeDomProvider codeDomProvider = new CSharpCodeProvider();
			generator = codeDomProvider.CreateGenerator("testcode");
			generatorOptions = new CodeGeneratorOptions();
			generatorOptions.VerbatimOrder = true;
			generatorOptions.BlankLinesBetweenMembers = true;
		}

		protected void RetrieveTestAttributes()
		{
			testClassBase = variableResolver.Resolve("testclassbase");
			testCaseName = transitionSystem.GetSwitch("testcasename");
			generateEventHandlers = transitionSystem.GetSwitch("generateeventhandlers");
			suppressGeneratedTestLogging = transitionSystem.GetSwitch("suppressgeneratedtestlogging");
			if (string.Compare(testClassBase, "vs", true) == 0)
			{
				testClassBase = "VsTestClassBase";
				testAttributeProvider = new VsTestAttributeProvider(transitionSystem);
			}
			else if (string.Compare(testClassBase, "ptf", true) == 0)
			{
				testClassBase = "PtfTestClassBase";
				testAttributeProvider = new PtfTestAttributeProvider(transitionSystem);
			}
			else
			{
				testAttributeProvider = new VsTestAttributeProvider(transitionSystem);
			}
		}

		protected IEnumerable<CodeNamespaceImport> GenerateImportNamespaces()
		{
			yield return new CodeNamespaceImport("System");
			yield return new CodeNamespaceImport("System.Collections.Generic");
			yield return new CodeNamespaceImport("System.Text");
			yield return new CodeNamespaceImport("System.Reflection");
			yield return new CodeNamespaceImport("Microsoft.SpecExplorer.Runtime.Testing");
			if (testClassBase == "PtfTestClassBase")
			{
				yield return new CodeNamespaceImport("Microsoft.Protocols.TestTools");
			}
			else if (testClassBase == "VsTestClassBase")
			{
				yield return new CodeNamespaceImport("Microsoft.VisualStudio.TestTools.UnitTesting");
			}
		}

		protected void InitializeInstanceFieldMaps()
		{
			foreach (Edge<State, Transition> edge in graph.Edges)
			{
				Transition label = edge.Label;
				SerializableMemberInfo member = label.Action.Symbol.Member;
				if (adapterTypes.Contains(member.DeclaringType) && !adapterInstanceFieldMap.ContainsKey(member.DeclaringType))
				{
					adapterInstanceFieldMap[member.DeclaringType] = new CodeMemberField(MakeTypeReference(member.DeclaringType), MakeAdapterInstanceName(member.DeclaringType.FullName));
				}
				switch (label.Action.Symbol.Kind)
				{
				case ActionSymbolKind.Return:
					if (transitionSystem.ActionMembers.Contains(member) && !delegateInstanceFieldMap.ContainsKey(member) && (graph.OutgoingCount(edge.Source) > 1 || graph.IncomingCount(edge.Source) > 1))
					{
						delegateInstanceFieldMap[member] = GenerateDelegateFieldInstance(member);
					}
					break;
				case ActionSymbolKind.Event:
				case ActionSymbolKind.PreConstraintCheck:
					if (transitionSystem.ActionMembers.Contains(member) && !delegateInstanceFieldMap.ContainsKey(member))
					{
						delegateInstanceFieldMap[member] = GenerateDelegateFieldInstance(member);
					}
					break;
				}
			}
		}

		protected static CodeExpression MakeValue(object value)
		{
			return new CodePrimitiveExpression(value);
		}

		protected static CodeExpression MakeThis()
		{
			return new CodeThisReferenceExpression();
		}

		protected static CodeExpression MakeThisInvoke(string name, params CodeExpression[] parameters)
		{
			return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(MakeThis(), name), parameters);
		}

		protected static CodeExpression MakeBase()
		{
			return new CodeBaseReferenceExpression();
		}

		protected static CodeExpression MakeThisReference(string name)
		{
			return new CodeFieldReferenceExpression(MakeThis(), name);
		}

		protected static CodeTypeReference MakeTypeReference(SerializableType type)
		{
			return new CodeTypeReference(type.FullName);
		}

		protected static CodeExpression MakeManagerInvoke(string name, params CodeExpression[] parameters)
		{
			return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(MakeThis(), "Manager"), name), parameters);
		}

		protected CodeMemberField GetMetadataField(SerializableMemberInfo member)
		{
			CodeMemberField value;
			if (!usedMetadataFields.TryGetValue(member.Header, out value))
			{
				value = GenerateMetadataDeclarationOrInitialization(member);
				if (value == null)
				{
					host.DiagMessage(DiagnosisKind.Error, "Cannot find the metadata", null);
				}
				usedMetadataFields[member.Header] = value;
			}
			return value;
		}

		protected static CodeExpression MakeBaseInvoke(string name, params CodeExpression[] parameters)
		{
			return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(MakeBase(), name), parameters);
		}

		protected string MakeUnique(string name)
		{
			int value;
			if (!testClassMemberNames.TryGetValue(name, out value))
			{
				value = 0;
			}
			string result = ((value > 0) ? (name + value) : name);
			testClassMemberNames[name] = value + 1;
			return result;
		}

		private string MakeMetadataFieldName(SerializableMemberInfo method)
		{
			return MakeUnique(string.Format("{0}Info", GetMethodName(method)));
		}

		protected string GetMethodName(SerializableMemberInfo method)
		{
			string text;
			if (method is SerializableConstructorInfo)
			{
				text = method.DeclaringType.FullName;
				int num = text.LastIndexOf(".");
				if (num >= 0)
				{
					text = text.Substring(num + 1);
				}
			}
			else
			{
				text = method.Name;
			}
			return text;
		}

		protected static CodeExpression MakeHelperInvoke(string name, params CodeExpression[] parameters)
		{
			return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("TestManagerHelpers"), name), parameters);
		}

		protected string MakeCheckerDelegateName(SerializableMemberInfo method)
		{
			return MakeUnique(string.Format("{0}Delegate", GetMethodName(method)));
		}

		protected string MakeCheckerMethodName(string testName, SerializableMemberInfo method)
		{
			return MakeUnique(string.Format("{0}{1}Checker", testName, GetMethodName(method)));
		}

		protected string CodeExpressionToString(CodeExpression expression)
		{
			using (StringWriter stringWriter = new StringWriter())
			{
				generator.GenerateCodeFromExpression(expression, stringWriter, generatorOptions);
				return stringWriter.ToString();
			}
		}

		private CodeTypeMemberCollection GenerateClassInitializationAndCleanup()
		{
			CodeTypeMemberCollection codeTypeMemberCollection = new CodeTypeMemberCollection();
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeTypeMemberCollection.Add(codeMemberMethod);
			codeMemberMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Class Initialization and Cleanup"));
			codeMemberMethod.Name = "ClassInitialize";
			codeMemberMethod.Attributes = (MemberAttributes)24579;
			codeMemberMethod.CustomAttributes.AddRange(testAttributeProvider.CreateTestClassInitializeAttributes().ToArray());
			codeMemberMethod.Parameters.Add(new CodeParameterDeclarationExpression("Microsoft.VisualStudio.TestTools.UnitTesting.TestContext", "context"));
			codeMemberMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(testClassBase), "Initialize"), new CodeVariableReferenceExpression("context")));
			CodeMemberMethod codeMemberMethod2 = new CodeMemberMethod();
			codeTypeMemberCollection.Add(codeMemberMethod2);
			codeMemberMethod2.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
			codeMemberMethod2.Name = "ClassCleanup";
			codeMemberMethod2.Attributes = (MemberAttributes)24579;
			codeMemberMethod2.CustomAttributes.AddRange(testAttributeProvider.CreateTestClassCleanupAttributes().ToArray());
			codeMemberMethod2.Statements.Add(new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(testClassBase), "Cleanup")));
			return codeTypeMemberCollection;
		}

		private CodeTypeMemberCollection GenerateTestInitializationAndCleanup(CodeStatement testManagerSetStatement)
		{
			CodeTypeMemberCollection codeTypeMemberCollection = new CodeTypeMemberCollection();
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeTypeMemberCollection.Add(codeMemberMethod);
			codeMemberMethod.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Test Initialization and Cleanup"));
			codeMemberMethod.Name = "TestInitialize";
			CodeAttributeDeclaration[] array = testAttributeProvider.CreateTestCaseInitializeAttributes().ToArray();
			if (array.Length > 0)
			{
				codeMemberMethod.Attributes = MemberAttributes.Public;
				codeMemberMethod.CustomAttributes.AddRange(array);
			}
			else if (testClassBase == "PtfTestClassBase")
			{
				codeMemberMethod.Attributes = (MemberAttributes)12292;
			}
			else
			{
				codeMemberMethod.Attributes = MemberAttributes.Public;
			}
			codeMemberMethod.Statements.Add(new CodeExpressionStatement(MakeThisInvoke("InitializeTestManager")));
			if (testManagerSetStatement != null)
			{
				codeMemberMethod.Statements.Add(testManagerSetStatement);
			}
			CodeStatementCollection eventAttachStatements;
			CodeStatementCollection eventRemovalStatements;
			GenerateEventAttachAndRemovalStatements(out eventAttachStatements, out eventRemovalStatements);
			foreach (CodeMemberField value in adapterInstanceFieldMap.Values)
			{
				codeMemberMethod.Statements.Add(new CodeAssignStatement(MakeThisReference(value.Name), new CodeCastExpression(value.Type, MakeManagerInvoke("GetAdapter", new CodeTypeOfExpression(value.Type)))));
			}
			codeMemberMethod.Statements.AddRange(eventAttachStatements);
			SerializableParameterExpression[] variables = transitionSystem.Variables;
			foreach (SerializableParameterExpression serializableParameterExpression in variables)
			{
				if (!transitionSystem.IsPlaceholder(serializableParameterExpression))
				{
					codeMemberMethod.Statements.Add(new CodeAssignStatement(MakeThisReference(serializableParameterExpression.Name), new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(MakeThis(), "Manager"), "CreateVariable", MakeTypeReference(serializableParameterExpression.ParameterType)), MakeValue(serializableParameterExpression.Name))));
				}
			}
			CodeMemberMethod codeMemberMethod2 = new CodeMemberMethod();
			codeTypeMemberCollection.Add(codeMemberMethod2);
			codeMemberMethod2.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
			codeMemberMethod2.Name = "TestCleanup";
			CodeAttributeDeclaration[] array2 = testAttributeProvider.CreateTestCaseCleanupAttributes().ToArray();
			if (array2.Length > 0)
			{
				codeMemberMethod2.Attributes = MemberAttributes.Public;
				codeMemberMethod2.CustomAttributes.AddRange(array2);
			}
			else if (testClassBase == "PtfTestClassBase")
			{
				codeMemberMethod2.Attributes = (MemberAttributes)12292;
				codeMemberMethod2.Statements.Add(new CodeExpressionStatement(MakeBaseInvoke("TestCleanup")));
			}
			else
			{
				codeMemberMethod2.Attributes = MemberAttributes.Public;
			}
			codeMemberMethod2.Statements.AddRange(eventRemovalStatements);
			codeMemberMethod2.Statements.Add(new CodeExpressionStatement(MakeThisInvoke("CleanupTestManager")));
			return codeTypeMemberCollection;
		}

		private void GenerateEventAttachAndRemovalStatements(out CodeStatementCollection eventAttachStatements, out CodeStatementCollection eventRemovalStatements)
		{
			eventAttachStatements = new CodeStatementCollection();
			eventRemovalStatements = new CodeStatementCollection();
			foreach (SerializableEventInfo item in transitionSystem.ActionMembers.OfType<SerializableEventInfo>())
			{
				if (!usedMetadataFields.ContainsKey(item.Header) || item.IsPreConstraintCheck)
				{
					continue;
				}
				bool isStatic = false;
				CodeExpression codeExpression = null;
				if (adapterTypes.Contains(item.DeclaringType))
				{
					codeExpression = MakeThisReference(adapterInstanceFieldMap[item.DeclaringType].Name);
				}
				else
				{
					if (!item.IsStatic)
					{
						continue;
					}
					isStatic = true;
					codeExpression = new CodeSnippetExpression("null");
				}
				if (string.Compare(generateEventHandlers, "true", true) == 0)
				{
					CodeStatement eventAttachStatement;
					CodeStatement eventRemovalStatement;
					GenerateEventAttachAndRemovalStatement(item, isStatic, codeExpression, out eventAttachStatement, out eventRemovalStatement);
					eventAttachStatements.Add(eventAttachStatement);
					eventRemovalStatements.Add(eventRemovalStatement);
				}
				else
				{
					List<CodeExpression> list = new List<CodeExpression>();
					list.Add(new CodeSnippetExpression(GetMetadataField(item).Name));
					list.Add(codeExpression);
					eventAttachStatements.Add(new CodeExpressionStatement(MakeManagerInvoke("Subscribe", list.ToArray())));
				}
			}
		}

		private void GenerateEventAttachAndRemovalStatement(SerializableEventInfo eventInfo, bool isStatic, CodeExpression target, out CodeStatement eventAttachStatement, out CodeStatement eventRemovalStatement)
		{
			CodeMemberMethod codeMemberMethod = GenerateEventHandlersMethod(eventInfo, target);
			CodeMethodReferenceExpression listener = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), codeMemberMethod.Name);
			if (isStatic)
			{
				eventAttachStatement = new CodeAttachEventStatement(new CodeTypeReferenceExpression(eventInfo.DeclaringType.FullName), eventInfo.Name, listener);
				eventRemovalStatement = new CodeRemoveEventStatement(new CodeTypeReferenceExpression(eventInfo.DeclaringType.FullName), eventInfo.Name, listener);
			}
			else
			{
				eventAttachStatement = new CodeAttachEventStatement(target, eventInfo.Name, listener);
				eventRemovalStatement = new CodeRemoveEventStatement(target, eventInfo.Name, listener);
			}
		}

		private CodeMemberMethod GenerateEventHandlersMethod(SerializableEventInfo eventInfo, CodeExpression target)
		{
			CodeMemberMethod value;
			if (!eventHandlerMethods.TryGetValue(eventInfo, out value))
			{
				value = new CodeMemberMethod();
				value.Name = MakeUnique(eventInfo.Name + "Handler");
				if (eventInfo.ReturnType != null)
				{
					value.ReturnType = new CodeTypeReference(eventInfo.ReturnType.FullName);
				}
				CodeSnippetExpression item = new CodeSnippetExpression(GetMetadataField(eventInfo).Name);
				SerializableParameterInfo[] parameters = eventInfo.Parameters;
				foreach (SerializableParameterInfo serializableParameterInfo in parameters)
				{
					value.Parameters.Add(new CodeParameterDeclarationExpression(MakeTypeReference(serializableParameterInfo.Type), serializableParameterInfo.Name));
				}
				List<CodeExpression> list = new List<CodeExpression>();
				list.Add(item);
				list.Add(target);
				List<CodeExpression> list2 = new List<CodeExpression>();
				foreach (CodeParameterDeclarationExpression parameter in value.Parameters)
				{
					list2.Add(new CodeArgumentReferenceExpression(parameter.Name));
				}
				list.Add(new CodeArrayCreateExpression(typeof(object), list2.ToArray()));
				value.Statements.Add(new CodeExpressionStatement(MakeManagerInvoke("AddEvent", list.ToArray())));
				eventHandlerMethods[eventInfo] = value;
			}
			return value;
		}

		private CodeTypeMemberCollection GenerateAdapterInstanceFieldDeclarations()
		{
			if (adapterInstanceFieldMap.Count > 0)
			{
				adapterInstanceFieldMap.Values.First().StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Adapter Instances"));
				adapterInstanceFieldMap.Values.Last().EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
			}
			return new CodeTypeMemberCollection(adapterInstanceFieldMap.Values.ToArray());
		}

		private CodeTypeMemberCollection GenerateExpectDelegateInstanceFieldDeclarations()
		{
			if (delegateInstanceFieldMap.Count > 0)
			{
				delegateInstanceFieldMap.Values.First().StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Expect Delegates"));
				delegateInstanceFieldMap.Values.Last().EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
			}
			return new CodeTypeMemberCollection(delegateInstanceFieldMap.Values.ToArray());
		}

		private CodeTypeMemberCollection GenerateVariableDeclarations()
		{
			CodeTypeMemberCollection codeTypeMemberCollection = new CodeTypeMemberCollection();
			CodeMemberField codeMemberField = null;
			CodeMemberField codeMemberField2 = null;
			SerializableParameterExpression[] variables = transitionSystem.Variables;
			foreach (SerializableParameterExpression serializableParameterExpression in variables)
			{
				if (!transitionSystem.IsPlaceholder(serializableParameterExpression))
				{
					CodeTypeReference type = new CodeTypeReference("IVariable", MakeTypeReference(serializableParameterExpression.ParameterType));
					CodeMemberField codeMemberField3 = new CodeMemberField(type, serializableParameterExpression.Name);
					codeTypeMemberCollection.Add(codeMemberField3);
					if (codeMemberField == null)
					{
						codeMemberField = codeMemberField3;
					}
					codeMemberField2 = codeMemberField3;
				}
			}
			if (codeMemberField != null)
			{
				codeMemberField.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Variables"));
				codeMemberField2.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
			}
			return codeTypeMemberCollection;
		}

		private CodeTypeMemberCollection GenerateMetadataDeclarationsAndInitialization()
		{
			CodeTypeMemberCollection codeTypeMemberCollection = new CodeTypeMemberCollection();
			CodeMemberField codeMemberField = null;
			CodeMemberField codeMemberField2 = null;
			foreach (CodeMemberField value in usedMetadataFields.Values)
			{
				codeTypeMemberCollection.Add(value);
				if (codeMemberField == null)
				{
					codeMemberField = value;
				}
				codeMemberField2 = value;
			}
			if (codeMemberField != null)
			{
				codeMemberField.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Event Metadata"));
				codeMemberField2.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
			}
			return codeTypeMemberCollection;
		}

		private CodeMemberField GenerateMetadataDeclarationOrInitialization(SerializableMemberInfo member)
		{
			CodeTypeReference type;
			if (member is SerializableEventInfo)
			{
				type = new CodeTypeReference(typeof(EventInfo));
			}
			else
			{
				if (!(member is SerializableMethodInfo) && !(member is SerializableConstructorInfo))
				{
					return null;
				}
				type = new CodeTypeReference(typeof(MethodBase));
			}
			CodeMemberField codeMemberField = new CodeMemberField(type, MakeMetadataFieldName(member));
			codeMemberField.Attributes = MemberAttributes.Static;
			if (member is SerializableEventInfo)
			{
				codeMemberField.InitExpression = MakeHelperInvoke("GetEventInfo", new CodeTypeOfExpression(MakeTypeReference(member.DeclaringType)), MakeValue(member.Name));
			}
			else
			{
				SerializableMethodBase serializableMethodBase = member as SerializableMethodBase;
				List<CodeExpression> list = new List<CodeExpression>();
				list.Add(new CodeTypeOfExpression(MakeTypeReference(member.DeclaringType)));
				if (member is SerializableMethodInfo)
				{
					list.Add(MakeValue(member.Name));
				}
				SerializableParameterInfo[] parameters = serializableMethodBase.Parameters;
				for (int i = 0; i < parameters.Length; i++)
				{
					SerializableType serializableType = parameters[i].Type;
					bool isByRef = serializableType.IsByRef;
					if (isByRef)
					{
						serializableType = serializableType.ElementType;
					}
					CodeExpression codeExpression = new CodeTypeOfExpression(MakeTypeReference(serializableType));
					if (isByRef)
					{
						codeExpression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(codeExpression, "MakeByRefType"));
					}
					list.Add(codeExpression);
				}
				codeMemberField.InitExpression = MakeHelperInvoke((member is SerializableConstructorInfo) ? "GetConstructorInfo" : "GetMethodInfo", list.ToArray());
			}
			return codeMemberField;
		}

		protected CodeTypeDelegate GenerateDelegateFieldInstance(SerializableMemberInfo member)
		{
			CodeTypeDelegate codeTypeDelegate = new CodeTypeDelegate(MakeUnique(MakeCheckerDelegateName(member)));
			codeTypeDelegate.Attributes |= MemberAttributes.Public;
			codeTypeDelegate.Parameters.AddRange(GenerateParameterDeclarationExpressions(member));
			return codeTypeDelegate;
		}

		private CodeParameterDeclarationExpressionCollection GenerateParameterDeclarationExpressions(SerializableMemberInfo member)
		{
			CodeParameterDeclarationExpressionCollection codeParameterDeclarationExpressionCollection = new CodeParameterDeclarationExpressionCollection();
			if (!member.IsStatic && !adapterTypes.Contains(member.DeclaringType))
			{
				codeParameterDeclarationExpressionCollection.Add(new CodeParameterDeclarationExpression(member.DeclaringType.FullName, "this"));
			}
			SerializableMethodBase serializableMethodBase = member as SerializableMethodBase;
			SerializableMethodInfo serializableMethodInfo = member as SerializableMethodInfo;
			SerializableParameterInfo[] array = ((serializableMethodBase != null) ? serializableMethodBase.Parameters : ((SerializableEventInfo)member).Parameters);
			foreach (SerializableParameterInfo serializableParameterInfo in array)
			{
				if (serializableMethodBase == null)
				{
					codeParameterDeclarationExpressionCollection.Add(new CodeParameterDeclarationExpression(MakeTypeReference(serializableParameterInfo.Type), serializableParameterInfo.Name));
				}
				else if (serializableParameterInfo.Type.IsByRef)
				{
					codeParameterDeclarationExpressionCollection.Add(new CodeParameterDeclarationExpression(MakeTypeReference(serializableParameterInfo.Type.ElementType), serializableParameterInfo.Name));
				}
			}
			if (serializableMethodInfo != null && serializableMethodInfo.ReturnType != null)
			{
				codeParameterDeclarationExpressionCollection.Add(new CodeParameterDeclarationExpression(serializableMethodInfo.ReturnType.FullName, "return"));
			}
			return codeParameterDeclarationExpressionCollection;
		}

		private string MakeCheckerMethod(Transition transition, CodeTypeDelegate checkerDelegate, out CodeMemberMethod checkerMethod)
		{
			ActionSymbol symbol = transition.Action.Symbol;
			SerializableMemberInfo method = methodMap[symbol.Member.Header];
			checkerMethod = new CodeMemberMethod();
			checkerMethod.Name = MakeCheckerMethodName(currentTestMethodName, method);
			CodeParameterDeclarationExpressionCollection parameters = checkerDelegate.Parameters;
			checkerMethod.Parameters.AddRange(parameters);
			AddChecker(checkerMethod.Statements, transition, parameters, null);
			return checkerMethod.Name;
		}

		private void AddChecker(CodeStatementCollection stms, Transition transition, CodeParameterDeclarationExpressionCollection parameters, List<CodeExpression> actuals)
		{
			AddChecker(stms, transition, parameters, actuals, new List<SerializableExpression>(transition.Action.Arguments));
		}

		private void AddChecker(CodeStatementCollection stms, Transition transition, CodeParameterDeclarationExpressionCollection parameters, List<CodeExpression> actuals, List<SerializableExpression> args)
		{
			AddComment(stms, string.Format("checking step '{0}'", ToString(transition.Action)));
			ActionSymbol symbol = transition.Action.Symbol;
			SerializableMemberInfo serializableMemberInfo = methodMap[symbol.Member.Header];
			SerializableMethodInfo serializableMethodInfo = serializableMemberInfo as SerializableMethodInfo;
			bool flag = serializableMemberInfo is SerializableConstructorInfo;
			Dictionary<CodeParameterDeclarationExpression, SerializableExpression> dictionary = new Dictionary<CodeParameterDeclarationExpression, SerializableExpression>();
			int num = 0;
			int num2 = 0;
			if (symbol.Kind == ActionSymbolKind.Event || symbol.Kind == ActionSymbolKind.Return)
			{
				if (!serializableMemberInfo.IsStatic && !adapterTypes.Contains(serializableMemberInfo.DeclaringType))
				{
					if (flag)
					{
						dictionary[parameters[num]] = args[num2];
					}
					num++;
					num2++;
				}
				else if (!serializableMemberInfo.IsStatic)
				{
					num2++;
				}
				bool flag2 = serializableMethodInfo != null && serializableMethodInfo.ReturnType != null;
				int num3 = (flag2 ? (parameters.Count - 1) : parameters.Count);
				while (num < num3)
				{
					dictionary[parameters[num]] = args[num2];
					num++;
					num2++;
				}
				if (flag2)
				{
					dictionary[parameters[num]] = args[num2];
					num++;
					num2++;
				}
				num = 0;
				CodeStatementCollection codeStatementCollection = new CodeStatementCollection();
				foreach (CodeParameterDeclarationExpression parameter in parameters)
				{
					CodeExpression actual = ((actuals != null) ? actuals[num] : new CodeVariableReferenceExpression(parameter.Name));
					SerializableExpression value;
					if (dictionary.TryGetValue(parameter, out value) && value != null)
					{
						string arg = ((!(serializableMemberInfo is SerializableConstructorInfo)) ? serializableMemberInfo.Name : serializableMemberInfo.DeclaringType.FullName);
						AddAssertEquality(codeStatementCollection, parameter.Type, actual, value, string.Format("{0} of {1}, state {2}", parameter.Name, arg, transition.Source));
					}
					num++;
				}
				if (codeStatementCollection.Count > 0)
				{
					CodeStatementCollection codeStatementCollection2 = new CodeStatementCollection();
					if (transition.CapturedRequirements != null && transition.CapturedRequirements.Length > 0)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append("This step would have covered ");
						bool flag3 = true;
						string[] capturedRequirements = transition.CapturedRequirements;
						foreach (string text in capturedRequirements)
						{
							if (flag3)
							{
								flag3 = false;
								stringBuilder.Append(text);
							}
							else
							{
								stringBuilder.AppendFormat(", {0}", text);
							}
						}
						AddComment(codeStatementCollection2, stringBuilder.ToString());
					}
					if (codeStatementCollection2.Count > 0)
					{
						codeStatementCollection2.Add(new CodeThrowExceptionStatement());
						CodeTryCatchFinallyStatement value2 = new CodeTryCatchFinallyStatement(MakeArray(codeStatementCollection), new CodeCatchClause[1]
						{
							new CodeCatchClause(null, new CodeTypeReference("TransactionFailedException"), MakeArray(codeStatementCollection2))
						});
						stms.Add(value2);
					}
					else
					{
						stms.AddRange(codeStatementCollection);
					}
				}
				AddAssumptions(stms, transition);
				AddCheckpoints(stms, transition);
				AddVariableUnbinding(stms, transition);
				return;
			}
			throw new TestCodeGenerationException(string.Format("unexpected action '{0}'", transition.Action.ToString()));
		}

		private void AddAssertEquality(CodeStatementCollection statements, CodeTypeReference type, CodeExpression actual, SerializableExpression expected, string context)
		{
			AddAssertEquality(statements, type, actual, expected, context, "");
		}

		private void AddAssertEquality(CodeStatementCollection statements, CodeTypeReference type, CodeExpression actual, SerializableExpression expected, string context, string selector)
		{
			if (expected is SerializableParameterExpression && transitionSystem.IsPlaceholder((SerializableParameterExpression)expected))
			{
				return;
			}
			if (expected.NodeType == Microsoft.SpecExplorer.ObjectModel.ExpressionType.MemberInit)
			{
				SerializableMemberInitExpression serializableMemberInitExpression = (SerializableMemberInitExpression)expected;
				AddAssertEquality(statements, actual, serializableMemberInitExpression.Bindings, context, selector);
				return;
			}
			string contextAndSelector = MakeContextAndSelector(context, selector);
			if (expected is SerializableBinaryExpression)
			{
				SerializableBinaryExpression expected2 = expected as SerializableBinaryExpression;
				AddBindingOrAssertionStatementForBinaryExpression(statements, type, actual, expected2, contextAndSelector);
			}
			else if (expected is SerializableUnaryExpression)
			{
				SerializableUnaryExpression serializableUnaryExpression = expected as SerializableUnaryExpression;
				if (serializableUnaryExpression.NodeType == Microsoft.SpecExplorer.ObjectModel.ExpressionType.Convert && serializableUnaryExpression.Operand is SerializableParameterExpression)
				{
					SerializableParameterExpression serializableParameterExpression = (SerializableParameterExpression)serializableUnaryExpression.Operand;
					CodeSnippetExpression actual2 = new CodeSnippetExpression(string.Format("({0}){1}", serializableParameterExpression.ParameterType.FullName, CodeExpressionToString(actual)));
					AddBindingOrAssertionStatement(statements, MakeTypeReference(serializableParameterExpression.ParameterType), actual2, serializableParameterExpression, contextAndSelector);
				}
				else
				{
					AddBindingOrAssertionStatement(statements, type, actual, expected, contextAndSelector);
				}
			}
			else
			{
				AddBindingOrAssertionStatement(statements, type, actual, expected, contextAndSelector);
			}
		}

		private void AddBindingOrAssertionStatement(CodeStatementCollection statements, CodeTypeReference type, CodeExpression actual, SerializableExpression expected, string contextAndSelector)
		{
			if (expected is SerializableParameterExpression)
			{
				SerializableParameterExpression serializableParameterExpression = (SerializableParameterExpression)expected;
				statements.Add(MakeAssertBind(type, MakeThisReference(serializableParameterExpression.Name), actual, MakeValue(contextAndSelector)));
				InstallEventHandlerToInstance(statements, ExpressionToCode(expected), serializableParameterExpression.ParameterType, true);
			}
			else
			{
				statements.Add(MakeAssertAreEqual(type, ExpressionToCode(expected), actual, MakeValue(contextAndSelector)));
			}
		}

		private void AddAssertEquality(CodeStatementCollection statements, CodeExpression actual, IEnumerable<SerializableMemberBinding> bindings, string context, string selector)
		{
			bool flag = false;
			foreach (SerializableMemberBinding binding in bindings)
			{
				if (binding.BindingType != 0)
				{
					throw new TestCodeGenerationException(string.Format("{0}: {1}", "Cannot process member bindings of this kind", binding));
				}
				SerializableMemberAssignment serializableMemberAssignment = (SerializableMemberAssignment)binding;
				SerializableFieldInfo serializableFieldInfo = (SerializableFieldInfo)serializableMemberAssignment.Member;
				if (serializableFieldInfo.DeclaringType.TypeCode == TypeCode.Object && !flag)
				{
					flag = true;
					statements.Add(MakeAssertNotNull(actual, MakeValue(MakeContextAndSelector(context, selector))));
				}
				CodeExpression codeExpression = MakeFieldSelection(actual, serializableFieldInfo);
				if (serializableMemberAssignment.Expression.NodeType == Microsoft.SpecExplorer.ObjectModel.ExpressionType.MemberInit)
				{
					string text = NewTemporary();
					statements.Add(new CodeVariableDeclarationStatement(MakeTypeReference(serializableFieldInfo.Type), text, codeExpression));
					codeExpression = new CodeVariableReferenceExpression(text);
				}
				AddAssertEquality(statements, MakeTypeReference(serializableFieldInfo.Type), codeExpression, serializableMemberAssignment.Expression, context, string.IsNullOrEmpty(selector) ? serializableFieldInfo.Name : (selector + "." + serializableFieldInfo.Name));
			}
		}

		private void AddBindingOrAssertionStatementForBinaryExpression(CodeStatementCollection statements, CodeTypeReference type, CodeExpression actual, SerializableBinaryExpression expected, string contextAndSelector)
		{
			if (expected.Type.IsBoolean() && expected.NodeType == Microsoft.SpecExplorer.ObjectModel.ExpressionType.NotEqual)
			{
				if (expected.Left is SerializableParameterExpression && expected.Right is SerializableConstantExpression && ((SerializableConstantExpression)expected.Right).Value == "0")
				{
					SerializableParameterExpression serializableParameterExpression = (SerializableParameterExpression)expected.Left;
					actual = new CodeSnippetExpression(string.Format("System.Convert.ToInt32({0})", CodeExpressionToString(actual)));
					statements.Add(MakeAssertBind(new CodeTypeReference("System.Int32"), MakeThisReference(serializableParameterExpression.Name), actual, MakeValue(contextAndSelector)));
				}
				else if (expected.Right is SerializableParameterExpression && expected.Left is SerializableConstantExpression && ((SerializableConstantExpression)expected.Left).Value == "0")
				{
					SerializableParameterExpression serializableParameterExpression2 = (SerializableParameterExpression)expected.Right;
					actual = new CodeSnippetExpression(string.Format("System.Convert.ToInt32({0})", CodeExpressionToString(actual)));
					statements.Add(MakeAssertBind(new CodeTypeReference("System.Int32"), MakeThisReference(serializableParameterExpression2.Name), actual, MakeValue(contextAndSelector)));
				}
				else
				{
					AddBindingOrAssertionStatement(statements, type, actual, expected, contextAndSelector);
				}
			}
			else
			{
				AddBindingOrAssertionStatement(statements, type, actual, expected, contextAndSelector);
			}
		}

		private static string MakeContextAndSelector(string context, string selector)
		{
			if (!string.IsNullOrEmpty(selector))
			{
				return string.Format("{0}, field selection {1}", context, selector);
			}
			return context;
		}

		private void InstallEventHandlerToInstance(CodeStatementCollection statements, CodeExpression target, SerializableType targetType, bool checkNull)
		{
			foreach (SerializableEventInfo item in from m in transitionSystem.ActionMembers.OfType<SerializableEventInfo>()
				where !m.IsStatic && IsEventFeasibleToType(m, targetType)
				select m)
			{
				if (string.Compare(generateEventHandlers, "true", true) == 0)
				{
					CodeStatement eventAttachStatement;
					CodeStatement eventRemovalStatement;
					GenerateEventAttachAndRemovalStatement(item, false, target, out eventAttachStatement, out eventRemovalStatement);
					if (!checkNull)
					{
						statements.Add(eventAttachStatement);
						instanceEventRemovalStatements.Add(eventRemovalStatement);
						continue;
					}
					CodeStatement value = new CodeConditionStatement(new CodeBinaryOperatorExpression(target, CodeBinaryOperatorType.IdentityInequality, new CodeSnippetExpression("null")), eventAttachStatement);
					statements.Add(value);
					CodeStatement value2 = new CodeConditionStatement(new CodeBinaryOperatorExpression(target, CodeBinaryOperatorType.IdentityInequality, new CodeSnippetExpression("null")), eventRemovalStatement);
					instanceEventRemovalStatements.Add(value2);
				}
				else
				{
					List<CodeExpression> list = new List<CodeExpression>();
					list.Add(new CodeSnippetExpression(GetMetadataField(item).Name));
					list.Add(target);
					CodeStatement codeStatement = new CodeExpressionStatement(MakeManagerInvoke("Subscribe", list.ToArray()));
					if (!checkNull)
					{
						statements.Add(codeStatement);
						continue;
					}
					CodeStatement value3 = new CodeConditionStatement(new CodeBinaryOperatorExpression(target, CodeBinaryOperatorType.IdentityInequality, new CodeSnippetExpression("null")), codeStatement);
					statements.Add(value3);
				}
			}
		}

		private static bool IsEventFeasibleToType(SerializableMemberInfo @event, SerializableType type)
		{
			if (!(@event.DeclaringType.FullName == type.FullName))
			{
				if (type.BaseType != null)
				{
					return IsEventFeasibleToType(@event, type.BaseType);
				}
				return false;
			}
			return true;
		}

		protected void AddVariableUnbinding(CodeStatementCollection statements, Transition trans)
		{
			if (trans == null || trans.VariablesToUnbind == null)
			{
				return;
			}
			SerializableParameterExpression[] variablesToUnbind = trans.VariablesToUnbind;
			foreach (SerializableParameterExpression serializableParameterExpression in variablesToUnbind)
			{
				if (serializableParameterExpression != null)
				{
					AddComment(statements, string.Format("Unbinding variable '{0}'", serializableParameterExpression.Name));
					statements.Add(new CodeMethodInvokeExpression(MakeThisReference(serializableParameterExpression.Name), "Unbind"));
				}
			}
		}

		private CodeExpression ExpressionToCode(SerializableExpression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			switch (expression.NodeType)
			{
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.ArrayLength:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Convert:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.ConvertChecked:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Negate:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.NegateChecked:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Not:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Quote:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.TypeAs:
				return ExpressionToCode((SerializableUnaryExpression)expression);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Add:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.AddChecked:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.And:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.AndAlso:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.ArrayIndex:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Coalesce:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Divide:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Equal:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.ExclusiveOr:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.GreaterThan:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.GreaterThanOrEqual:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.LeftShift:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.LessThan:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.LessThanOrEqual:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Modulo:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Multiply:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.MultiplyChecked:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.NotEqual:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Or:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.OrElse:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.RightShift:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Subtract:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.SubtractChecked:
				return ExpressionToCode((SerializableBinaryExpression)expression);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.TypeIs:
				return ExpressionToCode((SerializableTypeBinaryExpression)expression);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Conditional:
				return ExpressionToCode((SerializableConditionalExpression)expression);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Constant:
			{
				SerializableConstantExpression serializableConstantExpression = expression as SerializableConstantExpression;
				if (serializableConstantExpression != null)
				{
					return ExpressionToCode((SerializableConstantExpression)expression);
				}
				return ExpressionToCode((SerializableEnumExpression)expression);
			}
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Parameter:
				return ExpressionToCode((SerializableParameterExpression)expression);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.MemberAccess:
				return ExpressionToCode((SerializableMemberExpression)expression);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Call:
				return ExpressionToCode((SerializableMethodCallExpression)expression);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.New:
				return ExpressionToCode((SerializableNewExpression)expression);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.NewArrayInit:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.NewArrayBounds:
				return ExpressionToCode((SerializableNewArrayExpression)expression);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.MemberInit:
				return ExpressionToCode((SerializableMemberInitExpression)expression);
			default:
				throw new TestCodeGenerationException(string.Format("{0}: {1}", "Unsupported expression", expression));
			}
		}

		private CodeExpression ExpressionToCode(SerializableBinaryExpression b)
		{
			CodeExpression codeExpression = ExpressionToCode(b.Left);
			CodeExpression codeExpression2 = ExpressionToCode(b.Right);
			switch (b.NodeType)
			{
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Add:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.AddChecked:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.Add, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Subtract:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.SubtractChecked:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.Subtract, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Multiply:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.MultiplyChecked:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.Multiply, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Divide:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.Divide, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Modulo:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.Modulus, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.And:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.BitwiseAnd, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.AndAlso:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.BooleanAnd, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Or:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.BitwiseOr, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.OrElse:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.BooleanOr, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.LessThan:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.LessThan, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.LessThanOrEqual:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.LessThanOrEqual, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.GreaterThan:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.GreaterThan, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.GreaterThanOrEqual:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.GreaterThanOrEqual, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Equal:
				return new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.ValueEquality, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.NotEqual:
				return new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.ValueEquality, codeExpression2), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(false));
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.ArrayIndex:
				return new CodeArrayIndexerExpression(codeExpression, codeExpression2);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.RightShift:
				return CodeExpressionFromString(string.Format("({0}) >> ({1})", CodeExpressionToString(codeExpression), CodeExpressionToString(codeExpression2)));
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.LeftShift:
				return CodeExpressionFromString(string.Format("({0}) << ({1})", CodeExpressionToString(codeExpression), CodeExpressionToString(codeExpression2)));
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.ExclusiveOr:
				return CodeExpressionFromString(string.Format("({0}) ^ ({1})", CodeExpressionToString(codeExpression), CodeExpressionToString(codeExpression2)));
			default:
				throw new TestCodeGenerationException(string.Format("{0}: {1}", "Unsupported binary expression", b));
			}
		}

		private CodeExpression ExpressionToCode(SerializableUnaryExpression u)
		{
			CodeExpression codeExpression = ExpressionToCode(u.Operand);
			switch (u.NodeType)
			{
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Negate:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.NegateChecked:
				return new CodeBinaryOperatorExpression(new CodeDefaultValueExpression(MakeTypeReference(u.Type)), CodeBinaryOperatorType.Subtract, codeExpression);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Not:
				if (u.Operand.Type.FullName == "System.Int32")
				{
					return CodeExpressionFromString(string.Format("~({0})", CodeExpressionToString(codeExpression)));
				}
				return CodeExpressionFromString(string.Format("!({0})", CodeExpressionToString(codeExpression)));
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.Convert:
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.ConvertChecked:
				return new CodeCastExpression(MakeTypeReference(u.Type), codeExpression);
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.ArrayLength:
				return CodeExpressionFromString(string.Format("({0}).Length", CodeExpressionToString(codeExpression)));
			case Microsoft.SpecExplorer.ObjectModel.ExpressionType.TypeAs:
				return new CodeCastExpression(MakeTypeReference(u.Type), codeExpression);
			default:
				throw new TestCodeGenerationException(string.Format("{0}: {1}", "Unsupported unary expression", u));
			}
		}

		private CodeExpression ExpressionToCode(SerializableConstantExpression c)
		{
			Type type = Type.GetType(c.ValueType.FullName);
			if (type == null)
			{
				throw new TestCodeGenerationException(string.Format("{0}: {1}", "Cannot discover type of constant expression", c));
			}
			ConstantExpression constantExpression = (ConstantExpression)c.ToExpression((string t) => Type.GetType(t));
			return new CodePrimitiveExpression(constantExpression.Value);
		}

		private CodeExpression ExpressionToCode(SerializableEnumExpression e)
		{
			return CodeExpressionFromString(e.Value.Replace("+", "."));
		}

		private CodeExpression ExpressionToCode(SerializableTypeBinaryExpression b)
		{
			return CodeExpressionFromString(string.Format("({0}) is {1}", CodeExpressionToString(ExpressionToCode(b.Expression)), TypeToCode(b.TypeOperand)));
		}

		private string TypeToCode(SerializableType serializableType)
		{
			StringWriter stringWriter = new StringWriter();
			CodeTypeReferenceExpression e = new CodeTypeReferenceExpression(serializableType.FullName);
			generator.GenerateCodeFromExpression(e, stringWriter, generatorOptions);
			return stringWriter.ToString();
		}

		private CodeExpression ExpressionToCode(SerializableConditionalExpression c)
		{
			return CodeExpressionFromString(string.Format("({0}) ? ({1}) : ({2})", CodeExpressionToString(ExpressionToCode(c.Test)), CodeExpressionToString(ExpressionToCode(c.IfTrue)), CodeExpressionToString(ExpressionToCode(c.IfFalse))));
		}

		private CodeExpression ExpressionToCode(SerializableParameterExpression p)
		{
			if (transitionSystem.IsPlaceholder(p))
			{
				throw new TestCodeGenerationException(string.Format("{0}: {1}", "Placeholder is not supported in test code generation", p));
			}
			return new CodePropertyReferenceExpression(MakeThisReference(p.Name), "Value");
		}

		private CodeExpression ExpressionToCode(SerializableMemberExpression m)
		{
			CodeExpression codeExpression = ExpressionToCode(m.Expression);
			if (m.Member is SerializableFieldInfo)
			{
				return MakeFieldSelection(codeExpression, (SerializableFieldInfo)m.Member);
			}
			return new CodePropertyReferenceExpression(codeExpression, m.Member.Name);
		}

		private CodeExpression ExpressionToCode(SerializableMethodCallExpression c)
		{
			CodeExpression codeExpression = ((c.Object != null) ? ExpressionToCode(c.Object) : null);
			string fullMemberName = GetFullMemberName(c.Method);
			if (codeExpression == null && fullMemberName == "Microsoft.Xrt.Runtime.RuntimeSupport.Create" && c.Arguments != null && c.Arguments.Length == 2 && c.Arguments[0].NodeType == Microsoft.SpecExplorer.ObjectModel.ExpressionType.NewArrayInit && c.Arguments[1].NodeType == Microsoft.SpecExplorer.ObjectModel.ExpressionType.NewArrayInit)
			{
				codeExpression = new CodeThisReferenceExpression();
				SerializableExpression[] expressions = ((SerializableNewArrayExpression)c.Arguments[0]).Expressions;
				SerializableExpression[] expressions2 = ((SerializableNewArrayExpression)c.Arguments[1]).Expressions;
				return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(codeExpression, "Make", c.Method.TypeParameters.Select((SerializableType t) => MakeTypeReference(t)).ToArray()), new CodeArrayCreateExpression(new CodeTypeReference(typeof(string)), expressions.Select((SerializableExpression e) => ExpressionToCode(RemoveObjectCast(e))).ToArray()), new CodeArrayCreateExpression(new CodeTypeReference(typeof(object)), expressions2.Select((SerializableExpression e) => ExpressionToCode(RemoveObjectCast(e))).ToArray()));
			}
			CodeExpression[] parameters = ((c.Arguments != null) ? c.Arguments.Select((SerializableExpression e) => ExpressionToCode(e)).ToArray() : new CodeExpression[0]);
			CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(codeExpression, fullMemberName, c.Method.TypeParameters.Select((SerializableType t) => MakeTypeReference(t)).ToArray());
			return new CodeMethodInvokeExpression(method, parameters);
		}

		private SerializableExpression RemoveObjectCast(SerializableExpression e)
		{
			if (e.NodeType == Microsoft.SpecExplorer.ObjectModel.ExpressionType.Convert)
			{
				SerializableUnaryExpression serializableUnaryExpression = (SerializableUnaryExpression)e;
				if (serializableUnaryExpression.Type != null && serializableUnaryExpression.Type.FullName == "System.Object")
				{
					return serializableUnaryExpression.Operand;
				}
			}
			return e;
		}

		private CodeExpression ExpressionToCode(SerializableNewExpression n)
		{
			return new CodeObjectCreateExpression(MakeTypeReference(n.Constructor.DeclaringType), (n.Arguments != null) ? n.Arguments.Select((SerializableExpression e) => ExpressionToCode(e)).ToArray() : new CodeExpression[0]);
		}

		private CodeExpression ExpressionToCode(SerializableNewArrayExpression n)
		{
			if (n.NodeType == Microsoft.SpecExplorer.ObjectModel.ExpressionType.NewArrayBounds)
			{
				if (n.Expressions.Length > 1 || n.Expressions.Length == 0)
				{
					throw new TestCodeGenerationException(string.Format("{0}: {1}", "Unsupported multi-dimensional array creation", n));
				}
				return new CodeArrayCreateExpression(MakeTypeReference(n.ArrayType), ExpressionToCode(n.Expressions[0]));
			}
			return new CodeArrayCreateExpression(MakeTypeReference(n.ArrayType), n.Expressions.Select((SerializableExpression e) => ExpressionToCode(e)).ToArray());
		}

		private CodeExpression ExpressionToCode(SerializableMemberInitExpression m)
		{
			List<CodeExpression> list = new List<CodeExpression>();
			List<CodeExpression> list2 = new List<CodeExpression>();
			SerializableMemberBinding[] bindings = m.Bindings;
			foreach (SerializableMemberBinding serializableMemberBinding in bindings)
			{
				if (serializableMemberBinding.BindingType != 0)
				{
					throw new TestCodeGenerationException(string.Format("{0}: {1}", "Unsupported member binding", serializableMemberBinding));
				}
				list2.Add(MakeValue(serializableMemberBinding.Member.Name));
				list.Add(ExpressionToCode(((SerializableMemberAssignment)serializableMemberBinding).Expression));
			}
			return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(MakeThis(), "Make", MakeTypeReference(m.NewExpression.Constructor.DeclaringType)), new CodeArrayCreateExpression(new CodeTypeReference(typeof(string)), list2.ToArray()), new CodeArrayCreateExpression(new CodeTypeReference(typeof(object)), list.ToArray()));
		}

		private static CodeExpression CodeExpressionFromString(string s)
		{
			return new CodeSnippetExpression(s);
		}

		private string ExpressionToString(SerializableExpression expr)
		{
			return CodeExpressionToString(ExpressionToCode(expr));
		}

		protected void AddComment(CodeStatementCollection statements, string comment)
		{
			AddComment(statements, MakeValue(comment));
		}

		protected void AddComment(CodeStatementCollection statements, CodeExpression parameter)
		{
			if (string.Compare("true", suppressGeneratedTestLogging, true) != 0)
			{
				statements.Add(new CodeExpressionStatement(MakeManagerInvoke("Comment", parameter)));
			}
		}

		protected void AddCheckpoints(CodeStatementCollection stms, Transition trans)
		{
			string[] capturedRequirements = trans.CapturedRequirements;
			foreach (string value in capturedRequirements)
			{
				stms.Add(new CodeExpressionStatement(MakeManagerInvoke("Checkpoint", MakeValue(value))));
			}
		}

		private CodeExpression MakeFieldSelection(CodeExpression value, SerializableFieldInfo field)
		{
			if (field.IsPublic)
			{
				return new CodeFieldReferenceExpression(value, field.Name);
			}
			AddGetFieldMethod();
			return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, getFieldMethod.Name, MakeTypeReference(field.Type)), value, new CodePrimitiveExpression(field.Name));
		}

		private void AddGetFieldMethod()
		{
			if (getFieldMethod == null)
			{
				CodeTypeParameter codeTypeParameter = new CodeTypeParameter("T");
				getFieldMethod = new CodeMemberMethod();
				getFieldMethod.Name = "GetField";
				getFieldMethod.Attributes = (MemberAttributes)20483;
				getFieldMethod.ReturnType = new CodeTypeReference(codeTypeParameter);
				getFieldMethod.TypeParameters.Add(codeTypeParameter);
				getFieldMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(object)), "obj"));
				getFieldMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string)), "fieldName"));
				CodeStatementCollection statements = getFieldMethod.Statements;
				CodeExpression[] parameters = new CodeExpression[2]
				{
					new CodeArgumentReferenceExpression("obj"),
					new CodeArgumentReferenceExpression("fieldName")
				};
				statements.Add(new CodeMethodReturnStatement(new CodeCastExpression(new CodeTypeReference(codeTypeParameter), MakeHelperInvoke("GetFieldValueByName", parameters))));
			}
		}

		private CodeExpression MakeAssertAreEqual(CodeTypeReference type, CodeExpression expected, CodeExpression actual, CodeExpression context)
		{
			return MakeHelperInvoke("AssertAreEqual", new CodeTypeReference[1] { type }, MakeThisReference("Manager"), expected, actual, context);
		}

		private CodeExpression MakeAssertNotNull(CodeExpression actual, CodeExpression context)
		{
			return MakeHelperInvoke("AssertNotNull", MakeThisReference("Manager"), actual, context);
		}

		private CodeExpression MakeAssertBind(CodeTypeReference type, CodeExpression var, CodeExpression actual, CodeExpression context)
		{
			return MakeHelperInvoke("AssertBind", new CodeTypeReference[1] { type }, MakeThisReference("Manager"), var, actual, context);
		}

		private CodeExpression MakeManagerReference(string name)
		{
			return new CodeFieldReferenceExpression(MakeThisReference("Manager"), name);
		}

		private static CodeExpression MakeHelperInvoke(string name, CodeTypeReference[] typeArgs, params CodeExpression[] parameters)
		{
			return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("TestManagerHelpers"), name, typeArgs), parameters);
		}

		private string NewTemporary()
		{
			return string.Format("temp{0}", tempCounter++);
		}

		private string NewLabel()
		{
			return string.Format("label{0}", labelCounter++);
		}

		private void ResetTemporaryCounters()
		{
			tempCounter = 0;
			labelCounter = 0;
		}

		private string ToString(ActionInvocation action)
		{
			return action.Text;
		}

		protected void Warning(string message, params object[] parameters)
		{
			host.DiagMessage(DiagnosisKind.Warning, string.Format(message, parameters), null);
		}

		private string GetFullMemberName(SerializableMemberInfo member)
		{
			if (member.IsStatic && !adapterTypes.Contains(member.DeclaringType))
			{
				return string.Format("{0}.{1}", member.DeclaringType.FullName, member.Name);
			}
			return member.Name;
		}

		private static CodeExpression MakeEquals(TypeCode typeCode, CodeExpression left, CodeExpression right)
		{
			if (typeCode == TypeCode.Object)
			{
				return new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(null, "Object.Equals"), left, right);
			}
			return new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.IdentityEquality, right);
		}

		private CodeExpression MakeNot(CodeExpression operand)
		{
			return new CodeSnippetExpression(string.Format("!({0})", CodeExpressionToString(operand)));
		}

		private CodeStatementCollection MakeSwitch(CodeExpression selector, List<CodeExpression> caseLabels, List<CodeStatementCollection> caseStms, CodeStatementCollection defaultStms)
		{
			if (defaultStms == null)
			{
				defaultStms = MakeStms(new CodeThrowExceptionStatement(new CodeObjectCreateExpression("InvalidOperationException", MakeValue("never reached"))));
			}
			CodeStatementCollection codeStatementCollection = new CodeStatementCollection();
			string label = NewLabel();
			for (int i = 0; i < caseLabels.Count; i++)
			{
				CodeStatementCollection codeStatementCollection2 = new CodeStatementCollection(caseStms[i]);
				codeStatementCollection2.Add(new CodeGotoStatement(label));
				codeStatementCollection.Add(new CodeConditionStatement(MakeEquals(TypeCode.Int32, selector, caseLabels[i]), MakeArray(codeStatementCollection2)));
			}
			codeStatementCollection.AddRange(defaultStms);
			codeStatementCollection.Add(new CodeLabeledStatement(label, new CodeSnippetStatement(";")));
			return codeStatementCollection;
		}

		private static CodeStatementCollection MakeStms(CodeStatement stm)
		{
			return new CodeStatementCollection(new CodeStatement[1] { stm });
		}

		private static CodeStatement[] MakeArray(CodeStatementCollection stms)
		{
			CodeStatement[] array = new CodeStatement[stms.Count];
			stms.CopyTo(array, 0);
			return array;
		}

		private string MakeAdapterInstanceName(SerializableType type)
		{
			string name = type.Name;
			StringBuilder stringBuilder = new StringBuilder("Instance");
			bool flag = false;
			int num = 0;
			int num2 = name.Length - 1;
			while (num2 >= 0 && !flag)
			{
				switch (name[num2])
				{
				case '<':
					num--;
					break;
				case '>':
					num++;
					break;
				case '+':
				case '.':
					if (num == 0)
					{
						flag = true;
					}
					break;
				default:
					if (num == 0)
					{
						stringBuilder.Insert(0, name[num2]);
					}
					break;
				}
				num2--;
			}
			return MakeUnique(stringBuilder.ToString());
		}

		private string MakeAdapterInstanceName(string name)
		{
			StringBuilder stringBuilder = new StringBuilder("Instance");
			bool flag = false;
			int num = 0;
			int num2 = name.Length - 1;
			while (num2 >= 0 && !flag)
			{
				switch (name[num2])
				{
				case '<':
					num--;
					break;
				case '>':
					num++;
					break;
				case '+':
				case '.':
					if (num == 0)
					{
						flag = true;
					}
					break;
				default:
					if (num == 0)
					{
						stringBuilder.Insert(0, name[num2]);
					}
					break;
				}
				num2--;
			}
			return MakeUnique(stringBuilder.ToString());
		}

		private static string MakeNameFromType(SerializableType type)
		{
			return type.FullName.Replace('.', '_').Replace('+', '_').Replace('<', '_')
				.Replace('>', '_');
		}

		private void AddPreConstraints(CodeStatementCollection stms, Transition trans)
		{
			if (IsPreConstraintCheckAction(trans) && trans.PreConstraints != null)
			{
				Constraint[] preConstraints = trans.PreConstraints;
				foreach (Constraint constraint in preConstraints)
				{
					AddConstraintCheckOrBinding(stms, constraint, "Fail to check preconstraint");
				}
			}
			AddVariableUnbinding(stms, trans);
		}

		private void AddAssumptions(CodeStatementCollection stms, Transition trans)
		{
			if ((trans.Action.Symbol.Kind == ActionSymbolKind.Event || trans.Action.Symbol.Kind == ActionSymbolKind.Return) && trans.PostConstraints != null)
			{
				Constraint[] postConstraints = trans.PostConstraints;
				foreach (Constraint constraint in postConstraints)
				{
					AddConstraintCheckOrBinding(stms, constraint, "Fail to check the assumption");
				}
			}
		}

		private void AddConstraintCheckOrBinding(CodeStatementCollection stms, Constraint constraint, string failureContext)
		{
			if (constraint.Expression.NodeType == Microsoft.SpecExplorer.ObjectModel.ExpressionType.Call)
			{
				SerializableMethodCallExpression serializableMethodCallExpression = (SerializableMethodCallExpression)constraint.Expression;
				if (helperEqualityMethod == null)
				{
					helperEqualityMethod = typeof(TestManagerHelpers).GetMethod("Equality", new Type[2]
					{
						typeof(object),
						typeof(object)
					});
					if (helperEqualityMethod == null)
					{
						host.FatalError("Unable to retrieve method TestManagerHelpers.Equality(System.Object, System.Object)");
					}
				}
				if (serializableMethodCallExpression.Method.ToMethodInfo(ObjectModelHelpers.DefaultTypeResolver) == helperEqualityMethod)
				{
					if (serializableMethodCallExpression.Arguments.Length != 2)
					{
						host.FatalError("Equality call has to take two arguments");
					}
					SerializableExpression serializableExpression = ((serializableMethodCallExpression.Arguments[0].NodeType == Microsoft.SpecExplorer.ObjectModel.ExpressionType.Convert) ? ((SerializableUnaryExpression)serializableMethodCallExpression.Arguments[0]).Operand : serializableMethodCallExpression.Arguments[0]);
					SerializableExpression serializableExpression2 = ((serializableMethodCallExpression.Arguments[1].NodeType == Microsoft.SpecExplorer.ObjectModel.ExpressionType.Convert) ? ((SerializableUnaryExpression)serializableMethodCallExpression.Arguments[1]).Operand : serializableMethodCallExpression.Arguments[1]);
					SerializableParameterExpression serializableParameterExpression = serializableExpression as SerializableParameterExpression;
					SerializableParameterExpression serializableParameterExpression2 = serializableExpression2 as SerializableParameterExpression;
					if (serializableParameterExpression != null && serializableParameterExpression2 != null)
					{
						stms.Add(MakeAssertBind(new CodeTypeReference(serializableParameterExpression.Type.FullName), MakeThisReference(serializableParameterExpression.Name), MakeThisReference(serializableParameterExpression2.Name), MakeValue(constraint.Text)));
						return;
					}
					if (serializableParameterExpression != null)
					{
						stms.Add(MakeAssertBind(new CodeTypeReference(serializableParameterExpression.Type.FullName), MakeThisReference(serializableParameterExpression.Name), ExpressionToCode(serializableExpression2), MakeValue(constraint.Text)));
						return;
					}
					if (serializableParameterExpression2 != null)
					{
						stms.Add(MakeAssertBind(new CodeTypeReference(serializableParameterExpression2.Type.FullName), MakeThisReference(serializableParameterExpression2.Name), ExpressionToCode(serializableExpression), MakeValue(constraint.Text)));
						return;
					}
				}
			}
			CodeExpression value = MakeManagerInvoke("Assert", ExpressionToCode(constraint.Expression), MakeValue(string.Format("{0} : {1}", failureContext, constraint.Text)));
			stms.Add(value);
		}

		protected CodeStatementCollection MakeExpectPreConstraintStatement(List<CodeExpression> expects, List<CodeStatementCollection> expectContinuations, CodeStatementCollection defaultContinuation)
		{
			List<CodeExpression> list = new List<CodeExpression>();
			if (defaultContinuation.Count > 0)
			{
				list.Add(MakeValue(true));
			}
			else
			{
				list.Add(MakeValue(false));
			}
			list.AddRange(expects);
			CodeStatementCollection codeStatementCollection = new CodeStatementCollection();
			CodeExpression codeExpression = MakeManagerInvoke("SelectSatisfiedPreConstraint", list.ToArray());
			if (defaultContinuation == null && expectContinuations.Count == 1)
			{
				codeStatementCollection.Add(new CodeExpressionStatement(codeExpression));
				codeStatementCollection.AddRange(expectContinuations[0]);
			}
			else
			{
				MakeLabelSwitch(expects, expectContinuations, defaultContinuation, codeStatementCollection, codeExpression);
			}
			return codeStatementCollection;
		}

		private void MakeLabelSwitch(List<CodeExpression> expects, List<CodeStatementCollection> expectContinuations, CodeStatementCollection defaultContinuation, CodeStatementCollection statements, CodeExpression expectInvoke)
		{
			string text = NewTemporary();
			statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)), text, expectInvoke));
			List<CodeExpression> list = new List<CodeExpression>();
			for (int i = 0; i < expects.Count; i++)
			{
				list.Add(MakeValue(i));
			}
			statements.AddRange(MakeSwitch(new CodeSnippetExpression(text), list, expectContinuations, defaultContinuation));
		}

		protected CodeStatementCollection MakeExpectStatement(State state, ExpectKind expectKind, CodeExpression timeout, List<CodeExpression> expects, List<CodeStatementCollection> expectContinuations, CodeStatementCollection defaultContinuation)
		{
			List<CodeExpression> list = new List<CodeExpression>();
			list.Add(timeout);
			bool flag = (state.Flags & StateFlags.Accepting) != 0;
			if (defaultContinuation != null || flag)
			{
				list.Add(MakeValue(false));
			}
			else
			{
				list.Add(MakeValue(true));
			}
			list.AddRange(expects);
			CodeStatementCollection codeStatementCollection = new CodeStatementCollection();
			string name = ((expectKind == ExpectKind.Event) ? "ExpectEvent" : "ExpectReturn");
			CodeExpression codeExpression = MakeManagerInvoke(name, list.ToArray());
			if (defaultContinuation == null && expectContinuations.Count == 1)
			{
				if (expectKind == ExpectKind.Event)
				{
					List<CodeStatement> list2 = new List<CodeStatement>();
					foreach (CodeStatement item in expectContinuations[0])
					{
						list2.Add(item);
					}
					codeStatementCollection.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(codeExpression, CodeBinaryOperatorType.IdentityInequality, MakeValue(-1)), list2.ToArray(), MakeObservationTimeoutCheck(flag, expects.ToArray())));
				}
				else
				{
					codeStatementCollection.Add(new CodeExpressionStatement(codeExpression));
					codeStatementCollection.AddRange(expectContinuations[0]);
				}
			}
			else
			{
				if (defaultContinuation == null && expectKind == ExpectKind.Event)
				{
					defaultContinuation = new CodeStatementCollection(MakeObservationTimeoutCheck(flag, expects.ToArray()));
				}
				MakeLabelSwitch(expects, expectContinuations, defaultContinuation, codeStatementCollection, codeExpression);
			}
			return codeStatementCollection;
		}

		private CodeStatement[] MakeObservationTimeoutCheck(bool isAccepting, CodeExpression[] expected)
		{
			List<CodeExpression> list = new List<CodeExpression>();
			list.Add(MakeValue(isAccepting));
			list.AddRange(expected);
			return new CodeStatement[1]
			{
				new CodeExpressionStatement(MakeManagerInvoke("CheckObservationTimeout", list.ToArray()))
			};
		}

		protected CodeExpression MakeExpect(Transition transition, out CodeMemberMethod checkerMethod)
		{
			bool flag = IsPreConstraintCheckAction(transition);
			CodeExpression codeExpression = null;
			checkerMethod = null;
			CodeTypeDelegate value;
			codeExpression = ((!delegateInstanceFieldMap.TryGetValue(transition.Action.Symbol.Member, out value)) ? MakeValue(null) : new CodeDelegateCreateExpression(methodName: (!flag) ? MakeCheckerMethod(transition, value, out checkerMethod) : MakePreConstraintCheckerMethod(transition, out checkerMethod), delegateType: new CodeTypeReference(value.Name), targetObject: MakeThis()));
			ActionInvocation action = transition.Action;
			CodeTypeReference createType;
			if (flag)
			{
				createType = new CodeTypeReference("ExpectedPreConstraint");
				return new CodeObjectCreateExpression(createType, codeExpression);
			}
			createType = ((action.Symbol.Kind != ActionSymbolKind.Event) ? new CodeTypeReference("ExpectedReturn") : new CodeTypeReference("ExpectedEvent"));
			SerializableMemberInfo serializableMemberInfo = methodMap[action.Symbol.Member.Header];
			CodeExpression codeExpression2 = MakeValue(null);
			if (!serializableMemberInfo.IsStatic && !adapterTypes.Contains(serializableMemberInfo.DeclaringType))
			{
				if (action.Arguments.Length == 0)
				{
					host.FatalError("cannot get target of non-static non-adapter event or action " + serializableMemberInfo.Name);
					return new CodeSnippetExpression("Error");
				}
				codeExpression2 = ExpressionToCode(action.Arguments[0]);
			}
			return new CodeObjectCreateExpression(createType, new CodeSnippetExpression(string.Format("{0}.{1}", testLogicClassName, GetMetadataField(action.Symbol.Member).Name)), codeExpression2, codeExpression);
		}

		private string MakePreConstraintCheckerMethod(Transition trans, out CodeMemberMethod checkerMethod)
		{
			checkerMethod = new CodeMemberMethod();
			checkerMethod.Name = MakeUnique("PreConstraintChecker");
			AddPreConstraints(checkerMethod.Statements, trans);
			return checkerMethod.Name;
		}

		private static bool IsPreConstraintCheckAction(Transition trans)
		{
			return trans.Action.Symbol.Kind == ActionSymbolKind.PreConstraintCheck;
		}

		protected void GenerateCallActionStep(Transition transition, out List<CodeVariableDeclarationStatement> outputs, out SerializableMethodBase mbase, out CodeExpression receiver, out List<CodeExpression> codeArgs, out bool isCtor, out CodeStatementCollection statements)
		{
			outputs = new List<CodeVariableDeclarationStatement>();
			mbase = (SerializableMethodBase)methodMap[transition.Action.Symbol.Member.Header];
			SerializableMethodInfo serializableMethodInfo = mbase as SerializableMethodInfo;
			receiver = null;
			codeArgs = new List<CodeExpression>();
			SerializableExpression[] arguments = transition.Action.Arguments;
			SerializableParameterInfo[] parameters = mbase.Parameters;
			int num = 0;
			string text = null;
			isCtor = serializableMethodInfo == null;
			if (isCtor)
			{
				text = NewTemporary();
				outputs.Add(new CodeVariableDeclarationStatement(MakeTypeReference(mbase.DeclaringType), text));
				num++;
			}
			if (adapterTypes.Contains(mbase.DeclaringType))
			{
				receiver = MakeThisReference(adapterInstanceFieldMap[mbase.DeclaringType].Name);
			}
			else if (!mbase.IsStatic && !isCtor)
			{
				if (num >= arguments.Length)
				{
					throw new TestCodeGenerationException(string.Format("{0}: {1}", "Placeholder is not supported in test code generation", transition.Action.Text));
				}
				receiver = ExpressionToCode(arguments[num++]);
			}
			foreach (SerializableParameterInfo serializableParameterInfo in parameters)
			{
				if (serializableParameterInfo.Type.IsByRef)
				{
					string text2 = NewTemporary();
					CodeVariableDeclarationStatement codeVariableDeclarationStatement = null;
					if (!serializableParameterInfo.IsOut)
					{
						if (num >= arguments.Length)
						{
							throw new TestCodeGenerationException(string.Format("{0}: {1}", "Placeholder is not supported in test code generation", transition.Action.Text));
						}
						codeVariableDeclarationStatement = new CodeVariableDeclarationStatement(MakeTypeReference(serializableParameterInfo.Type.ElementType), text2, ExpressionToCode(arguments[num++]));
						codeArgs.Add(new CodeSnippetExpression("ref " + text2));
					}
					else
					{
						codeVariableDeclarationStatement = new CodeVariableDeclarationStatement(MakeTypeReference(serializableParameterInfo.Type.ElementType), text2);
						codeArgs.Add(new CodeSnippetExpression("out " + text2));
					}
					outputs.Add(codeVariableDeclarationStatement);
				}
				else
				{
					if (num >= arguments.Length)
					{
						throw new TestCodeGenerationException(string.Format("{0}: {1}", "Placeholder is not supported in test code generation", transition.Action.Text));
					}
					codeArgs.Add(ExpressionToCode(arguments[num++]));
				}
			}
			if (serializableMethodInfo != null && serializableMethodInfo.ReturnType != null)
			{
				text = NewTemporary();
				outputs.Add(new CodeVariableDeclarationStatement(MakeTypeReference(serializableMethodInfo.ReturnType), text));
			}
			statements = new CodeStatementCollection();
			statements.AddRange(outputs.ToArray());
			AddComment(statements, string.Format("executing step '{0}'", ToString(transition.Action)));
			if (isCtor)
			{
				CodeExpression codeExpression = new CodeObjectCreateExpression(MakeTypeReference(mbase.DeclaringType), codeArgs.ToArray());
				if (text != null)
				{
					statements.Add(new CodeAssignStatement(new CodeSnippetExpression(text), codeExpression));
				}
				else
				{
					statements.Add(codeExpression);
				}
			}
			else if (serializableMethodInfo.AssociationReference != null)
			{
				if (serializableMethodInfo.AssociationReference.Kind == Microsoft.SpecExplorer.ObjectModel.AssociationReferenceKind.GetMethod)
				{
					if (codeArgs.Count != 0)
					{
						throw new TestCodeGenerationException(string.Format("get method of property '{0}' must not take any argument.", GetFullMemberName(serializableMethodInfo.AssociationReference.Association)));
					}
					CodeExpression right = ((receiver == null) ? new CodePropertyReferenceExpression(receiver, GetFullMemberName(serializableMethodInfo.AssociationReference.Association)) : new CodePropertyReferenceExpression(receiver, serializableMethodInfo.AssociationReference.Association.Name));
					if (text == null)
					{
						throw new TestCodeGenerationException(string.Format("get method of property '{0}' must have non-void return type.", GetFullMemberName(serializableMethodInfo.AssociationReference.Association)));
					}
					statements.Add(new CodeAssignStatement(new CodeSnippetExpression(text), right));
				}
				else
				{
					if (codeArgs.Count != 1)
					{
						throw new TestCodeGenerationException(string.Format("set method of property '{0}' must take exact one argument.", GetFullMemberName(serializableMethodInfo.AssociationReference.Association)));
					}
					if (receiver != null)
					{
						statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(receiver, serializableMethodInfo.AssociationReference.Association.Name), codeArgs.First()));
					}
					else
					{
						statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(receiver, GetFullMemberName(serializableMethodInfo.AssociationReference.Association)), codeArgs.First()));
					}
				}
			}
			else
			{
				CodeExpression codeExpression2 = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(receiver, GetFullMemberName(mbase)), codeArgs.ToArray());
				if (text != null)
				{
					statements.Add(new CodeAssignStatement(new CodeSnippetExpression(text), codeExpression2));
				}
				else
				{
					statements.Add(codeExpression2);
				}
			}
		}

		protected void GenerateReturnStatement(Transition transition, List<CodeExpression> codeArgs, bool isCtor, CodeStatementCollection statements, SerializableExpression[] actionArgs, Transition successor)
		{
			List<SerializableExpression> list = new List<SerializableExpression>(successor.Action.Arguments);
			if (isCtor)
			{
				if (actionArgs.Length <= 0)
				{
					throw new TestCodeGenerationException(string.Format("{0}: {1}", "Placeholder is not supported in test code generation", transition.Action.Text));
				}
				list.Insert(0, actionArgs[0]);
			}
			if (transitionSystem.ActionMembers.Contains(successor.Action.Symbol.Member))
			{
				AddChecker(statements, successor, GenerateParameterDeclarationExpressions(successor.Action.Symbol.Member), codeArgs, list);
			}
			else
			{
				AddChecker(statements, successor, new CodeParameterDeclarationExpressionCollection(), codeArgs, list);
			}
		}

		protected bool CalculateArguments(List<CodeVariableDeclarationStatement> outputs, SerializableMemberInfo member, CodeExpression receiver, List<CodeExpression> codeArgs, bool isCtor)
		{
			codeArgs.Clear();
			bool flag = !member.IsStatic && !isCtor && !adapterTypes.Contains(member.DeclaringType);
			if (flag)
			{
				codeArgs.Add(receiver);
			}
			foreach (CodeVariableDeclarationStatement output in outputs)
			{
				codeArgs.Add(new CodeVariableReferenceExpression(output.Name));
			}
			return flag;
		}

		protected void GenerateAddReturn(Transition transition, SerializableMemberInfo memberInfo, List<CodeExpression> codeArgs, bool isCtor, CodeStatementCollection statements, bool hasReceiver)
		{
			if (isCtor)
			{
				throw new TestCodeGenerationException(string.Format("constructor call followed by non-deterministic return not supported by code generation method: {0}", transition.Action.ToString()));
			}
			if (!hasReceiver)
			{
				codeArgs.Insert(0, new CodeSnippetExpression("null"));
			}
			codeArgs.Insert(0, new CodeFieldReferenceExpression(null, GetMetadataField(memberInfo).Name));
			statements.Add(MakeManagerInvoke("AddReturn", codeArgs.ToArray()));
		}

		protected string GenerateTestMethodReturnValue(CodeMemberMethod testMethod, CodeStatementCollection currentStatements, bool IsDynamicTraversal)
		{
			string text = transitionSystem.GetSwitch("TestMethodReturnType").ExpandPrimitiveType();
			if (text.IsNoneOrEmptyValue() || string.Compare(text, "System.Void", false) == 0)
			{
				if (instanceEventRemovalStatements.Count > 0)
				{
					CodeTryCatchFinallyStatement codeTryCatchFinallyStatement = new CodeTryCatchFinallyStatement();
					if (!IsDynamicTraversal)
					{
						codeTryCatchFinallyStatement.TryStatements.Add(MakeManagerInvoke("BeginTest", MakeValue(testMethod.Name)));
					}
					codeTryCatchFinallyStatement.TryStatements.AddRange(currentStatements);
					codeTryCatchFinallyStatement.FinallyStatements.AddRange(instanceEventRemovalStatements);
					if (!IsDynamicTraversal)
					{
						codeTryCatchFinallyStatement.FinallyStatements.Add(MakeManagerInvoke("EndTest"));
					}
					testMethod.Statements.Add(codeTryCatchFinallyStatement);
				}
				else
				{
					if (!IsDynamicTraversal)
					{
						testMethod.Statements.Add(MakeManagerInvoke("BeginTest", MakeValue(testMethod.Name)));
					}
					testMethod.Statements.AddRange(currentStatements);
					if (!IsDynamicTraversal)
					{
						testMethod.Statements.Add(MakeManagerInvoke("EndTest"));
					}
				}
			}
			else
			{
				testMethod.ReturnType = new CodeTypeReference(text.ExpandPrimitiveType());
				CodeTryCatchFinallyStatement codeTryCatchFinallyStatement2 = new CodeTryCatchFinallyStatement();
				if (!IsDynamicTraversal)
				{
					codeTryCatchFinallyStatement2.TryStatements.Add(MakeManagerInvoke("BeginTest", MakeValue(testMethod.Name)));
				}
				codeTryCatchFinallyStatement2.TryStatements.AddRange(currentStatements);
				string @switch = transitionSystem.GetSwitch("TestPassedReturnValue");
				if (@switch.IsNoneOrEmptyValue())
				{
					host.DiagMessage(DiagnosisKind.Error, "Return type of test method has been configured with switch 'TestMethodReturnType', please set switch 'TestPassedReturnValue' accordingly.", null);
				}
				else
				{
					codeTryCatchFinallyStatement2.TryStatements.Add(new CodeMethodReturnStatement(CodeExpressionFromString(@switch)));
				}
				string switch2 = transitionSystem.GetSwitch("TestFailedExceptionType");
				CodeCatchClause codeCatchClause = new CodeCatchClause(NewTemporary(), new CodeTypeReference(switch2));
				AddComment(codeCatchClause.Statements, new CodeMethodInvokeExpression(CodeExpressionFromString(codeCatchClause.LocalName), "ToString"));
				string switch3 = transitionSystem.GetSwitch("TestFailedReturnValue");
				if (switch3.IsNoneOrEmptyValue())
				{
					host.DiagMessage(DiagnosisKind.Error, "Return type of test method has been configured with switch 'TestMethodReturnType', please set switch 'TestFailedReturnValue' accordingly.", null);
				}
				else
				{
					codeCatchClause.Statements.Add(new CodeMethodReturnStatement(CodeExpressionFromString(switch3)));
				}
				codeTryCatchFinallyStatement2.CatchClauses.Add(codeCatchClause);
				codeTryCatchFinallyStatement2.FinallyStatements.AddRange(instanceEventRemovalStatements);
				if (!IsDynamicTraversal)
				{
					codeTryCatchFinallyStatement2.FinallyStatements.Add(MakeManagerInvoke("EndTest"));
				}
				testMethod.Statements.Add(codeTryCatchFinallyStatement2);
			}
			return text;
		}

		protected void GenerateStaticTestMethod(CodeAttributeDeclaration[] attributes, CodeMemberMethod testMethod, string testMethodReturnType)
		{
			string @switch = transitionSystem.GetSwitch("GenerateStaticTestMethods");
			if (string.Compare(@switch, "true", true) == 0)
			{
				WrapTestMethod(testMethod, testMethodReturnType, attributes, (MemberAttributes)24579);
			}
			else
			{
				testMethod.CustomAttributes.AddRange(attributes);
			}
		}

		private void WrapTestMethod(CodeMemberMethod testMethod, string testMethodReturnType, CodeAttributeDeclaration[] customAttributes, MemberAttributes memberAttributes)
		{
			CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = testMethod.Name;
			codeMemberMethod.Attributes = memberAttributes;
			codeMemberMethod.CustomAttributes.AddRange(customAttributes);
			string text = NewTemporary();
			CodeVariableDeclarationStatement value = new CodeVariableDeclarationStatement(testLogicClassName, text, new CodeObjectCreateExpression(testLogicClassName));
			CodeVariableReferenceExpression targetObject = new CodeVariableReferenceExpression(text);
			CodeMethodInvokeExpression value2 = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject, "TestInitialize"));
			CodeMethodInvokeExpression codeMethodInvokeExpression = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject, testMethod.Name));
			CodeMethodInvokeExpression value3 = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(targetObject, "TestCleanup"));
			codeMemberMethod.Statements.Add(value);
			CodeTryCatchFinallyStatement codeTryCatchFinallyStatement = new CodeTryCatchFinallyStatement();
			codeTryCatchFinallyStatement.TryStatements.Add(value2);
			if (testMethodReturnType.IsNoneOrEmptyValue() || string.Compare(testMethodReturnType, "System.Void", false) == 0)
			{
				codeTryCatchFinallyStatement.TryStatements.Add(codeMethodInvokeExpression);
			}
			else
			{
				codeMemberMethod.ReturnType = new CodeTypeReference(testMethodReturnType);
				codeTryCatchFinallyStatement.TryStatements.Add(new CodeMethodReturnStatement(codeMethodInvokeExpression));
			}
			codeTryCatchFinallyStatement.FinallyStatements.Add(value3);
			codeMemberMethod.Statements.Add(codeTryCatchFinallyStatement);
			testMethodWrapperCollection.Add(codeMemberMethod);
		}

		protected void PreprocessInnerTestClass(out string generatedTestClassName, out string generateStaticTestMethods)
		{
			generatedTestClassName = variableResolver.Resolve("generatedtestclass");
			if (string.IsNullOrEmpty(generatedTestClassName))
			{
				generatedTestClassName = transitionSystem.Name;
			}
			generateStaticTestMethods = transitionSystem.GetSwitch("GenerateStaticTestMethods");
			if (string.Compare(generateStaticTestMethods, "true", true) == 0)
			{
				testLogicClassName = generatedTestClassName + "Inner";
			}
			else
			{
				testLogicClassName = generatedTestClassName;
			}
		}

		protected CodeTypeDeclaration GenerateInnerTestClass(string generatedTestClassName, string generateStaticTestMethods, CodeConstructor constructor, CodeTypeMemberCollection additionalMembers, CodeStatement testManagerSetStatement)
		{
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(generatedTestClassName);
			codeTypeDeclaration.IsClass = true;
			codeTypeDeclaration.IsPartial = true;
			codeTypeDeclaration.Attributes = MemberAttributes.Public;
			codeTypeDeclaration.CustomAttributes.AddRange(testAttributeProvider.CreateTestClassAttributes().ToArray());
			if (string.Compare(generateStaticTestMethods, "true", true) == 0)
			{
				CodeTypeDeclaration codeTypeDeclaration2 = new CodeTypeDeclaration(testLogicClassName);
				codeTypeDeclaration2.IsClass = true;
				codeTypeDeclaration2.IsPartial = true;
				codeTypeDeclaration2.Attributes = MemberAttributes.Public;
				AddTestClassMembers(codeTypeDeclaration2, constructor, additionalMembers, testManagerSetStatement);
				int num = 0;
				foreach (CodeMemberMethod item in testMethodWrapperCollection)
				{
					if (num == 0)
					{
						item.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, "Test Methods"));
					}
					if (num == testMethodWrapperCollection.Count - 1)
					{
						item.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
					}
					codeTypeDeclaration.Members.Add(item);
					num++;
				}
				codeTypeDeclaration.Members.Add(codeTypeDeclaration2);
			}
			else
			{
				AddTestClassMembers(codeTypeDeclaration, constructor, additionalMembers, testManagerSetStatement);
			}
			return codeTypeDeclaration;
		}
	}
}
