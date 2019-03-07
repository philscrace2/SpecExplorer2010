// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.CordSyntaxElementBuilder
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using EnvDTE80;
using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.Modeling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer.VS
{
  internal class CordSyntaxElementBuilder
  {
    private Project containerProject;
    private IEnumerable<string> importedNamespaces;

    public CordSyntaxElementBuilder(
      SpecExplorerPackage package,
      string containerProjectName,
      string containerScriptPath)
    {
      this.containerProject = package.GetProjectByUniqueName(containerProjectName);
      this.importedNamespaces = ((IEnumerable<Namespace>) package.CordScopeManager.GetCordDesignTimeManager(containerProjectName).GetScriptSyntax(containerScriptPath).GlobalNamespaces).Select<Namespace, string>((Func<Namespace, string>) (import => ((InstantiatedName) import.Name).Flatten()));
    }

    public Config CreateConfig(string configName)
    {
      return new Config((Documentary) null, (Location) Location.None, configName, (IEnumerable<ConfigClause>) null);
    }

    public ConfigClause.ImportMethod CreateActionImport(
      CodeElement codeElement,
      CodeType container)
    {
      MethodDescriptor methodDescriptor;
      switch (codeElement.Kind)
      {
        case vsCMElement.vsCMElementFunction:
          methodDescriptor = this.CreateMethodDescriptor(codeElement as CodeFunction, container);
          break;
        case vsCMElement.vsCMElementEvent:
          methodDescriptor = this.CreateMethodDescriptor(codeElement as CodeEvent, container);
          break;
        default:
          methodDescriptor = (MethodDescriptor) null;
          break;
      }
      if (methodDescriptor != null)
        return new ConfigClause.ImportMethod((Location) Location.None, methodDescriptor);
      return (ConfigClause.ImportMethod) null;
    }

    public MethodDescriptor CreateMethodDescriptor(
      CodeFunction codeFunction,
      CodeType container)
    {
      Parameter[] parameters = this.CreateParameters(codeFunction.Parameters, false);
      string str = EnvDTEUtils.ShortenTypeName(container.FullName, this.importedNamespaces, this.containerProject);
      InstantiatedName instantiatedName1 = new InstantiatedName((Location) Location.None, (InstantiatedName) null, str, (Type[]) null);
      Type.Simple type;
      InstantiatedName instantiatedName2;
      if (codeFunction.FunctionKind == vsCMFunction.vsCMFunctionConstructor)
      {
        type = this.CreateType((codeFunction.Parent as CodeElement).FullName);
        instantiatedName2 = (InstantiatedName) null;
      }
      else
      {
        type = this.CreateType(codeFunction.Type);
        instantiatedName2 = new InstantiatedName((Location) Location.None, instantiatedName1, codeFunction[], (Type[]) null);
      }
      if (parameters == null || type == null)
        return (MethodDescriptor) null;
      return new MethodDescriptor(instantiatedName2, codeFunction.IsShared, parameters, (Type) type, (ActionKind) 11, (ParameterDomainDefinition[]) null, (CodeBlock[]) null);
    }

    public MethodDescriptor CreateMethodDescriptor(
      CodeEvent codeEvent,
      CodeType container)
    {
      CodeDelegate codeType = codeEvent.Type.CodeType as CodeDelegate;
      Type.Simple type = this.CreateType(codeType.Type);
      Parameter[] parameters = this.CreateParameters(codeType.Parameters, true);
      string str = EnvDTEUtils.ShortenTypeName(container.FullName, this.importedNamespaces, this.containerProject);
      InstantiatedName instantiatedName = new InstantiatedName((Location) Location.None, (InstantiatedName) null, str, (Type[]) null);
      if (type == null || !((Type) type).get_IsVoid() || parameters == null)
        return (MethodDescriptor) null;
      return new MethodDescriptor(new InstantiatedName((Location) Location.None, instantiatedName, codeEvent[], (Type[]) null), codeEvent.IsShared, parameters, (Type) type, (ActionKind) 4, (ParameterDomainDefinition[]) null, (CodeBlock[]) null);
    }

    public Parameter[] CreateParameters(CodeElements parametersCol, bool isForEvent)
    {
      Parameter[] parameterArray = new Parameter[parametersCol.Count];
      int index = 0;
      foreach (CodeElement codeElement in parametersCol)
      {
        parameterArray[index] = this.CreateParameter((CodeParameter2) codeElement, isForEvent);
        if (parameterArray[index] == null)
          return (Parameter[]) null;
        ++index;
      }
      return parameterArray;
    }

    public Parameter CreateParameter(CodeParameter2 param, bool isForEvent)
    {
      Location none = (Location) Location.None;
      ParameterKind parameterKind;
      switch (param.ParameterKind)
      {
        case vsCMParameterKind.vsCMParameterKindNone:
        case vsCMParameterKind.vsCMParameterKindIn:
          parameterKind = (ParameterKind) 2;
          break;
        case vsCMParameterKind.vsCMParameterKindRef:
          if (isForEvent)
            return (Parameter) null;
          parameterKind = (ParameterKind) 8;
          break;
        case vsCMParameterKind.vsCMParameterKindOut:
          if (isForEvent)
            return (Parameter) null;
          parameterKind = (ParameterKind) 4;
          break;
        default:
          return (Parameter) null;
      }
      Type type = (Type) this.CreateType(param.Type);
      if (type != null)
        return new Parameter(none, parameterKind, type, param[]);
      return (Parameter) null;
    }

    public Type.Simple CreateType(CodeTypeRef typeRef)
    {
      switch (typeRef.TypeKind)
      {
        case vsCMTypeRef.vsCMTypeRefArray:
          return (Type.Simple) null;
        case vsCMTypeRef.vsCMTypeRefVoid:
          return this.CreateType("void");
        default:
          return this.CreateType(typeRef.AsString);
      }
    }

    public Type.Simple CreateType(string typeString)
    {
      string str = EnvDTEUtils.ShortenTypeName(typeString, this.importedNamespaces, this.containerProject);
      return new Type.Simple(new InstantiatedName((Location) Location.None, (InstantiatedName) null, str, (Type[]) null));
    }

    public static bool IsFunctionValid(CodeFunction codeFunction)
    {
      if (codeFunction.Access == vsCMAccess.vsCMAccessPublic && !codeFunction.Type.AsString.Contains("[]") && !codeFunction.Attributes.Cast<CodeAttribute2>().Any<CodeAttribute2>((Func<CodeAttribute2, bool>) (attr => attr.FullName == typeof (RuleAttribute).FullName)))
        return !codeFunction.Parameters.Cast<CodeParameter2>().Any<CodeParameter2>((Func<CodeParameter2, bool>) (param => param.Type.AsString.Contains("[]")));
      return false;
    }

    public static bool IsEventValid(CodeEvent codeEvent)
    {
      if (codeEvent.Access != vsCMAccess.vsCMAccessPublic)
        return false;
      CodeDelegate codeType = codeEvent.Type.CodeType as CodeDelegate;
      if (codeType.Type.TypeKind != vsCMTypeRef.vsCMTypeRefVoid)
        return false;
      foreach (CodeParameter2 parameter in codeType.Parameters)
      {
        if (parameter.Type.TypeKind == vsCMTypeRef.vsCMTypeRefArray || parameter.ParameterKind == vsCMParameterKind.vsCMParameterKindOut || (parameter.ParameterKind == vsCMParameterKind.vsCMParameterKindRef || parameter.ParameterKind == vsCMParameterKind.vsCMParameterKindParamArray) || parameter.Type.AsString.Contains("[]"))
          return false;
      }
      return true;
    }
  }
}
