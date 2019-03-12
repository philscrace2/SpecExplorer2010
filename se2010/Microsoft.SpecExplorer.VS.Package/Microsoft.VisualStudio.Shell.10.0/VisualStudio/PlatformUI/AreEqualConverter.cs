// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.AreEqualConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Globalization;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class AreEqualConverter : MultiValueConverter<object, object, bool>
  {
    protected override bool Convert(
      object value1,
      object value2,
      object parameter,
      CultureInfo culture)
    {
      if (object.Equals(value1, value2))
        return true;
      if (value1 != null)
        return value1.Equals(value2);
      return false;
    }
  }
}
