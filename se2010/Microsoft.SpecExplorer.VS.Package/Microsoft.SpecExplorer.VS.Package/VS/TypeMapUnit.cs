using System.ComponentModel;
using EnvDTE;

namespace Microsoft.SpecExplorer.VS
{
	public class TypeMapUnit : INotifyPropertyChanged
	{
		private CodeClass modelClass;

		public ProcedureType ImplementationType { get; private set; }

		public CodeClass ModelClass
		{
			get
			{
				return modelClass;
			}
			set
			{
				modelClass = value;
				SendNotification("ModelClassText");
			}
		}

		public string ImplementationTypeText
		{
			get
			{
				return ImplementationType.ShortName;
			}
		}

		public string ModelClassText
		{
			get
			{
				if (ModelClass != null)
				{
					return ModelClass.FullName;
				}
				return "<Auto Generated Class>";
			}
		}

		public TypeMapUnit SelfInstance
		{
			get
			{
				return this;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public TypeMapUnit(ProcedureType implementationType)
		{
			ImplementationType = implementationType;
		}

		private void SendNotification(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
