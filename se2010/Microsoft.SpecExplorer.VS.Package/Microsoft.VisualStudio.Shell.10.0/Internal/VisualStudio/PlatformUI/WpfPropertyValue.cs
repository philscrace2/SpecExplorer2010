// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.WpfPropertyValue
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class WpfPropertyValue : PropertyValueBase
  {
    public WpfPropertyValue(object value)
      : base(value)
    {
    }

    private WpfPropertyValue(object value, string type)
      : base(value, type)
    {
    }

    public static IVsUIObject CreateEmptyValue(string type)
    {
      return (IVsUIObject) new WpfPropertyValue((object) null, type);
    }

    public static IVsUIObject CreateBitmapObject(ImageSource source)
    {
      return (IVsUIObject) new WpfPropertyValue((object) source, "VsUI.Bitmap");
    }

    public static IVsUIObject CreateIconObject(ImageSource source)
    {
      return (IVsUIObject) new WpfPropertyValue((object) source, "VsUI.Icon");
    }

    public override uint Format
    {
      get
      {
        return 3;
      }
    }

    protected override string TypeNameFromValue(object value)
    {
      if (value is Color)
        return "VsUI.Color";
      if (value is ImageSource)
        return "VsUI.Bitmap";
      if (value is IList<ImageSource>)
        return "VsUI.ImageList";
      return (string) null;
    }
  }
}
