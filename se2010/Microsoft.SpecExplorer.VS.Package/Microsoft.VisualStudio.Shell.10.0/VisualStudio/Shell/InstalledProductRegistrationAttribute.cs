// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.InstalledProductRegistrationAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public sealed class InstalledProductRegistrationAttribute : RegistrationAttribute
  {
    private string _productName;
    private string _name;
    private string _productId;
    private string _productDetails;
    private string _iconResourceId;
    private bool _useInterface;
    private bool _usePackage;
    private bool _useVsProductId;

    public InstalledProductRegistrationAttribute(
      string productName,
      string productDetails,
      string productId)
      : this(productName, productDetails, productId, false)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public InstalledProductRegistrationAttribute(
      string productName,
      string productDetails,
      string productId,
      bool useVsProductId)
    {
      this._useVsProductId = useVsProductId;
      this.Initialize(productName, productDetails, productId);
    }

    [Obsolete("This InstalledProductRegistrationAttribute constructor has been deprecated. Please use other constructor instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Browsable(false)]
    public InstalledProductRegistrationAttribute(
      bool useInterface,
      string productName,
      string productDetails,
      string productId)
    {
      this._useInterface = useInterface;
      if (this._useInterface)
        this._usePackage = true;
      else
        this.Initialize(productName, productDetails, productId);
    }

    private void Initialize(string productName, string productDetails, string productId)
    {
      if (this.UseInterface)
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_ErrorIncompatibleParametersValues, (object) "useInterface", (object) nameof (productName)));
      if (productName == null || productName.Trim().Length == 0)
        throw new ArgumentNullException(nameof (productName));
      productName = productName.Trim();
      if (productDetails != null)
        productDetails = productDetails.Trim();
      if (!this.UseVsProductId)
      {
        if (productId == null || productId.Trim().Length == 0)
          throw new ArgumentNullException(nameof (productId));
        productId = productId.Trim();
      }
      else if (productId != null)
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_ErrorIncompatibleParametersValues, (object) nameof (productId), (object) "useVsProductId"));
      this._productName = productName;
      this._productDetails = productDetails;
      this._productId = productId;
      if (!string.IsNullOrEmpty(this.ProductDetails) && (this.ProductNameResourceID != 0 && this.ProductDetailsResourceID == 0 || this.ProductNameResourceID == 0 && this.ProductDetailsResourceID != 0))
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_ErrorIncompatibleParametersTypes, (object) nameof (productName), (object) nameof (productDetails)));
      this._usePackage = this.ProductNameResourceID != 0;
    }

    public int ProductNameResourceID
    {
      get
      {
        if (this._productName == null || this._productName.Length < 2 || (this._productName[0] != '#' || !char.IsDigit(this._productName[1])))
          return 0;
        return int.Parse(this._productName.Substring(1), (IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    public int ProductDetailsResourceID
    {
      get
      {
        if (this._productDetails == null || this._productDetails.Length < 2 || (this._productDetails[0] != '#' || !char.IsDigit(this._productDetails[1])))
          return 0;
        return int.Parse(this._productDetails.Substring(1), (IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    public int IconResourceID
    {
      get
      {
        if (string.IsNullOrEmpty(this._iconResourceId) || this._iconResourceId.Length < 2)
          return 0;
        return int.Parse(this._iconResourceId.Substring(1), (IFormatProvider) CultureInfo.InvariantCulture);
      }
      set
      {
        this._iconResourceId = "#" + value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    public string ProductId
    {
      get
      {
        return this._productId;
      }
    }

    public string ProductName
    {
      get
      {
        return this._productName;
      }
    }

    public string LanguageIndependentName
    {
      get
      {
        return this._name;
      }
      set
      {
        this._name = value;
      }
    }

    private string GetNonEmptyName(RegistrationAttribute.RegistrationContext context)
    {
      string str = this.LanguageIndependentName;
      if (str != null)
        str = str.Trim();
      if (string.IsNullOrEmpty(str))
        str = context.ComponentType.Name;
      return str;
    }

    public string ProductDetails
    {
      get
      {
        return this._productDetails;
      }
    }

    public bool UseInterface
    {
      get
      {
        return this._useInterface;
      }
    }

    public bool UsePackage
    {
      get
      {
        return this._usePackage;
      }
    }

    public bool UseVsProductId
    {
      get
      {
        return this._useVsProductId;
      }
    }

    private string RegKeyName(RegistrationAttribute.RegistrationContext context)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "InstalledProducts\\{0}", (object) this.GetNonEmptyName(context));
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      if (this.UseInterface)
        context.Log.WriteLine(Resources.Reg_NotifyInstalledProductInterface);
      else
        context.Log.WriteLine(Resources.Reg_NotifyInstalledProduct, (object) this.GetNonEmptyName(context), (object) (this.ProductId ?? this.ProductName));
      using (RegistrationAttribute.Key key = context.CreateKey(this.RegKeyName(context)))
      {
        if (this.UsePackage)
          key.SetValue("Package", (object) context.ComponentType.GUID.ToString("B"));
        if (!string.IsNullOrEmpty(this._name))
          key.SetValue("UseRegNameAsSplashName", (object) 1);
        if (this.UseInterface)
        {
          key.SetValue("UseInterface", (object) 1);
        }
        else
        {
          key.SetValue("", (object) this.ProductName);
          if (this.UseVsProductId)
            key.SetValue("UseVsProductID", (object) 1);
          else
            key.SetValue("PID", (object) this.ProductId);
          if (!string.IsNullOrEmpty(this.ProductDetails))
            key.SetValue("ProductDetails", (object) this.ProductDetails);
          if (!this.UsePackage || string.IsNullOrEmpty(this._iconResourceId))
            return;
          key.SetValue("LogoID", (object) this._iconResourceId);
        }
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.RegKeyName(context));
    }
  }
}
