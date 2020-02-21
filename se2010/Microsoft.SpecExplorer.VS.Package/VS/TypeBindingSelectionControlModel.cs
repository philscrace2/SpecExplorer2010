// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.TypeBindingSelectionControlModel
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.SpecExplorer.VS
{
  public class TypeBindingSelectionControlModel : INotifyPropertyChanged
  {
    private bool existingClassSelected = true;

    public bool ExistingClassSelected
    {
      get
      {
        return this.existingClassSelected;
      }
      set
      {
        if (this.existingClassSelected == value)
          return;
        this.existingClassSelected = value;
        this.SendNotification(ExistingClassSelected.ToString());
      }
    }

    public CodeClass SelectedClass
    {
      get
      {
        if (!this.ExistingClassSelected)
          return (CodeClass) null;
        CodeElementAndContainerPair andContainerPair = this.ViewerModel.RetrieveSelectedItems(vsCMElement.vsCMElementClass).SingleOrDefault<CodeElementAndContainerPair>();
        if (andContainerPair == null)
          return (CodeClass) null;
        return andContainerPair.Element as CodeClass;
      }
    }

    public CodeElementViewerModel ViewerModel { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    private void SendNotification(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
