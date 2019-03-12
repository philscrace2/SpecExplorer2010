// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.Win32ColorUIObject
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows.Media;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public sealed class Win32ColorUIObject : IVsUIObject
  {
    private uint colorref;

    public Win32ColorUIObject(uint colorref)
    {
      this.colorref = colorref;
    }

    public int Equals(IVsUIObject pOtherObject, out bool pfAreEqual)
    {
      if (pOtherObject == null)
        throw new ArgumentNullException(nameof (pOtherObject));
      object objectData = Utilities.GetObjectData(pOtherObject);
      // ISSUE: variable of a compiler-generated type
      __VSUIDATAFORMAT objectFormat = Utilities.GetObjectFormat(pOtherObject);
      switch (objectFormat)
      {
        case __VSUIDATAFORMAT.VSDF_WIN32:
          if (objectData is uint)
          {
            pfAreEqual = (int) (uint) objectData == (int) this.colorref;
            return 0;
          }
          break;
        case __VSUIDATAFORMAT.VSDF_WPF:
          if (objectData is Color)
          {
            NativeMethods.COLORREF colorref = new NativeMethods.COLORREF((Color) objectData);
            pfAreEqual = (int) colorref.dwColor == (int) this.colorref;
            return 0;
          }
          break;
      }
      pfAreEqual = false;
      return 0;
    }

    public int get_Data(out object pVar)
    {
      pVar = (object) this.colorref;
      return 0;
    }

    public int get_Format(out uint pdwDataFormat)
    {
      pdwDataFormat = 1U;
      return 0;
    }

    public int get_Type(out string pTypeName)
    {
      pTypeName = "VsUI.Color";
      return 0;
    }
  }
}
