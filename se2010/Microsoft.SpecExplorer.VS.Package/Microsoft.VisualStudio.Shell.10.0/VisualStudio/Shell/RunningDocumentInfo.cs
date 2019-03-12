// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.RunningDocumentInfo
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public struct RunningDocumentInfo
  {
    public uint DocCookie;
    public uint Flags;
    public uint ReadLocks;
    public uint EditLocks;
    public IVsHierarchy Hierarchy;
    public uint ItemId;
    public string Moniker;
    public object DocData;
  }
}
