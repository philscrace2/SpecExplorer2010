// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.IComEnumeratorRelay`2
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public interface IComEnumeratorRelay<TComEnumerator, TEnumerated>
  {
    int Clone(TComEnumerator enumerator, out TComEnumerator newInstance);

    int NextItems(TComEnumerator enumerator, uint count, TEnumerated[] items, out uint fetched);

    int Reset(TComEnumerator enumerator);

    int Skip(TComEnumerator enumerator, uint count);
  }
}
