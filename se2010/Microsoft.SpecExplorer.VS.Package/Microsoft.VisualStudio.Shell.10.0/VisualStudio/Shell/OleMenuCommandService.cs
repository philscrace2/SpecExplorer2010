// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.OleMenuCommandService
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  [ComVisible(true)]
  public class OleMenuCommandService : MenuCommandService, IOleCommandTarget
  {
    internal new static TraceSwitch MENUSERVICE = new TraceSwitch(nameof (MENUSERVICE), "MenuCommandService: Track menu command routing");
    private static uint _queryStatusCount = 0;
    private IOleCommandTarget _parentTarget;
    private System.IServiceProvider _provider;

    public OleMenuCommandService(System.IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
      this._provider = serviceProvider;
    }

    public OleMenuCommandService(
      System.IServiceProvider serviceProvider,
      IOleCommandTarget parentCommandTarget)
      : base(serviceProvider)
    {
      if (parentCommandTarget == null)
        throw new ArgumentNullException(nameof (parentCommandTarget));
      this._parentTarget = parentCommandTarget;
      this._provider = serviceProvider;
    }

    [Obsolete("This method is obsolete and will be removed before the end of M3.2.  Use the proected GetService method instead.")]
    protected System.IServiceProvider ServiceProvider
    {
      get
      {
        return this._provider;
      }
    }

    public IOleCommandTarget ParentTarget
    {
      get
      {
        return this._parentTarget;
      }
      set
      {
        this._parentTarget = value;
      }
    }

    private MenuCommand FindCommand(Guid guid, int id, ref int hrReturn)
    {
      hrReturn = -2147221244;
      MenuCommand menuCommand1 = (MenuCommand) null;
      IMenuCommandService service = this.GetService(typeof (IMenuCommandService)) as IMenuCommandService;
      if (service != null)
        menuCommand1 = service.FindCommand(new CommandID(guid, id));
      if (menuCommand1 == null && this != service)
        menuCommand1 = this.FindCommand(guid, id);
      if (menuCommand1 == null)
      {
        ICollection commandList = this.GetCommandList(guid);
        if (commandList != null)
        {
          hrReturn = -2147221248;
          foreach (MenuCommand menuCommand2 in (IEnumerable) commandList)
          {
            IOleMenuCommand oleMenuCommand = menuCommand2 as IOleMenuCommand;
            if (oleMenuCommand != null && oleMenuCommand.DynamicItemMatch(id))
            {
              hrReturn = 0;
              menuCommand1 = menuCommand2;
            }
          }
        }
      }
      else
        hrReturn = 0;
      return menuCommand1;
    }

    public override bool GlobalInvoke(CommandID commandID)
    {
      if (base.GlobalInvoke(commandID))
        return true;
      IVsUIShell service = this.GetService(typeof (SVsUIShell)) as IVsUIShell;
      if (service == null)
        return false;
      object pvaIn = (object) null;
      Guid guid = commandID.Guid;
      return !Microsoft.VisualStudio.NativeMethods.Failed(service.PostExecCommand(ref guid, (uint) commandID.ID, 0U, ref pvaIn));
    }

    public override bool GlobalInvoke(CommandID commandID, object arg)
    {
      if (base.GlobalInvoke(commandID, arg))
        return true;
      IVsUIShell service = this.GetService(typeof (SVsUIShell)) as IVsUIShell;
      if (service == null)
        return false;
      object pvaIn = arg;
      Guid guid = commandID.Guid;
      return !Microsoft.VisualStudio.NativeMethods.Failed(service.PostExecCommand(ref guid, (uint) commandID.ID, 0U, ref pvaIn));
    }

    protected override void OnCommandsChanged(MenuCommandsChangedEventArgs e)
    {
      base.OnCommandsChanged(e);
      if (OleMenuCommandService._queryStatusCount != 0U)
        return;
      IVsUIShell service = this.GetService(typeof (SVsUIShell)) as IVsUIShell;
      if (service == null)
        return;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service.UpdateCommandUI(0));
    }

    public override void ShowContextMenu(CommandID menuID, int x, int y)
    {
      IOleComponentUIManager service = this.GetService(typeof (Microsoft.VisualStudio.NativeMethods.OleComponentUIManager)) as IOleComponentUIManager;
      if (service == null)
        return;
      POINTS[] pos = new POINTS[1]{ new POINTS() };
      pos[0].x = (short) x;
      pos[0].y = (short) y;
      Guid guid = menuID.Guid;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service.ShowContextMenu(0U, ref guid, menuID.ID, pos, (IOleCommandTarget) this));
    }

    private uint HiWord(uint val)
    {
      return val >> 16 & (uint) ushort.MaxValue;
    }

    private uint LoWord(uint val)
    {
      return val & (uint) ushort.MaxValue;
    }

    int IOleCommandTarget.Exec(
      ref Guid guidGroup,
      uint nCmdId,
      uint nCmdExcept,
      IntPtr pIn,
      IntPtr vOut)
    {
      Guid empty = Guid.Empty;
      Guid pguidCmdGroup;
      try
      {
        pguidCmdGroup = guidGroup;
      }
      catch (NullReferenceException ex)
      {
        return -2147221248;
      }
      int hrReturn = 0;
      MenuCommand command = this.FindCommand(pguidCmdGroup, (int) nCmdId, ref hrReturn);
      if ((command == null || !command.Supported) && this._parentTarget != null)
        return this._parentTarget.Exec(ref pguidCmdGroup, nCmdId, nCmdExcept, pIn, vOut);
      if (command != null)
      {
        IOleMenuCommand oleMenuCommand = command as IOleMenuCommand;
        uint num = this.LoWord(nCmdExcept);
        if (3U == num && oleMenuCommand == null)
          return 0;
        object inArg = (object) null;
        if (pIn != IntPtr.Zero)
          inArg = Marshal.GetObjectForNativeVariant(pIn);
        if (oleMenuCommand == null)
        {
          command.Invoke(inArg);
        }
        else
        {
          switch (num)
          {
            case 0:
            case 1:
            case 2:
              IMenuCommandInvokeEx menuCommandInvokeEx = oleMenuCommand as IMenuCommandInvokeEx;
              if (menuCommandInvokeEx != null)
              {
                menuCommandInvokeEx.Invoke(inArg, vOut, (OLECMDEXECOPT) num);
                break;
              }
              oleMenuCommand.Invoke(inArg, vOut);
              break;
            case 3:
              if (1U == this.HiWord(nCmdExcept) && IntPtr.Zero != vOut && !string.IsNullOrEmpty(oleMenuCommand.ParametersDescription))
              {
                Marshal.GetNativeVariantForObject((object) oleMenuCommand.ParametersDescription, vOut);
                break;
              }
              break;
          }
        }
      }
      return hrReturn;
    }

    int IOleCommandTarget.QueryStatus(
      ref Guid guidGroup,
      uint nCmdId,
      OLECMD[] oleCmd,
      IntPtr oleText)
    {
      Guid empty = Guid.Empty;
      Guid pguidCmdGroup;
      try
      {
        pguidCmdGroup = guidGroup;
      }
      catch (NullReferenceException ex)
      {
        return -2147221248;
      }
      ++OleMenuCommandService._queryStatusCount;
      int hrReturn = 0;
      try
      {
        for (uint index = 0; (long) index < (long) oleCmd.Length; ++index)
        {
          if (Microsoft.VisualStudio.NativeMethods.Succeeded(hrReturn))
          {
            MenuCommand command = this.FindCommand(pguidCmdGroup, (int) oleCmd[(IntPtr) index].cmdID, ref hrReturn);
            oleCmd[(IntPtr) index].cmdf = 0U;
            if (command != null && Microsoft.VisualStudio.NativeMethods.Succeeded(hrReturn))
              oleCmd[(IntPtr) index].cmdf = (uint) command.OleStatus;
            if (((int) oleCmd[(IntPtr) index].cmdf & 1) != 0)
            {
              if (IntPtr.Zero != oleText && Microsoft.VisualStudio.NativeMethods.OLECMDTEXT.GetFlags(oleText) == Microsoft.VisualStudio.NativeMethods.OLECMDTEXT.OLECMDTEXTF.OLECMDTEXTF_NAME)
              {
                string text = (string) null;
                if (command is DesignerVerb)
                  text = ((DesignerVerb) command).Text;
                else if (command is IOleMenuCommand)
                  text = ((IOleMenuCommand) command).Text;
                if (text != null)
                  Microsoft.VisualStudio.NativeMethods.OLECMDTEXT.SetText(oleText, text);
              }
            }
            else if (this._parentTarget != null)
            {
              OLECMD[] prgCmds = new OLECMD[1]
              {
                oleCmd[(IntPtr) index]
              };
              hrReturn = this._parentTarget.QueryStatus(ref pguidCmdGroup, 1U, prgCmds, oleText);
              oleCmd[(IntPtr) index] = prgCmds[0];
            }
            if (oleCmd[(IntPtr) index].cmdf == 0U)
              hrReturn = -2147221248;
          }
          else
            break;
        }
      }
      finally
      {
        if (0U < OleMenuCommandService._queryStatusCount)
          --OleMenuCommandService._queryStatusCount;
      }
      return hrReturn;
    }
  }
}
