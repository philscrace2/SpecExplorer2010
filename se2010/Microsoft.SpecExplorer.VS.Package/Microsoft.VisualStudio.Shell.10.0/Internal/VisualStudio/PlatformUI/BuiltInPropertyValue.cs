// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.BuiltInPropertyValue
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public sealed class BuiltInPropertyValue : PropertyValueBase
  {
    public BuiltInPropertyValue(object value)
      : base(value)
    {
    }

    private BuiltInPropertyValue(string type)
      : base((object) null, type)
    {
    }

    private BuiltInPropertyValue(object value, string type)
      : base(value, type)
    {
    }

    public static IVsUIObject CreateEmptyValue(string type)
    {
      return (IVsUIObject) new BuiltInPropertyValue(type);
    }

    public static IVsUIObject CreateUnknownValue(object value)
    {
      return (IVsUIObject) new BuiltInPropertyValue(value, "VsUI.Unknown");
    }

    public static IVsUIObject CreateDispatchValue(object value)
    {
      return (IVsUIObject) new BuiltInPropertyValue(value, "VsUI.Dispatch");
    }

    public override uint Format
    {
      get
      {
        return 0;
      }
    }

    protected override string TypeNameFromValue(object value)
    {
      Type type = value.GetType();
      if (value is IVsUIDataSource)
        type = typeof (IVsUIDataSource);
      else if (value is IVsUICollection)
        type = typeof (IVsUICollection);
      else if (value is UnknownWrapper)
        type = typeof (UnknownWrapper);
      else if (value is DispatchWrapper)
        type = typeof (DispatchWrapper);
      return PropertyDescription.VsUITypeFromType(type);
    }
  }
}
