// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.Win32BitmapHandle
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class Win32BitmapHandle : IVsUIWin32Bitmap
  {
    private readonly IntPtr _handle;
    private readonly bool _hasAlpha;

    public Win32BitmapHandle(IntPtr handle, bool hasAlpha)
    {
      this._handle = handle;
      this._hasAlpha = hasAlpha;
    }

    public int GetHBITMAP(out int phBitmap)
    {
      phBitmap = this._handle.ToInt32();
      return 0;
    }

    public int BitmapContainsAlphaInfo(out bool hasAlpha)
    {
      hasAlpha = this._hasAlpha;
      return 0;
    }
  }
}
