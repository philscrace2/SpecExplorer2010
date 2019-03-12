// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Ole2Bcl.StructConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

namespace Microsoft.VisualStudio.Shell.Ole2Bcl
{
  internal sealed class StructConverter
  {
    private StructConverter()
    {
    }

    internal static Microsoft.VisualStudio.OLE.Interop.FORMATETC BclFormatETC2Ole(
      ref System.Runtime.InteropServices.ComTypes.FORMATETC bclFormat)
    {
      Microsoft.VisualStudio.OLE.Interop.FORMATETC formatetc;
      formatetc.cfFormat = (ushort) bclFormat.cfFormat;
      formatetc.dwAspect = (uint) bclFormat.dwAspect;
      formatetc.lindex = bclFormat.lindex;
      formatetc.ptd = bclFormat.ptd;
      formatetc.tymed = (uint) bclFormat.tymed;
      return formatetc;
    }

    internal static System.Runtime.InteropServices.ComTypes.FORMATETC OleFormatETC2Bcl(
      ref Microsoft.VisualStudio.OLE.Interop.FORMATETC oleFormat)
    {
      System.Runtime.InteropServices.ComTypes.FORMATETC formatetc;
      formatetc.cfFormat = (short) oleFormat.cfFormat;
      formatetc.dwAspect = (System.Runtime.InteropServices.ComTypes.DVASPECT) oleFormat.dwAspect;
      formatetc.lindex = oleFormat.lindex;
      formatetc.ptd = oleFormat.ptd;
      formatetc.tymed = (System.Runtime.InteropServices.ComTypes.TYMED) oleFormat.tymed;
      return formatetc;
    }

    internal static Microsoft.VisualStudio.OLE.Interop.STGMEDIUM BclSTGMEDIUM2Ole(
      ref System.Runtime.InteropServices.ComTypes.STGMEDIUM bclMedium)
    {
      Microsoft.VisualStudio.OLE.Interop.STGMEDIUM stgmedium;
      stgmedium.pUnkForRelease = bclMedium.pUnkForRelease;
      stgmedium.tymed = (uint) bclMedium.tymed;
      stgmedium.unionmember = bclMedium.unionmember;
      return stgmedium;
    }

    internal static System.Runtime.InteropServices.ComTypes.STGMEDIUM OleSTGMEDIUM2Bcl(
      ref Microsoft.VisualStudio.OLE.Interop.STGMEDIUM oleMedium)
    {
      System.Runtime.InteropServices.ComTypes.STGMEDIUM stgmedium;
      stgmedium.pUnkForRelease = oleMedium.pUnkForRelease;
      stgmedium.tymed = (System.Runtime.InteropServices.ComTypes.TYMED) oleMedium.tymed;
      stgmedium.unionmember = oleMedium.unionmember;
      return stgmedium;
    }

    internal static Microsoft.VisualStudio.OLE.Interop.STATDATA BclSTATDATA2Ole(
      ref System.Runtime.InteropServices.ComTypes.STATDATA bclData)
    {
      Microsoft.VisualStudio.OLE.Interop.STATDATA statdata;
      if (bclData.advSink == null)
      {
        statdata.pAdvSink = (Microsoft.VisualStudio.OLE.Interop.IAdviseSink) null;
      }
      else
      {
        statdata.pAdvSink = bclData.advSink as Microsoft.VisualStudio.OLE.Interop.IAdviseSink;
        if (statdata.pAdvSink == null)
          statdata.pAdvSink = (Microsoft.VisualStudio.OLE.Interop.IAdviseSink) new AdviseSink(bclData.advSink);
      }
      statdata.ADVF = (uint) bclData.advf;
      statdata.dwConnection = (uint) bclData.connection;
      statdata.FORMATETC = StructConverter.BclFormatETC2Ole(ref bclData.formatetc);
      return statdata;
    }

    internal static System.Runtime.InteropServices.ComTypes.STATDATA OleSTATDATA2Bcl(
      ref Microsoft.VisualStudio.OLE.Interop.STATDATA oleData)
    {
      System.Runtime.InteropServices.ComTypes.STATDATA statdata;
      if (oleData.pAdvSink == null)
      {
        statdata.advSink = (System.Runtime.InteropServices.ComTypes.IAdviseSink) null;
      }
      else
      {
        statdata.advSink = oleData.pAdvSink as System.Runtime.InteropServices.ComTypes.IAdviseSink;
        if (statdata.advSink == null)
          statdata.advSink = (System.Runtime.InteropServices.ComTypes.IAdviseSink) new AdviseSink(oleData.pAdvSink);
      }
      statdata.advf = (System.Runtime.InteropServices.ComTypes.ADVF) oleData.ADVF;
      statdata.connection = (int) oleData.dwConnection;
      statdata.formatetc = StructConverter.OleFormatETC2Bcl(ref oleData.FORMATETC);
      return statdata;
    }
  }
}
