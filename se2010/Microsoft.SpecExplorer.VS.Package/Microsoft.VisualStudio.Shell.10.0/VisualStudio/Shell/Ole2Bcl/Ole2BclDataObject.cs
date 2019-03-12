// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Ole2Bcl.Ole2BclDataObject
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell.Ole2Bcl
{
  internal sealed class Ole2BclDataObject : Microsoft.VisualStudio.OLE.Interop.IDataObject, System.Runtime.InteropServices.ComTypes.IDataObject
  {
    private Microsoft.VisualStudio.OLE.Interop.IDataObject oleData;
    private System.Runtime.InteropServices.ComTypes.IDataObject bclData;

    private Ole2BclDataObject()
    {
    }

    internal Ole2BclDataObject(Microsoft.VisualStudio.OLE.Interop.IDataObject oleData)
    {
      if (oleData == null)
        throw new ArgumentNullException("Microsoft.VisualStudio.OLE.Interop.IDataObject");
      this.oleData = oleData;
      this.bclData = (System.Runtime.InteropServices.ComTypes.IDataObject) null;
    }

    internal Ole2BclDataObject(System.Runtime.InteropServices.ComTypes.IDataObject bclData)
    {
      if (bclData == null)
        throw new ArgumentNullException("System.Runtime.InteropServices.ComTypes.IDataObject");
      this.oleData = (Microsoft.VisualStudio.OLE.Interop.IDataObject) null;
      this.bclData = bclData;
    }

    int Microsoft.VisualStudio.OLE.Interop.IDataObject.DAdvise(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pFormatetc,
      uint ADVF,
      Microsoft.VisualStudio.OLE.Interop.IAdviseSink pAdvSink,
      out uint pdwConnection)
    {
      if (this.oleData != null)
        return this.oleData.DAdvise(pFormatetc, ADVF, pAdvSink, out pdwConnection);
      if (pFormatetc == null || pFormatetc.Length != 1)
        throw new ArgumentException();
      System.Runtime.InteropServices.ComTypes.FORMATETC pFormatetc1 = StructConverter.OleFormatETC2Bcl(ref pFormatetc[0]);
      System.Runtime.InteropServices.ComTypes.IAdviseSink adviseSink = pAdvSink as System.Runtime.InteropServices.ComTypes.IAdviseSink ?? (System.Runtime.InteropServices.ComTypes.IAdviseSink) new AdviseSink(pAdvSink);
      int connection;
      int num = this.bclData.DAdvise(ref pFormatetc1, (System.Runtime.InteropServices.ComTypes.ADVF) ADVF, adviseSink, out connection);
      pdwConnection = (uint) connection;
      return num;
    }

    void Microsoft.VisualStudio.OLE.Interop.IDataObject.DUnadvise(uint dwConnection)
    {
      if (this.oleData != null)
        this.oleData.DUnadvise(dwConnection);
      else
        this.bclData.DUnadvise((int) dwConnection);
    }

    int Microsoft.VisualStudio.OLE.Interop.IDataObject.EnumDAdvise(
      out Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA ppenumAdvise)
    {
      if (this.oleData != null)
        return this.oleData.EnumDAdvise(out ppenumAdvise);
      System.Runtime.InteropServices.ComTypes.IEnumSTATDATA enumAdvise;
      int hr = this.bclData.EnumDAdvise(out enumAdvise);
      NativeMethods.ThrowOnFailure(hr);
      if (enumAdvise == null)
      {
        ppenumAdvise = (Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA) null;
      }
      else
      {
        ppenumAdvise = enumAdvise as Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA;
        if (ppenumAdvise == null)
          ppenumAdvise = (Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA) new EnumSTATDATA(enumAdvise);
      }
      return hr;
    }

    int Microsoft.VisualStudio.OLE.Interop.IDataObject.EnumFormatEtc(
      uint dwDirection,
      out Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC ppenumFormatEtc)
    {
      if (this.oleData != null)
        return this.oleData.EnumFormatEtc(dwDirection, out ppenumFormatEtc);
      System.Runtime.InteropServices.ComTypes.IEnumFORMATETC bclEnum = this.bclData.EnumFormatEtc((System.Runtime.InteropServices.ComTypes.DATADIR) dwDirection);
      if (bclEnum == null)
      {
        ppenumFormatEtc = (Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC) null;
      }
      else
      {
        ppenumFormatEtc = bclEnum as Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC;
        if (ppenumFormatEtc == null)
          ppenumFormatEtc = (Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC) new EnumFORMATETC(bclEnum);
      }
      return 0;
    }

    int Microsoft.VisualStudio.OLE.Interop.IDataObject.GetCanonicalFormatEtc(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pformatectIn,
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pformatetcOut)
    {
      if (this.oleData != null)
        return this.oleData.GetCanonicalFormatEtc(pformatectIn, pformatetcOut);
      if (pformatectIn == null || pformatectIn.Length != 1 || (pformatetcOut == null || pformatetcOut.Length != 1))
        throw new ArgumentException();
      System.Runtime.InteropServices.ComTypes.FORMATETC formatIn = StructConverter.OleFormatETC2Bcl(ref pformatectIn[0]);
      System.Runtime.InteropServices.ComTypes.FORMATETC formatOut;
      int canonicalFormatEtc = this.bclData.GetCanonicalFormatEtc(ref formatIn, out formatOut);
      NativeMethods.ThrowOnFailure(canonicalFormatEtc);
      pformatetcOut[0] = StructConverter.BclFormatETC2Ole(ref formatOut);
      return canonicalFormatEtc;
    }

    void Microsoft.VisualStudio.OLE.Interop.IDataObject.GetData(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pformatetcIn,
      Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[] pRemoteMedium)
    {
      if (this.oleData != null)
      {
        this.oleData.GetData(pformatetcIn, pRemoteMedium);
      }
      else
      {
        if (pformatetcIn == null || pformatetcIn.Length != 1 || (pRemoteMedium == null || pRemoteMedium.Length != 1))
          throw new ArgumentException();
        System.Runtime.InteropServices.ComTypes.FORMATETC format = StructConverter.OleFormatETC2Bcl(ref pformatetcIn[0]);
        System.Runtime.InteropServices.ComTypes.STGMEDIUM medium;
        this.bclData.GetData(ref format, out medium);
        pRemoteMedium[0] = StructConverter.BclSTGMEDIUM2Ole(ref medium);
      }
    }

    void Microsoft.VisualStudio.OLE.Interop.IDataObject.GetDataHere(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pFormatetc,
      Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[] pRemoteMedium)
    {
      if (this.oleData != null)
      {
        this.oleData.GetDataHere(pFormatetc, pRemoteMedium);
      }
      else
      {
        if (pFormatetc == null || pFormatetc.Length != 1 || (pRemoteMedium == null || pRemoteMedium.Length != 1))
          throw new ArgumentException();
        System.Runtime.InteropServices.ComTypes.FORMATETC format = StructConverter.OleFormatETC2Bcl(ref pFormatetc[0]);
        System.Runtime.InteropServices.ComTypes.STGMEDIUM stgmedium = StructConverter.OleSTGMEDIUM2Bcl(ref pRemoteMedium[0]);
        this.bclData.GetDataHere(ref format, ref stgmedium);
        pRemoteMedium[0] = StructConverter.BclSTGMEDIUM2Ole(ref stgmedium);
      }
    }

    int Microsoft.VisualStudio.OLE.Interop.IDataObject.QueryGetData(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pFormatetc)
    {
      if (this.oleData != null)
        return this.oleData.QueryGetData(pFormatetc);
      if (pFormatetc == null || 1 != pFormatetc.Length)
        throw new ArgumentException();
      System.Runtime.InteropServices.ComTypes.FORMATETC format = StructConverter.OleFormatETC2Bcl(ref pFormatetc[0]);
      return this.bclData.QueryGetData(ref format);
    }

    void Microsoft.VisualStudio.OLE.Interop.IDataObject.SetData(
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pFormatetc,
      Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[] pmedium,
      int fRelease)
    {
      if (this.oleData != null)
      {
        this.oleData.SetData(pFormatetc, pmedium, fRelease);
      }
      else
      {
        if (pFormatetc == null || 1 != pFormatetc.Length || (pmedium == null || 1 != pmedium.Length))
          throw new ArgumentException();
        System.Runtime.InteropServices.ComTypes.FORMATETC formatIn = StructConverter.OleFormatETC2Bcl(ref pFormatetc[0]);
        System.Runtime.InteropServices.ComTypes.STGMEDIUM medium = StructConverter.OleSTGMEDIUM2Bcl(ref pmedium[0]);
        this.bclData.SetData(ref formatIn, ref medium, fRelease != 0);
      }
    }

    int System.Runtime.InteropServices.ComTypes.IDataObject.DAdvise(
      ref System.Runtime.InteropServices.ComTypes.FORMATETC pFormatetc,
      System.Runtime.InteropServices.ComTypes.ADVF advf,
      System.Runtime.InteropServices.ComTypes.IAdviseSink adviseSink,
      out int connection)
    {
      if (this.bclData != null)
        return this.bclData.DAdvise(ref pFormatetc, advf, adviseSink, out connection);
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pFormatetc1 = new Microsoft.VisualStudio.OLE.Interop.FORMATETC[1]
      {
        StructConverter.BclFormatETC2Ole(ref pFormatetc)
      };
      Microsoft.VisualStudio.OLE.Interop.IAdviseSink pAdvSink = adviseSink as Microsoft.VisualStudio.OLE.Interop.IAdviseSink ?? (Microsoft.VisualStudio.OLE.Interop.IAdviseSink) new AdviseSink(adviseSink);
      uint pdwConnection;
      int hr = this.oleData.DAdvise(pFormatetc1, (uint) advf, pAdvSink, out pdwConnection);
      NativeMethods.ThrowOnFailure(hr);
      connection = (int) pdwConnection;
      return hr;
    }

    void System.Runtime.InteropServices.ComTypes.IDataObject.DUnadvise(int connection)
    {
      if (this.bclData != null)
        this.bclData.DUnadvise(connection);
      else
        this.oleData.DUnadvise((uint) connection);
    }

    int System.Runtime.InteropServices.ComTypes.IDataObject.EnumDAdvise(
      out System.Runtime.InteropServices.ComTypes.IEnumSTATDATA enumAdvise)
    {
      if (this.bclData != null)
        return this.bclData.EnumDAdvise(out enumAdvise);
      Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA ppenumAdvise;
      int hr = this.oleData.EnumDAdvise(out ppenumAdvise);
      NativeMethods.ThrowOnFailure(hr);
      if (ppenumAdvise == null)
      {
        enumAdvise = (System.Runtime.InteropServices.ComTypes.IEnumSTATDATA) null;
      }
      else
      {
        enumAdvise = ppenumAdvise as System.Runtime.InteropServices.ComTypes.IEnumSTATDATA;
        if (enumAdvise == null)
          enumAdvise = (System.Runtime.InteropServices.ComTypes.IEnumSTATDATA) new EnumSTATDATA(ppenumAdvise);
      }
      return hr;
    }

    System.Runtime.InteropServices.ComTypes.IEnumFORMATETC System.Runtime.InteropServices.ComTypes.IDataObject.EnumFormatEtc(
      System.Runtime.InteropServices.ComTypes.DATADIR direction)
    {
      if (this.bclData != null)
        return this.bclData.EnumFormatEtc(direction);
      Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC ppenumFormatEtc;
      NativeMethods.ThrowOnFailure(this.oleData.EnumFormatEtc((uint) direction, out ppenumFormatEtc));
      if (ppenumFormatEtc == null)
        return (System.Runtime.InteropServices.ComTypes.IEnumFORMATETC) null;
      return ppenumFormatEtc as System.Runtime.InteropServices.ComTypes.IEnumFORMATETC ?? (System.Runtime.InteropServices.ComTypes.IEnumFORMATETC) new EnumFORMATETC(ppenumFormatEtc);
    }

    int System.Runtime.InteropServices.ComTypes.IDataObject.GetCanonicalFormatEtc(
      ref System.Runtime.InteropServices.ComTypes.FORMATETC formatIn,
      out System.Runtime.InteropServices.ComTypes.FORMATETC formatOut)
    {
      if (this.bclData != null)
        return this.bclData.GetCanonicalFormatEtc(ref formatIn, out formatOut);
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pformatectIn = new Microsoft.VisualStudio.OLE.Interop.FORMATETC[1];
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pformatetcOut = new Microsoft.VisualStudio.OLE.Interop.FORMATETC[1];
      pformatectIn[0] = StructConverter.BclFormatETC2Ole(ref formatIn);
      int canonicalFormatEtc = this.oleData.GetCanonicalFormatEtc(pformatectIn, pformatetcOut);
      NativeMethods.ThrowOnFailure(canonicalFormatEtc);
      formatOut = StructConverter.OleFormatETC2Bcl(ref pformatetcOut[0]);
      return canonicalFormatEtc;
    }

    void System.Runtime.InteropServices.ComTypes.IDataObject.GetData(
      ref System.Runtime.InteropServices.ComTypes.FORMATETC format,
      out System.Runtime.InteropServices.ComTypes.STGMEDIUM medium)
    {
      if (this.bclData != null)
      {
        this.bclData.GetData(ref format, out medium);
      }
      else
      {
        Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pformatetcIn = new Microsoft.VisualStudio.OLE.Interop.FORMATETC[1]
        {
          StructConverter.BclFormatETC2Ole(ref format)
        };
        Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[] pRemoteMedium = new Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[1];
        this.oleData.GetData(pformatetcIn, pRemoteMedium);
        medium = StructConverter.OleSTGMEDIUM2Bcl(ref pRemoteMedium[0]);
      }
    }

    void System.Runtime.InteropServices.ComTypes.IDataObject.GetDataHere(
      ref System.Runtime.InteropServices.ComTypes.FORMATETC format,
      ref System.Runtime.InteropServices.ComTypes.STGMEDIUM medium)
    {
      if (this.bclData != null)
      {
        this.bclData.GetDataHere(ref format, ref medium);
      }
      else
      {
        Microsoft.VisualStudio.OLE.Interop.FORMATETC[] pFormatetc = new Microsoft.VisualStudio.OLE.Interop.FORMATETC[1]
        {
          StructConverter.BclFormatETC2Ole(ref format)
        };
        Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[] pRemoteMedium = new Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[1]
        {
          StructConverter.BclSTGMEDIUM2Ole(ref medium)
        };
        this.oleData.GetDataHere(pFormatetc, pRemoteMedium);
        medium = StructConverter.OleSTGMEDIUM2Bcl(ref pRemoteMedium[0]);
      }
    }

    int System.Runtime.InteropServices.ComTypes.IDataObject.QueryGetData(
      ref System.Runtime.InteropServices.ComTypes.FORMATETC format)
    {
      if (this.bclData != null)
        return this.bclData.QueryGetData(ref format);
      return this.oleData.QueryGetData(new Microsoft.VisualStudio.OLE.Interop.FORMATETC[1]
      {
        StructConverter.BclFormatETC2Ole(ref format)
      });
    }

    void System.Runtime.InteropServices.ComTypes.IDataObject.SetData(
      ref System.Runtime.InteropServices.ComTypes.FORMATETC formatIn,
      ref System.Runtime.InteropServices.ComTypes.STGMEDIUM medium,
      bool release)
    {
      if (this.bclData != null)
        this.bclData.SetData(ref formatIn, ref medium, release);
      else
        this.oleData.SetData(new Microsoft.VisualStudio.OLE.Interop.FORMATETC[1]
        {
          StructConverter.BclFormatETC2Ole(ref formatIn)
        }, new Microsoft.VisualStudio.OLE.Interop.STGMEDIUM[1]
        {
          StructConverter.BclSTGMEDIUM2Ole(ref medium)
        }, release ? 1 : 0);
    }
  }
}
