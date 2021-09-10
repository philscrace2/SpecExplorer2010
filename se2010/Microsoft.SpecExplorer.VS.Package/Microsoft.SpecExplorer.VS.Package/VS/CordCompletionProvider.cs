// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.CordCompletionProvider
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using EnvDTE80;
using Microsoft.ActionMachines.Cord;
using Microsoft.SpecExplorer.VS.Common;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.Xrt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Microsoft.SpecExplorer.VS
{
  internal class CordCompletionProvider : ComponentBase, ICordCompletionProvider
  {
    private SpecExplorerPackage package;

    internal CordCompletionProvider(SpecExplorerPackage package)
    {      
      if (package == null)
        throw new ArgumentNullException(package.ToString());
      this.package = package;
    }

    private IEnumerable<Completion> GetTypeCompletions(
      CodeElement element,
      IList<Namespace> imports)
    {
      if (element == null)
        throw new ArgumentNullException(element.Name);
      if (imports == null)
        throw new ArgumentNullException(imports.ToString());
      HashSet<Completion> completionSet = new HashSet<Completion>();
      if (element != null)
      {
        switch (element.Kind)
        {
          case vsCMElement.vsCMElementClass:
            CodeClass2 codeClass = element as CodeClass2;
            if (codeClass != null && codeClass.Namespace != null && ((IEnumerable<Namespace>) imports).Any<Namespace>((Func<Namespace, bool>) (import =>
            {
              if (import != null && import.Name != null)
                return ((InstantiatedName) import.Name).Flatten().Equals(codeClass.Namespace.FullName);
              return false;
            })))
            {
              completionSet.Add(new Completion(codeClass.FullName, codeClass.Name, "class " + codeClass.FullName, (ImageSource) CompletionResources.ClassCompletionIcon, "Class"));
              break;
            }
            break;
          case vsCMElement.vsCMElementNamespace:
            CodeNamespace codeNamespace = element as CodeNamespace;
            if (codeNamespace != null && codeNamespace.Members != null)
            {
              IEnumerator enumerator = codeNamespace.Members.GetEnumerator();
              try
              {
                while (enumerator.MoveNext())
                {
                  CodeElement current = (CodeElement) enumerator.Current;
                  if (current != null)
                    completionSet.UnionWith(this.GetTypeCompletions(current, imports));
                }
                break;
              }
              finally
              {
                (enumerator as IDisposable).Dispose();
              }
            }
            else
              break;
          case vsCMElement.vsCMElementInterface:
            CodeInterface2 codeInterface = element as CodeInterface2;
            if (codeInterface != null && codeInterface.Namespace != null && ((IEnumerable<Namespace>) imports).Any<Namespace>((Func<Namespace, bool>) (import =>
            {
              if (import != null && import.Name != null)
                return ((InstantiatedName) import.Name).Flatten().Equals(codeInterface.Namespace.FullName);
              return false;
            })))
            {
              completionSet.Add(new Completion(codeInterface.FullName, codeInterface.Name, "interface " + codeInterface.FullName, (ImageSource) CompletionResources.InterfaceCompletionIcon, "Interface"));
              break;
            }
            break;
        }
      }
      return (IEnumerable<Completion>) completionSet;
    }

    private IEnumerable<Completion> GetTypeCompletions(
      CodeModel codeModel,
      IList<Namespace> imports)
    {
      if (codeModel == null)
        throw new ArgumentNullException(codeModel.ToString());
      if (imports == null)
        throw new ArgumentNullException(imports.ToString());
      HashSet<Completion> completionSet = new HashSet<Completion>();
      foreach (CodeElement codeElement in codeModel.CodeElements)
      {
        if (codeElement != null)
        {
          if (codeElement.Kind == vsCMElement.vsCMElementNamespace)
          {
            CodeNamespace codeNamespace = codeElement as CodeNamespace;
            if (codeNamespace != null)
            {
              completionSet.Add(new Completion(codeNamespace.FullName, codeNamespace.Name, "namespace " + codeNamespace, (ImageSource) CompletionResources.NamespaceCompletionIcon, "Namespace"));
              if (codeNamespace.Members != null)
              {
                foreach (CodeElement member in codeNamespace.Members)
                  completionSet.UnionWith(this.GetTypeCompletions(member, imports));
              }
            }
          }
          else if (codeElement.Kind == vsCMElement.vsCMElementClass)
          {
            CodeClass2 codeClass2 = codeElement as CodeClass2;
            if (codeClass2 != null)
              completionSet.Add(new Completion(codeClass2.FullName, codeClass2.Name, "class " + codeClass2.FullName, (ImageSource) CompletionResources.ClassCompletionIcon, "Class"));
          }
          else if (codeElement.Kind == vsCMElement.vsCMElementInterface)
          {
            CodeInterface2 codeInterface2 = codeElement as CodeInterface2;
            if (codeInterface2 != null)
              completionSet.Add(new Completion(codeInterface2.FullName, codeInterface2.Name, "interface " + codeInterface2.FullName, (ImageSource) CompletionResources.InterfaceCompletionIcon, "Interface"));
          }
        }
      }
      return (IEnumerable<Completion>) completionSet;
    }

    private IEnumerable<Completion> GetPrimitiveTypeCompletions()
    {
      yield return new Completion("sbyte", "sbyte", "struct System.SByte\r\nRepresents an 8-bit signed integer.", (ImageSource) CompletionResources.StructureCompletionIcon, "Structure");
      yield return new Completion("byte", "byte", "struct System.Byte\r\nRepresents an 8-bit unsigned integer.", (ImageSource) CompletionResources.StructureCompletionIcon, "Structure");
      yield return new Completion("short", "short", "struct System.Int16\r\nRepresents a 16-bit signed integer.", (ImageSource) CompletionResources.StructureCompletionIcon, "Structure");
      yield return new Completion("ushort", "ushort", "struct System.UInt16\r\nRepresents a 16-bit unsigned integer.", (ImageSource) CompletionResources.StructureCompletionIcon, "Structure");
      yield return new Completion("int", "int", "struct System.Int32\r\nRepresents a 32-bit signed integer.", (ImageSource) CompletionResources.StructureCompletionIcon, "Structure");
      yield return new Completion("uint", "uint", "struct System.UInt32\r\nRepresents a 32-bit unsigned integer.", (ImageSource) CompletionResources.StructureCompletionIcon, "Structure");
      yield return new Completion("long", "long", "struct System.Int64\r\nRepresents a 64-bit signed integer.", (ImageSource) CompletionResources.StructureCompletionIcon, "Structure");
      yield return new Completion("ulong", "ulong", "struct System.UInt64\r\nRepresents a 64-bit unsigned integer.", (ImageSource) CompletionResources.StructureCompletionIcon, "Structure");
      yield return new Completion("char", "char", "struct System.Char\r\nRepresents a Unicode character.", (ImageSource) CompletionResources.StructureCompletionIcon, "Structure");
      yield return new Completion("float", "float", "struct System.Single\r\nRepresents a single-precision floating-point number.", (ImageSource) CompletionResources.StructureCompletionIcon, "Structure");
      yield return new Completion("double", "double", "struct System.Double\r\nRepresents a double-precision floating-point number.", (ImageSource) CompletionResources.StructureCompletionIcon, "Structure");
      yield return new Completion("bool", "bool", "struct System.Boolean\r\nRepresents a Boolean value.", (ImageSource) CompletionResources.StructureCompletionIcon, "Structure");
      yield return new Completion("object", "object", "class System.Object\r\nSupports all classes in the .NET Framework class hierarchy and provides low-level\r\nservices to derived classes. This is the ultimate base class of all classes\r\nin the .NET Framework; it is the root of the type hierarchy.", (ImageSource) CompletionResources.ClassCompletionIcon, "Class");
      yield return new Completion("string", "string", "class System.String\r\nRepresents text as a series of Unicode characters.", (ImageSource) CompletionResources.ClassCompletionIcon, "Class");
    }

    public ICordDesignTimeManager GetCordDesignTimeManager(
      string scriptFilePath)
    {
      if (string.IsNullOrEmpty(scriptFilePath))
        throw new ArgumentNullException("cannot be null or empty.", scriptFilePath);
      ICordDesignTimeManager designTimeManager = (ICordDesignTimeManager) null;
      Project projectOfFile = ProjectUtils.GetProjectOfFile(scriptFilePath, this.package.DTE);
      if (projectOfFile != null && this.package.CordScopeManager != null)
        designTimeManager = this.package.CordScopeManager.GetCordDesignTimeManager(projectOfFile.UniqueName);
      return designTimeManager;
    }

    public IEnumerable<Completion> GetTypeCompletions(string scriptFilePath)
    {
      if (scriptFilePath == null)
        throw new ArgumentNullException(scriptFilePath.ToString());
      HashSet<Completion> completionSet = new HashSet<Completion>();
      Project projectOfFile = ProjectUtils.GetProjectOfFile(scriptFilePath, this.package.DTE);
      if (projectOfFile != null && this.package.CordScopeManager != null)
      {
        ICordDesignTimeManager designTimeManager = this.package.CordScopeManager.GetCordDesignTimeManager(projectOfFile.UniqueName);
        if (designTimeManager != null)
        {
          CoordinationScript scriptSyntax = designTimeManager.GetScriptSyntax(scriptFilePath);
          if (scriptSyntax != null)
            completionSet.UnionWith(this.GetTypeCompletions(projectOfFile.CodeModel, (IList<Namespace>) scriptSyntax.GlobalNamespaces));
        }
      }
      completionSet.UnionWith(this.GetPrimitiveTypeCompletions());
      return (IEnumerable<Completion>) completionSet;
    }

    public IEnumerable<Completion> GetMemberCompletions(
      string parentText,
      string scriptFilePath,
      bool includeMembersOfType)
    {
      if (string.IsNullOrEmpty(parentText))
        throw new ArgumentException("parentText cannot be null or empty.", parentText.ToString());
      if (scriptFilePath == null)
        throw new ArgumentNullException(scriptFilePath.ToString());
      HashSet<Completion> completionSet = new HashSet<Completion>();
      Project projectOfFile = ProjectUtils.GetProjectOfFile(scriptFilePath, this.package.DTE);
      if (projectOfFile != null && this.package.CordScopeManager != null)
      {
        ICordDesignTimeManager designTimeManager = this.package.CordScopeManager.GetCordDesignTimeManager(projectOfFile.UniqueName);
        if (designTimeManager != null)
        {
          CoordinationScript scriptSyntax = designTimeManager.GetScriptSyntax(scriptFilePath);
          if (scriptSyntax != null && projectOfFile.CodeModel != null)
          {
            CodeType codeType1 = projectOfFile.CodeModel.CodeTypeFromFullName(parentText);
            if (codeType1 != null)
              completionSet.UnionWith(this.GetCodeTypeMemberCompletions(codeType1, includeMembersOfType));
            if (scriptSyntax.GlobalNamespaces != null)
            {
              using (IEnumerator<Namespace> enumerator = ((IEnumerable<Namespace>) scriptSyntax.GlobalNamespaces).GetEnumerator())
              {
                while (((IEnumerator) enumerator).MoveNext())
                {
                  Namespace current = enumerator.Current;
                  if (current != null && current.Name != null)
                  {
                    CodeType codeType2 = projectOfFile.CodeModel.CodeTypeFromFullName(((InstantiatedName) current.Name).Flatten() + "." + parentText);
                    if (codeType2 != null)
                      completionSet.UnionWith(this.GetCodeTypeMemberCompletions(codeType2, includeMembersOfType));
                  }
                }
              }
            }
            if (projectOfFile.CodeModel.CodeElements != null)
            {
              foreach (CodeElement codeElement in projectOfFile.CodeModel.CodeElements)
              {
                CodeNamespace codeNamespace = this.MatchNamespace(parentText, codeElement);
                if (codeNamespace != null)
                {
                  completionSet.UnionWith(this.GetNamespaceMemberCompletions(codeNamespace));
                  break;
                }
              }
            }
          }
        }
      }
      return (IEnumerable<Completion>) completionSet;
    }

    private IEnumerable<Completion> GetCodeTypeMemberCompletions(
      CodeType codeType,
      bool includeMembersOfType)
    {
      HashSet<Completion> completionSet = new HashSet<Completion>();
      if (codeType != null && codeType.Members != null)
      {
        foreach (CodeElement allMember in codeType.Members)
        {
          if (allMember != null)
          {
            switch (allMember.Kind)
            {
              case vsCMElement.vsCMElementClass:
                CodeClass2 codeClass2 = allMember as CodeClass2;
                if (codeClass2 != null)
                {
                  completionSet.Add(new Completion(codeClass2.FullName, codeClass2.Name, "class " + codeClass2.FullName, (ImageSource) CompletionResources.ClassCompletionIcon, "Class"));
                  continue;
                }
                continue;
              case vsCMElement.vsCMElementFunction:
                CodeFunction2 codeFunction2 = allMember as CodeFunction2;
                if (codeFunction2 != null && includeMembersOfType)
                {
                  completionSet.Add(new Completion(codeFunction2.FullName, codeFunction2.Name, codeFunction2.FullName, (ImageSource) CompletionResources.MethodCompletionIcon, "Method"));
                  continue;
                }
                continue;
              case vsCMElement.vsCMElementEvent:
                CodeEvent codeEvent = allMember as CodeEvent;
                if (codeEvent != null && includeMembersOfType)
                {
                  completionSet.Add(new Completion(codeEvent.FullName, codeEvent.Name, codeEvent.FullName, (ImageSource) CompletionResources.EventCompletionIcon, "Event"));
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
      }
      return (IEnumerable<Completion>) completionSet;
    }

    private CodeNamespace MatchNamespace(string namespaceName, CodeElement element)
    {
      if (element != null && element.Kind == vsCMElement.vsCMElementNamespace)
      {
        CodeNamespace codeNamespace1 = element as CodeNamespace;
        if (codeNamespace1 != null)
        {
          if (codeNamespace1.FullName.Equals(namespaceName))
            return codeNamespace1;
          if (codeNamespace1.Members != null)
          {
            foreach (CodeElement member in codeNamespace1.Members)
            {
              CodeNamespace codeNamespace2 = this.MatchNamespace(namespaceName, member);
              if (codeNamespace2 != null)
                return codeNamespace2;
            }
          }
        }
      }
      return (CodeNamespace) null;
    }

    private IEnumerable<Completion> GetNamespaceMemberCompletions(
      CodeNamespace codeNamespace)
    {
      HashSet<Completion> completionSet = new HashSet<Completion>();
      if (codeNamespace != null && codeNamespace.Members != null)
      {
        foreach (CodeElement member in codeNamespace.Members)
        {
          if (member != null)
          {
            switch (member.Kind)
            {
              case vsCMElement.vsCMElementClass:
                CodeClass2 codeClass2 = member as CodeClass2;
                if (codeClass2 != null)
                {
                  completionSet.Add(new Completion(codeClass2.FullName, codeClass2.Name, "class " + codeClass2.FullName, (ImageSource) CompletionResources.ClassCompletionIcon, "Class"));
                  continue;
                }
                continue;
              case vsCMElement.vsCMElementNamespace:
                CodeNamespace codeNamespace1 = member as CodeNamespace;
                if (codeNamespace1 != null)
                {
                  completionSet.Add(new Completion(codeNamespace1.FullName, codeNamespace1.Name, "namespace " + codeNamespace1.FullName, (ImageSource) CompletionResources.NamespaceCompletionIcon, "Namespace"));
                  continue;
                }
                continue;
              case vsCMElement.vsCMElementInterface:
                CodeInterface2 codeInterface2 = member as CodeInterface2;
                if (codeInterface2 != null)
                {
                  completionSet.Add(new Completion(codeInterface2.FullName, codeInterface2.Name, "interface " + codeInterface2.FullName, (ImageSource) CompletionResources.InterfaceCompletionIcon, "Interface"));
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
      }
      return (IEnumerable<Completion>) completionSet;
    }

    public IEnumerable<Completion> GetBehaviorCompletions(
      MachineDefinition machine,
      string scriptFilePath)
    {
      if (machine == null)
        throw new ArgumentNullException(machine.Name);
      if (scriptFilePath == null)
        throw new ArgumentNullException(scriptFilePath);
      HashSet<Completion> completionSet = new HashSet<Completion>();
      Project projectOfFile = ProjectUtils.GetProjectOfFile(scriptFilePath, this.package.DTE);
      if (projectOfFile != null && this.package.CordScopeManager != null)
      {
        ICordDesignTimeManager designTimeManager = this.package.CordScopeManager.GetCordDesignTimeManager(projectOfFile.UniqueName);
        if (designTimeManager != null)
        {
          CoordinationScript scriptSyntax = designTimeManager.GetScriptSyntax(scriptFilePath);
          if (scriptSyntax != null)
          {
            completionSet.UnionWith(CordCompletionProvider.GetMachineCompletions(machine, scriptSyntax));
            completionSet.UnionWith(CordCompletionProvider.GetActionCompletions(machine, projectOfFile, scriptSyntax));
          }
        }
      }
      return (IEnumerable<Completion>) completionSet;
    }

    private static IEnumerable<Completion> GetActionCompletions(
      MachineDefinition machine,
      Project project,
      CoordinationScript ast)
    {
      HashSet<Completion> completionSet = new HashSet<Completion>();
      if (machine != null && machine.Vocabularies != null && ast != null)
      {
        List<ConfigClause.IncludeConfig> includeConfigList = new List<ConfigClause.IncludeConfig>();
        foreach (ConfigReference vocabulary in (ConfigReference[]) machine.Vocabularies)
          includeConfigList.Add(new ConfigClause.IncludeConfig(new ConfigReference(vocabulary.Name, ((SyntaxElement) machine).Location)));
        Config config = new Config((Documentary) null, ((SyntaxElement) machine).Location, "", (IEnumerable<ConfigClause>) includeConfigList);
        VocabularyVisitor vocabularyVisitor = new VocabularyVisitor(ast, (IList<Namespace>) ast.GlobalNamespaces, project);
        ((SyntaxVisitor) vocabularyVisitor).VisitVocabulary(config);
        foreach (ActionDeclaration actionDeclaration in vocabularyVisitor.ImportActions.Except<ActionDeclaration>(vocabularyVisitor.ExcludedActions))
          completionSet.Add(new Completion(actionDeclaration.Name, actionDeclaration.Name, "action " + actionDeclaration.FullName, (ImageSource) CompletionResources.MethodCompletionIcon, "Method"));
      }
      return (IEnumerable<Completion>) completionSet;
    }

    public IEnumerable<Completion> GetMachineCompletions(
      MachineDefinition machine,
      string scriptFilePath)
    {
      if (machine == null)
        throw new ArgumentNullException(machine.Name);
      if (scriptFilePath == null)
        throw new ArgumentNullException(scriptFilePath);
      HashSet<Completion> completionSet = new HashSet<Completion>();
      Project projectOfFile = ProjectUtils.GetProjectOfFile(scriptFilePath, this.package.DTE);
      if (projectOfFile != null && this.package.CordScopeManager != null)
      {
        ICordDesignTimeManager designTimeManager = this.package.CordScopeManager.GetCordDesignTimeManager(projectOfFile.UniqueName);
        if (designTimeManager != null)
        {
          CoordinationScript scriptSyntax = designTimeManager.GetScriptSyntax(scriptFilePath);
          if (scriptSyntax != null)
            completionSet.UnionWith(CordCompletionProvider.GetMachineCompletions(machine, scriptSyntax));
        }
      }
      return (IEnumerable<Completion>) completionSet;
    }

    private static IEnumerable<Completion> GetMachineCompletions(
      MachineDefinition machine,
      CoordinationScript ast)
    {
      HashSet<Completion> completionSet = new HashSet<Completion>();
      if (ast != null && ast.Machines != null)
      {
        using (IEnumerator<MachineDefinition> enumerator = ((IEnumerable<MachineDefinition>) ast.Machines).GetEnumerator())
        {
          while (((IEnumerator) enumerator).MoveNext())
          {
            MachineDefinition current = enumerator.Current;
            if (current != null && (string) current.Name != (string) machine.Name)
              completionSet.Add(new Completion((string) current.Name, (string) current.Name, "machine " + (string) current.Name, (ImageSource) CompletionResources.ClassCompletionIcon, "Class"));
          }
        }
      }
      return (IEnumerable<Completion>) completionSet;
    }

    public void NavigateTo(string fileName, int line, int column)
    {
      this.package.NavigateTo(fileName, line, column);
    }
  }
}
