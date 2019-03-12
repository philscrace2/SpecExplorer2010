// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.OleDataObject
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Ole2Bcl;
using System;
using System.Windows.Forms;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public class OleDataObject : DataObject, Microsoft.VisualStudio.OLE.Interop.IDataObject
  {
    private Microsoft.VisualStudio.OLE.Interop.IDataObject oleData;

    public OleDataObject()
    {
      this.oleData = (Microsoft.VisualStudio.OLE.Interop.IDataObject) new Ole2BclDataObject((System.Runtime.InteropServices.ComTypes.IDataObject) this);
    }

    public OleDataObject(System.Windows.Forms.IDataObject winData)
      : base((object) winData)
    {
      this.oleData = winData as Microsoft.VisualStudio.OLE.Interop.IDataObject;
      if (this.oleData != null)
        return;
      this.oleData = (Microsoft.VisualStudio.OLE.Interop.IDataObject) new Ole2BclDataObject((System.Runtime.InteropServices.ComTypes.IDataObject) this);
    }

    public OleDataObject(System.Runtime.InteropServices.ComTypes.IDataObject comData)
      : base((object) comData)
    {
      this.oleData = comData as Microsoft.VisualStudio.OLE.Interop.IDataObject;
      if (this.oleData != null)
        return;
      this.oleData = (Microsoft.VisualStudio.OLE.Interop.IDataObject) new Ole2BclDataObject(comData);
    }

    public OleDataObject(Microsoft.VisualStudio.OLE.Interop.IDataObject oleData)
      : base(oleData is System.Runtime.InteropServices.ComTypes.IDataObject ? (object) (System.Runtime.InteropServices.ComTypes.IDataObject) oleData : (object) new Ole2BclDataObject(oleData))
    {
      this.oleData = oleData;
    }

    int Microsoft.VisualStudio.OLE.Interop.IDataObject.DAdvise(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pFormatetc,
      uint ADVF,
      Microsoft.VisualStudio.OLE.Interop.IAdviseSink pAdvSink,
      out uint pdwConnection)
    {
      return this.oleData.DAdvise(pFormatetc, ADVF, pAdvSink, out pdwConnection);
    }

    void Microsoft.VisualStudio.OLE.Interop.IDataObject.DUnadvise(uint dwConnection)
    {
      this.oleData.DUnadvise(dwConnection);
    }

    int Microsoft.VisualStudio.OLE.Interop.IDataObject.EnumDAdvise(
      out Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA ppenumAdvise)
    {
      return this.oleData.EnumDAdvise(out ppenumAdvise);
    }

    int Microsoft.VisualStudio.OLE.Interop.IDataObject.EnumFormatEtc(
      uint dwDirection,
      out Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC ppenumFormatEtc)
    {
      return this.oleData.EnumFormatEtc(dwDirection, out ppenumFormatEtc);
    }

    int Microsoft.VisualStudio.OLE.Interop.IDataObject.GetCanonicalFormatEtc(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pformatectIn,
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pformatetcOut)
    {
      return this.oleData.GetCanonicalFormatEtc(pformatectIn, pformatetcOut);
    }

    void Microsoft.VisualStudio.OLE.Interop.IDataObject.GetData(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pformatetcIn,
      Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[] pRemoteMedium)
    {
      this.oleData.GetData(pformatetcIn, pRemoteMedium);
    }

    void Microsoft.VisualStudio.OLE.Interop.IDataObject.GetDataHere(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pFormatetc,
      Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[] pRemoteMedium)
    {
      this.oleData.GetDataHere(pFormatetc, pRemoteMedium);
    }

    int Microsoft.VisualStudio.OLE.Interop.IDataObject.QueryGetData(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pFormatetc)
    {
      return this.oleData.QueryGetData(pFormatetc);
    }

    void Microsoft.VisualStudio.OLE.Interop.IDataObject.SetData(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pFormatetc,
      Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[] pmedium,
      int fRelease)
    {
      this.oleData.SetData(pFormatetc, pmedium, fRelease);
    }
  }
}
