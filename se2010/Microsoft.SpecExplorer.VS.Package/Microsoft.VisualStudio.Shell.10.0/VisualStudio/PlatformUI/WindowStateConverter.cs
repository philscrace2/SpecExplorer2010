// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.WindowStateConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.PlatformUI.Common;
using System;
using System.Globalization;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class WindowStateConverter : ValueConverter<int, WindowState>
  {
    protected override WindowState Convert(
      int state,
      object parameter,
      CultureInfo culture)
    {
      switch (state)
      {
        case 1:
          return WindowState.Normal;
        case 2:
          return WindowState.Minimized;
        case 3:
          return WindowState.Maximized;
        default:
          throw new ArgumentException(Resources.Error_InvalidWin32State);
      }
    }

    protected override int ConvertBack(WindowState state, object parameter, CultureInfo culture)
    {
      switch (state)
      {
        case WindowState.Normal:
          return 1;
        case WindowState.Minimized:
          return 2;
        case WindowState.Maximized:
          return 3;
        default:
          throw new ArgumentException(Resources.Error_InvalidWPFState);
      }
    }
  }
}
