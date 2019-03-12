// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideMenuResourceAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideMenuResourceAttribute : RegistrationAttribute
  {
    private string _resourceID;
    private int _version;

    public ProvideMenuResourceAttribute(short resourceID, int version)
    {
      this._resourceID = resourceID.ToString();
      this._version = version;
    }

    public ProvideMenuResourceAttribute(string resourceID, int version)
    {
      if (string.IsNullOrEmpty(resourceID))
        throw new ArgumentNullException(nameof (resourceID));
      this._resourceID = resourceID;
      this._version = version;
    }

    public string ResourceID
    {
      get
      {
        return this._resourceID;
      }
    }

    public int Version
    {
      get
      {
        return this._version;
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyMenuResource, (object) this.ResourceID, (object) this.Version));
      using (RegistrationAttribute.Key key = context.CreateKey("Menus"))
        key.SetValue(context.ComponentType.GUID.ToString("B"), (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, ", {0}, {1}", (object) this.ResourceID, (object) this.Version));
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveValue("Menus", context.ComponentType.GUID.ToString("B"));
    }
  }
}
