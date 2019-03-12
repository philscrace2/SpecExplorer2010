// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.UIDataSourceProperty
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class UIDataSourceProperty : PropertyDescription
  {
    private IVsUIObject value;

    public UIDataSourceProperty(string name, IVsUIObject initialValue)
      : base(name, Utilities.GetObjectType(initialValue))
    {
      this.Value = initialValue;
    }

    public UIDataSourceProperty(string name, string type)
      : base(name, type)
    {
      this.Value = BuiltInPropertyValue.CreateEmptyValue(type);
    }

    public IVsUIObject Value
    {
      get
      {
        return this.value;
      }
      set
      {
        IVsUIObject vsUiObject = value;
        if (vsUiObject == null)
          throw new ArgumentException(Resources.Error_PropValueNotUIObject);
        if (!Utilities.GetObjectType(vsUiObject).Equals(this.Type, StringComparison.Ordinal))
          throw new InvalidCastException(Resources.Error_IncorrectPropValueType);
        this.value = value;
      }
    }

    public bool HasValue
    {
      get
      {
        return !this.IsEmptyValue(this.Value);
      }
    }

    private bool IsEmptyValue(IVsUIObject value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (value is IIndirectPropertyValue)
        return false;
      return Utilities.GetObjectData(value) == null;
    }
  }
}
