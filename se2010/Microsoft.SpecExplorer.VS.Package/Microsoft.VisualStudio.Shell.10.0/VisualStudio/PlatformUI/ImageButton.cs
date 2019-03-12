// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.ImageButton
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class ImageButton : Button
  {
    public static readonly DependencyProperty ImageNormalProperty = DependencyProperty.Register(nameof (ImageNormal), typeof (ImageSource), typeof (ImageButton));
    public static readonly DependencyProperty ImageHoverProperty = DependencyProperty.Register(nameof (ImageHover), typeof (ImageSource), typeof (ImageButton));
    public static readonly DependencyProperty ImagePressedProperty = DependencyProperty.Register(nameof (ImagePressed), typeof (ImageSource), typeof (ImageButton));

    static ImageButton()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (ImageButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (ImageButton)));
    }

    public ImageSource ImageHover
    {
      get
      {
        return (ImageSource) this.GetValue(ImageButton.ImageHoverProperty);
      }
      set
      {
        this.SetValue(ImageButton.ImageHoverProperty, (object) value);
      }
    }

    public ImageSource ImageNormal
    {
      get
      {
        return (ImageSource) this.GetValue(ImageButton.ImageNormalProperty);
      }
      set
      {
        this.SetValue(ImageButton.ImageNormalProperty, (object) value);
      }
    }

    public ImageSource ImagePressed
    {
      get
      {
        return (ImageSource) this.GetValue(ImageButton.ImagePressedProperty);
      }
      set
      {
        this.SetValue(ImageButton.ImagePressedProperty, (object) value);
      }
    }
  }
}
