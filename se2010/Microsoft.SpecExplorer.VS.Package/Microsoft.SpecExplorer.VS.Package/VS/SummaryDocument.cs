// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.SummaryDocument
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows.Forms;

namespace Microsoft.SpecExplorer.VS
{
  internal class SummaryDocument : WindowPane, IVsPersistDocData
  {
    private SpecExplorerPackage package;
    private SummaryDocumentControl summaryDocumentControl;
    private string filePath;

    public SummaryDocument(
      SpecExplorerPackage package,
      Microsoft.VisualStudio.OLE.Interop.IServiceProvider oleServiceProvider,
      string fileName)
      : base((System.IServiceProvider) new ServiceProvider(oleServiceProvider))
    {
      this.package = package;
      this.filePath = fileName;
      this.summaryDocumentControl = new SummaryDocumentControl(this.package, fileName);
    }

    public override IWin32Window Window
    {
      get
      {
        return (IWin32Window) this.summaryDocumentControl;
      }
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (this.summaryDocumentControl == null)
          return;
        this.summaryDocumentControl = (SummaryDocumentControl) null;
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    public int Close()
    {
      return 0;
    }

    public int GetGuidEditorType(out Guid pClassID)
    {
      pClassID = GuidList.guidSummaryDocumentFactory;
      return 0;
    }

    public int IsDocDataDirty(out int pfDirty)
    {
      pfDirty = 0;
      return 0;
    }

    public int IsDocDataReloadable(out int pfReloadable)
    {
      pfReloadable = 1;
      return 0;
    }

    public int LoadDocData(string pszMkDocument)
    {
      return 0;
    }

    public int OnRegisterDocData(uint docCookie, IVsHierarchy pHierNew, uint itemidNew)
    {
      return 0;
    }

    public int ReloadDocData(uint grfFlags)
    {
      return 0;
    }

    public int RenameDocData(
      uint grfAttribs,
      IVsHierarchy pHierNew,
      uint itemidNew,
      string pszMkDocumentNew)
    {
      return 0;
    }

    public int SaveDocData(
      VSSAVEFLAGS dwSave,
      out string pbstrMkDocumentNew,
      out int pfSaveCanceled)
    {
      pbstrMkDocumentNew = (string) null;
      pfSaveCanceled = 0;
      return 0;
    }

    public int SetUntitledDocPath(string pszDocDataPath)
    {
      return 0;
    }
  }
}
