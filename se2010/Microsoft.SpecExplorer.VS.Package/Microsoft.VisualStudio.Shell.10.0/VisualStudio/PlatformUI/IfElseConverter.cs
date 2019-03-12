// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.IfElseConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.PlatformUI.Common;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class IfElseConverter : IValueConverter
  {
    public object TrueValue { get; set; }

    public object FalseValue { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool? nullable = value as bool?;
      if (value == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ValueNotOfType, (object) typeof (bool).FullName));
      if (!nullable.Value)
        return this.FalseValue;
      return this.TrueValue;
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConverterFunctionNotDefined, (object) nameof (ConvertBack)));
    }
  }
}
