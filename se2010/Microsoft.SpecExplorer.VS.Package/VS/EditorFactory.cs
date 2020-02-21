// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.EditorFactory
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.SpecExplorer.VS
{
  [Guid("04C7681D-A337-4705-8AD9-2206D31A9F7B")]
  public class EditorFactory : IVsEditorFactory, IVsTextManagerEvents
  {
    private SpecExplorerPackage package;
    private Microsoft.VisualStudio.OLE.Interop.IServiceProvider oleServiceProvider;
    private Dictionary<string, CordDocument> documents;
    private uint textManagerEventsCookie;

    public EditorFactory(SpecExplorerPackage host)
    {
      this.package = host;
      this.documents = new Dictionary<string, CordDocument>();
      this.GetTextManagerConnectionPoint().Advise((object) this, out this.textManagerEventsCookie);
    }

    private IConnectionPoint GetTextManagerConnectionPoint()
    {
      IConnectionPointContainer requiredService = this.package.GetRequiredService<IVsTextManager>(typeof (SVsTextManager)) as IConnectionPointContainer;
      Guid guid = typeof (IVsTextManagerEvents).GUID;
      IConnectionPoint ppCP;
      requiredService.FindConnectionPoint(ref guid, out ppCP);
      return ppCP;
    }

    internal CordDocument GetOpenedCordDocumentByName(string fileName)
    {
      foreach (CordDocument cordDocument in this.documents.Values)
      {
        if (string.Compare(cordDocument.FileName, fileName, true) == 0)
          return cordDocument;
      }
      return (CordDocument) null;
    }

    internal void OpenDocument(string path)
    {
      Guid logviewidTextView = VSConstants.LOGVIEWID_TextView;
      IVsUIHierarchy hierarchy;
      uint itemID;
      IVsWindowFrame windowFrame;
      IVsTextView view;
      VsShellUtilities.OpenDocument((System.IServiceProvider) this.package, path, logviewidTextView, out hierarchy, out itemID, out windowFrame, out view);
    }

    internal void Remove(string filename)
    {
      this.documents.Remove(filename);
    }

    internal void Rename(string oldName, CordDocument doc)
    {
      this.documents.Remove(oldName);
      this.documents[doc.FileName] = doc;
    }

    internal void Clear()
    {
      this.documents.Clear();
    }

    public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
    {
      this.oleServiceProvider = psp;
      return 0;
    }

    public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
    {
      pbstrPhysicalView = (string) null;
      return rguidLogicalView == VSConstants.LOGVIEWID_Primary || rguidLogicalView == VSConstants.LOGVIEWID_TextView ? 0 : -2147467263;
    }

    public int Close()
    {
      IConnectionPoint managerConnectionPoint = this.GetTextManagerConnectionPoint();
      if (managerConnectionPoint != null && this.textManagerEventsCookie != 0U)
        managerConnectionPoint.Unadvise(this.textManagerEventsCookie);
      return 0;
    }

    public int CreateEditorInstance(
      uint grfCreateDoc,
      string pszMkDocument,
      string pszPhysicalView,
      IVsHierarchy pvHier,
      uint itemid,
      IntPtr punkDocDataExisting,
      out IntPtr ppunkDocView,
      out IntPtr ppunkDocData,
      out string pbstrEditorCaption,
      out Guid pguidCmdUI,
      out int pgrfCDW)
    {
      ppunkDocView = IntPtr.Zero;
      ppunkDocData = IntPtr.Zero;
      pbstrEditorCaption = string.Empty;
      pguidCmdUI = Guid.Empty;
      pgrfCDW = 0;
      this.package.Assert(((int) grfCreateDoc & 6) != 0, "invalid editor creation mode");
      this.package.Assert(string.IsNullOrEmpty(pszPhysicalView));
      if (punkDocDataExisting != IntPtr.Zero)
        return -2147213334;
      CordDocument doc = new CordDocument(this.package, this.oleServiceProvider, pszMkDocument, pvHier, itemid);
      this.documents[pszMkDocument] = doc;
      this.package.UnregisterCordDocumentFromDesignTimeManager(doc);
      this.package.RegisterCordDocumentToDesignTimeManager(doc, true);
      ppunkDocView = Marshal.GetIUnknownForObject((object) doc.VsCodeWindow);
      ppunkDocData = Marshal.GetIUnknownForObject((object) doc.VsTextBuffer);
      pguidCmdUI = doc.GetCommandUIGuid();
      return 0;
    }

    public void OnRegisterMarkerType(int iMarkerType)
    {
    }

    public void OnRegisterView(IVsTextView pView)
    {
    }

    public void OnUnregisterView(IVsTextView pView)
    {
      IVsTextLines ppBuffer;
      this.package.AssertOk(pView.GetBuffer(out ppBuffer));
      IPersistFileFormat persistFileFormat = ppBuffer as IPersistFileFormat;
      string ppszFilename;
      uint pnFormatIndex;
      if (persistFileFormat == null || persistFileFormat.GetCurFile(out ppszFilename, out pnFormatIndex) != 0)
        return;
      CordDocument doc = (CordDocument) null;
      if (string.IsNullOrEmpty(ppszFilename))
        return;
      if (!this.documents.TryGetValue(ppszFilename, out doc))
        return;
      try
      {
        this.Remove(doc.FileName);
        this.package.UnregisterCordDocumentFromDesignTimeManager(doc);
        this.package.RegisterCordDocumentToDesignTimeManager(doc, false);
      }
      finally
      {
        doc.Dispose();
      }
    }

    public void OnUserPreferencesChanged(
      VIEWPREFERENCES[] pViewPrefs,
      FRAMEPREFERENCES[] pFramePrefs,
      LANGPREFERENCES[] pLangPrefs,
      FONTCOLORPREFERENCES[] pColorPrefs)
    {
    }
  }
}
