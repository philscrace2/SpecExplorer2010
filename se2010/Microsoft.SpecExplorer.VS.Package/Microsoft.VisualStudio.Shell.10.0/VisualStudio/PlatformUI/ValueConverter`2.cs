// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.ValueConverter`2
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.PlatformUI.Common;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class ValueConverter<TSource, TTarget> : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is TSource) && (value != null || typeof (TSource).IsValueType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ValueNotOfType, (object) typeof (TSource).FullName));
      if (!targetType.IsAssignableFrom(typeof (TTarget)))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_TargetNotExtendingType, (object) typeof (TTarget).FullName));
      return (object) this.Convert((TSource) value, parameter, culture);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      if (!(value is TTarget) && (value != null || typeof (TTarget).IsValueType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ValueNotOfType, (object) typeof (TTarget).FullName));
      if (!targetType.IsAssignableFrom(typeof (TSource)))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_TargetNotExtendingType, (object) typeof (TSource).FullName));
      return (object) this.ConvertBack((TTarget) value, parameter, culture);
    }

    protected virtual TTarget Convert(TSource value, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConverterFunctionNotDefined, (object) nameof (Convert)));
    }

    protected virtual TSource ConvertBack(TTarget value, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ConverterFunctionNotDefined, (object) nameof (ConvertBack)));
    }
  }
}
