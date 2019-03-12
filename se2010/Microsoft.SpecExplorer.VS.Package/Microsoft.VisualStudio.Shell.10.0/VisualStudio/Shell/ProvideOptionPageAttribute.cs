// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideOptionPageAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideOptionPageAttribute : ProvideOptionDialogPageAttribute
  {
    private string _categoryName;
    private string _pageName;
    private short _categoryResourceID;
    private bool _supportsAutomation;
    private bool _noShowAllView;
    private bool _supportsProfiles;
    private ProfileMigrationType _migrationType;

    public ProvideOptionPageAttribute(
      Type pageType,
      string categoryName,
      string pageName,
      short categoryResourceID,
      short pageNameResourceID,
      bool supportsAutomation)
      : base(pageType, "#" + pageNameResourceID.ToString())
    {
      if (categoryName == null)
        throw new ArgumentNullException(nameof (categoryName));
      if (pageName == null)
        throw new ArgumentNullException(nameof (pageName));
      this._categoryName = categoryName;
      this._pageName = pageName;
      this._categoryResourceID = categoryResourceID;
      this._supportsAutomation = supportsAutomation;
    }

    public bool NoShowAllView
    {
      get
      {
        return this._noShowAllView;
      }
      set
      {
        this._noShowAllView = value;
      }
    }

    public override object TypeId
    {
      get
      {
        return (object) this;
      }
    }

    public string CategoryName
    {
      get
      {
        return this._categoryName;
      }
    }

    public short CategoryResourceID
    {
      get
      {
        return this._categoryResourceID;
      }
    }

    public string PageName
    {
      get
      {
        return this._pageName;
      }
    }

    public bool SupportsAutomation
    {
      get
      {
        return this._supportsAutomation;
      }
    }

    public bool SupportsProfiles
    {
      get
      {
        return this._supportsProfiles;
      }
      set
      {
        this._supportsProfiles = value;
      }
    }

    public ProfileMigrationType ProfileMigrationType
    {
      get
      {
        return this._migrationType;
      }
      set
      {
        this._migrationType = value;
      }
    }

    private string ToolsOptionsPagesRegKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ToolsOptionsPages\\{0}", (object) this.CategoryName);
      }
    }

    private string AutomationCategoryRegKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AutomationProperties\\{0}", (object) this.CategoryName);
      }
    }

    private string AutomationRegKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) this.AutomationCategoryRegKey, (object) this.PageName);
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyOptionPage, (object) this.CategoryName, (object) this.PageName));
      using (RegistrationAttribute.Key key = context.CreateKey(this.ToolsOptionsPagesRegKey))
      {
        key.SetValue(string.Empty, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "#{0}", (object) this.CategoryResourceID));
        key.SetValue("Package", (object) context.ComponentType.GUID.ToString("B"));
        using (RegistrationAttribute.Key subkey = key.CreateSubkey(this.PageName))
        {
          subkey.SetValue(string.Empty, (object) this.PageNameResourceId);
          subkey.SetValue("Package", (object) context.ComponentType.GUID.ToString("B"));
          subkey.SetValue("Page", (object) this.PageType.GUID.ToString("B"));
          if (this.NoShowAllView)
            subkey.SetValue("NoShowAllView", (object) 1);
        }
      }
      if (!this.SupportsAutomation)
        return;
      using (RegistrationAttribute.Key key = context.CreateKey(this.AutomationRegKey))
      {
        key.SetValue("Name", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) this.CategoryName, (object) this.PageName));
        key.SetValue("Package", (object) context.ComponentType.GUID.ToString("B"));
        if (!this.SupportsProfiles)
          return;
        key.SetValue("ProfileSave", (object) 1);
        key.SetValue("VSSettingsMigration", (object) (int) this.ProfileMigrationType);
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.ToolsOptionsPagesRegKey);
      if (!this.SupportsAutomation)
        return;
      context.RemoveKey(this.AutomationRegKey);
      context.RemoveKeyIfEmpty(this.AutomationCategoryRegKey);
    }
  }
}
