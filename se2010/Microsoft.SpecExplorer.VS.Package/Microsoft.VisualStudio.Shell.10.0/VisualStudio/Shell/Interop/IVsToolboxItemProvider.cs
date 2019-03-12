// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsToolboxItemProvider
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [Guid("F2F94425-E001-4C4D-816C-70202E9A594C")]
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [ComImport]
  public interface IVsToolboxItemProvider
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetItemContent([MarshalAs(UnmanagedType.LPWStr), In] string szItemID, [In] ushort format, out IntPtr pGlobal);
  }
}
