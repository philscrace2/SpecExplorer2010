﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.NotNullConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Globalization;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class NotNullConverter : ValueConverter<object, bool>
  {
    protected override bool Convert(object value, object parameter, CultureInfo culture)
    {
      return value != null;
    }
  }
}