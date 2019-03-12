// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.UIObject
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public sealed class UIObject : IVsUIObject
  {
    private IVsUIObject innerObject;

    public UIObject(IVsUIObject innerObject)
    {
      if (innerObject == null)
        throw new ArgumentNullException(nameof (innerObject));
      this.innerObject = innerObject;
    }

    public string Type
    {
      get
      {
        return Utilities.GetObjectType(this.innerObject);
      }
    }

    public object Data
    {
      get
      {
        return Utilities.GetObjectData(this.innerObject);
      }
    }

    public __VSUIDATAFORMAT Format
    {
      get
      {
        return Utilities.GetObjectFormat(this.innerObject);
      }
    }

    public override bool Equals(object obj)
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIObject pOtherObject = obj as IVsUIObject;
      if (pOtherObject == null)
        return false;
      bool pfAreEqual = false;
      // ISSUE: reference to a compiler-generated method
      if (this.innerObject.Equals(pOtherObject, out pfAreEqual) != 0)
        return false;
      return pfAreEqual;
    }

    public override int GetHashCode()
    {
      return this.innerObject.GetHashCode();
    }

    public int Equals(IVsUIObject pOtherObject, out bool pfAreEqual)
    {
      // ISSUE: reference to a compiler-generated method
      return this.innerObject.Equals(pOtherObject, out pfAreEqual);
    }

    int IVsUIObject.get_Data(out object pVar)
    {
      // ISSUE: reference to a compiler-generated method
      return this.innerObject.get_Data(out pVar);
    }

    int IVsUIObject.get_Format(out uint pdwDataFormat)
    {
      // ISSUE: reference to a compiler-generated method
      return this.innerObject.get_Format(out pdwDataFormat);
    }

    int IVsUIObject.get_Type(out string pTypeName)
    {
      // ISSUE: reference to a compiler-generated method
      return this.innerObject.get_Type(out pTypeName);
    }
  }
}
