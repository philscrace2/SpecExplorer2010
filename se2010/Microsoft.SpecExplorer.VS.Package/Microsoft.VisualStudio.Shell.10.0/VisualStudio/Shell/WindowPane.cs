// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.WindowPane
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.VisualStudio.Shell
{
  [ContentProperty("Content")]
  [ComVisible(true)]
  public abstract class WindowPane : Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget, System.IServiceProvider, IVsWindowPane, IVsUIElementPane, IDisposable
  {
    private System.IServiceProvider _parentProvider;
    private ServiceProvider _provider;
    private IMenuCommandService _commandService;
    private HelpService _helpService;
    private bool _zombie;
    private UIWin32ElementWrapper win32Wrapper;

    protected WindowPane()
    {
    }

    protected WindowPane(System.IServiceProvider provider)
      : this()
    {
      this._parentProvider = provider;
    }

    public virtual System.Windows.Forms.IWin32Window Window
    {
      get
      {
        return (System.Windows.Forms.IWin32Window) null;
      }
    }

    protected WindowPane.PaneInitializationMode InitializationMode { get; private set; }

    public virtual object Content { get; set; }

    public void Dispose()
    {
      this.Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.win32Wrapper != null)
        this.win32Wrapper.Dispose();
      this.win32Wrapper = (UIWin32ElementWrapper) null;
      IDisposable disposable1 = (IDisposable) null;
      IDisposable disposable2 = this.Content == null ? this.Window as IDisposable : this.Content as IDisposable;
      if (disposable2 != null)
      {
        try
        {
          disposable2.Dispose();
        }
        catch (Exception ex)
        {
        }
      }
      disposable1 = (IDisposable) null;
      if (this._commandService != null)
      {
        if (this._commandService is IDisposable)
        {
          try
          {
            ((IDisposable) this._commandService).Dispose();
          }
          catch (Exception ex)
          {
          }
        }
      }
      this._commandService = (IMenuCommandService) null;
      if (this._parentProvider != null)
        this._parentProvider = (System.IServiceProvider) null;
      if (this._helpService != null)
        this._helpService = (HelpService) null;
      this._zombie = true;
    }

    private void EnsureCommandService()
    {
      if (this._commandService != null)
        return;
      this._commandService = (IMenuCommandService) new OleMenuCommandService((System.IServiceProvider) this);
    }

    protected virtual object GetService(System.Type serviceType)
    {
      if (this._zombie)
        return (object) null;
      if (serviceType == (System.Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      if (serviceType.IsEquivalentTo(typeof (IMenuCommandService)))
      {
        this.EnsureCommandService();
        return (object) this._commandService;
      }
      if (serviceType.IsEquivalentTo(typeof (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget)))
        return (object) this._commandService;
      if (serviceType.IsEquivalentTo(typeof (IHelpService)))
      {
        if (this._helpService == null)
          this._helpService = new HelpService((System.IServiceProvider) this);
        return (object) this._helpService;
      }
      if (this._provider != null)
      {
        object service = this._provider.GetService(serviceType);
        if (service != null)
          return service;
      }
      if (serviceType.IsEquivalentTo(typeof (IObjectWithSite)))
        return (object) null;
      if (this._parentProvider != null)
        return this._parentProvider.GetService(serviceType);
      return (object) null;
    }

    protected virtual void Initialize()
    {
    }

    protected virtual void OnClose()
    {
      this.Dispose();
    }

    protected virtual void OnCreate()
    {
    }

    protected virtual bool PreProcessMessage(ref Message m)
    {
      Control control = Control.FromChildHandle(m.HWnd);
      if (control != null)
        return control.PreProcessControlMessage(ref m) == PreProcessControlState.MessageProcessed;
      return false;
    }

    int Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget.Exec(
      ref Guid guidGroup,
      uint nCmdId,
      uint nCmdExcept,
      IntPtr pIn,
      IntPtr vOut)
    {
      Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget service = this.GetService(typeof (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget)) as Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;
      if (service != null)
        return service.Exec(ref guidGroup, nCmdId, nCmdExcept, pIn, vOut);
      return -2147221248;
    }

    int Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget.QueryStatus(
      ref Guid guidGroup,
      uint nCmdId,
      Microsoft.VisualStudio.OLE.Interop.OLECMD[] oleCmd,
      IntPtr oleText)
    {
      Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget service = this.GetService(typeof (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget)) as Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;
      if (service != null)
        return service.QueryStatus(ref guidGroup, nCmdId, oleCmd, oleText);
      return -2147221248;
    }

    object System.IServiceProvider.GetService(System.Type serviceType)
    {
      return this.GetService(serviceType);
    }

    int IVsUIElementPane.CloseUIElementPane()
    {
      this.OnClose();
      return 0;
    }

    int IVsUIElementPane.CreateUIElementPane(out object uiElement)
    {
      uiElement = (object) null;
      if (this.InitializationMode != WindowPane.PaneInitializationMode.Uninitialized)
        throw new InvalidOperationException("The WindowPane is already initialized");
      this.InitializationMode = WindowPane.PaneInitializationMode.IVsUIElementPane;
      this.OnCreate();
      if (this.Content != null)
      {
        uiElement = this.Content;
      }
      else
      {
        if (this.Window == null)
          return -2147418113;
        this.win32Wrapper = new UIWin32ElementWrapper(this);
        uiElement = (object) this.win32Wrapper;
      }
      return 0;
    }

    int IVsUIElementPane.GetDefaultUIElementSize(Microsoft.VisualStudio.OLE.Interop.SIZE[] size)
    {
      return -2147467263;
    }

    public virtual int LoadUIState(Stream stateStream)
    {
      return -2147467263;
    }

    private static byte[] GetBufferFromIStream(Microsoft.VisualStudio.OLE.Interop.IStream comStream)
    {
      LARGE_INTEGER dlibMove1;
      dlibMove1.QuadPart = 0L;
      ULARGE_INTEGER[] plibNewPosition = new ULARGE_INTEGER[1];
      comStream.Seek(dlibMove1, 1U, plibNewPosition);
      comStream.Seek(dlibMove1, 0U, (ULARGE_INTEGER[]) null);
      Microsoft.VisualStudio.OLE.Interop.STATSTG[] pstatstg = new Microsoft.VisualStudio.OLE.Interop.STATSTG[1];
      comStream.Stat(pstatstg, 1U);
      byte[] pv = new byte[(int) pstatstg[0].cbSize.QuadPart];
      uint pcbRead = 0;
      comStream.Read(pv, (uint) pv.Length, out pcbRead);
      LARGE_INTEGER dlibMove2;
      dlibMove2.QuadPart = (long) plibNewPosition[0].QuadPart;
      comStream.Seek(dlibMove2, 0U, (ULARGE_INTEGER[]) null);
      return pv;
    }

    int IVsUIElementPane.LoadUIElementState(Microsoft.VisualStudio.OLE.Interop.IStream pstream)
    {
      byte[] bufferFromIstream = WindowPane.GetBufferFromIStream(pstream);
      if (bufferFromIstream.Length <= 0)
        return 0;
      using (MemoryStream memoryStream = new MemoryStream(bufferFromIstream))
        return this.LoadUIState((Stream) memoryStream);
    }

    public virtual int SaveUIState(out Stream stateStream)
    {
      stateStream = (Stream) null;
      return 0;
    }

    int IVsUIElementPane.SaveUIElementState(Microsoft.VisualStudio.OLE.Interop.IStream pstream)
    {
      Stream stateStream;
      int hr = this.SaveUIState(out stateStream);
      if (ErrorHandler.Succeeded(hr))
      {
        using (stateStream)
        {
          if (stateStream != null)
          {
            if (stateStream.CanRead)
            {
              if (stateStream.Length > 0L)
              {
                using (BinaryReader binaryReader = new BinaryReader(stateStream))
                {
                  byte[] numArray = new byte[stateStream.Length];
                  stateStream.Position = 0L;
                  binaryReader.Read(numArray, 0, numArray.Length);
                  uint pcbWritten = 0;
                  pstream.Write(numArray, (uint) numArray.Length, out pcbWritten);
                  pstream.Commit(0U);
                }
              }
            }
          }
        }
      }
      return hr;
    }

    private int InternalSetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider p)
    {
      if (this._provider != null)
      {
        this._provider.Dispose();
        this._provider = (ServiceProvider) null;
      }
      IObjectWithSite service1 = this.GetService(typeof (IObjectWithSite)) as IObjectWithSite;
      ServiceProviderHierarchy service2 = this.GetService(typeof (ServiceProviderHierarchy)) as ServiceProviderHierarchy;
      if (service2 != null)
      {
        ServiceProvider serviceProvider = p == null ? (ServiceProvider) null : new ServiceProvider(p);
        service2[50] = (System.IServiceProvider) serviceProvider;
      }
      else if (service1 != null)
        service1.SetSite((object) p);
      else if (p != null)
        this._provider = new ServiceProvider(p);
      if (p != null)
        this.Initialize();
      return 0;
    }

    int IVsUIElementPane.SetUIElementSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider p)
    {
      return ((IVsWindowPane) this).SetSite(p);
    }

    private int InternalTranslateAccelerator(Microsoft.VisualStudio.OLE.Interop.MSG[] msg)
    {
      Message m = Message.Create(msg[0].hwnd, (int) msg[0].message, msg[0].wParam, msg[0].lParam);
      bool flag = this.PreProcessMessage(ref m);
      msg[0].message = (uint) m.Msg;
      msg[0].wParam = m.WParam;
      msg[0].lParam = m.LParam;
      return flag ? 0 : -2147467259;
    }

    int IVsUIElementPane.TranslateUIElementAccelerator(Microsoft.VisualStudio.OLE.Interop.MSG[] msg)
    {
      return this.InternalTranslateAccelerator(msg);
    }

    [Obsolete("The IVsWindowPane interface on the WindowPane is obsolete, use IVsUIElementPane")]
    int IVsWindowPane.ClosePane()
    {
      this.OnClose();
      return 0;
    }

    [Obsolete("The IVsWindowPane interface on the WindowPane is obsolete, use IVsUIElementPane")]
    int IVsWindowPane.CreatePaneWindow(
      IntPtr hwndParent,
      int x,
      int y,
      int cx,
      int cy,
      out IntPtr hwnd)
    {
      if (this.InitializationMode != WindowPane.PaneInitializationMode.Uninitialized)
        throw new InvalidOperationException("The WindowPane is already initialized");
      this.InitializationMode = WindowPane.PaneInitializationMode.IVsWindowPane;
      this.OnCreate();
      hwnd = IntPtr.Zero;
      if (this.Content == null && this.Window == null)
        throw new InvalidOperationException("A WindowPane derived type must provide either a content control or a HWND.   If the tool is WPF based IVsUIElementPane.CreteUIElement should be used.");
      int num = 0;
      if (this.Content != null)
      {
        if (this.Content is FrameworkElement || this.Content is IVsUIWpfElement)
        {
          HwndSource hwndSource = new HwndSource(0, 1409286144, 0, 0, 0, "", hwndParent);
          hwndSource.SizeToContent = SizeToContent.Manual;
          if (this.Content is IVsUIWpfElement)
          {
            object ppUnkElement = (object) null;
            // ISSUE: reference to a compiler-generated method
            ((IVsUIWpfElement) this.Content).CreateFrameworkElement(out ppUnkElement);
            hwndSource.RootVisual = (Visual) ppUnkElement;
          }
          else
            hwndSource.RootVisual = (Visual) this.Content;
          hwndSource.Disposed += (EventHandler) ((sender, e) => this.Dispose());
          hwnd = hwndSource.Handle;
        }
        else if (this.Content is IVsUIWin32Element)
        {
          // ISSUE: reference to a compiler-generated method
          num = ((IVsUIWin32Element) this.Content).Create(hwndParent, out hwnd);
        }
      }
      else if (this.Window != null)
      {
        this.win32Wrapper = new UIWin32ElementWrapper(this);
        num = this.win32Wrapper.Create(hwndParent, out hwnd);
      }
      return num;
    }

    [Obsolete("The IVsWindowPane interface on the WindowPane is obsolete, use IVsUIElementPane")]
    int IVsWindowPane.GetDefaultSize(Microsoft.VisualStudio.OLE.Interop.SIZE[] pSize)
    {
      return -2147467263;
    }

    [Obsolete("The IVsWindowPane interface on the WindowPane is obsolete, use IVsUIElementPane")]
    int IVsWindowPane.LoadViewState(Microsoft.VisualStudio.OLE.Interop.IStream pStream)
    {
      return -2147467263;
    }

    [Obsolete("The IVsWindowPane interface on the WindowPane is obsolete, use IVsUIElementPane")]
    int IVsWindowPane.SaveViewState(Microsoft.VisualStudio.OLE.Interop.IStream pStream)
    {
      return -2147467263;
    }

    [Obsolete("The IVsWindowPane interface on the WindowPane is obsolete, use IVsUIElementPane")]
    int IVsWindowPane.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
    {
      return this.InternalSetSite(psp);
    }

    [Obsolete("The IVsWindowPane interface on the WindowPane is obsolete, use IVsUIElementPane")]
    int IVsWindowPane.TranslateAccelerator(Microsoft.VisualStudio.OLE.Interop.MSG[] lpmsg)
    {
      return this.InternalTranslateAccelerator(lpmsg);
    }

    protected enum PaneInitializationMode
    {
      Uninitialized,
      IVsWindowPane,
      IVsUIElementPane,
    }
  }
}
