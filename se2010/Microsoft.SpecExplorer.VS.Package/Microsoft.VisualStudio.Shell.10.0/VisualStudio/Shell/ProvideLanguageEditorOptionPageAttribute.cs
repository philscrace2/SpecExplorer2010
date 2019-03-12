// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideLanguageEditorOptionPageAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideLanguageEditorOptionPageAttribute : ProvideOptionDialogPageAttribute
  {
    private string languageName;
    private string pageName;
    private string category;

    public ProvideLanguageEditorOptionPageAttribute(
      Type pageType,
      string languageName,
      string category,
      string pageName,
      string pageNameResourceId)
      : base(pageType, pageNameResourceId)
    {
      this.languageName = languageName;
      this.pageName = pageName;
      this.category = category;
    }

    public string LanguageName
    {
      get
      {
        return this.languageName;
      }
    }

    public Guid PageGuid
    {
      get
      {
        return this.PageType.GUID;
      }
    }

    private string FullPathToPage
    {
      get
      {
        if (string.IsNullOrEmpty(this.category))
          return this.pageName;
        return string.Format("{0}\\{1}", (object) this.category, (object) this.pageName);
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyLanguageOptionPage, (object) this.LanguageName, (object) this.PageNameResourceId));
      LanguageToolsOptionCreator.CreateRegistryEntries(context, this.LanguageName, this.FullPathToPage, this.PageNameResourceId, this.PageGuid);
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      LanguageToolsOptionCreator.RemoveRegistryEntries(context, this.LanguageName, this.FullPathToPage);
    }
  }
}
