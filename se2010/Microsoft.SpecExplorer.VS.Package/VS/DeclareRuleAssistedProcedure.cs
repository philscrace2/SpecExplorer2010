// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.DeclareRuleAssistedProcedure
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using EnvDTE80;
using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Xrt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Type = Microsoft.ActionMachines.Cord.Type;

namespace Microsoft.SpecExplorer.VS
{
  internal class DeclareRuleAssistedProcedure : IAssistedProcedure
  {
    private SpecExplorerPackage package;
    private RuleDeclarationWizardController wizardController;
    private Dictionary<IType, ProcedureType> procedureTypes;
    private uint pdwCookie;
    private bool isValidating;

    public DeclareRuleAssistedProcedure(SpecExplorerPackage package)
    {
      this.package = package;
      this.wizardController = new RuleDeclarationWizardController(package);
      this.procedureTypes = new Dictionary<IType, ProcedureType>();
      this.wizardController.ResolveActionClause += new ActionConfigClauseResolver(this.ResolveConfigClauses);
      this.wizardController.GetSourceBindingTypes += new SourceBindingTypeProvider(this.GetSourceBindingTypes);
      this.package.SolutionBuildFinished += new EventHandler<SolutionBuildEventArgs>(this.OnSolutionBuildFinished);
    }

    public void Invoke()
    {
      if (ProjectUtils.GetAllRealProjects(this.package.DTE).Count<Project>() <= 0)
        this.package.NotificationDialog("Assisted Procedure Launch Failed", "No solution has been opened.");
      else if (!ProjectUtils.GetAllRealProjects(this.package.DTE).Any<Project>((Func<Project, bool>) (p => ProjectUtils.GetDocumentsInProject(p, ".cord").Count<string>() > 0)))
      {
        this.package.NotificationDialog("Assisted Procedure Launch Failed", "The solution does not contain any Cord config script.");
      }
      else
      {
        IEnumerable<Project> containingCordScript = ProjectUtils.GetProjectsContainingCordScript(this.package.DTE, (ICordDesignTimeScopeManager) this.package.CoreServices.GetRequiredService<ICordDesignTimeScopeManager>());
        int num = containingCordScript.Count<Project>();
        if (num > 0)
        {
          this.isValidating = true;
          IVsSolutionBuildManager2 solutionBuildManager = this.package.SolutionBuildManager;
          this.package.AssertOk(solutionBuildManager.AdviseUpdateSolutionEvents((IVsUpdateSolutionEvents) this.package, out this.pdwCookie));
          if (ErrorHandler.Succeeded(solutionBuildManager.StartUpdateProjectConfigurations((uint) num, containingCordScript.Select<Project, IVsHierarchy>((Func<Project, IVsHierarchy>) (p => this.package.ToHierarchy(p))).ToArray<IVsHierarchy>(), 65536U, 1)))
            return;
          this.package.AssertOk(solutionBuildManager.UnadviseUpdateSolutionEvents(this.pdwCookie));
          this.isValidating = false;
          this.package.DiagMessage((DiagnosisKind) 0, Resources.FailedToBuildProject, (object) null);
          this.package.ProgressMessage((VerbosityLevel) 0, Resources.ValidationFailed);
          this.package.MakeErrorListVisible();
          this.package.NotificationDialog("Assisted Procedure Launch Failed", "Failed Launching Rule Declaration Assisted Procedure, due to Validation failure. Kindly go through the Error List for details");
        }
        else
          this.Launch();
      }
    }

    private void OnSolutionBuildFinished(object sender, SolutionBuildEventArgs e)
    {
      if (!this.isValidating)
        return;
      if (!e.IsCanceled && e.IsSucceeded && this.package.ValidateAllScripts())
      {
        this.package.ProgressMessage((VerbosityLevel) 0, Resources.ValidationSucceeded);
        this.Launch();
      }
      else
      {
        this.package.ProgressMessage((VerbosityLevel) 0, Resources.ValidationFailed);
        this.package.MakeErrorListVisible();
        this.package.NotificationDialog("Assisted Procedure Launch Failed", "Failed Launching Rule Declaration Assisted Procedure, due to Validation failure. Kindly go through the Error List for details");
      }
      this.isValidating = false;
      this.package.AssertOk(this.package.SolutionBuildManager.UnadviseUpdateSolutionEvents(this.pdwCookie));
      this.pdwCookie = 0U;
    }

    private void Launch()
    {
      if (this.package.SolutionHasCSharpClass())
      {
        if (!this.wizardController.LaunchWizard())
          return;
        this.ResolveTypeBindingClassesAutoGeneration(this.wizardController.WizardData);
        this.AddTypeBindings(this.wizardController.WizardData.TypeBindingMap);
        using (Dictionary<ProcedureType, HashSet<MethodDescriptor>>.Enumerator enumerator = this.wizardController.WizardData.MethodDescriptors.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            KeyValuePair<ProcedureType, HashSet<MethodDescriptor>> current = enumerator.Current;
            HashSet<MethodDescriptor> methodDescriptorSet = current.Value;
            CodeClass codeClass;
            if (methodDescriptorSet.Count > 0 && this.wizardController.WizardData.HostClassMap.TryGetValue(current.Key, out codeClass))
            {
              this.AddRulesForActions(codeClass, (IEnumerable<MethodDescriptor>) methodDescriptorSet, this.wizardController.WizardData.AdapterTypes.Contains(current.Key), this.wizardController.WizardData.TypeBindingMap);
              this.package.NavigateTo(codeClass.ProjectItem.get_FileNames((short) 0), codeClass.StartPoint.Line - 1, codeClass.StartPoint.LineCharOffset - 1);
            }
          }
        }
      }
      else
        this.package.NotificationDialog("Assisted Procedure Launch Failed", "The solution does not contain any CSharp class to host rule method stub.");
    }

    private bool ResolveConfigClauses(
      IEnumerable<ConfigClause> selectedActionList,
      out Dictionary<ProcedureType, HashSet<MethodDescriptor>> methodDescriptorDict,
      out HashSet<ProcedureType> adapterTypes)
    {
      methodDescriptorDict = new Dictionary<ProcedureType, HashSet<MethodDescriptor>>();
      adapterTypes = new HashSet<ProcedureType>();
      using (IEnumerator<ConfigClause> enumerator = selectedActionList.GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          ConfigClause current = enumerator.Current;
          MethodDescriptor method1;
          if (current is ConfigClause.DeclareMethod)
            method1 = (MethodDescriptor) (current as ConfigClause.DeclareMethod).Method;
          else if (current is ConfigClause.ImportMethod)
          {
            method1 = (MethodDescriptor) (current as ConfigClause.ImportMethod).Method;
          }
          else
          {
            if (current is ConfigClause.ImportAllFromScope)
            {
              IType type = ((IEnumerable<IType>) ((Type) (current as ConfigClause.ImportAllFromScope).FromType).ResolvedTypes).First<IType>();
              ProcedureType procedureType = this.GetProcedureType(type);
              if (type.IsAdapter)
                adapterTypes.Add(procedureType);
              foreach (IMethod declaredMethod in type.DeclaredMethods)
              {
                if ((declaredMethod.AssociationReferences == null || declaredMethod.AssociationReferences.Length <= 0 || (declaredMethod.AssociationReferences[0].Kind == null || declaredMethod.AssociationReferences[0].Kind == 1)) && (!declaredMethod.IsConstructor || !declaredMethod.IsStatic))
                {
                  MethodDescriptor method2 = this.BuildMethodDescriptor(declaredMethod, type);
                  this.CollectMethodDescriptor(methodDescriptorDict, procedureType, method2);
                }
              }
              foreach (IAssociation declaredAssociation in type.DeclaredAssociations)
              {
                if (declaredAssociation.Kind == 1)
                {
                  MethodDescriptor method2 = this.BuildMethodDescriptor(declaredAssociation, type);
                  this.CollectMethodDescriptor(methodDescriptorDict, procedureType, method2);
                }
              }
              continue;
            }
            continue;
          }
          ProcedureType methodDeclaringType = this.GetMethodDeclaringType(method1);
          if (methodDeclaringType.IsAdapter)
            adapterTypes.Add(methodDeclaringType);
          this.CollectMethodDescriptor(methodDescriptorDict, methodDeclaringType, method1);
        }
      }
      return methodDescriptorDict.Count > 0;
    }

    private void CollectMethodDescriptor(
      Dictionary<ProcedureType, HashSet<MethodDescriptor>> methodDescriptorDict,
      ProcedureType type,
      MethodDescriptor method)
    {
      HashSet<MethodDescriptor> methodDescriptorSet;
      if (!methodDescriptorDict.TryGetValue(type, out methodDescriptorSet))
      {
        methodDescriptorSet = new HashSet<MethodDescriptor>();
        methodDescriptorDict[type] = methodDescriptorSet;
      }
      methodDescriptorSet.Add(method);
    }

    private void ResolveTypeBindingClassesAutoGeneration(RuleDeclarationWizardData wizardData)
    {
      CodeNamespace codeNamespace = wizardData.HostClassMap.FirstOrDefault<KeyValuePair<ProcedureType, CodeClass>>().Value.Namespace;
      List<KeyValuePair<ProcedureType, CodeClass>> keyValuePairList = new List<KeyValuePair<ProcedureType, CodeClass>>();
      foreach (KeyValuePair<ProcedureType, CodeClass> typeBinding in wizardData.TypeBindingMap)
      {
        if (typeBinding.Value == null)
        {
          string automaticClassName = this.ComputeAutomaticClassName("Model" + typeBinding.Key.ShortName, codeNamespace);
          keyValuePairList.Add(new KeyValuePair<ProcedureType, CodeClass>(typeBinding.Key, codeNamespace.AddClass(automaticClassName, (object) -1, (object) null, (object) null, vsCMAccess.vsCMAccessDefault)));
        }
      }
      foreach (KeyValuePair<ProcedureType, CodeClass> keyValuePair in keyValuePairList)
        wizardData.TypeBindingMap[keyValuePair.Key] = keyValuePair.Value;
    }

    private string ComputeAutomaticClassName(string namePrefix, CodeNamespace codeNamespace)
    {
      string tempName = namePrefix;
      for (int index = 1; index <= 1000000000; ++index)
      {
        if (!codeNamespace.Members.OfType<CodeType>().Any<CodeType>((Func<CodeType, bool>) (elem => elem[] == tempName)))
          return tempName;
        tempName = namePrefix + (object) index;
      }
      return string.Empty;
    }

    private void AddRulesForActions(
      CodeClass codeClass,
      IEnumerable<MethodDescriptor> methods,
      bool isAdapter,
      Dictionary<ProcedureType, CodeClass> typeBindingClassCache)
    {
      IEnumerable<CodeFunction> allFunctions = this.GetAllFunctions(codeClass);
      Dictionary<string, CodeFunction> attributes;
      this.TryGetMethodAttributes(allFunctions, out attributes);
      HashSet<string> allMethodNames = this.GetAllMethodNames(allFunctions);
      IEnumerable<string> importedNamespaces = EnvDTEUtils.RetrieveNamespaceImports(codeClass).Select<CodeImport, string>((Func<CodeImport, string>) (import => import.Namespace));
      Func<string, string> shortenTypeName = (Func<string, string>) (fullTypeName =>
      {
        Project containingProject = codeClass.ProjectItem.ContainingProject;
        CodeType codeType = containingProject.CodeModel.CodeTypeFromFullName(fullTypeName);
        return EnvDTEUtils.ShortenTypeName(codeType == null ? fullTypeName : containingProject.CodeModel.CreateCodeTypeRef((object) codeType).AsString, importedNamespaces, codeClass.ProjectItem.ContainingProject);
      });
      using (IEnumerator<MethodDescriptor> enumerator = methods.GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          MethodDescriptor current = enumerator.Current;
          List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
          Dictionary<string, vsCMParameterKind> parametersKind = new Dictionary<string, vsCMParameterKind>();
          foreach (Parameter parameter in (Parameter[]) current.Parameters)
          {
            KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>((string) parameter.Name, this.GetParameterTypeName((IType) parameter.ResolvedType, typeBindingClassCache));
            parameters.Add(keyValuePair);
            if (parameter.Kind == 4)
              parametersKind[(string) parameter.Name] = vsCMParameterKind.vsCMParameterKindOut;
            if (parameter.Kind == 8)
              parametersKind[(string) parameter.Name] = vsCMParameterKind.vsCMParameterKindRef;
          }
          string methodName1 = this.GetMethodName(current);
          string methodName2 = methodName1;
          if (current.MethodName == null)
          {
            if ((codeClass as CodeClass2).IsShared)
              this.package.DiagMessage((DiagnosisKind) 0, string.Format("Constructor method {0} cannot add to static class", (object) methodName1), (object) null);
            else if (attributes == null || this.IsNeedAddingRuleForAction(attributes, allFunctions, typeBindingClassCache, allMethodNames, current, true, ref methodName2))
            {
              string ruleAttributeLabel = this.GetRuleAttributeLabel(methodName1, parameters, parametersKind, true, typeBindingClassCache.Values.Contains<CodeClass>(codeClass), false, false, (string) null);
              this.AddConstructor(codeClass, methodName2, parameters, parametersKind, ruleAttributeLabel, shortenTypeName);
            }
            else
              this.package.DiagMessage((DiagnosisKind) 1, string.Format("Method {0} has already in the class", (object) methodName2), (object) null);
          }
          else if (attributes == null || this.IsNeedAddingRuleForAction(attributes, allFunctions, typeBindingClassCache, allMethodNames, current, false, ref methodName2))
          {
            bool isStatic1 = isAdapter || (int) current.IsStatic != 0;
            string str = (string) null;
            CodeClass2 codeClass2 = codeClass as CodeClass2;
            if (!isStatic1 && codeClass2.IsShared)
            {
              string parameterTypeName;
              if (current.RepresentationType != null)
              {
                str = this.BuildParameterName(((IMember) current.RepresentationType).ShortName, parameters);
                parameterTypeName = this.GetParameterTypeName((IType) current.RepresentationType, typeBindingClassCache);
              }
              else
              {
                this.package.Assert(((InstantiatedName) current.MethodName).Prefix != null, "method.MethodName.Prefix cannot be null");
                str = this.BuildParameterName((string) ((InstantiatedName) ((InstantiatedName) current.MethodName).Prefix).Name, parameters);
                if (((ICollection<IType>) ((InstantiatedName) ((InstantiatedName) current.MethodName).Prefix).ResolvedTypes).Count == 1)
                {
                  parameterTypeName = this.GetParameterTypeName(((IEnumerable<IType>) ((InstantiatedName) ((InstantiatedName) current.MethodName).Prefix).ResolvedTypes).First<IType>(), typeBindingClassCache);
                }
                else
                {
                  this.package.DiagMessage((DiagnosisKind) 0, string.Format("Failed to resolve declaration type of method {0}", (object) ((InstantiatedName) current.MethodName).Name), (object) null);
                  continue;
                }
              }
              KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(str, parameterTypeName);
              parameters.Insert(0, keyValuePair);
            }
            string ruleAttributeLabel = this.GetRuleAttributeLabel(methodName1, parameters, parametersKind, false, typeBindingClassCache.Values.Contains<CodeClass>(codeClass), isStatic1, !((Type) current.ResultType).IsVoid(), str);
            bool isStatic2 = isStatic1 || codeClass2.IsShared;
            this.AddFunction(codeClass, methodName2, (Type) current.ResultType, parameters, parametersKind, isStatic2, typeBindingClassCache, ruleAttributeLabel, shortenTypeName);
          }
          else
            this.package.DiagMessage((DiagnosisKind) 1, string.Format("Method {0} has already in the class", (object) methodName1), (object) null);
        }
      }
    }

    private string BuildParameterName(string name, List<KeyValuePair<string, string>> parameters)
    {
      name = name.Substring(0, 1).ToLower() + name.Substring(1);
      while (parameters.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => p.Key == name)))
        name += "1";
      return name;
    }

    private object ResolveType(
      Type type,
      Dictionary<ProcedureType, CodeClass> typeBindingClassCache)
    {
      if (type.IsVoid)
        return (object) "void";
      IType type1 = ((IEnumerable<IType>) type.ResolvedTypes).First<IType>();
      CodeClass codeClass;
      if (!typeBindingClassCache.TryGetValue(this.GetProcedureType(type1), out codeClass))
        return (object) ((IMember) type1).FullName;
      return (object) codeClass;
    }

    private ProcedureType GetMethodDeclaringType(MethodDescriptor method)
    {
      if (method.MethodName == null)
        return this.GetProcedureType(((IEnumerable<IType>) ((Type) method.ResultType).ResolvedTypes()).First<IType>());
      if (((ICollection<IType>) ((InstantiatedName) ((InstantiatedName) method.MethodName).Prefix).ResolvedTypes).Count == 0)
        return new ProcedureType((string) ((InstantiatedName) ((InstantiatedName) method.MethodName).Prefix).Name, (string) ((InstantiatedName) ((InstantiatedName) method.MethodName).Prefix).Name);
      return this.GetProcedureType(((IEnumerable<IType>) ((InstantiatedName) ((InstantiatedName) method.MethodName).Prefix).ResolvedTypes).First<IType>());
    }

    private ProcedureType GetProcedureType(IType type)
    {
      ProcedureType procedureType;
      if (!this.procedureTypes.TryGetValue(type, out procedureType))
      {
        procedureType = new ProcedureType(type);
        this.procedureTypes[type] = procedureType;
      }
      return procedureType;
    }

    private string GetMethodName(MethodDescriptor method)
    {
      if (method.MethodName == null)
        return ((IMember) ((IEnumerable<IType>) ((Type) method.ResultType).ResolvedTypes()).First<IType>()).ShortName();
      return (string) ((InstantiatedName) method.MethodName).Name;
    }

    private HashSet<ProcedureType> GetSourceBindingTypes(
      Dictionary<ProcedureType, HashSet<MethodDescriptor>> methodDescriptorDict,
      IEnumerable<CodeElement> codeElements,
      out Dictionary<ProcedureType, CodeClass> existTypeBindings)
    {
      existTypeBindings = new Dictionary<ProcedureType, CodeClass>();
      HashSet<IType> itypeSet = new HashSet<IType>();
      HashSet<ProcedureType> procedureTypeSet = new HashSet<ProcedureType>();
      using (Dictionary<ProcedureType, HashSet<MethodDescriptor>>.Enumerator enumerator1 = methodDescriptorDict.GetEnumerator())
      {
        while (enumerator1.MoveNext())
        {
          KeyValuePair<ProcedureType, HashSet<MethodDescriptor>> current1 = enumerator1.Current;
          bool flag = false;
          using (HashSet<MethodDescriptor>.Enumerator enumerator2 = current1.Value.GetEnumerator())
          {
            while (enumerator2.MoveNext())
            {
              MethodDescriptor current2 = enumerator2.Current;
              if (current2.IsStatic == null && !flag)
              {
                flag = true;
                if (current2.MethodName == null)
                  itypeSet.Add(((IEnumerable<IType>) ((Type) current2.ResultType).ResolvedTypes()).First<IType>());
                else if (((ICollection<IType>) ((InstantiatedName) ((InstantiatedName) current2.MethodName).Prefix).ResolvedTypes()).Count == 1)
                  itypeSet.Add(((IEnumerable<IType>) ((InstantiatedName) ((InstantiatedName) current2.MethodName).Prefix).ResolvedTypes()).First<IType>());
              }
              if (current2.MethodName != null)
              {
                if (!((Type) current2.ResultType).IsVoid())
                  itypeSet.Add(((IEnumerable<IType>) ((Type) current2.ResultType).ResolvedTypes()).First<IType>());
                foreach (Parameter parameter in (Parameter[]) current2.Parameters)
                  itypeSet.Add((IType) parameter.ResolvedType);
              }
            }
          }
        }
      }
      Dictionary<string, CodeClass> existBindingTypes = this.GetExistBindingTypes(codeElements);
      using (HashSet<IType>.Enumerator enumerator1 = itypeSet.GetEnumerator())
      {
        while (enumerator1.MoveNext())
        {
          IType current1 = enumerator1.Current;
          CodeClass codeClass;
          using (IEnumerator<IType> enumerator2 = this.EnuemrateTypeArgument(current1).GetEnumerator())
          {
            while (((IEnumerator) enumerator2).MoveNext())
            {
              IType current2 = enumerator2.Current;
              if (this.IsNeedTypeBinding(current2))
              {
                if (existBindingTypes.TryGetValue(((IMember) current2).FullName(), out codeClass))
                  existTypeBindings[this.GetProcedureType(current2)] = codeClass;
                else
                  procedureTypeSet.Add(this.GetProcedureType(current2));
              }
            }
          }
          if (this.IsNeedTypeBinding(current1))
          {
            if (existBindingTypes.TryGetValue(((IMember) current1).FullName, out codeClass))
              existTypeBindings[this.GetProcedureType(current1)] = codeClass;
            else
              procedureTypeSet.Add(this.GetProcedureType(current1));
          }
        }
      }
      return procedureTypeSet;
    }

    private IEnumerable<IType> EnuemrateTypeArgument(IType type)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerable<IType>) new DeclareRuleAssistedProcedure.\u003CEnuemrateTypeArgument\u003Ed__10(-2)
      {
        \u003C\u003E4__this = this,
        \u003C\u003E3__type = type
      };
    }

    private bool IsNeedTypeBinding(IType type)
    {
      if (type.Category == 4 && !type.IsCompoundValue && !type.IsNative)
        return string.Compare(((IMember) type).FullName, "System.Object", true) != 0;
      return false;
    }

    private void AddTypeBindings(Dictionary<ProcedureType, CodeClass> types)
    {
      foreach (KeyValuePair<ProcedureType, CodeClass> type in types)
      {
        string nameForGenericType = this.GetAliasNameForGenericType(type.Key.FullName, new Dictionary<ProcedureType, CodeClass>(), true);
        this.AddTypeBindingAttribute(type.Value, string.Format("\"{0}\"", (object) nameForGenericType));
      }
    }

    private Dictionary<string, CodeClass> GetExistBindingTypes(
      IEnumerable<CodeElement> codeElements)
    {
      Dictionary<string, CodeClass> dictionary = new Dictionary<string, CodeClass>();
      foreach (CodeClass2 allCodeClass in this.GetAllCodeClasses(codeElements))
      {
        foreach (CodeAttribute attribute in allCodeClass.Attributes)
        {
          if (attribute.FullName == "Microsoft.Modeling.TypeBindingAttribute")
          {
            string str = attribute.Value;
            string index = str.Substring(1, str.Length - 2);
            dictionary[index] = (CodeClass) allCodeClass;
          }
        }
      }
      return dictionary;
    }

    private IEnumerable<CodeClass2> GetAllCodeClasses(
      IEnumerable<CodeElement> elems)
    {
      foreach (CodeElement elem in elems)
      {
        if (elem.Kind == vsCMElement.vsCMElementNamespace)
        {
          CodeNamespace nspace = elem as CodeNamespace;
          foreach (CodeClass2 codeClass2 in this.GetAllCodeClassesFromNameSpace(nspace.Members))
            yield return codeClass2;
        }
      }
    }

    private HashSet<CodeClass2> GetAllCodeClassesFromNameSpace(CodeElements elems)
    {
      HashSet<CodeClass2> codeClass2Set = new HashSet<CodeClass2>();
      foreach (CodeElement elem in elems)
      {
        if (elem.Kind == vsCMElement.vsCMElementClass)
        {
          CodeClass2 codeClass2 = elem as CodeClass2;
          codeClass2Set.Add(codeClass2);
          codeClass2Set.UnionWith((IEnumerable<CodeClass2>) this.GetAllCodeClassesFromNameSpace(codeClass2.Members));
        }
      }
      return codeClass2Set;
    }

    private bool IsNeedAddingRuleForAction(
      Dictionary<string, CodeFunction> attributes,
      IEnumerable<CodeFunction> allFunctions,
      Dictionary<ProcedureType, CodeClass> typeBindingClassCache,
      HashSet<string> methodNames,
      MethodDescriptor method,
      bool isCtor,
      ref string methodName)
    {
      string methodName1 = methodName;
      CodeFunction codeFunction;
      if (attributes.TryGetValue(methodName, out codeFunction) && method.IsStatic == (codeFunction.IsShared ? 1 : 0) && this.HasSameParameters(codeFunction.Parameters, (Parameter[]) method.Parameters, typeBindingClassCache) || isCtor && this.HasSameMethod(allFunctions, typeBindingClassCache, method, methodName1))
        return false;
      methodName = this.MakeUniqueName(methodName, this.GetMethodName(method), methodNames);
      return true;
    }

    private bool HasSameMethod(
      IEnumerable<CodeFunction> allFunctions,
      Dictionary<ProcedureType, CodeClass> typeBindingClassCache,
      MethodDescriptor method,
      string methodName)
    {
      foreach (CodeFunction codeFunction in allFunctions.Where<CodeFunction>((Func<CodeFunction, bool>) (f => f[] == methodName)))
      {
        if (this.HasSameParameters(codeFunction.Parameters, (Parameter[]) method.Parameters, typeBindingClassCache))
          return true;
      }
      return false;
    }

    private string MakeUniqueName(
      string oldUniqueName,
      string baseName,
      HashSet<string> methodNames)
    {
      while (!methodNames.Add(oldUniqueName))
      {
        if (oldUniqueName == baseName)
        {
          oldUniqueName += "1";
        }
        else
        {
          try
          {
            int num = Convert.ToInt32(oldUniqueName.Substring(baseName.Length)) + 1;
            oldUniqueName = baseName + (object) num;
          }
          catch
          {
            oldUniqueName += "1";
          }
        }
      }
      return oldUniqueName;
    }

    private string GetDefaultAttributeValueFromMethodName(string attributeValue)
    {
      int length = attributeValue.IndexOf("(");
      if (length >= 0)
        attributeValue = attributeValue.Substring(0, length);
      int num1 = attributeValue.IndexOf("\"");
      if (num1 >= 0)
        attributeValue = attributeValue.Substring(num1 + 1);
      int num2 = attributeValue.LastIndexOf(".");
      if (num2 >= 0)
        attributeValue = attributeValue.Substring(num2 + 1);
      attributeValue = attributeValue.Trim();
      int num3 = attributeValue.LastIndexOf(" ");
      if (num3 >= 0)
        attributeValue = attributeValue.Substring(num3 + 1);
      return attributeValue;
    }

    private string GetAttributeValueFromMethodName(string attributeValue, string methodName)
    {
      if (string.IsNullOrEmpty(attributeValue))
        return methodName;
      return this.GetDefaultAttributeValueFromMethodName(attributeValue);
    }

    private string GetParameterTypeName(
      IType type,
      Dictionary<ProcedureType, CodeClass> typeBindingClassCache)
    {
      ProcedureType procedureType = this.GetProcedureType(type);
      CodeClass codeClass;
      string typeFullName;
      if (typeBindingClassCache.TryGetValue(procedureType, out codeClass))
      {
        typeFullName = codeClass.FullName;
        if (typeFullName.IndexOf('<') > 0)
        {
          this.package.Session.Host.NotificationDialog("Fail", "Type binding do not support generic type");
          return typeFullName;
        }
      }
      else
        typeFullName = type.IsAddressType || type.IsArrayType ? ((IMember) type.ElementType).FullName : ((IMember) type).FullName;
      return this.GetAliasNameForGenericType(typeFullName, typeBindingClassCache, false);
    }

    private string GetAliasNameForGenericType(
      string typeFullName,
      Dictionary<ProcedureType, CodeClass> typeBindingClassCache,
      bool useCShanrpAlias)
    {
      if (!typeFullName.Contains<char>('<') && !typeFullName.Contains<char>(','))
        return typeFullName;
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      CharEnumerator enumerator = typeFullName.GetEnumerator();
      while (enumerator.MoveNext())
      {
        char current = enumerator.Current;
        switch (current)
        {
          case ',':
          case '<':
          case '>':
            stringBuilder1.Append(this.ConvertType(stringBuilder2.ToString(), typeBindingClassCache, useCShanrpAlias));
            stringBuilder1.Append(current);
            stringBuilder2 = new StringBuilder();
            continue;
          default:
            stringBuilder2.Append(current);
            continue;
        }
      }
      return stringBuilder1.ToString();
    }

    private string ConvertType(
      string name,
      Dictionary<ProcedureType, CodeClass> typeBindingClassCache,
      bool useCSharpAlias)
    {
      foreach (KeyValuePair<ProcedureType, CodeClass> keyValuePair in typeBindingClassCache)
      {
        if (keyValuePair.Key.FullName == name)
          return keyValuePair.Value.FullName;
      }
      if (useCSharpAlias)
        name = ExtensionMethods.CollapsePrimitiveType(name);
      return name;
    }

    private bool TryGetMethodAttributes(
      IEnumerable<CodeFunction> allFunctions,
      out Dictionary<string, CodeFunction> attributes)
    {
      attributes = new Dictionary<string, CodeFunction>();
      foreach (CodeFunction allFunction in allFunctions)
      {
        foreach (CodeElement attribute in allFunction.Attributes)
        {
          if (attribute.Kind == vsCMElement.vsCMElementAttribute)
          {
            CodeAttribute2 codeAttribute2 = attribute as CodeAttribute2;
            if (codeAttribute2[] == "Action")
              attributes[this.GetAttributeValueFromMethodName(codeAttribute2.Value, allFunction[])] = allFunction;
            else if ("Rule" == codeAttribute2[])
            {
              if (codeAttribute2.Arguments.Count == 0)
              {
                attributes[this.GetAttributeValueFromMethodName("", allFunction[])] = allFunction;
              }
              else
              {
                foreach (CodeElement codeElement in codeAttribute2.Arguments)
                {
                  CodeAttributeArgument attributeArgument = codeElement as CodeAttributeArgument;
                  if (attributeArgument[] == "Action")
                  {
                    attributes[this.GetAttributeValueFromMethodName(attributeArgument.Value, allFunction[])] = allFunction;
                    break;
                  }
                }
              }
            }
          }
        }
      }
      return attributes.Count > 0;
    }

    private HashSet<string> GetAllMethodNames(IEnumerable<CodeFunction> allFunctions)
    {
      HashSet<string> stringSet = new HashSet<string>();
      foreach (CodeFunction allFunction in allFunctions)
        stringSet.Add(allFunction[]);
      return stringSet;
    }

    private bool HasSameParameters(
      CodeElements funcParameters,
      Parameter[] methodParameters,
      Dictionary<ProcedureType, CodeClass> typeBindingClassCache)
    {
      List<CodeTypeRef> codeTypeRefList = new List<CodeTypeRef>();
      foreach (CodeElement funcParameter in funcParameters)
      {
        CodeParameter codeParameter = funcParameter as CodeParameter;
        codeTypeRefList.Add(codeParameter.Type);
      }
      int index = 0;
      foreach (Parameter methodParameter in methodParameters)
      {
        if ((string) methodParameter.Name != "this")
        {
          if (index >= codeTypeRefList.Count || !this.HasSameType((IType) methodParameter.ResolvedType, codeTypeRefList[index].AsFullName, typeBindingClassCache))
            return false;
          ++index;
        }
      }
      return true;
    }

    private bool HasSameType(
      IType type,
      string modelMethodName,
      Dictionary<ProcedureType, CodeClass> typeBindingClassCache)
    {
      return this.GetParameterTypeName(type, typeBindingClassCache) == modelMethodName;
    }

    private string GetDescriptiveLocalName(ILocal local)
    {
      string str = local.Name();
      int length = str.LastIndexOf("_p" + (object) local.Index());
      if (length >= 0)
        str = str.Substring(0, length);
      return str;
    }

    private IEnumerable<CodeFunction> GetAllFunctions(CodeClass codeClass)
    {
      IEnumerator enumerator = codeClass.Members.GetEnumerator();
      try
      {
        while (enumerator.MoveNext())
        {
          CodeElement elem = (CodeElement) enumerator.Current;
          if (elem.Kind == vsCMElement.vsCMElementFunction)
            yield return elem as CodeFunction;
        }
      }
      finally
      {
        IDisposable disposable = enumerator as IDisposable;
        disposable?.Dispose();
      }
    }

    private MethodDescriptor BuildMethodDescriptor(IMethod method, IType type)
    {
      InstantiatedName instantiatedName = (InstantiatedName) null;
      if (!method.IsConstructor)
        instantiatedName = this.BuildMethodInstantiatedName(((IMember) method).ShortName, type);
      bool isStatic = method.IsStatic;
      Type type1 = !method.IsConstructor ? this.BuildReturnType(method.ResultType, method.ResultIsVoid) : this.BuildReturnType(type, false);
      return new MethodDescriptor(instantiatedName, isStatic, this.BuildParameters(method.Parameters), type1, (ActionKind) 11, (ParameterDomainDefinition[]) null, (CodeBlock[]) null);
    }

    private InstantiatedName BuildMethodInstantiatedName(
      string shortName,
      IType type)
    {
      InstantiatedName instantiatedName1 = (InstantiatedName) null;
      string str = ((IMember) type).FullName;
      int length = str.LastIndexOf(".");
      if (length > 0)
      {
        instantiatedName1 = new InstantiatedName((Location) Location.None, (InstantiatedName) null, str.Substring(0, length), (Type[]) null);
        str = str.Substring(length + 1);
      }
      InstantiatedName instantiatedName2 = new InstantiatedName((Location) Location.None, instantiatedName1, str, (Type[]) null);
      InstantiatedName instantiatedName3 = instantiatedName2;
      List<IType> itypeList1 = new List<IType>();
      itypeList1.Add(type);
      List<IType> itypeList2 = itypeList1;
      instantiatedName3.ResolvedTypes((IList<IType>) itypeList2);
      return new InstantiatedName((Location) Location.None, instantiatedName2, shortName, (Type[]) null);
    }

    private MethodDescriptor BuildMethodDescriptor(
      IAssociation association,
      IType type)
    {
      return new MethodDescriptor(this.BuildMethodInstantiatedName(((IMember) association).ShortName, type), association.IsStatic, this.BuildParameters(association.FireMethod.Parameters()), this.BuildReturnType(association.FireMethod.ResultType, association.FireMethod.ResultIsVoid()), (ActionKind) 4, (ParameterDomainDefinition[]) null, (CodeBlock[]) null);
    }

    private Type BuildReturnType(IType type, bool isVoid)
    {
      Type type1 = (Type) new Type.Simple(!isVoid ? new InstantiatedName((Location) Location.None, (InstantiatedName) null, ((IMember) type).FullName(), (Type[]) null) : new InstantiatedName((Location) Location.None, (InstantiatedName) null, "void", (Type[]) null));
      Type type2 = type1;
      List<IType> itypeList1 = new List<IType>();
      itypeList1.Add(type);
      List<IType> itypeList2 = itypeList1;
      type2.set_ResolvedTypes((IList<IType>) itypeList2);
      return type1;
    }

    private Parameter[] BuildParameters(ILocal[] locals)
    {
      List<Parameter> parameterList = new List<Parameter>();
      foreach (ILocal local in locals)
      {
        if (!(local.Name == "this"))
        {
          string descriptiveLocalName = this.GetDescriptiveLocalName(local);
          IType itype = local.Type;
          ParameterKind parameterKind = (ParameterKind) 2;
          if (itype.IsAddressType)
            parameterKind = !local.IsOut ? (ParameterKind) 8 : (ParameterKind) 4;
          if (itype.IsAddressType || itype.IsArrayType)
            itype = itype.ElementType;
          parameterList.Add(new Parameter((Location) Location.None, parameterKind, (Type) null, descriptiveLocalName)
          {
            ResolvedType = (__Null) itype
          });
        }
      }
      return parameterList.ToArray();
    }

    private void AddFunction(
      CodeClass addedClass,
      string modifiedMethodName,
      Type returnType,
      List<KeyValuePair<string, string>> parameters,
      Dictionary<string, vsCMParameterKind> parametersKind,
      bool isStatic,
      Dictionary<ProcedureType, CodeClass> typeBindingClassCache,
      string attrString,
      Func<string, string> shortenTypeName)
    {
      string str1 = this.ResolveType(returnType, typeBindingClassCache) as string;
      string str2 = shortenTypeName != null ? shortenTypeName(str1) : str1;
      CodeFunction codeFun = addedClass.AddFunction(modifiedMethodName, vsCMFunction.vsCMFunctionFunction, (object) str2, (object) -1, vsCMAccess.vsCMAccessDefault, (object) null);
      codeFun.IsShared = isStatic;
      this.AddParameters(codeFun, parameters, parametersKind, shortenTypeName);
      this.AddRuleAttribute(codeFun, attrString);
      this.AddFunctionNotImplementedException(codeFun);
    }

    private string GetRuleAttributeLabel(
      string methodName,
      List<KeyValuePair<string, string>> parameters,
      Dictionary<string, vsCMParameterKind> parametersKind,
      bool isCtor,
      bool isBindingType,
      bool isStatic,
      bool hasReturnValue,
      string instanceName)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("\"");
      if (isCtor)
        stringBuilder.Append("new ");
      if (!isStatic)
      {
        if (isBindingType)
          stringBuilder.Append("this.");
        else
          stringBuilder.AppendFormat("{0}.", (object) instanceName);
      }
      stringBuilder.AppendFormat(methodName);
      stringBuilder.Append("(");
      if (parameters.Count > 0)
      {
        bool flag = true;
        foreach (KeyValuePair<string, string> parameter in parameters)
        {
          string key = parameter.Key;
          if (!(key == instanceName))
          {
            if (flag)
              flag = false;
            else
              stringBuilder.Append(", ");
            vsCMParameterKind vsCmParameterKind;
            parametersKind.TryGetValue(key, out vsCmParameterKind);
            switch (vsCmParameterKind)
            {
              case vsCMParameterKind.vsCMParameterKindRef:
                stringBuilder.Append("ref ");
                break;
              case vsCMParameterKind.vsCMParameterKindOut:
                stringBuilder.Append("out ");
                break;
            }
            stringBuilder.Append(key);
          }
        }
      }
      stringBuilder.Append(")");
      if (hasReturnValue)
        stringBuilder.Append("/result");
      stringBuilder.Append("\"");
      return stringBuilder.ToString();
    }

    private void AddConstructor(
      CodeClass addedClass,
      string methodName,
      List<KeyValuePair<string, string>> parameters,
      Dictionary<string, vsCMParameterKind> parametersKind,
      string attrString,
      Func<string, string> shortenTypeName)
    {
      CodeFunction codeFun = addedClass.AddFunction(methodName, vsCMFunction.vsCMFunctionConstructor, (object) null, (object) -1, vsCMAccess.vsCMAccessPublic, (object) null);
      this.AddParameters(codeFun, parameters, parametersKind, shortenTypeName);
      this.AddRuleAttribute(codeFun, attrString);
      this.AddConstructorNotImplementedException(codeFun);
    }

    private void AddTypeBindingAttribute(CodeClass addedClass, string attribute)
    {
      if ((addedClass as CodeClass2).IsShared)
        this.package.Session.Host.DiagMessage((DiagnosisKind) 1, string.Format("type binding to a static class will result in validation failure: class {0}", (object) addedClass.FullName), (object) null);
      foreach (CodeElement attribute1 in addedClass.Attributes)
      {
        if ((attribute1 as CodeAttribute)[] == "TypeBinding")
        {
          this.package.Session.Host.DiagMessage((DiagnosisKind) 1, string.Format("The type binding {0} has already been added", (object) attribute), (object) null);
          return;
        }
      }
      addedClass.AddAttribute("TypeBinding", attribute, (object) null);
    }

    private void AddRuleAttribute(CodeFunction codeFun, string attribute)
    {
      codeFun.AddAttribute("Rule", string.Format("Action = {0}", (object) attribute), (object) null);
    }

    private void AddParameters(
      CodeFunction codeFun,
      List<KeyValuePair<string, string>> parameters,
      Dictionary<string, vsCMParameterKind> parametersKind,
      Func<string, string> shortenTypeName)
    {
      foreach (KeyValuePair<string, string> parameter in parameters)
      {
        string str = shortenTypeName != null ? shortenTypeName(parameter.Value) : parameter.Value;
        CodeParameter2 codeParameter2 = (CodeParameter2) codeFun.AddParameter(parameter.Key, (object) str, (object) -1);
        vsCMParameterKind vsCmParameterKind;
        if (parametersKind.TryGetValue(parameter.Key, out vsCmParameterKind))
          codeParameter2.ParameterKind = vsCmParameterKind;
      }
    }

    private void AddFunctionNotImplementedException(CodeFunction codeFun)
    {
      EditPoint2 editPoint = (EditPoint2) codeFun.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
      if (editPoint.LineLength > 0)
        editPoint.Delete((object) editPoint.LineLength);
      editPoint.Indent((TextPoint) null, 3);
      editPoint.Insert("throw new NotImplementedException();");
    }

    private void AddConstructorNotImplementedException(CodeFunction codeFun)
    {
      EditPoint2 editPoint = (EditPoint2) codeFun.GetStartPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
      editPoint.Indent((TextPoint) null, 1);
      editPoint.Insert("throw new NotImplementedException();");
      editPoint.InsertNewLine(1);
      editPoint.Indent((TextPoint) null, 2);
    }
  }
}
