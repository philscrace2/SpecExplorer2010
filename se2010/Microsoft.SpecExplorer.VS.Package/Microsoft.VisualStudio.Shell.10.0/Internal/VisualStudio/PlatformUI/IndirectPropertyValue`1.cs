// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.IndirectPropertyValue`1
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class IndirectPropertyValue<T> : IVsUIObject, IIndirectPropertyValue
  {
    private GetterThunk<T> getterThunk;
    private ParameterizedGetterThunk<T> parameterizedGetterThunk;
    private object parameter;
    private string type;
    private uint dataFormat;

    public IndirectPropertyValue(GetterThunk<T> getterThunk, string type, uint dataFormat)
      : this(type, dataFormat)
    {
      if (getterThunk == null)
        throw new ArgumentNullException(nameof (getterThunk));
      this.getterThunk = getterThunk;
    }

    public IndirectPropertyValue(
      ParameterizedGetterThunk<T> parameterizedGetterThunk,
      object parameter,
      string type,
      uint dataFormat)
      : this(type, dataFormat)
    {
      if (parameterizedGetterThunk == null)
        throw new ArgumentNullException(nameof (parameterizedGetterThunk));
      this.parameterizedGetterThunk = parameterizedGetterThunk;
      this.parameter = parameter;
    }

    private IndirectPropertyValue(string type, uint dataFormat)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      if (type.Length == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_EmptyString, (object) nameof (type)));
      this.type = type;
      this.dataFormat = dataFormat;
    }

    public override bool Equals(object obj)
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIObject pOtherObject = obj as IVsUIObject;
      if (pOtherObject == null)
        return base.Equals(obj);
      bool pfAreEqual = false;
      this.Equals(pOtherObject, out pfAreEqual);
      return pfAreEqual;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public int Equals(IVsUIObject pOtherObject, out bool pfAreEqual)
    {
      pfAreEqual = false;
      uint pdwDataFormat;
      int format;
      // ISSUE: reference to a compiler-generated method
      if ((format = pOtherObject.get_Format(out pdwDataFormat)) != 0)
        return format;
      if ((int) pdwDataFormat != (int) this.dataFormat)
        return 0;
      string pTypeName;
      int type;
      // ISSUE: reference to a compiler-generated method
      if ((type = pOtherObject.get_Type(out pTypeName)) != 0)
        return type;
      if (pTypeName == null || !pTypeName.Equals(this.type, StringComparison.Ordinal))
        return 0;
      object pVar;
      int data;
      // ISSUE: reference to a compiler-generated method
      if ((data = pOtherObject.get_Data(out pVar)) != 0)
        return data;
      T dataValue;
      try
      {
        dataValue = this.GetDataValue();
      }
      catch (Exception ex)
      {
        return Marshal.GetHRForException(ex);
      }
      pfAreEqual = pVar != null ? pVar.Equals((object) dataValue) : (object) dataValue == null;
      return 0;
    }

    public int get_Data(out object pVar)
    {
      try
      {
        pVar = (object) this.GetDataValue();
      }
      catch (Exception ex)
      {
        pVar = (object) null;
        return Marshal.GetHRForException(ex);
      }
      return 0;
    }

    public int get_Format(out uint pdwDataFormat)
    {
      pdwDataFormat = this.dataFormat;
      return 0;
    }

    public int get_Type(out string pTypeName)
    {
      pTypeName = this.type;
      return 0;
    }

    private T GetDataValue()
    {
      if (this.getterThunk != null)
        return this.getterThunk();
      return this.parameterizedGetterThunk(this.parameter);
    }
  }
}
