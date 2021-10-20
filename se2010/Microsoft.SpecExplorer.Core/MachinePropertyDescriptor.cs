using System;
using System.ComponentModel;

namespace Microsoft.SpecExplorer
{
	public class MachinePropertyDescriptor : PropertyDescriptor
	{
		private PropertyDescriptor property;

		private object value;

		public override Type ComponentType
		{
			get
			{
				return typeof(MachinePropertyTypeDescriptor);
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return property.PropertyType;
			}
		}

		public override TypeConverter Converter
		{
			get
			{
				return property.Converter;
			}
		}

		public MachinePropertyDescriptor(PropertyDescriptor property, object value)
			: base(property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.property = property;
			this.value = value;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override object GetValue(object component)
		{
			return Converter.ConvertFrom(value);
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
