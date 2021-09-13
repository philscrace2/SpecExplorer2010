// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.StepBrowserControl
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.Xrt.UI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
      this.InitializeComponent();
      this.AddEventHandlers();
    }

    public void LoadSteps(IEnumerable<BrowserEdge> browserEdges)
    {
      Dictionary<string, bool> expansionState = new Dictionary<string, bool>();
      ExpansionStateUtils.CaptureExpansionState(expansionState, "", this.TreeGrid.Nodes);
      this.TreeGrid.Nodes.Clear();
      foreach (BrowserEdge browserEdge in browserEdges)
        this.AddBrowserEdge(browserEdge);
      if (expansionState == null || expansionState.Count <= 0)
        return;
      ExpansionStateUtils.RestoreExpansionState(expansionState, this.TreeGrid);
    }

    private void AddEventHandlers()
    {
      int? adjustedRowHeight = new int?();
      this.TreeGrid.RowsAdded += (DataGridViewRowsAddedEventHandler) ((sender, e) =>
      {
        for (int index = 0; index < e.RowCount; ++index)
        {
          DataGridViewRow row = this.TreeGrid.Rows[e.RowIndex + index];
          if (!adjustedRowHeight.HasValue)
            adjustedRowHeight = new int?(row.Height * 4 / 5);
          if (row.Height != adjustedRowHeight.Value)
            row.Height = adjustedRowHeight.Value;
        }
      });
    }

    private void AddBrowserEdge(BrowserEdge bEdge)
    {
      this.TreeGrid.Nodes.Add((object) "Step Label", (object) string.Format("from {0} via {1} to {2}", (object) bEdge.Source.Label, (object) bEdge.Text, (object) bEdge.Target.Label));
      this.TreeGrid.Nodes.Add((object) "Action", (object) bEdge.ActionText);
      if (bEdge.CapturedRequirements.Length > 0 || bEdge.AssumeCapturedRequirements.Length > 0)
      {
        TreeGridNode treeGridNode1 = this.TreeGrid.Nodes.Add("Requirements");
        if (bEdge.CapturedRequirements.Length > 0)
        {
          int num = 0;
          TreeGridNode treeGridNode2 = treeGridNode1.Nodes.Add("Ensure Captured");
          foreach (string capturedRequirement in bEdge.CapturedRequirements)
            treeGridNode2.Nodes.Add((object) string.Format("#{0}", (object) num++), (object) capturedRequirement);
        }
        if (bEdge.AssumeCapturedRequirements.Length > 0)
        {
          int num = 0;
          TreeGridNode treeGridNode2 = treeGridNode1.Nodes.Add("Assume Captured");
          foreach (string capturedRequirement in bEdge.AssumeCapturedRequirements)
            treeGridNode2.Nodes.Add((object) string.Format("#{0}", (object) num++), (object) capturedRequirement);
        }
      }
      if (bEdge.PostConstraints.Length > 0)
      {
        TreeGridNode treeGridNode = this.TreeGrid.Nodes.Add("Step Constraints");
        int num = 0;
        foreach (string postConstraint in bEdge.PostConstraints)
          treeGridNode.Nodes.Add((object) string.Format("#{0}", (object) num++), (object) postConstraint);
      }
      if (bEdge.PreConstraints.Length > 0)
      {
        TreeGridNode treeGridNode = this.TreeGrid.Nodes.Add("Preconditions");
        int num = 0;
        foreach (string preConstraint in bEdge.PreConstraints)
          treeGridNode.Nodes.Add((object) string.Format("#{0}", (object) num++), (object) preConstraint);
      }
      if (bEdge.unboundVariables.Length <= 0)
        return;
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      foreach (string unboundVariable in bEdge.unboundVariables)
      {
        if (!flag)
          stringBuilder.Append(", ");
        else
          flag = false;
        stringBuilder.Append(unboundVariable);
      }
      this.TreeGrid.Nodes.Add((object) "Freed Variables", (object) stringBuilder.ToString());
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      DataGridViewCellStyle gridViewCellStyle = new DataGridViewCellStyle();
      this.TreeGrid = new TreeGridView();
      this.StepElement = new TreeGridColumn();
      this.StateElementValue = new DataGridViewTextBoxColumn();
      ((ISupportInitialize) this.TreeGrid).BeginInit();
      this.SuspendLayout();
      this.TreeGrid.AllowUserToAddRows = false;
      this.TreeGrid.AllowUserToDeleteRows = false;
      this.TreeGrid.AllowUserToResizeRows = false;
      this.TreeGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
      this.TreeGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
      this.TreeGrid.BackgroundColor = SystemColors.Window;
      this.TreeGrid.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
      this.TreeGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      this.TreeGrid.Columns.AddRange((DataGridViewColumn) this.StepElement, (DataGridViewColumn) this.StateElementValue);
      gridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      gridViewCellStyle.BackColor = SystemColors.Window;
      gridViewCellStyle.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      gridViewCellStyle.ForeColor = SystemColors.ControlText;
      gridViewCellStyle.SelectionBackColor = SystemColors.Highlight;
      gridViewCellStyle.SelectionForeColor = SystemColors.HighlightText;
      gridViewCellStyle.WrapMode = DataGridViewTriState.False;
      this.TreeGrid.DefaultCellStyle = gridViewCellStyle;
      this.TreeGrid.Dock = DockStyle.Fill;
      this.TreeGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
      this.TreeGrid.GridColor = SystemColors.Window;
      this.TreeGrid.ImageList = (ImageList) null;
      this.TreeGrid.Location = new Point(0, 0);
      this.TreeGrid.Name = "TreeGrid";
      this.TreeGrid.ReadOnly = true;
      this.TreeGrid.RowHeadersVisible = false;
      this.TreeGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.TreeGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
      this.TreeGrid.ShowCellErrors = false;
      this.TreeGrid.ShowEditingIcon = false;
      this.TreeGrid.ShowRowErrors = false;
      this.TreeGrid.Size = new Size(983, 631);
      this.TreeGrid.TabIndex = 0;
      this.StepElement.DefaultNodeImage = (Image) null;
      this.StepElement.HeaderText = "Element";
      this.StepElement.Name = "StepElement";
      this.StepElement.ReadOnly = true;
      this.StepElement.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.StateElementValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      this.StateElementValue.FillWeight = 200f;
      this.StateElementValue.HeaderText = "Value";
      this.StateElementValue.Name = "StateElementValue";
      this.StateElementValue.ReadOnly = true;
      this.StateElementValue.Resizable = DataGridViewTriState.True;
      this.StateElementValue.SortMode = DataGridViewColumnSortMode.NotSortable;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.TreeGrid);
      this.Name = nameof (StepBrowserControl);
      this.Size = new Size(983, 631);
      ((ISupportInitialize) this.TreeGrid).EndInit();
      this.ResumeLayout(false);
    }
  }
}
