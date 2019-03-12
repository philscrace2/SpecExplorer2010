// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIFactory
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
  [Guid("D416BA0D-25C6-463B-B2BD-F06142F0D4B7")]
  [CompilerGenerated]
  [ComImport]
  public interface IVsUIFactory
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int CreateUIElement([In] ref Guid guid, [In] uint dw, [MarshalAs(UnmanagedType.Interface)] out IVsUIElement ppUIElement);
  }
}
