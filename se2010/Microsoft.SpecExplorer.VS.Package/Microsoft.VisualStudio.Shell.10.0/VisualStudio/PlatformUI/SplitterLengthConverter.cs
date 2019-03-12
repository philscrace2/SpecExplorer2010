// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterLengthConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class SplitterLengthConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      switch (Type.GetTypeCode(sourceType))
      {
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
        case TypeCode.Single:
        case TypeCode.Double:
        case TypeCode.Decimal:
        case TypeCode.String:
          return true;
        default:
          return false;
      }
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType != typeof (InstanceDescriptor))
        return destinationType == typeof (string);
      return true;
    }

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      if (value == null || !this.CanConvertFrom(value.GetType()))
        throw this.GetConvertFromException(value);
      if (value is string)
        return (object) SplitterLengthConverter.FromString((string) value, culture);
      double d = Convert.ToDouble(value, (IFormatProvider) culture);
      if (double.IsNaN(d))
        d = 1.0;
      return (object) new SplitterLength(d, SplitterUnitType.Stretch);
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      if (destinationType == (Type) null)
        throw new ArgumentNullException(nameof (destinationType));
      if (value != null && value is SplitterLength)
      {
        SplitterLength length = (SplitterLength) value;
        if (destinationType == typeof (string))
          return (object) SplitterLengthConverter.ToString(length, culture);
        if (destinationType.IsEquivalentTo(typeof (InstanceDescriptor)))
          return (object) new InstanceDescriptor((MemberInfo) typeof (SplitterLength).GetConstructor(new Type[2]
          {
            typeof (double),
            typeof (SplitterUnitType)
          }), (ICollection) new object[2]
          {
            (object) length.Value,
            (object) length.SplitterUnitType
          });
      }
      throw this.GetConvertToException(value, destinationType);
    }

    internal static SplitterLength FromString(string s, CultureInfo cultureInfo)
    {
      string str = s.Trim();
      double num = 1.0;
      SplitterUnitType unitType = SplitterUnitType.Stretch;
      if (str == "*")
        unitType = SplitterUnitType.Fill;
      else
        num = Convert.ToDouble(str, (IFormatProvider) cultureInfo);
      return new SplitterLength(num, unitType);
    }

    internal static string ToString(SplitterLength length, CultureInfo cultureInfo)
    {
      if (length.SplitterUnitType == SplitterUnitType.Fill)
        return "*";
      return Convert.ToString(length.Value, (IFormatProvider) cultureInfo);
    }
  }
}
