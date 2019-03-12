// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.HelpService
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.VSHelp;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio
{
  internal sealed class HelpService : IHelpService, IDisposable
  {
    private IServiceProvider provider;
    private IVsUserContext context;
    private HelpService parentService;
    private uint cookie;
    private HelpContextType priority;
    private ArrayList subContextList;
    private bool needsRecreate;

    internal HelpService(IServiceProvider provider)
    {
      this.provider = provider;
    }

    private HelpService(
      HelpService parentService,
      IVsUserContext subContext,
      uint cookie,
      IServiceProvider provider,
      HelpContextType priority)
    {
      this.context = subContext;
      this.provider = provider;
      this.cookie = cookie;
      this.parentService = parentService;
      this.priority = priority;
    }

    private IHelpService CreateLocalContext(
      HelpContextType contextType,
      bool recreate,
      out IVsUserContext localContext,
      out uint cookie)
    {
      cookie = 0U;
      localContext = (IVsUserContext) null;
      if (this.provider == null)
        return (IHelpService) null;
      localContext = (IVsUserContext) null;
      int hr = 0;
      IVsMonitorUserContext service = (IVsMonitorUserContext) this.provider.GetService(typeof (IVsMonitorUserContext));
      if (service != null)
      {
        try
        {
          hr = service.CreateEmptyContext(out localContext);
        }
        catch (COMException ex)
        {
          hr = ex.ErrorCode;
        }
      }
      if (NativeMethods.Succeeded(hr) && localContext != null)
      {
        VSUSERCONTEXTPRIORITY vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_None;
        switch (contextType)
        {
          case HelpContextType.Ambient:
            vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_Ambient;
            break;
          case HelpContextType.Window:
            vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_Window;
            break;
          case HelpContextType.Selection:
            vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_Selection;
            break;
          case HelpContextType.ToolWindowSelection:
            vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_ToolWndSel;
            break;
        }
        IVsUserContext userContext = this.GetUserContext();
        if (userContext != null)
        {
          try
          {
            hr = userContext.AddSubcontext(localContext, (int) vsusercontextpriority, out cookie);
          }
          catch (COMException ex)
          {
            hr = ex.ErrorCode;
          }
        }
        if (NativeMethods.Succeeded(hr) && cookie != 0U && !recreate)
        {
          HelpService helpService = new HelpService(this, localContext, cookie, this.provider, contextType);
          if (this.subContextList == null)
            this.subContextList = new ArrayList();
          this.subContextList.Add((object) helpService);
          return (IHelpService) helpService;
        }
      }
      return (IHelpService) null;
    }

    private bool IsToolWindow(IVsWindowFrame frame)
    {
      int num = 0;
      object pvar;
      NativeMethods.ThrowOnFailure(frame.GetProperty(-3000, out pvar));
      if (pvar is int)
        num = (int) pvar;
      return num == 2;
    }

    private IVsUserContext GetUserContext()
    {
      this.RecreateContext();
      if (this.context == null)
      {
        if (this.provider == null)
          return (IVsUserContext) null;
        IVsWindowFrame service1 = (IVsWindowFrame) this.provider.GetService(typeof (IVsWindowFrame));
        if (service1 != null)
        {
          object pvar;
          NativeMethods.ThrowOnFailure(service1.GetProperty(-3010, out pvar));
          this.context = (IVsUserContext) pvar;
        }
        if (this.context == null)
        {
          IVsMonitorUserContext service2 = (IVsMonitorUserContext) this.provider.GetService(typeof (IVsMonitorUserContext));
          if (service2 != null)
          {
            NativeMethods.ThrowOnFailure(service2.CreateEmptyContext(out this.context));
            if (this.context != null && service1 != null && this.IsToolWindow(service1))
              NativeMethods.ThrowOnFailure(service1.SetProperty(-3010, (object) this.context));
          }
        }
        if (this.subContextList != null && this.context != null)
        {
          foreach (object subContext in this.subContextList)
            (subContext as HelpService)?.RecreateContext();
        }
      }
      return this.context;
    }

    private void RecreateContext()
    {
      if (this.parentService == null || !this.needsRecreate)
        return;
      this.needsRecreate = false;
      if (this.context == null)
      {
        this.parentService.CreateLocalContext(this.priority, true, out this.context, out this.cookie);
      }
      else
      {
        VSUSERCONTEXTPRIORITY vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_None;
        switch (this.priority)
        {
          case HelpContextType.Ambient:
            vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_Ambient;
            break;
          case HelpContextType.Window:
            vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_Window;
            break;
          case HelpContextType.Selection:
            vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_Selection;
            break;
          case HelpContextType.ToolWindowSelection:
            vsusercontextpriority = VSUSERCONTEXTPRIORITY.VSUC_Priority_ToolWndSel;
            break;
        }
        IVsUserContext userContext1 = this.parentService.GetUserContext();
        IVsUserContext userContext2 = this.GetUserContext();
        if (userContext2 == null || userContext1 == null)
          return;
        NativeMethods.ThrowOnFailure(userContext1.AddSubcontext(userContext2, (int) vsusercontextpriority, out this.cookie));
      }
    }

    private void NotifyContextChange(IVsUserContext cxt)
    {
      if (this.provider == null || this.parentService != null)
        return;
      IVsUserContext ppContext = (IVsUserContext) null;
      IVsMonitorUserContext service1 = (IVsMonitorUserContext) this.provider.GetService(typeof (IVsMonitorUserContext));
      if (service1 != null)
        NativeMethods.ThrowOnFailure(service1.get_ApplicationContext(out ppContext));
      if (ppContext == cxt)
        return;
      IVsWindowFrame service2 = (IVsWindowFrame) this.provider.GetService(typeof (IVsWindowFrame));
      if (service2 == null || this.IsToolWindow(service2))
        return;
      IVsTrackSelectionEx service3 = (IVsTrackSelectionEx) this.provider.GetService(typeof (IVsTrackSelectionEx));
      if (service3 == null)
        return;
      object varValue = (object) cxt;
      NativeMethods.ThrowOnFailure(service3.OnElementValueChange(5U, 0, varValue));
    }

    void IDisposable.Dispose()
    {
      if (this.subContextList != null && this.subContextList.Count > 0)
      {
        foreach (HelpService subContext in this.subContextList)
        {
          subContext.parentService = (HelpService) null;
          if (this.context != null)
          {
            try
            {
              this.context.RemoveSubcontext(subContext.cookie);
            }
            catch
            {
            }
          }
          ((IDisposable) subContext).Dispose();
        }
        this.subContextList = (ArrayList) null;
      }
      if (this.parentService != null)
      {
        IHelpService parentService = (IHelpService) this.parentService;
        this.parentService = (HelpService) null;
        parentService.RemoveLocalContext((IHelpService) this);
      }
      if (this.provider != null)
        this.provider = (IServiceProvider) null;
      if (this.context != null)
        this.context = (IVsUserContext) null;
      this.cookie = 0U;
    }

    void IHelpService.ClearContextAttributes()
    {
      if (this.context != null)
      {
        NativeMethods.ThrowOnFailure(this.context.RemoveAttribute((string) null, (string) null));
        if (this.subContextList != null)
        {
          foreach (object subContext in this.subContextList)
            (subContext as IHelpService)?.ClearContextAttributes();
        }
      }
      this.NotifyContextChange(this.context);
    }

    void IHelpService.AddContextAttribute(
      string name,
      string value,
      HelpKeywordType keywordType)
    {
      if (this.provider == null)
        return;
      IVsUserContext userContext = this.GetUserContext();
      if (userContext == null)
        return;
      VSUSERCONTEXTATTRIBUTEUSAGE usage = VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1;
      switch (keywordType)
      {
        case HelpKeywordType.F1Keyword:
          usage = VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1;
          break;
        case HelpKeywordType.GeneralKeyword:
          usage = VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Lookup;
          break;
        case HelpKeywordType.FilterKeyword:
          usage = VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Filter;
          break;
      }
      NativeMethods.ThrowOnFailure(userContext.AddAttribute(usage, name, value));
      this.NotifyContextChange(userContext);
    }

    IHelpService IHelpService.CreateLocalContext(HelpContextType contextType)
    {
      IVsUserContext localContext = (IVsUserContext) null;
      uint cookie = 0;
      return this.CreateLocalContext(contextType, false, out localContext, out cookie);
    }

    void IHelpService.RemoveContextAttribute(string name, string value)
    {
      if (this.provider == null)
        return;
      IVsUserContext userContext = this.GetUserContext();
      if (userContext == null)
        return;
      NativeMethods.ThrowOnFailure(userContext.RemoveAttribute(name, value));
      this.NotifyContextChange(userContext);
    }

    void IHelpService.RemoveLocalContext(IHelpService localContext)
    {
      if (this.subContextList == null || this.subContextList.IndexOf((object) localContext) == -1)
        return;
      this.subContextList.Remove((object) localContext);
      if (this.context != null)
        NativeMethods.ThrowOnFailure(this.context.RemoveSubcontext(((HelpService) localContext).cookie));
      ((HelpService) localContext).parentService = (HelpService) null;
    }

    void IHelpService.ShowHelpFromKeyword(string helpKeyword)
    {
      if (this.provider == null || helpKeyword == null)
        return;
      Help service = (Help) this.provider.GetService(typeof (Help));
      if (service == null)
        return;
      try
      {
        service.DisplayTopicFromF1Keyword(helpKeyword);
      }
      catch
      {
      }
    }

    void IHelpService.ShowHelpFromUrl(string helpUrl)
    {
      if (this.provider == null || helpUrl == null)
        return;
      Help service = (Help) this.provider.GetService(typeof (Help));
      if (service == null)
        return;
      try
      {
        service.DisplayTopicFromURL(helpUrl);
      }
      catch
      {
      }
    }
  }
}
