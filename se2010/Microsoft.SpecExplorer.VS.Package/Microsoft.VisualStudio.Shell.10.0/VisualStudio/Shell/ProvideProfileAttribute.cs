// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideProfileAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideProfileAttribute : RegistrationAttribute
  {
    private Type _objectType;
    private string _groupName;
    private string _categoryName;
    private string _objectName;
    private string _alternateParent;
    private string _resourcePackageGuid;
    private short _groupResourceID;
    private short _categoryResourceID;
    private short _objectNameResourceID;
    private short _descriptionResourceID;
    private bool _isToolsOptionPage;
    private ProfileMigrationType _migrationType;

    public ProvideProfileAttribute(
      Type objectType,
      string categoryName,
      string objectName,
      short categoryResourceID,
      short objectNameResourceID,
      bool isToolsOptionPage)
    {
      if (objectType == (Type) null)
        throw new ArgumentNullException(nameof (objectType));
      if (categoryName == null)
        throw new ArgumentNullException(nameof (categoryName));
      if (objectName == null)
        throw new ArgumentNullException(nameof (objectName));
      if (!typeof (IProfileManager).IsAssignableFrom(objectType))
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.General_InvalidType, (object) typeof (IProfileManager).FullName), objectType.FullName);
      this._objectType = objectType;
      this._categoryName = categoryName;
      this._objectName = objectName;
      this._categoryResourceID = categoryResourceID;
      this._objectNameResourceID = objectNameResourceID;
      this._isToolsOptionPage = isToolsOptionPage;
    }

    public string GroupName
    {
      get
      {
        return this._groupName;
      }
      set
      {
        this._groupName = value;
      }
    }

    public short GroupResourceID
    {
      get
      {
        return this._groupResourceID;
      }
      set
      {
        this._groupResourceID = value;
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

    public string ObjectName
    {
      get
      {
        return this._objectName;
      }
    }

    public short ObjectNameResourceID
    {
      get
      {
        return this._objectNameResourceID;
      }
    }

    public Type ObjectType
    {
      get
      {
        return this._objectType;
      }
    }

    public string ResourcePackageGuid
    {
      get
      {
        return this._resourcePackageGuid;
      }
      set
      {
        this._resourcePackageGuid = value;
      }
    }

    public short DescriptionResourceID
    {
      get
      {
        return this._descriptionResourceID;
      }
      set
      {
        this._descriptionResourceID = value;
      }
    }

    public string AlternateParent
    {
      get
      {
        return this._alternateParent;
      }
      set
      {
        this._alternateParent = value;
      }
    }

    public bool IsToolsOptionPage
    {
      get
      {
        return this._isToolsOptionPage;
      }
    }

    public ProfileMigrationType MigrationType
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

    private string SettingsRegKey
    {
      get
      {
        if (string.IsNullOrEmpty(this.GroupName))
          return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UserSettings\\{0}_{1}", (object) this.CategoryName, (object) this.ObjectName);
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UserSettings\\{0}\\{1}_{2}", (object) this.GroupName, (object) this.CategoryName, (object) this.ObjectName);
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyCreateObject, (object) this.ObjectType.Name));
      if (!string.IsNullOrEmpty(this.GroupName) && this.GroupResourceID > (short) 0)
      {
        using (RegistrationAttribute.Key key = context.CreateKey(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UserSettings\\{0}", (object) this.GroupName)))
          key.SetValue(string.Empty, (object) this.GroupResourceID.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      using (RegistrationAttribute.Key key = context.CreateKey(this.SettingsRegKey))
      {
        key.SetValue(string.Empty, (object) this.ObjectNameResourceID.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        key.SetValue("Package", (object) context.ComponentType.GUID.ToString("B"));
        key.SetValue("Category", (object) this.ObjectType.GUID.ToString("B"));
        if (this.IsToolsOptionPage)
          key.SetValue("ToolsOptionsPath", (object) this.CategoryName);
        if (!string.IsNullOrEmpty(this.AlternateParent))
          key.SetValue("AlternateParent", (object) this.AlternateParent);
        if (!string.IsNullOrEmpty(this.ResourcePackageGuid))
          key.SetValue("ResourcePackage", (object) this.ResourcePackageGuid);
        if (this.DescriptionResourceID > (short) 0)
          key.SetValue("Description", (object) this.DescriptionResourceID.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        key.SetValue("VSSettingsMigration", (object) (int) this.MigrationType);
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.SettingsRegKey);
    }
  }
}
