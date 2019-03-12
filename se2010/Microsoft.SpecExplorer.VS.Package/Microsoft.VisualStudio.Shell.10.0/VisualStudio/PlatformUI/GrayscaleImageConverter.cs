// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.GrayscaleImageConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.Internal.Performance;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class GrayscaleImageConverter : ValueConverter<BitmapSource, Image>
  {
    private const int BytesPerPixelBgra32 = 4;
    private const float BlueChannelWeight = 0.0004296875f;
    private const float GreenChannelWeight = 0.002304687f;
    private const float RedChannelWeight = 0.001171875f;

    protected override Image Convert(
      BitmapSource inputImage,
      object parameter,
      CultureInfo culture)
    {
      using (new CodeMarkerStartEnd(CodeMarkerEvent.perfShellUI_GrayscaleImageConvertValueBegin, CodeMarkerEvent.perfShellUI_GrayscaleImageConvertValueEnd))
      {
        if (inputImage == null)
          return (Image) null;
        Image image1 = new Image();
        Color biasColor = Colors.White;
        if (parameter is Color)
          biasColor = (Color) parameter;
        BitmapSource bitmapSource;
        if (inputImage.Format == PixelFormats.Bgra32 && inputImage.PixelWidth <= 128 && inputImage.PixelHeight <= 128)
        {
          bitmapSource = GrayscaleImageConverter.ConvertToGrayScale(inputImage, biasColor);
        }
        else
        {
          if (biasColor.R != byte.MaxValue || biasColor.G != byte.MaxValue || biasColor.B != byte.MaxValue)
            throw new NotSupportedException("Specifying non-white bias color is not supported for images with PixelFormat other than BGRA32, or larger than 128x128.");
          FormatConvertedBitmap formatConvertedBitmap = new FormatConvertedBitmap();
          formatConvertedBitmap.BeginInit();
          formatConvertedBitmap.DestinationFormat = PixelFormats.Gray32Float;
          formatConvertedBitmap.Source = inputImage;
          formatConvertedBitmap.EndInit();
          bitmapSource = (BitmapSource) formatConvertedBitmap;
          Image image2 = image1;
          ImageBrush imageBrush1 = new ImageBrush((ImageSource) inputImage);
          imageBrush1.Opacity = (double) biasColor.A / 256.0;
          ImageBrush imageBrush2 = imageBrush1;
          image2.OpacityMask = (Brush) imageBrush2;
        }
        image1.BeginInit();
        image1.Source = (ImageSource) bitmapSource;
        image1.EndInit();
        return image1;
      }
    }

    private static BitmapSource ConvertToGrayScale(
      BitmapSource inputImage,
      Color biasColor)
    {
      if (inputImage == null)
        throw new ArgumentNullException(nameof (inputImage));
      if (inputImage.Format != PixelFormats.Bgra32)
        throw new ArgumentException("Image is not the expected type", nameof (inputImage));
      int stride = inputImage.PixelWidth * 4;
      byte[] numArray = new byte[inputImage.PixelWidth * inputImage.PixelHeight * 4];
      inputImage.CopyPixels((Array) numArray, stride, 0);
      float num1 = (float) biasColor.A / 256f;
      for (int index = 0; index + 4 < numArray.Length; index += 4)
      {
        float num2 = (float) ((double) numArray[index] * 0.000429687497671694 + (double) numArray[index + 1] * 0.00230468739755452 + (double) numArray[index + 2] * 0.00117187504656613);
        numArray[index] = (byte) ((double) num2 * (double) biasColor.B);
        numArray[index + 1] = (byte) ((double) num2 * (double) biasColor.G);
        numArray[index + 2] = (byte) ((double) num2 * (double) biasColor.R);
        numArray[index + 3] = (byte) ((double) num1 * (double) numArray[index + 3]);
      }
      return BitmapSource.Create(inputImage.PixelWidth, inputImage.PixelHeight, inputImage.DpiX, inputImage.DpiY, PixelFormats.Bgra32, inputImage.Palette, (Array) numArray, stride);
    }
  }
}
