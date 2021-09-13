// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.MachinePropertyTypeDescriptor
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.ComponentModel;

namespace Microsoft.SpecExplorer
{
  public class MachinePropertyTypeDescriptor : CustomTypeDescriptor
  {
    private PropertyDescriptorCollection properties;
    private string componentName;

    public MachinePropertyTypeDescriptor(
      string componentName,
      PropertyDescriptorCollection properties)
    {
      if (componentName == null)
        throw new ArgumentNullException(nameof (componentName));
      if (properties == null)
        throw new ArgumentNullException(nameof (properties));
      this.componentName = componentName;
      this.properties = properties;
    }

    public override string GetClassName() => "Machine Properties";

    public override string GetComponentName() => this.componentName;

    public override object GetPropertyOwner(PropertyDescriptor pd) => (object) this;

    public override PropertyDescriptorCollection GetProperties() => this.properties;

    public override PropertyDescriptorCollection GetProperties(
      Attribute[] attributes)
    {
      return this.GetProperties();
    }
  }
}
