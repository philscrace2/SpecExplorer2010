// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.Shell.Interop.IVsInvokerPrivate
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.Shell.Interop
{
  [Guid("20705D94-A39B-4741-B5E1-041C5985EF61")]
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [ComImport]
  public interface IVsInvokerPrivate
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Invoke([MarshalAs(UnmanagedType.Interface), In] IVsInvokablePrivate pInvokable);
  }
}
