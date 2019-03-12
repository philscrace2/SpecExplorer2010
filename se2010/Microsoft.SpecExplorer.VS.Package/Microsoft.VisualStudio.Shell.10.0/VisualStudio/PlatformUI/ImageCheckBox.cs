// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.ImageCheckBox
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class ImageCheckBox : CheckBox
  {
    public static readonly DependencyProperty ImageNormalProperty = DependencyProperty.Register(nameof (ImageNormal), typeof (ImageSource), typeof (ImageCheckBox));
    public static readonly DependencyProperty ImageCheckedProperty = DependencyProperty.Register(nameof (ImageChecked), typeof (ImageSource), typeof (ImageCheckBox));
    public static readonly DependencyProperty ImageNormalHoverProperty = DependencyProperty.Register(nameof (ImageNormalHover), typeof (ImageSource), typeof (ImageCheckBox));
    public static readonly DependencyProperty ImageCheckedHoverProperty = DependencyProperty.Register(nameof (ImageCheckedHover), typeof (ImageSource), typeof (ImageCheckBox));
    public static readonly DependencyProperty ImagePressedProperty = DependencyProperty.Register(nameof (ImagePressed), typeof (ImageSource), typeof (ImageCheckBox));

    static ImageCheckBox()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (ImageCheckBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (ImageCheckBox)));
    }

    public ImageSource ImageChecked
    {
      get
      {
        return (ImageSource) this.GetValue(ImageCheckBox.ImageCheckedProperty);
      }
      set
      {
        this.SetValue(ImageCheckBox.ImageCheckedProperty, (object) value);
      }
    }

    public ImageSource ImageNormal
    {
      get
      {
        return (ImageSource) this.GetValue(ImageCheckBox.ImageNormalProperty);
      }
      set
      {
        this.SetValue(ImageCheckBox.ImageNormalProperty, (object) value);
      }
    }

    public ImageSource ImageNormalHover
    {
      get
      {
        return (ImageSource) this.GetValue(ImageCheckBox.ImageNormalHoverProperty);
      }
      set
      {
        this.SetValue(ImageCheckBox.ImageNormalHoverProperty, (object) value);
      }
    }

    public ImageSource ImageCheckedHover
    {
      get
      {
        return (ImageSource) this.GetValue(ImageCheckBox.ImageCheckedHoverProperty);
      }
      set
      {
        this.SetValue(ImageCheckBox.ImageCheckedHoverProperty, (object) value);
      }
    }

    public ImageSource ImagePressed
    {
      get
      {
        return (ImageSource) this.GetValue(ImageCheckBox.ImagePressedProperty);
      }
      set
      {
        this.SetValue(ImageCheckBox.ImagePressedProperty, (object) value);
      }
    }
  }
}
