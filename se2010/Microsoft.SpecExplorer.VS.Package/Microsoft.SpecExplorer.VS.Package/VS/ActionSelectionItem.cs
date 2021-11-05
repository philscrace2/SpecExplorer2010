using System.ComponentModel;
using Microsoft.ActionMachines.Cord;

namespace Microsoft.SpecExplorer.VS
{
	public class ActionSelectionItem : ICordSyntaxElementInfo, INotifyPropertyChanged
	{
		private bool? isSelected;

		public ConfigClause ActionClause { get; private set; }

		public bool? IsSelected
		{
			get
			{
				return isSelected;
			}
			set
			{
				isSelected = value;
				SendNotification("IsSelected");
			}
		}

		public string DisplayText { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public ActionSelectionItem(ConfigClause actionClause)
		{
			ActionClause = actionClause;
			DisplayText = ActionClause.ToString();
			isSelected = false;
		}

		public ActionSelectionItem(string displayText)
		{
			ActionClause = null;
			DisplayText = displayText;
			isSelected = false;
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
