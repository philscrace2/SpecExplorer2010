using System;
using System.ComponentModel;

namespace Microsoft.SpecExplorer
{
	public class MachinePropertyTypeDescriptor : CustomTypeDescriptor
	{
		private PropertyDescriptorCollection properties;

		private string componentName;

		public MachinePropertyTypeDescriptor(string componentName, PropertyDescriptorCollection properties)
		{
			if (componentName == null)
			{
				throw new ArgumentNullException("componentName");
			}
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			this.componentName = componentName;
			this.properties = properties;
		}

		public override string GetClassName()
		{
			return "Machine Properties";
		}

		public override string GetComponentName()
		{
			return componentName;
		}

		public override object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public override PropertyDescriptorCollection GetProperties()
		{
			return properties;
		}

		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return GetProperties();
		}
	}
}
