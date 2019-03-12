// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.LogicalViewConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  internal class LogicalViewConverter : EnumConverter
  {
    private Guid[] _guids = new Guid[8]
    {
      new Guid("00000000-0000-0000-0000-000000000000"),
      new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"),
      new Guid("7651A700-06E5-11D1-8EBD-00A0C90F26EA"),
      new Guid("7651A701-06E5-11D1-8EBD-00A0C90F26EA"),
      new Guid("7651A702-06E5-11D1-8EBD-00A0C90F26EA"),
      new Guid("7651A703-06E5-11D1-8EBD-00A0C90F26EA"),
      new Guid("7651A704-06E5-11D1-8EBD-00A0C90F26EA"),
      new Guid("80A3471A-6B87-433E-A75A-9D461DE0645F")
    };
    private LogicalView[] _views = new LogicalView[8]
    {
      LogicalView.Primary,
      LogicalView.Any,
      LogicalView.Debugging,
      LogicalView.Code,
      LogicalView.Designer,
      LogicalView.Text,
      LogicalView.UserChoose,
      LogicalView.ProjectSpecific
    };

    public LogicalViewConverter(Type enumType)
      : base(enumType)
    {
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof (Guid))
        return true;
      return base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof (Guid))
        return true;
      return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      if (value is Guid)
      {
        for (int index = 0; index < this._guids.Length; ++index)
        {
          if (value.Equals((object) this._guids[index]))
            return (object) this._views[index];
        }
      }
      return base.ConvertFrom(context, culture, value);
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      if (destinationType == (Type) null)
        throw new ArgumentNullException(nameof (destinationType));
      if (destinationType == typeof (Guid) && value != null)
      {
        for (int index = 0; index < this._views.Length; ++index)
        {
          if (value.Equals((object) this._views[index]))
            return (object) this._guids[index];
        }
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}
