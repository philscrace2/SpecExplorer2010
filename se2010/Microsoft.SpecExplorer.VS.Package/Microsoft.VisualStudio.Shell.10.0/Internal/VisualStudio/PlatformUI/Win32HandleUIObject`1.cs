// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.Win32HandleUIObject`1
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class Win32HandleUIObject<TData> : IVsUIObject
  {
    private readonly TData _data;
    private readonly string _gelTypeName;

    protected Win32HandleUIObject(TData data, string gelTypeName)
    {
      if ((object) data == null)
        throw new ArgumentNullException(nameof (data));
      if (gelTypeName == null)
        throw new ArgumentNullException(nameof (gelTypeName));
      if (gelTypeName == string.Empty)
        throw new ArgumentException(Resources.Error_GelTypeCannotBeEmpty);
      this._data = data;
      this._gelTypeName = gelTypeName;
    }

    public int Equals(IVsUIObject pOtherObject, out bool equal)
    {
      equal = pOtherObject != null && object.ReferenceEquals((object) pOtherObject, (object) this);
      return 0;
    }

    public int get_Data(out object pVar)
    {
      pVar = (object) this._data;
      return 0;
    }

    public int get_Format(out uint pdwDataFormat)
    {
      pdwDataFormat = 1U;
      return 0;
    }

    public int get_Type(out string pTypeName)
    {
      pTypeName = this._gelTypeName;
      return 0;
    }
  }
}
