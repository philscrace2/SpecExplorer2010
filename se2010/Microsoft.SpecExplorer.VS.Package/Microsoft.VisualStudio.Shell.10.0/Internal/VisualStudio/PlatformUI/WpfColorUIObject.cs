// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.WpfColorUIObject
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
  public sealed class WpfColorUIObject : IVsUIObject
  {
    private Color color;

    public WpfColorUIObject(Color color)
    {
      this.color = color;
    }

    public int Equals(IVsUIObject pOtherObject, out bool pfAreEqual)
    {
      if (pOtherObject == null)
        throw new ArgumentNullException(nameof (pOtherObject));
      object objectData = Utilities.GetObjectData(pOtherObject);
      if (object.ReferenceEquals(objectData, (object) this.color))
      {
        pfAreEqual = true;
        return 0;
      }
      // ISSUE: variable of a compiler-generated type
      __VSUIDATAFORMAT objectFormat = Utilities.GetObjectFormat(pOtherObject);
      switch (objectFormat)
      {
        case __VSUIDATAFORMAT.VSDF_WIN32:
          if (objectData is uint)
          {
            NativeMethods.COLORREF colorref = new NativeMethods.COLORREF((uint) objectData);
            pfAreEqual = this.color.Equals(colorref.GetMediaColor());
            return 0;
          }
          break;
        case __VSUIDATAFORMAT.VSDF_WPF:
          if (objectData is Color)
          {
            pfAreEqual = this.color.Equals(objectData);
            return 0;
          }
          break;
      }
      pfAreEqual = false;
      return 0;
    }

    public int get_Data(out object pVar)
    {
      pVar = (object) this.color;
      return 0;
    }

    public int get_Format(out uint pdwDataFormat)
    {
      pdwDataFormat = 3U;
      return 0;
    }

    public int get_Type(out string pTypeName)
    {
      pTypeName = "VsUI.Color";
      return 0;
    }
  }
}
