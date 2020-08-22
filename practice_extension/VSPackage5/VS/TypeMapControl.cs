// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.TypeMapControl
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using EnvDTE80;
using Microsoft.Modeling;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Microsoft.SpecExplorer.VS
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public partial class TypeMapControl : UserControl, IStyleConnector
  {
    private IEnumerable availableCodeElements;
    //internal TypeMapControl typeMapControl;
    //private bool _contentLoaded;

    public TypeMapControl()
    {
      this.TypeMap = new ObservableCollection<TypeMapUnit>();
      this.InitializeComponent();
    }

    public ObservableCollection<TypeMapUnit> TypeMap { get; private set; }

    public System.Windows.Window ContainerWindow { get; set; }

    public SpecExplorerPackage Package { get; set; }

    public void LoadImplementationTypes(
      IEnumerable<ProcedureType> types,
      IEnumerable availableCodeElements)
    {
      this.TypeMap.Clear();
      foreach (ProcedureType type in types)
        this.TypeMap.Add(new TypeMapUnit(type));
      this.availableCodeElements = availableCodeElements;
    }

    private void InvokeTypeMap(TypeMapUnit typeMapUnit)
    {
      TypeBindingSelectionWindow bindingSelectionWindow1 = new TypeBindingSelectionWindow();
      bindingSelectionWindow1.Owner = this.ContainerWindow;
      TypeBindingSelectionWindow bindingSelectionWindow2 = bindingSelectionWindow1;
      TypeBindingSelectionControlModel controlModel = bindingSelectionWindow2.ControlModel;
      controlModel.ViewerModel.CodeElementContentValidator = new Func<CodeElement, bool>(this.IsCodeElementContentValid);
      controlModel.ViewerModel.CodeElementDisplayTextFabricator = new Func<CodeElementItem, string>(this.FabricateCodeElementDisplayText);
      controlModel.ViewerModel.LoadCodeElements(this.availableCodeElements, false, CodeElementExpandOptions.ExpandToNamespaces | CodeElementExpandOptions.ExpandToClasses, false);
      bool? nullable = bindingSelectionWindow2.ShowDialog();
      if (!nullable.HasValue || !nullable.Value)
        return;
      typeMapUnit.ModelClass = controlModel.SelectedClass;
    }

    private void TypeMapButtonClicked(object sender, RoutedEventArgs e)
    {
      this.InvokeTypeMap((sender as Button).Tag as TypeMapUnit);
    }

    private void TypeMapItemSelected(object sender, RoutedEventArgs e)
    {
      this.InvokeTypeMap((sender as ListViewItem).Content as TypeMapUnit);
    }

    private bool IsCodeElementContentValid(CodeElement codeElement)
    {
      if (codeElement == null)
        return false;
      switch (codeElement.Kind)
      {
        case vsCMElement.vsCMElementClass:
          CodeClass2 codeClass2 = codeElement as CodeClass2;
          bool flag = codeClass2 != null && codeClass2.IsShared;
          if (!codeElement.FullName.Contains("System.") && !(codeElement as CodeClass).Attributes.Cast<CodeAttribute>().Any<CodeAttribute>((Func<CodeAttribute, bool>) (attr => attr.FullName == typeof (TypeBindingAttribute).FullName)) && !this.TypeMap.Any<TypeMapUnit>((Func<TypeMapUnit, bool>) (binding => binding.ModelClass == codeElement)))
            return !flag;
          return false;
        case vsCMElement.vsCMElementNamespace:
          CodeNamespace codeNamespace = codeElement as CodeNamespace;
          if (codeNamespace != null)
            return codeNamespace.IsValid();
          return false;
        default:
          return false;
      }
    }

    private string FabricateCodeElementDisplayText(CodeElementItem codeElementItem)
    {
      string str = codeElementItem.GetPrototype(false);
      if (this.Package != null)
      {
        string relativeToProject = this.Package.ComputePathRelativeToProject(codeElementItem.RootElement.ProjectItem.ContainingProject, codeElementItem.RootElement.ProjectItem);
        this.Package.Assert(!string.IsNullOrEmpty(relativeToProject));
        str = string.Format("{0} [{1}]", (object) str, (object) relativeToProject);
      }
      return str;
    }

    //[DebuggerNonUserCode]
    //public void InitializeComponent()
    //{
    //  if (this._contentLoaded)
    //    return;
    //  this._contentLoaded = true;
    //  Application.LoadComponent((object) this, new Uri("/Microsoft.SpecExplorer.VS.Package;V2.2.0.0;component/assistedprocedures/typemapcontrol.xaml", UriKind.Relative));
    //}

    //[EditorBrowsable(EditorBrowsableState.Never)]
    //[DebuggerNonUserCode]
    //void IComponentConnector.Connect(int connectionId, object target)
    //{
    //  if (connectionId == 1)
    //    this.typeMapControl = (TypeMapControl) target;
    //  else
    //    this._contentLoaded = true;
    //}

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 2:
          ((Style) target).Setters.Add((SetterBase) new EventSetter()
          {
            Event = ListBoxItem.SelectedEvent,
            Handler = (Delegate) new RoutedEventHandler(this.TypeMapItemSelected)
          });
          break;
        case 3:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.TypeMapButtonClicked);
          break;
      }
    }
  }
}
