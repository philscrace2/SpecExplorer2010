// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.ExplorationStateView
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Microsoft.SpecExplorer.Viewer
{
  public class ExplorationStateView : UserControl
  {
    private StateBrowserControl sbc;
    private Dictionary<string, StateNodeInfomation> nodeLabelDict = new Dictionary<string, StateNodeInfomation>();
    private Dictionary<string, State> stateLabelDict = new Dictionary<string, State>();
    private IContainer components;
    internal TreeView treeViewLeft;
    private SplitContainer splitContainer;

    public IHost Host { get; set; }

    public ExplorationStateView() => this.InitializeComponent();

    public void LoadStates(string fileName, IEnumerable<State> states, bool shouldDisplayLeftTree)
    {
      ExplorationResultLoader loader = (ExplorationResultLoader) null;
      SharedEntitySet sharedEntitySet = (SharedEntitySet) null;
      try
      {
        loader = new ExplorationResultLoader(fileName);
        sharedEntitySet = loader.LoadSharedEntities();
      }
      catch (ExplorationResultLoadingException ex)
      {
        this.Host.NotificationDialog(Microsoft.SpecExplorer.Properties.Resources.SpecExplorer, string.Format("Failed to load file {0}:\n{1}", (object) fileName, (object) ((Exception) ex).Message));
        return;
      }
      this.SuspendLayout();
      this.nodeLabelDict.Clear();
      this.treeViewLeft.Nodes.Clear();
      if (shouldDisplayLeftTree)
      {
        this.Controls.Clear();
        this.Controls.Add((Control) this.splitContainer);
        foreach (State state in states)
        {
          System.Windows.Forms.TreeNode node = new System.Windows.Forms.TreeNode(state.Label);
          this.stateLabelDict[state.Label] = state;
          this.treeViewLeft.Nodes.Add(node);
        }
        this.splitContainer.Panel1.Controls.Clear();
        this.treeViewLeft.Dock = DockStyle.Fill;
        this.splitContainer.Panel1.Controls.Add((Control) this.treeViewLeft);
        this.treeViewLeft.AfterSelect += (TreeViewEventHandler) ((sender, e) =>
        {
          State state = this.stateLabelDict[e.Node.Text];
          StateNodeInfomation stateNodeInfomation;
          if (!this.nodeLabelDict.TryGetValue(e.Node.Text, out stateNodeInfomation))
          {
            stateNodeInfomation = this.LoadStateInfo(state, loader);
            this.nodeLabelDict[stateNodeInfomation.StateLabel] = stateNodeInfomation;
          }
          this.sbc.LoadStates(sharedEntitySet, (IEnumerable<StateNodeInfomation>) new StateNodeInfomation[1]
          {
            stateNodeInfomation
          });
          this.splitContainer.Panel2.Controls.Clear();
          this.splitContainer.Panel2.Controls.Add((Control) this.sbc);
        });
        if (this.treeViewLeft.Nodes.Count > 0)
          this.treeViewLeft.SelectedNode = this.treeViewLeft.Nodes[0];
      }
      else
      {
        this.Controls.Clear();
        this.sbc.LoadStates(sharedEntitySet, (IEnumerable<StateNodeInfomation>) new StateNodeInfomation[1]
        {
          this.LoadStateInfo(states.First<State>(), loader)
        });
        this.Controls.Add((Control) this.sbc);
      }
      this.ResumeLayout();
    }

    private StateNodeInfomation LoadStateInfo(
      State state,
      ExplorationResultLoader loader)
    {
      Node state1 = (Node) null;
      try
      {
        state1 = loader.LoadState(state.Label).Content;
      }
      catch (ExplorationResultLoadingException ex)
      {
        this.Host.ProgressMessage(VerbosityLevel.Minimal, string.Format("Failed to load state content for state {0}. Exception: {1}", (object) state.Label, (object) ((Exception) ex).Message));
      }
      return new StateNodeInfomation(state1, state.Label, (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "Kind",
          (object) state.Flags
        },
        {
          "Description",
          (object) state.Description
        }
      });
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.treeViewLeft = new TreeView();
      this.splitContainer = new SplitContainer();
      this.sbc = new StateBrowserControl();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      this.treeViewLeft.Dock = DockStyle.Fill;
      this.treeViewLeft.HideSelection = false;
      this.treeViewLeft.LineColor = Color.Empty;
      this.treeViewLeft.Location = new Point(0, 0);
      this.treeViewLeft.Name = "treeViewLeft";
      this.treeViewLeft.Size = new Size(293, 350);
      this.treeViewLeft.TabIndex = 0;
      this.splitContainer.Dock = DockStyle.Fill;
      this.splitContainer.Location = new Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.SplitterDistance = 5;
      this.splitContainer.TabIndex = 1;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoSize = true;
      this.Name = nameof (ExplorationStateView);
      this.Size = new Size(486, 355);
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}
