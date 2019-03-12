// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.Win32ToWinFormsIconConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public sealed class Win32ToWinFormsIconConverter : IVsUIDataConverter
  {
    public int Convert(IVsUIObject pObject, out IVsUIObject ppConvertedObject)
    {
      if (pObject == null)
        throw new ArgumentNullException(nameof (pObject));
      // ISSUE: variable of a compiler-generated type
      IVsUIWin32Icon objectData = Utilities.GetObjectData(pObject) as IVsUIWin32Icon;
      if (objectData == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConversionFailed, (object) typeof (IVsUIWin32Icon).Name, (object) "Win32", (object) "VsUI.Icon"));
      int phIcon;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(objectData.GetHICON(out phIcon));
      IntPtr handle = (IntPtr) phIcon;
      if (handle == IntPtr.Zero)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InvalidObjectHandle, (object) "VsUI.Icon"));
      Icon icon = Icon.FromHandle(handle);
      ppConvertedObject = (IVsUIObject) new WinFormsIconUIObject(icon);
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
      pTypeName = "VsUI.Icon";
      return 0;
    }
  }
}
