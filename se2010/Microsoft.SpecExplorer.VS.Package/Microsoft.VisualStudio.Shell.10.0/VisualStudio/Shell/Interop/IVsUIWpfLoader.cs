// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIWpfLoader
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [Guid("89DB8AB3-9035-4016-AA8A-76F7AE09B65F")]
  [ComImport]
  public interface IVsUIWpfLoader
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int CreateUIElement([MarshalAs(UnmanagedType.LPWStr), In] string elementFQN, [MarshalAs(UnmanagedType.LPWStr), In] string codeBase, [MarshalAs(UnmanagedType.Interface)] out IVsUIElement ppUIElement);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int CreateUIElementOfType([MarshalAs(UnmanagedType.IUnknown), In] object pUnkElementType, [MarshalAs(UnmanagedType.Interface)] out IVsUIElement ppUIElement);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int ShowModalElement([MarshalAs(UnmanagedType.Interface), In] IVsUIElement pUIElement, [In] IntPtr hWndParent, out int pResult);
  }
}
