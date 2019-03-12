// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.MultiValueHelper
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.PlatformUI.Common;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.PlatformUI
{
  internal static class MultiValueHelper
  {
    public static void CheckValue<T>(object[] values, int index)
    {
      if (!(values[index] is T) && (values[index] != null || typeof (T).IsValueType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_ValueAtOffsetNotOfType, (object) index, (object) typeof (T).FullName));
    }

    public static void CheckType<T>(Type[] types, int index)
    {
      if (!types[index].IsAssignableFrom(typeof (T)))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_TargetAtOffsetNotExtendingType, (object) index, (object) typeof (T).FullName));
    }
  }
}
