// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIDispatch
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [Guid("0DF3E43A-5356-4A33-8AC1-3BE6E3337C37")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [TypeIdentifier]
  [ComImport]
  public interface IVsUIDispatch
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Invoke([MarshalAs(UnmanagedType.LPWStr), In] string verb, [MarshalAs(UnmanagedType.Struct), In] object pvaIn, [MarshalAs(UnmanagedType.Struct)] out object pvaOut);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int EnumVerbs([MarshalAs(UnmanagedType.Interface)] out IVsUIEnumDataSourceVerbs ppEnum);
  }
}
