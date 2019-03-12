// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.RangeValidationRule
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.PlatformUI.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class RangeValidationRule : BindableValidationRule
  {
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof (Minimum), typeof (int), typeof (RangeValidationRule));
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof (Maximum), typeof (int), typeof (RangeValidationRule));

    public int Minimum
    {
      get
      {
        return (int) this.BindingTarget.GetValue(RangeValidationRule.MinimumProperty);
      }
      set
      {
        this.BindingTarget.SetValue(RangeValidationRule.MinimumProperty, (object) value);
      }
    }

    public int Maximum
    {
      get
      {
        return (int) this.BindingTarget.GetValue(RangeValidationRule.MaximumProperty);
      }
      set
      {
        this.BindingTarget.SetValue(RangeValidationRule.MaximumProperty, (object) value);
      }
    }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      ValidationResult validationResult = new ValidationResult(false, (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_IntegerValueNotInRange, (object) this.Minimum, (object) this.Maximum));
      string s = value as string;
      int result;
      if (string.IsNullOrEmpty(s) || !int.TryParse(s, out result) || (result < this.Minimum || result > this.Maximum))
        return validationResult;
      return ValidationResult.ValidResult;
    }
  }
}
