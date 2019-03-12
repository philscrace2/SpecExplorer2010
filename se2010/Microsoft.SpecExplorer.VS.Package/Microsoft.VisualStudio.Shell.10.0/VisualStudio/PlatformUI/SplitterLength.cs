// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterLength
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualStudio.PlatformUI
{
  [TypeConverter(typeof (SplitterLengthConverter))]
  public struct SplitterLength : IEquatable<SplitterLength>
  {
    private double unitValue;
    private SplitterUnitType unitType;

    public SplitterLength(double value)
    {
      this = new SplitterLength(value, SplitterUnitType.Stretch);
    }

    public SplitterLength(double value, SplitterUnitType unitType)
    {
      this.unitValue = value;
      this.unitType = unitType;
    }

    public SplitterUnitType SplitterUnitType
    {
      get
      {
        return this.unitType;
      }
    }

    public double Value
    {
      get
      {
        return this.unitValue;
      }
    }

    public bool IsFill
    {
      get
      {
        return this.SplitterUnitType == SplitterUnitType.Fill;
      }
    }

    public bool IsStretch
    {
      get
      {
        return this.SplitterUnitType == SplitterUnitType.Stretch;
      }
    }

    public static bool operator ==(SplitterLength obj1, SplitterLength obj2)
    {
      if (obj1.SplitterUnitType == obj2.SplitterUnitType)
        return obj1.Value == obj2.Value;
      return false;
    }

    public static bool operator !=(SplitterLength obj1, SplitterLength obj2)
    {
      return !(obj1 == obj2);
    }

    public override bool Equals(object obj)
    {
      if (obj is SplitterLength)
        return this == (SplitterLength) obj;
      return false;
    }

    public override int GetHashCode()
    {
      return (int) ((int) this.unitValue + this.unitType);
    }

    public bool Equals(SplitterLength other)
    {
      return this == other;
    }

    public override string ToString()
    {
      return SplitterLengthConverter.ToString(this, CultureInfo.InvariantCulture);
    }
  }
}
