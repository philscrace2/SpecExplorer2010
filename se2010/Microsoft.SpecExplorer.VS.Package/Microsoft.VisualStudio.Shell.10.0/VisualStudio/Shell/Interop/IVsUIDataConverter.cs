// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIDataConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [Guid("6E48EB81-ADD0-4F9F-AF78-C02F053250B3")]
  [CompilerGenerated]
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IVsUIDataConverter
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int get_Type([MarshalAs(UnmanagedType.BStr)] out string pTypeName);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int get_ConvertibleFormats(out uint pdwDataFormatFrom, out uint pdwDataFormatTo);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Convert([MarshalAs(UnmanagedType.Interface), In] IVsUIObject pObject, [MarshalAs(UnmanagedType.Interface)] out IVsUIObject ppConvertedObject);
  }
}
