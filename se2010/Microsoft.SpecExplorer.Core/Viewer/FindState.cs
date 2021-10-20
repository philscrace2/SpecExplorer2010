using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.SpecExplorer.ObjectModel;

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

		private StateFindScope CurrentStateFindScope
		{
			get
			{
				return (StateFindScope)lookInComboBox.SelectedIndex;
			}
		}

		public FindState()
		{
			InitializeComponent();
			buttonTipHelper = new ToolTip();
			buttonTipHelper.SetToolTip(highlightButton, "Highlight all matching states");
			buttonTipHelper.SetToolTip(findNextButton, "Find the next matching state");
			findWhatTextBox.Focus();
		}

		private void OnFindNextButtonClick(object sender, EventArgs e)
		{
			int currentNodeIndex = findStateSettings.CurrentNodeIndex;
			CleanHighlightedNodes();
			DisplayNode currentSelectedNode = GetCurrentSelectedNode();
			if (currentSelectedNode != null)
			{
				findStateSettings.CurrentHighlightedNodes = new Dictionary<Microsoft.Msagl.Drawing.Node, NodeAttr>();
				findStateSettings.CurrentHighlightedNodes.Add(currentSelectedNode.DrawingNode, currentSelectedNode.DrawingNode.Attr.Clone());
				HighlightNode(currentSelectedNode.DrawingNode);
				FitSelectedNodesToScreen(currentSelectedNode.DrawingNode);
				((Control)(object)findStateSettings.CurrentGViewer).Invalidate();
				UpdateHighlightAllButton(false);
			}
			else
			{
				int currentNodeIndex2 = findStateSettings.CurrentNodeIndex;
				int count = findStateSettings.OrderedDisplayNode.Count;
				if ((currentNodeIndex < count - 1 && currentNodeIndex2 == count) || (currentNodeIndex > 0 && currentNodeIndex2 == -1) || currentNodeIndex2 < -1 || currentNodeIndex2 > count)
				{
					findStateSettings.CurrentNodeIndex = currentNodeIndex;
				}
				host.NotificationDialog("Spec Explorer", "Spec Explorer has finished searching the graph, the matching state was not found");
			}
		}

		private void OnHighlightButtonClick(object sender, EventArgs e)
		{
			if (cleanHighlightedAll)
			{
				CleanHighlightedNodes();
				RestoreGViewerZoonFValue();
				findStateSettings.CurrentNodeIndex = -1;
				UpdateHighlightAllButton(true);
				return;
			}
			List<DisplayNode> currentSelectedNodes = GetCurrentSelectedNodes();
			if (currentSelectedNodes.Count > 0)
			{
				foreach (DisplayNode item in currentSelectedNodes)
				{
					HighlightNode(item.DrawingNode);
				}
				((Control)(object)findStateSettings.CurrentGViewer).Invalidate();
				UpdateHighlightAllButton(false);
			}
			else
			{
				host.NotificationDialog("Spec Explorer", "Spec Explorer has finished searching the graph, the matching state was not found");
			}
		}

		private List<DisplayNode> GetCurrentSelectedNodes()
		{
			List<DisplayNode> list = new List<DisplayNode>();
			foreach (DisplayNode item in findStateSettings.OrderedDisplayNode)
			{
				if (item != null && ContainsSearchString(item, findWhatTextBox.Text.Trim(), CurrentStateFindScope, matchCaseCheckBox.Checked, matchWholeWoldCheckBox.Checked))
				{
					list.Add(item);
				}
			}
			return list;
		}

		private DisplayNode GetCurrentSelectedNode()
		{
			while (IsCurrentNodeIndexValid())
			{
				UpdateNodeIndex();
				if (findStateSettings.CurrentNodeIndex < 0 || findStateSettings.CurrentNodeIndex > findStateSettings.OrderedDisplayNode.Count - 1)
				{
					return null;
				}
				DisplayNode displayNode = findStateSettings.OrderedDisplayNode[findStateSettings.CurrentNodeIndex];
				if (displayNode != null && ContainsSearchString(displayNode, findWhatTextBox.Text.Trim(), CurrentStateFindScope, matchCaseCheckBox.Checked, matchWholeWoldCheckBox.Checked))
				{
					return displayNode;
				}
			}
			return null;
		}

		private void FitSelectedNodesToScreen(Microsoft.Msagl.Drawing.Node selectedNode)
		{
			GViewer currentGViewer = findStateSettings.CurrentGViewer;
			if (currentGViewer.ZoomF == 1.0)
			{
				int num = ((Control)(object)currentGViewer).Parent.Parent.Width * ((Control)(object)currentGViewer).Parent.Parent.Height;
				double area = selectedNode.BoundingBox.Area;
				int num2 = num / (int)area;
				int num3 = 350;
				if (num2 > num3)
				{
					currentGViewer.ZoomF = (double)num2 / 140.0;
				}
			}
			currentGViewer.CenterToPoint(selectedNode.BoundingBox.Center);
		}

		private void RestoreGViewerZoonFValue()
		{
			if (findStateSettings.CurrentGViewer.ZoomF != 1.0)
			{
				findStateSettings.CurrentGViewer.ZoomF = 1.0;
			}
		}

		private void UpdateNodeIndex()
		{
			if (IsCurrentNodeIndexValid())
			{
				if (searchUpCheckBox.Checked)
				{
					findStateSettings.CurrentNodeIndex--;
				}
				else
				{
					findStateSettings.CurrentNodeIndex++;
				}
			}
		}

		private void UpdateHighlightAllButton(bool highlight)
		{
			if (highlight)
			{
				highlightButton.Text = "Highlight All";
				cleanHighlightedAll = false;
				buttonTipHelper.SetToolTip(highlightButton, "Highlight all matching states");
			}
			else
			{
				highlightButton.Text = "New Search";
				buttonTipHelper.SetToolTip(highlightButton, "Start a new search from the initial state and clear highlighting");
				cleanHighlightedAll = true;
			}
		}

		private void HighlightNode(Microsoft.Msagl.Drawing.Node node)
		{
			if (findStateSettings.CurrentHighlightedNodes == null)
			{
				findStateSettings.CurrentHighlightedNodes = new Dictionary<Microsoft.Msagl.Drawing.Node, NodeAttr>();
			}
			findStateSettings.CurrentHighlightedNodes[node] = node.Attr.Clone();
			node.Attr.LineWidth = 3;
			node.Attr.AddStyle(Style.Bold);
			node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Yellow;
		}

		private void CleanHighlightedNodes()
		{
			Dictionary<Microsoft.Msagl.Drawing.Node, NodeAttr> currentHighlightedNodes = findStateSettings.CurrentHighlightedNodes;
			if (currentHighlightedNodes == null || currentHighlightedNodes.Count <= 0)
			{
				return;
			}
			foreach (Microsoft.Msagl.Drawing.Node key in currentHighlightedNodes.Keys)
			{
				key.Attr = currentHighlightedNodes[key];
				key.Attr.LineWidth = 1;
			}
			currentHighlightedNodes.Clear();
			((Control)(object)findStateSettings.CurrentGViewer).Invalidate();
		}

		private bool IsCurrentNodeIndexValid()
		{
			if (searchUpCheckBox.Checked)
			{
				return findStateSettings.CurrentNodeIndex >= -1;
			}
			return findStateSettings.CurrentNodeIndex <= findStateSettings.OrderedDisplayNode.Count;
		}

		private bool ContainsSearchString(DisplayNode node, string searchString, StateFindScope findScope, bool matchCase, bool matchWholeWord)
		{
			switch (findScope)
			{
			case StateFindScope.Error:
				if ((node.Label.Flags & StateFlags.Error) == 0)
				{
					return false;
				}
				break;
			case StateFindScope.NonAccepting:
				if ((node.Label.Flags & StateFlags.NonAcceptingEnd) == 0)
				{
					return false;
				}
				break;
			case StateFindScope.BoundHit:
				if ((node.Label.Flags & StateFlags.BoundStopped) == 0)
				{
					return false;
				}
				break;
			}
			if (string.IsNullOrEmpty(searchString))
			{
				return true;
			}
			return Compare(node.Text, searchString, matchCase, matchWholeWord);
		}

		private static bool Compare(string stringA, string stringB, bool matchCase, bool matchWholeWord)
		{
			if (matchWholeWord)
			{
				return string.Equals(stringA, stringB, matchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
			}
			return stringA.IndexOf(stringB, matchCase ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase) > -1;
		}

		internal void SetHost(IHost host)
		{
			this.host = host;
		}

		internal void UpdateSettings(FindStateSettings settings)
		{
			if (settings != null)
			{
				findStateSettings = settings;
				findStateSettings = settings;
				findWhatTextBox.Text = findStateSettings.SearchString ?? string.Empty;
				matchCaseCheckBox.Checked = findStateSettings.MatchCaseChecked;
				matchWholeWoldCheckBox.Checked = findStateSettings.MatchWholeWordChecked;
				searchUpCheckBox.Checked = findStateSettings.SearchUpChecked;
				lookInComboBox.SelectedIndex = findStateSettings.FindScopeIndex;
				if (findStateSettings.CurrentHighlightedNodes != null && findStateSettings.CurrentHighlightedNodes.Count > 0)
				{
					cleanHighlightedAll = true;
					UpdateHighlightAllButton(false);
				}
				else
				{
					cleanHighlightedAll = false;
					UpdateHighlightAllButton(true);
				}
			}
		}

		internal void SaveSettings()
		{
			findStateSettings.SearchString = findWhatTextBox.Text.Trim();
			findStateSettings.MatchCaseChecked = matchCaseCheckBox.Checked;
			findStateSettings.MatchWholeWordChecked = matchWholeWoldCheckBox.Checked;
			findStateSettings.SearchUpChecked = searchUpCheckBox.Checked;
			findStateSettings.FindScopeIndex = lookInComboBox.SelectedIndex;
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
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			findWhatTextBox = new System.Windows.Forms.TextBox();
			lookInComboBox = new System.Windows.Forms.ComboBox();
			panel1 = new System.Windows.Forms.Panel();
			findNextButton = new System.Windows.Forms.Button();
			highlightButton = new System.Windows.Forms.Button();
			searchUpCheckBox = new System.Windows.Forms.CheckBox();
			matchWholeWoldCheckBox = new System.Windows.Forms.CheckBox();
			matchCaseCheckBox = new System.Windows.Forms.CheckBox();
			tableLayoutPanel1.SuspendLayout();
			panel1.SuspendLayout();
			SuspendLayout();
			tableLayoutPanel1.ColumnCount = 4;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 63f));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 167f));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70f));
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230f));
			tableLayoutPanel1.Controls.Add(label1, 0, 0);
			tableLayoutPanel1.Controls.Add(label2, 2, 0);
			tableLayoutPanel1.Controls.Add(findWhatTextBox, 1, 0);
			tableLayoutPanel1.Controls.Add(lookInComboBox, 3, 0);
			tableLayoutPanel1.Controls.Add(panel1, 0, 1);
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 2;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28f));
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80f));
			tableLayoutPanel1.Size = new System.Drawing.Size(530, 60);
			tableLayoutPanel1.TabIndex = 0;
			label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(3, 7);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(53, 13);
			label1.TabIndex = 0;
			label1.Text = "Find what";
			label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(255, 7);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(42, 13);
			label2.TabIndex = 1;
			label2.Text = "Look in";
			findWhatTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			findWhatTextBox.Location = new System.Drawing.Point(66, 4);
			findWhatTextBox.Name = "findWhatTextBox";
			findWhatTextBox.Size = new System.Drawing.Size(161, 20);
			findWhatTextBox.TabIndex = 2;
			lookInComboBox.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			lookInComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			lookInComboBox.FormattingEnabled = true;
			lookInComboBox.Items.AddRange(new object[4] { "All states", "Error states", "Non-accepting end states", "Bound hit states" });
			lookInComboBox.Location = new System.Drawing.Point(303, 3);
			lookInComboBox.Name = "lookInComboBox";
			lookInComboBox.Size = new System.Drawing.Size(224, 21);
			lookInComboBox.TabIndex = 3;
			tableLayoutPanel1.SetColumnSpan(panel1, 4);
			panel1.Controls.Add(findNextButton);
			panel1.Controls.Add(highlightButton);
			panel1.Controls.Add(searchUpCheckBox);
			panel1.Controls.Add(matchWholeWoldCheckBox);
			panel1.Controls.Add(matchCaseCheckBox);
			panel1.Location = new System.Drawing.Point(3, 31);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(524, 27);
			panel1.TabIndex = 4;
			findNextButton.Location = new System.Drawing.Point(300, 1);
			findNextButton.Name = "findNextButton";
			findNextButton.Size = new System.Drawing.Size(95, 23);
			findNextButton.TabIndex = 3;
			findNextButton.Text = "Find Next";
			findNextButton.UseVisualStyleBackColor = true;
			findNextButton.Click += new System.EventHandler(OnFindNextButtonClick);
			highlightButton.Location = new System.Drawing.Point(430, 1);
			highlightButton.Name = "highlightButton";
			highlightButton.Size = new System.Drawing.Size(95, 23);
			highlightButton.TabIndex = 4;
			highlightButton.Text = "Highlight All";
			highlightButton.UseVisualStyleBackColor = true;
			highlightButton.Click += new System.EventHandler(OnHighlightButtonClick);
			searchUpCheckBox.AutoSize = true;
			searchUpCheckBox.Location = new System.Drawing.Point(214, 4);
			searchUpCheckBox.Name = "searchUpCheckBox";
			searchUpCheckBox.Size = new System.Drawing.Size(75, 17);
			searchUpCheckBox.TabIndex = 2;
			searchUpCheckBox.Text = "Search up";
			searchUpCheckBox.UseVisualStyleBackColor = true;
			matchWholeWoldCheckBox.AutoSize = true;
			matchWholeWoldCheckBox.Location = new System.Drawing.Point(90, 4);
			matchWholeWoldCheckBox.Name = "matchWholeWoldCheckBox";
			matchWholeWoldCheckBox.Size = new System.Drawing.Size(113, 17);
			matchWholeWoldCheckBox.TabIndex = 1;
			matchWholeWoldCheckBox.Text = "Match whole word";
			matchWholeWoldCheckBox.UseVisualStyleBackColor = true;
			matchCaseCheckBox.AutoSize = true;
			matchCaseCheckBox.Location = new System.Drawing.Point(4, 4);
			matchCaseCheckBox.Name = "matchCaseCheckBox";
			matchCaseCheckBox.Size = new System.Drawing.Size(83, 17);
			matchCaseCheckBox.TabIndex = 0;
			matchCaseCheckBox.Text = "Match Case";
			matchCaseCheckBox.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(tableLayoutPanel1);
			base.Name = "NodeSearchControl";
			base.Size = new System.Drawing.Size(530, 60);
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			ResumeLayout(false);
		}
	}
}
