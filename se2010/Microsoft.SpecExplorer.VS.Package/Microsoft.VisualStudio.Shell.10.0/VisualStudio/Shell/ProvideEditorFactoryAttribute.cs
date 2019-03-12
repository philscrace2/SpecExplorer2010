// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideEditorFactoryAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideEditorFactoryAttribute : RegistrationAttribute
  {
    private Type _factoryType;
    private short _nameResourceID;
    private __VSEDITORTRUSTLEVEL _trustLevel;

    public ProvideEditorFactoryAttribute(Type factoryType, short nameResourceID)
    {
      if (factoryType == (Type) null)
        throw new ArgumentNullException(nameof (factoryType));
      this._factoryType = factoryType;
      this._nameResourceID = nameResourceID;
      this._trustLevel = __VSEDITORTRUSTLEVEL.ETL_NeverTrusted;
    }

    public Type FactoryType
    {
      get
      {
        return this._factoryType;
      }
    }

    public __VSEDITORTRUSTLEVEL TrustLevel
    {
      get
      {
        return this._trustLevel;
      }
      set
      {
        this._trustLevel = value;
      }
    }

    public short NameResourceID
    {
      get
      {
        return this._nameResourceID;
      }
    }

    private string EditorRegKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Editors\\{0}", (object) this.FactoryType.GUID.ToString("B"));
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyEditorFactory, (object) this.FactoryType.Name));
      using (RegistrationAttribute.Key key = context.CreateKey(this.EditorRegKey))
      {
        key.SetValue(string.Empty, (object) this.FactoryType.Name);
        key.SetValue("DisplayName", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "#{0}", (object) this.NameResourceID));
        key.SetValue("Package", (object) context.ComponentType.GUID.ToString("B"));
        key.SetValue("EditorTrustLevel", (object) (int) this._trustLevel);
        using (RegistrationAttribute.Key subkey = key.CreateSubkey("LogicalViews"))
        {
          TypeConverter converter = TypeDescriptor.GetConverter(typeof (LogicalView));
          foreach (ProvideViewAttribute customAttribute in this.FactoryType.GetCustomAttributes(typeof (ProvideViewAttribute), true))
          {
            if (customAttribute.LogicalView != LogicalView.Primary)
            {
              context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyEditorView, (object) converter.ConvertToString((object) customAttribute.LogicalView)));
              Guid guid = (Guid) converter.ConvertTo((object) customAttribute.LogicalView, typeof (Guid));
              string str = customAttribute.PhysicalView ?? string.Empty;
              subkey.SetValue(guid.ToString("B"), (object) str);
            }
          }
        }
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.EditorRegKey);
    }
  }
}
