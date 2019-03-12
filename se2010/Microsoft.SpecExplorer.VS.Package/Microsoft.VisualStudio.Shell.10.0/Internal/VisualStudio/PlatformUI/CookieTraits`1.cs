// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.CookieTraits`1
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public abstract class CookieTraits<T> where T : IComparable<T>
  {
    private T _min;
    private T _max;
    private T _invalid;

    protected CookieTraits(T min, T max, T invalid)
    {
      if (min.CompareTo(max) >= 0)
        throw new ArgumentException(Resources.Error_CookieTable_InvalidRange);
      if (invalid.CompareTo(min) >= 0 && invalid.CompareTo(max) <= 0)
        throw new ArgumentException(Resources.Error_CookieTable_InvalidRange2);
      this._min = min;
      this._max = max;
      this._invalid = invalid;
    }

    public T InvalidCookie
    {
      get
      {
        return this._invalid;
      }
    }

    public T MinCookie
    {
      get
      {
        return this._min;
      }
    }

    public T MaxCookie
    {
      get
      {
        return this._max;
      }
    }

    public T GetNextCookie(T current)
    {
      if (current.CompareTo(this._max) < 0 && current.CompareTo(this._min) >= 0)
        return this.IncrementValue(current);
      return this._min;
    }

    public abstract T IncrementValue(T current);

    public abstract uint UniqueCookies { get; }
  }
}
