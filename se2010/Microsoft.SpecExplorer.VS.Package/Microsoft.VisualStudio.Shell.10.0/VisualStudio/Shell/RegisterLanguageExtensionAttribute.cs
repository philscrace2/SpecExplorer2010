// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.RegisterLanguageExtensionAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [Obsolete("RegisterLanguageExtensionAttribute has been deprecated. Please use ProvideLanguageExtensionAttribute instead.")]
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class RegisterLanguageExtensionAttribute : RegistrationAttribute
  {
    private Guid languageService;
    private string extension;

    public RegisterLanguageExtensionAttribute(string languageServiceGuid, string extension)
    {
      if (!extension.StartsWith(".", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Attributes_ExtensionNeedsDot, (object) extension));
      this.languageService = new Guid(languageServiceGuid);
      this.extension = extension;
    }

    public RegisterLanguageExtensionAttribute(Type languageService, string extension)
    {
      if (!extension.StartsWith(".", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Attributes_ExtensionNeedsDot, (object) extension));
      this.languageService = languageService.GUID;
      this.extension = extension;
    }

    public string Extension
    {
      get
      {
        return this.extension;
      }
    }

    public Guid LanguageService
    {
      get
      {
        return this.languageService;
      }
    }

    private string ExtensionsRegKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Languages\\File Extensions\\{0}", (object) this.Extension);
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyLanguageExtension, (object) this.Extension, (object) this.LanguageService.ToString("B")));
      using (RegistrationAttribute.Key key = context.CreateKey(this.ExtensionsRegKey))
        key.SetValue(string.Empty, (object) this.LanguageService.ToString("B"));
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.ExtensionsRegKey);
    }
  }
}
