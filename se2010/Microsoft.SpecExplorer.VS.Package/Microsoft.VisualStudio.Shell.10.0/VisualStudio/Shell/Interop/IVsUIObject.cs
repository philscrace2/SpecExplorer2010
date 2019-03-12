// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIObject
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [TypeIdentifier]
  [Guid("86FD1A37-A8C2-41DF-98FA-086D79BFD33E")]
  [ComImport]
  public interface IVsUIObject
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int get_Type([MarshalAs(UnmanagedType.BStr)] out string pTypeName);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int get_Format(out uint pdwDataFormat);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int get_Data([MarshalAs(UnmanagedType.Struct)] out object pVar);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Equals([MarshalAs(UnmanagedType.Interface), In] IVsUIObject pOtherObject, out bool pfAreEqual);
  }
}
