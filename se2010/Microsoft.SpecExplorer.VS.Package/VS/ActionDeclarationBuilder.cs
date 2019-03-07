// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ActionDeclarationBuilder
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.ActionMachines.Cord;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer.VS
{
  internal class ActionDeclarationBuilder
  {
    private HashSet<ActionDeclaration> importActions = new HashSet<ActionDeclaration>();
    private HashSet<ActionDeclaration> excludedActions = new HashSet<ActionDeclaration>();

    internal void AddAction(MethodDescriptor method)
    {
      this.importActions.Add(new ActionDeclaration(method));
    }

    internal void ExcludeAction(MethodDescriptor method)
    {
      this.excludedActions.Add(new ActionDeclaration(method));
    }

    internal IEnumerable<ActionDeclaration> ImportActions
    {
      get
      {
        return (IEnumerable<ActionDeclaration>) this.importActions;
      }
    }

    internal IEnumerable<ActionDeclaration> ExcludedActions
    {
      get
      {
        return (IEnumerable<ActionDeclaration>) this.excludedActions;
      }
    }
  }
}
