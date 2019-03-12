// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.MsiTokenAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public sealed class MsiTokenAttribute : Attribute
  {
    private string _name;
    private string _value;

    public MsiTokenAttribute(string name, string value)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this._name = name;
      this._value = value;
    }

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public string Value
    {
      get
      {
        return this._value;
      }
    }
  }
}
