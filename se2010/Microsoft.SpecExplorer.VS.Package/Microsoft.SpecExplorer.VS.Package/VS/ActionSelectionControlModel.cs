using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Microsoft.ActionMachines.Cord;

namespace Microsoft.SpecExplorer.VS
{
	public class ActionSelectionControlModel : INotifyPropertyChanged
	{
		private const string SelectAllText = "(Select All)";

		private bool propertyChangedEventHandlerLocked;

		private ActionSelectionItem selectAllItem;

		public ObservableCollection<ActionSelectionItem> ActionSelectionItems { get; private set; }

		public IEnumerable<ConfigClause> SelectedActions
		{
			get
			{
				return from action in ActionSelectionItems
					where action != selectAllItem && action.IsSelected.HasValue && action.IsSelected.Value
					select action.ActionClause;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ActionSelectionControlModel()
		{
			ActionSelectionItems = new ObservableCollection<ActionSelectionItem>();
			ActionSelectionItems.CollectionChanged += ActionsCollectionChanged;
		}

		public void LoadActions(IEnumerable<ConfigClause> clauses)
		{
			ActionSelectionItems.Clear();
			selectAllItem = new ActionSelectionItem("(Select All)");
			ActionSelectionItems.Add(selectAllItem);
			foreach (ConfigClause clause in clauses)
			{
				if (clause is ConfigClause.ImportMethod || clause is ConfigClause.DeclareMethod || clause is ConfigClause.ImportAllFromScope)
				{
					ActionSelectionItems.Add(new ActionSelectionItem(clause));
				}
			}
		}

		private void SendNotification(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void ActionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs evtArgs)
		{
			if (evtArgs.NewItems == null)
			{
				return;
			}
			foreach (ActionSelectionItem newItem in evtArgs.NewItems)
			{
				if (evtArgs.Action == NotifyCollectionChangedAction.Add)
				{
					newItem.PropertyChanged += ActionSelectionChanged;
				}
				else if (evtArgs.Action == NotifyCollectionChangedAction.Remove)
				{
					newItem.PropertyChanged -= ActionSelectionChanged;
				}
			}
		}

		private void ActionSelectionChanged(object sender, PropertyChangedEventArgs evtArgs)
		{
			if (!(evtArgs.PropertyName == "IsSelected") || !(sender is ActionSelectionItem))
			{
				return;
			}
			if (selectAllItem != sender && !propertyChangedEventHandlerLocked)
			{
				propertyChangedEventHandlerLocked = true;
				if (ActionSelectionItems.Where((ActionSelectionItem item) => item != selectAllItem).All((ActionSelectionItem info) => info.IsSelected.HasValue && info.IsSelected.Value))
				{
					selectAllItem.IsSelected = true;
				}
				else if (ActionSelectionItems.Where((ActionSelectionItem item) => item != selectAllItem).All((ActionSelectionItem info) => info.IsSelected.HasValue && !info.IsSelected.Value))
				{
					selectAllItem.IsSelected = false;
				}
				else
				{
					selectAllItem.IsSelected = null;
				}
				propertyChangedEventHandlerLocked = false;
			}
			else
			{
				if (selectAllItem != sender || propertyChangedEventHandlerLocked)
				{
					return;
				}
				propertyChangedEventHandlerLocked = true;
				foreach (ActionSelectionItem actionSelectionItem in ActionSelectionItems)
				{
					if (actionSelectionItem != selectAllItem)
					{
						actionSelectionItem.IsSelected = selectAllItem.IsSelected;
					}
				}
				propertyChangedEventHandlerLocked = false;
			}
		}
	}
}
