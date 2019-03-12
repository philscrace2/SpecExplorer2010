// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.OrBooleanConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.PlatformUI.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class OrBooleanConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = false;
      foreach (object obj in values)
      {
        if (obj == DependencyProperty.UnsetValue)
          flag = true;
        else if ((bool) obj)
          return (object) true;
      }
      if (!flag)
        return (object) false;
      return DependencyProperty.UnsetValue;
    }

    public object[] ConvertBack(
      object value,
      Type[] targetTypes,
      object parameter,
      CultureInfo culture)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConverterFunctionNotDefined, (object) nameof (ConvertBack)));
    }
  }
}
