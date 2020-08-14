// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.AssistedProcedureWizardWindow
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.SpecExplorer.VS
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public partial class AssistedProcedureWizardWindow : Window
  {
    public DependencyProperty BannerTextProperty = DependencyProperty.Register(BannerText, typeof (string), typeof (AssistedProcedureWizardWindow));
    public static readonly DependencyProperty BannerHeaderProperty = DependencyProperty.Register(nameof (BannerHeader), typeof (string), typeof (AssistedProcedureWizardWindow));
    public static readonly DependencyProperty IsStartStateProperty = DependencyProperty.Register(nameof (IsStartState), typeof (bool), typeof (AssistedProcedureWizardWindow));
    public static readonly DependencyProperty IsFinalStateProperty = DependencyProperty.Register(nameof (IsFinalState), typeof (bool), typeof (AssistedProcedureWizardWindow));
    public static readonly DependencyProperty WarningTextProperty = DependencyProperty.Register(nameof (WanringText), typeof (string), typeof (AssistedProcedureWizardWindow));
    public static readonly DependencyProperty WarningTextVisibleProperty = DependencyProperty.Register(nameof (WarningTextVisible), typeof (Visibility), typeof (AssistedProcedureWizardWindow));
    internal AssistedProcedureWizardWindow wizardWindow;
    internal Grid userControlContainer;
    internal Button previousPageButton;
    internal Button nextPageButton;
    internal Button cancelWizardButton;
    private bool _contentLoaded;

    public AssistedProcedureWizardWindow(string title)
    {
      this.InitializeComponent();
      this.Title = title;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
    }

    private void previousPageButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.PreviousPageRequestedEvent == null)
        return;
      this.PreviousPageRequestedEvent((object) this, (EventArgs) e);
    }

    private void nextPageButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.NextPageRequestedEvent == null)
        return;
      this.NextPageRequestedEvent((object) this, (EventArgs) e);
    }

    private void cancelWizardButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.WizardCancelRequestedEvent == null)
        return;
      this.WizardCancelRequestedEvent((object) this, (EventArgs) e);
    }

    public event EventHandler PreviousPageRequestedEvent;

    public event EventHandler NextPageRequestedEvent;

    public event EventHandler WizardCancelRequestedEvent;

    public string BannerHeader
    {
      get
      {
        return (string) this.GetValue(AssistedProcedureWizardWindow.BannerHeaderProperty);
      }
      set
      {
        this.SetValue(AssistedProcedureWizardWindow.BannerHeaderProperty, (object) value);
      }
    }

    public string BannerText
    {
      get
      {
        return (string) this.GetValue(AssistedProcedureWizardWindow.BannerTextProperty);
      }
      set
      {
        this.SetValue(AssistedProcedureWizardWindow.BannerTextProperty, (object) value);
      }
    }

    public string WanringText
    {
      get
      {
        return (string) this.GetValue(AssistedProcedureWizardWindow.WarningTextProperty);
      }
      set
      {
        this.SetValue(AssistedProcedureWizardWindow.WarningTextProperty, (object) value);
      }
    }

    public Visibility WarningTextVisible
    {
      get
      {
        return (Visibility) this.GetValue(AssistedProcedureWizardWindow.WarningTextVisibleProperty);
      }
      set
      {
        this.SetValue(AssistedProcedureWizardWindow.WarningTextVisibleProperty, (object) value);
      }
    }

    public bool IsStartState
    {
      get
      {
        return (bool) this.GetValue(AssistedProcedureWizardWindow.IsStartStateProperty);
      }
      set
      {
        this.SetValue(AssistedProcedureWizardWindow.IsStartStateProperty, (object) value);
      }
    }

    public bool IsFinalState
    {
      get
      {
        return (bool) this.GetValue(AssistedProcedureWizardWindow.IsFinalStateProperty);
      }
      set
      {
        this.SetValue(AssistedProcedureWizardWindow.IsFinalStateProperty, (object) value);
      }
    }

    public void LoadUserControl(UserControl userControl)
    {
      this.userControlContainer.Children.Clear();
      this.userControlContainer.Children.Add((UIElement) userControl);
    }

    //[DebuggerNonUserCode]
    //public void InitializeComponent()
    //{
    //  if (this._contentLoaded)
    //    return;
    //  this._contentLoaded = true;
    //  Application.LoadComponent((object) this, new Uri("/Microsoft.SpecExplorer.VS.Package;V2.2.0.0;component/assistedprocedures/assistedprocedurewizardwindow.xaml", UriKind.Relative));
    //}

    //[DebuggerNonUserCode]
    //[EditorBrowsable(EditorBrowsableState.Never)]
    //void IComponentConnector.Connect(int connectionId, object target)
    //{
    //  switch (connectionId)
    //  {
    //    case 1:
    //      this.wizardWindow = (AssistedProcedureWizardWindow) target;
    //      break;
    //    case 2:
    //      this.userControlContainer = (Grid) target;
    //      break;
    //    case 3:
    //      this.previousPageButton = (Button) target;
    //      this.previousPageButton.Click += new RoutedEventHandler(this.previousPageButton_Click);
    //      break;
    //    case 4:
    //      this.nextPageButton = (Button) target;
    //      this.nextPageButton.Click += new RoutedEventHandler(this.nextPageButton_Click);
    //      break;
    //    case 5:
    //      this.cancelWizardButton = (Button) target;
    //      this.cancelWizardButton.Click += new RoutedEventHandler(this.cancelWizardButton_Click);
    //      break;
    //    default:
    //      this._contentLoaded = true;
    //      break;
    //  }
    //}
  }
}
