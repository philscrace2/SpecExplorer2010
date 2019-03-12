// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.VsShellUtilities
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public static class VsShellUtilities
  {
    private const int TV_FIRST = 4352;
    private const int TVM_SETEXTENDEDSTYLE = 4396;
    private const int TVM_GETEXTENDEDSTYLE = 4397;
    private const int TVS_EX_FADEINOUTEXPANDOS = 64;
    private const int TVS_EX_DOUBLEBUFFER = 4;
    private const int LVM_FIRST = 4096;
    private const int LVM_SETEXTENDEDLISTVIEWSTYLE = 4150;
    private const int LVS_EX_DOUBLEBUFFER = 65536;

    public static void RenameDocument(System.IServiceProvider site, string oldName, string newName)
    {
      if (site == null)
        throw new ArgumentException(nameof (site));
      if (string.IsNullOrEmpty(oldName))
        throw new ArgumentException(nameof (oldName));
      if (string.IsNullOrEmpty(newName))
        throw new ArgumentException(nameof (newName));
      IVsRunningDocumentTable service1 = site.GetService(typeof (SVsRunningDocumentTable)) as IVsRunningDocumentTable;
      IVsUIShellOpenDocument service2 = site.GetService(typeof (SVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
      site.GetService(typeof (SVsUIShell));
      if (service1 == null || service2 == null)
        return;
      IVsHierarchy ppHier;
      uint pitemid;
      IntPtr ppunkDocData;
      uint pdwCookie;
      ErrorHandler.ThrowOnFailure(service1.FindAndLockDocument(0U, oldName, out ppHier, out pitemid, out ppunkDocData, out pdwCookie));
      if (!(ppunkDocData != IntPtr.Zero))
        return;
      try
      {
        IntPtr iunknownForObject = Marshal.GetIUnknownForObject((object) ppHier);
        Guid guid = typeof (IVsHierarchy).GUID;
        IntPtr ppv;
        Marshal.QueryInterface(iunknownForObject, ref guid, out ppv);
        try
        {
          ErrorHandler.ThrowOnFailure(service1.RenameDocument(oldName, newName, ppv, pitemid));
        }
        finally
        {
          Marshal.Release(ppv);
          Marshal.Release(iunknownForObject);
        }
        string fileName = Path.GetFileName(newName);
        foreach (IVsWindowFrame vsWindowFrame in VsShellUtilities.GetFramesForDocument(site, Marshal.GetObjectForIUnknown(ppunkDocData)))
          ErrorHandler.ThrowOnFailure(vsWindowFrame.SetProperty(-4001, (object) fileName));
      }
      finally
      {
        Marshal.Release(ppunkDocData);
      }
    }

    public static void OpenDocument(
      System.IServiceProvider provider,
      string fullPath,
      Guid logicalView,
      out IVsUIHierarchy hierarchy,
      out uint itemID,
      out IVsWindowFrame windowFrame,
      out IVsTextView view)
    {
      itemID = uint.MaxValue;
      windowFrame = (IVsWindowFrame) null;
      hierarchy = (IVsUIHierarchy) null;
      view = (IVsTextView) null;
      if (provider == null)
        throw new ArgumentException(nameof (provider));
      if (string.IsNullOrEmpty(fullPath))
        throw new ArgumentException(nameof (fullPath));
      VsShellUtilities.OpenDocument(provider, fullPath, logicalView, out hierarchy, out itemID, out windowFrame);
      view = VsShellUtilities.GetTextView(windowFrame);
    }

    public static void OpenDocument(
      System.IServiceProvider provider,
      string fullPath,
      Guid logicalView,
      out IVsUIHierarchy hierarchy,
      out uint itemID,
      out IVsWindowFrame windowFrame)
    {
      windowFrame = (IVsWindowFrame) null;
      itemID = uint.MaxValue;
      hierarchy = (IVsUIHierarchy) null;
      if (provider == null)
        throw new ArgumentException(nameof (provider));
      if (string.IsNullOrEmpty(fullPath))
        throw new ArgumentException(nameof (fullPath));
      if (!VsShellUtilities.IsDocumentOpen(provider, fullPath, logicalView, out hierarchy, out itemID, out windowFrame))
      {
        IVsUIShellOpenDocument service = provider.GetService(typeof (IVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
        if (service != null)
        {
          Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP;
          uint pitemid;
          ErrorHandler.ThrowOnFailure(service.OpenDocumentViaProject(fullPath, ref logicalView, out ppSP, out hierarchy, out pitemid, out windowFrame));
        }
      }
      if (windowFrame == null)
        return;
      ErrorHandler.ThrowOnFailure(windowFrame.Show());
    }

    public static IVsTextView GetTextView(IVsWindowFrame windowFrame)
    {
      if (windowFrame == null)
        throw new ArgumentException(nameof (windowFrame));
      object pvar;
      ErrorHandler.ThrowOnFailure(windowFrame.GetProperty(-3001, out pvar));
      IVsTextView ppView = pvar as IVsTextView;
      if (ppView == null)
      {
        IVsCodeWindow vsCodeWindow = pvar as IVsCodeWindow;
        if (vsCodeWindow != null)
          ErrorHandler.ThrowOnFailure(vsCodeWindow.GetPrimaryView(out ppView));
      }
      return ppView;
    }

    public static Window GetWindowObject(IVsWindowFrame windowFrame)
    {
      if (windowFrame == null)
        throw new ArgumentException(nameof (windowFrame));
      Window window = (Window) null;
      object pvar;
      ErrorHandler.ThrowOnFailure(windowFrame.GetProperty(-5003, out pvar));
      if (pvar is Window)
        window = (Window) pvar;
      return window;
    }

    public static bool IsDocumentOpen(
      System.IServiceProvider provider,
      string fullPath,
      Guid logicalView,
      out IVsUIHierarchy hierarchy,
      out uint itemID,
      out IVsWindowFrame windowFrame)
    {
      windowFrame = (IVsWindowFrame) null;
      itemID = uint.MaxValue;
      hierarchy = (IVsUIHierarchy) null;
      if (provider == null)
        throw new ArgumentException(nameof (provider));
      if (string.IsNullOrEmpty(fullPath))
        throw new ArgumentException(nameof (fullPath));
      IVsUIShellOpenDocument service1 = provider.GetService(typeof (IVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
      IVsRunningDocumentTable service2 = provider.GetService(typeof (IVsRunningDocumentTable)) as IVsRunningDocumentTable;
      if (service2 != null && service1 != null)
      {
        IntPtr ppunkDocData = IntPtr.Zero;
        uint[] pitemidOpen = new uint[1];
        try
        {
          IVsHierarchy ppHier;
          uint pdwCookie;
          ErrorHandler.ThrowOnFailure(service2.FindAndLockDocument(0U, fullPath, out ppHier, out pitemidOpen[0], out ppunkDocData, out pdwCookie));
          uint grfIDO = logicalView == Guid.Empty ? 2U : 0U;
          int pfOpen;
          ErrorHandler.ThrowOnFailure(service1.IsDocumentOpen((IVsUIHierarchy) ppHier, pitemidOpen[0], fullPath, ref logicalView, grfIDO, out hierarchy, pitemidOpen, out windowFrame, out pfOpen));
          if (windowFrame != null)
          {
            itemID = pitemidOpen[0];
            return pfOpen == 1;
          }
        }
        finally
        {
          if (ppunkDocData != IntPtr.Zero)
            Marshal.Release(ppunkDocData);
        }
      }
      return false;
    }

    public static void OpenAsMiscellaneousFile(
      System.IServiceProvider provider,
      string path,
      string caption,
      Guid editor,
      string physicalView,
      Guid logicalView)
    {
      if (provider == null)
        throw new ArgumentException(nameof (provider));
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(nameof (path));
      IVsProject3 miscellaneousProject = VsShellUtilities.GetMiscellaneousProject(provider);
      VSADDRESULT[] pResult = new VSADDRESULT[1];
      VSADDITEMOPERATION dwAddItemOperation = VSADDITEMOPERATION.VSADDITEMOP_CLONEFILE;
      __VSCREATEEDITORFLAGS vscreateeditorflags = __VSCREATEEDITORFLAGS.CEF_CLONEFILE;
      ErrorHandler.ThrowOnFailure(miscellaneousProject.AddItemWithSpecific(uint.MaxValue, dwAddItemOperation, caption, 1U, new string[1]
      {
        path
      }, IntPtr.Zero, (uint) vscreateeditorflags, ref editor, physicalView, ref logicalView, pResult));
      if (pResult[0] != VSADDRESULT.ADDRESULT_Success)
        throw new ApplicationException(pResult[0].ToString());
    }

    public static IVsProject3 GetMiscellaneousProject(System.IServiceProvider provider)
    {
      if (provider == null)
        throw new ArgumentNullException(nameof (provider));
      IVsExternalFilesManager service = (IVsExternalFilesManager) provider.GetService(typeof (SVsExternalFilesManager));
      if (service == null)
        return (IVsProject3) null;
      IVsProject ppProject;
      ErrorHandler.ThrowOnFailure(service.GetExternalFilesProject(out ppProject));
      return ppProject as IVsProject3;
    }

    public static IVsProject3 GetMiscellaneousProject(
      System.IServiceProvider provider,
      bool create)
    {
      if (provider == null)
        throw new ArgumentNullException(nameof (provider));
      IVsHierarchy ppHierarchy = (IVsHierarchy) null;
      Guid miscellaneousFilesProject = VSConstants.CLSID_MiscellaneousFilesProject;
      if ((Microsoft.VisualStudio.NativeMethods.Failed(((IVsSolution2) provider.GetService(typeof (SVsSolution))).GetProjectOfGuid(ref miscellaneousFilesProject, out ppHierarchy)) || ppHierarchy == null) && create)
        return VsShellUtilities.GetMiscellaneousProject(provider);
      return ppHierarchy as IVsProject3;
    }

    public static void OpenDocument(System.IServiceProvider provider, string path)
    {
      if (provider == null)
        throw new ArgumentException(nameof (provider));
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(nameof (path));
      Guid empty = Guid.Empty;
      IVsUIHierarchy hierarchy;
      uint itemID;
      IVsWindowFrame windowFrame;
      VsShellUtilities.OpenDocument(provider, path, empty, out hierarchy, out itemID, out windowFrame);
      windowFrame = (IVsWindowFrame) null;
      hierarchy = (IVsUIHierarchy) null;
    }

    public static IVsWindowFrame OpenDocumentWithSpecificEditor(
      System.IServiceProvider provider,
      string fullPath,
      Guid editorType,
      Guid logicalView)
    {
      if (provider == null)
        throw new ArgumentException(nameof (provider));
      if (string.IsNullOrEmpty(fullPath))
        throw new ArgumentException(nameof (fullPath));
      IVsUIHierarchy hierarchy;
      uint itemID;
      IVsWindowFrame windowFrame;
      VsShellUtilities.OpenDocumentWithSpecificEditor(provider, fullPath, editorType, logicalView, out hierarchy, out itemID, out windowFrame);
      hierarchy = (IVsUIHierarchy) null;
      return windowFrame;
    }

    public static void OpenDocumentWithSpecificEditor(
      System.IServiceProvider provider,
      string fullPath,
      Guid editorType,
      Guid logicalView,
      out IVsUIHierarchy hierarchy,
      out uint itemID,
      out IVsWindowFrame windowFrame)
    {
      windowFrame = (IVsWindowFrame) null;
      itemID = uint.MaxValue;
      hierarchy = (IVsUIHierarchy) null;
      if (provider == null)
        throw new ArgumentException(nameof (provider));
      if (string.IsNullOrEmpty(fullPath))
        throw new ArgumentException(nameof (fullPath));
      IVsUIShellOpenDocument service1 = provider.GetService(typeof (IVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
      IVsRunningDocumentTable service2 = provider.GetService(typeof (IVsRunningDocumentTable)) as IVsRunningDocumentTable;
      string pbstrPhysicalView = (string) null;
      if (service2 == null || service1 == null)
        return;
      ErrorHandler.ThrowOnFailure(service1.MapLogicalView(ref editorType, ref logicalView, out pbstrPhysicalView));
      IntPtr ppunkDocData = IntPtr.Zero;
      try
      {
        IVsHierarchy ppHier;
        uint pdwCookie;
        ErrorHandler.ThrowOnFailure(service2.FindAndLockDocument(0U, fullPath, out ppHier, out itemID, out ppunkDocData, out pdwCookie));
        uint grfIDO = 1;
        int pfOpen;
        if (ErrorHandler.Succeeded(service1.IsSpecificDocumentViewOpen((IVsUIHierarchy) ppHier, itemID, fullPath, ref editorType, pbstrPhysicalView, grfIDO, out hierarchy, out itemID, out windowFrame, out pfOpen)))
        {
          if (pfOpen == 1)
            return;
        }
      }
      finally
      {
        if (ppunkDocData != IntPtr.Zero)
          Marshal.Release(ppunkDocData);
      }
      uint grfEditorFlags = 3;
      Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP;
      ErrorHandler.ThrowOnFailure(service1.OpenDocumentViaProjectWithSpecific(fullPath, grfEditorFlags, ref editorType, pbstrPhysicalView, ref logicalView, out ppSP, out hierarchy, out itemID, out windowFrame));
      if (windowFrame != null)
        ErrorHandler.ThrowOnFailure(windowFrame.Show());
      ppSP = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider) null;
    }

    public static IVsHierarchy GetProject(System.IServiceProvider site, string moniker)
    {
      if (site == null)
        throw new ArgumentException(nameof (site));
      if (string.IsNullOrEmpty(moniker))
        throw new ArgumentException(nameof (moniker));
      IVsUIShellOpenDocument service = site.GetService(typeof (SVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
      IVsUIHierarchy ppUIH = (IVsUIHierarchy) null;
      uint pitemid;
      Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP;
      int pDocInProj;
      ErrorHandler.ThrowOnFailure(service.IsDocumentInAProject(moniker, out ppUIH, out pitemid, out ppSP, out pDocInProj));
      return (IVsHierarchy) ppUIH;
    }

    public static string GetRunningDocumentContents(System.IServiceProvider site, string path)
    {
      if (site == null)
        throw new ArgumentException(nameof (site));
      if (string.IsNullOrEmpty(path))
        throw new ArgumentException(nameof (path));
      string pbstrBuf = (string) null;
      IVsRunningDocumentTable service = (IVsRunningDocumentTable) site.GetService(typeof (SVsRunningDocumentTable));
      if (service != null)
      {
        IntPtr ppunkDocData = IntPtr.Zero;
        try
        {
          IVsHierarchy ppHier;
          uint pitemid;
          uint pdwCookie;
          ErrorHandler.ThrowOnFailure(service.FindAndLockDocument(0U, path, out ppHier, out pitemid, out ppunkDocData, out pdwCookie));
          if (ppunkDocData != IntPtr.Zero)
          {
            object objectForIunknown = Marshal.GetObjectForIUnknown(ppunkDocData);
            IVsTextLines ppTextBuffer = (IVsTextLines) null;
            if (objectForIunknown is IVsTextLines)
              ppTextBuffer = (IVsTextLines) objectForIunknown;
            else if (objectForIunknown is IVsTextBufferProvider && ((IVsTextBufferProvider) objectForIunknown).GetTextBuffer(out ppTextBuffer) != 0)
              ppTextBuffer = (IVsTextLines) null;
            if (ppTextBuffer != null)
            {
              int piLine;
              int piIndex;
              ErrorHandler.ThrowOnFailure(ppTextBuffer.GetLastLineIndex(out piLine, out piIndex));
              ErrorHandler.ThrowOnFailure(ppTextBuffer.GetLineText(0, 0, piLine, piIndex, out pbstrBuf));
              ppTextBuffer = (IVsTextLines) null;
              return pbstrBuf;
            }
          }
        }
        finally
        {
          if (ppunkDocData != IntPtr.Zero)
            Marshal.Release(ppunkDocData);
        }
      }
      return (string) null;
    }

    public static void GetRDTDocumentInfo(
      System.IServiceProvider site,
      string documentName,
      out IVsHierarchy hierarchy,
      out uint itemid,
      out IVsPersistDocData persistDocData,
      out uint docCookie)
    {
      hierarchy = (IVsHierarchy) null;
      itemid = uint.MaxValue;
      persistDocData = (IVsPersistDocData) null;
      docCookie = 0U;
      if (site == null)
        throw new ArgumentException(nameof (site));
      if (string.IsNullOrEmpty(documentName))
        throw new ArgumentException(nameof (documentName));
      IVsRunningDocumentTable service = site.GetService(typeof (IVsRunningDocumentTable)) as IVsRunningDocumentTable;
      if (service == null)
        return;
      IntPtr ppunkDocData = IntPtr.Zero;
      try
      {
        ErrorHandler.ThrowOnFailure(service.FindAndLockDocument(0U, documentName, out hierarchy, out itemid, out ppunkDocData, out docCookie));
        if (!(ppunkDocData != IntPtr.Zero))
          return;
        persistDocData = Marshal.GetObjectForIUnknown(ppunkDocData) as IVsPersistDocData;
      }
      finally
      {
        if (ppunkDocData != IntPtr.Zero)
          Marshal.Release(ppunkDocData);
      }
    }

    private static List<IVsWindowFrame> GetFramesForDocument(
      System.IServiceProvider site,
      object docData)
    {
      List<IVsWindowFrame> vsWindowFrameList = new List<IVsWindowFrame>();
      IVsRunningDocumentTable service1 = site.GetService(typeof (SVsRunningDocumentTable)) as IVsRunningDocumentTable;
      IVsUIShell service2 = site.GetService(typeof (SVsUIShell)) as IVsUIShell;
      if (service2 == null || service1 == null)
        return vsWindowFrameList;
      IEnumWindowFrames ppenum;
      ErrorHandler.ThrowOnFailure(service2.GetDocumentWindowEnum(out ppenum));
      IVsWindowFrame[] rgelt = new IVsWindowFrame[16];
label_3:
      uint pceltFetched;
      ErrorHandler.ThrowOnFailure(ppenum.Next((uint) rgelt.Length, rgelt, out pceltFetched));
      if (pceltFetched == 0U)
        return vsWindowFrameList;
      for (int index = 0; (long) index < (long) pceltFetched; ++index)
      {
        IVsWindowFrame vsWindowFrame = rgelt[index];
        object pvar = (object) ErrorHandler.ThrowOnFailure(vsWindowFrame.GetProperty(-4004, out pvar));
        if (Microsoft.VisualStudio.NativeMethods.IsSameComObject(pvar, docData))
          vsWindowFrameList.Add(vsWindowFrame);
      }
      goto label_3;
    }

    public static void SaveFileIfDirty(System.IServiceProvider site, string fullPath)
    {
      if (site == null)
        throw new ArgumentException(nameof (site));
      if (string.IsNullOrEmpty(fullPath))
        throw new ArgumentException(nameof (fullPath));
      IVsRunningDocumentTable service = (IVsRunningDocumentTable) site.GetService(typeof (SVsRunningDocumentTable));
      if (service != null)
      {
        IVsHierarchy ppHier;
        uint pitemid;
        IntPtr ppunkDocData;
        uint pdwCookie;
        ErrorHandler.ThrowOnFailure(service.FindAndLockDocument(0U, fullPath, out ppHier, out pitemid, out ppunkDocData, out pdwCookie));
        if (ppunkDocData != IntPtr.Zero)
        {
          try
          {
            object objectForIunknown = Marshal.GetObjectForIUnknown(ppunkDocData);
            IVsPersistDocData2 vsPersistDocData2 = (IVsPersistDocData2) objectForIunknown;
            int pfDirty;
            ErrorHandler.ThrowOnFailure(vsPersistDocData2.IsDocDataDirty(out pfDirty));
            if (pfDirty != 0)
            {
              string pbstrMkDocumentNew;
              int pfSaveCanceled;
              int hr = vsPersistDocData2.SaveDocData(VSSAVEFLAGS.VSSAVE_Save, out pbstrMkDocumentNew, out pfSaveCanceled);
              if (pfSaveCanceled <= 0)
                ErrorHandler.ThrowOnFailure(hr);
              if (!string.IsNullOrEmpty(pbstrMkDocumentNew))
              {
                string fileName = Path.GetFileName(pbstrMkDocumentNew);
                foreach (IVsWindowFrame vsWindowFrame in VsShellUtilities.GetFramesForDocument(site, objectForIunknown))
                  ErrorHandler.ThrowOnFailure(vsWindowFrame.SetProperty(-4001, (object) fileName));
              }
            }
          }
          finally
          {
            Marshal.Release(ppunkDocData);
          }
        }
        ppHier = (IVsHierarchy) null;
      }
    }

    public static void SaveFileIfDirty(IVsTextView view)
    {
      if (view == null)
        throw new ArgumentException(nameof (view));
      IVsTextLines ppBuffer;
      ErrorHandler.ThrowOnFailure(view.GetBuffer(out ppBuffer));
      IVsPersistDocData2 vsPersistDocData2 = (IVsPersistDocData2) ppBuffer;
      int pfDirty;
      ErrorHandler.ThrowOnFailure(vsPersistDocData2.IsDocDataDirty(out pfDirty));
      if (pfDirty != 0)
      {
        string pbstrMkDocumentNew;
        int pfSaveCanceled;
        int hr = vsPersistDocData2.SaveDocData(VSSAVEFLAGS.VSSAVE_Save, out pbstrMkDocumentNew, out pfSaveCanceled);
        if (pfSaveCanceled <= 0)
          ErrorHandler.ThrowOnFailure(hr);
        if (!string.IsNullOrEmpty(pbstrMkDocumentNew))
        {
          string fileName = Path.GetFileName(pbstrMkDocumentNew);
          IVsTextViewEx vsTextViewEx = view as IVsTextViewEx;
          if (vsTextViewEx != null)
          {
            object ppFrame;
            vsTextViewEx.GetWindowFrame(out ppFrame);
            IVsWindowFrame vsWindowFrame = ppFrame as IVsWindowFrame;
            if (vsWindowFrame != null)
              ErrorHandler.ThrowOnFailure(vsWindowFrame.SetProperty(-4001, (object) fileName));
          }
        }
      }
      ppBuffer = (IVsTextLines) null;
    }

    public static bool PromptYesNo(
      string message,
      string title,
      OLEMSGICON icon,
      IVsUIShell uiShell)
    {
      Guid empty = Guid.Empty;
      int pnResult = 0;
      ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(0U, ref empty, title, message, (string) null, 0U, OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_SECOND, icon, 0, out pnResult));
      return pnResult == 6;
    }

    public static int ShowMessageBox(
      System.IServiceProvider serviceProvider,
      string message,
      string title,
      OLEMSGICON icon,
      OLEMSGBUTTON msgButton,
      OLEMSGDEFBUTTON defaultButton)
    {
      if (serviceProvider == null)
        throw new ArgumentException(nameof (serviceProvider));
      IVsUIShell service = serviceProvider.GetService(typeof (IVsUIShell)) as IVsUIShell;
      if (service == null)
        throw new InvalidOperationException();
      Guid empty = Guid.Empty;
      int pnResult = 0;
      ErrorHandler.ThrowOnFailure(service.ShowMessageBox(0U, ref empty, title, message, (string) null, 0U, msgButton, defaultButton, icon, 0, out pnResult));
      return pnResult;
    }

    [Obsolete("This method is obsolete. Please use GetTaskItems2 instead.")]
    public static IList<IVsTaskItem2> GetTaskItems(System.IServiceProvider serviceProvider)
    {
      IList<IVsTaskItem2> vsTaskItem2List = (IList<IVsTaskItem2>) new List<IVsTaskItem2>();
      foreach (IVsTaskItem vsTaskItem in (IEnumerable<IVsTaskItem>) VsShellUtilities.GetTaskItems2(serviceProvider))
        vsTaskItem2List.Add(vsTaskItem as IVsTaskItem2);
      return vsTaskItem2List;
    }

    public static IList<IVsTaskItem> GetTaskItems2(System.IServiceProvider serviceProvider)
    {
      IList<IVsTaskItem> vsTaskItemList = (IList<IVsTaskItem>) new List<IVsTaskItem>();
      if (serviceProvider == null)
        throw new ArgumentException(nameof (serviceProvider));
      IVsTaskList service = serviceProvider.GetService(typeof (SVsTaskList)) as IVsTaskList;
      if (service == null)
        throw new InvalidOperationException();
      try
      {
        IVsEnumTaskItems ppenum;
        ErrorHandler.ThrowOnFailure(service.EnumTaskItems(out ppenum));
        if (ppenum == null)
          return vsTaskItemList;
        uint[] pceltFetched = new uint[1];
        do
        {
          IVsTaskItem[] rgelt = new IVsTaskItem[1];
          int num = ppenum.Next(1U, rgelt, pceltFetched);
          if (pceltFetched[0] == 1U)
          {
            IVsTaskItem vsTaskItem = rgelt[0];
            vsTaskItemList.Add(vsTaskItem);
          }
          if (num != 0)
            break;
        }
        while (pceltFetched[0] == 1U);
      }
      catch (COMException ex)
      {
        Trace.WriteLine("Exception : " + ex.Message);
      }
      return vsTaskItemList;
    }

    public static int EmptyTaskList(System.IServiceProvider serviceProvider)
    {
      if (serviceProvider == null)
        throw new ArgumentException(nameof (serviceProvider));
      IVsTaskList service = serviceProvider.GetService(typeof (IVsTaskList)) as IVsTaskList;
      if (service == null)
        throw new InvalidOperationException();
      int num;
      try
      {
        IVsEnumTaskItems ppenum;
        ErrorHandler.ThrowOnFailure(service.EnumTaskItems(out ppenum));
        if (ppenum == null)
          throw new InvalidOperationException();
        uint[] pceltFetched = new uint[1];
        do
        {
          IVsTaskItem[] rgelt = new IVsTaskItem[1];
          num = ppenum.Next(1U, rgelt, pceltFetched);
          if (pceltFetched[0] == 1U)
          {
            IVsTaskItem2 vsTaskItem2 = rgelt[0] as IVsTaskItem2;
            if (vsTaskItem2 != null)
            {
              int pfCanDelete;
              ErrorHandler.ThrowOnFailure(vsTaskItem2.CanDelete(out pfCanDelete));
              if (pfCanDelete == 1)
                ErrorHandler.ThrowOnFailure(vsTaskItem2.OnDeleteTask());
            }
          }
          if (num != 0)
            break;
        }
        while (pceltFetched[0] == 1U);
      }
      catch (COMException ex)
      {
        Trace.WriteLine("Exception : " + ex.Message);
        num = ex.ErrorCode;
      }
      return num;
    }

    public static void LaunchDebugger(System.IServiceProvider serviceProvider, VsDebugTargetInfo info)
    {
      if (serviceProvider == null)
        throw new ArgumentException(nameof (serviceProvider));
      info.cbSize = (uint) Marshal.SizeOf((object) info);
      IntPtr num = Marshal.AllocCoTaskMem((int) info.cbSize);
      Marshal.StructureToPtr((object) info, num, false);
      try
      {
        IVsDebugger service = serviceProvider.GetService(typeof (IVsDebugger)) as IVsDebugger;
        if (service == null)
          throw new InvalidOperationException();
        ErrorHandler.ThrowOnFailure(service.LaunchDebugTargets(1U, num));
      }
      catch (COMException ex)
      {
        Trace.WriteLine("Exception : " + ex.Message);
      }
      finally
      {
        if (num != IntPtr.Zero)
          Marshal.FreeCoTaskMem(num);
      }
    }

    public static IVsHierarchy GetHierarchy(System.IServiceProvider site, Guid projectGuid)
    {
      if (site == null)
        throw new ArgumentException(nameof (site));
      if (projectGuid == Guid.Empty)
        throw new ArgumentException(nameof (projectGuid));
      IVsHierarchy ppHierarchy = (IVsHierarchy) null;
      IVsSolution service = site.GetService(typeof (SVsSolution)) as IVsSolution;
      if (service == null)
        throw new InvalidOperationException();
      try
      {
        service.GetProjectOfGuid(ref projectGuid, out ppHierarchy);
      }
      catch (COMException ex)
      {
        Trace.WriteLine("Exception :" + ex.Message);
      }
      catch (InvalidCastException ex)
      {
        Trace.WriteLine("Exception :" + ex.Message);
      }
      return ppHierarchy;
    }

    public static IVsUIHierarchyWindow GetUIHierarchyWindow(
      System.IServiceProvider serviceProvider,
      Guid guidPersistenceSlot)
    {
      if (serviceProvider == null)
        throw new ArgumentException(nameof (serviceProvider));
      IVsUIShell service = serviceProvider.GetService(typeof (SVsUIShell)) as IVsUIShell;
      if (service == null)
        throw new InvalidOperationException();
      object pvar = (object) null;
      IVsWindowFrame ppWindowFrame = (IVsWindowFrame) null;
      IVsUIHierarchyWindow uiHierarchyWindow = (IVsUIHierarchyWindow) null;
      try
      {
        ErrorHandler.ThrowOnFailure(service.FindToolWindow(0U, ref guidPersistenceSlot, out ppWindowFrame));
        ErrorHandler.ThrowOnFailure(ppWindowFrame.GetProperty(-3001, out pvar));
      }
      catch (COMException ex)
      {
        Trace.WriteLine("Exception :" + ex.Message);
      }
      finally
      {
        if (pvar != null)
          uiHierarchyWindow = (IVsUIHierarchyWindow) pvar;
      }
      return uiHierarchyWindow;
    }

    public static IVsOutputWindowPane GetOutputWindowPane(
      System.IServiceProvider serviceProvider,
      Guid guidPane)
    {
      if (serviceProvider == null)
        throw new ArgumentException(nameof (serviceProvider));
      IVsOutputWindow service = serviceProvider.GetService(typeof (IVsOutputWindow)) as IVsOutputWindow;
      if (service == null)
        throw new InvalidOperationException();
      IVsOutputWindowPane ppPane = (IVsOutputWindowPane) null;
      try
      {
        ErrorHandler.ThrowOnFailure(service.GetPane(ref guidPane, out ppPane));
      }
      catch (COMException ex)
      {
        Trace.WriteLine("Exception :" + ex.Message);
      }
      return ppPane;
    }

    public static DBGMODE GetDebugMode(System.IServiceProvider serviceProvider)
    {
      DBGMODE[] pdbgmode = new DBGMODE[1];
      if (serviceProvider == null)
        throw new ArgumentException(nameof (serviceProvider));
      IVsDebugger service = serviceProvider.GetService(typeof (IVsDebugger)) as IVsDebugger;
      if (service == null)
        throw new InvalidOperationException();
      try
      {
        ErrorHandler.ThrowOnFailure(service.GetMode(pdbgmode));
      }
      catch (COMException ex)
      {
        Trace.WriteLine("Exception :" + ex.Message);
      }
      return pdbgmode[0];
    }

    public static bool IsVisualStudioInDesignMode(System.IServiceProvider serviceProvider)
    {
      if (serviceProvider == null)
        throw new ArgumentException(nameof (serviceProvider));
      return (VsShellUtilities.GetDebugMode(serviceProvider) & ~DBGMODE.DBGMODE_EncMask) == DBGMODE.DBGMODE_Design;
    }

    public static bool IsSolutionBuilding(System.IServiceProvider serviceProvider)
    {
      if (serviceProvider == null)
        throw new ArgumentException(nameof (serviceProvider));
      IVsSolutionBuildManager service = serviceProvider.GetService(typeof (IVsSolutionBuildManager)) as IVsSolutionBuildManager;
      if (service == null)
        throw new InvalidOperationException();
      int pfBuildManagerBusy = 0;
      ErrorHandler.ThrowOnFailure(service.QueryBuildManagerBusy(out pfBuildManagerBusy));
      return pfBuildManagerBusy == 1;
    }

    public static bool IsInAutomationFunction(System.IServiceProvider serviceProvider)
    {
      if (serviceProvider == null)
        throw new ArgumentException(nameof (serviceProvider));
      IVsExtensibility service = serviceProvider.GetService(typeof (IVsExtensibility)) as IVsExtensibility;
      if (service == null)
        throw new InvalidOperationException();
      return service.IsInAutomationFunction() != 0;
    }

    private static int TreeView_GetExtendedStyle(IntPtr handle)
    {
      return Microsoft.VisualStudio.NativeMethods.SendMessage(handle, 4397, IntPtr.Zero, IntPtr.Zero).ToInt32();
    }

    private static void TreeView_SetExtendedStyle(IntPtr handle, int extendedStyle, int mask)
    {
      Microsoft.VisualStudio.NativeMethods.SendMessage(handle, 4396, new IntPtr(mask), new IntPtr(extendedStyle));
    }

    public static void ApplyTreeViewThemeStyles(TreeView treeView)
    {
      VsShellUtilities.ApplyTreeViewThemeStyles(treeView, true);
    }

    public static void ApplyTreeViewThemeStyles(TreeView treeView, bool enableHotTracking)
    {
      if (treeView == null)
        throw new ArgumentNullException(nameof (treeView));
      treeView.HotTracking = enableHotTracking;
      treeView.ShowLines = false;
      IntPtr handle = treeView.Handle;
      Microsoft.VisualStudio.NativeMethods.SetWindowTheme(handle, "Explorer", (string) null);
      int extendedStyle = VsShellUtilities.TreeView_GetExtendedStyle(handle) | 68;
      VsShellUtilities.TreeView_SetExtendedStyle(handle, extendedStyle, 0);
    }

    private static void ListView_SetExtendedListViewStyleEx(
      IntPtr handle,
      int mask,
      int extendedStyle)
    {
      Microsoft.VisualStudio.NativeMethods.SendMessage(handle, 4150, new IntPtr(mask), new IntPtr(extendedStyle));
    }

    public static void ApplyListViewThemeStyles(ListView listView)
    {
      if (listView == null)
        throw new ArgumentNullException(nameof (listView));
      IntPtr handle = listView.Handle;
      Microsoft.VisualStudio.NativeMethods.SetWindowTheme(handle, "Explorer", (string) null);
      VsShellUtilities.ListView_SetExtendedListViewStyleEx(handle, 65536, 65536);
    }

    public static Font GetEnvironmentFont(System.IServiceProvider provider)
    {
      if (provider == null)
        throw new ArgumentNullException(nameof (provider));
      IUIService service = provider.GetService(typeof (IUIService)) as IUIService;
      if (service == null)
        throw new InvalidOperationException();
      return (Font) service.Styles[(object) "DialogFont"];
    }
  }
}
