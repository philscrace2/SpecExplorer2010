// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.DesignPropertyDescriptor
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Shell
{
  public class DesignPropertyDescriptor : PropertyDescriptor
  {
    private Hashtable editors = new Hashtable();
    private string displayName;
    private PropertyDescriptor property;
    private TypeConverter converter;

    public override string DisplayName
    {
      get
      {
        return this.displayName;
      }
    }

    public override Type ComponentType
    {
      get
      {
        return this.property.ComponentType;
      }
    }

    public override bool IsReadOnly
    {
      get
      {
        return this.property.IsReadOnly;
      }
    }

    public override Type PropertyType
    {
      get
      {
        return this.property.PropertyType;
      }
    }

    public override object GetEditor(Type editorBaseType)
    {
      object obj = this.editors[(object) editorBaseType];
      if (obj == null)
      {
        for (int index = 0; index < this.Attributes.Count; ++index)
        {
          EditorAttribute attribute = this.Attributes[index] as EditorAttribute;
          if (attribute != null)
          {
            Type type = Type.GetType(attribute.EditorBaseTypeName);
            if (editorBaseType == type)
            {
              Type fromNameProperty = this.GetTypeFromNameProperty(attribute.EditorTypeName);
              if (fromNameProperty != (Type) null)
              {
                obj = this.CreateInstance(fromNameProperty);
                this.editors[(object) fromNameProperty] = obj;
                break;
              }
            }
          }
        }
      }
      return obj;
    }

    public override TypeConverter Converter
    {
      get
      {
        if (this.converter == null)
        {
          PropertyPageTypeConverterAttribute attribute = (PropertyPageTypeConverterAttribute) this.Attributes[typeof (PropertyPageTypeConverterAttribute)];
          if (attribute != null && attribute.ConverterType != (Type) null)
            this.converter = (TypeConverter) this.CreateInstance(attribute.ConverterType);
          if (this.converter == null)
            this.converter = TypeDescriptor.GetConverter(this.PropertyType);
        }
        return this.converter;
      }
    }

    public virtual Type GetTypeFromNameProperty(string typeName)
    {
      return Type.GetType(typeName);
    }

    public override bool CanResetValue(object component)
    {
      return this.property.CanResetValue(component);
    }

    public override object GetValue(object component)
    {
      return this.property.GetValue(component);
    }

    public override void ResetValue(object component)
    {
      this.property.ResetValue(component);
    }

    public override void SetValue(object component, object value)
    {
      this.property.SetValue(component, value);
    }

    public override bool ShouldSerializeValue(object component)
    {
      return this.property.ShouldSerializeValue(component);
    }

    public DesignPropertyDescriptor(PropertyDescriptor prop)
      : base((MemberDescriptor) prop)
    {
      this.property = prop;
      Attribute attribute = prop.Attributes[typeof (DisplayNameAttribute)];
      if (attribute is DisplayNameAttribute)
        this.displayName = ((DisplayNameAttribute) attribute).DisplayName;
      else
        this.displayName = prop.Name;
    }
  }
}
