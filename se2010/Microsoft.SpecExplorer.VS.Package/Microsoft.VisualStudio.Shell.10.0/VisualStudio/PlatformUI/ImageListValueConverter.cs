// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.ImageListValueConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.PlatformUI.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class ImageListValueConverter : MultiValueConverter<int, IList<ImageSource>, ImageSource>
  {
    protected override ImageSource Convert(
      int imageIndex,
      IList<ImageSource> imageList,
      object parameter,
      CultureInfo culture)
    {
      if (imageIndex < 0)
        return (ImageSource) null;
      if (imageList == null)
        return (ImageSource) null;
      if (imageIndex >= imageList.Count)
        throw new ArgumentOutOfRangeException(nameof (imageIndex), Resources.Error_InvalidImageIndex);
      ImageSource image = imageList[imageIndex];
      if (image == null)
        throw new ArgumentException(Resources.Error_InvalidImagelist);
      return image;
    }
  }
}
