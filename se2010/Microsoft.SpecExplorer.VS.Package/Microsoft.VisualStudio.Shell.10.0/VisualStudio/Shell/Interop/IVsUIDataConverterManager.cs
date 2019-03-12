// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIDataConverterManager
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [CompilerGenerated]
  [TypeIdentifier]
  [Guid("806BA229-8188-4663-A918-65B0E0CC0503")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IVsUIDataConverterManager
  {
    [SpecialName]
    extern void _VtblGap1_3();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetObjectConverter(
      [MarshalAs(UnmanagedType.Interface), In] IVsUIObject pObject,
      [In] uint dwDataFormatTo,
      [MarshalAs(UnmanagedType.Interface)] out IVsUIDataConverter ppConverter);
  }
}
