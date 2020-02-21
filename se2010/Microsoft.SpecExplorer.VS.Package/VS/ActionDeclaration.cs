// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ActionDeclaration
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.ActionMachines.Cord;
using System;

namespace Microsoft.SpecExplorer.VS
{
  internal class ActionDeclaration
  {
    internal MethodDescriptor Method { get; private set; }

    internal ActionDeclaration(MethodDescriptor method)
    {
      if (method == null)
        throw new ArgumentNullException(method.ToString());
      this.Method = method;
    }

    internal string Name
    {
      get
      {
        if (this.Method.MethodName == null)
          return (this.Method.ResultType).Flatten();
        return (string) ((InstantiatedName) this.Method.MethodName).Name;
      }
    }

    internal string FullName
    {
      get
      {
        return ((object) this.Method).ToString();
      }
    }

    public override bool Equals(object obj)
    {
      ActionDeclaration actionDeclaration = obj as ActionDeclaration;
      if (actionDeclaration != null)
        return this.Name.Equals(actionDeclaration.Name);
      return false;
    }

    public override int GetHashCode()
    {
      return this.Name.GetHashCode();
    }
  }
}
