// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.ViewDefinitionsControl
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Microsoft.SpecExplorer.Viewer
{
  public class ViewDefinitionsControl : UserControl
  {
    private List<IViewDefinition> viewDefinitions;
    private IContainer components;
    internal PropertyGrid propertyGrid;
    private ComboBox comboBox;
    private Label label;

    public IEnumerable<IViewDefinition> ViewDefinitionList
    {
      get => (IEnumerable<IViewDefinition>) this.viewDefinitions;
      set
      {
        this.viewDefinitions = new List<IViewDefinition>();
        this.comboBox.Items.Clear();
        foreach (IViewDefinition viewDefinition in value)
        {
          this.viewDefinitions.Add(viewDefinition);
          this.comboBox.Items.Add((object) viewDefinition.Name);
        }
        if (this.CurrentViewDefinition != null)
          this.comboBox.SelectedText = this.CurrentViewDefinition.Name;
        this.IsDirty = true;
      }
    }

    public IViewDefinition CurrentViewDefinition
    {
      get => (IViewDefinition) this.propertyGrid.SelectedObject;
      set
      {
        if (value == null)
          return;
        this.comboBox.SelectedItem = (object) value.Name;
        if (this.comboBox.SelectedIndex != -1)
          return;
        this.comboBox.SelectedIndex = 0;
      }
    }

    public bool IsDirty { get; set; }

    public void Add(IViewDefinition viewDefinition)
    {
      if (this.viewDefinitions.Find((Predicate<IViewDefinition>) (d => d.Name == viewDefinition.Name)) != null)
        throw new InvalidOperationException("View definition with the same name already exists");
      this.viewDefinitions.Add(viewDefinition);
      this.comboBox.Items.Add((object) viewDefinition.Name);
      this.IsDirty = true;
    }

    public void RemoveCurrentViewDefinition()
    {
      if (this.CurrentViewDefinition == null)
        throw new InvalidOperationException("Current view definition is null");
      this.viewDefinitions.Remove(this.CurrentViewDefinition);
      this.comboBox.Items.Remove((object) this.CurrentViewDefinition.Name);
      if (this.comboBox.Items.Count > 0)
        this.comboBox.SelectedIndex = 0;
      this.IsDirty = true;
    }

    private void OnSelectObjectChanged(object sender, EventArgs e)
    {
      ViewDefinition selectedObject = (ViewDefinition) this.propertyGrid.SelectedObject;
      if (selectedObject == null)
        return;
      this.comboBox.SelectedItem = (object) selectedObject.Name;
    }

    private void OnComboBoxSelect(object sender, EventArgs e)
    {
      this.propertyGrid.SelectedObject = (object) this.viewDefinitions[this.comboBox.SelectedIndex];
      this.propertyGrid.Enabled = !this.viewDefinitions[this.comboBox.SelectedIndex].IsDefault;
    }

    private void OnPropertyValueChanged(object sender, EventArgs e)
    {
      PropertyValueChangedEventArgs changedEventArgs = e as PropertyValueChangedEventArgs;
      ViewDefinition selectedObject = (ViewDefinition) this.propertyGrid.SelectedObject;
      switch (changedEventArgs.ChangedItem.Label)
      {
        case "Name":
          if (!Regex.IsMatch(selectedObject.Name, "^([a-zA-Z][a-zA-Z0-9]{0,19})$"))
          {
            this.Host.NotificationDialog(Resources.SpecExplorer, "View name must use alphanumeric (with upper and lower case distinct), the first of which must alphabetic (upper or lower case) and maximum length is 20 characters.");
            selectedObject.Name = changedEventArgs.OldValue as string;
            return;
          }
          foreach (IViewDefinition viewDefinition in this.viewDefinitions)
          {
            if (selectedObject != viewDefinition && selectedObject.Name.Equals(viewDefinition.Name))
            {
              this.Host.NotificationDialog(Resources.SpecExplorer, "Duplicate view name.");
              selectedObject.Name = changedEventArgs.OldValue as string;
              return;
            }
          }
          this.comboBox.Items[this.comboBox.SelectedIndex] = (object) selectedObject.Name;
          break;
        case "GroupQuery":
          if (!this.IsIdentifierOrEmpty(selectedObject.GroupQueryParam))
          {
            this.Host.NotificationDialog(Resources.SpecExplorer, "GroupQuery must be an identifier.");
            selectedObject.GroupQueryParam = changedEventArgs.OldValue as string;
            return;
          }
          break;
        case "HideQuery":
          if (!this.IsIdentifierOrEmpty(selectedObject.HideQueryParam))
          {
            this.Host.NotificationDialog(Resources.SpecExplorer, "HideQuery must be an identifier.");
            selectedObject.HideQueryParam = changedEventArgs.OldValue as string;
            return;
          }
          break;
        case "StateDescription":
          if (!this.IsIdentifierOrEmpty(selectedObject.StateDescriptionParam))
          {
            this.Host.NotificationDialog(Resources.SpecExplorer, "StateDescription must be an identifier.");
            selectedObject.StateDescriptionParam = changedEventArgs.OldValue as string;
            return;
          }
          break;
      }
      this.IsDirty = true;
    }

    private void InitializeUI()
    {
      this.comboBox.SelectedIndexChanged += new EventHandler(this.OnComboBoxSelect);
      this.propertyGrid.SelectedObjectsChanged += new EventHandler(this.OnSelectObjectChanged);
      this.propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(this.OnPropertyValueChanged);
    }

    private bool IsIdentifierOrEmpty(string name) => string.IsNullOrEmpty(name) || Regex.IsMatch(name, "^[a-zA-Z_][a-zA-Z0-9_]*(\\.[a-zA-Z_][a-zA-Z0-9_]*)*$");

    internal IHost Host { get; set; }

    public ViewDefinitionsControl()
    {
      this.InitializeComponent();
      this.InitializeUI();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.propertyGrid = new PropertyGrid();
      this.comboBox = new ComboBox();
      this.label = new Label();
      this.SuspendLayout();
      this.propertyGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.propertyGrid.Location = new Point(0, 43);
      this.propertyGrid.Name = "propertyGrid";
      this.propertyGrid.Size = new Size(469, 365);
      this.propertyGrid.TabIndex = 0;
      this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBox.FormattingEnabled = true;
      this.comboBox.Location = new Point(0, 16);
      this.comboBox.Name = "comboBox";
      this.comboBox.Size = new Size(121, 21);
      this.comboBox.TabIndex = 3;
      this.label.AutoSize = true;
      this.label.Location = new Point(0, 0);
      this.label.Margin = new Padding(0, 0, 3, 0);
      this.label.Name = "label";
      this.label.Size = new Size(80, 13);
      this.label.TabIndex = 4;
      this.label.Text = "View Definition:";
      this.label.TextAlign = ContentAlignment.MiddleLeft;
      this.AutoScaleMode = AutoScaleMode.Inherit;
      this.BackColor = SystemColors.ButtonFace;
      this.Controls.Add((Control) this.label);
      this.Controls.Add((Control) this.comboBox);
      this.Controls.Add((Control) this.propertyGrid);
      this.Name = nameof (ViewDefinitionsControl);
      this.Size = new Size(469, 408);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
