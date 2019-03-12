// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.OleComponentSupport.OleComponent
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.PlatformUI.OleComponentSupport
{
  [CLSCompliant(false)]
  public class OleComponent : DisposableObject, IOleComponent
  {
    private const uint NeedsPeriodicIdleFlag = 2;
    private IOleComponentManager manager;
    private uint rawComponentFlags;
    private uint idleTimeInterval;
    private uint componentCookie;
    private uint componentNotifications;
    private bool isTrackingComponent;

    public static OleComponent CreateHostedComponent()
    {
      return new OleComponent(OleComponentManager.Instance);
    }

    public OleComponent(IOleComponentManager manager)
    {
      if (manager == null)
        throw new ArgumentNullException(nameof (manager));
      OLECRINFO olecrinfo = new OLECRINFO()
      {
        grfcrf = 0,
        grfcadvf = 0,
        uIdleTimeInterval = 0
      };
      olecrinfo.cbSize = (uint) Marshal.SizeOf((object) olecrinfo);
      if (manager.FRegisterComponent((IOleComponent) this, new OLECRINFO[1]
      {
        olecrinfo
      }, out this.componentCookie) != 1)
        throw new ComponentRegistrationFailedException();
      this.manager = manager;
    }

    public uint PeriodicIdleTimePeriod
    {
      get
      {
        this.ThrowIfDisposed();
        return this.idleTimeInterval;
      }
      set
      {
        this.ThrowIfDisposed();
        this.idleTimeInterval = value;
        if (this.idleTimeInterval != 0U)
          this.SetComponentRegistrationFlag(2U);
        else
          this.ClearComponentRegistrationFlag(2U);
        this.UpdateComponentRegistration();
      }
    }

    public bool IsTrackingComponent
    {
      get
      {
        this.ThrowIfDisposed();
        return this.isTrackingComponent;
      }
      private set
      {
        this.ThrowIfDisposed();
        if (this.isTrackingComponent == value || this.manager.FSetTrackingComponent(this.componentCookie, value ? 1 : 0) != 1)
          return;
        this.isTrackingComponent = value;
      }
    }

    public int PushMessageLoop(_OLELOOP reason, IntPtr pvLoopData)
    {
      return OleComponentManager.Instance.FPushMessageLoop(this.componentCookie, (uint) reason, pvLoopData);
    }

    public void BeginTracking()
    {
      this.IsTrackingComponent = true;
    }

    public void EndTracking()
    {
      this.IsTrackingComponent = false;
    }

    public static void DoOleEvents(IOleComponentManager manager, Func<bool> continuePumping)
    {
      if (manager == null)
        throw new ArgumentNullException(nameof (manager));
      if (continuePumping == null)
        continuePumping = (Func<bool>) (() => true);
      using (OleComponent oleComponent = new OleComponent(manager))
      {
        oleComponent.ContinueMessageLoop += (EventHandler<ContinueMessageLoopEventArgs>) ((sender, args) => args.ContinuePumping = continuePumping());
        manager.FPushMessageLoop(oleComponent.componentCookie, 2U, IntPtr.Zero);
      }
    }

    int IOleComponent.FContinueMessageLoop(
      uint uReason,
      IntPtr pvLoopData,
      MSG[] pMsgPeeked)
    {
      this.ThrowIfDisposed();
      return !this.FContinueMessageLoopCore(uReason, pvLoopData, pMsgPeeked) ? 0 : 1;
    }

    int IOleComponent.FDoIdle(uint grfidlef)
    {
      this.ThrowIfDisposed();
      return !this.FDoIdleCore(grfidlef) ? 0 : 1;
    }

    int IOleComponent.FPreTranslateMessage(MSG[] pMsg)
    {
      this.ThrowIfDisposed();
      return !this.FPreTranslateMessageCore(pMsg) ? 0 : 1;
    }

    int IOleComponent.FQueryTerminate(int fPromptUser)
    {
      this.ThrowIfDisposed();
      return !this.FQueryTerminateCore(fPromptUser) ? 0 : 1;
    }

    int IOleComponent.FReserved1(
      uint dwReserved,
      uint message,
      IntPtr wParam,
      IntPtr lParam)
    {
      this.ThrowIfDisposed();
      return 0;
    }

    IntPtr IOleComponent.HwndGetWindow(uint dwWhich, uint dwReserved)
    {
      this.ThrowIfDisposed();
      return this.HwndGetWindowCore(dwWhich, dwReserved);
    }

    void IOleComponent.OnActivationChange(
      IOleComponent pic,
      int fSameComponent,
      OLECRINFO[] pcrinfo,
      int fHostIsActivating,
      OLECHOSTINFO[] pchostinfo,
      uint dwReserved)
    {
      this.ThrowIfDisposed();
      this.OnActivationChangeCore(pic, fSameComponent, pcrinfo, fHostIsActivating, pchostinfo, dwReserved);
    }

    void IOleComponent.OnAppActivate(int fActive, uint dwOtherThreadID)
    {
      this.ThrowIfDisposed();
      this.OnAppActivateCore(fActive, dwOtherThreadID);
    }

    void IOleComponent.OnEnterState(uint uStateID, int fEnter)
    {
      this.ThrowIfDisposed();
      this.OnChangeStateCore(uStateID, fEnter);
    }

    void IOleComponent.OnLoseActivation()
    {
      this.ThrowIfDisposed();
      this.OnLoseActivationCore();
    }

    void IOleComponent.Terminate()
    {
      this.ThrowIfDisposed();
      this.TerminateCore();
    }

    protected virtual bool FContinueMessageLoopCore(
      uint uReason,
      IntPtr pvLoopData,
      MSG[] pMsgPeeked)
    {
      EventHandler<ContinueMessageLoopEventArgs> continueMessageLoop = this.ContinueMessageLoop;
      if (continueMessageLoop == null)
        return true;
      if (pMsgPeeked != null && pMsgPeeked.Length != 1)
        throw new ArgumentException("A MSG[] with more than one MSG[] object in it was given to FContinueMessageLoopCore.");
      ContinueMessageLoopEventArgs e = new ContinueMessageLoopEventArgs(uReason, pvLoopData, pMsgPeeked?[0]);
      continueMessageLoop((object) this, e);
      return e.ContinuePumping;
    }

    protected virtual bool FDoIdleCore(uint grfidlef)
    {
      EventHandler<DoIdleEventArgs> periodicIdleEvent = this.doPeriodicIdleEvent;
      EventHandler<DoIdleEventArgs> doIdleEvent = this.doIdleEvent;
      DoIdleEventArgs e = new DoIdleEventArgs(grfidlef);
      bool flag1 = OleComponent.ContainsFlag(grfidlef, 1U);
      bool flag2 = OleComponent.ContainsFlag(grfidlef, 2U);
      bool flag3 = false;
      if (periodicIdleEvent != null && flag1)
      {
        periodicIdleEvent((object) this, e);
        flag3 = e.MoreTimeNeededForIdleTasks;
      }
      if (!flag3 && doIdleEvent != null && flag2)
      {
        doIdleEvent((object) this, e);
        flag3 = e.MoreTimeNeededForIdleTasks;
      }
      return flag3;
    }

    protected virtual bool FPreTranslateMessageCore(MSG[] pMsg)
    {
      EventHandler<PreTranslateMessageEventArgs> translateMessageEvent = this.preTranslateMessageEvent;
      if (translateMessageEvent == null)
        return false;
      if (pMsg == null)
        throw new ArgumentNullException(nameof (pMsg));
      if (pMsg.Length != 1)
        throw new ArgumentException("MSG[] given to FPreTranslateMessageCore which had more than one MSG in it.");
      PreTranslateMessageEventArgs e = new PreTranslateMessageEventArgs(pMsg[0]);
      Delegate[] invocationList = translateMessageEvent.GetInvocationList();
      for (int index = invocationList.Length - 1; index >= 0; --index)
        ((EventHandler<PreTranslateMessageEventArgs>) invocationList[index])((object) this, e);
      return e.MessageConsumed;
    }

    protected virtual bool FQueryTerminateCore(int fPromptUser)
    {
      EventHandler<QueryTerminateEventArgs> queryTerminate = this.QueryTerminate;
      if (queryTerminate == null)
        return true;
      QueryTerminateEventArgs e = new QueryTerminateEventArgs(fPromptUser != 0);
      queryTerminate((object) this, e);
      return e.CanTerminate;
    }

    protected virtual IntPtr HwndGetWindowCore(uint dwWhich, uint dwReserved)
    {
      EventHandler<GetWindowEventArgs> getWindow = this.GetWindow;
      if (getWindow == null)
        return IntPtr.Zero;
      GetWindowEventArgs e = new GetWindowEventArgs((WindowType) dwWhich);
      getWindow((object) this, e);
      return e.WindowHandle;
    }

    protected virtual void OnActivationChangeCore(
      IOleComponent pic,
      int fSameComponent,
      OLECRINFO[] pcrinfo,
      int fHostIsActivating,
      OLECHOSTINFO[] pchostinfo,
      uint dwReserved)
    {
      EventHandler<ActivationChangeEventArgs> activationChangeEvent = this.activationChangeEvent;
      if (activationChangeEvent == null)
        return;
      if (pcrinfo != null && pcrinfo.Length != 1)
        throw new ArgumentException("A OLECRINFO[] was given to OnActivationChangeCore that had more than one OLECRINFO object in it.");
      if (pchostinfo != null && pchostinfo.Length != 1)
        throw new ArgumentException("A OLECHOSTINFO[] was given to OnActivationChangeCore that had more than one OLECHOSTINFO object in it.");
      activationChangeEvent((object) this, new ActivationChangeEventArgs(pic, fSameComponent != 0, pcrinfo?[0], fHostIsActivating != 0, pchostinfo?[0]));
    }

    protected virtual void OnAppActivateCore(int fActive, uint dwOtherThreadID)
    {
      EventHandler<AppActivateEventArgs> appActivateEvent = this.appActivateEvent;
      if (appActivateEvent == null)
        return;
      AppActivateEventArgs e = new AppActivateEventArgs(fActive != 0, dwOtherThreadID);
      appActivateEvent((object) this, e);
    }

    protected virtual void OnChangeStateCore(uint uStateID, int fEnter)
    {
      EventHandler<StateChangedEventArgs> eventHandler;
      switch (uStateID)
      {
        case 1:
          eventHandler = this.modalStateChangeEvent;
          break;
        case 2:
          eventHandler = this.redrawOffStateChangedEvent;
          break;
        case 3:
          eventHandler = this.warningsOffStateChangedEvent;
          break;
        case 4:
          eventHandler = this.recordingStateChangedEvent;
          break;
        default:
          return;
      }
      if (eventHandler == null)
        return;
      StateChangedEventArgs e = new StateChangedEventArgs(fEnter);
      eventHandler((object) this, e);
    }

    protected virtual void OnLoseActivationCore()
    {
      EventHandler<EventArgs> activationLostEvent = this.activationLostEvent;
      if (activationLostEvent == null)
        return;
      activationLostEvent((object) this, EventArgs.Empty);
    }

    protected virtual void TerminateCore()
    {
      EventHandler<EventArgs> terminate = this.Terminate;
      if (terminate != null)
        terminate((object) this, EventArgs.Empty);
      this.Dispose();
    }

    public event EventHandler<ContinueMessageLoopEventArgs> ContinueMessageLoop;

    private event EventHandler<DoIdleEventArgs> doIdleEvent;

    public event EventHandler<DoIdleEventArgs> DoIdle
    {
      add
      {
        this.UpdateStateOnAdd(new Func<uint, bool>(this.ComponentContainsRegistrationFlag), new Action<uint>(this.SetComponentRegistrationFlag), 1U);
        this.doIdleEvent += value;
      }
      remove
      {
        this.ThrowIfDisposed();
        this.doIdleEvent -= value;
        this.UpdateStateOnRemove<DoIdleEventArgs>(this.doIdleEvent, new Action<uint>(this.ClearComponentRegistrationFlag), 1U);
      }
    }

    private event EventHandler<DoIdleEventArgs> doPeriodicIdleEvent;

    public event EventHandler<DoIdleEventArgs> DoPeriodicIdle
    {
      add
      {
        this.ThrowIfDisposed();
        if (this.PeriodicIdleTimePeriod == 0U)
          this.idleTimeInterval = 500U;
        this.UpdateStateOnAdd(new Func<uint, bool>(this.ComponentContainsRegistrationFlag), new Action<uint>(this.SetComponentRegistrationFlag), 2U);
        this.doPeriodicIdleEvent += value;
      }
      remove
      {
        this.ThrowIfDisposed();
        this.doPeriodicIdleEvent -= value;
        if (this.doPeriodicIdleEvent != null)
          return;
        this.ClearComponentRegistrationFlag(2U);
        this.idleTimeInterval = 0U;
        this.UpdateComponentRegistration();
      }
    }

    private event EventHandler<PreTranslateMessageEventArgs> preTranslateMessageEvent;

    public event EventHandler<PreTranslateMessageEventArgs> PreTranslateMessage
    {
      add
      {
        this.UpdateStateOnAdd(new Func<uint, bool>(this.ComponentContainsRegistrationFlag), new Action<uint>(this.SetComponentRegistrationFlag), 8U);
        this.preTranslateMessageEvent += value;
      }
      remove
      {
        this.ThrowIfDisposed();
        this.preTranslateMessageEvent -= value;
        this.UpdateStateOnRemove<PreTranslateMessageEventArgs>(this.preTranslateMessageEvent, new Action<uint>(this.ClearComponentRegistrationFlag), 8U);
      }
    }

    public event EventHandler<QueryTerminateEventArgs> QueryTerminate;

    public event EventHandler<GetWindowEventArgs> GetWindow;

    private event EventHandler<ActivationChangeEventArgs> activationChangeEvent;

    public event EventHandler<ActivationChangeEventArgs> ActivationChange
    {
      add
      {
        this.EnsureAppActivationFlagsRegistered();
        this.activationChangeEvent += value;
      }
      remove
      {
        this.activationChangeEvent -= value;
        if (this.activationChangeEvent != null)
          return;
        this.UnregisterAppActivate();
      }
    }

    private event EventHandler<AppActivateEventArgs> appActivateEvent;

    public event EventHandler<AppActivateEventArgs> AppActivate
    {
      add
      {
        this.EnsureAppActivationFlagsRegistered();
        this.appActivateEvent += value;
      }
      remove
      {
        this.appActivateEvent -= value;
        if (this.appActivateEvent != null)
          return;
        this.UnregisterAppActivate();
      }
    }

    private event EventHandler<StateChangedEventArgs> modalStateChangeEvent;

    public event EventHandler<StateChangedEventArgs> ModalStateChanged
    {
      add
      {
        this.UpdateStateOnAdd(new Func<uint, bool>(this.ComponentContainsNotificationFlag), new Action<uint>(this.SetComponentNotificationFlag), 1U);
        this.modalStateChangeEvent += value;
      }
      remove
      {
        this.ThrowIfDisposed();
        this.modalStateChangeEvent -= value;
        this.UpdateStateOnRemove<StateChangedEventArgs>(this.modalStateChangeEvent, new Action<uint>(this.ClearComponentNotificationFlag), 1U);
      }
    }

    private event EventHandler<StateChangedEventArgs> recordingStateChangedEvent;

    public event EventHandler<StateChangedEventArgs> RecordingStateChanged
    {
      add
      {
        this.UpdateStateOnAdd(new Func<uint, bool>(this.ComponentContainsNotificationFlag), new Action<uint>(this.SetComponentNotificationFlag), 8U);
        this.recordingStateChangedEvent += value;
      }
      remove
      {
        this.ThrowIfDisposed();
        this.recordingStateChangedEvent -= value;
        this.UpdateStateOnRemove<StateChangedEventArgs>(this.recordingStateChangedEvent, new Action<uint>(this.ClearComponentNotificationFlag), 8U);
      }
    }

    private event EventHandler<StateChangedEventArgs> warningsOffStateChangedEvent;

    public event EventHandler<StateChangedEventArgs> WarningsOffStateChanged
    {
      add
      {
        this.UpdateStateOnAdd(new Func<uint, bool>(this.ComponentContainsNotificationFlag), new Action<uint>(this.SetComponentNotificationFlag), 4U);
        this.warningsOffStateChangedEvent += value;
      }
      remove
      {
        this.ThrowIfDisposed();
        this.warningsOffStateChangedEvent -= value;
        this.UpdateStateOnRemove<StateChangedEventArgs>(this.warningsOffStateChangedEvent, new Action<uint>(this.ClearComponentNotificationFlag), 4U);
      }
    }

    private event EventHandler<StateChangedEventArgs> redrawOffStateChangedEvent;

    public event EventHandler<StateChangedEventArgs> RedrawOffStateChanged
    {
      add
      {
        this.UpdateStateOnAdd(new Func<uint, bool>(this.ComponentContainsNotificationFlag), new Action<uint>(this.SetComponentNotificationFlag), 2U);
        this.redrawOffStateChangedEvent += value;
      }
      remove
      {
        this.ThrowIfDisposed();
        this.redrawOffStateChangedEvent -= value;
        this.UpdateStateOnRemove<StateChangedEventArgs>(this.redrawOffStateChangedEvent, new Action<uint>(this.ClearComponentNotificationFlag), 2U);
      }
    }

    private event EventHandler<EventArgs> activationLostEvent;

    public event EventHandler<EventArgs> ActivationLost
    {
      add
      {
        this.ThrowIfDisposed();
        this.EnsureAppActivationFlagsRegistered();
        this.activationLostEvent += value;
      }
      remove
      {
        this.ThrowIfDisposed();
        this.activationLostEvent -= value;
        if (this.appActivateEvent != null)
          return;
        this.UnregisterAppActivate();
      }
    }

    public event EventHandler<EventArgs> Terminate;

    protected override void DisposeManagedResources()
    {
      if (this.manager != null && this.componentCookie != 0U && this.manager.FRevokeComponent(this.componentCookie) != 1)
        throw new ComponentRevocationFailedException();
      this.manager = (IOleComponentManager) null;
    }

    private void UpdateComponentRegistration()
    {
      OLECRINFO olecrinfo = new OLECRINFO()
      {
        grfcrf = this.rawComponentFlags,
        grfcadvf = this.componentNotifications,
        uIdleTimeInterval = this.idleTimeInterval
      };
      olecrinfo.cbSize = (uint) Marshal.SizeOf((object) olecrinfo);
      if (this.manager.FUpdateComponentRegistration(this.componentCookie, new OLECRINFO[1]
      {
        olecrinfo
      }) != 1)
        throw new ComponentRegistrationFailedException("Failed to update component registration in UpdateComponentRegistration.");
    }

    private static bool ContainsFlag(uint flags, uint rawFlag)
    {
      return ((int) flags & (int) rawFlag) == (int) rawFlag;
    }

    private bool ComponentContainsNotificationFlag(uint rawFlag)
    {
      return OleComponent.ContainsFlag(this.componentNotifications, rawFlag);
    }

    private bool ComponentContainsRegistrationFlag(uint rawFlag)
    {
      return OleComponent.ContainsFlag(this.rawComponentFlags, rawFlag);
    }

    private void SetComponentRegistrationFlag(uint newFlag)
    {
      this.rawComponentFlags = OleComponent.SetFlag(this.rawComponentFlags, newFlag);
    }

    private void ClearComponentRegistrationFlag(uint flagToClear)
    {
      this.rawComponentFlags = OleComponent.ClearFlag(this.rawComponentFlags, flagToClear);
    }

    private void SetComponentNotificationFlag(uint newFlag)
    {
      this.componentNotifications = OleComponent.SetFlag(this.componentNotifications, newFlag);
    }

    private void ClearComponentNotificationFlag(uint flagToClear)
    {
      this.componentNotifications = OleComponent.ClearFlag(this.componentNotifications, flagToClear);
    }

    private static uint SetFlag(uint originalFlags, uint newFlags)
    {
      return originalFlags | newFlags;
    }

    private static uint ClearFlag(uint originalFlags, uint newFlags)
    {
      return originalFlags & ~newFlags;
    }

    private void EnsureAppActivationFlagsRegistered()
    {
      if (this.ComponentContainsRegistrationFlag(32U))
        return;
      this.RegisterAppActivate();
    }

    private void RegisterAppActivate()
    {
      this.SetComponentRegistrationFlag(32U);
      this.UpdateComponentRegistration();
    }

    private void UnregisterAppActivate()
    {
      this.ClearComponentRegistrationFlag(32U);
      this.UpdateComponentRegistration();
    }

    private void UpdateStateOnAdd(Func<uint, bool> checkFlag, Action<uint> setFlag, uint flag)
    {
      this.ThrowIfDisposed();
      if (checkFlag(flag))
        return;
      setFlag(flag);
      this.UpdateComponentRegistration();
    }

    private void UpdateStateOnRemove<T>(
      EventHandler<T> listenerChain,
      Action<uint> clearFlag,
      uint flag)
      where T : EventArgs
    {
      if (listenerChain != null)
        return;
      clearFlag(flag);
      this.UpdateComponentRegistration();
    }
  }
}
