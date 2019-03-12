// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideLanguageEditorToolsOptionCategoryAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideLanguageEditorToolsOptionCategoryAttribute : RegistrationAttribute
  {
    private string languageName;
    private string categoryName;
    private string categoryResourceId;

    public ProvideLanguageEditorToolsOptionCategoryAttribute(
      string languageName,
      string categoryName,
      string categoryResourceId)
    {
      this.languageName = languageName;
      this.categoryName = categoryName;
      this.categoryResourceId = categoryResourceId;
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyLanguageOptionCategory, (object) this.languageName, (object) this.categoryName));
      LanguageToolsOptionCreator.CreateRegistryEntries(context, this.languageName, this.categoryName, this.categoryResourceId, Guid.Empty);
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      LanguageToolsOptionCreator.RemoveRegistryEntries(context, this.languageName, this.categoryName);
    }
  }
}
