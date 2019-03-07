// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.SummaryDocumentFactory
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.SpecExplorer.VS
{
  [Guid("3C73969B-5878-4A18-B68B-0F1FD24094D3")]
  internal class SummaryDocumentFactory : IVsEditorFactory
  {
    private SpecExplorerPackage package;
    private Microsoft.VisualStudio.OLE.Interop.IServiceProvider oleServiceProvider;

    public SummaryDocumentFactory(SpecExplorerPackage package)
    {
      this.package = package;
    }

    public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
    {
      this.oleServiceProvider = psp;
      return 0;
    }

    public int MapLogicalView(ref Guid rguidLogicalView, out string pbstrPhysicalView)
    {
      pbstrPhysicalView = (string) null;
      return rguidLogicalView == VSConstants.LOGVIEWID_Primary ? 0 : -2147467263;
    }

    public int Close()
    {
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
      pbstrEditorCaption = "";
      pguidCmdUI = Guid.Empty;
      pgrfCDW = 0;
      pszMkDocument = pszMkDocument.ToLower();
      if (punkDocDataExisting != IntPtr.Zero)
        return -2147213334;
      SummaryDocument summaryDocument = new SummaryDocument(this.package, this.oleServiceProvider, pszMkDocument);
      ppunkDocData = Marshal.GetIUnknownForObject((object) summaryDocument);
      ppunkDocView = Marshal.GetIUnknownForObject((object) summaryDocument);
      pguidCmdUI = VSConstants.GUID_TextEditorFactory;
      pbstrEditorCaption = "";
      return 0;
    }
  }
}
