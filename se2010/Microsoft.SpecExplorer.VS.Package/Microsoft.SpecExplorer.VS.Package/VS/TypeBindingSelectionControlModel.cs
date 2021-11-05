using System.ComponentModel;
using System.Linq;
using EnvDTE;

namespace Microsoft.SpecExplorer.VS
{
	public class TypeBindingSelectionControlModel : INotifyPropertyChanged
	{
		private bool existingClassSelected = true;

		public bool ExistingClassSelected
		{
			get
			{
				return existingClassSelected;
			}
			set
			{
				if (existingClassSelected != value)
				{
					existingClassSelected = value;
					SendNotification("ExistingClassSelected");
				}
			}
		}

		public CodeClass SelectedClass
		{
			get
			{
				if (ExistingClassSelected)
				{
					CodeElementAndContainerPair codeElementAndContainerPair = ViewerModel.RetrieveSelectedItems((vsCMElement)1).SingleOrDefault();
					if (codeElementAndContainerPair == null)
					{
						return null;
					}
					CodeElement element = codeElementAndContainerPair.Element;
					return (CodeClass)(object)((element is CodeClass) ? element : null);
				}
				return null;
			}
		}

		public CodeElementViewerModel ViewerModel { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		private void SendNotification(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
