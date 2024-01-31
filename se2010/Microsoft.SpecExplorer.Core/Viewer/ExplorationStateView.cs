using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt.UI;

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

		public ExplorationStateView()
		{
			InitializeComponent();
		}

		public void LoadStates(string fileName, IEnumerable<State> states, bool shouldDisplayLeftTree)
		{
			ExplorationResultLoader loader = null;
			SharedEntitySet sharedEntitySet = null;
			try
			{
				loader = new ExplorationResultLoader(fileName);
				sharedEntitySet = loader.LoadSharedEntities();
			}
			catch (ExplorationResultLoadingException ex)
			{
				Host.NotificationDialog(Resource.SpecExplorer, string.Format("Failed to load file {0}:\n{1}", fileName, ex.Message));
				return;
			}
			SuspendLayout();
			nodeLabelDict.Clear();
			treeViewLeft.Nodes.Clear();
			if (shouldDisplayLeftTree)
			{
				base.Controls.Clear();
				base.Controls.Add(splitContainer);
				foreach (State state2 in states)
				{
					TreeNode node = new TreeNode(state2.Label);
					stateLabelDict[state2.Label] = state2;
					treeViewLeft.Nodes.Add(node);
				}
				splitContainer.Panel1.Controls.Clear();
				treeViewLeft.Dock = DockStyle.Fill;
				splitContainer.Panel1.Controls.Add(treeViewLeft);
				treeViewLeft.AfterSelect += delegate(object sender, TreeViewEventArgs e)
				{
					State state = stateLabelDict[e.Node.Text];
					StateNodeInfomation value;
					if (!nodeLabelDict.TryGetValue(e.Node.Text, out value))
					{
						value = LoadStateInfo(state, loader);
						nodeLabelDict[value.StateLabel] = value;
					}
					sbc.LoadStates(sharedEntitySet, new StateNodeInfomation[1] { value });
					splitContainer.Panel2.Controls.Clear();
					splitContainer.Panel2.Controls.Add(sbc);
				};
				if (treeViewLeft.Nodes.Count > 0)
				{
					treeViewLeft.SelectedNode = treeViewLeft.Nodes[0];
				}
			}
			else
			{
				base.Controls.Clear();
				sbc.LoadStates(sharedEntitySet, new StateNodeInfomation[1] { LoadStateInfo(states.First(), loader) });
				base.Controls.Add(sbc);
			}
			ResumeLayout();
		}

		private StateNodeInfomation LoadStateInfo(State state, ExplorationResultLoader loader)
		{
			Node node = null;
			try
			{
				StateEntity stateEntity = loader.LoadState(state.Label);
				node = stateEntity.Content;
			}
			catch (ExplorationResultLoadingException ex)
			{
				Host.ProgressMessage(VerbosityLevel.Minimal, string.Format("Failed to load state content for state {0}. Exception: {1}", state.Label, ex.Message));
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Kind", state.Flags);
			dictionary.Add("Description", state.Description);
			return new StateNodeInfomation(node, state.Label, dictionary);
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
			treeViewLeft = new System.Windows.Forms.TreeView();
			splitContainer = new System.Windows.Forms.SplitContainer();
			sbc = new Microsoft.Xrt.UI.StateBrowserControl();
			splitContainer.SuspendLayout();
			SuspendLayout();
			treeViewLeft.Dock = System.Windows.Forms.DockStyle.Fill;
			treeViewLeft.HideSelection = false;
			treeViewLeft.LineColor = System.Drawing.Color.Empty;
			treeViewLeft.Location = new System.Drawing.Point(0, 0);
			treeViewLeft.Name = "treeViewLeft";
			treeViewLeft.Size = new System.Drawing.Size(293, 350);
			treeViewLeft.TabIndex = 0;
			splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer.Location = new System.Drawing.Point(0, 0);
			splitContainer.Name = "splitContainer";
			splitContainer.SplitterDistance = 5;
			splitContainer.TabIndex = 1;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			AutoSize = true;
			base.Name = "ExplorationStateView";
			base.Size = new System.Drawing.Size(486, 355);
			splitContainer.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
