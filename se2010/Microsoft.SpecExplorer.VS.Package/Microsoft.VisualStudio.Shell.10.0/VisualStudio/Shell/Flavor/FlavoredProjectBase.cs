// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Flavor.FlavoredProjectBase
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Flavor
{
  [CLSCompliant(false)]
  public abstract class FlavoredProjectBase : IVsAggregatableProjectCorrected, System.IServiceProvider, IVsUIHierarchy, IVsHierarchy, IOleCommandTarget
  {
    protected IVsAggregatableProjectCorrected _innerVsAggregatableProject;
    protected IVsHierarchy _innerVsHierarchy;
    protected IVsUIHierarchy _innerVsUIHierarchy;
    protected IOleCommandTarget _innerOleCommandTarget;
    protected System.IServiceProvider serviceProvider;
    private OleMenuCommandService _menuService;
    private FlavoredProjectBase.DocumentsEventsSink _documentsEventsSink;
    private bool _hierarchyClosed;
    private int _inExecCommand;
    private uint cookie;

    public FlavoredProjectBase()
    {
      this._documentsEventsSink = new FlavoredProjectBase.DocumentsEventsSink(this);
    }

    public Interface_T GetComInterface<Interface_T>() where Interface_T : class
    {
      IntPtr pUnk = IntPtr.Zero;
      IntPtr ppv = IntPtr.Zero;
      try
      {
        pUnk = Marshal.GetIUnknownForObject((object) this);
        Guid guid = typeof (Interface_T).GUID;
        if (ErrorHandler.Failed(Marshal.QueryInterface(pUnk, ref guid, out ppv)) || IntPtr.Zero == ppv)
          return default (Interface_T);
        return Marshal.GetObjectForIUnknown(ppv) as Interface_T;
      }
      finally
      {
        if (IntPtr.Zero != pUnk)
          Marshal.Release(pUnk);
        if (IntPtr.Zero != ppv)
          Marshal.Release(ppv);
      }
    }

    int IVsAggregatableProjectCorrected.SetInnerProject(
      IntPtr innerIUnknown)
    {
      this.SetInnerProject(innerIUnknown);
      return 0;
    }

    protected virtual void SetInnerProject(IntPtr innerIUnknown)
    {
      object objectForIunknown = Marshal.GetObjectForIUnknown(innerIUnknown);
      this._innerVsAggregatableProject = objectForIunknown as IVsAggregatableProjectCorrected;
      this._innerVsHierarchy = (IVsHierarchy) objectForIunknown;
      this._innerVsUIHierarchy = (IVsUIHierarchy) objectForIunknown;
      this._innerOleCommandTarget = objectForIunknown as IOleCommandTarget;
      if (this.serviceProvider == null)
        throw new NotSupportedException("serviceProvider should have been set before SetInnerProject gets called.");
      this._menuService = new OleMenuCommandService((System.IServiceProvider) this, this._innerOleCommandTarget);
      IntPtr pUnk = IntPtr.Zero;
      try
      {
        pUnk = Marshal.GetIUnknownForObject((object) this);
        ((IVsProjectAggregator2) Marshal.GetObjectForIUnknown(pUnk))?.SetInner(innerIUnknown);
      }
      finally
      {
        if (pUnk != IntPtr.Zero)
          Marshal.Release(pUnk);
      }
    }

    int IVsAggregatableProjectCorrected.InitializeForOuter(
      string fileName,
      string location,
      string name,
      uint flags,
      ref Guid guidProject,
      out IntPtr project,
      out int canceled)
    {
      int num = 0;
      project = IntPtr.Zero;
      canceled = 0;
      if (this._innerVsAggregatableProject == null || guidProject != VSConstants.IID_IUnknown)
        throw new NotSupportedException();
      IntPtr pUnk = IntPtr.Zero;
      try
      {
        pUnk = Marshal.GetIUnknownForObject((object) this);
        if (pUnk != IntPtr.Zero)
          num = Marshal.QueryInterface(pUnk, ref guidProject, out project);
      }
      finally
      {
        if (pUnk != IntPtr.Zero)
          Marshal.Release(pUnk);
      }
      bool cancel;
      this.InitializeForOuter(fileName, location, name, flags, ref guidProject, out cancel);
      if (cancel)
        canceled = 1;
      return num;
    }

    protected virtual void InitializeForOuter(
      string fileName,
      string location,
      string name,
      uint flags,
      ref Guid guidProject,
      out bool cancel)
    {
      cancel = false;
    }

    int IVsAggregatableProjectCorrected.OnAggregationComplete()
    {
      this.OnAggregationComplete();
      if (this._innerVsAggregatableProject != null)
        return this._innerVsAggregatableProject.OnAggregationComplete();
      return 0;
    }

    protected virtual void OnAggregationComplete()
    {
      ErrorHandler.ThrowOnFailure(this.GetTrackProjectDocuments().AdviseTrackProjectDocumentsEvents((IVsTrackProjectDocumentsEvents2) this._documentsEventsSink, out this.cookie));
    }

    int IVsAggregatableProjectCorrected.SetAggregateProjectTypeGuids(
      string projectTypeGuids)
    {
      if (this._innerVsAggregatableProject == null)
        throw new NotSupportedException();
      return this._innerVsAggregatableProject.SetAggregateProjectTypeGuids(projectTypeGuids);
    }

    int IVsAggregatableProjectCorrected.GetAggregateProjectTypeGuids(
      out string projectTypeGuids)
    {
      if (this._innerVsAggregatableProject == null)
        throw new NotSupportedException();
      return this._innerVsAggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuids);
    }

    int IVsHierarchy.AdviseHierarchyEvents(
      IVsHierarchyEvents eventSink,
      out uint cookie)
    {
      cookie = this.AdviseHierarchyEvents(eventSink);
      return 0;
    }

    int IVsHierarchy.Close()
    {
      this._hierarchyClosed = true;
      this.Close();
      if (this.cookie != 0U)
      {
        this.GetTrackProjectDocuments().UnadviseTrackProjectDocumentsEvents(this.cookie);
        this.cookie = 0U;
      }
      if (this._menuService != null)
      {
        OleMenuCommandService menuService = this._menuService;
        this._menuService = (OleMenuCommandService) null;
        menuService.Dispose();
      }
      if (this._inExecCommand == 0)
        this.FreeInterfaces();
      return 0;
    }

    public virtual void FreeInterfaces()
    {
      if (this._menuService != null)
      {
        OleMenuCommandService menuService = this._menuService;
        this._menuService = (OleMenuCommandService) null;
        menuService.Dispose();
      }
      this._innerOleCommandTarget = (IOleCommandTarget) null;
      this._innerVsAggregatableProject = (IVsAggregatableProjectCorrected) null;
      this._innerVsUIHierarchy = (IVsUIHierarchy) null;
      this._innerVsHierarchy = (IVsHierarchy) null;
    }

    int IVsHierarchy.GetCanonicalName(uint itemId, out string name)
    {
      return this.GetCanonicalName(itemId, out name);
    }

    int IVsHierarchy.GetGuidProperty(uint itemId, int propId, out Guid guid)
    {
      guid = this.GetGuidProperty(itemId, propId);
      return 0;
    }

    int IVsHierarchy.GetNestedHierarchy(
      uint itemId,
      ref Guid guidHierarchyNested,
      out IntPtr hierarchyNested,
      out uint itemIdNested)
    {
      return this.GetNestedHierarchy(itemId, ref guidHierarchyNested, out hierarchyNested, out itemIdNested);
    }

    int IVsHierarchy.GetProperty(uint itemId, int propId, out object property)
    {
      return this.GetProperty(itemId, propId, out property);
    }

    int IVsHierarchy.GetSite(out Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider)
    {
      serviceProvider = this.GetSite();
      return 0;
    }

    int IVsHierarchy.ParseCanonicalName(string name, out uint itemId)
    {
      return this.ParseCanonicalName(name, out itemId);
    }

    int IVsHierarchy.QueryClose(out int canClose)
    {
      canClose = 0;
      if (this.QueryClose())
        canClose = 1;
      return 0;
    }

    int IVsHierarchy.SetGuidProperty(uint itemId, int propId, ref Guid guid)
    {
      this.SetGuidProperty(itemId, propId, ref guid);
      return 0;
    }

    int IVsHierarchy.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider)
    {
      this.serviceProvider = (System.IServiceProvider) new ServiceProvider(serviceProvider);
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.SetSite(serviceProvider));
      return 0;
    }

    int IVsHierarchy.UnadviseHierarchyEvents(uint cookie)
    {
      this.UnadviseHierarchyEvents(cookie);
      return 0;
    }

    int IVsHierarchy.SetProperty(uint itemId, int propId, object property)
    {
      return this.SetProperty(itemId, propId, property);
    }

    int IVsHierarchy.Unused0()
    {
      this.Unused0();
      return 0;
    }

    int IVsHierarchy.Unused1()
    {
      this.Unused1();
      return 0;
    }

    int IVsHierarchy.Unused2()
    {
      this.Unused2();
      return 0;
    }

    int IVsHierarchy.Unused3()
    {
      this.Unused3();
      return 0;
    }

    int IVsHierarchy.Unused4()
    {
      this.Unused4();
      return 0;
    }

    protected virtual uint AdviseHierarchyEvents(IVsHierarchyEvents eventSink)
    {
      uint pdwCookie = 0;
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.AdviseHierarchyEvents(eventSink, out pdwCookie));
      return pdwCookie;
    }

    protected virtual void Close()
    {
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.Close());
    }

    protected virtual int GetCanonicalName(uint itemId, out string name)
    {
      return this._innerVsHierarchy.GetCanonicalName(itemId, out name);
    }

    protected virtual Guid GetGuidProperty(uint itemId, int propId)
    {
      Guid pguid = Guid.Empty;
      if (this._innerVsHierarchy == null)
        return Guid.Empty;
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.GetGuidProperty(itemId, propId, out pguid));
      return pguid;
    }

    protected virtual int GetNestedHierarchy(
      uint itemId,
      ref Guid guidHierarchyNested,
      out IntPtr hierarchyNested,
      out uint itemIdNested)
    {
      if (this._innerVsHierarchy != null)
        return this._innerVsHierarchy.GetNestedHierarchy(itemId, ref guidHierarchyNested, out hierarchyNested, out itemIdNested);
      hierarchyNested = IntPtr.Zero;
      itemIdNested = uint.MaxValue;
      return -2147467262;
    }

    protected virtual int GetProperty(uint itemId, int propId, out object property)
    {
      if (this._innerVsHierarchy != null)
        return this._innerVsHierarchy.GetProperty(itemId, propId, out property);
      property = (object) null;
      return -2147418113;
    }

    protected virtual Microsoft.VisualStudio.OLE.Interop.IServiceProvider GetSite()
    {
      Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP;
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.GetSite(out ppSP));
      return ppSP;
    }

    protected virtual int ParseCanonicalName(string name, out uint itemId)
    {
      return this._innerVsHierarchy.ParseCanonicalName(name, out itemId);
    }

    protected virtual bool QueryClose()
    {
      int pfCanClose;
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.QueryClose(out pfCanClose));
      return pfCanClose != 0;
    }

    protected virtual void SetGuidProperty(uint itemId, int propId, ref Guid guid)
    {
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.SetGuidProperty(itemId, propId, ref guid));
    }

    protected virtual void UnadviseHierarchyEvents(uint cookie)
    {
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.UnadviseHierarchyEvents(cookie));
    }

    protected virtual int SetProperty(uint itemId, int propId, object property)
    {
      return this._innerVsHierarchy.SetProperty(itemId, propId, property);
    }

    protected virtual void Unused0()
    {
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.Unused0());
    }

    protected virtual void Unused1()
    {
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.Unused1());
    }

    protected virtual void Unused2()
    {
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.Unused2());
    }

    protected virtual void Unused3()
    {
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.Unused3());
    }

    protected virtual void Unused4()
    {
      ErrorHandler.ThrowOnFailure(this._innerVsHierarchy.Unused4());
    }

    int IVsUIHierarchy.QueryStatusCommand(
      uint itemid,
      ref Guid pguidCmdGroup,
      uint cCmds,
      OLECMD[] prgCmds,
      IntPtr pCmdText)
    {
      return this.QueryStatusCommand(itemid, ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    }

    protected virtual int QueryStatusCommand(
      uint itemid,
      ref Guid pguidCmdGroup,
      uint cCmds,
      OLECMD[] prgCmds,
      IntPtr pCmdText)
    {
      return this._innerVsUIHierarchy.QueryStatusCommand(itemid, ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    }

    int IVsUIHierarchy.ExecCommand(
      uint itemid,
      ref Guid pguidCmdGroup,
      uint nCmdID,
      uint nCmdexecopt,
      IntPtr pvaIn,
      IntPtr pvaOut)
    {
      try
      {
        ++this._inExecCommand;
        return this.ExecCommand(itemid, ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
      }
      finally
      {
        --this._inExecCommand;
        if (this._hierarchyClosed && this._inExecCommand == 0)
          this.FreeInterfaces();
      }
    }

    protected virtual int ExecCommand(
      uint itemid,
      ref Guid pguidCmdGroup,
      uint nCmdID,
      uint nCmdexecopt,
      IntPtr pvaIn,
      IntPtr pvaOut)
    {
      return this._innerVsUIHierarchy.ExecCommand(itemid, ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
    }

    int IVsUIHierarchy.AdviseHierarchyEvents(
      IVsHierarchyEvents pEventSink,
      out uint pdwCookie)
    {
      return ((IVsHierarchy) this).AdviseHierarchyEvents(pEventSink, out pdwCookie);
    }

    int IVsUIHierarchy.Close()
    {
      return ((IVsHierarchy) this).Close();
    }

    int IVsUIHierarchy.GetCanonicalName(uint itemid, out string pbstrName)
    {
      return ((IVsHierarchy) this).GetCanonicalName(itemid, out pbstrName);
    }

    int IVsUIHierarchy.GetGuidProperty(uint itemid, int propid, out Guid pguid)
    {
      return ((IVsHierarchy) this).GetGuidProperty(itemid, propid, out pguid);
    }

    int IVsUIHierarchy.GetNestedHierarchy(
      uint itemid,
      ref Guid iidHierarchyNested,
      out IntPtr ppHierarchyNested,
      out uint pitemidNested)
    {
      return ((IVsHierarchy) this).GetNestedHierarchy(itemid, ref iidHierarchyNested, out ppHierarchyNested, out pitemidNested);
    }

    int IVsUIHierarchy.GetProperty(uint itemid, int propid, out object pvar)
    {
      return ((IVsHierarchy) this).GetProperty(itemid, propid, out pvar);
    }

    int IVsUIHierarchy.GetSite(out Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP)
    {
      return ((IVsHierarchy) this).GetSite(out ppSP);
    }

    int IVsUIHierarchy.ParseCanonicalName(string pszName, out uint pitemid)
    {
      return ((IVsHierarchy) this).ParseCanonicalName(pszName, out pitemid);
    }

    int IVsUIHierarchy.QueryClose(out int pfCanClose)
    {
      return ((IVsHierarchy) this).QueryClose(out pfCanClose);
    }

    int IVsUIHierarchy.SetGuidProperty(uint itemid, int propid, ref Guid rguid)
    {
      return ((IVsHierarchy) this).SetGuidProperty(itemid, propid, ref rguid);
    }

    int IVsUIHierarchy.SetProperty(uint itemid, int propid, object var)
    {
      return ((IVsHierarchy) this).SetProperty(itemid, propid, var);
    }

    int IVsUIHierarchy.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
    {
      return ((IVsHierarchy) this).SetSite(psp);
    }

    int IVsUIHierarchy.UnadviseHierarchyEvents(uint dwCookie)
    {
      return ((IVsHierarchy) this).UnadviseHierarchyEvents(dwCookie);
    }

    int IVsUIHierarchy.Unused0()
    {
      return ((IVsHierarchy) this).Unused0();
    }

    int IVsUIHierarchy.Unused1()
    {
      return ((IVsHierarchy) this).Unused1();
    }

    int IVsUIHierarchy.Unused2()
    {
      return ((IVsHierarchy) this).Unused2();
    }

    int IVsUIHierarchy.Unused3()
    {
      return ((IVsHierarchy) this).Unused3();
    }

    int IVsUIHierarchy.Unused4()
    {
      return ((IVsHierarchy) this).Unused4();
    }

    int IOleCommandTarget.Exec(
      ref Guid pguidCmdGroup,
      uint nCmdID,
      uint nCmdexecopt,
      IntPtr pvaIn,
      IntPtr pvaOut)
    {
      return ((IOleCommandTarget) this._menuService).Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
    }

    int IOleCommandTarget.QueryStatus(
      ref Guid pguidCmdGroup,
      uint cCmds,
      OLECMD[] prgCmds,
      IntPtr pCmdText)
    {
      return ((IOleCommandTarget) this._menuService).QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    }

    object System.IServiceProvider.GetService(Type serviceType)
    {
      if (serviceType.IsEquivalentTo(typeof (IOleCommandTarget)))
        return (object) this._menuService;
      if (serviceType.IsEquivalentTo(typeof (IMenuCommandService)))
        return (object) this._menuService;
      return this.serviceProvider.GetService(serviceType);
    }

    public event FlavoredProjectBase.EventHandler<ProjectDocumentsChangeEventArgs> FileAdded;

    public event FlavoredProjectBase.EventHandler<ProjectDocumentsChangeEventArgs> FileRemoved;

    public event FlavoredProjectBase.EventHandler<ProjectDocumentsChangeEventArgs> FileRenamed;

    public event FlavoredProjectBase.EventHandler<ProjectDocumentsChangeEventArgs> DirectoryAdded;

    public event FlavoredProjectBase.EventHandler<ProjectDocumentsChangeEventArgs> DirectoryRemoved;

    public event FlavoredProjectBase.EventHandler<ProjectDocumentsChangeEventArgs> DirectoryRenamed;

    public event FlavoredProjectBase.EventHandler<ProjectDocumentsChangeEventArgs> SccStatusChanged;

    private IVsTrackProjectDocuments2 GetTrackProjectDocuments()
    {
      IVsTrackProjectDocuments2 service = ((System.IServiceProvider) this).GetService(typeof (SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;
      if (service == null)
        throw new InvalidOperationException();
      return service;
    }

    private void GenerateEvents(
      IVsProject[] projects,
      int[] firstFiles,
      string[] mkDocuments,
      FlavoredProjectBase.EventHandler<ProjectDocumentsChangeEventArgs> eventToGenerate,
      ProjectDocumentsChangeEventArgs e)
    {
      if (eventToGenerate == null)
        return;
      if (projects == null || firstFiles == null || mkDocuments == null)
        throw new ArgumentNullException();
      if (projects.Length != firstFiles.Length)
        throw new ArgumentException();
      int num1 = -1;
      int num2 = mkDocuments.Length - 1;
      for (int index = 0; index < projects.Length; ++index)
      {
        if (num1 > -1)
        {
          num2 = firstFiles[index] - 1;
          break;
        }
        if (this.IsThisProject(projects[index]))
          num1 = firstFiles[index];
      }
      if (num2 >= mkDocuments.Length)
        throw new ArgumentException();
      if (num1 < 0)
        return;
      for (int index = num1; index <= num2; ++index)
      {
        try
        {
          e.MkDocument = mkDocuments[index];
          eventToGenerate((object) this, e);
        }
        catch (Exception ex)
        {
        }
      }
    }

    private bool IsThisProject(IVsProject prj)
    {
      Guid iidIunknown = VSConstants.IID_IUnknown;
      IntPtr pUnk1 = IntPtr.Zero;
      IntPtr ppv1 = IntPtr.Zero;
      IntPtr pUnk2 = IntPtr.Zero;
      IntPtr ppv2 = IntPtr.Zero;
      try
      {
        pUnk1 = Marshal.GetIUnknownForObject((object) prj);
        Marshal.QueryInterface(pUnk1, ref iidIunknown, out ppv1);
        pUnk2 = Marshal.GetIUnknownForObject((object) this);
        Marshal.QueryInterface(pUnk2, ref iidIunknown, out ppv2);
        return ppv1 == ppv2;
      }
      finally
      {
        if (IntPtr.Zero != pUnk1)
          Marshal.Release(pUnk1);
        if (IntPtr.Zero != ppv1)
          Marshal.Release(ppv1);
        if (IntPtr.Zero != pUnk2)
          Marshal.Release(pUnk2);
        if (IntPtr.Zero != ppv2)
          Marshal.Release(ppv2);
      }
    }

    public delegate void EventHandler<ProjectDocumentsChangeEventArgs>(
      object sender,
      ProjectDocumentsChangeEventArgs e);

    internal class DocumentsEventsSink : IVsTrackProjectDocumentsEvents2
    {
      private FlavoredProjectBase _flavoredProjectBase;

      internal DocumentsEventsSink(FlavoredProjectBase flavoredProjectBase)
      {
        this._flavoredProjectBase = flavoredProjectBase;
      }

      int IVsTrackProjectDocumentsEvents2.OnAfterAddDirectoriesEx(
        int cProjects,
        int cDirectories,
        IVsProject[] rgpProjects,
        int[] rgFirstIndices,
        string[] rgpszMkDocuments,
        VSADDDIRECTORYFLAGS[] rgFlags)
      {
        this._flavoredProjectBase.GenerateEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, this._flavoredProjectBase.DirectoryAdded, new ProjectDocumentsChangeEventArgs());
        return 0;
      }

      int IVsTrackProjectDocumentsEvents2.OnAfterAddFilesEx(
        int cProjects,
        int cFiles,
        IVsProject[] rgpProjects,
        int[] rgFirstIndices,
        string[] rgpszMkDocuments,
        VSADDFILEFLAGS[] rgFlags)
      {
        this._flavoredProjectBase.GenerateEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, this._flavoredProjectBase.FileAdded, new ProjectDocumentsChangeEventArgs());
        return 0;
      }

      int IVsTrackProjectDocumentsEvents2.OnAfterRemoveDirectories(
        int cProjects,
        int cDirectories,
        IVsProject[] rgpProjects,
        int[] rgFirstIndices,
        string[] rgpszMkDocuments,
        VSREMOVEDIRECTORYFLAGS[] rgFlags)
      {
        this._flavoredProjectBase.GenerateEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, this._flavoredProjectBase.DirectoryRemoved, new ProjectDocumentsChangeEventArgs());
        return 0;
      }

      int IVsTrackProjectDocumentsEvents2.OnAfterRemoveFiles(
        int cProjects,
        int cFiles,
        IVsProject[] rgpProjects,
        int[] rgFirstIndices,
        string[] rgpszMkDocuments,
        VSREMOVEFILEFLAGS[] rgFlags)
      {
        this._flavoredProjectBase.GenerateEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, this._flavoredProjectBase.FileRemoved, new ProjectDocumentsChangeEventArgs());
        return 0;
      }

      int IVsTrackProjectDocumentsEvents2.OnAfterRenameDirectories(
        int cProjects,
        int cDirs,
        IVsProject[] rgpProjects,
        int[] rgFirstIndices,
        string[] rgszMkOldNames,
        string[] rgszMkNewNames,
        VSRENAMEDIRECTORYFLAGS[] rgFlags)
      {
        this._flavoredProjectBase.GenerateEvents(rgpProjects, rgFirstIndices, rgszMkNewNames, this._flavoredProjectBase.DirectoryRenamed, new ProjectDocumentsChangeEventArgs());
        return 0;
      }

      int IVsTrackProjectDocumentsEvents2.OnAfterRenameFiles(
        int cProjects,
        int cFiles,
        IVsProject[] rgpProjects,
        int[] rgFirstIndices,
        string[] rgszMkOldNames,
        string[] rgszMkNewNames,
        VSRENAMEFILEFLAGS[] rgFlags)
      {
        this._flavoredProjectBase.GenerateEvents(rgpProjects, rgFirstIndices, rgszMkNewNames, this._flavoredProjectBase.FileRenamed, new ProjectDocumentsChangeEventArgs());
        return 0;
      }

      int IVsTrackProjectDocumentsEvents2.OnAfterSccStatusChanged(
        int cProjects,
        int cFiles,
        IVsProject[] rgpProjects,
        int[] rgFirstIndices,
        string[] rgpszMkDocuments,
        uint[] rgdwSccStatus)
      {
        this._flavoredProjectBase.GenerateEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, this._flavoredProjectBase.SccStatusChanged, new ProjectDocumentsChangeEventArgs());
        return 0;
      }

      int IVsTrackProjectDocumentsEvents2.OnQueryAddDirectories(
        IVsProject pProject,
        int cDirectories,
        string[] rgpszMkDocuments,
        VSQUERYADDDIRECTORYFLAGS[] rgFlags,
        VSQUERYADDDIRECTORYRESULTS[] pSummaryResult,
        VSQUERYADDDIRECTORYRESULTS[] rgResults)
      {
        return 0;
      }

      int IVsTrackProjectDocumentsEvents2.OnQueryAddFiles(
        IVsProject pProject,
        int cFiles,
        string[] rgpszMkDocuments,
        VSQUERYADDFILEFLAGS[] rgFlags,
        VSQUERYADDFILERESULTS[] pSummaryResult,
        VSQUERYADDFILERESULTS[] rgResults)
      {
        return 0;
      }

      int IVsTrackProjectDocumentsEvents2.OnQueryRemoveDirectories(
        IVsProject pProject,
        int cDirectories,
        string[] rgpszMkDocuments,
        VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags,
        VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult,
        VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
      {
        return 0;
      }

      int IVsTrackProjectDocumentsEvents2.OnQueryRemoveFiles(
        IVsProject pProject,
        int cFiles,
        string[] rgpszMkDocuments,
        VSQUERYREMOVEFILEFLAGS[] rgFlags,
        VSQUERYREMOVEFILERESULTS[] pSummaryResult,
        VSQUERYREMOVEFILERESULTS[] rgResults)
      {
        return 0;
      }

      int IVsTrackProjectDocumentsEvents2.OnQueryRenameDirectories(
        IVsProject pProject,
        int cDirs,
        string[] rgszMkOldNames,
        string[] rgszMkNewNames,
        VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags,
        VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult,
        VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
      {
        return 0;
      }

      int IVsTrackProjectDocumentsEvents2.OnQueryRenameFiles(
        IVsProject pProject,
        int cFiles,
        string[] rgszMkOldNames,
        string[] rgszMkNewNames,
        VSQUERYRENAMEFILEFLAGS[] rgFlags,
        VSQUERYRENAMEFILERESULTS[] pSummaryResult,
        VSQUERYRENAMEFILERESULTS[] rgResults)
      {
        return 0;
      }
    }
  }
}
