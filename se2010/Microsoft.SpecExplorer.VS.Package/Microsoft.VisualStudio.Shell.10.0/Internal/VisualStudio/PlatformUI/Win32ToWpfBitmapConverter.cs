// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.Win32ToWpfBitmapConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public sealed class Win32ToWpfBitmapConverter : IVsUIDataConverter
  {
    public int Convert(IVsUIObject pObject, out IVsUIObject ppConvertedObject)
    {
      if (pObject == null)
        throw new ArgumentNullException(nameof (pObject));
      // ISSUE: variable of a compiler-generated type
      IVsUIWin32Bitmap objectData = Utilities.GetObjectData(pObject) as IVsUIWin32Bitmap;
      if (objectData == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConversionFailed, (object) typeof (IVsUIWin32Bitmap).Name, (object) "Win32", (object) "VsUI.Bitmap"));
      int phBitmap;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(objectData.GetHBITMAP(out phBitmap));
      IntPtr bitmap = (IntPtr) phBitmap;
      BitmapSource source = (BitmapSource) null;
      if (bitmap != IntPtr.Zero)
      {
        source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        bool pfHasAlpha;
        // ISSUE: reference to a compiler-generated method
        if (objectData.BitmapContainsAlphaInfo(out pfHasAlpha) != 0)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConversionFailed, (object) typeof (IVsUIWin32Bitmap).Name, (object) "Win32", (object) "VsUI.Bitmap"));
        if (!pfHasAlpha)
          source = (BitmapSource) new FormatConvertedBitmap(source, PixelFormats.Rgb24, (BitmapPalette) null, 0.0);
      }
      ppConvertedObject = WpfPropertyValue.CreateBitmapObject((ImageSource) source);
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
      pTypeName = "VsUI.Bitmap";
      return 0;
    }
  }
}
