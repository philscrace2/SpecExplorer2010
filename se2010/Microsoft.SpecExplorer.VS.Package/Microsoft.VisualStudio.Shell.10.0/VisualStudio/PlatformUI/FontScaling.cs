// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.FontScaling
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI
{
  public static class FontScaling
  {
    public static readonly DependencyProperty PreScaledFontSizeProperty = DependencyProperty.RegisterAttached("PreScaledFontSize", typeof (double), typeof (FontScaling), (PropertyMetadata) new FrameworkPropertyMetadata((object) 10.0));

    public static double GetPreScaledFontSize(DependencyObject element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      return (double) element.GetValue(FontScaling.PreScaledFontSizeProperty);
    }

    public static void SetPreScaledFontSize(DependencyObject element, double value)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      element.SetValue(FontScaling.PreScaledFontSizeProperty, (object) value);
    }
  }
}
