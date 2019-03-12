// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsCommonMessagePumpClientEvents
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [CompilerGenerated]
  [Guid("9C6D9104-7DB9-4ABD-841D-F0CFD24DE3D0")]
  [TypeIdentifier]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IVsCommonMessagePumpClientEvents
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int OnHandleSignaled([In] uint nHandle, out bool pfContinue);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int OnTimeout(out bool pfContinue);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int OnAfterMessageProcessed(out bool pfContinue);
  }
}
