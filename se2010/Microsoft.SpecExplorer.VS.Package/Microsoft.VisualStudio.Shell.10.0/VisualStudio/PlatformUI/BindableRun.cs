﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.BindableRun
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Windows;
using System.Windows.Documents;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class BindableRun : Run
  {
    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof (Content), typeof (string), typeof (BindableRun), (PropertyMetadata) new UIPropertyMetadata((object) string.Empty, new PropertyChangedCallback(BindableRun.ContentChanged)));

    public string Content
    {
      get
      {
        return (string) this.GetValue(BindableRun.ContentProperty);
      }
      set
      {
        this.SetValue(BindableRun.ContentProperty, (object) value);
      }
    }

    private static void ContentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      ((BindableRun) o).ContentChanged((string) e.NewValue);
    }

    private void ContentChanged(string value)
    {
      this.Text = value;
    }
  }
}