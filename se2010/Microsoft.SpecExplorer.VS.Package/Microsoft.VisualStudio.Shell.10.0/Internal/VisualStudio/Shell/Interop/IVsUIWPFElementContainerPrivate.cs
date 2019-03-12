// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.Shell.Interop.IVsUIWPFElementContainerPrivate
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.Shell.Interop
{
  [CompilerGenerated]
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("472DA35D-30E4-4254-87C0-735A1C8F11E0")]
  [ComImport]
  public interface IVsUIWPFElementContainerPrivate
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetHandle(out IntPtr pHandle);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int ShowWindow();
  }
}
