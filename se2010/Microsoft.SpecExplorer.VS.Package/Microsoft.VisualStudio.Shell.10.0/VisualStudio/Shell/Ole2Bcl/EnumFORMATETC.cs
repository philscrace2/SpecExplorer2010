// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Ole2Bcl.EnumFORMATETC
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell.Ole2Bcl
{
  internal sealed class EnumFORMATETC : Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC, System.Runtime.InteropServices.ComTypes.IEnumFORMATETC
  {
    private Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC oleEnum;
    private System.Runtime.InteropServices.ComTypes.IEnumFORMATETC bclEnum;

    private EnumFORMATETC()
    {
    }

    internal EnumFORMATETC(Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC oleEnum)
    {
      if (oleEnum == null)
        throw new ArgumentNullException("Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC");
      this.oleEnum = oleEnum;
      this.bclEnum = oleEnum as System.Runtime.InteropServices.ComTypes.IEnumFORMATETC;
    }

    internal EnumFORMATETC(System.Runtime.InteropServices.ComTypes.IEnumFORMATETC bclEnum)
    {
      if (bclEnum == null)
        throw new ArgumentNullException("System.Runtime.InteropServices.ComTypes.IEnumFORMATETC");
      this.oleEnum = bclEnum as Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC;
      this.bclEnum = bclEnum;
    }

    void Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC.Clone(
      out Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC ppEnum)
    {
      ppEnum = (Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC) null;
      if (this.oleEnum != null)
      {
        this.oleEnum.Clone(out ppEnum);
      }
      else
      {
        System.Runtime.InteropServices.ComTypes.IEnumFORMATETC newEnum;
        this.bclEnum.Clone(out newEnum);
        ppEnum = newEnum as Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC;
        if (ppEnum != null)
          return;
        ppEnum = (Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC) new EnumFORMATETC(newEnum);
      }
    }

    void System.Runtime.InteropServices.ComTypes.IEnumFORMATETC.Clone(
      out System.Runtime.InteropServices.ComTypes.IEnumFORMATETC newEnum)
    {
      newEnum = (System.Runtime.InteropServices.ComTypes.IEnumFORMATETC) null;
      if (this.bclEnum != null)
      {
        this.bclEnum.Clone(out newEnum);
      }
      else
      {
        Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC ppEnum;
        this.oleEnum.Clone(out ppEnum);
        newEnum = ppEnum as System.Runtime.InteropServices.ComTypes.IEnumFORMATETC;
        if (newEnum != null)
          return;
        newEnum = (System.Runtime.InteropServices.ComTypes.IEnumFORMATETC) new EnumFORMATETC(ppEnum);
      }
    }

    int Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC.Next(
      uint celt,
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] rgelt,
      uint[] pceltFetched)
    {
      if (this.oleEnum != null)
        return this.oleEnum.Next(celt, rgelt, pceltFetched);
      System.Runtime.InteropServices.ComTypes.FORMATETC[] rgelt1 = new System.Runtime.InteropServices.ComTypes.FORMATETC[(IntPtr) celt];
      int[] pceltFetched1 = new int[1];
      int hr = this.bclEnum.Next((int) celt, rgelt1, pceltFetched1);
      if (NativeMethods.Failed(hr))
        return hr;
      if (pceltFetched != null)
        pceltFetched[0] = (uint) pceltFetched1[0];
      for (int index = 0; index < pceltFetched1[0]; ++index)
        rgelt[index] = StructConverter.BclFormatETC2Ole(ref rgelt1[index]);
      return hr;
    }

    int System.Runtime.InteropServices.ComTypes.IEnumFORMATETC.Next(
      int celt,
      System.Runtime.InteropServices.ComTypes.FORMATETC[] rgelt,
      int[] pceltFetched)
    {
      if (this.bclEnum != null)
        return this.bclEnum.Next(celt, rgelt, pceltFetched);
      Microsoft.VisualStudio.OLE.Interop.FORMATETC[] rgelt1 = new Microsoft.VisualStudio.OLE.Interop.FORMATETC[celt];
      uint[] pceltFetched1 = new uint[1];
      int hr = this.oleEnum.Next((uint) celt, rgelt1, pceltFetched1);
      if (NativeMethods.Failed(hr))
        return hr;
      if (pceltFetched != null)
        pceltFetched[0] = (int) pceltFetched1[0];
      for (uint index = 0; index < pceltFetched1[0]; ++index)
        rgelt[(IntPtr) index] = StructConverter.OleFormatETC2Bcl(ref rgelt1[(IntPtr) index]);
      return hr;
    }

    int Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC.Reset()
    {
      if (this.oleEnum != null)
        return this.oleEnum.Reset();
      return this.bclEnum.Reset();
    }

    int System.Runtime.InteropServices.ComTypes.IEnumFORMATETC.Reset()
    {
      if (this.bclEnum != null)
        return this.bclEnum.Reset();
      return this.oleEnum.Reset();
    }

    int Microsoft.VisualStudio.OLE.Interop.IEnumFORMATETC.Skip(uint celt)
    {
      if (this.oleEnum != null)
        return this.oleEnum.Skip(celt);
      return this.bclEnum.Skip((int) celt);
    }

    int System.Runtime.InteropServices.ComTypes.IEnumFORMATETC.Skip(int celt)
    {
      if (this.bclEnum != null)
        return this.bclEnum.Skip(celt);
      return this.oleEnum.Skip((uint) celt);
    }
  }
}
