// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.ProvideUIDataConverterAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell;
using System;
using System.Globalization;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideUIDataConverterAttribute : RegistrationAttribute
  {
    private string _convertedType;
    private string _converterName;
    private Guid _converterGuid;
    private Guid _converterPackageGuid;

    public ProvideUIDataConverterAttribute(
      string convertedType,
      string converterName,
      string converterGuid,
      string converterPackage)
    {
      if (convertedType == null)
        throw new ArgumentNullException(nameof (convertedType));
      if (convertedType.Length == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_EmptyString, (object) nameof (convertedType)));
      this._convertedType = convertedType;
      this._converterName = converterName ?? "";
      this._converterGuid = new Guid(converterGuid);
      this._converterPackageGuid = new Guid(converterPackage);
    }

    public string ConvertedType
    {
      get
      {
        return this._convertedType;
      }
    }

    public string ConverterName
    {
      get
      {
        return this._converterName;
      }
    }

    public Guid ConverterGuid
    {
      get
      {
        return this._converterGuid;
      }
    }

    public Guid ConverterPackage
    {
      get
      {
        return this._converterPackageGuid;
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Log_Registering, (object) "UIDataConverter", (object) this.ConverterGuid.ToString("B"), (object) this.ConverterName));
      using (RegistrationAttribute.Key key = context.CreateKey("UIDataConverters"))
      {
        using (RegistrationAttribute.Key subkey1 = key.CreateSubkey(this.ConvertedType))
        {
          using (RegistrationAttribute.Key subkey2 = subkey1.CreateSubkey(this.ConverterGuid.ToString("B")))
          {
            subkey2.SetValue("", (object) this.ConverterName);
            subkey2.SetValue("Package", (object) this.ConverterPackage.ToString("B"));
          }
        }
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Log_Unregistering, (object) "UIDataConverter", (object) this.ConverterGuid.ToString("B")));
      context.RemoveKey("UIDataConverters\\" + this.ConvertedType + "\\" + this.ConverterGuid.ToString("B"));
    }
  }
}
