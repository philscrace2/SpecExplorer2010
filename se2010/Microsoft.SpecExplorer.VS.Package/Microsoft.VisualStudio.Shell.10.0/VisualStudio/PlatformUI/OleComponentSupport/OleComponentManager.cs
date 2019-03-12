// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.OleComponentSupport.OleComponentManager
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.PlatformUI.OleComponentSupport
{
  [CLSCompliant(false)]
  public sealed class OleComponentManager
  {
    private static HandleRef NullHandleRef = new HandleRef((object) null, IntPtr.Zero);
    [ThreadStatic]
    private static IOleComponentManager instance;

    private OleComponentManager()
    {
    }

    public static IOleComponentManager Instance
    {
      get
      {
        if (OleComponentManager.instance == null)
          OleComponentManager.instance = OleComponentManager.GetActiveComponentManager() ?? OleComponentManager.CreateDefaultComponentManager();
        return OleComponentManager.instance;
      }
    }

    private static IOleComponentManager GetActiveComponentManager()
    {
      ServiceProvider globalProvider = ServiceProvider.GlobalProvider;
      if (globalProvider == null)
        return (IOleComponentManager) null;
      return OleComponentManager.GetComponentManager(globalProvider);
    }

    private static IOleComponentManager GetComponentManager(
      ServiceProvider serviceProvider)
    {
      if (serviceProvider == null)
        throw new ArgumentNullException(nameof (serviceProvider));
      object service = serviceProvider.GetService(typeof (SOleComponentManager));
      if (service == null)
        return (IOleComponentManager) null;
      return service as IOleComponentManager;
    }

    private static IOleComponentManager CreateDefaultComponentManager()
    {
      return (IOleComponentManager) null;
    }
  }
}
