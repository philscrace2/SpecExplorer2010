// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.WinFormsImageListUIObject
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public sealed class WinFormsImageListUIObject : IVsUIObject
  {
    private ImageList imageList;

    public WinFormsImageListUIObject(ImageList imageList)
    {
      if (imageList == null)
        throw new ArgumentNullException(nameof (imageList));
      this.imageList = imageList;
    }

    public int Equals(IVsUIObject pOtherObject, out bool pfAreEqual)
    {
      if (pOtherObject == null)
        throw new ArgumentNullException(nameof (pOtherObject));
      object pVar;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(pOtherObject.get_Data(out pVar));
      if (object.ReferenceEquals(pVar, (object) this.imageList))
      {
        pfAreEqual = true;
        return 0;
      }
      // ISSUE: variable of a compiler-generated type
      __VSUIDATAFORMAT objectFormat = Utilities.GetObjectFormat(pOtherObject);
      switch (objectFormat)
      {
        case __VSUIDATAFORMAT.VSDF_WIN32:
          if (pVar is IVsUIWin32ImageList)
          {
            int phImageList;
            // ISSUE: reference to a compiler-generated method
            Marshal.ThrowExceptionForHR(((IVsUIWin32ImageList) pVar).GetHIMAGELIST(out phImageList));
            pfAreEqual = phImageList == this.imageList.Handle.ToInt32();
            return 0;
          }
          break;
        case __VSUIDATAFORMAT.VSDF_WINFORMS:
          if (pVar is ImageList)
          {
            pfAreEqual = ((ImageList) pVar).Handle == this.imageList.Handle;
            return 0;
          }
          break;
      }
      pfAreEqual = false;
      return 0;
    }

    public int get_Data(out object pVar)
    {
      pVar = (object) this.imageList;
      return 0;
    }

    public int get_Format(out uint pdwDataFormat)
    {
      pdwDataFormat = 2U;
      return 0;
    }

    public int get_Type(out string pTypeName)
    {
      pTypeName = "VsUI.ImageList";
      return 0;
    }
  }
}
