// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.RegisterLoadKeyAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  [Obsolete("RegisterLoadKeyAttribute has been deprecated. Please use ProvideLoadKeyAttribute instead.")]
  public sealed class RegisterLoadKeyAttribute : RegistrationAttribute
  {
    private string _minimumEdition;
    private string _productVersion;
    private string _productName;
    private string _companyName;
    private short _resourceId;

    public RegisterLoadKeyAttribute(
      string minimumEdition,
      string productVersion,
      string productName,
      string companyName,
      short resourceId)
    {
      if (minimumEdition == null)
        throw new ArgumentNullException(nameof (minimumEdition));
      if (productVersion == null)
        throw new ArgumentNullException(nameof (productVersion));
      if (productName == null)
        throw new ArgumentNullException(nameof (productName));
      if (companyName == null)
        throw new ArgumentNullException(nameof (companyName));
      this._minimumEdition = minimumEdition;
      this._productVersion = productVersion;
      this._productName = productName;
      this._companyName = companyName;
      this._resourceId = resourceId;
    }

    public string MinimumEdition
    {
      get
      {
        return this._minimumEdition;
      }
    }

    public string ProductVersion
    {
      get
      {
        return this._productVersion;
      }
    }

    public string ProductName
    {
      get
      {
        return this._productName;
      }
    }

    public string CompanyName
    {
      get
      {
        return this._companyName;
      }
    }

    public short ResourceId
    {
      get
      {
        return this._resourceId;
      }
    }

    public string RegKeyName(RegistrationAttribute.RegistrationContext context)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Packages\\{0}", (object) context.ComponentType.GUID.ToString("B"));
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyLoadKey, (object) this.CompanyName, (object) this.ProductName, (object) this.ProductVersion, (object) this.MinimumEdition));
      using (RegistrationAttribute.Key key = context.CreateKey(this.RegKeyName(context)))
      {
        key.SetValue("ID", (object) this.ResourceId);
        key.SetValue("MinEdition", (object) this.MinimumEdition);
        key.SetValue("ProductVersion", (object) this.ProductVersion);
        key.SetValue("ProductName", (object) this.ProductName);
        key.SetValue("CompanyName", (object) this.CompanyName);
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.RegKeyName(context));
    }
  }
}
