// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.ShapeIdentifier
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public struct ShapeIdentifier : IComparable, IComparable<ShapeIdentifier>, IEquatable<ShapeIdentifier>
  {
    private Guid uuid;
    private uint id;

    public ShapeIdentifier(Guid value, uint id)
    {
      this.uuid = value;
      this.id = id;
    }

    public ShapeIdentifier(IVsUIDataSource dataSource)
    {
      if (dataSource == null)
        throw new ArgumentNullException(nameof (dataSource));
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(dataSource.GetShapeIdentifier(out this.uuid, out this.id));
    }

    public override int GetHashCode()
    {
      return this.id.GetHashCode() ^ this.uuid.GetHashCode();
    }

    public bool Equals(ShapeIdentifier other)
    {
      if (this.uuid == other.uuid)
        return (int) this.id == (int) other.id;
      return false;
    }

    public override bool Equals(object obj)
    {
      if (obj == null || !(obj is ShapeIdentifier))
        return false;
      return this.Equals((ShapeIdentifier) obj);
    }

    public int CompareTo(object obj)
    {
      if (obj == null)
        return 1;
      if (!(obj is ShapeIdentifier))
        throw new ArgumentException("Compared object must be a ShapeIdentifier", nameof (obj));
      return this.CompareTo((ShapeIdentifier) obj);
    }

    public int CompareTo(ShapeIdentifier other)
    {
      int num = this.uuid.CompareTo(other.uuid);
      if (num == 0)
        num = (int) this.id - (int) other.id;
      return num;
    }

    bool IEquatable<ShapeIdentifier>.Equals(ShapeIdentifier other)
    {
      if (this.uuid == other.uuid)
        return (int) this.id == (int) other.id;
      return false;
    }

    public static bool operator ==(ShapeIdentifier first, ShapeIdentifier second)
    {
      return first.Equals(second);
    }

    public static bool operator !=(ShapeIdentifier first, ShapeIdentifier second)
    {
      return !first.Equals(second);
    }

    public static bool operator <(ShapeIdentifier first, ShapeIdentifier second)
    {
      return first.CompareTo(second) < 0;
    }

    public static bool operator >(ShapeIdentifier first, ShapeIdentifier second)
    {
      return first.CompareTo(second) > 0;
    }
  }
}
