// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.CodeElementTypeResolver
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
using Type = Microsoft.ActionMachines.Cord.Type;



namespace Microsoft.SpecExplorer.VS
{
  internal class CodeElementTypeResolver
  {
    private HashSet<CodeElement> candidates = new HashSet<CodeElement>();
    private IList<Namespace> imports;
    private Project project;

    internal CodeElementTypeResolver(IList<Namespace> imports, Project project)
    {
      if (project == null)
        throw new ArgumentNullException(project.Name);
      this.project = project;
      this.imports = imports;
    }

    internal CodeElement ResolveTypeUnique(Type type)
    {
      if (type == null)
        throw new ArgumentNullException(type.ToString());
      IList<CodeElement> source = this.ResolveType(type);
      if (source != null && source.Count == 1)
        return source.First<CodeElement>();
      return (CodeElement) null;
    }

    internal IList<CodeElement> ResolveType(Type type)
    {
      if (type == null)
        throw new ArgumentNullException(type.ToString());
      this.candidates.Clear();
      foreach (CodeElement codeElement in this.project.CodeModel.CodeElements)
        this.ResolveType(type, codeElement);
      return (IList<CodeElement>) this.candidates.ToList<CodeElement>();
    }

    private void ResolveType(Type type, CodeElement element)
    {
      if (element == null)
        return;
      switch (element.Kind)
      {
        case vsCMElement.vsCMElementClass:
          CodeClass2 codeClass = element as CodeClass2;
          if (codeClass != null && this.IsTypeMatch(type, codeClass.FullName))
            this.candidates.Add(element);
          using (IEnumerator<CodeElement> enumerator = codeClass.GetAllMembers().GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              CodeElement current = enumerator.Current;
              this.ResolveType(type, current);
            }
            break;
          }
        case vsCMElement.vsCMElementNamespace:
          CodeNamespace codeNamespace = element as CodeNamespace;
          if (codeNamespace == null || codeNamespace.Members == null)
            break;
          IEnumerator enumerator1 = codeNamespace.Members.GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
            {
              CodeElement current = (CodeElement) enumerator1.Current;
              this.ResolveType(type, current);
            }
            break;
          }
          finally
          {
            (enumerator1 as IDisposable).Dispose();
          }
        case vsCMElement.vsCMElementInterface:
          CodeInterface2 codeInterface2 = element as CodeInterface2;
          if (codeInterface2 == null || !this.IsTypeMatch(type, codeInterface2.FullName))
            break;
          this.candidates.Add(element);
          break;
      }
    }

    private bool IsTypeMatch(Type type, string fullName)
    {
      string typeName = type.Flatten();
      return fullName == typeName || ((IEnumerable<Namespace>) this.imports).Any<Namespace>((Func<Namespace, bool>) (import =>
      {
        if (import != null && import.Name != null)
          return fullName.Equals(((InstantiatedName) import.Name).Flatten() + "." + typeName);
        return false;
      }));
    }
  }
}
