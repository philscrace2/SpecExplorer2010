// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideToolWindowVisibilityAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideToolWindowVisibilityAttribute : RegistrationAttribute
  {
    private string _name;
    private Guid _toolwindow;
    private Guid _commandUIGuid;

    public ProvideToolWindowVisibilityAttribute(object toolWindow, string commandUIGuid)
    {
      if ((object) (toolWindow as Type) != null)
      {
        Type type = (Type) toolWindow;
        this._toolwindow = type.GUID;
        this._name = type.Name;
      }
      else if (toolWindow is string)
        this._toolwindow = new Guid(toolWindow as string);
      else
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.General_InvalidType, (object) typeof (Type).FullName), nameof (toolWindow));
      this._commandUIGuid = new Guid(commandUIGuid);
    }

    public Guid CommandUIGuid
    {
      get
      {
        return this._commandUIGuid;
      }
    }

    public string Name
    {
      get
      {
        return this._name;
      }
      set
      {
        this._name = value;
      }
    }

    private string RegistryPath
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ToolWindows\\{0}\\Visibility", (object) this._toolwindow.ToString("B"));
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyToolVisibility, (object) this._name, (object) this.CommandUIGuid.ToString("B")));
      using (RegistrationAttribute.Key key = context.CreateKey(this.RegistryPath))
        key.SetValue(this.CommandUIGuid.ToString("B"), (object) 0);
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveValue(this.RegistryPath, this.CommandUIGuid.ToString("B"));
    }
  }
}
