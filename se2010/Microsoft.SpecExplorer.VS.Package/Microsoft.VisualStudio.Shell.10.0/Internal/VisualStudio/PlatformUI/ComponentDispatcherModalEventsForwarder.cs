// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.ComponentDispatcherModalEventsForwarder
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  internal static class ComponentDispatcherModalEventsForwarder
  {
    private static IOleComponentManager componentManager;
    private static uint componentId;

    public static void Install(IOleComponentManager componentManagerArg, uint componentIdArg)
    {
      if (componentManagerArg == null)
        throw new ArgumentNullException(nameof (componentManagerArg));
      ComponentDispatcherModalEventsForwarder.componentManager = componentManagerArg;
      ComponentDispatcherModalEventsForwarder.componentId = componentIdArg;
      ComponentDispatcher.EnterThreadModal += new EventHandler(ComponentDispatcherModalEventsForwarder.ComponentDispatcher_EnterThreadModal);
      ComponentDispatcher.LeaveThreadModal += new EventHandler(ComponentDispatcherModalEventsForwarder.ComponentDispatcher_LeaveThreadModal);
      Dispatcher.CurrentDispatcher.ShutdownStarted += new EventHandler(ComponentDispatcherModalEventsForwarder.CurrentDispatcher_ShutdownStarted);
    }

    public static void Uninstall()
    {
      if (ComponentDispatcherModalEventsForwarder.componentManager == null)
        return;
      Dispatcher.CurrentDispatcher.ShutdownStarted -= new EventHandler(ComponentDispatcherModalEventsForwarder.CurrentDispatcher_ShutdownStarted);
      ComponentDispatcher.EnterThreadModal -= new EventHandler(ComponentDispatcherModalEventsForwarder.ComponentDispatcher_EnterThreadModal);
      ComponentDispatcher.LeaveThreadModal -= new EventHandler(ComponentDispatcherModalEventsForwarder.ComponentDispatcher_LeaveThreadModal);
      ComponentDispatcherModalEventsForwarder.componentManager = (IOleComponentManager) null;
    }

    private static void ComponentDispatcher_EnterThreadModal(object sender, EventArgs e)
    {
      ComponentDispatcherModalEventsForwarder.componentManager.OnComponentEnterState(ComponentDispatcherModalEventsForwarder.componentId, 1U, 0U, 0U, (IOleComponentManager[]) null, 0U);
    }

    private static void ComponentDispatcher_LeaveThreadModal(object sender, EventArgs e)
    {
      Marshal.ThrowExceptionForHR(ComponentDispatcherModalEventsForwarder.componentManager.FOnComponentExitState(ComponentDispatcherModalEventsForwarder.componentId, 1U, 0U, 0U, (IOleComponentManager[]) null));
    }

    private static void CurrentDispatcher_ShutdownStarted(object sender, EventArgs e)
    {
      ComponentDispatcherModalEventsForwarder.Uninstall();
    }
  }
}
