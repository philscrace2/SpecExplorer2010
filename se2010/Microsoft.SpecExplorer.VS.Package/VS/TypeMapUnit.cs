// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.TypeMapUnit
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using System.ComponentModel;

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
        return this.modelClass;
      }
      set
      {
        this.modelClass = value;
        this.SendNotification("ModelClassText");
      }
    }

    public TypeMapUnit(ProcedureType implementationType)
    {
      this.ImplementationType = implementationType;
    }

    public string ImplementationTypeText
    {
      get
      {
        return this.ImplementationType.ShortName;
      }
    }

    public string ModelClassText
    {
      get
      {
        if (this.ModelClass != null)
          return this.ModelClass.FullName;
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

    private void SendNotification(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
