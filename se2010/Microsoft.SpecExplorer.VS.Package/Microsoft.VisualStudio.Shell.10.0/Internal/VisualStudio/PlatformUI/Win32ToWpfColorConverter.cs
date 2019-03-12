// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.Win32ToWpfColorConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Globalization;
using System.Windows.Media;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public sealed class Win32ToWpfColorConverter : IVsUIDataConverter
  {
    public int Convert(IVsUIObject pObject, out IVsUIObject ppConvertedObject)
    {
      if (pObject == null)
        throw new ArgumentNullException(nameof (pObject));
      object objectData = Utilities.GetObjectData(pObject);
      if (!(objectData is uint))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConversionFailed, (object) typeof (uint).Name, (object) "Win32", (object) "VsUI.Color"));
      Color mediaColor = new NativeMethods.COLORREF((uint) objectData).GetMediaColor();
      ppConvertedObject = (IVsUIObject) new WpfColorUIObject(mediaColor);
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
      pTypeName = "VsUI.Color";
      return 0;
    }
  }
}
