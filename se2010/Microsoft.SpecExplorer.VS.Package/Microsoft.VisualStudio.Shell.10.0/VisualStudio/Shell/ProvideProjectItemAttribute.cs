// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideProjectItemAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideProjectItemAttribute : RegistrationAttribute
  {
    private int priority;
    private Guid factory;
    private string templateDir;
    private string itemType;

    public ProvideProjectItemAttribute(
      object projectFactoryType,
      string itemCategoryName,
      string templatesDir,
      int priority)
    {
      if (templatesDir == null || templatesDir.Length == 0)
        throw new ArgumentNullException(nameof (templatesDir));
      if (itemCategoryName == null || itemCategoryName.Length == 0)
        throw new ArgumentNullException(nameof (itemCategoryName));
      if (projectFactoryType is string)
        this.factory = new Guid((string) projectFactoryType);
      else if ((object) (projectFactoryType as Type) != null)
        this.factory = ((Type) projectFactoryType).GUID;
      else if (projectFactoryType is Guid)
        this.factory = (Guid) projectFactoryType;
      else
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Attributes_InvalidFactoryType, projectFactoryType));
      this.priority = priority;
      this.templateDir = templatesDir;
      this.itemType = itemCategoryName;
    }

    public Guid ProjectFactoryType
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

    public string TemplateDir
    {
      get
      {
        return this.templateDir;
      }
    }

    public string ItemType
    {
      get
      {
        return this.itemType;
      }
    }

    private string ProjectRegKeyName(RegistrationAttribute.RegistrationContext context)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Projects\\{0}\\AddItemTemplates\\TemplateDirs\\{1}\\/1", (object) this.factory.ToString("B"), (object) context.ComponentType.GUID.ToString("B"));
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyProjectItems, (object) this.factory.ToString("B")));
      using (RegistrationAttribute.Key key = context.CreateKey(this.ProjectRegKeyName(context)))
      {
        key.SetValue("", (object) this.itemType);
        string path = Path.Combine(Path.GetDirectoryName(new Uri(context.ComponentType.Assembly.CodeBase).LocalPath), this.templateDir);
        string str = context.EscapePath(Path.GetFullPath(path));
        key.SetValue("TemplatesDir", (object) str);
        key.SetValue("SortPriority", (object) this.Priority);
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.ProjectRegKeyName(context));
    }
  }
}
