// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.RegisterExtenderAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  [Obsolete("RegisterExtenderAttribute has been deprecated. Please use ProvideExtenderAttribute instead.")]
  public sealed class RegisterExtenderAttribute : RegistrationAttribute
  {
    private Guid CATID = Guid.Empty;
    private Guid extender = Guid.Empty;
    private string name;

    public RegisterExtenderAttribute(
      string extendeeCatId,
      string extenderGuid,
      string extenderName)
    {
      this.CATID = new Guid(extendeeCatId);
      this.extender = new Guid(extenderGuid);
      this.name = extenderName;
    }

    public Guid ExtendeeCatId
    {
      get
      {
        return this.CATID;
      }
    }

    public Guid Extender
    {
      get
      {
        return this.extender;
      }
    }

    public string ExtenderName
    {
      get
      {
        return this.name;
      }
    }

    private string RegKeyName
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Extenders\\{0}\\{1}", (object) this.CATID.ToString("B"), (object) this.name);
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyExtender, (object) this.name, (object) this.CATID.ToString("B")));
      using (RegistrationAttribute.Key key = context.CreateKey(this.RegKeyName))
        key.SetValue(string.Empty, (object) this.extender.ToString("B"));
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.RegKeyName);
    }
  }
}
