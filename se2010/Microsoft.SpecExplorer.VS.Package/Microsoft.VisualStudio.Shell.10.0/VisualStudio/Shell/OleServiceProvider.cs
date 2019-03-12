// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.OleServiceProvider
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.VisualStudio.Shell
{
  internal static class OleServiceProvider
  {
    private static HandleRef NullHandleRef = new HandleRef((object) null, IntPtr.Zero);
    [ThreadStatic]
    private static Microsoft.VisualStudio.OLE.Interop.IServiceProvider globalProvider;

    public static Microsoft.VisualStudio.OLE.Interop.IServiceProvider GlobalProvider
    {
      get
      {
        return OleServiceProvider.globalProvider ?? (OleServiceProvider.globalProvider = OleServiceProvider.GetGlobalProviderFromMessageFilter());
      }
      set
      {
        OleServiceProvider.globalProvider = value;
      }
    }

    private static Microsoft.VisualStudio.OLE.Interop.IServiceProvider GetGlobalProviderFromMessageFilter()
    {
      object forCallingThread = OleServiceProvider.GetOleMessageFilterForCallingThread();
      if (forCallingThread == null)
        return (Microsoft.VisualStudio.OLE.Interop.IServiceProvider) null;
      return forCallingThread as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
    }

    private static object GetOleMessageFilterForCallingThread()
    {
      if (Thread.CurrentThread.GetApartmentState() == ApartmentState.MTA)
        return (object) null;
      IntPtr zero1 = IntPtr.Zero;
      if (ErrorHandler.Failed(Microsoft.VisualStudio.NativeMethods.CoRegisterMessageFilter(OleServiceProvider.NullHandleRef, ref zero1)))
        return (object) null;
      if (zero1 == IntPtr.Zero)
        return (object) null;
      IntPtr zero2 = IntPtr.Zero;
      Microsoft.VisualStudio.NativeMethods.CoRegisterMessageFilter(new HandleRef((object) null, zero1), ref zero2);
      object objectForIunknown = Marshal.GetObjectForIUnknown(zero1);
      Marshal.Release(zero1);
      return objectForIunknown;
    }
  }
}
