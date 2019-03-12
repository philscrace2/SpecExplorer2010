// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideToolboxPageAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideToolboxPageAttribute : ProvideOptionDialogPageAttribute
  {
    private short _pageOrder;
    private string _helpKeyword;

    public ProvideToolboxPageAttribute(Type pageType, short nameResourceID)
      : this(pageType, nameResourceID, (short) 0)
    {
    }

    public override object TypeId
    {
      get
      {
        return (object) this;
      }
    }

    public ProvideToolboxPageAttribute(Type pageType, short nameResourceID, short pageOrder)
      : this(pageType, nameResourceID, pageOrder, (string) null)
    {
    }

    public ProvideToolboxPageAttribute(
      Type pageType,
      short nameResourceID,
      short pageOrder,
      string helpKeyword)
      : base(pageType, "#" + nameResourceID.ToString())
    {
      this._pageOrder = pageOrder;
      this._helpKeyword = helpKeyword;
    }

    public string HelpKeyword
    {
      get
      {
        return this._helpKeyword;
      }
    }

    public short PageOrder
    {
      get
      {
        return this._pageOrder;
      }
    }

    private string ToolboxPageRegKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ToolboxPages\\{0}", (object) this.PageType.FullName);
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyToolboxPage, (object) this.PageType.Name));
      using (RegistrationAttribute.Key key = context.CreateKey(this.ToolboxPageRegKey))
      {
        key.SetValue(string.Empty, (object) this.PageNameResourceId);
        key.SetValue("Package", (object) context.ComponentType.GUID.ToString("B"));
        key.SetValue("Page", (object) this.PageType.GUID.ToString("B"));
        if (this.PageOrder != (short) 0)
          key.SetValue("DefaultTbx", (object) this.PageOrder);
        if (this._helpKeyword == null || this._helpKeyword.Length <= 0)
          return;
        key.SetValue("HelpKeyword", (object) this._helpKeyword);
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.ToolboxPageRegKey);
    }
  }
}
