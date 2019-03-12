// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Flavor.FlavoredProject
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.ProjectAggregator;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Flavor
{
  [CLSCompliant(false)]
  public abstract class FlavoredProject : CProjectAggregatorClass, IVsAggregatableProject, System.IServiceProvider, IVsUIHierarchy, IVsHierarchy, IOleCommandTarget, IVsTrackProjectDocumentsEvents2
  {
    protected IVsAggregatableProject innerVsAggregatableProject;
    protected IVsHierarchy innerVsHierarchy;
    protected IVsUIHierarchy innerVsUIHierarchy;
    protected IOleCommandTarget innerOleCommandTarget;
    protected System.IServiceProvider serviceProvider;
    private OleMenuCommandService menuService;
    private uint cookie;

    int IVsAggregatableProject.SetInnerProject(object inner)
    {
      this.SetInnerProject(inner);
      return 0;
    }

    protected virtual void SetInnerProject(object inner)
    {
      this.innerVsAggregatableProject = (IVsAggregatableProject) inner;
      this.innerVsHierarchy = (IVsHierarchy) inner;
      this.innerVsUIHierarchy = (IVsUIHierarchy) inner;
      this.innerOleCommandTarget = inner as IOleCommandTarget;
      if (this.serviceProvider == null)
        throw new NotSupportedException("serviceProvider should have been set before SetInnerProject gets called.");
      this.menuService = new OleMenuCommandService((System.IServiceProvider) this, this.innerOleCommandTarget);
      this.SetInner(inner);
    }

    int IVsAggregatableProject.InitializeForOuter(
      string fileName,
      string location,
      string name,
      uint flags,
      ref Guid guidProject,
      out IntPtr project,
      out int canceled)
    {
      if (this.innerVsAggregatableProject == null || guidProject != Microsoft.VisualStudio.NativeMethods.IID_IUnknown)
        throw new NotSupportedException();
      Marshal.QueryInterface(Marshal.GetIUnknownForObject((object) this), ref guidProject, out project);
      canceled = 0;
      bool cancel;
      this.InitializeForOuter(fileName, location, name, flags, ref guidProject, out cancel);
      if (cancel)
        canceled = 1;
      return 0;
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

    int IVsAggregatableProject.OnAggregationComplete()
    {
      this.OnAggregationComplete();
      if (this.innerVsAggregatableProject != null)
        return this.innerVsAggregatableProject.OnAggregationComplete();
      return 0;
    }

    protected virtual void OnAggregationComplete()
    {
      ErrorHandler.ThrowOnFailure(this.GetTrackProjectDocuments().AdviseTrackProjectDocumentsEvents((IVsTrackProjectDocumentsEvents2) this, out this.cookie));
    }

    int IVsAggregatableProject.SetAggregateProjectTypeGuids(
      string projectTypeGuids)
    {
      if (this.innerVsAggregatableProject == null)
        throw new NotSupportedException();
      return this.innerVsAggregatableProject.SetAggregateProjectTypeGuids(projectTypeGuids);
    }

    int IVsAggregatableProject.GetAggregateProjectTypeGuids(
      out string projectTypeGuids)
    {
      if (this.innerVsAggregatableProject == null)
        throw new NotSupportedException();
      return this.innerVsAggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuids);
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
      if (this.cookie != 0U)
      {
        this.GetTrackProjectDocuments().UnadviseTrackProjectDocumentsEvents(this.cookie);
        this.cookie = 0U;
      }
      this.Close();
      return 0;
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
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.SetSite(serviceProvider));
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
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.AdviseHierarchyEvents(eventSink, out pdwCookie));
      return pdwCookie;
    }

    protected virtual void Close()
    {
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.Close());
    }

    protected virtual int GetCanonicalName(uint itemId, out string name)
    {
      return this.innerVsHierarchy.GetCanonicalName(itemId, out name);
    }

    protected virtual Guid GetGuidProperty(uint itemId, int propId)
    {
      Guid pguid;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.GetGuidProperty(itemId, propId, out pguid));
      return pguid;
    }

    protected virtual int GetNestedHierarchy(
      uint itemId,
      ref Guid guidHierarchyNested,
      out IntPtr hierarchyNested,
      out uint itemIdNested)
    {
      return this.innerVsHierarchy.GetNestedHierarchy(itemId, ref guidHierarchyNested, out hierarchyNested, out itemIdNested);
    }

    protected virtual int GetProperty(uint itemId, int propId, out object property)
    {
      return this.innerVsHierarchy.GetProperty(itemId, propId, out property);
    }

    protected virtual Microsoft.VisualStudio.OLE.Interop.IServiceProvider GetSite()
    {
      Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.GetSite(out ppSP));
      return ppSP;
    }

    protected virtual int ParseCanonicalName(string name, out uint itemId)
    {
      return this.innerVsHierarchy.ParseCanonicalName(name, out itemId);
    }

    protected virtual bool QueryClose()
    {
      int pfCanClose;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.QueryClose(out pfCanClose));
      return pfCanClose != 0;
    }

    protected virtual void SetGuidProperty(uint itemId, int propId, ref Guid guid)
    {
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.SetGuidProperty(itemId, propId, ref guid));
    }

    protected virtual void UnadviseHierarchyEvents(uint cookie)
    {
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.UnadviseHierarchyEvents(cookie));
    }

    protected virtual int SetProperty(uint itemId, int propId, object property)
    {
      return this.innerVsHierarchy.SetProperty(itemId, propId, property);
    }

    protected virtual void Unused0()
    {
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.Unused0());
    }

    protected virtual void Unused1()
    {
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.Unused1());
    }

    protected virtual void Unused2()
    {
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.Unused2());
    }

    protected virtual void Unused3()
    {
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.Unused3());
    }

    protected virtual void Unused4()
    {
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.innerVsHierarchy.Unused4());
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
      return this.innerVsUIHierarchy.QueryStatusCommand(itemid, ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    }

    int IVsUIHierarchy.ExecCommand(
      uint itemid,
      ref Guid pguidCmdGroup,
      uint nCmdID,
      uint nCmdexecopt,
      IntPtr pvaIn,
      IntPtr pvaOut)
    {
      return this.ExecCommand(itemid, ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
    }

    protected virtual int ExecCommand(
      uint itemid,
      ref Guid pguidCmdGroup,
      uint nCmdID,
      uint nCmdexecopt,
      IntPtr pvaIn,
      IntPtr pvaOut)
    {
      return this.innerVsUIHierarchy.ExecCommand(itemid, ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
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
      return ((IOleCommandTarget) this.menuService).Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
    }

    int IOleCommandTarget.QueryStatus(
      ref Guid pguidCmdGroup,
      uint cCmds,
      OLECMD[] prgCmds,
      IntPtr pCmdText)
    {
      return ((IOleCommandTarget) this.menuService).QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    }

    object System.IServiceProvider.GetService(Type serviceType)
    {
      if (serviceType.IsEquivalentTo(typeof (IOleCommandTarget)))
        return (object) this.menuService;
      if (serviceType.IsEquivalentTo(typeof (IMenuCommandService)))
        return (object) this.menuService;
      return this.serviceProvider.GetService(serviceType);
    }

    public event FlavoredProject.EventHandler<ProjectDocumentsChangeEventArgs> FileAdded;

    public event FlavoredProject.EventHandler<ProjectDocumentsChangeEventArgs> FileRemoved;

    public event FlavoredProject.EventHandler<ProjectDocumentsChangeEventArgs> FileRenamed;

    public event FlavoredProject.EventHandler<ProjectDocumentsChangeEventArgs> DirectoryAdded;

    public event FlavoredProject.EventHandler<ProjectDocumentsChangeEventArgs> DirectoryRemoved;

    public event FlavoredProject.EventHandler<ProjectDocumentsChangeEventArgs> DirectoryRenamed;

    public event FlavoredProject.EventHandler<ProjectDocumentsChangeEventArgs> SccStatusChanged;

    int IVsTrackProjectDocumentsEvents2.OnAfterAddDirectoriesEx(
      int cProjects,
      int cDirectories,
      IVsProject[] rgpProjects,
      int[] rgFirstIndices,
      string[] rgpszMkDocuments,
      VSADDDIRECTORYFLAGS[] rgFlags)
    {
      this.GenerateEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, this.DirectoryAdded, new ProjectDocumentsChangeEventArgs());
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
      this.GenerateEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, this.FileAdded, new ProjectDocumentsChangeEventArgs());
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
      this.GenerateEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, this.DirectoryRemoved, new ProjectDocumentsChangeEventArgs());
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
      this.GenerateEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, this.FileRemoved, new ProjectDocumentsChangeEventArgs());
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
      this.GenerateEvents(rgpProjects, rgFirstIndices, rgszMkNewNames, this.DirectoryRenamed, new ProjectDocumentsChangeEventArgs());
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
      this.GenerateEvents(rgpProjects, rgFirstIndices, rgszMkNewNames, this.FileRenamed, new ProjectDocumentsChangeEventArgs());
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
      this.GenerateEvents(rgpProjects, rgFirstIndices, rgpszMkDocuments, this.SccStatusChanged, new ProjectDocumentsChangeEventArgs());
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

    private IVsTrackProjectDocuments2 GetTrackProjectDocuments()
    {
      IVsTrackProjectDocuments2 service = ((System.IServiceProvider) this).GetService(typeof (SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;
      if (service == null)
        throw new ApplicationException(string.Format((IFormatProvider) Resources.Culture, Resources.Flavor_FailedToGetService, (object) "SVsTrackProjectDocuments"));
      return service;
    }

    private void GenerateEvents(
      IVsProject[] projects,
      int[] firstFiles,
      string[] mkDocuments,
      FlavoredProject.EventHandler<ProjectDocumentsChangeEventArgs> eventToGenerate,
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
        if (object.ReferenceEquals((object) projects[index], (object) this))
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

    public delegate void EventHandler<ProjectDocumentsChangeEventArgs>(
      object sender,
      ProjectDocumentsChangeEventArgs e);
  }
}
