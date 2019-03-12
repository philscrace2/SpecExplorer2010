// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.UIDataSourcePropertyEnumerator
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class UIDataSourcePropertyEnumerator : IVsUIEnumDataSourceProperties
  {
    private ICollection<UIDataSourceProperty> properties;
    private IEnumerator<UIDataSourceProperty> enumerator;

    public UIDataSourcePropertyEnumerator(ICollection<UIDataSourceProperty> properties)
    {
      this.properties = properties;
      this.enumerator = properties.GetEnumerator();
    }

    private UIDataSourcePropertyEnumerator(
      ICollection<UIDataSourceProperty> properties,
      IEnumerator<UIDataSourceProperty> enumerator)
    {
      this.properties = properties;
      this.enumerator = enumerator;
    }

    public int Clone(out IVsUIEnumDataSourceProperties ppEnum)
    {
      ppEnum = (IVsUIEnumDataSourceProperties) new UIDataSourcePropertyEnumerator(this.properties, this.enumerator);
      return 0;
    }

    public int Next(uint celt, VsUIPropertyDescriptor[] rgelt, out uint pceltFetched)
    {
      uint num = 0;
      while (celt-- != 0U && this.enumerator.MoveNext())
      {
        // ISSUE: reference to a compiler-generated field
        rgelt[(IntPtr) num].name = this.enumerator.Current.Name;
        // ISSUE: reference to a compiler-generated field
        rgelt[(IntPtr) num].type = this.enumerator.Current.Type;
        ++num;
      }
      pceltFetched = num;
      return num <= 0U ? 1 : 0;
    }

    public int Reset()
    {
      this.enumerator = this.properties.GetEnumerator();
      return 0;
    }

    public int Skip(uint celt)
    {
      bool flag = true;
      while (celt-- != 0U && flag)
        flag = this.enumerator.MoveNext();
      return !flag ? 1 : 0;
    }
  }
}
