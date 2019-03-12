// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.RegisterAutoLoadAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [Obsolete("RegisterAutoLoadAttribute has been deprecated. Please use ProvideAutoLoadAttribute instead.")]
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class RegisterAutoLoadAttribute : RegistrationAttribute
  {
    private Guid loadGuid = Guid.Empty;

    public RegisterAutoLoadAttribute(string cmdUiContextGuid)
    {
      this.loadGuid = new Guid(cmdUiContextGuid);
    }

    public Guid LoadGuid
    {
      get
      {
        return this.loadGuid;
      }
    }

    private string RegKeyName
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AutoLoadPackages\\{0}", (object) this.loadGuid.ToString("B"));
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyAutoLoad, (object) this.loadGuid.ToString("B")));
      using (RegistrationAttribute.Key key = context.CreateKey(this.RegKeyName))
        key.SetValue(context.ComponentType.GUID.ToString("B"), (object) 0);
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveValue(this.RegKeyName, context.ComponentType.GUID.ToString("B"));
    }
  }
}
