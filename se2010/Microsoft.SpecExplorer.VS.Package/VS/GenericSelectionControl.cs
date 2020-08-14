// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.GenericSelectionControl
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.SpecExplorer.VS
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public partial class GenericSelectionControl : UserControl
  {
    public static readonly DependencyProperty TextInputLabelProperty = DependencyProperty.Register("TextInputLabel", typeof (string), typeof (GenericSelectionControl));
    public static readonly DependencyProperty TextInputValueProperty = DependencyProperty.Register("TextInputValue", typeof (string), typeof (GenericSelectionControl));
    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof (ICordSyntaxElementInfo), typeof (GenericSelectionControl));
    internal GenericSelectionControl selectionControl;
    internal Grid textInputPanel;
    internal TextBox textInputBox;
    internal ListView itemListView;
    private bool _contentLoaded;

    public GenericSelectionControl(bool textInputEnabled, string headerText)
    {
      this.ItemList = new ObservableCollection<ICordSyntaxElementInfo>();
      this.HeaderText = headerText;
      this.InitializeComponent();
      if (textInputEnabled)
        return;
      this.textInputPanel.Visibility = Visibility.Collapsed;
    }

    public ObservableCollection<ICordSyntaxElementInfo> ItemList { get; private set; }

    public string TextInputLabel
    {
      get
      {
        return (string) this.GetValue(GenericSelectionControl.TextInputLabelProperty);
      }
      set
      {
        this.SetValue(GenericSelectionControl.TextInputLabelProperty, (object) value);
      }
    }

    public string TextInputValue
    {
      get
      {
        return (string) this.GetValue(GenericSelectionControl.TextInputValueProperty);
      }
      set
      {
        this.SetValue(GenericSelectionControl.TextInputValueProperty, (object) value);
      }
    }

    public ICordSyntaxElementInfo SelectedItem
    {
      get
      {
        return (ICordSyntaxElementInfo) this.GetValue(GenericSelectionControl.SelectedItemProperty);
      }
      set
      {
        this.SetValue(GenericSelectionControl.SelectedItemProperty, (object) value);
      }
    }

    public string HeaderText { get; private set; }

    private void selectionControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.textInputPanel.IsVisible)
        this.textInputBox.Focus();
      else
        this.itemListView.Focus();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.SpecExplorer.VS.Package;V2.2.0.0;component/assistedprocedures/genericselectioncontrol.xaml", UriKind.Relative));
    }

    //[EditorBrowsable(EditorBrowsableState.Never)]
    //[DebuggerNonUserCode]
    //void IComponentConnector.Connect(int connectionId, object target)
    //{
    //  switch (connectionId)
    //  {
    //    case 1:
    //      this.selectionControl = (GenericSelectionControl) target;
    //      this.selectionControl.Loaded += new RoutedEventHandler(this.selectionControl_Loaded);
    //      break;
    //    case 2:
    //      this.textInputPanel = (Grid) target;
    //      break;
    //    case 3:
    //      this.textInputBox = (TextBox) target;
    //      break;
    //    case 4:
    //      this.itemListView = (ListView) target;
    //      break;
    //    default:
    //      this._contentLoaded = true;
    //      break;
    //  }
    //}
  }
}
