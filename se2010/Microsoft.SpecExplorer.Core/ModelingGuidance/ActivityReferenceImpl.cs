using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	[Serializable]
	public class ActivityReferenceImpl : IActivityReference, INotifyPropertyChanged
	{
		private bool isSelected;

		private bool isCompleted;

		[XmlAttribute("Ref")]
		public string RefId { get; set; }

		[XmlIgnore]
		public IActivity Activity { get; set; }

		[XmlIgnore]
		public bool IsCompleted
		{
			get
			{
				return isCompleted;
			}
			set
			{
				isCompleted = value;
				SendNotification("IsCompleted");
			}
		}

		[XmlIgnore]
		public bool IsSelected
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

		[XmlIgnore]
		public int Index { get; set; }

		[XmlAttribute("Optional")]
		public bool IsOptional { get; set; }

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
