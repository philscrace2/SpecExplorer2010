// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIWin32Element
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [TypeIdentifier]
  [Guid("AD9A00F2-AC5B-4A49-94B7-17CC3CE1A46A")]
  [CompilerGenerated]
  [ComImport]
  public interface IVsUIWin32Element
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Create(IntPtr parent, out IntPtr pHandle);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Destroy();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetHandle(out IntPtr pHandle);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int ShowModal(IntPtr parent, out int pDlgResult);
  }
}
