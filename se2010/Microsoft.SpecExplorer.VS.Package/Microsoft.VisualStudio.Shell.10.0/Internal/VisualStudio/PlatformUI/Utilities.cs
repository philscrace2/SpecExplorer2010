// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.Utilities
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public static class Utilities
  {
    public static T GetTypedValue<T>(IVsUIDataSource ds, string prop)
    {
      return (T) Utilities.GetValue(ds, prop);
    }

    public static object GetValue(IVsUIDataSource ds, string prop)
    {
      if (ds == null)
        throw new ArgumentNullException(nameof (ds));
      // ISSUE: variable of a compiler-generated type
      IVsUIObject ppValue;
      // ISSUE: reference to a compiler-generated method
      int errorCode = ds.GetValue(prop, out ppValue);
      if (errorCode != 0)
        throw new COMException(Resources.Error_CannotGetProperty, errorCode);
      if (ppValue != null)
        return Utilities.GetObjectData(ppValue);
      return (object) null;
    }

    public static object GetObjectData(IVsUIObject obj)
    {
      if (obj == null)
        throw new ArgumentNullException(nameof (obj));
      object pVar;
      // ISSUE: reference to a compiler-generated method
      int data = obj.get_Data(out pVar);
      if (data != 0)
        throw new COMException(Resources.Error_CannotGetPropertyData, data);
      return pVar;
    }

    public static T QueryTypedValue<T>(IVsUIDataSource ds, string prop)
    {
      return (T) Utilities.QueryValue(ds, prop);
    }

    public static object QueryValue(IVsUIDataSource ds, string prop)
    {
      if (ds == null)
        throw new ArgumentNullException(nameof (ds));
      object[] pValue = new object[1];
      // ISSUE: reference to a compiler-generated method
      int errorCode = ds.QueryValue(prop, (string[]) null, (uint[]) null, pValue);
      if (errorCode != 0)
        throw new COMException(Resources.Error_CannotGetProperty, errorCode);
      return pValue[0];
    }

    public static void SetValue(IVsUIDataSource ds, string prop, object value)
    {
      Utilities.SetValue(ds, prop, (IVsUIObject) new BuiltInPropertyValue(value));
    }

    public static void SetValue(IVsUIDataSource ds, string prop, IVsUIObject value)
    {
      if (ds == null)
        throw new ArgumentNullException(nameof (ds));
      // ISSUE: reference to a compiler-generated method
      int num = ds.SetValue(prop, value);
      if (!ErrorHandler.Succeeded(num))
        throw new COMException(Resources.Error_CannotSetProperty, num);
    }

    public static string GetObjectType(IVsUIObject obj)
    {
      if (obj == null)
        throw new ArgumentNullException(nameof (obj));
      string pTypeName;
      // ISSUE: reference to a compiler-generated method
      int type = obj.get_Type(out pTypeName);
      if (type != 0)
        throw new COMException(Resources.Error_CannotGetPropertyValueType, type);
      return pTypeName;
    }

    public static __VSUIDATAFORMAT GetObjectFormat(IVsUIObject obj)
    {
      if (obj == null)
        throw new ArgumentNullException(nameof (obj));
      uint pdwDataFormat;
      // ISSUE: reference to a compiler-generated method
      int format = obj.get_Format(out pdwDataFormat);
      if (format != 0)
        throw new COMException(Resources.Error_CannotGetPropertyValueFormat, format);
      return (__VSUIDATAFORMAT) pdwDataFormat;
    }
  }
}
