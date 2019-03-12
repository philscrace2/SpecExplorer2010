// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.StyleKey`1
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Reflection;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI
{
  public sealed class StyleKey<T> : ResourceKey
  {
    private Assembly assembly;

    public override Assembly Assembly
    {
      get
      {
        Assembly assembly = this.assembly;
        if ((object) assembly != null)
          return assembly;
        return this.assembly = typeof (T).Assembly;
      }
    }
  }
}
