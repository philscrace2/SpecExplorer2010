// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.OleMenuCommand
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Microsoft.VisualStudio.Shell
{
  [ComVisible(true)]
  public class OleMenuCommand : MenuCommand, IOleMenuCommand, IMenuCommandInvokeEx
  {
    private EventHandler execHandler;
    private EventHandler beforeQueryStatusHandler;
    private string text;
    private int matchedCommandId;
    private string parametersDescription;

    public OleMenuCommand(EventHandler invokeHandler, CommandID id)
      : base(invokeHandler, id)
    {
      this.PrivateInit(invokeHandler, (EventHandler) null, (EventHandler) null, string.Empty);
    }

    public OleMenuCommand(EventHandler invokeHandler, CommandID id, string Text)
      : base(invokeHandler, id)
    {
      this.PrivateInit(invokeHandler, (EventHandler) null, (EventHandler) null, Text);
    }

    public OleMenuCommand(EventHandler invokeHandler, EventHandler changeHandler, CommandID id)
      : base(invokeHandler, id)
    {
      this.PrivateInit(invokeHandler, changeHandler, (EventHandler) null, string.Empty);
    }

    public OleMenuCommand(
      EventHandler invokeHandler,
      EventHandler changeHandler,
      CommandID id,
      string Text)
      : base(invokeHandler, id)
    {
      this.PrivateInit(invokeHandler, changeHandler, (EventHandler) null, Text);
    }

    public OleMenuCommand(
      EventHandler invokeHandler,
      EventHandler changeHandler,
      EventHandler beforeQueryStatus,
      CommandID id)
      : base(invokeHandler, id)
    {
      this.PrivateInit(invokeHandler, changeHandler, beforeQueryStatus, string.Empty);
    }

    public OleMenuCommand(
      EventHandler invokeHandler,
      EventHandler changeHandler,
      EventHandler beforeQueryStatus,
      CommandID id,
      string Text)
      : base(invokeHandler, id)
    {
      this.PrivateInit(invokeHandler, changeHandler, beforeQueryStatus, Text);
    }

    private void PrivateInit(
      EventHandler handler,
      EventHandler changeHandler,
      EventHandler beforeQS,
      string Text)
    {
      this.execHandler = handler;
      if (changeHandler != null)
        this.CommandChanged += changeHandler;
      this.beforeQueryStatusHandler = beforeQS;
      this.text = Text;
      this.parametersDescription = (string) null;
    }

    public event EventHandler BeforeQueryStatus
    {
      add
      {
        this.beforeQueryStatusHandler += value;
      }
      remove
      {
        this.beforeQueryStatusHandler -= value;
      }
    }

    public override int OleStatus
    {
      [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")] get
      {
        if (this.beforeQueryStatusHandler != null)
          this.beforeQueryStatusHandler((object) this, EventArgs.Empty);
        return base.OleStatus;
      }
    }

    public string ParametersDescription
    {
      get
      {
        return this.parametersDescription;
      }
      set
      {
        this.parametersDescription = value;
      }
    }

    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    public override void Invoke(object inArg)
    {
      try
      {
        this.execHandler((object) this, (EventArgs) new OleMenuCmdEventArgs(inArg, Microsoft.VisualStudio.NativeMethods.InvalidIntPtr));
      }
      catch (CheckoutException ex)
      {
        if (CheckoutException.Canceled == ex)
          return;
        throw;
      }
    }

    [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
    public virtual void Invoke(object inArg, IntPtr outArg)
    {
      this.Invoke(inArg, outArg, OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT);
    }

    [CLSCompliant(false)]
    [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
    public virtual void Invoke(object inArg, IntPtr outArg, OLECMDEXECOPT options)
    {
      try
      {
        this.execHandler((object) this, (EventArgs) new OleMenuCmdEventArgs(inArg, outArg, options));
      }
      catch (CheckoutException ex)
      {
        if (CheckoutException.Canceled == ex)
          return;
        throw;
      }
    }

    public virtual string Text
    {
      get
      {
        return this.text;
      }
      set
      {
        if (!(this.text != value))
          return;
        this.text = value;
        this.OnCommandChanged(EventArgs.Empty);
      }
    }

    public virtual bool DynamicItemMatch(int cmdId)
    {
      return false;
    }

    public int MatchedCommandId
    {
      get
      {
        return this.matchedCommandId;
      }
      set
      {
        this.matchedCommandId = value;
      }
    }
  }
}
