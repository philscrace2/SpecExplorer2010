// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.NullableBooleanConverter
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.SpecExplorer.VS
{
  [ValueConversion(typeof (bool?), typeof (bool))]
  public class NullableBooleanConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return (object) false;
      bool flag = parameter == null || bool.Parse(parameter.ToString());
      return (object) !((bool) value ^ flag);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      bool flag = parameter == null || bool.Parse(parameter.ToString());
      return (object) !((bool) value ^ flag);
    }
  }
}
