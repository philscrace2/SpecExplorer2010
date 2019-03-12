// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.RegisterEditorExtensionAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Shell
{
  [Obsolete("RegisterEditorExtensionAttribute has been deprecated. Please use ProvideEditorExtensionAttribute instead.")]
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class RegisterEditorExtensionAttribute : RegistrationAttribute
  {
    private Guid factory;
    private string extension;
    private int priority;
    private Guid project;
    private string templateDir;
    private int resId;
    private bool editorFactoryNotify;

    public RegisterEditorExtensionAttribute(object factoryType, string extension, int priority)
    {
      if (!extension.StartsWith(".", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Attributes_ExtensionNeedsDot, (object) extension));
      if (factoryType is string)
        this.factory = new Guid((string) factoryType);
      else if ((object) (factoryType as Type) != null)
        this.factory = ((Type) factoryType).GUID;
      else if (factoryType is Guid)
        this.factory = (Guid) factoryType;
      else
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Attributes_InvalidFactoryType, factoryType));
      this.extension = extension;
      this.priority = priority;
      this.project = Guid.Empty;
      this.templateDir = "";
      this.resId = 0;
      this.editorFactoryNotify = false;
    }

    public string Extension
    {
      get
      {
        return this.extension;
      }
    }

    public Guid Factory
    {
      get
      {
        return this.factory;
      }
    }

    public int Priority
    {
      get
      {
        return this.priority;
      }
    }

    public string ProjectGuid
    {
      set
      {
        this.project = new Guid(value);
      }
      get
      {
        return this.project.ToString();
      }
    }

    public bool EditorFactoryNotify
    {
      get
      {
        return this.editorFactoryNotify;
      }
      set
      {
        this.editorFactoryNotify = value;
      }
    }

    public string TemplateDir
    {
      get
      {
        return this.templateDir;
      }
      set
      {
        this.templateDir = value;
      }
    }

    public int NameResourceID
    {
      get
      {
        return this.resId;
      }
      set
      {
        this.resId = value;
      }
    }

    private string RegKeyName
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Editors\\{0}", (object) this.Factory.ToString("B"));
      }
    }

    private string ProjectRegKeyName(RegistrationAttribute.RegistrationContext context)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Projects\\{0}\\AddItemTemplates\\TemplateDirs\\{1}", (object) this.project.ToString("B"), (object) context.ComponentType.GUID.ToString("B"));
    }

    private string EditorFactoryNotifyKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Projects\\{0}\\FileExtensions\\{1}", (object) this.project.ToString("B"), (object) this.Extension);
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyEditorExtension, (object) this.Extension, (object) this.Factory.ToString("B")));
      using (RegistrationAttribute.Key key = context.CreateKey(this.RegKeyName))
      {
        if (this.resId != 0)
          key.SetValue("DisplayName", (object) ("#" + this.resId.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        key.SetValue("Package", (object) context.ComponentType.GUID.ToString("B"));
      }
      using (RegistrationAttribute.Key key = context.CreateKey(this.RegKeyName + "\\Extensions"))
        key.SetValue(this.Extension.Substring(1), (object) this.Priority);
      if (this.project != Guid.Empty)
      {
        string name = this.ProjectRegKeyName(context) + "\\/1";
        using (RegistrationAttribute.Key key = context.CreateKey(name))
        {
          if (this.resId != 0)
            key.SetValue("", (object) ("#" + this.resId.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
          if (this.templateDir.Length != 0)
          {
            string path = Path.Combine(Path.GetDirectoryName(new Uri(context.ComponentType.Assembly.CodeBase, true).LocalPath), this.templateDir);
            string str = context.EscapePath(Path.GetFullPath(path));
            key.SetValue("TemplatesDir", (object) str);
          }
          key.SetValue("SortPriority", (object) this.Priority);
        }
      }
      if (!this.EditorFactoryNotify)
        return;
      if (this.project == Guid.Empty)
        throw new ArgumentException(Resources.Attributes_NoPrjForEditorFactoryNotify);
      using (RegistrationAttribute.Key key = context.CreateKey(this.EditorFactoryNotifyKey))
        key.SetValue("EditorFactoryNotify", (object) context.ComponentType.GUID.ToString("B"));
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.RegKeyName);
      if (!(this.project != Guid.Empty))
        return;
      context.RemoveKey(this.ProjectRegKeyName(context));
      if (!this.EditorFactoryNotify)
        return;
      context.RemoveKey(this.EditorFactoryNotifyKey);
    }
  }
}
