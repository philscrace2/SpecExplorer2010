// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.SettableOleDataObject
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public class SettableOleDataObject : Microsoft.VisualStudio.OLE.Interop.IDataObject
  {
    private System.Windows.Forms.IDataObject _inner;
    private Microsoft.VisualStudio.OLE.Interop.IDataObject _innerOle;

    public SettableOleDataObject(System.Windows.Forms.IDataObject inner)
    {
      this._inner = inner;
      this._innerOle = inner as Microsoft.VisualStudio.OLE.Interop.IDataObject;
      if (this._innerOle == null)
        throw new ArgumentException("Data object should implement Microsoft.VisualStudio.OLE.Interop.IDataObject.", nameof (inner));
    }

    public int DAdvise(
      FORMATETC[] pFormatetc,
      uint ADVF,
      IAdviseSink pAdvSink,
      out uint pdwConnection)
    {
      return this._innerOle.DAdvise(pFormatetc, ADVF, pAdvSink, out pdwConnection);
    }

    public void DUnadvise(uint dwConnection)
    {
      this._innerOle.DUnadvise(dwConnection);
    }

    public int EnumDAdvise(out IEnumSTATDATA ppenumAdvise)
    {
      return this._innerOle.EnumDAdvise(out ppenumAdvise);
    }

    public int EnumFormatEtc(uint dwDirection, out IEnumFORMATETC ppenumFormatEtc)
    {
      return this._innerOle.EnumFormatEtc(dwDirection, out ppenumFormatEtc);
    }

    public int GetCanonicalFormatEtc(FORMATETC[] pformatectIn, FORMATETC[] pformatetcOut)
    {
      return this._innerOle.GetCanonicalFormatEtc(pformatectIn, pformatetcOut);
    }

    public void GetData(FORMATETC[] pformatetcIn, STGMEDIUM[] pRemoteMedium)
    {
      this._innerOle.GetData(pformatetcIn, pRemoteMedium);
    }

    public void GetDataHere(FORMATETC[] pFormatetc, STGMEDIUM[] pRemoteMedium)
    {
      this._innerOle.GetDataHere(pFormatetc, pRemoteMedium);
    }

    public int QueryGetData(FORMATETC[] pFormatetc)
    {
      return this._innerOle.QueryGetData(pFormatetc);
    }

    public unsafe void SetData(FORMATETC[] pFormatetc, STGMEDIUM[] pmedium, int fRelease)
    {
      if (pFormatetc[0].tymed == 1U && pFormatetc[0].ptd == IntPtr.Zero && pFormatetc[0].dwAspect == 1U)
      {
        HandleRef handle = new HandleRef((object) null, pmedium[0].unionmember);
        IntPtr num = Microsoft.VisualStudio.NativeMethods.GlobalLock(handle);
        try
        {
          UnmanagedMemoryStream unmanagedMemoryStream = new UnmanagedMemoryStream((byte*) (void*) num, (long) Microsoft.VisualStudio.NativeMethods.GlobalSize(handle));
          byte[] buffer = new byte[unmanagedMemoryStream.Length];
          unmanagedMemoryStream.Read(buffer, 0, buffer.Length);
          MemoryStream memoryStream = new MemoryStream(buffer);
          this._inner.SetData(DataFormats.GetFormat((int) pFormatetc[0].cfFormat).Name, (object) memoryStream);
          if (fRelease == 0)
            return;
          Microsoft.VisualStudio.NativeMethods.GlobalUnlock(handle);
          Microsoft.VisualStudio.NativeMethods.GlobalFree(handle);
          handle = new HandleRef((object) null, IntPtr.Zero);
        }
        finally
        {
          if (handle.Handle != IntPtr.Zero)
            Microsoft.VisualStudio.NativeMethods.GlobalUnlock(handle);
        }
      }
      else
        Marshal.ThrowExceptionForHR(-2147467263);
    }
  }
}
