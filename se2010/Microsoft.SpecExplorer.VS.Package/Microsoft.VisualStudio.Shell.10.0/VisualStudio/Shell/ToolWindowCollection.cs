// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ToolWindowCollection
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Shell
{
  internal sealed class ToolWindowCollection : IDisposable
  {
    private Dictionary<ToolWindowCollection.ToolWindowID, ToolWindowCollection.ToolWindowListener> windows;

    public ToolWindowCollection()
    {
      this.windows = new Dictionary<ToolWindowCollection.ToolWindowID, ToolWindowCollection.ToolWindowListener>();
    }

    public void Dispose()
    {
      if (this.windows == null)
        return;
      foreach (ToolWindowCollection.ToolWindowListener toolWindowListener in this.windows.Values)
        toolWindowListener.Dispose();
      this.windows = (Dictionary<ToolWindowCollection.ToolWindowID, ToolWindowCollection.ToolWindowListener>) null;
    }

    public void Add(Guid toolGuid, int toolId, ToolWindowPane pane)
    {
      ToolWindowCollection.ToolWindowID toolWindowId = new ToolWindowCollection.ToolWindowID(toolGuid, toolId);
      this.Remove(toolWindowId);
      ToolWindowCollection.ToolWindowListener toolWindowListener = new ToolWindowCollection.ToolWindowListener(pane, toolWindowId);
      toolWindowListener.OnFrameClosed += new ToolWindowCollection.FrameClosedHandler(this.OnFrameClosed);
      this.windows.Add(toolWindowId, toolWindowListener);
    }

    public bool Remove(Guid toolGuid, int toolId)
    {
      return this.Remove(new ToolWindowCollection.ToolWindowID(toolGuid, toolId));
    }

    public ToolWindowPane GetToolWindowPane(Guid toolGuid, int toolId)
    {
      ToolWindowCollection.ToolWindowID key = new ToolWindowCollection.ToolWindowID(toolGuid, toolId);
      ToolWindowCollection.ToolWindowListener toolWindowListener = (ToolWindowCollection.ToolWindowListener) null;
      if (!this.windows.TryGetValue(key, out toolWindowListener))
        return (ToolWindowPane) null;
      return toolWindowListener.WindowPane;
    }

    private void OnFrameClosed(ToolWindowCollection.ToolWindowID id)
    {
      this.Remove(id);
    }

    private bool Remove(ToolWindowCollection.ToolWindowID id)
    {
      ToolWindowCollection.ToolWindowListener toolWindowListener;
      if (!this.windows.TryGetValue(id, out toolWindowListener))
        return false;
      this.windows.Remove(id);
      toolWindowListener.Dispose();
      return true;
    }

    private struct ToolWindowID
    {
      private Guid toolGuid;
      private int toolID;

      public ToolWindowID(Guid guid, int id)
      {
        this.toolGuid = guid;
        this.toolID = id;
      }

      public static bool operator ==(
        ToolWindowCollection.ToolWindowID first,
        ToolWindowCollection.ToolWindowID second)
      {
        if ((object) first is null)
          return (object) second is null;
        return first.Equals((object) second);
      }

      public static bool operator !=(
        ToolWindowCollection.ToolWindowID first,
        ToolWindowCollection.ToolWindowID second)
      {
        return !(first == second);
      }

      public override bool Equals(object other)
      {
        if (!(other is ToolWindowCollection.ToolWindowID))
          return false;
        ToolWindowCollection.ToolWindowID toolWindowId = (ToolWindowCollection.ToolWindowID) other;
        if (this.toolGuid != toolWindowId.toolGuid)
          return false;
        return this.toolID == toolWindowId.toolID;
      }

      public override int GetHashCode()
      {
        return this.toolGuid.GetHashCode() ^ this.toolID;
      }
    }

    private delegate void FrameClosedHandler(ToolWindowCollection.ToolWindowID windowID);

    private class ToolWindowListener : IDisposable, IVsWindowFrameNotify, IVsWindowFrameNotify3
    {
      private ToolWindowPane window;
      private ToolWindowCollection.ToolWindowID id;
      private uint cookie;
      private ToolWindowCollection.FrameClosedHandler onCloseDelegate;

      public ToolWindowListener(
        ToolWindowPane windowPane,
        ToolWindowCollection.ToolWindowID windowID)
      {
        if (windowPane == null)
          throw new ArgumentNullException(nameof (windowPane));
        this.window = windowPane;
        this.id = windowID;
        this.SubscribeForEvents();
      }

      public void Dispose()
      {
        if (this.window == null)
          return;
        if (this.cookie != 0U)
          this.UnsubscribeForEvents();
        this.window = (ToolWindowPane) null;
      }

      public event ToolWindowCollection.FrameClosedHandler OnFrameClosed
      {
        add
        {
          this.onCloseDelegate += value;
        }
        remove
        {
          this.onCloseDelegate -= value;
        }
      }

      public ToolWindowPane WindowPane
      {
        get
        {
          return this.window;
        }
      }

      public ToolWindowCollection.ToolWindowID WindowId
      {
        get
        {
          return this.id;
        }
      }

      private void SubscribeForEvents()
      {
        if (this.window == null)
          throw new InvalidOperationException();
        IVsWindowFrame2 frame = this.window.Frame as IVsWindowFrame2;
        if (frame == null)
          throw new InvalidOperationException();
        if (this.cookie != 0U)
          throw new InvalidOperationException();
        ErrorHandler.ThrowOnFailure(frame.Advise((IVsWindowFrameNotify) this, out this.cookie));
      }

      private void UnsubscribeForEvents()
      {
        if (this.window == null || this.cookie == 0U)
          return;
        IVsWindowFrame2 frame = this.window.Frame as IVsWindowFrame2;
        if (frame == null)
          return;
        frame.Unadvise(this.cookie);
        this.cookie = 0U;
      }

      private void OnClose()
      {
        this.UnsubscribeForEvents();
        if (this.onCloseDelegate == null)
          return;
        this.onCloseDelegate(this.id);
      }

      int IVsWindowFrameNotify3.OnClose(ref uint pgrfSaveOptions)
      {
        return 0;
      }

      int IVsWindowFrameNotify3.OnDockableChange(
        int fDockable,
        int x,
        int y,
        int w,
        int h)
      {
        return 0;
      }

      int IVsWindowFrameNotify3.OnMove(int x, int y, int w, int h)
      {
        return 0;
      }

      int IVsWindowFrameNotify3.OnShow(int fShow)
      {
        if (this.window == null)
          return 0;
        switch (fShow)
        {
          case 7:
          case 8:
            this.OnClose();
            break;
        }
        return 0;
      }

      int IVsWindowFrameNotify3.OnSize(int x, int y, int w, int h)
      {
        return 0;
      }

      int IVsWindowFrameNotify.OnDockableChange(int fDockable)
      {
        return 0;
      }

      int IVsWindowFrameNotify.OnMove()
      {
        return 0;
      }

      int IVsWindowFrameNotify.OnShow(int fShow)
      {
        return ((IVsWindowFrameNotify3) this).OnShow(fShow);
      }

      int IVsWindowFrameNotify.OnSize()
      {
        return 0;
      }
    }
  }
}
