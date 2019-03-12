// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.CommonMessagePump
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.Internal.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public class CommonMessagePump : IVsCommonMessagePump, IOleComponent2Private, IOleComponent
  {
    private bool m_hasInfiniteTimeout = true;
    private int m_signaledHandleIndex = -1;
    private string m_waitText = string.Empty;
    private string m_waitTitle = string.Empty;
    private string m_statusBarText = string.Empty;
    private string m_progressText = string.Empty;
    private const uint INFINITE = 4294967295;
    private const int DELAY_TO_SHOW_WAIT_DIALOG = 2;
    private const int WAIT_OBJECT_0 = 0;
    private const int WAIT_TIMEOUT = 258;
    private const int MAXIMUM_WAIT_OBJECTS = 64;
    private System.IServiceProvider m_serviceProvider;
    private IOleComponentManager m_componentManager;
    private IVsThreadedWaitDialog2 m_waitDialog;
    private IVsCommonMessagePumpClientEvents m_client;
    private List<IntPtr> m_handles;
    private DateTime m_endTime;
    private TimeSpan m_originalTimeout;
    private bool m_allowCancel;
    private bool m_enableRealProgress;
    private int m_totalSteps;
    private int m_currentStep;
    private uint m_compId;

    public CommonMessagePump()
    {
      this.m_serviceProvider = (System.IServiceProvider) ServiceProvider.GlobalProvider;
      this.m_waitText = Resources.CommonMessagePumpDefaultWaitText;
      this.m_statusBarText = Resources.CommonMessagePumpDefaultWaitStatusBarText;
    }

    private CommonMessagePumpExitCode ModalWaitForHandles(
      IntPtr[] handles,
      IVsCommonMessagePumpClientEvents client)
    {
      if (handles == null)
        throw new ArgumentNullException(nameof (handles));
      if (handles.GetLength(0) == 0 || 64 < handles.GetLength(0))
        throw new ArgumentOutOfRangeException(nameof (handles));
      if (this.m_compId != 0U)
        Marshal.ThrowExceptionForHR(-2147418113);
      this.m_handles = new List<IntPtr>((IEnumerable<IntPtr>) handles);
      this.m_client = client;
      // ISSUE: variable of a compiler-generated type
      IVsThreadedWaitDialogFactory service = this.m_serviceProvider.GetService(typeof (SVsThreadedWaitDialogFactory)) as IVsThreadedWaitDialogFactory;
      // ISSUE: reference to a compiler-generated method
      if (ErrorHandler.Failed(service.CreateInstance(out this.m_waitDialog)) || this.m_waitDialog == null)
        Marshal.ThrowExceptionForHR(-2147467259);
      if (!this.RegisterWithComponentManager())
        Marshal.ThrowExceptionForHR(-2147467259);
      try
      {
        // ISSUE: reference to a compiler-generated method
        // ISSUE: reference to a compiler-generated method
        int num = !this.m_enableRealProgress ? this.m_waitDialog.StartWaitDialog(this.m_waitTitle, this.m_waitText, (string) null, (object) null, this.m_statusBarText, 2, true, true) : this.m_waitDialog.StartWaitDialogWithPercentageProgress(this.m_waitTitle, this.m_waitText, this.m_progressText, (object) null, this.m_statusBarText, true, 2, this.m_totalSteps, this.m_currentStep);
        if (ErrorHandler.Failed(num))
          Marshal.ThrowExceptionForHR(num);
        return this.RunCommonMessagePump();
      }
      finally
      {
        this.UnRegisterFromComponentManager();
      }
    }

    private CommonMessagePumpExitCode ModalWaitForHandles(
      IntPtr[] handles,
      out int handleSignaledIndex)
    {
      DefaultCommonMessagePumpClientImpl messagePumpClientImpl = new DefaultCommonMessagePumpClientImpl();
      CommonMessagePumpExitCode messagePumpExitCode = this.ModalWaitForHandles(handles, (IVsCommonMessagePumpClientEvents) messagePumpClientImpl);
      handleSignaledIndex = messagePumpExitCode != CommonMessagePumpExitCode.HandleSignaled ? -1 : this.m_signaledHandleIndex;
      return messagePumpExitCode;
    }

    private void ResetTimeout()
    {
      if (this.m_hasInfiniteTimeout)
        this.m_endTime = DateTime.MaxValue;
      else
        this.m_endTime = DateTime.Now.Add(this.m_originalTimeout);
    }

    private bool HasTimedOut
    {
      get
      {
        if (this.m_hasInfiniteTimeout)
          return false;
        return DateTime.Now.CompareTo(this.m_endTime) > 0;
      }
    }

    private void UpdateWaitDialog()
    {
      if (this.m_waitDialog == null)
        return;
      bool pfCanceled;
      // ISSUE: reference to a compiler-generated method
      this.m_waitDialog.UpdateProgress(this.m_waitText, (string) null, this.m_statusBarText, 0, 0, this.m_allowCancel, out pfCanceled);
    }

    private IOleComponentManager ComponentManager
    {
      get
      {
        if (this.m_componentManager == null)
          this.m_componentManager = this.m_serviceProvider.GetService(typeof (SOleComponentManager)) as IOleComponentManager;
        return this.m_componentManager;
      }
    }

    private bool RegisterWithComponentManager()
    {
      OLECRINFO[] pcrinfo = new OLECRINFO[1];
      pcrinfo[0].cbSize = (uint) Marshal.SizeOf(typeof (OLECRINFO));
      pcrinfo[0].grfcadvf = 0U;
      pcrinfo[0].grfcrf = 0U;
      pcrinfo[0].uIdleTimeInterval = 0U;
      return this.ComponentManager.FRegisterComponent((IOleComponent) this, pcrinfo, out this.m_compId) != 0;
    }

    private bool UnRegisterFromComponentManager()
    {
      if (this.m_compId == 0U)
        return true;
      uint compId = this.m_compId;
      this.m_compId = 0U;
      return this.ComponentManager.FRevokeComponent(compId) != 0;
    }

    private CommonMessagePumpExitCode RunCommonMessagePump()
    {
      int pfCanceled = 0;
      try
      {
        if (this.ComponentManager.FPushMessageLoop(this.m_compId, 23U, IntPtr.Zero) == 0)
          return CommonMessagePumpExitCode.ApplicationExit;
      }
      finally
      {
        // ISSUE: reference to a compiler-generated method
        this.m_waitDialog.EndWaitDialog(out pfCanceled);
      }
      if (pfCanceled != 0)
        return CommonMessagePumpExitCode.UserCanceled;
      return this.HasTimedOut ? CommonMessagePumpExitCode.Timeout : CommonMessagePumpExitCode.HandleSignaled;
    }

    public CommonMessagePumpExitCode ModalWaitForHandles(
      WaitHandle waitHandle)
    {
      int handleSignaledIndex;
      return this.ModalWaitForHandles(new WaitHandle[1]
      {
        waitHandle
      }, out handleSignaledIndex);
    }

    public CommonMessagePumpExitCode ModalWaitForHandles(
      WaitHandle[] waitHandles,
      out int handleSignaledIndex)
    {
      IntPtr[] handles = new IntPtr[waitHandles.GetLength(0)];
      for (int index = 0; index < waitHandles.GetLength(0); ++index)
        handles[index] = waitHandles[index].SafeWaitHandle.DangerousGetHandle();
      return this.ModalWaitForHandles(handles, out handleSignaledIndex);
    }

    public CommonMessagePumpExitCode ModalWaitForHandles(
      WaitHandle[] waitHandles,
      IVsCommonMessagePumpClientEvents client)
    {
      IntPtr[] handles = new IntPtr[waitHandles.GetLength(0)];
      for (int index = 0; index < waitHandles.GetLength(0); ++index)
        handles[index] = waitHandles[index].SafeWaitHandle.DangerousGetHandle();
      return this.ModalWaitForHandles(handles, client);
    }

    public TimeSpan Timeout
    {
      get
      {
        return this.m_originalTimeout;
      }
      set
      {
        this.m_hasInfiniteTimeout = value == TimeSpan.MaxValue;
        this.m_originalTimeout = value;
        this.ResetTimeout();
      }
    }

    public bool AllowCancel
    {
      get
      {
        return this.m_allowCancel;
      }
      set
      {
        if (this.m_allowCancel == value)
          return;
        this.m_allowCancel = value;
        this.UpdateWaitDialog();
      }
    }

    public string WaitText
    {
      get
      {
        return this.m_waitText;
      }
      set
      {
        if (string.Equals(this.m_waitText, value, StringComparison.CurrentCulture))
          return;
        this.m_waitText = value;
        this.UpdateWaitDialog();
      }
    }

    public string WaitTitle
    {
      get
      {
        return this.m_waitTitle;
      }
      set
      {
        if (string.Equals(this.m_waitTitle, value, StringComparison.CurrentCulture))
          return;
        this.m_waitTitle = value;
        this.UpdateWaitDialog();
      }
    }

    public string StatusBarText
    {
      get
      {
        return this.m_statusBarText;
      }
      set
      {
        if (string.Equals(this.m_statusBarText, value, StringComparison.CurrentCulture))
          return;
        this.m_statusBarText = value;
        this.UpdateWaitDialog();
      }
    }

    public bool EnableRealProgress
    {
      set
      {
        this.m_enableRealProgress = value;
      }
    }

    public int TotalSteps
    {
      get
      {
        return this.m_totalSteps;
      }
      set
      {
        if (this.m_totalSteps == value)
          return;
        this.m_totalSteps = value;
        this.UpdateWaitDialog();
      }
    }

    public int CurrentStep
    {
      get
      {
        return this.m_currentStep;
      }
      set
      {
        if (this.m_currentStep == value)
          return;
        this.m_currentStep = value;
        this.UpdateWaitDialog();
      }
    }

    public string ProgressText
    {
      get
      {
        return this.m_progressText;
      }
      set
      {
        if (string.Equals(this.m_progressText, value, StringComparison.CurrentCulture))
          return;
        this.m_progressText = value;
        this.UpdateWaitDialog();
      }
    }

    int IVsCommonMessagePump.ModalWaitForObjects(
      IntPtr[] handles,
      uint handleCount,
      out uint waitResult)
    {
      int num = 0;
      waitResult = uint.MaxValue;
      int handleSignaledIndex = 0;
      try
      {
        switch (this.ModalWaitForHandles(handles, out handleSignaledIndex))
        {
          case CommonMessagePumpExitCode.Timeout:
            waitResult = 258U;
            break;
          case CommonMessagePumpExitCode.UserCanceled:
            num = -2147483638;
            break;
          case CommonMessagePumpExitCode.ApplicationExit:
            num = -2147467260;
            break;
          case CommonMessagePumpExitCode.HandleSignaled:
            waitResult = (uint) this.m_signaledHandleIndex;
            break;
        }
      }
      catch (Exception ex)
      {
        num = Marshal.GetHRForException(ex);
      }
      return num;
    }

    int IVsCommonMessagePump.ModalWaitForObjectsWithClient(
      IntPtr[] handles,
      uint handleCount,
      IVsCommonMessagePumpClientEvents pClient)
    {
      int num = 0;
      try
      {
        switch (this.ModalWaitForHandles(handles, pClient))
        {
          case CommonMessagePumpExitCode.UserCanceled:
            num = -2147483638;
            break;
          case CommonMessagePumpExitCode.ApplicationExit:
            num = -2147467260;
            break;
        }
      }
      catch (Exception ex)
      {
        num = Marshal.GetHRForException(ex);
      }
      return num;
    }

    int IVsCommonMessagePump.SetTimeout(uint timeoutInMilliseconds)
    {
      this.Timeout = uint.MaxValue != timeoutInMilliseconds ? TimeSpan.FromMilliseconds((double) timeoutInMilliseconds) : TimeSpan.MaxValue;
      return 0;
    }

    int IVsCommonMessagePump.SetAllowCancel(bool allowCancel)
    {
      this.AllowCancel = allowCancel;
      return 0;
    }

    int IVsCommonMessagePump.SetWaitText(string waitText)
    {
      this.WaitText = waitText;
      return 0;
    }

    int IVsCommonMessagePump.SetWaitTitle(string waitTitle)
    {
      this.WaitTitle = waitTitle;
      return 0;
    }

    int IVsCommonMessagePump.SetStatusBarText(string statusBarText)
    {
      this.StatusBarText = statusBarText;
      return 0;
    }

    int IVsCommonMessagePump.EnableRealProgress(bool enableRealProgress)
    {
      this.EnableRealProgress = enableRealProgress;
      return 0;
    }

    int IVsCommonMessagePump.SetProgressInfo(
      int iTotalSteps,
      int iCurrentStep,
      string progressText)
    {
      this.TotalSteps = iTotalSteps;
      this.CurrentStep = iCurrentStep;
      this.ProgressText = progressText;
      return 0;
    }

    int IOleComponent2Private.FContinueMessageLoop(
      uint uReason,
      IntPtr pvLoopData,
      MSG[] pMsgPeeked)
    {
      bool pfCanceled = false;
      if (this.m_waitDialog != null)
      {
        // ISSUE: reference to a compiler-generated method
        this.m_waitDialog.HasCanceled(out pfCanceled);
      }
      if (pfCanceled)
        return 0;
      this.UpdateWaitDialog();
      return 1;
    }

    int IOleComponent2Private.FDoIdle(uint grfidlef)
    {
      return 0;
    }

    int IOleComponent2Private.FPreTranslateMessage(MSG[] pMsg)
    {
      return 0;
    }

    int IOleComponent2Private.FQueryTerminate(int fPromptUser)
    {
      return 1;
    }

    int IOleComponent2Private.FReserved1(
      uint dwReserved,
      uint message,
      IntPtr wParam,
      IntPtr lParam)
    {
      return 1;
    }

    int IOleComponent2Private.GetWaitHandlesAndTimeout(
      IntPtr[] handles,
      out uint handleCount,
      out uint timeout,
      IntPtr loopData)
    {
      this.m_handles.CopyTo(handles);
      handleCount = (uint) this.m_handles.Count;
      timeout = (uint) this.m_endTime.Subtract(DateTime.Now).Ticks;
      return 0;
    }

    IntPtr IOleComponent2Private.HwndGetWindow(
      uint dwWhich,
      uint dwReserved)
    {
      return IntPtr.Zero;
    }

    void IOleComponent2Private.OnActivationChange(
      IOleComponent pic,
      int fSameComponent,
      OLECRINFO[] pcrinfo,
      int fHostIsActivating,
      OLECHOSTINFO[] pchostinfo,
      uint dwReserved)
    {
    }

    void IOleComponent2Private.OnAppActivate(int fActive, uint dwOtherThreadID)
    {
    }

    void IOleComponent2Private.OnEnterState(uint uStateID, int fEnter)
    {
    }

    int IOleComponent2Private.OnHandleSignaled(
      uint handleIndex,
      IntPtr pvLoopData,
      out bool shouldContinue)
    {
      if (this.m_handles.Count > 1)
        this.m_handles.RemoveAt((int) handleIndex);
      else if (this.m_handles.Count == 1)
        this.m_signaledHandleIndex = (int) handleIndex;
      // ISSUE: reference to a compiler-generated method
      int num = this.m_client.OnHandleSignaled(handleIndex, out shouldContinue);
      if (this.HasTimedOut)
      {
        // ISSUE: reference to a compiler-generated method
        this.m_client.OnTimeout(out shouldContinue);
        if (shouldContinue)
          this.ResetTimeout();
      }
      return num;
    }

    void IOleComponent2Private.OnLoseActivation()
    {
    }

    int IOleComponent2Private.OnTimeout(
      IntPtr pvLoopData,
      out bool shouldContinue)
    {
      // ISSUE: reference to a compiler-generated method
      this.m_client.OnTimeout(out shouldContinue);
      if (shouldContinue)
        this.ResetTimeout();
      return 0;
    }

    void IOleComponent2Private.Terminate()
    {
      this.m_componentManager.FRevokeComponent(this.m_compId);
    }

    int IOleComponent.FContinueMessageLoop(
      uint uReason,
      IntPtr pvLoopData,
      MSG[] pMsgPeeked)
    {
      return ((IOleComponent2Private) this).FContinueMessageLoop(uReason, pvLoopData, pMsgPeeked);
    }

    int IOleComponent.FDoIdle(uint grfidlef)
    {
      return ((IOleComponent2Private) this).FDoIdle(grfidlef);
    }

    int IOleComponent.FPreTranslateMessage(MSG[] pMsg)
    {
      return ((IOleComponent2Private) this).FPreTranslateMessage(pMsg);
    }

    int IOleComponent.FQueryTerminate(int fPromptUser)
    {
      return ((IOleComponent2Private) this).FQueryTerminate(fPromptUser);
    }

    int IOleComponent.FReserved1(
      uint dwReserved,
      uint message,
      IntPtr wParam,
      IntPtr lParam)
    {
      return ((IOleComponent2Private) this).FReserved1(dwReserved, message, wParam, lParam);
    }

    IntPtr IOleComponent.HwndGetWindow(uint dwWhich, uint dwReserved)
    {
      return ((IOleComponent2Private) this).HwndGetWindow(dwWhich, dwReserved);
    }

    void IOleComponent.OnActivationChange(
      IOleComponent pic,
      int fSameComponent,
      OLECRINFO[] pcrinfo,
      int fHostIsActivating,
      OLECHOSTINFO[] pchostinfo,
      uint dwReserved)
    {
      ((IOleComponent2Private) this).OnActivationChange(pic, fSameComponent, pcrinfo, fHostIsActivating, pchostinfo, dwReserved);
    }

    void IOleComponent.OnAppActivate(int fActive, uint dwOtherThreadID)
    {
      ((IOleComponent2Private) this).OnAppActivate(fActive, dwOtherThreadID);
    }

    void IOleComponent.OnEnterState(uint uStateID, int fEnter)
    {
      ((IOleComponent2Private) this).OnEnterState(uStateID, fEnter);
    }

    void IOleComponent.OnLoseActivation()
    {
      ((IOleComponent2Private) this).OnLoseActivation();
    }

    void IOleComponent.Terminate()
    {
      ((IOleComponent2Private) this).Terminate();
    }
  }
}
