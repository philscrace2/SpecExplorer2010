// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Ole2Bcl.EnumSTATDATA
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell.Ole2Bcl
{
  internal sealed class EnumSTATDATA : Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA, System.Runtime.InteropServices.ComTypes.IEnumSTATDATA
  {
    private Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA oleEnum;
    private System.Runtime.InteropServices.ComTypes.IEnumSTATDATA bclEnum;

    private EnumSTATDATA()
    {
    }

    internal EnumSTATDATA(Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA oleEnum)
    {
      if (oleEnum == null)
        throw new ArgumentNullException("Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA");
      this.oleEnum = oleEnum;
      this.bclEnum = oleEnum as System.Runtime.InteropServices.ComTypes.IEnumSTATDATA;
    }

    internal EnumSTATDATA(System.Runtime.InteropServices.ComTypes.IEnumSTATDATA bclEnum)
    {
      if (bclEnum == null)
        throw new ArgumentNullException("System.Runtime.InteropServices.ComTypes.IEnumSTATDATA");
      this.oleEnum = bclEnum as Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA;
      this.bclEnum = bclEnum;
    }

    void Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA.Clone(
      out Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA ppEnum)
    {
      ppEnum = (Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA) null;
      if (this.oleEnum != null)
      {
        this.oleEnum.Clone(out ppEnum);
      }
      else
      {
        System.Runtime.InteropServices.ComTypes.IEnumSTATDATA newEnum;
        this.bclEnum.Clone(out newEnum);
        ppEnum = newEnum as Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA;
        if (ppEnum != null)
          return;
        ppEnum = (Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA) new EnumSTATDATA(newEnum);
      }
    }

    void System.Runtime.InteropServices.ComTypes.IEnumSTATDATA.Clone(
      out System.Runtime.InteropServices.ComTypes.IEnumSTATDATA newEnum)
    {
      newEnum = (System.Runtime.InteropServices.ComTypes.IEnumSTATDATA) null;
      if (this.bclEnum != null)
      {
        this.bclEnum.Clone(out newEnum);
      }
      else
      {
        Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA ppEnum;
        this.oleEnum.Clone(out ppEnum);
        newEnum = ppEnum as System.Runtime.InteropServices.ComTypes.IEnumSTATDATA;
        if (newEnum != null)
          return;
        newEnum = (System.Runtime.InteropServices.ComTypes.IEnumSTATDATA) new EnumSTATDATA(ppEnum);
      }
    }

    int Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA.Next(
      uint celt,
      Microsoft.VisualStudio.OLE.Interop.STATDATA[] rgelt,
      out uint pceltFetched)
    {
      pceltFetched = 0U;
      if (this.oleEnum != null)
        return this.oleEnum.Next(celt, rgelt, out pceltFetched);
      System.Runtime.InteropServices.ComTypes.STATDATA[] rgelt1 = new System.Runtime.InteropServices.ComTypes.STATDATA[(IntPtr) celt];
      int[] pceltFetched1 = new int[1]{ (int) pceltFetched };
      int hr = this.bclEnum.Next((int) celt, rgelt1, pceltFetched1);
      if (NativeMethods.Failed(hr))
        return hr;
      pceltFetched = (uint) pceltFetched1[0];
      for (int index = 0; (long) index < (long) pceltFetched; ++index)
        rgelt[index] = StructConverter.BclSTATDATA2Ole(ref rgelt1[index]);
      return hr;
    }

    int System.Runtime.InteropServices.ComTypes.IEnumSTATDATA.Next(
      int celt,
      System.Runtime.InteropServices.ComTypes.STATDATA[] rgelt,
      int[] pceltFetched)
    {
      if (this.bclEnum != null)
        return this.bclEnum.Next(celt, rgelt, pceltFetched);
      Microsoft.VisualStudio.OLE.Interop.STATDATA[] rgelt1 = new Microsoft.VisualStudio.OLE.Interop.STATDATA[celt];
      uint pceltFetched1;
      int hr = this.oleEnum.Next((uint) celt, rgelt1, out pceltFetched1);
      if (NativeMethods.Failed(hr))
        return hr;
      if (pceltFetched != null)
        pceltFetched[0] = (int) pceltFetched1;
      for (int index = 0; (long) index < (long) pceltFetched1; ++index)
        rgelt[index] = StructConverter.OleSTATDATA2Bcl(ref rgelt1[index]);
      return hr;
    }

    int Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA.Reset()
    {
      if (this.oleEnum != null)
        return this.oleEnum.Reset();
      return this.bclEnum.Reset();
    }

    int System.Runtime.InteropServices.ComTypes.IEnumSTATDATA.Reset()
    {
      if (this.bclEnum != null)
        return this.bclEnum.Reset();
      return this.oleEnum.Reset();
    }

    int Microsoft.VisualStudio.OLE.Interop.IEnumSTATDATA.Skip(uint celt)
    {
      if (this.oleEnum != null)
        return this.oleEnum.Skip(celt);
      return this.bclEnum.Skip((int) celt);
    }

    int System.Runtime.InteropServices.ComTypes.IEnumSTATDATA.Skip(int celt)
    {
      if (this.bclEnum != null)
        return this.bclEnum.Skip(celt);
      return this.oleEnum.Skip((uint) celt);
    }
  }
}
