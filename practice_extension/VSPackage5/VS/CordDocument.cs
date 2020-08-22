// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.CordDocument
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.SpecExplorer.VS
{
  public class CordDocument : IVsTextLinesEvents, IDisposable
  {
    private SpecExplorerPackage package;
    private uint textLinesEventCookie;

    internal string FileName { get; set; }

    internal IVsHierarchy Hierarchy { get; private set; }

    internal uint HierarchyItem { get; private set; }

    internal IVsCodeWindow VsCodeWindow { get; private set; }

    internal IVsTextBuffer VsTextBuffer { get; private set; }

    internal string ShortName
    {
      get
      {
        return Path.GetFileName(this.FileName);
      }
    }

    internal CordDocument(
      SpecExplorerPackage package,
      Microsoft.VisualStudio.OLE.Interop.IServiceProvider oleServiceProvider,
      string fileName,
      IVsHierarchy hierarchy,
      uint hierarchyItem)
    {
      this.package = package;
      this.FileName = fileName;
      this.Hierarchy = hierarchy;
      this.HierarchyItem = hierarchyItem;
      this.CreateCodeEditor(oleServiceProvider);
    }

    internal Guid GetCommandUIGuid()
    {
      return VSConstants.GUID_TextEditorFactory;
    }

    private void CreateCodeEditor(Microsoft.VisualStudio.OLE.Interop.IServiceProvider oleServiceProvider)
    {
      Guid guid1 = typeof (VsCodeWindowClass).GUID;
      Guid guid2 = typeof (IVsCodeWindow).GUID;
      Guid guid3 = typeof (VsTextBufferClass).GUID;
      Guid iidIunknown = VSConstants.IID_IUnknown;
      this.VsTextBuffer = (IVsTextBuffer) this.package.CreateInstance(ref guid3, ref iidIunknown, typeof (object));
      ((IObjectWithSite) this.VsTextBuffer).SetSite((object) oleServiceProvider);
      this.VsCodeWindow = (IVsCodeWindow) this.package.CreateInstance(ref guid1, ref guid2, typeof (IVsCodeWindow));
      INITVIEW[] pInitView = new INITVIEW[1];
      pInitView[0].fSelectionMargin = 1U;
      pInitView[0].fWidgetMargin = 0U;
      pInitView[0].fVirtualSpace = 0U;
      pInitView[0].fDragDropMove = 1U;
      this.package.AssertOk(((IVsCodeWindowEx) this.VsCodeWindow).Initialize(3U, VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Filter, (string) null, (string) null, 135U, pInitView));
      IVsTextLines vsTextBuffer = this.VsTextBuffer as IVsTextLines;
      this.package.Assert(vsTextBuffer != null);
      this.package.AssertOk(this.VsCodeWindow.SetBuffer(vsTextBuffer));
      this.AdviseTextLinesEvents();
    }

    private IConnectionPoint GetTextLinesEventConnectionPoint()
    {
      IConnectionPointContainer vsTextBuffer = this.VsTextBuffer as IConnectionPointContainer;
      this.package.Assert(vsTextBuffer != null, "Unable to cast text buffer to IConnectionPointContainer");
      Guid guid = typeof (IVsTextLinesEvents).GUID;
      IConnectionPoint ppCP;
      vsTextBuffer.FindConnectionPoint(ref guid, out ppCP);
      return ppCP;
    }

    private void AdviseTextLinesEvents()
    {
      IConnectionPoint eventConnectionPoint = this.GetTextLinesEventConnectionPoint();
      if (eventConnectionPoint == null)
        return;
      eventConnectionPoint.Advise((object) this, out this.textLinesEventCookie);
      this.package.Assert(this.textLinesEventCookie != 0U, "Failed to advise text lines events");
    }

    private void UnadviseTextLinesEvents()
    {
      if (this.textLinesEventCookie == 0U)
        return;
      IConnectionPoint eventConnectionPoint = this.GetTextLinesEventConnectionPoint();
      if (eventConnectionPoint == null)
        return;
      eventConnectionPoint.Unadvise(this.textLinesEventCookie);
      this.textLinesEventCookie = 0U;
    }

    internal string GetBufferContent()
    {
      IVsTextStream vsTextBuffer = (IVsTextStream) this.VsTextBuffer;
      int piLength;
      this.package.AssertOk(vsTextBuffer.GetSize(out piLength));
      char[] chArray = new char[piLength + 1];
      this.package.AssertOk(vsTextBuffer.GetStream(0, piLength, Marshal.UnsafeAddrOfPinnedArrayElement((Array) chArray, 0)));
      return new string(chArray, 0, piLength);
    }

    internal void SetBufferContent(string content)
    {
      IVsTextStream vsTextBuffer = (IVsTextStream) this.VsTextBuffer;
      int piLength;
      this.package.AssertOk(vsTextBuffer.GetSize(out piLength));
      char[] charArray = content.ToCharArray();
      this.package.AssertOk(vsTextBuffer.ReloadStream(0, piLength, Marshal.UnsafeAddrOfPinnedArrayElement((Array) charArray, 0), charArray.Length));
    }

    void IVsTextLinesEvents.OnChangeLineAttributes(int iFirstLine, int iLastLine)
    {
    }

    void IVsTextLinesEvents.OnChangeLineText(
      TextLineChange[] pTextLineChange,
      int fLast)
    {
      this.package.GetDesignTimeForCordDocument(this).RefreshScriptSyntax(this.FileName);
    }

    public void Dispose()
    {
      if (this.textLinesEventCookie == 0U)
        return;
      this.UnadviseTextLinesEvents();
      GC.SuppressFinalize((object) this);
    }
  }
}
