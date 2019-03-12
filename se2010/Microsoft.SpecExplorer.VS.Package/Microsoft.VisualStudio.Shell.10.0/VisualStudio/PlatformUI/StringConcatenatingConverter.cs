// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.StringConcatenatingConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class StringConcatenatingConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = (string) parameter;
      if (str1 == string.Empty)
        str1 = (string) null;
      foreach (object obj in values)
      {
        string str2 = obj != null ? obj.ToString() : string.Empty;
        if (!string.IsNullOrEmpty(str2))
        {
          if (str1 != null && stringBuilder.Length > 0)
            stringBuilder.Append(str1);
          stringBuilder.Append(str2);
        }
      }
      return (object) stringBuilder.ToString();
    }

    public object[] ConvertBack(
      object value,
      Type[] targetTypes,
      object parameter,
      CultureInfo culture)
    {
      object[] objArray = new object[targetTypes.Length];
      for (int index = 0; index < objArray.Length; ++index)
        objArray[index] = DependencyProperty.UnsetValue;
      return objArray;
    }
  }
}
