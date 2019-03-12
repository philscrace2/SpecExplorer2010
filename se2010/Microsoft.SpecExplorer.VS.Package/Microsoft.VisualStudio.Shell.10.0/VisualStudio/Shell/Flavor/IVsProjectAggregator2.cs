// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Flavor.IVsProjectAggregator2
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Flavor
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("D6CEA324-8E81-4e0e-91DE-E5D7394A45CE")]
  [ComImport]
  public interface IVsProjectAggregator2
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetInner(IntPtr innerIUnknown);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetMyProject(IntPtr projectIUnknown);
  }
}
