// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.RunningDocumentTable
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell
{
  public class RunningDocumentTable : IEnumerable<RunningDocumentInfo>, IEnumerable
  {
    private IServiceProvider site;
    private IVsRunningDocumentTable rdt;

    public RunningDocumentTable(IServiceProvider site)
    {
      this.site = site;
      this.rdt = site.GetService(typeof (SVsRunningDocumentTable)) as IVsRunningDocumentTable;
      if (this.rdt == null)
        throw new NotSupportedException(typeof (SVsRunningDocumentTable).FullName);
    }

    public object FindDocument(string moniker)
    {
      IVsHierarchy hierarchy;
      uint itemid;
      uint docCookie;
      return this.FindDocument(moniker, out hierarchy, out itemid, out docCookie);
    }

    [CLSCompliant(false)]
    public object FindDocument(string moniker, out uint docCookie)
    {
      IVsHierarchy hierarchy;
      uint itemid;
      return this.FindDocument(moniker, out hierarchy, out itemid, out docCookie);
    }

    [CLSCompliant(false)]
    public object FindDocument(
      string moniker,
      out IVsHierarchy hierarchy,
      out uint itemid,
      out uint docCookie)
    {
      itemid = 0U;
      hierarchy = (IVsHierarchy) null;
      docCookie = 0U;
      if (this.rdt == null)
        return (object) null;
      IntPtr ppunkDocData = IntPtr.Zero;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.rdt.FindAndLockDocument(0U, moniker, out hierarchy, out itemid, out ppunkDocData, out docCookie));
      if (ppunkDocData == IntPtr.Zero)
        return (object) null;
      try
      {
        return Marshal.GetObjectForIUnknown(ppunkDocData);
      }
      finally
      {
        Marshal.Release(ppunkDocData);
      }
    }

    [CLSCompliant(false)]
    public IVsHierarchy GetHierarchyItem(string moniker)
    {
      IVsHierarchy hierarchy;
      uint itemid;
      uint docCookie;
      this.FindDocument(moniker, out hierarchy, out itemid, out docCookie);
      return hierarchy;
    }

    public string GetRunningDocumentContents(string path)
    {
      object document = this.FindDocument(path);
      if (document != null)
        return RunningDocumentTable.GetBufferContents(document);
      return (string) null;
    }

    private static string GetBufferContents(object docDataObj)
    {
      string pbstrBuf = (string) null;
      IVsTextLines ppTextBuffer = (IVsTextLines) null;
      if (docDataObj is IVsTextLines)
        ppTextBuffer = (IVsTextLines) docDataObj;
      else if (docDataObj is IVsTextBufferProvider && ((IVsTextBufferProvider) docDataObj).GetTextBuffer(out ppTextBuffer) != 0)
        ppTextBuffer = (IVsTextLines) null;
      if (ppTextBuffer != null)
      {
        int piLine;
        int piIndex;
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(ppTextBuffer.GetLastLineIndex(out piLine, out piIndex));
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(ppTextBuffer.GetLineText(0, 0, piLine, piIndex, out pbstrBuf));
      }
      return pbstrBuf;
    }

    [CLSCompliant(false)]
    public string GetRunningDocumentContents(uint docCookie)
    {
      uint pgrfRDTFlags;
      uint pdwReadLocks;
      uint pdwEditLocks;
      string pbstrMkDocument;
      IVsHierarchy ppHier;
      uint pitemid;
      IntPtr ppunkDocData;
      if (this.rdt.GetDocumentInfo(docCookie, out pgrfRDTFlags, out pdwReadLocks, out pdwEditLocks, out pbstrMkDocument, out ppHier, out pitemid, out ppunkDocData) == 0)
      {
        if (ppunkDocData != IntPtr.Zero)
        {
          try
          {
            return RunningDocumentTable.GetBufferContents(Marshal.GetObjectForIUnknown(ppunkDocData));
          }
          finally
          {
            Marshal.Release(ppunkDocData);
          }
        }
      }
      return "";
    }

    [CLSCompliant(false)]
    public RunningDocumentInfo GetDocumentInfo(uint docCookie)
    {
      RunningDocumentInfo runningDocumentInfo = new RunningDocumentInfo();
      runningDocumentInfo.DocCookie = docCookie;
      IntPtr ppunkDocData;
      if (this.rdt.GetDocumentInfo(docCookie, out runningDocumentInfo.Flags, out runningDocumentInfo.ReadLocks, out runningDocumentInfo.EditLocks, out runningDocumentInfo.Moniker, out runningDocumentInfo.Hierarchy, out runningDocumentInfo.ItemId, out ppunkDocData) != 0)
        return runningDocumentInfo;
      try
      {
        if (ppunkDocData != IntPtr.Zero)
          runningDocumentInfo.DocData = Marshal.GetObjectForIUnknown(ppunkDocData);
        return runningDocumentInfo;
      }
      finally
      {
        Marshal.Release(ppunkDocData);
      }
    }

    public string SaveFileIfDirty(string fullPath)
    {
      object document = this.FindDocument(fullPath);
      if (document is IVsPersistDocData2)
      {
        IVsPersistDocData2 vsPersistDocData2 = (IVsPersistDocData2) document;
        int pfDirty = 0;
        if (Microsoft.VisualStudio.NativeMethods.Succeeded(vsPersistDocData2.IsDocDataDirty(out pfDirty)) && pfDirty != 0)
        {
          string pbstrMkDocumentNew;
          int pfSaveCanceled;
          Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(vsPersistDocData2.SaveDocData(VSSAVEFLAGS.VSSAVE_Save, out pbstrMkDocumentNew, out pfSaveCanceled));
          return pbstrMkDocumentNew;
        }
      }
      return fullPath;
    }

    [CLSCompliant(false)]
    public void RenameDocument(
      string oldName,
      string newName,
      IVsHierarchy pIVsHierarchy,
      uint itemId)
    {
      IntPtr iunknownForObject = Marshal.GetIUnknownForObject((object) pIVsHierarchy);
      if (!(iunknownForObject != IntPtr.Zero))
        return;
      try
      {
        IntPtr ppv = IntPtr.Zero;
        Guid guid = typeof (IVsHierarchy).GUID;
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(Marshal.QueryInterface(iunknownForObject, ref guid, out ppv));
        try
        {
          Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.rdt.RenameDocument(oldName, newName, ppv, itemId));
        }
        finally
        {
          Marshal.Release(ppv);
        }
      }
      finally
      {
        Marshal.Release(iunknownForObject);
      }
    }

    [CLSCompliant(false)]
    public uint Advise(IVsRunningDocTableEvents sink)
    {
      uint pdwCookie;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.rdt.AdviseRunningDocTableEvents(sink, out pdwCookie));
      return pdwCookie;
    }

    [CLSCompliant(false)]
    public void Unadvise(uint cookie)
    {
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.rdt.UnadviseRunningDocTableEvents(cookie));
    }

    [CLSCompliant(false)]
    public uint RegisterAndLockDocument(
      _VSRDTFLAGS lockType,
      string mkDocument,
      IVsHierarchy hierarchy,
      uint itemid,
      IntPtr docData)
    {
      uint pdwCookie;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.rdt.RegisterAndLockDocument((uint) lockType, mkDocument, hierarchy, itemid, docData, out pdwCookie));
      return pdwCookie;
    }

    [CLSCompliant(false)]
    public void LockDocument(_VSRDTFLAGS lockType, uint cookie)
    {
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.rdt.LockDocument((uint) lockType, cookie));
    }

    [CLSCompliant(false)]
    public void UnlockDocument(_VSRDTFLAGS lockType, uint cookie)
    {
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.rdt.UnlockDocument((uint) lockType, cookie));
    }

    public IEnumerator<RunningDocumentInfo> GetEnumerator()
    {
      IList<RunningDocumentInfo> runningDocumentInfoList = (IList<RunningDocumentInfo>) new List<RunningDocumentInfo>();
      IEnumRunningDocuments ppenum;
      if (Microsoft.VisualStudio.NativeMethods.Succeeded(this.rdt.GetRunningDocumentsEnum(out ppenum)))
      {
        uint[] rgelt = new uint[1];
        uint pceltFetched = 0;
        while (Microsoft.VisualStudio.NativeMethods.Succeeded(ppenum.Next(1U, rgelt, out pceltFetched)) && pceltFetched == 1U)
          runningDocumentInfoList.Add(this.GetDocumentInfo(rgelt[0]));
      }
      return runningDocumentInfoList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
