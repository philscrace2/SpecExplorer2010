// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideToolboxItemsAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public class ProvideToolboxItemsAttribute : RegistrationAttribute
  {
    private int _version;
    private bool _needsCallbackAfterReset;

    public ProvideToolboxItemsAttribute(int version)
    {
      this._version = version;
      this._needsCallbackAfterReset = false;
    }

    public ProvideToolboxItemsAttribute(int version, bool needsCallbackAfterReset)
    {
      this._version = version;
      this._needsCallbackAfterReset = needsCallbackAfterReset;
    }

    public int Version
    {
      get
      {
        return this._version;
      }
    }

    public bool NeedsCallBackAfterReset
    {
      get
      {
        return this._needsCallbackAfterReset;
      }
      set
      {
        this._needsCallbackAfterReset = value;
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      using (RegistrationAttribute.Key key = context.CreateKey(this.GetPackageRegKeyPath(context.ComponentType.GUID)))
      {
        using (RegistrationAttribute.Key subkey = key.CreateSubkey("Toolbox"))
        {
          subkey.SetValue("Default Items", (object) this.Version);
          string str = string.Empty;
          foreach (ProvideToolboxFormatAttribute customAttribute in context.ComponentType.GetCustomAttributes(typeof (ProvideToolboxFormatAttribute), true))
          {
            if (str.Length == 0)
              str = customAttribute.Format;
            else
              str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, {1}", (object) str, (object) customAttribute.Format);
          }
          if (str.Length > 0)
            subkey.SetValue("Formats", (object) str);
          if (this._needsCallbackAfterReset)
            subkey.SetValue("NeedsCallbackAfterReset", (object) 1);
          context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyToolboxItem, (object) this.Version, (object) str));
        }
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.GetPackageRegKeyPath(context.ComponentType.GUID));
    }
  }
}
