// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsThreadedWaitDialogFactory
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [CompilerGenerated]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("D10D92B6-D073-456F-8A26-B63811202B21")]
  [TypeIdentifier]
  [ComImport]
  public interface IVsThreadedWaitDialogFactory
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int CreateInstance([MarshalAs(UnmanagedType.Interface)] out IVsThreadedWaitDialog2 ppIVsThreadedWaitDialog);
  }
}
