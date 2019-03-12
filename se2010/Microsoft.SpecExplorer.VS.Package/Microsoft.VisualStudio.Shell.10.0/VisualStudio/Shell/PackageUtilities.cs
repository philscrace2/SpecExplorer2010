// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.PackageUtilities
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Microsoft.VisualStudio.Shell
{
  public static class PackageUtilities
  {
    public static string GetSystemAssemblyPath()
    {
      return Path.GetDirectoryName(typeof (object).Assembly.Location);
    }

    public static void EnsureOutputPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return;
      if (Directory.Exists(path))
        return;
      try
      {
        Directory.CreateDirectory(path);
      }
      catch (IOException ex)
      {
        Trace.WriteLine("Exception : " + ex.Message);
      }
      catch (UnauthorizedAccessException ex)
      {
        Trace.WriteLine("Exception : " + ex.Message);
      }
      catch (ArgumentException ex)
      {
        Trace.WriteLine("Exception : " + ex.Message);
      }
      catch (NotSupportedException ex)
      {
        Trace.WriteLine("Exception : " + ex.Message);
      }
    }

    public static bool ContainsInvalidFileNameChars(string name)
    {
      if (string.IsNullOrEmpty(name))
        return true;
      if (Path.IsPathRooted(name))
      {
        string pathRoot = Path.GetPathRoot(name);
        name = name.Substring(pathRoot.Length);
      }
      string[] segments = new Url(name).Segments;
      if (segments == null)
        return PackageUtilities.IsFilePartInValid(name);
      foreach (string filePart in segments)
      {
        if (PackageUtilities.IsFilePartInValid(filePart))
          return true;
      }
      return false;
    }

    public static bool IsFileNameInvalid(string fileName)
    {
      if (string.IsNullOrEmpty(fileName) || PackageUtilities.IsFileNameAllGivenCharacter('.', fileName) || PackageUtilities.IsFileNameAllGivenCharacter(' ', fileName))
        return true;
      return PackageUtilities.IsFilePartInValid(fileName);
    }

    public static void CopyUrlToLocal(Uri uri, string local)
    {
      if (uri.IsFile)
      {
        new FileInfo(uri.LocalPath).CopyTo(local, true);
      }
      else
      {
        FileStream fileStream = new FileStream(local, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        try
        {
          WebRequest webRequest = WebRequest.Create(uri);
          webRequest.Timeout = 10000;
          webRequest.Credentials = CredentialCache.DefaultCredentials;
          Stream responseStream = webRequest.GetResponse().GetResponseStream();
          byte[] buffer = new byte[10240];
          int count;
          while ((count = responseStream.Read(buffer, 0, buffer.Length)) != 0)
            fileStream.Write(buffer, 0, count);
        }
        finally
        {
          fileStream.Close();
        }
      }
    }

    public static string MakeRelativeIfRooted(string fileName, Url url)
    {
      string path = fileName;
      if (Path.IsPathRooted(path))
      {
        string absoluteUrl1 = new Url(path).AbsoluteUrl;
        string absoluteUrl2 = url.AbsoluteUrl;
        if (absoluteUrl1.StartsWith(absoluteUrl2, StringComparison.OrdinalIgnoreCase))
          path = absoluteUrl1.Substring(absoluteUrl2.Length);
      }
      return path;
    }

    public static string GetPathDistance(Uri uriBase, Uri uriRelativeTo)
    {
      string str = string.Empty;
      if (uriRelativeTo != (Uri) null && uriBase != (Uri) null)
      {
        if (uriRelativeTo.Segments.Length > 0 && uriBase.Segments.Length > 0 && string.Compare(uriRelativeTo.Segments[1], uriBase.Segments[1], StringComparison.OrdinalIgnoreCase) == 0)
        {
          Uri uri = uriBase.MakeRelativeUri(uriRelativeTo);
          if (uri != (Uri) null)
            str = Url.Unescape(uri.ToString(), true);
        }
        else
          str = uriRelativeTo.LocalPath;
      }
      return str;
    }

    public static string MakeRelative(string filename, string filename2)
    {
      string[] strArray1 = filename.Split(Path.DirectorySeparatorChar);
      string[] strArray2 = filename2.Split(Path.DirectorySeparatorChar);
      if (strArray1.Length == 0 || strArray2.Length == 0 || strArray1[0] != strArray2[0])
        return filename2;
      int index1 = 1;
      while (index1 < strArray1.Length && index1 < strArray2.Length && !(strArray1[index1] != strArray2[index1]))
        ++index1;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index2 = index1; index2 < strArray1.Length - 1; ++index2)
      {
        stringBuilder.Append("..");
        stringBuilder.Append(Path.DirectorySeparatorChar);
      }
      for (int index2 = index1; index2 < strArray2.Length; ++index2)
      {
        stringBuilder.Append(strArray2[index2]);
        if (index2 < strArray2.Length - 1)
          stringBuilder.Append(Path.DirectorySeparatorChar);
      }
      return stringBuilder.ToString();
    }

    [CLSCompliant(false)]
    public static CAUUID CreateCAUUIDFromGuidArray(Guid[] guids)
    {
      CAUUID cauuid = new CAUUID();
      if (guids != null)
      {
        cauuid.cElems = (uint) guids.Length;
        int num = Marshal.SizeOf(typeof (Guid));
        cauuid.pElems = Marshal.AllocCoTaskMem(guids.Length * num);
        IntPtr ptr = cauuid.pElems;
        for (int index = 0; index < guids.Length; ++index)
        {
          Marshal.StructureToPtr((object) guids[index], ptr, false);
          ptr = new IntPtr(ptr.ToInt64() + (long) num);
        }
      }
      return cauuid;
    }

    public static int GetIntPointerFromImage(Image image)
    {
      Bitmap bitmap = image as Bitmap;
      if (bitmap != null)
        return bitmap.GetHicon().ToInt32();
      return 0;
    }

    public static ImageList GetImageList(Assembly assembly, string[] resourceNames)
    {
      if (resourceNames == null || resourceNames.Length == 0 || assembly == (Assembly) null)
        return (ImageList) null;
      ImageList imageList = new ImageList();
      imageList.ImageSize = new Size(16, 16);
      foreach (string resourceName in resourceNames)
      {
        Stream manifestResourceStream = assembly.GetManifestResourceStream(resourceName);
        if (manifestResourceStream != null)
        {
          Icon icon = new Icon(manifestResourceStream);
          imageList.Images.Add((Image) icon.ToBitmap());
        }
      }
      return imageList;
    }

    public static ImageList GetImageList(Stream imageStream)
    {
      ImageList imageList = new ImageList();
      if (imageStream == null)
        return imageList;
      imageList.ImageSize = new Size(16, 16);
      Bitmap bitmap = new Bitmap(imageStream);
      imageList.Images.AddStrip((Image) bitmap);
      imageList.TransparentColor = Color.Magenta;
      return imageList;
    }

    public static ImageList GetImageList(object imageListAsPointer)
    {
      ImageList imageList = (ImageList) null;
      HandleRef himl = new HandleRef((object) null, new IntPtr((int) imageListAsPointer));
      int imageCount = Microsoft.VisualStudio.UnsafeNativeMethods.ImageList_GetImageCount(himl);
      if (imageCount > 0)
      {
        Bitmap bitmap = new Bitmap(16 * imageCount, 16);
        Graphics graphics = Graphics.FromImage((Image) bitmap);
        IntPtr num = IntPtr.Zero;
        try
        {
          num = graphics.GetHdc();
          HandleRef hdcDst = new HandleRef((object) null, num);
          for (int i = 0; i < imageCount; ++i)
            Microsoft.VisualStudio.UnsafeNativeMethods.ImageList_Draw(himl, i, hdcDst, i * 16, 0, 0);
        }
        finally
        {
          if (graphics != null && num != IntPtr.Zero)
            graphics.ReleaseHdc(num);
        }
        imageList = new ImageList();
        imageList.ImageSize = new Size(16, 16);
        imageList.Images.AddStrip((Image) bitmap);
      }
      return imageList;
    }

    [CLSCompliant(false)]
    public static object ConvertToType<T>(T value, System.Type typeToConvert, CultureInfo culture) where T : struct
    {
      EnumConverter enumConverter = PackageUtilities.GetEnumConverter<T>();
      if (enumConverter == null)
        return (object) null;
      if (enumConverter.CanConvertTo(typeToConvert))
        return enumConverter.ConvertTo((ITypeDescriptorContext) null, culture, (object) value, typeToConvert);
      return (object) null;
    }

    [CLSCompliant(false)]
    public static T? ConvertFromType<T>(string value, CultureInfo culture) where T : struct
    {
      T? nullable = new T?();
      nullable = new T?(nullable.GetValueOrDefault());
      if (value == null)
        return nullable;
      EnumConverter enumConverter = PackageUtilities.GetEnumConverter<T>();
      if (enumConverter == null || !enumConverter.CanConvertFrom(value.GetType()))
        return nullable;
      object obj = enumConverter.ConvertFrom((ITypeDescriptorContext) null, culture, (object) value);
      if (obj != null && obj is T)
        nullable = new T?((T) obj);
      return nullable;
    }

    [CLSCompliant(false)]
    public static string SetStringValueFromConvertedEnum<T>(T enumValue, CultureInfo culture) where T : struct
    {
      object type = PackageUtilities.ConvertToType<T>(enumValue, typeof (string), culture);
      if (type == null || !(type is string))
        return string.Empty;
      return (string) type;
    }

    private static EnumConverter GetEnumConverter<T>() where T : struct
    {
      object[] customAttributes = typeof (T).GetCustomAttributes(typeof (PropertyPageTypeConverterAttribute), true);
      if (customAttributes != null && customAttributes.Length == 1)
      {
        PropertyPageTypeConverterAttribute converterAttribute = (PropertyPageTypeConverterAttribute) customAttributes[0];
        if (converterAttribute.ConverterType.IsSubclassOf(typeof (EnumConverter)))
          return Activator.CreateInstance(converterAttribute.ConverterType) as EnumConverter;
      }
      return (EnumConverter) null;
    }

    private static bool IsFilePartInValid(string filePart)
    {
      if (string.IsNullOrEmpty(filePart))
        return true;
      return new Regex("[/?:&\\\\*<>|#%" + (object) '"' + "]", RegexOptions.Compiled).IsMatch(filePart);
    }

    private static bool IsFileNameAllGivenCharacter(char c, string fileName)
    {
      int index = 0;
      while (index < fileName.Length && (int) fileName[index] == (int) c)
        ++index;
      return index >= fileName.Length;
    }
  }
}
