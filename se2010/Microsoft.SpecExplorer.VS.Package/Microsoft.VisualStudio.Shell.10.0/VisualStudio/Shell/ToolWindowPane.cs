// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ToolWindowPane
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell
{
  [ComVisible(true)]
  public class ToolWindowPane : WindowPane
  {
    private string caption;
    private IVsWindowFrame frame;
    private Microsoft.VisualStudio.Shell.Package package;
    private CommandID toolBarCommandID;
    private IDropTarget toolBarDropTarget;
    private VSTWT_LOCATION toolBarLocation;
    private int bitmapResourceID;
    private int bitmapIndex;
    private Guid toolClsid;

    public ToolWindowPane()
    {
      this.toolClsid = Guid.Empty;
      this.bitmapIndex = -1;
      this.bitmapResourceID = -1;
      this.toolBarLocation = VSTWT_LOCATION.VSTWT_TOP;
    }

    protected ToolWindowPane(System.IServiceProvider provider)
      : base(provider)
    {
      this.toolClsid = Guid.Empty;
      this.bitmapIndex = -1;
      this.bitmapResourceID = -1;
      this.toolBarLocation = VSTWT_LOCATION.VSTWT_TOP;
    }

    public string Caption
    {
      get
      {
        return this.caption;
      }
      set
      {
        this.caption = value;
        if (this.frame == null)
          return;
        if (this.caption == null)
          return;
        try
        {
          this.frame.SetProperty(-3004, (object) this.caption);
        }
        catch (COMException ex)
        {
          int errorCode = ex.ErrorCode;
        }
      }
    }

    public object Frame
    {
      get
      {
        return (object) this.frame;
      }
      set
      {
        this.frame = (IVsWindowFrame) value;
        this.OnToolWindowCreated();
      }
    }

    public object Package
    {
      get
      {
        return (object) this.package;
      }
      set
      {
        if (this.frame != null || this.package != null)
          throw new NotSupportedException(Resources.ToolWindow_PackageOnlySetByCreator);
        this.package = (Microsoft.VisualStudio.Shell.Package) value;
      }
    }

    public CommandID ToolBar
    {
      get
      {
        return this.toolBarCommandID;
      }
      set
      {
        if (this.frame != null)
          throw new Exception(Resources.ToolWindow_TooLateToAddToolbar);
        this.toolBarCommandID = value;
      }
    }

    [CLSCompliant(false)]
    public IDropTarget ToolBarDropTarget
    {
      get
      {
        return this.toolBarDropTarget;
      }
      set
      {
        if (this.frame != null)
          throw new Exception(Resources.ToolWindow_TooLateToAddToolbar);
        this.toolBarDropTarget = value;
      }
    }

    public int ToolBarLocation
    {
      get
      {
        return (int) this.toolBarLocation;
      }
      set
      {
        if (this.frame != null)
          throw new Exception(Resources.ToolWindow_TooLateToAddToolbar);
        this.toolBarLocation = (VSTWT_LOCATION) value;
      }
    }

    public Guid ToolClsid
    {
      get
      {
        return this.toolClsid;
      }
      set
      {
        if (this.frame != null)
          throw new Exception(Resources.ToolWindow_TooLateToAddTool);
        this.toolClsid = value;
      }
    }

    public int BitmapResourceID
    {
      get
      {
        return this.bitmapResourceID;
      }
      set
      {
        this.bitmapResourceID = value;
        if (this.frame == null)
          return;
        if (this.bitmapResourceID == -1)
          return;
        try
        {
          this.frame.SetProperty(-5006, (object) this.bitmapResourceID);
        }
        catch (COMException ex)
        {
          int errorCode = ex.ErrorCode;
        }
      }
    }

    public int BitmapIndex
    {
      get
      {
        return this.bitmapIndex;
      }
      set
      {
        this.bitmapIndex = value;
        if (this.frame == null)
          return;
        if (this.bitmapIndex == -1)
          return;
        try
        {
          this.frame.SetProperty(-5007, (object) this.bitmapIndex);
        }
        catch (COMException ex)
        {
          int errorCode = ex.ErrorCode;
        }
      }
    }

    public virtual object GetIVsWindowPane()
    {
      return (object) this;
    }

    public virtual void OnToolWindowCreated()
    {
      this.Caption = this.caption;
      this.BitmapResourceID = this.bitmapResourceID;
      this.BitmapIndex = this.bitmapIndex;
    }

    public virtual void OnToolBarAdded()
    {
    }
  }
}
