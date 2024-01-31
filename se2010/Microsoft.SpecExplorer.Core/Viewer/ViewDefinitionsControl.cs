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
			get
			{
				return viewDefinitions;
			}
			set
			{
				viewDefinitions = new List<IViewDefinition>();
				comboBox.Items.Clear();
				foreach (IViewDefinition item in value)
				{
					viewDefinitions.Add(item);
					comboBox.Items.Add(item.Name);
				}
				if (CurrentViewDefinition != null)
				{
					comboBox.SelectedText = CurrentViewDefinition.Name;
				}
				IsDirty = true;
			}
		}

		public IViewDefinition CurrentViewDefinition
		{
			get
			{
				return (IViewDefinition)propertyGrid.SelectedObject;
			}
			set
			{
				if (value != null)
				{
					comboBox.SelectedItem = value.Name;
					if (comboBox.SelectedIndex == -1)
					{
						comboBox.SelectedIndex = 0;
					}
				}
			}
		}

		public bool IsDirty { get; set; }

		internal IHost Host { get; set; }

		public void Add(IViewDefinition viewDefinition)
		{
			if (viewDefinitions.Find((IViewDefinition d) => d.Name == viewDefinition.Name) != null)
			{
				throw new InvalidOperationException("View definition with the same name already exists");
			}
			viewDefinitions.Add(viewDefinition);
			comboBox.Items.Add(viewDefinition.Name);
			IsDirty = true;
		}

		public void RemoveCurrentViewDefinition()
		{
			if (CurrentViewDefinition == null)
			{
				throw new InvalidOperationException("Current view definition is null");
			}
			viewDefinitions.Remove(CurrentViewDefinition);
			comboBox.Items.Remove(CurrentViewDefinition.Name);
			if (comboBox.Items.Count > 0)
			{
				comboBox.SelectedIndex = 0;
			}
			IsDirty = true;
		}

		private void OnSelectObjectChanged(object sender, EventArgs e)
		{
			ViewDefinition viewDefinition = (ViewDefinition)propertyGrid.SelectedObject;
			if (viewDefinition != null)
			{
				comboBox.SelectedItem = viewDefinition.Name;
			}
		}

		private void OnComboBoxSelect(object sender, EventArgs e)
		{
			propertyGrid.SelectedObject = viewDefinitions[comboBox.SelectedIndex];
			propertyGrid.Enabled = !viewDefinitions[comboBox.SelectedIndex].IsDefault;
		}

		private void OnPropertyValueChanged(object sender, EventArgs e)
		{
			PropertyValueChangedEventArgs propertyValueChangedEventArgs = e as PropertyValueChangedEventArgs;
			ViewDefinition viewDefinition = (ViewDefinition)propertyGrid.SelectedObject;
			switch (propertyValueChangedEventArgs.ChangedItem.Label)
			{
			case "Name":
				if (!Regex.IsMatch(viewDefinition.Name, "^([a-zA-Z][a-zA-Z0-9]{0,19})$"))
				{
					Host.NotificationDialog(Resource.SpecExplorer, "View name must use alphanumeric (with upper and lower case distinct), the first of which must alphabetic (upper or lower case) and maximum length is 20 characters.");
					viewDefinition.Name = propertyValueChangedEventArgs.OldValue as string;
					return;
				}
				foreach (IViewDefinition viewDefinition2 in viewDefinitions)
				{
					if (viewDefinition != viewDefinition2 && viewDefinition.Name.Equals(viewDefinition2.Name))
					{
						Host.NotificationDialog(Resource.SpecExplorer, "Duplicate view name.");
						viewDefinition.Name = propertyValueChangedEventArgs.OldValue as string;
						return;
					}
				}
				comboBox.Items[comboBox.SelectedIndex] = viewDefinition.Name;
				break;
			case "GroupQuery":
				if (!IsIdentifierOrEmpty(viewDefinition.GroupQueryParam))
				{
					Host.NotificationDialog(Resource.SpecExplorer, "GroupQuery must be an identifier.");
					viewDefinition.GroupQueryParam = propertyValueChangedEventArgs.OldValue as string;
					return;
				}
				break;
			case "HideQuery":
				if (!IsIdentifierOrEmpty(viewDefinition.HideQueryParam))
				{
					Host.NotificationDialog(Resource.SpecExplorer, "HideQuery must be an identifier.");
					viewDefinition.HideQueryParam = propertyValueChangedEventArgs.OldValue as string;
					return;
				}
				break;
			case "StateDescription":
				if (!IsIdentifierOrEmpty(viewDefinition.StateDescriptionParam))
				{
					Host.NotificationDialog(Resource.SpecExplorer, "StateDescription must be an identifier.");
					viewDefinition.StateDescriptionParam = propertyValueChangedEventArgs.OldValue as string;
					return;
				}
				break;
			}
			IsDirty = true;
		}

		private void InitializeUI()
		{
			comboBox.SelectedIndexChanged += OnComboBoxSelect;
			propertyGrid.SelectedObjectsChanged += OnSelectObjectChanged;
			propertyGrid.PropertyValueChanged += OnPropertyValueChanged;
		}

		private bool IsIdentifierOrEmpty(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				return Regex.IsMatch(name, "^[a-zA-Z_][a-zA-Z0-9_]*(\\.[a-zA-Z_][a-zA-Z0-9_]*)*$");
			}
			return true;
		}

		public ViewDefinitionsControl()
		{
			InitializeComponent();
			InitializeUI();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			propertyGrid = new System.Windows.Forms.PropertyGrid();
			comboBox = new System.Windows.Forms.ComboBox();
			label = new System.Windows.Forms.Label();
			SuspendLayout();
			propertyGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			propertyGrid.Location = new System.Drawing.Point(0, 43);
			propertyGrid.Name = "propertyGrid";
			propertyGrid.Size = new System.Drawing.Size(469, 365);
			propertyGrid.TabIndex = 0;
			comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBox.FormattingEnabled = true;
			comboBox.Location = new System.Drawing.Point(0, 16);
			comboBox.Name = "comboBox";
			comboBox.Size = new System.Drawing.Size(121, 21);
			comboBox.TabIndex = 3;
			label.AutoSize = true;
			label.Location = new System.Drawing.Point(0, 0);
			label.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			label.Name = "label";
			label.Size = new System.Drawing.Size(80, 13);
			label.TabIndex = 4;
			label.Text = "View Definition:";
			label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			BackColor = System.Drawing.SystemColors.ButtonFace;
			base.Controls.Add(label);
			base.Controls.Add(comboBox);
			base.Controls.Add(propertyGrid);
			base.Name = "ViewDefinitionsControl";
			base.Size = new System.Drawing.Size(469, 408);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
