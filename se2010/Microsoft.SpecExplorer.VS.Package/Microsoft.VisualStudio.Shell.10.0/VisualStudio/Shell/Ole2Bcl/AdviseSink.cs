// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Ole2Bcl.AdviseSink
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell.Ole2Bcl
{
  internal sealed class AdviseSink : Microsoft.VisualStudio.OLE.Interop.IAdviseSink, System.Runtime.InteropServices.ComTypes.IAdviseSink
  {
    private Microsoft.VisualStudio.OLE.Interop.IAdviseSink oleSink;
    private System.Runtime.InteropServices.ComTypes.IAdviseSink bclSink;

    private AdviseSink()
    {
    }

    internal AdviseSink(Microsoft.VisualStudio.OLE.Interop.IAdviseSink oleSink)
    {
      if (oleSink == null)
        throw new ArgumentNullException("Microsoft.VisualStudio.OLE.Interop.IAdviseSink");
      this.oleSink = oleSink;
      this.bclSink = oleSink as System.Runtime.InteropServices.ComTypes.IAdviseSink;
    }

    internal AdviseSink(System.Runtime.InteropServices.ComTypes.IAdviseSink bclSink)
    {
      if (bclSink == null)
        throw new ArgumentNullException("System.Runtime.InteropServices.ComTypes.IAdviseSink");
      this.oleSink = bclSink as Microsoft.VisualStudio.OLE.Interop.IAdviseSink;
      this.bclSink = bclSink;
    }

    void Microsoft.VisualStudio.OLE.Interop.IAdviseSink.OnClose()
    {
      if (this.oleSink != null)
        this.oleSink.OnClose();
      else
        this.bclSink.OnClose();
    }

    void System.Runtime.InteropServices.ComTypes.IAdviseSink.OnClose()
    {
      ((Microsoft.VisualStudio.OLE.Interop.IAdviseSink) this).OnClose();
    }

    void Microsoft.VisualStudio.OLE.Interop.IAdviseSink.OnDataChange(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pFormatetc,
      Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[] pStgmed)
    {
      if (this.oleSink != null)
      {
        this.oleSink.OnDataChange(pFormatetc, pStgmed);
      }
      else
      {
        if (pFormatetc == null || pStgmed == null)
          throw new ArgumentNullException("");
        if (1 != pFormatetc.Length || 1 != pStgmed.Length)
          throw new InvalidOperationException();
        System.Runtime.InteropServices.ComTypes.FORMATETC formatetc = StructConverter.OleFormatETC2Bcl(ref pFormatetc[0]);
        System.Runtime.InteropServices.ComTypes.STGMEDIUM stgmedium = StructConverter.OleSTGMEDIUM2Bcl(ref pStgmed[0]);
        this.bclSink.OnDataChange(ref formatetc, ref stgmedium);
        pFormatetc[0] = StructConverter.BclFormatETC2Ole(ref formatetc);
        pStgmed[0] = StructConverter.BclSTGMEDIUM2Ole(ref stgmedium);
      }
    }

    void System.Runtime.InteropServices.ComTypes.IAdviseSink.OnDataChange(
      ref System.Runtime.InteropServices.ComTypes.FORMATETC format,
      ref System.Runtime.InteropServices.ComTypes.STGMEDIUM stgmedium)
    {
      if (this.bclSink != null)
        this.bclSink.OnDataChange(ref format, ref stgmedium);
      else
        this.oleSink.OnDataChange(new Microsoft.VisualStudio.OLE.Interop.FORMATETC[1]
        {
          StructConverter.BclFormatETC2Ole(ref format)
        }, new Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[1]
        {
          StructConverter.BclSTGMEDIUM2Ole(ref stgmedium)
        });
    }

    void Microsoft.VisualStudio.OLE.Interop.IAdviseSink.OnRename(Microsoft.VisualStudio.OLE.Interop.IMoniker pmk)
    {
      if (this.oleSink != null)
        this.oleSink.OnRename(pmk);
      else
        this.bclSink.OnRename((System.Runtime.InteropServices.ComTypes.IMoniker) null);
    }

    void System.Runtime.InteropServices.ComTypes.IAdviseSink.OnRename(System.Runtime.InteropServices.ComTypes.IMoniker moniker)
    {
      if (this.bclSink != null)
        this.bclSink.OnRename(moniker);
      else
        this.oleSink.OnRename((Microsoft.VisualStudio.OLE.Interop.IMoniker) null);
    }

    void Microsoft.VisualStudio.OLE.Interop.IAdviseSink.OnSave()
    {
      if (this.oleSink != null)
        this.oleSink.OnSave();
      else
        this.bclSink.OnSave();
    }

    void System.Runtime.InteropServices.ComTypes.IAdviseSink.OnSave()
    {
      ((Microsoft.VisualStudio.OLE.Interop.IAdviseSink) this).OnSave();
    }

    void Microsoft.VisualStudio.OLE.Interop.IAdviseSink.OnViewChange(
      uint aspect,
      int index)
    {
      if (this.oleSink != null)
        this.oleSink.OnViewChange(aspect, index);
      else
        this.bclSink.OnViewChange((int) aspect, index);
    }

    void System.Runtime.InteropServices.ComTypes.IAdviseSink.OnViewChange(
      int aspect,
      int index)
    {
      ((Microsoft.VisualStudio.OLE.Interop.IAdviseSink) this).OnViewChange((uint) aspect, index);
    }
  }
}
