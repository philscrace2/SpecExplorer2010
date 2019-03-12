// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsSettingsManager
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
  [Guid("94D59A1D-A3A8-46AB-B1DE-B7F034018137")]
  [ComImport]
  public interface IVsSettingsManager
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetCollectionScopes([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, out uint scopes);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetPropertyScopes([MarshalAs(UnmanagedType.LPWStr), In] string collectionPath, [MarshalAs(UnmanagedType.LPWStr), In] string propertyName, out uint scopes);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetReadOnlySettingsStore([In] uint scope, [MarshalAs(UnmanagedType.Interface)] out IVsSettingsStore store);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetWritableSettingsStore([In] uint scope, [MarshalAs(UnmanagedType.Interface)] out IVsWritableSettingsStore writableStore);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetApplicationDataFolder([In] uint folder, [MarshalAs(UnmanagedType.BStr)] out string folderPath);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetCommonExtensionsSearchPaths(
      [In] uint paths,
      [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.BStr), Out] string[] commonExtensionsPaths,
      out uint actualPaths);
  }
}
