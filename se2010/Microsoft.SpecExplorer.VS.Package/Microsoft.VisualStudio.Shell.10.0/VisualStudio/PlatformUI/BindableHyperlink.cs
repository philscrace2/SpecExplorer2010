﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.BindableHyperlink
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Windows;
using System.Windows.Documents;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class BindableHyperlink : Hyperlink
  {
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof (Content), typeof (string), typeof (BindableHyperlink), (PropertyMetadata) new UIPropertyMetadata((object) string.Empty, new PropertyChangedCallback(BindableHyperlink.ContentChanged)));

    public string Content
    {
      get
      {
        return (string) this.GetValue(BindableHyperlink.ContentProperty);
      }
      set
      {
        this.SetValue(BindableHyperlink.ContentProperty, (object) value);
      }
    }

    private static void ContentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      ((BindableHyperlink) o).ContentChanged((string) e.NewValue);
    }

    private void ContentChanged(string value)
    {
      this.Inlines.Clear();
      if (string.IsNullOrEmpty(value))
        return;
      this.Inlines.Add((Inline) new Run(value));
    }
  }
}
