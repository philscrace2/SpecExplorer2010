﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideAssemblyFilterAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideAssemblyFilterAttribute : Attribute
  {
    private string _assemblyFilter;

    public ProvideAssemblyFilterAttribute(string assemblyFilter)
    {
      if (assemblyFilter == null)
        throw new ArgumentNullException(nameof (assemblyFilter));
      if (assemblyFilter.Length == 0)
        throw new ArgumentException(Resources.General_ExpectedNonEmptyString, nameof (assemblyFilter));
      this._assemblyFilter = assemblyFilter;
    }

    public string AssemblyFilter
    {
      get
      {
        return this._assemblyFilter;
      }
    }
  }
}