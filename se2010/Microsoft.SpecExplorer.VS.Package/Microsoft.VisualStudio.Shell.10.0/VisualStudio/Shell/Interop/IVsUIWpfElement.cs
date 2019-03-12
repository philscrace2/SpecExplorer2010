// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIWpfElement
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [Guid("CA87E95D-5AEE-4A16-BDCA-94A1F7F769A9")]
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [ComImport]
  public interface IVsUIWpfElement
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int CreateFrameworkElement([MarshalAs(UnmanagedType.IUnknown)] out object ppUnkElement);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetFrameworkElement([MarshalAs(UnmanagedType.IUnknown)] out object ppUnkElement);
  }
}
