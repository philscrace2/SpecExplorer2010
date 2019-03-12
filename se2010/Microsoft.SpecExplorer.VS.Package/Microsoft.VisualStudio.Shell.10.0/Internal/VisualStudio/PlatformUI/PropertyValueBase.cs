// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.PropertyValueBase
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public abstract class PropertyValueBase : IVsUIObject
  {
    private string _typeName;

    public object Value { get; private set; }

    public string TypeName
    {
      get
      {
        return this._typeName ?? (this._typeName = this.DeduceTypeName());
      }
    }

    public abstract uint Format { get; }

    protected abstract string TypeNameFromValue(object value);

    protected PropertyValueBase(object value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.Value = value;
    }

    protected PropertyValueBase(object value, string type)
    {
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      this.Value = value;
      this._typeName = type;
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
      if ((int) pdwDataFormat != (int) this.Format)
        return 0;
      string pTypeName;
      int type;
      // ISSUE: reference to a compiler-generated method
      if ((type = pOtherObject.get_Type(out pTypeName)) != 0)
        return type;
      if (pTypeName == null || !pTypeName.Equals(this.TypeName, StringComparison.Ordinal))
        return 0;
      object pVar;
      int data;
      // ISSUE: reference to a compiler-generated method
      if ((data = pOtherObject.get_Data(out pVar)) != 0)
        return data;
      pfAreEqual = pVar != null ? pVar.Equals(this.Value) : this.Value == null;
      return 0;
    }

    public int get_Data(out object pVar)
    {
      pVar = this.Value;
      return 0;
    }

    public int get_Format(out uint pdwDataFormat)
    {
      pdwDataFormat = this.Format;
      return 0;
    }

    public int get_Type(out string pTypeName)
    {
      pTypeName = this.TypeName;
      return 0;
    }

    private string DeduceTypeName()
    {
      string str = this.TypeNameFromValue(this.Value);
      if (string.IsNullOrEmpty(str))
        throw new ArgumentException(Resources.Error_UnrecognizedBuiltInValueType);
      return str;
    }
  }
}
