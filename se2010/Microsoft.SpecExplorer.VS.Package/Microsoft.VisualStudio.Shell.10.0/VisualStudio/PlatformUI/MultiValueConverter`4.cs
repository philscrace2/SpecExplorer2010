// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.MultiValueConverter`4
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
  public class MultiValueConverter<TSource1, TSource2, TSource3, TTarget> : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length != 3)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InsufficientSourceParameters, (object) 3));
      foreach (object obj in values)
      {
        if (obj == DependencyProperty.UnsetValue)
          return (object) default (TTarget);
      }
      MultiValueHelper.CheckValue<TSource1>(values, 0);
      MultiValueHelper.CheckValue<TSource2>(values, 1);
      MultiValueHelper.CheckValue<TSource3>(values, 2);
      if (!targetType.IsAssignableFrom(typeof (TTarget)))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_TargetNotExtendingType, (object) typeof (TTarget).FullName));
      return (object) this.Convert((TSource1) values[0], (TSource2) values[1], (TSource3) values[2], parameter, culture);
    }

    public object[] ConvertBack(
      object value,
      Type[] targetTypes,
      object parameter,
      CultureInfo culture)
    {
      if (targetTypes.Length != 3)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InsufficientTypeParameters, (object) 3));
      if (!(value is TTarget) && (value != null || typeof (TTarget).IsValueType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ValueNotOfType, (object) typeof (TTarget).FullName));
      MultiValueHelper.CheckType<TSource1>(targetTypes, 0);
      MultiValueHelper.CheckType<TSource2>(targetTypes, 1);
      MultiValueHelper.CheckType<TSource3>(targetTypes, 2);
      TSource1 out1;
      TSource2 out2;
      TSource3 out3;
      this.ConvertBack((TTarget) value, out out1, out out2, out out3, parameter, culture);
      return new object[3]
      {
        (object) out1,
        (object) out2,
        (object) out3
      };
    }

    protected virtual TTarget Convert(
      TSource1 value1,
      TSource2 value2,
      TSource3 value3,
      object parameter,
      CultureInfo culture)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConverterFunctionNotDefined, (object) nameof (Convert)));
    }

    protected virtual void ConvertBack(
      TTarget value,
      out TSource1 out1,
      out TSource2 out2,
      out TSource3 out3,
      object parameter,
      CultureInfo culture)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConverterFunctionNotDefined, (object) nameof (ConvertBack)));
    }
  }
}
