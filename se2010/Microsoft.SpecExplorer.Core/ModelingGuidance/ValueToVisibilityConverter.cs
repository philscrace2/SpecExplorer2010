// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ModelingGuidance.ValueToVisibilityConverter
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
  public class ValueToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag1 = parameter == null || bool.Parse(parameter.ToString());
      if (!(value is bool flag2))
        flag2 = !(value is int num2) ? value != null : num2 != 0;
      return (object) (Visibility) ((flag1 ? (flag2 ? 1 : 0) : (!flag2 ? 1 : 0)) != 0 ? 0 : 2);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
