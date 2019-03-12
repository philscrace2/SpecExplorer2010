// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideProjectFactoryAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideProjectFactoryAttribute : RegistrationAttribute
  {
    private int _sortPriority = 100;
    private Guid _folderGuid = Guid.Empty;
    private Type _factoryType;
    private string _displayProjectFileExtensions;
    private string _name;
    private string _defaultProjectExtension;
    private string _possibleProjectExtensions;
    private string _projectTemplatesDirectory;
    private string languageVsTemplate;
    private string templateGroupIDsVsTemplate;
    private string templateIDsVsTemplate;
    private string displayProjectTypeVsTemplate;
    private string projectSubTypeVsTemplate;
    private bool newProjectRequireNewFolderVsTemplate;
    private bool showOnlySpecifiedTemplatesVsTemplate;

    public ProvideProjectFactoryAttribute(
      Type factoryType,
      string name,
      string displayProjectFileExtensionsResourceID,
      string defaultProjectExtension,
      string possibleProjectExtensions,
      string projectTemplatesDirectory)
    {
      if (factoryType == (Type) null)
        throw new ArgumentNullException(nameof (factoryType));
      this._factoryType = factoryType;
      this._name = name;
      this._displayProjectFileExtensions = displayProjectFileExtensionsResourceID;
      this._defaultProjectExtension = defaultProjectExtension;
      this._possibleProjectExtensions = possibleProjectExtensions;
      this._projectTemplatesDirectory = projectTemplatesDirectory;
    }

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public int SortPriority
    {
      get
      {
        return this._sortPriority;
      }
      set
      {
        this._sortPriority = value;
      }
    }

    public Type FactoryType
    {
      get
      {
        return this._factoryType;
      }
    }

    public string DisplayProjectFileExtensions
    {
      get
      {
        return this._displayProjectFileExtensions;
      }
    }

    public string DefaultProjectExtension
    {
      get
      {
        return this._defaultProjectExtension;
      }
    }

    public string PossibleProjectExtensions
    {
      get
      {
        return this._possibleProjectExtensions;
      }
    }

    public string ProjectTemplatesDirectory
    {
      get
      {
        return this._projectTemplatesDirectory;
      }
    }

    public string FolderGuid
    {
      get
      {
        return this._folderGuid.ToString("B");
      }
      set
      {
        this._folderGuid = new Guid(value);
      }
    }

    public string LanguageVsTemplate
    {
      get
      {
        return this.languageVsTemplate;
      }
      set
      {
        this.languageVsTemplate = value;
      }
    }

    public string DisplayProjectTypeVsTemplate
    {
      get
      {
        return this.displayProjectTypeVsTemplate;
      }
      set
      {
        this.displayProjectTypeVsTemplate = value;
      }
    }

    public bool DisableOnlineTemplates { get; set; }

    public string ProjectSubTypeVsTemplate
    {
      get
      {
        return this.projectSubTypeVsTemplate;
      }
      set
      {
        this.projectSubTypeVsTemplate = value;
      }
    }

    public bool NewProjectRequireNewFolderVsTemplate
    {
      get
      {
        return this.newProjectRequireNewFolderVsTemplate;
      }
      set
      {
        this.newProjectRequireNewFolderVsTemplate = value;
      }
    }

    public bool ShowOnlySpecifiedTemplatesVsTemplate
    {
      get
      {
        return this.showOnlySpecifiedTemplatesVsTemplate;
      }
      set
      {
        this.showOnlySpecifiedTemplatesVsTemplate = value;
      }
    }

    public string TemplateGroupIDsVsTemplate
    {
      get
      {
        return this.templateGroupIDsVsTemplate;
      }
      set
      {
        this.templateGroupIDsVsTemplate = value;
      }
    }

    public string TemplateIDsVsTemplate
    {
      get
      {
        return this.templateIDsVsTemplate;
      }
      set
      {
        this.templateIDsVsTemplate = value;
      }
    }

    private string ProjectRegKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Projects\\{0}", (object) this.FactoryType.GUID.ToString("B"));
      }
    }

    private string NewPrjTemplateRegKey(RegistrationAttribute.RegistrationContext context)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "NewProjectTemplates\\TemplateDirs\\{0}\\/1", (object) context.ComponentType.GUID.ToString("B"));
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyProjectFactory, (object) this.FactoryType.Name));
      using (RegistrationAttribute.Key key = context.CreateKey(this.ProjectRegKey))
      {
        key.SetValue(string.Empty, (object) this.FactoryType.Name);
        if (this._name != null)
          key.SetValue("DisplayName", (object) this._name);
        if (this._displayProjectFileExtensions != null)
          key.SetValue("DisplayProjectFileExtensions", (object) this._displayProjectFileExtensions);
        if (this.DisableOnlineTemplates)
          key.SetValue("DisableOnlineTemplates", (object) 1);
        key.SetValue("Package", (object) context.ComponentType.GUID.ToString("B"));
        if (this._defaultProjectExtension != null)
          key.SetValue("DefaultProjectExtension", (object) this._defaultProjectExtension);
        if (this._possibleProjectExtensions != null)
          key.SetValue("PossibleProjectExtensions", (object) this._possibleProjectExtensions);
        if (this._projectTemplatesDirectory != null)
        {
          if (!Path.IsPathRooted(this._projectTemplatesDirectory))
            this._projectTemplatesDirectory = Path.Combine(context.ComponentPath, this._projectTemplatesDirectory);
          key.SetValue("ProjectTemplatesDir", (object) this._projectTemplatesDirectory);
        }
        if (this.languageVsTemplate != null)
          key.SetValue("Language(VsTemplate)", (object) this.languageVsTemplate);
        if (this.showOnlySpecifiedTemplatesVsTemplate)
          key.SetValue("ShowOnlySpecifiedTemplates(VsTemplate)", (object) 1);
        if (this.templateGroupIDsVsTemplate != null)
          key.SetValue("TemplateGroupIDs(VsTemplate)", (object) this.templateGroupIDsVsTemplate);
        if (this.templateIDsVsTemplate != null)
          key.SetValue("TemplateIDs(VsTemplate)", (object) this.templateIDsVsTemplate);
        if (this.displayProjectTypeVsTemplate != null)
          key.SetValue("DisplayProjectType(VsTemplate)", (object) this.displayProjectTypeVsTemplate);
        if (this.projectSubTypeVsTemplate != null)
          key.SetValue("ProjectSubType(VsTemplate)", (object) this.projectSubTypeVsTemplate);
        if (this.newProjectRequireNewFolderVsTemplate)
          key.SetValue("NewProjectRequireNewFolder(VsTemplate)", (object) 1);
      }
      using (RegistrationAttribute.Key key = context.CreateKey(this.NewPrjTemplateRegKey(context)))
      {
        string empty = string.Empty;
        if (this._name != null)
          key.SetValue(empty, (object) this._name);
        key.SetValue("SortPriority", (object) this._sortPriority);
        if (this._projectTemplatesDirectory != null)
          key.SetValue("TemplatesDir", (object) this._projectTemplatesDirectory);
        if (!(this._folderGuid != Guid.Empty))
          return;
        key.SetValue("Folder", (object) this.FolderGuid);
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.ProjectRegKey);
      context.RemoveKey(this.NewPrjTemplateRegKey(context));
    }
  }
}
