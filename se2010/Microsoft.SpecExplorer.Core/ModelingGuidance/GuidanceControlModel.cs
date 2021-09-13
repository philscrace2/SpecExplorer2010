// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ModelingGuidance.GuidanceControlModel
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
  public class GuidanceControlModel : INotifyPropertyChanged
  {
    private const string selectGuidanceEntry = "<Select Guidance ...>";
    private IGuidance selectedGuidance;
    private DelegateCommand assistedProcedureCommand;
    private DelegateCommand copyCodeCommand;
    private ObservableCollection<IGuidance> guidanceList;

    public GuidanceControlModel()
    {
      this.guidanceList = new ObservableCollection<IGuidance>();
      this.guidanceList.CollectionChanged += new NotifyCollectionChangedEventHandler(this.GuidanceListChanged);
      this.selectedGuidance = (IGuidance) new GuidanceImpl()
      {
        Description = "<Select Guidance ...>",
        Id = string.Empty,
        StructureField = new ActivityReferenceImpl[0]
      };
      this.guidanceList.Add(this.selectedGuidance);
      this.PropertyChanged += new PropertyChangedEventHandler(this.SelfPropertyChanged);
      this.assistedProcedureCommand = new DelegateCommand(new Action<object>(this.InvokeAssistedProcedureEvent));
      this.copyCodeCommand = new DelegateCommand((Action<object>) (codeText => Clipboard.SetText(codeText.ToString())));
    }

    public ObservableCollection<IGuidance> GuidanceList => this.guidanceList;

    public IGuidance SelectedGuidance
    {
      get => this.selectedGuidance;
      set
      {
        this.selectedGuidance = value;
        this.SendNotification(nameof (SelectedGuidance));
      }
    }

    public ICommand AssistedProcedureCommand => (ICommand) this.assistedProcedureCommand;

    public ICommand CopyCodeCommand => (ICommand) this.copyCodeCommand;

    public event EventHandler<AssistedProcedureRequestEventArgs> AssistedProcedureRequested;

    public event PropertyChangedEventHandler PropertyChanged;

    private void SendNotification(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void InvokeAssistedProcedureEvent(object procId)
    {
      if (this.AssistedProcedureRequested == null)
        return;
      try
      {
        this.AssistedProcedureRequested((object) this, new AssistedProcedureRequestEventArgs(Convert.ToUInt32(procId.ToString().Trim(), 16)));
      }
      catch (FormatException ex)
      {
        int num = (int) MessageBox.Show("Failed Invoking assisted procedure: Invalid format for Assisted Procedure Id", Resources.SpecExplorer);
      }
    }

    private void SelfPropertyChanged(object sender, PropertyChangedEventArgs evtArgs)
    {
      if (!(evtArgs.PropertyName == "SelectedGuidance"))
        return;
      IActivityReference[] structure = (sender as GuidanceControlModel).SelectedGuidance.Structure;
      IActivityReference activityReference1 = ((IEnumerable<IActivityReference>) structure).FirstOrDefault<IActivityReference>((Func<IActivityReference, bool>) (activityRef => !activityRef.IsCompleted));
      IActivityReference activityReference2 = activityReference1 == null ? ((IEnumerable<IActivityReference>) structure).LastOrDefault<IActivityReference>() : activityReference1;
      if (activityReference2 == null)
        return;
      activityReference2.IsSelected = true;
    }

    private void GuidanceListChanged(object sender, NotifyCollectionChangedEventArgs evtArgs)
    {
      foreach (IGuidance newItem in (IEnumerable) evtArgs.NewItems)
      {
        foreach (IActivityReference activityReference in newItem.Structure)
        {
          if (evtArgs.Action == NotifyCollectionChangedAction.Add)
            activityReference.PropertyChanged += new PropertyChangedEventHandler(this.ActivityPropertyChanged);
          else if (evtArgs.Action == NotifyCollectionChangedAction.Remove)
            activityReference.PropertyChanged -= new PropertyChangedEventHandler(this.ActivityPropertyChanged);
        }
      }
    }

    private void ActivityPropertyChanged(object sender, PropertyChangedEventArgs evtArgs)
    {
      IActivityReference actRef = sender as IActivityReference;
      if (!((IEnumerable<IActivityReference>) this.SelectedGuidance.Structure).Contains<IActivityReference>(actRef))
        return;
      if (evtArgs.PropertyName == "IsCompleted" && actRef.IsCompleted)
      {
        for (int index = 0; index < this.SelectedGuidance.Structure.Length; ++index)
        {
          if (actRef.Activity.Id == this.SelectedGuidance.Structure[index].Activity.Id && index + 1 < this.SelectedGuidance.Structure.Length)
          {
            this.SelectedGuidance.Structure[index + 1].IsSelected = true;
            break;
          }
        }
      }
      else
      {
        if (!(evtArgs.PropertyName == "IsSelected") || !actRef.IsSelected)
          return;
        Array.ForEach<IActivityReference>(this.SelectedGuidance.Structure, (Action<IActivityReference>) (act =>
        {
          if (actRef == act)
            return;
          act.IsSelected = false;
        }));
      }
    }
  }
}
