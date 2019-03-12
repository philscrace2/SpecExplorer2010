// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.DeviceToLogicalPixelConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class DeviceToLogicalPixelConverter : ValueConverter<int, double>
  {
    private const string HorizontalAxis = "HorizontalAxis";
    private const string VerticalAxis = "VerticalAxis";

    protected override double Convert(int value, object parameter, CultureInfo culture)
    {
      if (parameter == null)
        throw new ArgumentNullException(nameof (parameter), "Converter parameter should be a string that defines the axis of conversion, either 'HorizontalAxis' or 'VerticalAxis'.");
      string str = parameter as string;
      if (str == null || str != "HorizontalAxis" && str != "VerticalAxis")
        throw new ArgumentException("Converter parameter should be a string that defines the axis of conversion, either 'HorizontalAxis' or 'VerticalAxis'.", nameof (parameter));
      if (str == "HorizontalAxis")
        return (double) value * DpiHelper.DeviceToLogicalUnitsScalingFactorX;
      return (double) value * DpiHelper.DeviceToLogicalUnitsScalingFactorY;
    }
  }
}
