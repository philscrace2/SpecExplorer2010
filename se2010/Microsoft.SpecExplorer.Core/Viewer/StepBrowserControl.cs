using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xrt.UI;

namespace Microsoft.SpecExplorer.Viewer
{
	public class StepBrowserControl : UserControl
	{
		private IContainer components;

		internal TreeGridView TreeGrid;

		private TreeGridColumn StepElement;

		private DataGridViewTextBoxColumn StateElementValue;

		public StepBrowserControl()
		{
			InitializeComponent();
			AddEventHandlers();
		}

		public void LoadSteps(IEnumerable<BrowserEdge> browserEdges)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			ExpansionStateUtils.CaptureExpansionState(dictionary, "", TreeGrid.Nodes);
			TreeGrid.Nodes.Clear();
			foreach (BrowserEdge browserEdge in browserEdges)
			{
				AddBrowserEdge(browserEdge);
			}
			if (dictionary != null && dictionary.Count > 0)
			{
				ExpansionStateUtils.RestoreExpansionState(dictionary, TreeGrid);
			}
		}

		private void AddEventHandlers()
		{
			int? adjustedRowHeight = null;
			TreeGrid.RowsAdded += delegate(object sender, DataGridViewRowsAddedEventArgs e)
			{
				for (int i = 0; i < e.RowCount; i++)
				{
					DataGridViewRow dataGridViewRow = TreeGrid.Rows[e.RowIndex + i];
					if (!adjustedRowHeight.HasValue)
					{
						adjustedRowHeight = dataGridViewRow.Height * 4 / 5;
					}
					if (dataGridViewRow.Height != adjustedRowHeight.Value)
					{
						dataGridViewRow.Height = adjustedRowHeight.Value;
					}
				}
			};
		}

		private void AddBrowserEdge(BrowserEdge bEdge)
		{
			string text = string.Format("from {0} via {1} to {2}", bEdge.Source.Label, bEdge.Text, bEdge.Target.Label);
			TreeGrid.Nodes.Add("Step Label", text);
			TreeGrid.Nodes.Add("Action", bEdge.ActionText);
			if (bEdge.CapturedRequirements.Length > 0 || bEdge.AssumeCapturedRequirements.Length > 0)
			{
				TreeGridNode treeGridNode = TreeGrid.Nodes.Add("Requirements");
				if (bEdge.CapturedRequirements.Length > 0)
				{
					int num = 0;
					TreeGridNode treeGridNode2 = treeGridNode.Nodes.Add("Ensure Captured");
					string[] capturedRequirements = bEdge.CapturedRequirements;
					foreach (string text2 in capturedRequirements)
					{
						treeGridNode2.Nodes.Add(string.Format("#{0}", num++), text2);
					}
				}
				if (bEdge.AssumeCapturedRequirements.Length > 0)
				{
					int num2 = 0;
					TreeGridNode treeGridNode3 = treeGridNode.Nodes.Add("Assume Captured");
					string[] assumeCapturedRequirements = bEdge.AssumeCapturedRequirements;
					foreach (string text3 in assumeCapturedRequirements)
					{
						treeGridNode3.Nodes.Add(string.Format("#{0}", num2++), text3);
					}
				}
			}
			if (bEdge.PostConstraints.Length > 0)
			{
				TreeGridNode treeGridNode4 = TreeGrid.Nodes.Add("Step Constraints");
				int num3 = 0;
				string[] postConstraints = bEdge.PostConstraints;
				foreach (string text4 in postConstraints)
				{
					treeGridNode4.Nodes.Add(string.Format("#{0}", num3++), text4);
				}
			}
			if (bEdge.PreConstraints.Length > 0)
			{
				TreeGridNode treeGridNode5 = TreeGrid.Nodes.Add("Preconditions");
				int num4 = 0;
				string[] preConstraints = bEdge.PreConstraints;
				foreach (string text5 in preConstraints)
				{
					treeGridNode5.Nodes.Add(string.Format("#{0}", num4++), text5);
				}
			}
			if (bEdge.unboundVariables.Length <= 0)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			string[] unboundVariables = bEdge.unboundVariables;
			foreach (string value in unboundVariables)
			{
				if (!flag)
				{
					stringBuilder.Append(", ");
				}
				else
				{
					flag = false;
				}
				stringBuilder.Append(value);
			}
			TreeGrid.Nodes.Add("Freed Variables", stringBuilder.ToString());
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
			TreeGrid = new Microsoft.Xrt.UI.TreeGridView();
			StepElement = new Microsoft.Xrt.UI.TreeGridColumn();
			StateElementValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)TreeGrid).BeginInit();
			SuspendLayout();
			TreeGrid.AllowUserToAddRows = false;
			TreeGrid.AllowUserToDeleteRows = false;
			TreeGrid.AllowUserToResizeRows = false;
			TreeGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			TreeGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
			TreeGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			TreeGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
			TreeGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			TreeGrid.Columns.AddRange(StepElement, StateElementValue);
			dataGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			dataGridViewCellStyle.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			TreeGrid.DefaultCellStyle = dataGridViewCellStyle;
			TreeGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			TreeGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			TreeGrid.GridColor = System.Drawing.SystemColors.Window;
			TreeGrid.ImageList = null;
			TreeGrid.Location = new System.Drawing.Point(0, 0);
			TreeGrid.Name = "TreeGrid";
			TreeGrid.ReadOnly = true;
			TreeGrid.RowHeadersVisible = false;
			TreeGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			TreeGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			TreeGrid.ShowCellErrors = false;
			TreeGrid.ShowEditingIcon = false;
			TreeGrid.ShowRowErrors = false;
			TreeGrid.Size = new System.Drawing.Size(983, 631);
			TreeGrid.TabIndex = 0;
			StepElement.DefaultNodeImage = null;
			StepElement.HeaderText = "Element";
			StepElement.Name = "StepElement";
			StepElement.ReadOnly = true;
			StepElement.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			StateElementValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			StateElementValue.FillWeight = 200f;
			StateElementValue.HeaderText = "Value";
			StateElementValue.Name = "StateElementValue";
			StateElementValue.ReadOnly = true;
			StateElementValue.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			StateElementValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(TreeGrid);
			base.Name = "StepBrowserControl";
			base.Size = new System.Drawing.Size(983, 631);
			((System.ComponentModel.ISupportInitialize)TreeGrid).EndInit();
			ResumeLayout(false);
		}
	}
}
