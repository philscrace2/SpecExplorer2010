// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.VocabularyVisitor
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using EnvDTE80;
using Microsoft.ActionMachines.Cord;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer.VS
{
  internal class VocabularyVisitor : SyntaxVisitor
  {
    private Dictionary<Config, VisitingState> visitingState;
    private Stack<Config> visitingConfigs;
    private ActionDeclarationBuilder actionBuilder;
    private IList<Namespace> imports;
    private Project project;
    private CoordinationScript ast;

    internal VocabularyVisitor(CoordinationScript ast, IList<Namespace> imports, Project project)
    {
      this.\u002Ector();
      this.ast = ast;
      this.imports = imports;
      this.project = project;
      this.ImportActions = Enumerable.Empty<ActionDeclaration>();
      this.ExcludedActions = Enumerable.Empty<ActionDeclaration>();
    }

    public IEnumerable<ActionDeclaration> ImportActions { get; private set; }

    public IEnumerable<ActionDeclaration> ExcludedActions { get; private set; }

    public virtual void VisitVocabulary(Config voc)
    {
      if (this.visitingState.ContainsKey(voc))
      {
        switch ((int) this.visitingState[voc])
        {
          case 0:
            break;
          case 1:
            return;
          case 2:
            return;
          default:
            return;
        }
      }
      this.visitingState[voc] = (VisitingState) 1;
      this.visitingConfigs.Push(voc);
      HashSet<ActionDeclaration> actionDeclarationSet = new HashSet<ActionDeclaration>();
      if (voc.Clauses != null)
      {
        using (IEnumerator<ConfigClause.IncludeConfig> enumerator = ((IEnumerable) voc.Clauses).OfType<ConfigClause.IncludeConfig>().Where<ConfigClause.IncludeConfig>((Func<ConfigClause.IncludeConfig, bool>) (c => c.Vocabulary != null)).GetEnumerator())
        {
          while (((IEnumerator) enumerator).MoveNext())
          {
            ConfigClause.IncludeConfig baseVoc = enumerator.Current;
            Config config = ((IEnumerable<Config>) this.ast.Configs).FirstOrDefault<Config>((Func<Config, bool>) (c => (string) c.Name == ((ConfigReference) baseVoc.Vocabulary).get_Name()));
            if (config != null)
            {
              ((SyntaxElement) config).Accept((SyntaxVisitor) this);
              actionDeclarationSet.UnionWith(this.ImportActions);
              actionDeclarationSet.ExceptWith(this.ExcludedActions);
              this.ImportActions = Enumerable.Empty<ActionDeclaration>();
              this.ExcludedActions = Enumerable.Empty<ActionDeclaration>();
            }
          }
        }
      }
      ActionDeclarationBuilder actionBuilder = this.actionBuilder;
      this.actionBuilder = new ActionDeclarationBuilder();
      ((SyntaxElement) voc).AcceptOnChildren((SyntaxVisitor) this);
      actionDeclarationSet.UnionWith(this.actionBuilder.ImportActions);
      this.ImportActions = (IEnumerable<ActionDeclaration>) actionDeclarationSet;
      this.ExcludedActions = this.actionBuilder.ExcludedActions;
      this.visitingState[voc] = (VisitingState) 2;
      this.visitingConfigs.Pop();
      this.actionBuilder = actionBuilder;
    }

    public virtual void VisitImportAllFromScope(ConfigClause.ImportAllFromScope ua)
    {
      if (ua == null || ua.FromType == null)
        return;
      CodeElement codeElement = new CodeElementTypeResolver(this.imports, this.project).ResolveTypeUnique((Type) ua.FromType);
      if (codeElement == null)
        return;
      CordSyntaxElementBuilder syntaxElementBuilder = new CordSyntaxElementBuilder((SpecExplorerPackage) null, (string) null, (string) null);
      if (codeElement.Kind == vsCMElement.vsCMElementClass)
      {
        CodeClass2 codeClass = codeElement as CodeClass2;
        if (codeClass == null)
          return;
        foreach (CodeElement allMember in codeClass.GetAllMembers())
        {
          if (allMember != null)
          {
            ConfigClause.ImportMethod actionImport = syntaxElementBuilder.CreateActionImport(allMember, this.project.CodeModel.CodeTypeFromFullName(codeClass.FullName));
            if (actionImport != null && actionImport.Method != null)
              this.actionBuilder.AddAction((MethodDescriptor) actionImport.Method);
          }
        }
      }
      else
      {
        if (codeElement.Kind != vsCMElement.vsCMElementInterface)
          return;
        CodeInterface2 codeInterface2 = codeElement as CodeInterface2;
        if (codeInterface2 == null)
          return;
        foreach (CodeElement member in codeInterface2.Members)
        {
          if (member != null)
          {
            ConfigClause.ImportMethod actionImport = syntaxElementBuilder.CreateActionImport(member, this.project.CodeModel.CodeTypeFromFullName(codeInterface2.FullName));
            if (actionImport != null && actionImport.Method != null)
              this.actionBuilder.AddAction((MethodDescriptor) actionImport.Method);
          }
        }
      }
    }

    public virtual void VisitImportEvent(ConfigClause.ImportMethod um)
    {
      if (um == null || um.Method == null)
        return;
      this.actionBuilder.AddAction((MethodDescriptor) um.Method);
    }

    public virtual void VisitImportMethod(ConfigClause.ImportMethod um)
    {
      if (um == null || um.Method == null)
        return;
      this.actionBuilder.AddAction((MethodDescriptor) um.Method);
    }

    public virtual void VisitExcludeMethod(ConfigClause.ExcludeMethod um)
    {
      if (um == null || um.Method == null)
        return;
      this.actionBuilder.ExcludeAction((MethodDescriptor) um.Method);
    }

    public virtual void VisitDeclareEvent(ConfigClause.DeclareMethod dm)
    {
      if (dm == null || dm.Method == null)
        return;
      this.actionBuilder.AddAction((MethodDescriptor) dm.Method);
    }

    public virtual void VisitDeclareMethod(ConfigClause.DeclareMethod dm)
    {
      if (dm == null || dm.Method == null)
        return;
      this.actionBuilder.AddAction((MethodDescriptor) dm.Method);
    }
  }
}
