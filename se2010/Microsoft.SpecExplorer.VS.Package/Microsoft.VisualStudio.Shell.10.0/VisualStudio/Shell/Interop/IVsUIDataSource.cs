// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIDataSource
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("8D11DD44-7EF2-4C7A-B188-7DA136657F68")]
  [CompilerGenerated]
  [TypeIdentifier]
  [ComImport]
  public interface IVsUIDataSource : IVsUISimpleDataSource, IVsUIDispatch
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int Invoke([MarshalAs(UnmanagedType.LPWStr), In] string verb, [MarshalAs(UnmanagedType.Struct), In] object pvaIn, [MarshalAs(UnmanagedType.Struct)] out object pvaOut);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int EnumVerbs([MarshalAs(UnmanagedType.Interface)] out IVsUIEnumDataSourceVerbs ppEnum);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    new int Close();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetValue([MarshalAs(UnmanagedType.LPWStr), In] string prop, [MarshalAs(UnmanagedType.Interface)] out IVsUIObject ppValue);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetValue([MarshalAs(UnmanagedType.LPWStr), In] string prop, [MarshalAs(UnmanagedType.Interface), In] IVsUIObject pValue);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int AdvisePropertyChangeEvents([MarshalAs(UnmanagedType.Interface), In] IVsUIDataSourcePropertyChangeEvents pAdvise, out uint pCookie);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int UnadvisePropertyChangeEvents([In] uint cookie);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int EnumProperties([MarshalAs(UnmanagedType.Interface)] out IVsUIEnumDataSourceProperties ppEnum);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetShapeIdentifier(out Guid pGuid, out uint pdw);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int QueryValue([MarshalAs(UnmanagedType.LPWStr), In] string prop, [MarshalAs(UnmanagedType.LPArray), Out, Optional] string[] pTypeName, [MarshalAs(UnmanagedType.LPArray), Out, Optional] uint[] pDataFormat, [MarshalAs(UnmanagedType.LPArray), Out, Optional] object[] pValue);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int ResetValue([MarshalAs(UnmanagedType.LPWStr), In] string prop);
  }
}
