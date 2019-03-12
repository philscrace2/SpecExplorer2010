// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.UIDataSourceVerbEnumerator
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class UIDataSourceVerbEnumerator : IVsUIEnumDataSourceVerbs
  {
    private ICollection<UIDataSourceVerb> verbs;
    private IEnumerator<UIDataSourceVerb> enumerator;

    public UIDataSourceVerbEnumerator(ICollection<UIDataSourceVerb> verbs)
    {
      this.verbs = verbs;
      this.enumerator = verbs.GetEnumerator();
    }

    private UIDataSourceVerbEnumerator(
      ICollection<UIDataSourceVerb> verbs,
      IEnumerator<UIDataSourceVerb> enumerator)
    {
      this.verbs = verbs;
      this.enumerator = enumerator;
    }

    public int Clone(out IVsUIEnumDataSourceVerbs ppEnum)
    {
      ppEnum = (IVsUIEnumDataSourceVerbs) new UIDataSourceVerbEnumerator(this.verbs, this.enumerator);
      return 0;
    }

    public int Next(uint celt, string[] rgelt, out uint pceltFetched)
    {
      uint num = 0;
      while (celt-- != 0U && this.enumerator.MoveNext())
      {
        rgelt[(IntPtr) num] = this.enumerator.Current.Name;
        ++num;
      }
      pceltFetched = num;
      return num <= 0U ? 1 : 0;
    }

    public int Reset()
    {
      this.enumerator = this.verbs.GetEnumerator();
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
