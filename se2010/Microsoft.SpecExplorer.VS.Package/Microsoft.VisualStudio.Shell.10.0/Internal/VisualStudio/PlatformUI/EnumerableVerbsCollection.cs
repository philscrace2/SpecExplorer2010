// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.EnumerableVerbsCollection
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class EnumerableVerbsCollection : EnumerableComCollection<IVsUIEnumDataSourceVerbs, string>
  {
    public EnumerableVerbsCollection(IVsUIEnumDataSourceVerbs enumerator)
      : base(enumerator)
    {
    }

    public override int Clone(
      IVsUIEnumDataSourceVerbs enumerator,
      out IVsUIEnumDataSourceVerbs clone)
    {
      // ISSUE: reference to a compiler-generated method
      return enumerator.Clone(out clone);
    }

    public override int NextItems(
      IVsUIEnumDataSourceVerbs enumerator,
      uint count,
      string[] items,
      out uint fetched)
    {
      // ISSUE: reference to a compiler-generated method
      return enumerator.Next(count, items, out fetched);
    }

    public override int Reset(IVsUIEnumDataSourceVerbs enumerator)
    {
      // ISSUE: reference to a compiler-generated method
      return enumerator.Reset();
    }

    public override int Skip(IVsUIEnumDataSourceVerbs enumerator, uint count)
    {
      // ISSUE: reference to a compiler-generated method
      return enumerator.Skip(count);
    }
  }
}
