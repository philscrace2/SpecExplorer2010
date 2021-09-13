// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.MachinePropertyDescriptor
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.ComponentModel;

namespace Microsoft.SpecExplorer
{
  public class MachinePropertyDescriptor : PropertyDescriptor
  {
    private PropertyDescriptor property;
    private object value;

    public MachinePropertyDescriptor(PropertyDescriptor property, object value)
      : base((MemberDescriptor) property)
    {
      if (property == null)
        throw new ArgumentNullException(nameof (property));
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      this.property = property;
      this.value = value;
    }

    public override bool CanResetValue(object component) => false;

    public override Type ComponentType => typeof (MachinePropertyTypeDescriptor);

    public override object GetValue(object component) => this.Converter.ConvertFrom(this.value);

    public override bool IsReadOnly => true;

    public override Type PropertyType => this.property.PropertyType;

    public override void ResetValue(object component)
    {
    }

    public override TypeConverter Converter => this.property.Converter;

    public override void SetValue(object component, object value)
    {
    }

    public override bool ShouldSerializeValue(object component) => false;
  }
}
