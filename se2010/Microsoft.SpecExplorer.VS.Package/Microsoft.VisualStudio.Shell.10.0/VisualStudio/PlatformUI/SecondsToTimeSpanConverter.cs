// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SecondsToTimeSpanConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class SecondsToTimeSpanConverter : ValueConverter<uint, TimeSpan>
  {
    protected override TimeSpan Convert(
      uint seconds,
      object parameter,
      CultureInfo culture)
    {
      return TimeSpan.FromSeconds((double) seconds);
    }

    protected override uint ConvertBack(TimeSpan span, object parameter, CultureInfo culture)
    {
      return (uint) span.TotalSeconds;
    }
  }
}
