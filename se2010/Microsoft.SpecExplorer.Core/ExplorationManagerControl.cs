// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ExplorationManagerControl
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines.Cord;
using Microsoft.Xrt;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.SpecExplorer
{
  public partial class ExplorationManagerControl : UserControl
  {
    private const string TestEnabled = "TestEnabled";
    private const string ForExploration = "ForExploration";
    private const string Group = "Group";
    private const string Description = "Description";
    private const string RecommendedViews = "RecommendedViews";
    private const string RecommendedViewsFilter = "Recommended Views";
    private const string AllColumnsFilter = "[All Columns]";
    private const string MachineNameFilter = "Machine";
    private const string ProjectFilter = "Project";
    private const string TestEnabledFilter = "Test Enabled";
    public static readonly RoutedCommand ClearFilterCommand = new RoutedCommand();
    public static readonly RoutedCommand ExploreCommand = new RoutedCommand();
    public static readonly RoutedCommand ExploreSelectedMachineCommand = new RoutedCommand();
    public static readonly RoutedCommand ReexploreCommand = new RoutedCommand();
    public static readonly RoutedCommand ReexploreSelectedMachineCommand = new RoutedCommand();
    public static readonly RoutedCommand PerformCheckedTasks = new RoutedCommand();
    public static readonly RoutedCommand ValidateCommand = new RoutedCommand();
    public static readonly RoutedCommand NavigateToMachineCommand = new RoutedCommand();
    public static readonly RoutedCommand GenerateTestCodeCommand = new RoutedCommand();
    public static readonly RoutedCommand GenerateTestCodeForSelectedMachineCommand = new RoutedCommand();
    public static readonly RoutedCommand AbortTaskCommand = new RoutedCommand();
    public static readonly RoutedCommand OnTheFlyTestCommand = new RoutedCommand();
    public static readonly RoutedCommand OnTheFlyTestSelectedMachineCommand = new RoutedCommand();
    public static readonly RoutedCommand OnTheFlyReplayCommand = new RoutedCommand();
    private GridViewColumnHeader lastSortColumn;
    private ListSortDirection lastDirection;
    private ListView machineListView;
    private Label unAvailableMessageLabel;
    private Func<ComponentBase> serviceProvider;
    private List<MenuItem> postProcessorMenuItems;
    public static readonly DependencyProperty IsAvailableViewProperty = DependencyProperty.Register(nameof (IsAvailableView), typeof (bool), typeof (ExplorationManagerControl), new PropertyMetadata((object) false));
    public static readonly DependencyProperty IsOperatingProperty = DependencyProperty.Register(nameof (IsOperating), typeof (bool), typeof (ExplorationManagerControl), new PropertyMetadata((object) false));
    public static readonly DependencyProperty StoppableProperty = DependencyProperty.Register(nameof (Stoppable), typeof (bool), typeof (ExplorationManagerControl), new PropertyMetadata((object) false));
    private List<Machine> selectedMachines;
    private string filterColumn; 

    public event EventHandler Validate;

    public event EventHandler<MachineEventArgs> Explore;

    public event EventHandler<MachineEventArgs> OnTheFlyTest;

    public event EventHandler<MachineEventArgs> OnTheFlyTestReplay;

    public event EventHandler<MachineEventArgs> GenerateTestCode;

    public event EventHandler<MachineEventArgs> ExecutePostProcessing;

    public event EventHandler AbortOperation;

    public event EventHandler<MachineEventArgs> NavigateToMachine;

    public event EventHandler<MachineEventArgs> ShowProperties;

    public event EventHandler<MachineEventArgs> ChangeSelectedMachine;

    private void PropertiesCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = this.SingleMachineSelected;

    private void PropertiesCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (!this.SingleMachineSelected || this.ShowProperties == null)
        return;
      this.ShowProperties((object) this, new MachineEventArgs((IList<Machine>) this.selectedMachines));
    }

    private void ClearFilterCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = !string.IsNullOrEmpty(this.searchTextBox.Text);

    private void ClearFilterCommandExecuted(object sender, ExecutedRoutedEventArgs e) => this.searchTextBox.Text = string.Empty;

    private void ExploreCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = !this.IsOperating && this.MachinesSelected && this.Explore != null;

    private void ExploreCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (this.IsOperating || !this.MachinesSelected || this.Explore == null)
        return;
      this.Stoppable = true;
      this.Explore((object) this, new MachineEventArgs((IList<Machine>) this.selectedMachines));
    }

    private void ReexploreCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (this.IsOperating || !this.MachinesSelected || this.Explore == null)
        return;
      this.Stoppable = true;
      this.Explore((object) this, new MachineEventArgs((IList<Machine>) this.selectedMachines, true));
    }

    private void PerformCheckedTasksCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (this.IsOperating || !this.MachinesSelected || (this.postProcessorMenuItems == null || this.ExecutePostProcessing == null))
        return;
      this.Stoppable = true;
      List<string> stringList = new List<string>();
      foreach (MenuItem processorMenuItem in this.postProcessorMenuItems)
      {
        if (processorMenuItem.IsCheckable && processorMenuItem.IsChecked)
          stringList.Add(processorMenuItem.Name);
      }
      this.ExecutePostProcessing((object) this, new MachineEventArgs((IList<Machine>) this.selectedMachines, postProcessors: ((IEnumerable<string>) stringList)));
    }

    private void ValidateCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = !this.IsOperating;

    private void ValidateCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (this.IsOperating || this.Validate == null)
        return;
      this.Stoppable = false;
      this.Validate((object) this, (EventArgs) null);
    }

    private void NavigateToMachineCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = this.SingleMachineSelected;

    private void NavigateToMachineCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (!this.SingleMachineSelected || this.NavigateToMachine == null)
        return;
      this.NavigateToMachine((object) this, new MachineEventArgs((IList<Machine>) this.selectedMachines));
    }

    private void GenerateTestCodeCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (this.IsOperating || !this.MachinesSelected)
        return;
      e.CanExecute = this.selectedMachines.Any<Machine>((Func<Machine, bool>) (m => string.Compare("true", m.TestEnabled) == 0));
    }

    private void GenerateTestCodeCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (this.IsOperating || !this.MachinesSelected || this.GenerateTestCode == null)
        return;
      this.Stoppable = true;
      this.GenerateTestCode((object) this, new MachineEventArgs((IList<Machine>) this.selectedMachines));
    }

    private void AbortTaskCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = this.IsOperating && this.Stoppable;

    private void AbortTaskCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (!this.IsOperating || !this.Stoppable || this.AbortOperation == null)
        return;
      this.IsOperating = true;
      this.Stoppable = false;
      this.AbortOperation((object) this, (EventArgs) null);
    }

    private void OnTheFlyTestCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (this.IsOperating || !this.MachinesSelected || this.OnTheFlyTest == null)
        return;
      this.Stoppable = true;
      this.OnTheFlyTest((object) this, new MachineEventArgs((IList<Machine>) this.selectedMachines));
    }

    private void OnTheFlyReplayCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = !this.IsOperating && this.SingleMachineSelected && this.OnTheFlyTestReplay != null;

    private void OnTheFlyReplayCommandExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (this.IsOperating || !this.SingleMachineSelected || this.OnTheFlyTestReplay == null)
        return;
      this.Stoppable = true;
      this.OnTheFlyTestReplay((object) this, new MachineEventArgs((IList<Machine>) this.selectedMachines, true));
    }

    public void FinishOperation() => this.InvokeIfNecessary((System.Action) (() =>
    {
      this.IsOperating = false;
      this.Stoppable = false;
      CommandManager.InvalidateRequerySuggested();
    }));

    public void StartOperation() => this.InvokeIfNecessary((System.Action) (() =>
    {
      this.IsOperating = true;
      CommandManager.InvalidateRequerySuggested();
    }));

    private void InvokeIfNecessary(System.Action action)
    {
      if (this.Dispatcher.CheckAccess())
        action();
      else
        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) action);
    }

    private bool MachinesSelected => this.machineListView != null && this.selectedMachines != null && this.selectedMachines.Count > 0;

    private bool SingleMachineSelected => this.machineListView != null && this.selectedMachines != null && this.selectedMachines.Count == 1;

    public ExplorationManagerControl(
      Func<ComponentBase> serviceProvider,
      bool availableView,
      IDictionary<string, string> postProcessorDisplayNameMap)
    {
      this.serviceProvider = serviceProvider != null ? serviceProvider : throw new ArgumentNullException(nameof (serviceProvider));
      this.Machines = new ObservableCollection<Machine>();
      ObservableCollection<string> observableCollection = new ObservableCollection<string>();
      observableCollection.Add("[All Columns]");
      observableCollection.Add("Machine");
      observableCollection.Add("Test Enabled");
      observableCollection.Add(nameof (Description));
      observableCollection.Add(nameof (Group));
      observableCollection.Add("Project");
      observableCollection.Add("Recommended Views");
      this.FilterTexts = observableCollection;
      this.InitializeComponent();
      this.Loaded += (RoutedEventHandler) delegate
      {
        PresentationSource presentationSource = PresentationSource.FromVisual((Visual) this);
        if (presentationSource == null || !(presentationSource.CompositionTarget is HwndTarget compositionTarget2))
          return;
        compositionTarget2.RenderMode = RenderMode.SoftwareOnly;
      };
      this.machineListView = this.Resources[(object) "enabledView"] as ListView;
      this.unAvailableMessageLabel = this.Resources[(object) "disabledView"] as Label;
      this.InitializeCustomizedProcessorsMenu(postProcessorDisplayNameMap);
      this.UpdateView(availableView);
    }

    private void InitializeCustomizedProcessorsMenu(
      IDictionary<string, string> postProcessorDisplayNameMap)
    {
      if (this.postProcessorMenuItems != null)
        return;
      this.postProcessorMenuItems = new List<MenuItem>();
      this.postProcessorMenuItems = new List<MenuItem>();
      MenuItem menuItem1 = this.machineListView.ContextMenu.Items.OfType<MenuItem>().FirstOrDefault<MenuItem>((Func<MenuItem, bool>) (item => item.Name == "performUserTask"));
      if (menuItem1 == null)
        return;
      if (postProcessorDisplayNameMap != null && postProcessorDisplayNameMap.Count<KeyValuePair<string, string>>() > 0)
      {
        foreach (string key in (IEnumerable<string>) postProcessorDisplayNameMap.Keys)
        {
          MenuItem menuItem2 = new MenuItem();
          menuItem2.IsCheckable = true;
          menuItem2.IsChecked = true;
          menuItem2.Name = key;
          if (postProcessorDisplayNameMap[key].Length > 32)
            menuItem2.Header = (object) (postProcessorDisplayNameMap[key].Substring(0, 32) + "…");
          else
            menuItem2.Header = (object) postProcessorDisplayNameMap[key];
          menuItem1.Items.Add((object) menuItem2);
          this.postProcessorMenuItems.Add(menuItem2);
        }
      }
      else
        menuItem1.IsEnabled = false;
    }

    public void UpdateView(bool availableView) => this.InvokeIfNecessary((System.Action) (() =>
    {
      this.ProgressMessage(string.Empty);
      if (this.lastSortColumn != null)
      {
        this.lastSortColumn.Column.HeaderTemplate = (DataTemplate) null;
        this.lastSortColumn = (GridViewColumnHeader) null;
        ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) this.Machines);
        using (defaultView.DeferRefresh())
          defaultView.SortDescriptions.Clear();
      }
      this.lastDirection = ListSortDirection.Ascending;
      this.searchTextBox.Clear();
      this.filterColumn = (string) null;
      this.filterComboBox.SelectedIndex = 0;
      this.selectedMachines = (List<Machine>) null;
      UIElement element;
      if (availableView)
      {
        element = (UIElement) this.machineListView;
        this.machineListView.ItemsSource = (IEnumerable) this.Machines;
        this.UpdateMachines();
      }
      else
        element = (UIElement) this.unAvailableMessageLabel;
      Grid.SetColumn(element, 0);
      Grid.SetRow(element, 2);
      if (this.machineViewGrid.Children.Count > 2)
        this.machineViewGrid.Children.RemoveAt(2);
      this.machineViewGrid.Children.Insert(2, element);
      this.IsAvailableView = availableView;
    }));

    public bool IsAvailableView
    {
      get => (bool) this.GetValue(ExplorationManagerControl.IsAvailableViewProperty);
      set => this.SetValue(ExplorationManagerControl.IsAvailableViewProperty, (object) value);
    }

    public bool IsOperating
    {
      get => (bool) this.GetValue(IsOperatingProperty);
      set => this.SetValue(ExplorationManagerControl.IsOperatingProperty, (object) value);
    }

    public bool Stoppable
    {
      get => (bool) this.GetValue(ExplorationManagerControl.StoppableProperty);
      set => this.SetValue(ExplorationManagerControl.StoppableProperty, (object) value);
    }

    public ObservableCollection<Machine> Machines { get; private set; }

    public ObservableCollection<string> FilterTexts { get; private set; }

    public void ProgressMessage(string message) => this.InvokeIfNecessary((System.Action) (() => this.informationBar.Content = (object) message));

    public void UpdateMachines() => this.InvokeIfNecessary((System.Action) (() =>
    {
      List<Machine> selectedMachines = this.selectedMachines;
      this.Machines.Clear();
      List<MachineContainer> source1 = new List<MachineContainer>();
      ComponentBase componentBase = this.serviceProvider();
      if (componentBase == null)
        return;
      ICordDesignTimeScopeManager service = componentBase.GetService<ICordDesignTimeScopeManager>();
      if (service == null)
        return;
      foreach (string allScope in (IEnumerable<string>) service.AllScopes)
      {
        string str1 = Path.ChangeExtension(allScope, (string) null);
        int num = str1.LastIndexOf('\\');
        if (num > 0)
          str1 = str1.Substring(num + 1);
        MachineContainer machineContainer = new MachineContainer()
        {
          Name = str1,
          UniqueName = allScope
        };
        source1.Add(machineContainer);
        ICordDesignTimeManager designTimeManager = service.GetCordDesignTimeManager(allScope);
        if (designTimeManager != null)
        {
          ICollection<Config> allConfigurations = designTimeManager.AllConfigurations;
          foreach (MachineDefinition allMachine in (IEnumerable<MachineDefinition>) designTimeManager.AllMachines)
          {
            if (string.Compare(true.ToString(), this.GetOptionValue("ForExploration", (IEnumerable<Config>) allConfigurations, allMachine), true) == 0)
            {
              string str2 = this.GetOptionValue("TestEnabled", (IEnumerable<Config>) allConfigurations, allMachine);
              if (str2 != null)
                str2 = str2.ToLower();
              this.Machines.Add(new Machine()
              {
                Name = allMachine.Name,
                Container = machineContainer,
                TestEnabled = str2,
                Group = this.GetOptionValue("Group", (IEnumerable<Config>) allConfigurations, allMachine),
                RecommendedViews = this.GetOptionValue("RecommendedViews", (IEnumerable<Config>) allConfigurations, allMachine),
                Description = this.GetOptionValue("Description", (IEnumerable<Config>) allConfigurations, allMachine)
              });
            }
          }
        }
      }
      foreach (IGrouping<string, MachineContainer> source2 in source1.GroupBy<MachineContainer, string>((Func<MachineContainer, string>) (container => container.Name)).Select<IGrouping<string, MachineContainer>, IGrouping<string, MachineContainer>>((Func<IGrouping<string, MachineContainer>, IGrouping<string, MachineContainer>>) (g => g)))
      {
        if (source2.Count<MachineContainer>() > 1)
        {
          foreach (MachineContainer machineContainer in (IEnumerable<MachineContainer>) source2)
            machineContainer.Name = machineContainer.UniqueName.Replace('\\', '.');
        }
      }
      if (selectedMachines == null || selectedMachines.Count <= 0)
        return;
      foreach (object obj in selectedMachines)
        this.machineListView.SelectedItem = obj;
    }));

    private string GetOptionValue(
      string name,
      IEnumerable<Config> configs,
      MachineDefinition machine)
    {
      string str;
      if (machine.AnonymousConfig != null && this.TryGetOptionValue(name, machine.AnonymousConfig, configs, (HashSet<string>) null, out str))
        return str;
      if (machine.Vocabularies != null)
      {
        for (int index = machine.Vocabularies.Length - 1; index >= 0; --index)
        {
          ConfigReference vocReference = machine.Vocabularies[index];
          if (vocReference != null)
          {
            Config config = configs.FirstOrDefault<Config>((Func<Config, bool>) (v => v.Name == vocReference.Name));
            HashSet<string> visiting = new HashSet<string>();
            if (this.TryGetOptionValue(name, config, configs, visiting, out str))
              return str;
          }
        }
      }
      ComponentBase componentBase = this.serviceProvider();
      if (componentBase != null)
      {
        IOptionSetManager service = componentBase.GetService<IOptionSetManager>();
        if (service != null)
        {
          PropertyDescriptor property = service.FindProperty(name, Microsoft.Xrt.Visibility.Public);
          object obj;
          if (property != null && service.TryGetDefaultValue(property, out obj) && obj != null)
            return obj.ToString();
        }
      }
      return string.Empty;
    }

    private bool TryGetOptionValue(
      string name,
      Config config,
      IEnumerable<Config> allConfigs,
      HashSet<string> visiting,
      out string value)
    {
      value = (string) null;
      if (config != null && (visiting == null || config.Name == null || visiting.Add(config.Name)))
      {
        ConfigClause.DeclareSwitch declareSwitch = config.Clauses.OfType<ConfigClause.DeclareSwitch>().LastOrDefault<ConfigClause.DeclareSwitch>((Func<ConfigClause.DeclareSwitch, bool>) (c => string.Compare(c.Name, name, true) == 0));
        if (declareSwitch != null)
        {
          value = declareSwitch.Value;
          return true;
        }
        ConfigClause.IncludeConfig[] baseConfigs = config.Clauses.OfType<ConfigClause.IncludeConfig>().Where<ConfigClause.IncludeConfig>((Func<ConfigClause.IncludeConfig, bool>) (v => v.Vocabulary != null)).ToArray<ConfigClause.IncludeConfig>();
        for (int i = baseConfigs.Length - 1; i >= 0; --i)
        {
          Config config1 = allConfigs.FirstOrDefault<Config>((Func<Config, bool>) (v => v.Name == baseConfigs[i].Vocabulary.Name));
          if (config1 != null && this.TryGetOptionValue(name, config1, allConfigs, visiting, out value))
            return true;
        }
      }
      return false;
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e) => this.UpdateMachines();

    private void MachineSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.machineListView != null && this.machineListView.SelectedItem != null && (this.machineListView.SelectedItem is Machine && this.ChangeSelectedMachine != null))
      {
        this.selectedMachines = new List<Machine>();
        foreach (Machine machine in (IEnumerable) this.machineListView.Items)
        {
          if (this.machineListView.SelectedItems.Contains((object) machine))
            this.selectedMachines.Add(machine);
        }
        this.ChangeSelectedMachine((object) this, new MachineEventArgs((IList<Machine>) this.selectedMachines));
        this.ProgressMessage(string.Format("Machine(s) Selected: {0}", (object) this.selectedMachines.Count));
      }
      else
      {
        this.ProgressMessage(string.Empty);
        this.selectedMachines = (List<Machine>) null;
      }
      CommandManager.InvalidateRequerySuggested();
    }

    private void OnDoubleClickMachine(object sender, MouseButtonEventArgs e)
    {
      if (!(sender is ListViewItem))
        return;
      ListViewItem listViewItem = sender as ListViewItem;
      if (!(listViewItem.Content is Machine) || this.NavigateToMachine == null)
        return;
      this.NavigateToMachine((object) this, new MachineEventArgs((IList<Machine>) new List<Machine>()
      {
        listViewItem.Content as Machine
      }));
    }

    private void SortClick(object sender, RoutedEventArgs e)
    {
      if (!(e.OriginalSource is GridViewColumnHeader originalSource) || originalSource.Role == GridViewColumnHeaderRole.Padding)
        return;
      ListSortDirection direction = originalSource == this.lastSortColumn ? (this.lastDirection != ListSortDirection.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending) : ListSortDirection.Ascending;
      ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) this.Machines);
      using (defaultView.DeferRefresh())
      {
        defaultView.SortDescriptions.Clear();
        defaultView.SortDescriptions.Add(new SortDescription(originalSource.Tag as string, direction));
      }
      originalSource.Column.HeaderTemplate = direction != ListSortDirection.Ascending ? this.Resources[(object) "HeaderTemplateArrowDown"] as DataTemplate : this.Resources[(object) "HeaderTemplateArrowUp"] as DataTemplate;
      if (this.lastSortColumn != null && this.lastSortColumn != originalSource)
        this.lastSortColumn.Column.HeaderTemplate = (DataTemplate) null;
      this.lastSortColumn = originalSource;
      this.lastDirection = direction;
    }

    private void FilterSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.filterComboBox == null || e.AddedItems == null || e.AddedItems.Count <= 0)
        return;
      this.filterColumn = e.AddedItems[0] as string;
      this.FilterMachines();
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (this.filterComboBox == null)
        return;
      this.filterColumn = this.filterComboBox.SelectedItem as string;
      this.FilterMachines();
    }

    private void FilterMachines()
    {
      if (this.searchTextBox == null || this.filterColumn == null)
        return;
      ICollectionView defaultView = CollectionViewSource.GetDefaultView((object) this.Machines);
      if (!string.IsNullOrEmpty(this.searchTextBox.Text))
        defaultView.Filter = new Predicate<object>(this.Filter);
      else
        defaultView.Filter = (Predicate<object>) null;
    }

    private bool Filter(object item)
    {
      if (item is Machine machine && this.filterColumn != null)
      {
        string str = this.searchTextBox.Text.Trim();
        if (string.Compare(this.filterColumn, "[All Columns]") == 0)
          return machine.Name.ToLower().Contains(str.ToLower()) || machine.TestEnabled.ToLower().Contains(str.ToLower()) || (machine.Group.ToLower().Contains(str.ToLower()) || machine.RecommendedViews.ToLower().Contains(str.ToLower())) || machine.Container.Name.ToLower().Contains(str.ToLower()) || machine.Description.ToLower().Contains(str.ToLower());
        if (string.Compare(this.filterColumn, "Machine") == 0)
          return machine.Name.ToLower().Contains(str.ToLower());
        if (string.Compare(this.filterColumn, "Test Enabled") == 0)
          return machine.TestEnabled.ToLower().Contains(str.ToLower());
        if (string.Compare(this.filterColumn, "Group") == 0)
          return machine.Group.ToLower().Contains(str.ToLower());
        if (string.Compare(this.filterColumn, "Project") == 0)
          return machine.Container.Name.ToLower().Contains(str.ToLower());
        if (string.Compare(this.filterColumn, "Recommended Views") == 0)
          return machine.RecommendedViews.ToLower().Contains(str.ToLower());
        if (string.Compare(this.filterColumn, "Description") == 0)
          return machine.Description.ToLower().Contains(str.ToLower());
      }
      return true;
    }    
  }
}
