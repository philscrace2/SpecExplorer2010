// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.Win32ToWinFormsImageListConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public sealed class Win32ToWinFormsImageListConverter : IVsUIDataConverter
  {
    private static void GetHImageListInfo(
      IntPtr hImageList,
      out ColorDepth colorDepth,
      out Size size,
      out int numberOfImages,
      out Color backgroundColor)
    {
      colorDepth = ColorDepth.Depth8Bit;
      size = new Size(0, 0);
      numberOfImages = Microsoft.VisualStudio.NativeMethods.ImageList_GetImageCount(hImageList);
      backgroundColor = ColorTranslator.FromWin32(Microsoft.VisualStudio.NativeMethods.ImageList_GetBkColor(hImageList));
      Microsoft.VisualStudio.NativeMethods.IMAGEINFO pImageInfo = new Microsoft.VisualStudio.NativeMethods.IMAGEINFO();
      if (Microsoft.VisualStudio.NativeMethods.ImageList_GetImageInfo(hImageList, 0, out pImageInfo))
      {
        size.Width = pImageInfo.rcImage.Width;
        size.Height = pImageInfo.rcImage.Height;
        IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf(typeof (Microsoft.VisualStudio.NativeMethods.BITMAP)));
        if (Microsoft.VisualStudio.NativeMethods.GetObject(pImageInfo.hbmImage, Marshal.SizeOf(typeof (Microsoft.VisualStudio.NativeMethods.BITMAP)), num) != 0)
        {
          Microsoft.VisualStudio.NativeMethods.BITMAP structure = (Microsoft.VisualStudio.NativeMethods.BITMAP) Marshal.PtrToStructure(num, typeof (Microsoft.VisualStudio.NativeMethods.BITMAP));
          colorDepth = (ColorDepth) structure.bmBitsPixel;
        }
        Marshal.FreeHGlobal(num);
      }
      else
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CannotGetImageInfo, (object) 0));
    }

    public int Convert(IVsUIObject pObject, out IVsUIObject ppConvertedObject)
    {
      if (pObject == null)
        throw new ArgumentNullException(nameof (pObject));
      // ISSUE: variable of a compiler-generated type
      IVsUIWin32ImageList objectData = Utilities.GetObjectData(pObject) as IVsUIWin32ImageList;
      if (objectData == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConversionFailed, (object) typeof (IVsUIWin32ImageList).Name, (object) "Win32", (object) "VsUI.ImageList"));
      int phImageList;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(objectData.GetHIMAGELIST(out phImageList));
      IntPtr num = (IntPtr) phImageList;
      if (num == IntPtr.Zero)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InvalidObjectHandle, (object) "VsUI.ImageList"));
      ImageList imageList = new ImageList();
      int numberOfImages = 0;
      Color backgroundColor = Color.Transparent;
      ColorDepth colorDepth = ColorDepth.Depth8Bit;
      Size size = new Size();
      Win32ToWinFormsImageListConverter.GetHImageListInfo(num, out colorDepth, out size, out numberOfImages, out backgroundColor);
      imageList.ColorDepth = colorDepth;
      imageList.ImageSize = size;
      imageList.TransparentColor = backgroundColor;
      for (int i = 0; i < numberOfImages; ++i)
      {
        IntPtr icon1 = Microsoft.VisualStudio.NativeMethods.ImageList_GetIcon(num, i, 1U);
        if (icon1 == IntPtr.Zero)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CannotGetImageFromList, (object) i));
        Icon icon2 = Icon.FromHandle(icon1);
        imageList.Images.Add(icon2);
      }
      if (!imageList.HandleCreated)
      {
        IntPtr handle = imageList.Handle;
      }
      ppConvertedObject = (IVsUIObject) new WinFormsImageListUIObject(imageList);
      return 0;
    }

    public int get_ConvertibleFormats(out uint pdwDataFormatFrom, out uint pdwDataFormatTo)
    {
      pdwDataFormatFrom = 1U;
      pdwDataFormatTo = 2U;
      return 0;
    }

    public int get_Type(out string pTypeName)
    {
      pTypeName = "VsUI.ImageList";
      return 0;
    }
  }
}
