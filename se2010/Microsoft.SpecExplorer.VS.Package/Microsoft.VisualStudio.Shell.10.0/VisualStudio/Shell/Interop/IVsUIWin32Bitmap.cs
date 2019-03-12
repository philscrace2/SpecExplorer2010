// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Interop.IVsUIWin32Bitmap
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Interop
{
  [TypeIdentifier]
  [Guid("0DD7CF3A-6203-466F-B1C1-7653809CB73A")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CompilerGenerated]
  [ComImport]
  public interface IVsUIWin32Bitmap
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetHBITMAP(out int phBitmap);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int BitmapContainsAlphaInfo(out bool pfHasAlpha);
  }
}
