using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.SpecExplorer.Properties;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	public class GuidanceControlModel : INotifyPropertyChanged
	{
		private const string selectGuidanceEntry = "<Select Guidance ...>";

		private IGuidance selectedGuidance;

		private DelegateCommand assistedProcedureCommand;

		private DelegateCommand copyCodeCommand;

		private ObservableCollection<IGuidance> guidanceList;

		public ObservableCollection<IGuidance> GuidanceList
		{
			get
			{
				return guidanceList;
			}
		}

		public IGuidance SelectedGuidance
		{
			get
			{
				return selectedGuidance;
			}
			set
			{
				selectedGuidance = value;
				SendNotification("SelectedGuidance");
			}
		}

		public ICommand AssistedProcedureCommand
		{
			get
			{
				return assistedProcedureCommand;
			}
		}

		public ICommand CopyCodeCommand
		{
			get
			{
				return copyCodeCommand;
			}
		}

		public event EventHandler<AssistedProcedureRequestEventArgs> AssistedProcedureRequested;

		public event PropertyChangedEventHandler PropertyChanged;

		public GuidanceControlModel()
		{
			guidanceList = new ObservableCollection<IGuidance>();
			guidanceList.CollectionChanged += GuidanceListChanged;
			selectedGuidance = new GuidanceImpl
			{
				Description = "<Select Guidance ...>",
				Id = string.Empty,
				StructureField = new ActivityReferenceImpl[0]
			};
			guidanceList.Add(selectedGuidance);
			PropertyChanged += SelfPropertyChanged;
			assistedProcedureCommand = new DelegateCommand(InvokeAssistedProcedureEvent);
			copyCodeCommand = new DelegateCommand(delegate(object codeText)
			{
				Clipboard.SetText(codeText.ToString());
			});
		}

		private void SendNotification(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void InvokeAssistedProcedureEvent(object procId)
		{
			if (this.AssistedProcedureRequested != null)
			{
				try
				{
					this.AssistedProcedureRequested(this, new AssistedProcedureRequestEventArgs(Convert.ToUInt32(procId.ToString().Trim(), 16)));
				}
				catch (FormatException)
				{
					MessageBox.Show("Failed Invoking assisted procedure: Invalid format for Assisted Procedure Id", Resources.SpecExplorer);
				}
			}
		}

		private void SelfPropertyChanged(object sender, PropertyChangedEventArgs evtArgs)
		{
			if (evtArgs.PropertyName == "SelectedGuidance")
			{
				IActivityReference[] structure = (sender as GuidanceControlModel).SelectedGuidance.Structure;
				IActivityReference activityReference = structure.FirstOrDefault((IActivityReference activityRef) => !activityRef.IsCompleted);
				activityReference = ((activityReference == null) ? structure.LastOrDefault() : activityReference);
				if (activityReference != null)
				{
					activityReference.IsSelected = true;
				}
			}
		}

		private void GuidanceListChanged(object sender, NotifyCollectionChangedEventArgs evtArgs)
		{
			foreach (IGuidance newItem in evtArgs.NewItems)
			{
				IActivityReference[] structure = newItem.Structure;
				foreach (IActivityReference activityReference in structure)
				{
					if (evtArgs.Action == NotifyCollectionChangedAction.Add)
					{
						activityReference.PropertyChanged += ActivityPropertyChanged;
					}
					else if (evtArgs.Action == NotifyCollectionChangedAction.Remove)
					{
						activityReference.PropertyChanged -= ActivityPropertyChanged;
					}
				}
			}
		}

		private void ActivityPropertyChanged(object sender, PropertyChangedEventArgs evtArgs)
		{
			IActivityReference actRef = sender as IActivityReference;
			if (!SelectedGuidance.Structure.Contains(actRef))
			{
				return;
			}
			if (evtArgs.PropertyName == "IsCompleted" && actRef.IsCompleted)
			{
				for (int i = 0; i < SelectedGuidance.Structure.Length; i++)
				{
					if (actRef.Activity.Id == SelectedGuidance.Structure[i].Activity.Id && i + 1 < SelectedGuidance.Structure.Length)
					{
						SelectedGuidance.Structure[i + 1].IsSelected = true;
						break;
					}
				}
			}
			else
			{
				if (!(evtArgs.PropertyName == "IsSelected") || !actRef.IsSelected)
				{
					return;
				}
				Array.ForEach(SelectedGuidance.Structure, delegate(IActivityReference act)
				{
					if (actRef != act)
					{
						act.IsSelected = false;
					}
				});
			}
		}
	}
}
