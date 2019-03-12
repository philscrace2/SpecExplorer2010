// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.ProvideUIProviderAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell;
using System;
using System.Globalization;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideUIProviderAttribute : RegistrationAttribute
  {
    private string _providerName;
    private Guid _providerGuid;
    private Guid _providerPackageGuid;

    public ProvideUIProviderAttribute(
      string providerGuid,
      string providerName,
      string providerPackage)
    {
      this._providerName = providerName ?? "";
      this._providerGuid = new Guid(providerGuid);
      this._providerPackageGuid = new Guid(providerPackage);
    }

    public string ProviderName
    {
      get
      {
        return this._providerName;
      }
    }

    public Guid ProviderGuid
    {
      get
      {
        return this._providerGuid;
      }
    }

    public Guid ProviderPackage
    {
      get
      {
        return this._providerPackageGuid;
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Log_Registering, (object) "UIProvider", (object) this.ProviderGuid.ToString("B"), (object) this.ProviderName));
      using (RegistrationAttribute.Key key = context.CreateKey("UIProviders"))
      {
        using (RegistrationAttribute.Key subkey = key.CreateSubkey(this.ProviderGuid.ToString("B")))
        {
          subkey.SetValue("", (object) this.ProviderName);
          subkey.SetValue("Package", (object) this.ProviderPackage.ToString("B"));
        }
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Log_Unregistering, (object) "UIProvider", (object) this.ProviderGuid.ToString("B")));
      context.RemoveKey("UIProviders\\" + this.ProviderGuid.ToString("B"));
    }
  }
}
