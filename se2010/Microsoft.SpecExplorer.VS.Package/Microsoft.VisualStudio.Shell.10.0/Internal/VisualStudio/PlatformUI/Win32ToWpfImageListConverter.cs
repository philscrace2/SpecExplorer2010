// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.Win32ToWpfImageListConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public sealed class Win32ToWpfImageListConverter : IVsUIDataConverter
  {
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
      IntPtr himl = (IntPtr) phImageList;
      List<ImageSource> imageSourceList = (List<ImageSource>) null;
      if (himl != IntPtr.Zero)
      {
        int imageCount = Microsoft.VisualStudio.NativeMethods.ImageList_GetImageCount(himl);
        imageSourceList = new List<ImageSource>(imageCount);
        for (int i = 0; i < imageCount; ++i)
        {
          IntPtr icon = Microsoft.VisualStudio.NativeMethods.ImageList_GetIcon(himl, i, 1U);
          if (icon == IntPtr.Zero)
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CannotGetImageFromList, (object) i));
          ImageSource imageSource = (ImageSource) null;
          try
          {
            imageSource = (ImageSource) System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
          }
          finally
          {
            Microsoft.VisualStudio.NativeMethods.DestroyIcon(icon);
          }
          imageSourceList.Add(imageSource);
        }
      }
      ppConvertedObject = (IVsUIObject) new WpfPropertyValue((object) imageSourceList);
      return 0;
    }

    public int get_ConvertibleFormats(out uint pdwDataFormatFrom, out uint pdwDataFormatTo)
    {
      pdwDataFormatFrom = 1U;
      pdwDataFormatTo = 3U;
      return 0;
    }

    public int get_Type(out string pTypeName)
    {
      pTypeName = "VsUI.ImageList";
      return 0;
    }
  }
}
