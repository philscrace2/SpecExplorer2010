// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.WinFormsToWin32IconConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public sealed class WinFormsToWin32IconConverter : IVsUIDataConverter
  {
    public int Convert(IVsUIObject pObject, out IVsUIObject ppConvertedObject)
    {
      if (pObject == null)
        throw new ArgumentNullException(nameof (pObject));
      object objectData = (object) (Utilities.GetObjectData(pObject) as Icon);
      if (!(objectData is Icon))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConversionFailed, (object) typeof (Icon).Name, (object) "WinForms", (object) "VsUI.Icon"));
      IntPtr handle = ((Icon) objectData).Handle;
      if (handle == IntPtr.Zero)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InvalidObjectHandle, (object) "VsUI.Icon"));
      ppConvertedObject = (IVsUIObject) new Win32IconUIObject(handle);
      return 0;
    }

    public int get_ConvertibleFormats(out uint pdwDataFormatFrom, out uint pdwDataFormatTo)
    {
      pdwDataFormatFrom = 2U;
      pdwDataFormatTo = 1U;
      return 0;
    }

    public int get_Type(out string pTypeName)
    {
      pTypeName = "VsUI.Icon";
      return 0;
    }
  }
}
