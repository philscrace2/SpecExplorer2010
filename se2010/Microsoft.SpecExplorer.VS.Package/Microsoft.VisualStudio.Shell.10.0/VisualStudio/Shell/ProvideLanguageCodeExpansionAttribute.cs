// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideLanguageCodeExpansionAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell
{
  [ComVisible(false)]
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideLanguageCodeExpansionAttribute : RegistrationAttribute
  {
    private Guid languageServiceGuid;
    private string languageName;
    private string snippetIndexPath;
    private string searchPaths;
    private string forceCreateDirs;
    private string languageIdString;
    private string displayName;
    private bool showRoots;

    public ProvideLanguageCodeExpansionAttribute(
      object languageService,
      string languageName,
      int languageResourceId,
      string languageIdentifier,
      string pathToSnippetIndexFile)
    {
      if ((object) (languageService as Type) != null)
      {
        this.languageServiceGuid = ((Type) languageService).GUID;
      }
      else
      {
        if (!(languageService is string))
          throw new ArgumentException();
        this.languageServiceGuid = new Guid((string) languageService);
      }
      this.languageName = languageName;
      this.snippetIndexPath = pathToSnippetIndexFile;
      this.displayName = languageResourceId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.languageIdString = languageIdentifier;
    }

    public Guid LanguageServiceSid
    {
      get
      {
        return this.languageServiceGuid;
      }
    }

    public string LanguageName
    {
      get
      {
        return this.languageName;
      }
    }

    public bool ShowRoots
    {
      get
      {
        return this.showRoots;
      }
      set
      {
        this.showRoots = value;
      }
    }

    public string SearchPaths
    {
      get
      {
        return this.searchPaths;
      }
      set
      {
        this.searchPaths = value;
      }
    }

    public string ForceCreateDirs
    {
      get
      {
        return this.forceCreateDirs;
      }
      set
      {
        this.forceCreateDirs = value;
      }
    }

    private string LanguageRegistryKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) RegistryPaths.codeExpansion, (object) this.LanguageName);
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyLanguageCodeExpansion, (object) this.LanguageServiceSid.ToString("B")));
      string str = context.ComponentType.GUID.ToString("B");
      using (RegistrationAttribute.Key key = context.CreateKey(this.LanguageRegistryKey))
      {
        key.SetValue(string.Empty, (object) this.LanguageServiceSid.ToString("B"));
        key.SetValue(RegistryPaths.package, (object) str);
        key.SetValue(RegistryPaths.displayName, (object) this.displayName);
        key.SetValue(RegistryPaths.languageStringId, (object) this.languageIdString);
        key.SetValue(RegistryPaths.indexPath, (object) this.snippetIndexPath);
        key.SetValue(RegistryPaths.showRoots, (object) (this.showRoots ? 1 : 0));
        if (!string.IsNullOrEmpty(this.SearchPaths))
        {
          using (RegistrationAttribute.Key subkey = key.CreateSubkey(RegistryPaths.paths))
            subkey.SetValue(this.LanguageName, (object) this.SearchPaths);
        }
        if (string.IsNullOrEmpty(this.ForceCreateDirs))
          return;
        using (RegistrationAttribute.Key subkey = key.CreateSubkey(RegistryPaths.forceCreateDirs))
          subkey.SetValue(this.LanguageName, (object) this.ForceCreateDirs);
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.LanguageRegistryKey);
    }
  }
}
