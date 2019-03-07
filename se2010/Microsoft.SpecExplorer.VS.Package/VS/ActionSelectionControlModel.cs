// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ActionSelectionControlModel
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.ActionMachines.Cord;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.SpecExplorer.VS
{
  public class ActionSelectionControlModel : INotifyPropertyChanged
  {
    private const string SelectAllText = "(Select All)";
    private bool propertyChangedEventHandlerLocked;
    private ActionSelectionItem selectAllItem;

    public ActionSelectionControlModel()
    {
      this.ActionSelectionItems = new ObservableCollection<ActionSelectionItem>();
      this.ActionSelectionItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ActionsCollectionChanged);
    }

    public ObservableCollection<ActionSelectionItem> ActionSelectionItems { get; private set; }

    public IEnumerable<ConfigClause> SelectedActions
    {
      get
      {
        return this.ActionSelectionItems.Where<ActionSelectionItem>((Func<ActionSelectionItem, bool>) (action =>
        {
          if (action != this.selectAllItem && action.IsSelected.HasValue)
            return action.IsSelected.Value;
          return false;
        })).Select<ActionSelectionItem, ConfigClause>((Func<ActionSelectionItem, ConfigClause>) (action => action.ActionClause));
      }
    }

    public void LoadActions(IEnumerable<ConfigClause> clauses)
    {
      this.ActionSelectionItems.Clear();
      this.selectAllItem = new ActionSelectionItem("(Select All)");
      this.ActionSelectionItems.Add(this.selectAllItem);
      using (IEnumerator<ConfigClause> enumerator = clauses.GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          ConfigClause current = enumerator.Current;
          if (current is ConfigClause.ImportMethod || current is ConfigClause.DeclareMethod || current is ConfigClause.ImportAllFromScope)
            this.ActionSelectionItems.Add(new ActionSelectionItem(current));
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void SendNotification(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void ActionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs evtArgs)
    {
      if (evtArgs.NewItems == null)
        return;
      foreach (ActionSelectionItem newItem in (IEnumerable) evtArgs.NewItems)
      {
        if (evtArgs.Action == NotifyCollectionChangedAction.Add)
          newItem.PropertyChanged += new PropertyChangedEventHandler(this.ActionSelectionChanged);
        else if (evtArgs.Action == NotifyCollectionChangedAction.Remove)
          newItem.PropertyChanged -= new PropertyChangedEventHandler(this.ActionSelectionChanged);
      }
    }

    private void ActionSelectionChanged(object sender, PropertyChangedEventArgs evtArgs)
    {
      if (!(evtArgs.PropertyName == "IsSelected") || !(sender is ActionSelectionItem))
        return;
      if (this.selectAllItem != sender && !this.propertyChangedEventHandlerLocked)
      {
        this.propertyChangedEventHandlerLocked = true;
        this.selectAllItem.IsSelected = !this.ActionSelectionItems.Where<ActionSelectionItem>((Func<ActionSelectionItem, bool>) (item => item != this.selectAllItem)).All<ActionSelectionItem>((Func<ActionSelectionItem, bool>) (info =>
        {
          if (info.IsSelected.HasValue)
            return info.IsSelected.Value;
          return false;
        })) ? (!this.ActionSelectionItems.Where<ActionSelectionItem>((Func<ActionSelectionItem, bool>) (item => item != this.selectAllItem)).All<ActionSelectionItem>((Func<ActionSelectionItem, bool>) (info =>
        {
          if (info.IsSelected.HasValue)
            return !info.IsSelected.Value;
          return false;
        })) ? new bool?() : new bool?(false)) : new bool?(true);
        this.propertyChangedEventHandlerLocked = false;
      }
      else
      {
        if (this.selectAllItem != sender || this.propertyChangedEventHandlerLocked)
          return;
        this.propertyChangedEventHandlerLocked = true;
        foreach (ActionSelectionItem actionSelectionItem in (Collection<ActionSelectionItem>) this.ActionSelectionItems)
        {
          if (actionSelectionItem != this.selectAllItem)
            actionSelectionItem.IsSelected = this.selectAllItem.IsSelected;
        }
        this.propertyChangedEventHandlerLocked = false;
      }
    }
  }
}
