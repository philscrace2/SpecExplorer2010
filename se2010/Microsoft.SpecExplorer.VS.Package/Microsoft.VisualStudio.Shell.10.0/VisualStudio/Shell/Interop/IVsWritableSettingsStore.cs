// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsWritableSettingsStore
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [TypeIdentifier]
  [Guid("16FA7461-9E7C-4F28-B28F-AABBF73C0193")]
  [CompilerGenerated]
  [ComImport]
  public interface IVsWritableSettingsStore : IVsSettingsStore
  {
    [SpecialName]
    extern void _VtblGap1_21();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetBool([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, [In] int value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetInt([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, [In] int value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetUnsignedInt([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, [In] uint value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetInt64([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, [In] long value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetUnsignedInt64([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, [In] ulong value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetString([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, [MarshalAs(UnmanagedType.LPWStr), In] string value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetBinary([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, [In] uint byteLength, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2), In] byte[] pBytes);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int DeleteProperty([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int CreateCollection([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int DeleteCollection([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath);
  }
}
