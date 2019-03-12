// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.WpfHostPrivate
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.Internal.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  internal class WpfHostPrivate : IVsUIWpfHostPrivate
  {
    private SynchronizationContext mainSynchronizationContext;

    public int CreateWPFUIElementContainer(
      string szWindowType,
      IntPtr hWndParent,
      IVsUIElement pElement,
      IVsUIDataSource pDataSource,
      object pSite,
      out IVsUIWPFElementContainerPrivate ppUIElementContainer)
    {
      try
      {
        Window containerInternal = WpfHostPrivate.CreateElementContainerInternal(szWindowType, hWndParent, pSite);
        containerInternal.DataContext = WpfHostPrivate.ConstructDataContext((FrameworkElement) containerInternal, pSite, pDataSource);
        containerInternal.Content = (object) WindowHelper.GetFrameworkElementFromUIElement(pElement);
        new WindowInteropHelper(containerInternal).EnsureHandle();
        ppUIElementContainer = (IVsUIWPFElementContainerPrivate) new WpfHostPrivate.UIWPFElementContainer(containerInternal);
        return 0;
      }
      catch (Exception ex)
      {
        ppUIElementContainer = (IVsUIWPFElementContainerPrivate) null;
        return Marshal.GetHRForException(ex);
      }
    }

    public int InvokeShutdown()
    {
      Dispatcher.CurrentDispatcher.InvokeShutdown();
      return 0;
    }

    private bool ExceptionMessageShownOnce { get; set; }

    public int EnableUnhandledExceptionDisplay()
    {
      Dispatcher.CurrentDispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(this.OnUnhandledException);
      this.ExceptionMessageShownOnce = false;
      return 0;
    }

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      if (!this.ExceptionMessageShownOnce)
      {
        this.ExceptionMessageShownOnce = true;
        int num = (int) System.Windows.Forms.MessageBox.Show(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_UnhandledException_Format, (object) e.Exception.GetType(), (object) e.Exception.Message).Replace("\\r", "\r"), Resources.Caption_WPFDispatcherUnhandledException, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      e.Handled = true;
    }

    public int VerifySynchronizationContext()
    {
      return this.mainSynchronizationContext != null && !object.ReferenceEquals((object) SynchronizationContext.Current, (object) this.mainSynchronizationContext) ? -2147467259 : 0;
    }

    private void InstallSynchronizationContext()
    {
      SynchronizationContext current = SynchronizationContext.Current;
      if (current != null && !current.GetType().IsEquivalentTo(typeof (SynchronizationContext)))
        return;
      this.mainSynchronizationContext = (SynchronizationContext) new DispatcherSynchronizationContext();
      SynchronizationContext.SetSynchronizationContext(this.mainSynchronizationContext);
    }

    public int RaiseIdle()
    {
      ComponentDispatcher.RaiseIdle();
      return 0;
    }

    public int RaiseThreadMessage(Microsoft.VisualStudio.OLE.Interop.MSG[] pMsg, out bool pResult)
    {
      pResult = ComponentDispatcher.RaiseThreadMessage(ref new System.Windows.Interop.MSG()
      {
        hwnd = pMsg[0].hwnd,
        message = (int) pMsg[0].message,
        wParam = pMsg[0].wParam,
        lParam = pMsg[0].lParam,
        time = (int) pMsg[0].time,
        pt_x = pMsg[0].pt.x,
        pt_y = pMsg[0].pt.y
      });
      return 0;
    }

    public int RegisterComponentForModalTracking(
      IOleComponentManager pComponentManager,
      uint dwComponentId)
    {
      try
      {
        ComponentDispatcherModalEventsForwarder.Install(pComponentManager, dwComponentId);
        this.InstallSynchronizationContext();
        return 0;
      }
      catch (Exception ex)
      {
        return Marshal.GetHRForException(ex);
      }
    }

    public int InvokeRender()
    {
      Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Render, (Delegate) (unused => (object) null), (object) null);
      return 0;
    }

    private static Window CreateElementContainerInternal(
      string windowType,
      IntPtr parent,
      object site)
    {
      System.Type type = System.Type.GetType(windowType);
      if (type == (System.Type) null)
        throw new ArgumentException("The type name could not be resolved", nameof (windowType));
      Window instance = Activator.CreateInstance(type) as Window;
      if (instance == null)
        throw new ArgumentException("The requested type is not derived from System.Windows.Window", nameof (windowType));
      new WindowInteropHelper(instance).Owner = parent;
      if (site != null)
        (instance as IObjectWithSite)?.SetSite(site);
      return instance;
    }

    private static object ConstructDataContext(
      FrameworkElement element,
      object site,
      IVsUIDataSource dataSource)
    {
      if (site == null)
        return (object) new DataSource(dataSource, new DataSourceParameters(element));
      Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp = site as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
      if (sp == null)
        throw new ArgumentException("The site does not implement IServiceProvider", nameof (site));
      return (object) new DataSource(dataSource, new DataSourceParameters(element, (System.IServiceProvider) new ServiceProvider(sp)));
    }

    private class UIWPFElementContainer : IVsUIWPFElementContainerPrivate
    {
      private Window Window { get; set; }

      public UIWPFElementContainer(Window window)
      {
        this.Window = window;
      }

      public int GetHandle(out IntPtr pHandle)
      {
        pHandle = new WindowInteropHelper(this.Window).Handle;
        return 0;
      }

      public int ShowWindow()
      {
        this.Window.Show();
        return 0;
      }
    }
  }
}
