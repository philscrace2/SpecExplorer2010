// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.LocalizableProperties
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell
{
  [ComVisible(true)]
  public class LocalizableProperties : ICustomTypeDescriptor
  {
    public AttributeCollection GetAttributes()
    {
      return TypeDescriptor.GetAttributes((object) this, true);
    }

    public EventDescriptor GetDefaultEvent()
    {
      return TypeDescriptor.GetDefaultEvent((object) this, true);
    }

    public PropertyDescriptor GetDefaultProperty()
    {
      return TypeDescriptor.GetDefaultProperty((object) this, true);
    }

    public object GetEditor(Type editorBaseType)
    {
      return TypeDescriptor.GetEditor((object) this, editorBaseType, true);
    }

    public EventDescriptorCollection GetEvents()
    {
      return TypeDescriptor.GetEvents((object) this, true);
    }

    public EventDescriptorCollection GetEvents(Attribute[] attributes)
    {
      return TypeDescriptor.GetEvents((object) this, attributes, true);
    }

    public object GetPropertyOwner(PropertyDescriptor pd)
    {
      return (object) this;
    }

    public PropertyDescriptorCollection GetProperties()
    {
      return this.GetProperties((Attribute[]) null);
    }

    public PropertyDescriptorCollection GetProperties(
      Attribute[] attributes)
    {
      ArrayList arrayList = new ArrayList();
      PropertyDescriptorCollection properties = TypeDescriptor.GetProperties((object) this, attributes, true);
      for (int index = 0; index < properties.Count; ++index)
        arrayList.Add((object) this.CreateDesignPropertyDescriptor(properties[index]));
      return new PropertyDescriptorCollection((PropertyDescriptor[]) arrayList.ToArray(typeof (PropertyDescriptor)));
    }

    public virtual DesignPropertyDescriptor CreateDesignPropertyDescriptor(
      PropertyDescriptor p)
    {
      return new DesignPropertyDescriptor(p);
    }

    public string GetComponentName()
    {
      return TypeDescriptor.GetComponentName((object) this, true);
    }

    public virtual TypeConverter GetConverter()
    {
      return TypeDescriptor.GetConverter((object) this, true);
    }

    public virtual string GetClassName()
    {
      return this.GetType().FullName;
    }
  }
}
