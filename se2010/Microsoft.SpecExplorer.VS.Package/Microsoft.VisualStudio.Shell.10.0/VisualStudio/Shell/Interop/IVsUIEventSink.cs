﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIEventSink
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [CompilerGenerated]
  [Guid("515953AC-99C6-4F1B-8645-636A57E4B1E2")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [TypeIdentifier]
  [ComImport]
  public interface IVsUIEventSink
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int Disconnect([MarshalAs(UnmanagedType.Interface), In] IVsUISimpleDataSource pSource);
  }
}
