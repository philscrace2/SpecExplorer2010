// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.MemberInfo
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.Xrt;
using System;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer
{
  [Serializable]
  public class MemberInfo
  {
    public string Name { get; private set; }

    public MemberKind Kind { get; private set; }

    public string TypeName { get; private set; }

    public IList<string> ParameterTypes { get; private set; }

    public MemberInfo(string typeName, MemberKind kind, string name)
      : this(typeName, kind, name, (ILocal[]) null)
    {
    }

    public MemberInfo(string typeName, MemberKind kind, string name, ILocal[] parameters)
    {
      this.TypeName = typeName;
      this.Kind = kind;
      this.Name = name;
      if (kind != MemberKind.Method)
        return;
      if (parameters == null)
        throw new ArgumentNullException(nameof (parameters));
      this.BuildParameters(parameters);
    }

    private void BuildParameters(ILocal[] parameters)
    {
      List<string> stringList = new List<string>();
      foreach (ILocal parameter in parameters)
      {
        if (parameter.Name != "this")
        {
          IType type = parameter.Type;
          if (type.IsAddressType || type.IsArrayType)
            type = type.ElementType;
          stringList.Add(type.FullName);
        }
      }
      this.ParameterTypes = (IList<string>) stringList;
    }
  }
}
