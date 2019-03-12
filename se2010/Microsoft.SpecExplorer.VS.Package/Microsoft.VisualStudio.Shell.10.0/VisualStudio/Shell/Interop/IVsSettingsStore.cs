// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsSettingsStore
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [Guid("6B43326C-AB7C-4263-A7EF-354B9DCBF3D8")]
  [ComImport]
  public interface IVsSettingsStore
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetBool([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, out int value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetInt([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, out int value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetUnsignedInt([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, out uint value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetInt64([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, out long value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetUnsignedInt64([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, out ulong value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetString([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, [MarshalAs(UnmanagedType.BStr)] out string value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetBinary(
      [MarshalAs(UnmanagedType.LPWStr), In] string collectionPath,
      [MarshalAs(UnmanagedType.LPWStr), In] string propertyName,
      [In] uint byteLength,
      [MarshalAs(UnmanagedType.LPArray), Out, Optional] byte[] pBytes,
      [MarshalAs(UnmanagedType.LPArray), Out, Optional] uint[] actualByteLength);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetBoolOrDefault(
      [MarshalAs(UnmanagedType.LPWStr), In] string collectionPath,
      [MarshalAs(UnmanagedType.LPWStr), In] string propertyName,
      [In] int defaultValue,
      out int value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetIntOrDefault(
      [MarshalAs(UnmanagedType.LPWStr), In] string collectionPath,
      [MarshalAs(UnmanagedType.LPWStr), In] string propertyName,
      [In] int defaultValue,
      out int value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetUnsignedIntOrDefault(
      [MarshalAs(UnmanagedType.LPWStr), In] string collectionPath,
      [MarshalAs(UnmanagedType.LPWStr), In] string propertyName,
      [In] uint defaultValue,
      out uint value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetInt64OrDefault(
      [MarshalAs(UnmanagedType.LPWStr), In] string collectionPath,
      [MarshalAs(UnmanagedType.LPWStr), In] string propertyName,
      [In] long defaultValue,
      out long value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetUnsignedInt64OrDefault(
      [MarshalAs(UnmanagedType.LPWStr), In] string collectionPath,
      [MarshalAs(UnmanagedType.LPWStr), In] string propertyName,
      [In] ulong defaultValue,
      out ulong value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetStringOrDefault(
      [MarshalAs(UnmanagedType.LPWStr), In] string collectionPath,
      [MarshalAs(UnmanagedType.LPWStr), In] string propertyName,
      [MarshalAs(UnmanagedType.LPWStr), In] string defaultValue,
      [MarshalAs(UnmanagedType.BStr)] out string value);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetPropertyType([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, out uint type);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int PropertyExists([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, out int pfExists);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int CollectionExists([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, out int pfExists);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetSubCollectionCount([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, out uint subCollectionCount);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetPropertyCount([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, out uint propertyCount);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetLastWriteTime([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPArray), Out] SYSTEMTIME[] lastWriteTime);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetSubCollectionName([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [In] uint index, [MarshalAs(UnmanagedType.BStr)] out string subCollectionName);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetPropertyName([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [In] uint index, [MarshalAs(UnmanagedType.BStr)] out string propertyName);
  }
}
