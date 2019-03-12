// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsToolWindowToolbarHost2
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [CompilerGenerated]
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("2EFC69A8-5E06-436D-88D5-F099353356DA")]
  [ComImport]
  public interface IVsToolWindowToolbarHost2
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int AddToolbar2([In] VSTWT_LOCATION dwLoc, [In] ref Guid pGuid, [In] uint dwId, [MarshalAs(UnmanagedType.Interface), In] IDropTarget pDropTarget);
  }
}
