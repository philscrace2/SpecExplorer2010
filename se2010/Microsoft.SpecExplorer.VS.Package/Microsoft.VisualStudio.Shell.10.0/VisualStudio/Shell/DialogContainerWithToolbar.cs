// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.DialogContainerWithToolbar
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public class DialogContainerWithToolbar : Form, IVsToolWindowToolbar, System.IServiceProvider, System.Windows.Forms.IMessageFilter
  {
    private DialogContainerWithToolbar.WindowPaneAdapter containedForm;
    private Size controlSize;
    private IVsToolWindowToolbarHost toolbarHost;
    private Microsoft.VisualStudio.OLE.Interop.RECT toolbarRect;
    private CommandID toolbarCommandId;
    private VSTWT_LOCATION toolbarLocation;
    private Microsoft.VisualStudio.OLE.Interop.IDropTarget toolbarDropTarget;
    private System.IServiceProvider provider;
    private OleMenuCommandService commandService;
    private uint commandTargetCookie;

    public DialogContainerWithToolbar(
      System.IServiceProvider sp,
      Control contained,
      Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget parentCommandTarget)
    {
      if (contained == null)
        throw new ArgumentNullException(nameof (contained));
      if (sp == null)
        throw new ArgumentNullException(nameof (sp));
      this.PrivateInit(sp, contained, parentCommandTarget);
    }

    public DialogContainerWithToolbar(System.IServiceProvider sp, Control contained)
    {
      if (contained == null)
        throw new ArgumentNullException(nameof (contained));
      if (sp == null)
        throw new ArgumentNullException(nameof (sp));
      Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget parentTarget = contained as Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;
      this.PrivateInit(sp, contained, parentTarget);
    }

    public DialogContainerWithToolbar(System.IServiceProvider sp)
    {
      if (sp == null)
        throw new ArgumentNullException(nameof (sp));
      this.PrivateInit(sp, (Control) null, (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget) null);
    }

    public DialogContainerWithToolbar()
    {
      this.PrivateInit((System.IServiceProvider) null, (Control) null, (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget) null);
    }

    private void RegisterCommandTarget()
    {
      if (this.provider == null)
        throw new InvalidOperationException();
      IVsRegisterPriorityCommandTarget service = (IVsRegisterPriorityCommandTarget) this.provider.GetService(typeof (SVsRegisterPriorityCommandTarget));
      if (service == null)
        return;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service.RegisterPriorityCommandTarget(0U, (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget) this.commandService, out this.commandTargetCookie));
    }

    private void PrivateInit(
      System.IServiceProvider sp,
      Control contained,
      Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget parentTarget)
    {
      this.provider = sp;
      this.commandTargetCookie = 0U;
      this.commandService = parentTarget != null ? new OleMenuCommandService(sp, parentTarget) : new OleMenuCommandService(sp);
      if (sp != null)
        this.RegisterCommandTarget();
      this.toolbarRect.left = 0;
      this.toolbarRect.top = 0;
      this.toolbarRect.right = 0;
      this.toolbarRect.bottom = 0;
      this.toolbarCommandId = (CommandID) null;
      this.toolbarLocation = VSTWT_LOCATION.VSTWT_TOP;
      if (contained == null)
      {
        this.containedForm = (DialogContainerWithToolbar.WindowPaneAdapter) null;
      }
      else
      {
        this.controlSize = contained.ClientSize;
        this.containedForm = new DialogContainerWithToolbar.WindowPaneAdapter(this, contained);
        this.Site = contained.Site;
        Form form = contained as Form;
        if (form != null)
        {
          this.AcceptButton = form.AcceptButton;
          this.AccessibleDefaultActionDescription = form.AccessibleDefaultActionDescription;
          this.AccessibleDescription = form.AccessibleDescription;
          this.AccessibleName = form.AccessibleName;
          this.AccessibleRole = form.AccessibleRole;
          this.AllowDrop = form.AllowDrop;
          this.AllowTransparency = form.AllowTransparency;
          this.AutoScaleDimensions = form.AutoScaleDimensions;
          this.AutoScaleMode = form.AutoScaleMode;
          this.AutoScroll = form.AutoScroll;
          this.AutoScrollMargin = form.AutoScrollMargin;
          this.AutoScrollMinSize = form.AutoScrollMinSize;
          this.AutoScrollPosition = form.AutoScrollPosition;
          this.BindingContext = form.BindingContext;
          this.Bounds = form.Bounds;
          this.CancelButton = form.CancelButton;
          this.ContextMenu = form.ContextMenu;
          this.ControlBox = form.ControlBox;
          this.Cursor = form.Cursor;
          this.DesktopBounds = form.DesktopBounds;
          this.DesktopLocation = form.DesktopLocation;
          this.Font = form.Font;
          this.FormBorderStyle = form.FormBorderStyle;
          this.Icon = form.Icon;
          this.IsAccessible = form.IsAccessible;
          this.MaximizeBox = form.MaximizeBox;
          this.MaximumSize = form.MaximumSize;
          this.Menu = form.Menu;
          this.MinimizeBox = form.MinimizeBox;
          this.MinimumSize = form.MinimumSize;
          this.Opacity = form.Opacity;
          this.Region = form.Region;
          this.RightToLeft = form.RightToLeft;
          this.ShowInTaskbar = form.ShowInTaskbar;
          this.SizeGripStyle = form.SizeGripStyle;
          this.StartPosition = form.StartPosition;
          this.Text = form.Text;
          this.TopLevel = form.TopLevel;
          this.TopMost = form.TopMost;
          this.TransparencyKey = form.TransparencyKey;
        }
      }
      this.HelpButton = true;
      this.Load += new EventHandler(this.FormLoad);
      this.Closing += new CancelEventHandler(this.OnClosing);
    }

    public void SetSite(System.IServiceProvider sp)
    {
      if (this.provider != null)
        throw new InvalidOperationException();
      this.provider = sp;
      this.RegisterCommandTarget();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.OnClosing((object) this, new CancelEventArgs());
      base.Dispose(disposing);
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
      if (this.toolbarHost != null)
      {
        this.toolbarHost.Close(0U);
        this.toolbarHost = (IVsToolWindowToolbarHost) null;
      }
      if (this.containedForm != null)
      {
        ((IVsWindowPane) this.containedForm).ClosePane();
        this.containedForm = (DialogContainerWithToolbar.WindowPaneAdapter) null;
      }
      if (this.commandTargetCookie != 0U && this.provider != null)
      {
        (this.GetService(typeof (SVsRegisterPriorityCommandTarget)) as IVsRegisterPriorityCommandTarget)?.UnregisterPriorityCommandTarget(this.commandTargetCookie);
        this.commandTargetCookie = 0U;
      }
      if (e == null)
        return;
      e.Cancel = false;
    }

    object System.IServiceProvider.GetService(System.Type serviceType)
    {
      if (serviceType.IsEquivalentTo(typeof (IVsToolWindowToolbar)))
        return (object) this;
      if (serviceType.IsEquivalentTo(typeof (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget)))
        return (object) this.commandService;
      if (serviceType.IsEquivalentTo(typeof (IVsToolWindowToolbarHost)) && this.ToolbarHost != null)
        return (object) this.ToolbarHost;
      return this.provider.GetService(serviceType);
    }

    public CommandID ToolbarID
    {
      get
      {
        return this.toolbarCommandId;
      }
      set
      {
        this.toolbarCommandId = value;
      }
    }

    public VSTWT_LOCATION ToolbarLocation
    {
      get
      {
        return this.toolbarLocation;
      }
      set
      {
        this.toolbarLocation = value;
      }
    }

    public Microsoft.VisualStudio.OLE.Interop.IDropTarget ToolbarDropTarget
    {
      get
      {
        return this.toolbarDropTarget;
      }
      set
      {
        this.toolbarDropTarget = value;
      }
    }

    public IVsToolWindowToolbarHost ToolbarHost
    {
      get
      {
        if (this.toolbarHost != null)
          return this.toolbarHost;
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(((IVsUIShell) this.provider.GetService(typeof (SVsUIShell))).SetupToolbar(this.Handle, (IVsToolWindowToolbar) this, out this.toolbarHost));
        return this.toolbarHost;
      }
    }

    public IMenuCommandService CommandService
    {
      get
      {
        return (IMenuCommandService) this.commandService;
      }
    }

    int IVsToolWindowToolbar.GetBorder(Microsoft.VisualStudio.OLE.Interop.RECT[] rect)
    {
      if (rect == null || rect.Length != 1)
        throw new ArgumentException(nameof (rect));
      rect[0].left = 0;
      rect[0].top = 0;
      rect[0].right = this.ClientSize.Width;
      rect[0].bottom = this.ClientSize.Height;
      return 0;
    }

    int IVsToolWindowToolbar.SetBorderSpace(Microsoft.VisualStudio.OLE.Interop.RECT[] rect)
    {
      if (rect == null || rect.Length != 1)
        throw new ArgumentException(nameof (rect));
      this.toolbarRect = rect[0];
      this.ResizePane();
      return 0;
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    bool System.Windows.Forms.IMessageFilter.PreFilterMessage(ref Message m)
    {
      if (this.ToolbarHost == null)
        return false;
      int plResult;
      int hr = this.ToolbarHost.ProcessMouseActivationModal(m.HWnd, (uint) m.Msg, (uint) (int) m.WParam, (int) m.LParam, out plResult);
      if (Microsoft.VisualStudio.NativeMethods.Failed(hr))
        return false;
      return hr == 1;
    }

    public new DialogResult ShowDialog()
    {
      if (this.provider == null)
        throw new InvalidOperationException();
      System.Windows.Forms.IMessageFilter messageFilter = (System.Windows.Forms.IMessageFilter) this;
      DialogContainerWithToolbar.ShowDialogContainer showDialogContainer = (DialogContainerWithToolbar.ShowDialogContainer) null;
      if (this.Site == null)
      {
        showDialogContainer = new DialogContainerWithToolbar.ShowDialogContainer((System.IServiceProvider) this);
        showDialogContainer.Add((IComponent) this);
      }
      try
      {
        Application.AddMessageFilter(messageFilter);
        return base.ShowDialog();
      }
      finally
      {
        showDialogContainer?.Remove((IComponent) this);
        Application.RemoveMessageFilter(messageFilter);
      }
    }

    private void ResizePane()
    {
      Size clientSize = this.ClientSize;
      int left = this.toolbarRect.left;
      int top = this.toolbarRect.top;
      int width = clientSize.Width - this.toolbarRect.left - this.toolbarRect.right;
      int height = clientSize.Height - this.toolbarRect.top - this.toolbarRect.bottom;
      this.containedForm.Move(left, top, height, width);
    }

    private void ResizeForm(object sender, EventArgs e)
    {
      this.ResizePane();
      if (this.ToolbarHost == null)
        return;
      this.ToolbarHost.BorderChanged();
    }

    private void FormLoad(object sender, EventArgs e)
    {
      if (this.DesignMode)
        return;
      if (this.containedForm == null)
      {
        Control control = (Control) new UserControl();
        while (this.Controls.Count > 0)
          this.Controls[0].Parent = control;
        this.containedForm = new DialogContainerWithToolbar.WindowPaneAdapter(this, control);
        this.controlSize = this.ClientSize;
      }
      Size clientSize = this.ClientSize;
      if (this.toolbarCommandId != null)
      {
        Guid guid = this.toolbarCommandId.Guid;
        // ISSUE: variable of a compiler-generated type
        IVsToolWindowToolbarHost2 toolbarHost = (IVsToolWindowToolbarHost2) this.ToolbarHost;
        // ISSUE: reference to a compiler-generated method
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(toolbarHost.AddToolbar2(this.toolbarLocation, ref guid, (uint) this.toolbarCommandId.ID, this.toolbarDropTarget));
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.ToolbarHost.Show(0U));
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.ToolbarHost.ForceUpdateUI());
      }
      clientSize.Width = this.controlSize.Width + this.toolbarRect.left + this.toolbarRect.right;
      clientSize.Height = this.controlSize.Height + this.toolbarRect.top + this.toolbarRect.bottom;
      this.ClientSize = clientSize;
      int left = this.toolbarRect.left;
      int top = this.toolbarRect.top;
      int width = clientSize.Width - this.toolbarRect.left - this.toolbarRect.right;
      int height = clientSize.Height - this.toolbarRect.top - this.toolbarRect.bottom;
      this.containedForm.Create(left, top, height, width);
      this.containedForm.Focus();
      this.Resize += new EventHandler(this.ResizeForm);
    }

    private class WindowPaneAdapter : WindowPane
    {
      private Control control;
      private DialogContainerWithToolbar container;
      private IntPtr paneHwnd;
      private int left;
      private int top;
      private int height;
      private int width;

      public WindowPaneAdapter(DialogContainerWithToolbar container, Control control)
        : base((System.IServiceProvider) container)
      {
        this.container = container;
        this.paneHwnd = IntPtr.Zero;
        this.control = control;
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing)
        {
          if (this.control != null)
          {
            this.control.Dispose();
            this.control = (Control) null;
          }
          this.paneHwnd = IntPtr.Zero;
        }
        base.Dispose(disposing);
      }

      public IntPtr Handle
      {
        get
        {
          return this.paneHwnd;
        }
      }

      public void Focus()
      {
        this.control.Focus();
      }

      public void Create(int left, int top, int height, int width)
      {
        if (IntPtr.Zero != this.paneHwnd)
          throw new InvalidOperationException();
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(((IVsWindowPane) this).CreatePaneWindow(this.container.Handle, left, top, width, height, out this.paneHwnd));
        this.left = left;
        this.top = top;
        this.height = height;
        this.width = width;
      }

      public override IWin32Window Window
      {
        get
        {
          return (IWin32Window) this.control;
        }
      }

      public void Move(int left, int top, int height, int width)
      {
        if (IntPtr.Zero == this.Handle)
          return;
        if (!Microsoft.VisualStudio.UnsafeNativeMethods.SetWindowPos(this.Handle, IntPtr.Zero, left, top, width, height, 20))
          throw new Exception();
        this.left = left;
        this.top = top;
        this.height = height;
        this.width = width;
      }

      public int Left
      {
        get
        {
          return this.left;
        }
      }

      public int Top
      {
        get
        {
          return this.top;
        }
      }

      public int Height
      {
        get
        {
          return this.height;
        }
      }

      public int Width
      {
        get
        {
          return this.width;
        }
      }
    }

    private class ShowDialogContainer : Container
    {
      private System.IServiceProvider provider;

      public ShowDialogContainer(System.IServiceProvider sp)
      {
        this.provider = sp;
      }

      protected override object GetService(System.Type serviceType)
      {
        if (this.provider != null)
        {
          object service = this.provider.GetService(serviceType);
          if (service != null)
            return service;
        }
        return base.GetService(serviceType);
      }
    }
  }
}
