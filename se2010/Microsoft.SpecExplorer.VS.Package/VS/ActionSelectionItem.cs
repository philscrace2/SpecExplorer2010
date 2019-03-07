// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ActionSelectionItem
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.ActionMachines.Cord;
using System.ComponentModel;

namespace Microsoft.SpecExplorer.VS
{
  public class ActionSelectionItem : ICordSyntaxElementInfo, INotifyPropertyChanged
  {
    private bool? isSelected;

    public ActionSelectionItem(ConfigClause actionClause)
    {
      this.ActionClause = actionClause;
      this.DisplayText = ((object) this.ActionClause).ToString();
      this.isSelected = new bool?(false);
    }

    public ActionSelectionItem(string displayText)
    {
      this.ActionClause = (ConfigClause) null;
      this.DisplayText = displayText;
      this.isSelected = new bool?(false);
    }

    public ConfigClause ActionClause { get; private set; }

    public bool? IsSelected
    {
      get
      {
        return this.isSelected;
      }
      set
      {
        this.isSelected = value;
        this.SendNotification(nameof (IsSelected));
      }
    }

    public string DisplayText { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;

    private void SendNotification(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
