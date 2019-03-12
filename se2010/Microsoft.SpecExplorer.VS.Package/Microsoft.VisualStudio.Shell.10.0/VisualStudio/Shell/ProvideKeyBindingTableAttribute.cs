// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideKeyBindingTableAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideKeyBindingTableAttribute : RegistrationAttribute
  {
    private short _nameResourceID;
    private Guid _tableGuid;
    private bool _allowNavKeys;

    public ProvideKeyBindingTableAttribute(string tableGuid, short nameResourceID)
    {
      if (tableGuid == null)
        throw new ArgumentNullException(nameof (tableGuid));
      this._tableGuid = new Guid(tableGuid);
      this._nameResourceID = nameResourceID;
    }

    public short NameResourceID
    {
      get
      {
        return this._nameResourceID;
      }
    }

    public Guid TableGuid
    {
      get
      {
        return this._tableGuid;
      }
    }

    public bool AllowNavKeyBinding
    {
      get
      {
        return this._allowNavKeys;
      }
      set
      {
        this._allowNavKeys = value;
      }
    }

    private string KeyBindingRegKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "KeyBindingTables\\{0}", (object) this.TableGuid.ToString("B"));
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyKeyBinding, (object) this.TableGuid.ToString("B"), (object) this.NameResourceID));
      using (RegistrationAttribute.Key key = context.CreateKey(this.KeyBindingRegKey))
      {
        key.SetValue(string.Empty, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "#{0}", (object) this.NameResourceID));
        key.SetValue("Package", (object) context.ComponentType.GUID.ToString("B"));
        key.SetValue("AllowNavKeyBinding", (object) (this._allowNavKeys ? 1 : 0));
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.KeyBindingRegKey);
    }
  }
}
