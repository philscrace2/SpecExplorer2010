// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.FindState
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.SpecExplorer.Viewer
{
  internal class FindState : UserControl
  {
    private const string HighlightAllString = "Highlight All";
    private const string ClearHighlightAllString = "New Search";
    private const string FindNextButtonTip = "Find the next matching state";
    private const string HighlightAllButtonTip = "Highlight all matching states";
    private const string CleanHightlightAllTip = "Start a new search from the initial state and clear highlighting";
    private const string NoMatchingStateMessage = "Spec Explorer has finished searching the graph, the matching state was not found";
    private const string EndMatchStateMessage = "Spec Explorer has finished searching the graph.";
    private bool cleanHighlightedAll;
    private IHost host;
    private FindStateSettings findStateSettings;
    private ToolTip buttonTipHelper;
    private IContainer components;
    private TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private TextBox findWhatTextBox;
    private ComboBox lookInComboBox;
    private Panel panel1;
    private Button findNextButton;
    private Button highlightButton;
    private CheckBox searchUpCheckBox;
    private CheckBox matchWholeWoldCheckBox;
    private CheckBox matchCaseCheckBox;

    public FindState()
    {
      this.InitializeComponent();
      this.buttonTipHelper = new ToolTip();
      this.buttonTipHelper.SetToolTip((Control) this.highlightButton, "Highlight all matching states");
      this.buttonTipHelper.SetToolTip((Control) this.findNextButton, "Find the next matching state");
      this.findWhatTextBox.Focus();
    }

    private void OnFindNextButtonClick(object sender, EventArgs e)
    {
      int currentNodeIndex1 = this.findStateSettings.CurrentNodeIndex;
      this.CleanHighlightedNodes();
      DisplayNode currentSelectedNode = this.GetCurrentSelectedNode();
      if (currentSelectedNode != null)
      {
        this.findStateSettings.CurrentHighlightedNodes = new Dictionary<Microsoft.Msagl.Drawing.Node, NodeAttr>();
        this.findStateSettings.CurrentHighlightedNodes.Add(currentSelectedNode.DrawingNode, currentSelectedNode.DrawingNode.Attr.Clone());
        this.HighlightNode(currentSelectedNode.DrawingNode);
        this.FitSelectedNodesToScreen(currentSelectedNode.DrawingNode);
        this.findStateSettings.CurrentGViewer.Invalidate();
        this.UpdateHighlightAllButton(false);
      }
      else
      {
        int currentNodeIndex2 = this.findStateSettings.CurrentNodeIndex;
        int count = this.findStateSettings.OrderedDisplayNode.Count;
        if (currentNodeIndex1 < count - 1 && currentNodeIndex2 == count || currentNodeIndex1 > 0 && currentNodeIndex2 == -1 || (currentNodeIndex2 < -1 || currentNodeIndex2 > count))
          this.findStateSettings.CurrentNodeIndex = currentNodeIndex1;
        this.host.NotificationDialog("Spec Explorer", "Spec Explorer has finished searching the graph, the matching state was not found");
      }
    }

    private void OnHighlightButtonClick(object sender, EventArgs e)
    {
      if (this.cleanHighlightedAll)
      {
        this.CleanHighlightedNodes();
        this.RestoreGViewerZoonFValue();
        this.findStateSettings.CurrentNodeIndex = -1;
        this.UpdateHighlightAllButton(true);
      }
      else
      {
        List<DisplayNode> currentSelectedNodes = this.GetCurrentSelectedNodes();
        if (currentSelectedNodes.Count > 0)
        {
          foreach (DisplayNode displayNode in currentSelectedNodes)
            this.HighlightNode(displayNode.DrawingNode);
          this.findStateSettings.CurrentGViewer.Invalidate();
          this.UpdateHighlightAllButton(false);
        }
        else
          this.host.NotificationDialog("Spec Explorer", "Spec Explorer has finished searching the graph, the matching state was not found");
      }
    }

    private StateFindScope CurrentStateFindScope => (StateFindScope) this.lookInComboBox.SelectedIndex;

    private List<DisplayNode> GetCurrentSelectedNodes()
    {
      List<DisplayNode> displayNodeList = new List<DisplayNode>();
      foreach (DisplayNode node in this.findStateSettings.OrderedDisplayNode)
      {
        if (node != null && this.ContainsSearchString(node, this.findWhatTextBox.Text.Trim(), this.CurrentStateFindScope, this.matchCaseCheckBox.Checked, this.matchWholeWoldCheckBox.Checked))
          displayNodeList.Add(node);
      }
      return displayNodeList;
    }

    private DisplayNode GetCurrentSelectedNode()
    {
      while (this.IsCurrentNodeIndexValid())
      {
        this.UpdateNodeIndex();
        if (this.findStateSettings.CurrentNodeIndex < 0 || this.findStateSettings.CurrentNodeIndex > this.findStateSettings.OrderedDisplayNode.Count - 1)
          return (DisplayNode) null;
        DisplayNode node = this.findStateSettings.OrderedDisplayNode[this.findStateSettings.CurrentNodeIndex];
        if (node != null && this.ContainsSearchString(node, this.findWhatTextBox.Text.Trim(), this.CurrentStateFindScope, this.matchCaseCheckBox.Checked, this.matchWholeWoldCheckBox.Checked))
          return node;
      }
      return (DisplayNode) null;
    }

    private void FitSelectedNodesToScreen(Microsoft.Msagl.Drawing.Node selectedNode)
    {
      GViewer currentGviewer = this.findStateSettings.CurrentGViewer;
      if (currentGviewer.ZoomF == 1.0)
      {
        int num1 = currentGviewer.Parent.Parent.Width * currentGviewer.Parent.Parent.Height / (int) selectedNode.BoundingBox.Area;
        int num2 = 350;
        if (num1 > num2)
          currentGviewer.ZoomF = (double) num1 / 140.0;
      }
      currentGviewer.CenterToPoint(selectedNode.BoundingBox.Center);
    }

    private void RestoreGViewerZoonFValue()
    {
      if (this.findStateSettings.CurrentGViewer.ZoomF == 1.0)
        return;
      this.findStateSettings.CurrentGViewer.ZoomF = 1.0;
    }

    private void UpdateNodeIndex()
    {
      if (!this.IsCurrentNodeIndexValid())
        return;
      if (this.searchUpCheckBox.Checked)
        --this.findStateSettings.CurrentNodeIndex;
      else
        ++this.findStateSettings.CurrentNodeIndex;
    }

    private void UpdateHighlightAllButton(bool highlight)
    {
      if (highlight)
      {
        this.highlightButton.Text = "Highlight All";
        this.cleanHighlightedAll = false;
        this.buttonTipHelper.SetToolTip((Control) this.highlightButton, "Highlight all matching states");
      }
      else
      {
        this.highlightButton.Text = "New Search";
        this.buttonTipHelper.SetToolTip((Control) this.highlightButton, "Start a new search from the initial state and clear highlighting");
        this.cleanHighlightedAll = true;
      }
    }

    private void HighlightNode(Microsoft.Msagl.Drawing.Node node)
    {
      if (this.findStateSettings.CurrentHighlightedNodes == null)
        this.findStateSettings.CurrentHighlightedNodes = new Dictionary<Microsoft.Msagl.Drawing.Node, NodeAttr>();
      this.findStateSettings.CurrentHighlightedNodes[node] = node.Attr.Clone();
      node.Attr.LineWidth = 3;
      node.Attr.AddStyle(Style.Bold);
      node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Yellow;
    }

    private void CleanHighlightedNodes()
    {
      Dictionary<Microsoft.Msagl.Drawing.Node, NodeAttr> highlightedNodes = this.findStateSettings.CurrentHighlightedNodes;
      if (highlightedNodes == null || highlightedNodes.Count <= 0)
        return;
      foreach (Microsoft.Msagl.Drawing.Node key in highlightedNodes.Keys)
      {
        key.Attr = highlightedNodes[key];
        key.Attr.LineWidth = 1;
      }
      highlightedNodes.Clear();
      this.findStateSettings.CurrentGViewer.Invalidate();
    }

    private bool IsCurrentNodeIndexValid() => this.searchUpCheckBox.Checked ? this.findStateSettings.CurrentNodeIndex >= -1 : this.findStateSettings.CurrentNodeIndex <= this.findStateSettings.OrderedDisplayNode.Count;

    private bool ContainsSearchString(
      DisplayNode node,
      string searchString,
      StateFindScope findScope,
      bool matchCase,
      bool matchWholeWord)
    {
      switch (findScope)
      {
        case StateFindScope.Error:
          if ((node.Label.Flags & ObjectModel.StateFlags.Error) == null)
            return false;
          break;
        case StateFindScope.NonAccepting:
          if ((node.Label.Flags & ObjectModel.StateFlags.NonAcceptingEnd) == null)
            return false;
          break;
        case StateFindScope.BoundHit:
          if ((node.Label.Flags & ObjectModel.StateFlags.BoundStopped) == null)
            return false;
          break;
      }
      return string.IsNullOrEmpty(searchString) || FindState.Compare(node.Text, searchString, matchCase, matchWholeWord);
    }

    private static bool Compare(
      string stringA,
      string stringB,
      bool matchCase,
      bool matchWholeWord)
    {
      return matchWholeWord ? string.Equals(stringA, stringB, matchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase) : stringA.IndexOf(stringB, matchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase) > -1;
    }

    internal void SetHost(IHost host) => this.host = host;

    internal void UpdateSettings(FindStateSettings settings)
    {
      if (settings == null)
        return;
      this.findStateSettings = settings;
      this.findStateSettings = settings;
      this.findWhatTextBox.Text = this.findStateSettings.SearchString ?? string.Empty;
      this.matchCaseCheckBox.Checked = this.findStateSettings.MatchCaseChecked;
      this.matchWholeWoldCheckBox.Checked = this.findStateSettings.MatchWholeWordChecked;
      this.searchUpCheckBox.Checked = this.findStateSettings.SearchUpChecked;
      this.lookInComboBox.SelectedIndex = this.findStateSettings.FindScopeIndex;
      if (this.findStateSettings.CurrentHighlightedNodes != null && this.findStateSettings.CurrentHighlightedNodes.Count > 0)
      {
        this.cleanHighlightedAll = true;
        this.UpdateHighlightAllButton(false);
      }
      else
      {
        this.cleanHighlightedAll = false;
        this.UpdateHighlightAllButton(true);
      }
    }

    internal void SaveSettings()
    {
      this.findStateSettings.SearchString = this.findWhatTextBox.Text.Trim();
      this.findStateSettings.MatchCaseChecked = this.matchCaseCheckBox.Checked;
      this.findStateSettings.MatchWholeWordChecked = this.matchWholeWoldCheckBox.Checked;
      this.findStateSettings.SearchUpChecked = this.searchUpCheckBox.Checked;
      this.findStateSettings.FindScopeIndex = this.lookInComboBox.SelectedIndex;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.tableLayoutPanel1 = new TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.findWhatTextBox = new TextBox();
      this.lookInComboBox = new ComboBox();
      this.panel1 = new Panel();
      this.findNextButton = new Button();
      this.highlightButton = new Button();
      this.searchUpCheckBox = new CheckBox();
      this.matchWholeWoldCheckBox = new CheckBox();
      this.matchCaseCheckBox = new CheckBox();
      this.tableLayoutPanel1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 63f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 167f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70f));
      this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230f));
      this.tableLayoutPanel1.Controls.Add((Control) this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.label2, 2, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.findWhatTextBox, 1, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.lookInComboBox, 3, 0);
      this.tableLayoutPanel1.Controls.Add((Control) this.panel1, 0, 1);
      this.tableLayoutPanel1.Location = new Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));
      this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 80f));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(530, 60);
      this.tableLayoutPanel1.TabIndex = 0;
      this.label1.Anchor = AnchorStyles.Left;
      this.label1.AutoSize = true;
      this.label1.Location = new Point(3, 7);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(53, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Find what";
      this.label2.Anchor = AnchorStyles.Right;
      this.label2.AutoSize = true;
      this.label2.Location = new Point((int) byte.MaxValue, 7);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(42, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Look in";
      this.findWhatTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
      this.findWhatTextBox.Location = new Point(66, 4);
      this.findWhatTextBox.Name = "findWhatTextBox";
      this.findWhatTextBox.Size = new System.Drawing.Size(161, 20);
      this.findWhatTextBox.TabIndex = 2;
      this.lookInComboBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
      this.lookInComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.lookInComboBox.FormattingEnabled = true;
      this.lookInComboBox.Items.AddRange(new object[4]
      {
        (object) "All states",
        (object) "Error states",
        (object) "Non-accepting end states",
        (object) "Bound hit states"
      });
      this.lookInComboBox.Location = new Point(303, 3);
      this.lookInComboBox.Name = "lookInComboBox";
      this.lookInComboBox.Size = new System.Drawing.Size(224, 21);
      this.lookInComboBox.TabIndex = 3;
      this.tableLayoutPanel1.SetColumnSpan((Control) this.panel1, 4);
      this.panel1.Controls.Add((Control) this.findNextButton);
      this.panel1.Controls.Add((Control) this.highlightButton);
      this.panel1.Controls.Add((Control) this.searchUpCheckBox);
      this.panel1.Controls.Add((Control) this.matchWholeWoldCheckBox);
      this.panel1.Controls.Add((Control) this.matchCaseCheckBox);
      this.panel1.Location = new Point(3, 31);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(524, 27);
      this.panel1.TabIndex = 4;
      this.findNextButton.Location = new Point(300, 1);
      this.findNextButton.Name = "findNextButton";
      this.findNextButton.Size = new System.Drawing.Size(95, 23);
      this.findNextButton.TabIndex = 3;
      this.findNextButton.Text = "Find Next";
      this.findNextButton.UseVisualStyleBackColor = true;
      this.findNextButton.Click += new EventHandler(this.OnFindNextButtonClick);
      this.highlightButton.Location = new Point(430, 1);
      this.highlightButton.Name = "highlightButton";
      this.highlightButton.Size = new System.Drawing.Size(95, 23);
      this.highlightButton.TabIndex = 4;
      this.highlightButton.Text = "Highlight All";
      this.highlightButton.UseVisualStyleBackColor = true;
      this.highlightButton.Click += new EventHandler(this.OnHighlightButtonClick);
      this.searchUpCheckBox.AutoSize = true;
      this.searchUpCheckBox.Location = new Point(214, 4);
      this.searchUpCheckBox.Name = "searchUpCheckBox";
      this.searchUpCheckBox.Size = new System.Drawing.Size(75, 17);
      this.searchUpCheckBox.TabIndex = 2;
      this.searchUpCheckBox.Text = "Search up";
      this.searchUpCheckBox.UseVisualStyleBackColor = true;
      this.matchWholeWoldCheckBox.AutoSize = true;
      this.matchWholeWoldCheckBox.Location = new Point(90, 4);
      this.matchWholeWoldCheckBox.Name = "matchWholeWoldCheckBox";
      this.matchWholeWoldCheckBox.Size = new System.Drawing.Size(113, 17);
      this.matchWholeWoldCheckBox.TabIndex = 1;
      this.matchWholeWoldCheckBox.Text = "Match whole word";
      this.matchWholeWoldCheckBox.UseVisualStyleBackColor = true;
      this.matchCaseCheckBox.AutoSize = true;
      this.matchCaseCheckBox.Location = new Point(4, 4);
      this.matchCaseCheckBox.Name = "matchCaseCheckBox";
      this.matchCaseCheckBox.Size = new System.Drawing.Size(83, 17);
      this.matchCaseCheckBox.TabIndex = 0;
      this.matchCaseCheckBox.Text = "Match Case";
      this.matchCaseCheckBox.UseVisualStyleBackColor = true;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.tableLayoutPanel1);
      this.Name = "NodeSearchControl";
      this.Size = new System.Drawing.Size(530, 60);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);
    }
  }
}
