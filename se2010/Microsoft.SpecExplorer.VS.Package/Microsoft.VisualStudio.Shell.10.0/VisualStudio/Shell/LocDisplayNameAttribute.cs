// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.LocDisplayNameAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.ComponentModel;
using System.Resources;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class LocDisplayNameAttribute : DisplayNameAttribute
  {
    private string name;

    public LocDisplayNameAttribute(string name)
    {
      this.name = name;
    }

    public override string DisplayName
    {
      get
      {
        string str = (string) null;
        try
        {
          str = Microsoft.VisualStudio.Shell.Resources.ResourceManager.GetString(this.name, Microsoft.VisualStudio.Shell.Resources.Culture);
        }
        catch (MissingManifestResourceException ex)
        {
        }
        if (str == null)
          str = this.name;
        return str;
      }
    }
  }
}
