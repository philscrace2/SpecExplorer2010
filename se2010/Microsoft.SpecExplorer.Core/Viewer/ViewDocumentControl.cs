using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer.Viewer
{
	public class ViewDocumentControl : UserControl
	{
		private delegate void SetGViewerRenderingStatus(string str);

		private Dictionary<IViewDefinition, DisplayGraph> displayGraphs;

		private Dictionary<IViewDefinition, GViewer> viewers;

		private DisplayGraphBuilder displayGraphBuilder;

		private GViewerControlBuilder viewerBuilder;

		private int comboxSplitterIndex;

		private GViewer currentGViewer;

		private DisplayGraph currentDisplayGraph;

		private IViewDefinitionManager vdm;

		private IHost host;

		private ContextMenuStrip contextMenuStrip;

		private ToolTip toolTip;

		private Microsoft.Msagl.Drawing.Node selectingNode;

		private Microsoft.Msagl.Drawing.Node lastSelectedNode;

		private Microsoft.Msagl.Drawing.Node lastSelectionNode;

		private Edge lastSelectionEdge;

		private Microsoft.Msagl.Drawing.Color lastSelectionRestoreColor;

		private Edge lastSelectedEdge;

		private Microsoft.Msagl.Drawing.Color lastSelectedRestoreColor;

		public EventHandler FullScreen;

		private IEnumerable<string> recommendedViews;

		private int initialStateCount;

		private int stateCount;

		private int errorStateCount;

		private int nonAcceptingEndStateCount;

		private int boundHitStateCount;

		private int stepCount;

		private int requirementCount;

		private bool nodeSearchControlInialized;

		private Dictionary<IViewDefinition, FindStateSettings> cachedFindStateSettings;

		private IContainer components;

		private ToolStrip viewDocumentTool;

		internal ToolStripButton zoomIn;

		internal ToolStripButton zoomOut;

		internal ToolStripButton fullScreen;

		internal ToolStripButton manageViews;

		internal ToolStripButton saveImage;

		private ComboBox comboBox;

		private System.Windows.Forms.Label comboBoxLabel;

		private TextBox gViewerRenderingTextBox;

		private ToolStripButton fitToScreen;

		private StatusStrip statusStrip;

		private ToolStripStatusLabel toolStripStatusLabel;

		private ToolStripStatusLabel toolStripStatusLabel1;

		private ToolStripStatusLabel toolStripStatusLabel2;

		private ToolStripStatusLabel toolStripStatusLabel3;

		private ToolStripSplitButton modeSplitButton;

		private ToolStripMenuItem magnifyModeToolStripMenuItem;

		private ToolStripMenuItem moveModeToolStripMenuItem;

		private ToolStripMenuItem panModeToolStripMenuItem;

		private ToolStripButton findState;

		private Panel searchControlPanel;

		internal Panel displayPanel;

		private FindState findStateControl;

		public TransitionSystem TransitionSystem
		{
			get
			{
				return displayGraphBuilder.TransitionSystem;
			}
			set
			{
				displayPanel.Controls.Clear();
				displayGraphBuilder = new DisplayGraphBuilder(value);
				displayGraphs.Clear();
				foreach (GViewer value2 in viewers.Values)
				{
					if (value2 != null)
					{
						value2.Dispose();
					}
				}
				viewers.Clear();
				SetRecommendedViews();
				RefreshViewsAndViewComboBox();
				GetTransitionSystemStatus(out initialStateCount, out stateCount, out errorStateCount, out nonAcceptingEndStateCount, out boundHitStateCount, out stepCount, out requirementCount);
				SetDisplayRecommendedView();
				if (cachedFindStateSettings != null)
				{
					cachedFindStateSettings.Clear();
				}
			}
		}

		public event EventHandler<StatesBrowserEventArgs> BrowseStates;

		public event EventHandler<CompareStateEventArgs> CompareStates;

		public event EventHandler<StepBrowserEventArgs> BrowseStep;

		public event EventHandler InvokeViewDefinitionManager;

		public ViewDocumentControl(IViewDefinitionManager vdm, IHost host)
		{
			InitializeComponent();
			this.vdm = vdm;
			this.host = host;
			displayGraphs = new Dictionary<IViewDefinition, DisplayGraph>();
			viewers = new Dictionary<IViewDefinition, GViewer>();
			viewerBuilder = new GViewerControlBuilder();
			comboBox.SelectedIndexChanged += OnComboBoxChanged;
			comboBox.DropDown += OnComboBoxDropDown;
			zoomIn.Click += OnZoomInClick;
			zoomOut.Click += OnZoomOutClick;
			magnifyModeToolStripMenuItem.Click += OnMagnifyMode;
			moveModeToolStripMenuItem.Click += OnMoveMode;
			panModeToolStripMenuItem.Click += OnPanMode;
			saveImage.Click += OnSaveImageClick;
			fullScreen.Click += OnFullScreenClick;
			manageViews.Click += OnInvokeViewDefinitionManager;
			fitToScreen.Click += OnFitScreenClick;
			findState.Click += OnFindStateClick;
			contextMenuStrip = new ContextMenuStrip();
			contextMenuStrip.Items.Add("Compare with selected state");
			ContextMenuStrip obj = contextMenuStrip;
			CancelEventHandler value = delegate(object sender, CancelEventArgs e)
			{
				if (contextMenuStrip.Visible)
				{
					e.Cancel = true;
				}
				if (lastSelectedNode != null && selectingNode != null)
				{
					contextMenuStrip.Items[0].Enabled = lastSelectedNode.Attr.Id != selectingNode.Attr.Id;
				}
				else
				{
					contextMenuStrip.Items[0].Enabled = false;
				}
			};
			obj.Opening += value;
			contextMenuStrip.Click += delegate
			{
				if (lastSelectedNode != null && selectingNode != null && selectingNode.Attr.Id != lastSelectedNode.Attr.Id)
				{
					DisplayNode nodeById = currentDisplayGraph.GetNodeById(lastSelectedNode.Id);
					DisplayNode nodeById2 = currentDisplayGraph.GetNodeById(selectingNode.Id);
					if (nodeById.DisplayNodeKind == DisplayNodeKind.Hyper || nodeById2.DisplayNodeKind == DisplayNodeKind.Hyper)
					{
						this.host.NotificationDialog("Spec Explorer", "Can not compare hyper node");
					}
					else if (this.CompareStates != null)
					{
						this.CompareStates(this, new CompareStateEventArgs(nodeById.Label, nodeById2.Label));
					}
				}
			};
			toolTip = new ToolTip();
			vdm.ViewDefinitionUpdate += OnViewDefinitionUpdated;
		}

		private void OnGViewerClick(object sender, MouseEventArgs e)
		{
			if (currentGViewer.SelectedObject == null || e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				return;
			}
			if (lastSelectedNode != null && currentGViewer.SelectedObject != lastSelectedNode)
			{
				lastSelectedNode.Attr.Color = lastSelectedRestoreColor;
				lastSelectedNode = null;
			}
			if (lastSelectedEdge != null && currentGViewer.SelectedObject != lastSelectedEdge)
			{
				SetEdgeAndLabelColor(lastSelectedEdge, lastSelectedRestoreColor);
				lastSelectedEdge = null;
			}
			lastSelectionEdge = null;
			lastSelectionNode = null;
			if (currentGViewer.SelectedObject is Microsoft.Msagl.Drawing.Node)
			{
				Microsoft.Msagl.Drawing.Node node = currentGViewer.SelectedObject as Microsoft.Msagl.Drawing.Node;
				if (e.Button == System.Windows.Forms.MouseButtons.Left)
				{
					if (lastSelectedNode != currentGViewer.SelectedObject)
					{
						lastSelectedNode = node;
						lastSelectedRestoreColor = lastSelectionRestoreColor;
						node.Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
					}
					StatesBrowserEventArgs e2 = new StatesBrowserEventArgs(currentDisplayGraph.GetNodeById(node.Id));
					if (this.BrowseStates != null)
					{
						this.BrowseStates(this, e2);
					}
				}
				((Control)(object)currentGViewer).Invalidate();
			}
			else
			{
				if (!(currentGViewer.SelectedObject is Edge))
				{
					return;
				}
				Edge edge = currentGViewer.SelectedObject as Edge;
				if (edge.UserData == null)
				{
					lastSelectionEdge = edge;
					return;
				}
				if (lastSelectedEdge != currentGViewer.SelectedObject)
				{
					lastSelectedEdge = edge;
					lastSelectedRestoreColor = lastSelectionRestoreColor;
					SetEdgeAndLabelColor(edge, Microsoft.Msagl.Drawing.Color.Blue);
				}
				DisplayEdge edgeById = currentDisplayGraph.GetEdgeById(edge.UserData as string);
				List<BrowserEdge> list = new List<BrowserEdge>();
				switch (edgeById.displayEdgeKind)
				{
				case DisplayEdgeKind.Normal:
				case DisplayEdgeKind.Collapsed:
					list.Add(BuildBrowserEdge(edgeById));
					break;
				}
				if (this.BrowseStep != null && list.Count > 0)
				{
					this.BrowseStep(this, new StepBrowserEventArgs(list, edgeById.Text));
				}
			}
		}

		private BrowserEdge BuildBrowserEdge(DisplayEdge displayEdge)
		{
			switch (displayEdge.displayEdgeKind)
			{
			case DisplayEdgeKind.Collapsed:
			{
				List<string> list = new List<string>();
				list.AddRange(displayEdge.subEdges[0].Label.VariablesToUnbindKeys);
				list.AddRange(displayEdge.subEdges[1].Label.VariablesToUnbindKeys);
				return new BrowserEdge(displayEdge.Text, displayEdge.ActionText, displayEdge.Source.Label, displayEdge.Target.Label, TranslateConstraints(displayEdge.subEdges.Where((DisplayEdge edge) => edge.Kind == ActionSymbolKind.Call).FirstOrDefault().Label.PreConstraints), TranslateConstraints(displayEdge.subEdges.Where((DisplayEdge edge) => edge.Kind == ActionSymbolKind.Return).FirstOrDefault().Label.PostConstraints), list.ToArray(), displayEdge.CapturedRequirements.ToArray(), displayEdge.AssumeCapturedRequirements.ToArray());
			}
			case DisplayEdgeKind.Normal:
				return new BrowserEdge(displayEdge.Text, displayEdge.ActionText, displayEdge.Source.Label, displayEdge.Target.Label, TranslateConstraints(displayEdge.Label.PreConstraints), TranslateConstraints(displayEdge.Label.PostConstraints), displayEdge.Label.VariablesToUnbindKeys, displayEdge.CapturedRequirements.ToArray(), displayEdge.AssumeCapturedRequirements.ToArray());
			default:
				throw new InvalidOperationException("Can not create a browser edge from a hyper edge.");
			}
		}

		private string[] TranslateConstraints(IEnumerable<Constraint> constraints)
		{
			return constraints.Select((Constraint t) => t.Text).ToArray();
		}

		private void OnSelectionChanged(object sender, EventArgs args)
		{
			if (contextMenuStrip != null)
			{
				contextMenuStrip.Items[0].Enabled = false;
			}
			if (lastSelectionNode != null)
			{
				lastSelectionNode.Attr.Color = lastSelectionRestoreColor;
				lastSelectionNode = null;
			}
			if (lastSelectionEdge != null)
			{
				SetEdgeAndLabelColor(lastSelectionEdge, lastSelectionRestoreColor);
				lastSelectionEdge = null;
			}
			if (currentGViewer.SelectedObject is Microsoft.Msagl.Drawing.Node)
			{
				Microsoft.Msagl.Drawing.Node node = currentGViewer.SelectedObject as Microsoft.Msagl.Drawing.Node;
				lastSelectionRestoreColor = node.Attr.Color;
				node.Attr.Color = Microsoft.Msagl.Drawing.Color.Magenta;
				if (((Control)(object)currentGViewer).ContextMenuStrip != null)
				{
					((Control)(object)currentGViewer).ContextMenuStrip.Items[0].Enabled = true;
				}
				lastSelectionNode = node;
				toolTip.SetToolTip((Control)(object)currentGViewer.DrawingPanel, GetStateDisplay(node.Id));
				selectingNode = node;
			}
			else if (currentGViewer.SelectedObject is Edge)
			{
				Edge edge = currentGViewer.SelectedObject as Edge;
				lastSelectionRestoreColor = edge.Attr.Color;
				SetEdgeAndLabelColor(edge, Microsoft.Msagl.Drawing.Color.Red);
				lastSelectionEdge = edge;
				selectingNode = null;
			}
			else
			{
				selectingNode = null;
			}
			((Control)(object)currentGViewer).Invalidate();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (Control.ModifierKeys == Keys.Control)
			{
				if (e.X >= 0 && e.X <= base.ClientRectangle.Width && e.Y >= 0 && e.Y <= base.ClientRectangle.Height && currentGViewer != null)
				{
					if (e.Delta < 0)
					{
						OnZoomOutClick(this, e);
					}
					else
					{
						OnZoomInClick(this, e);
					}
				}
			}
			else if (currentGViewer != null)
			{
				currentGViewer.Pan(0f, e.Delta);
			}
			base.OnMouseWheel(e);
		}

		private void OnComboBoxChanged(object sender, EventArgs e)
		{
			if (displayGraphBuilder == null)
			{
				throw new InvalidOperationException("Can not display graph before set transition system");
			}
			if (comboBox.SelectedIndex == comboxSplitterIndex)
			{
				comboBox.SelectedIndex = 0;
				return;
			}
			string text = (string)comboBox.SelectedItem;
			if (text == null)
			{
				return;
			}
			SaveFindStateFormSettings();
			IViewDefinition viewDefinition;
			vdm.TryGetViewDefinition(text, out viewDefinition);
			ViewDefinition viewDefinition2 = viewDefinition as ViewDefinition;
			vdm.CurrentView = viewDefinition2;
			if (currentGViewer != null)
			{
				currentGViewer.AbortAsyncLayout();
			}
			ResetGviewer();
			displayPanel.Controls.Add(gViewerRenderingTextBox);
			if (!displayGraphs.TryGetValue(viewDefinition, out currentDisplayGraph))
			{
				try
				{
					currentDisplayGraph = displayGraphBuilder.BuildDisplayGraph(viewDefinition2);
					displayGraphs[viewDefinition] = currentDisplayGraph;
				}
				catch (QueryException ex)
				{
					gViewerRenderingTextBox.Text = ex.Message;
					return;
				}
			}
			if (!viewers.TryGetValue(viewDefinition, out currentGViewer))
			{
				currentGViewer = viewerBuilder.BuildGViewerControl(viewDefinition2.RenderingTimeOut);
			}
			SetGViewer();
			RenderingAndSetGViewer(text);
			DisplayStatus();
		}

		private void OnComboBoxDropDown(object sender, EventArgs e)
		{
			if (displayGraphBuilder == null)
			{
				throw new InvalidOperationException("Can not display graph before set transition system");
			}
			string selectedItem = comboBox.SelectedItem as string;
			comboBox.SelectedIndexChanged -= OnComboBoxChanged;
			RefreshViewsAndViewComboBox();
			comboBox.SelectedItem = selectedItem;
			comboBox.SelectedIndexChanged += OnComboBoxChanged;
			if (comboBox.SelectedIndex < 0)
			{
				comboBox.SelectedIndex = 0;
			}
		}

		private void OnZoomInClick(object sender, EventArgs e)
		{
			if (currentGViewer != null)
			{
				currentGViewer.ZoomInPressed();
			}
		}

		private void OnZoomOutClick(object sender, EventArgs e)
		{
			if (currentGViewer != null)
			{
				currentGViewer.ZoomOutPressed();
			}
		}

		private void OnMagnifyMode(object sender, EventArgs e)
		{
			if (currentGViewer != null)
			{
				magnifyModeToolStripMenuItem.Checked = true;
				moveModeToolStripMenuItem.Checked = false;
				panModeToolStripMenuItem.Checked = false;
				currentGViewer.WindowZoomButtonPressed = true;
				currentGViewer.PanButtonPressed = false;
			}
		}

		private void OnMoveMode(object sender, EventArgs e)
		{
			if (currentGViewer != null)
			{
				magnifyModeToolStripMenuItem.Checked = false;
				moveModeToolStripMenuItem.Checked = true;
				panModeToolStripMenuItem.Checked = false;
				currentGViewer.WindowZoomButtonPressed = false;
				currentGViewer.PanButtonPressed = false;
			}
		}

		private void OnPanMode(object sender, EventArgs e)
		{
			if (currentGViewer != null)
			{
				magnifyModeToolStripMenuItem.Checked = false;
				moveModeToolStripMenuItem.Checked = false;
				panModeToolStripMenuItem.Checked = true;
				currentGViewer.WindowZoomButtonPressed = false;
				currentGViewer.PanButtonPressed = true;
			}
		}

		private void OnSaveImageClick(object sender, EventArgs e)
		{
			if (currentGViewer != null)
			{
				currentGViewer.SaveButtonPressed();
			}
		}

		private void OnFullScreenClick(object sender, EventArgs e)
		{
			if (FullScreen != null)
			{
				FullScreen(sender, e);
			}
		}

		private void OnInvokeViewDefinitionManager(object sender, EventArgs e)
		{
			this.InvokeViewDefinitionManager(sender, new EventArgs());
		}

		private void OnFitScreenClick(object sender, EventArgs e)
		{
			if (currentGViewer != null)
			{
				currentGViewer.ZoomF = 1.0;
			}
		}

		private void OnFindStateClick(object sender, EventArgs e)
		{
			if (!searchControlPanel.Visible)
			{
				if (!nodeSearchControlInialized)
				{
					FindStateSettings findStateSettings = GetFindStateSettings(vdm.CurrentView);
					findStateControl.UpdateSettings(findStateSettings);
					findStateControl.SetHost(host);
					nodeSearchControlInialized = true;
				}
				searchControlPanel.Visible = true;
			}
			else
			{
				searchControlPanel.Visible = false;
			}
		}

		private void OnViewDefinitionUpdated(object sender, ViewDefinitionUpdateEventArgs args)
		{
			foreach (IViewDefinition updatedViewDefinition in args.UpdatedViewDefinitions)
			{
				displayGraphs.Remove(updatedViewDefinition);
				GViewer value;
				if (viewers.TryGetValue(updatedViewDefinition, out value) && value != null)
				{
					viewers.Remove(updatedViewDefinition);
					if (value == currentGViewer)
					{
						lastSelectedEdge = null;
						lastSelectedNode = null;
						lastSelectionEdge = null;
						lastSelectionNode = null;
					}
					value.Dispose();
				}
			}
			RefreshViewsAndViewComboBox();
			comboBox.SelectedItem = vdm.CurrentView.Name;
		}

		private void RefreshViewsAndViewComboBox()
		{
			comboBox.Items.Clear();
			foreach (IViewDefinition defaultView in vdm.DefaultViews)
			{
				comboBox.Items.Add(defaultView.Name);
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (IViewDefinition customizedView in vdm.CustomizedViews)
			{
				hashSet.Add(customizedView.Name);
			}
			foreach (string recommendedView in recommendedViews)
			{
				if (hashSet.Contains(recommendedView))
				{
					comboBox.Items.Add(recommendedView);
					hashSet.Remove(recommendedView);
				}
			}
			if (hashSet.Count > 0)
			{
				comboxSplitterIndex = comboBox.Items.Add("------------------------");
				comboBox.Items.AddRange(hashSet.OrderBy((string item) => item).ToArray());
			}
			else
			{
				comboxSplitterIndex = comboBox.Items.Count;
			}
		}

		private void SetRecommendedViews()
		{
			string @switch = TransitionSystem.GetSwitch("RecommendedViews");
			recommendedViews = @switch.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (recommendedViews.Count() <= 0)
			{
				return;
			}
			List<string> list = new List<string>();
			foreach (string recommendedView in recommendedViews)
			{
				list.Add(recommendedView.Trim());
			}
			recommendedViews = list;
		}

		private void SetDisplayRecommendedView()
		{
			int selectedIndex = comboBox.SelectedIndex;
			if (selectedIndex <= 0)
			{
				foreach (string recommendedView in recommendedViews)
				{
					comboBox.SelectedItem = recommendedView;
					if (comboBox.SelectedIndex != -1)
					{
						break;
					}
				}
				if (comboBox.SelectedIndex == -1)
				{
					comboBox.SelectedIndex = 0;
				}
			}
			else
			{
				comboBox.SelectedIndex = selectedIndex;
			}
		}

		private void SetGViewer()
		{
			if (currentGViewer != null)
			{
				((Control)(object)currentGViewer).Dock = DockStyle.Fill;
				currentGViewer.SetContextMenumStrip((ContextMenuStrip)(object)contextMenuStrip);
				((Control)(object)currentGViewer).MouseClick += OnGViewerClick;
				currentGViewer.ObjectUnderMouseCursorChanged += (EventHandler<ObjectUnderMouseCursorChangedEventArgs>)(object)new EventHandler<ObjectUnderMouseCursorChangedEventArgs>(OnSelectionChanged);
				if (currentGViewer.PanButtonPressed)
				{
					OnPanMode(this, new EventArgs());
				}
				else if (currentGViewer.WindowZoomButtonPressed)
				{
					OnMagnifyMode(this, new EventArgs());
				}
				else
				{
					OnMoveMode(this, new EventArgs());
				}
				lastSelectionEdge = null;
				lastSelectionNode = null;
			}
		}

		private void DisplayGviewer()
		{
			displayPanel.Controls.Remove(gViewerRenderingTextBox);
			displayPanel.Controls.Add((Control)(object)currentGViewer);
			((Control)(object)currentGViewer).Focus();
		}

		private void ResetGviewer()
		{
			if (currentGViewer != null)
			{
				displayPanel.Controls.Remove((Control)(object)currentGViewer);
				((Control)(object)currentGViewer).MouseClick -= OnGViewerClick;
				currentGViewer.ObjectUnderMouseCursorChanged -= (EventHandler<ObjectUnderMouseCursorChangedEventArgs>)(object)new EventHandler<ObjectUnderMouseCursorChangedEventArgs>(OnSelectionChanged);
				currentGViewer.PanButtonPressed = false;
				if (lastSelectionEdge != null)
				{
					SetEdgeAndLabelColor(lastSelectionEdge, lastSelectionRestoreColor);
				}
				if (lastSelectionNode != null)
				{
					lastSelectionNode.Attr.Color = lastSelectionRestoreColor;
				}
			}
		}

		private static void SetEdgeAndLabelColor(Edge edge, Microsoft.Msagl.Drawing.Color color)
		{
			edge.Attr.Color = color;
			if (edge.Label != null)
			{
				edge.Label.FontColor = color;
			}
		}

		private void SetGViewerRenderingText(string str)
		{
			gViewerRenderingTextBox.Text = str;
		}

		private void RenderingAndSetGViewer(string viewName)
		{
			if (currentGViewer.Graph == null)
			{
				IViewDefinition ivd;
				vdm.TryGetViewDefinition(viewName, out ivd);
				if (currentGViewer.AsyncLayout)
				{
					currentGViewer.AsyncLayoutProgress += (EventHandler<LayoutProgressEventArgs>)(object)(EventHandler<LayoutProgressEventArgs>)delegate(object s, LayoutProgressEventArgs args)
					{
						if (base.Created)
						{
							SetGViewerRenderingStatus method = SetGViewerRenderingText;
							string text = "";
							switch (args.Progress)
							{
							case LayoutProgress.Aborted:
								if (string.IsNullOrEmpty(args.Diagnostics))
								{
									text = string.Format("Rendering aborted after {0} seconds. Increase timeout (view definition parameter Timeout) if you want to wait longer.", ivd.RenderingTimeOut.ToString());
									BeginInvoke(method, text);
								}
								else
								{
									BeginInvoke(method, args.Diagnostics);
								}
								ResetGviewer();
								break;
							case LayoutProgress.Finished:
								viewers[ivd] = currentGViewer;
								DisplayGviewer();
								UpdateFindStateFormSettings();
								break;
							case LayoutProgress.LayingOut:
							case LayoutProgress.Rendering:
								text = "Rendering ...";
								BeginInvoke(method, text);
								break;
							}
						}
					};
				}
				else
				{
					viewers[ivd] = currentGViewer;
					DisplayGviewer();
					UpdateFindStateFormSettings();
				}
				currentGViewer.Graph = viewerBuilder.BuildControlGraph(currentDisplayGraph);
			}
			else
			{
				DisplayGviewer();
				UpdateFindStateFormSettings();
			}
		}

		private string GetStateDisplay(string nodeId)
		{
			DisplayNode nodeById = currentDisplayGraph.GetNodeById(nodeId);
			if (nodeById != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if ((nodeById.StateFlags & StateFlags.PathDepthBoundStopped) != 0)
				{
					stringBuilder.Append("[path depth bound hit]");
				}
				if ((nodeById.StateFlags & StateFlags.StateBoundStopped) != 0)
				{
					stringBuilder.Append("[state bound hit]");
				}
				if ((nodeById.StateFlags & StateFlags.StepBoundStopped) != 0)
				{
					stringBuilder.Append("[step bound hit]");
				}
				if ((nodeById.StateFlags & StateFlags.ExplorationErrorBoundStopped) != 0)
				{
					stringBuilder.Append("[exploration error bound hit]");
				}
				if ((nodeById.StateFlags & StateFlags.StepsPerStateBoundStopped) != 0)
				{
					stringBuilder.Append("[steps per state bound hit]");
				}
				if ((nodeById.StateFlags & StateFlags.UserStopped) != 0)
				{
					stringBuilder.Append("[user stopped]");
				}
				if ((nodeById.StateFlags & StateFlags.NonAcceptingEnd) != 0)
				{
					stringBuilder.Append("[non-accepting end]");
				}
				if ((nodeById.StateFlags & StateFlags.Error) != 0)
				{
					switch (nodeById.DisplayNodeKind)
					{
					case DisplayNodeKind.Normal:
						stringBuilder.Append(nodeById.Label.Description);
						break;
					case DisplayNodeKind.Hyper:
						foreach (State leafNodeState in nodeById.LeafNodeStates)
						{
							stringBuilder.Append(leafNodeState.Description);
							if (leafNodeState != nodeById.LeafNodeStates.Last())
							{
								stringBuilder.Append(", ");
							}
						}
						break;
					}
				}
				return stringBuilder.ToString();
			}
			return null;
		}

		private void GetCurrentDisplayGraphStatus(out int displayInitialStateCount, out int displayStateCount, out int displayStepCount, out int displayRequirementCount, out int displayBoundCount, out string boundKind, out int displayErrorCount)
		{
			displayInitialStateCount = 0;
			displayStateCount = 0;
			displayStepCount = 0;
			displayRequirementCount = 0;
			displayBoundCount = 0;
			boundKind = "";
			displayErrorCount = 0;
			if (currentDisplayGraph == null)
			{
				return;
			}
			displayInitialStateCount = currentDisplayGraph.StartNodes.Count();
			displayStateCount = currentDisplayGraph.Nodes.Count();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (DisplayEdge edge in currentDisplayGraph.Edges)
			{
				displayStepCount++;
				foreach (string displayEdgeRequirement in GetDisplayEdgeRequirements(edge))
				{
					hashSet.Add(displayEdgeRequirement);
				}
			}
			ViewDefinition viewDefinition = vdm.CurrentView as ViewDefinition;
			if (viewDefinition.DisplayRequirements)
			{
				displayRequirementCount = hashSet.Count;
			}
			else
			{
				displayRequirementCount = 0;
			}
			StateFlags stateFlags = StateFlags.None;
			foreach (DisplayNode node in currentDisplayGraph.Nodes)
			{
				if ((node.StateFlags & StateFlags.Error) != 0)
				{
					displayErrorCount++;
				}
				if ((node.StateFlags & StateFlags.BoundStopped) != 0)
				{
					displayBoundCount++;
					stateFlags |= node.StateFlags;
				}
			}
			if (stateFlags != 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("[");
				if ((stateFlags & StateFlags.StateBoundStopped) != 0)
				{
					stringBuilder.Append("State, ");
				}
				if ((stateFlags & StateFlags.StepBoundStopped) != 0)
				{
					stringBuilder.Append("Step, ");
				}
				if ((stateFlags & StateFlags.PathDepthBoundStopped) != 0)
				{
					stringBuilder.Append("Path depth, ");
				}
				if ((stateFlags & StateFlags.StepsPerStateBoundStopped) != 0)
				{
					stringBuilder.Append("Steps per state, ");
				}
				if ((stateFlags & StateFlags.ExplorationErrorBoundStopped) != 0)
				{
					stringBuilder.Append("Exploration error, ");
				}
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
				stringBuilder.Append("]");
				boundKind = stringBuilder.ToString();
			}
		}

		private IEnumerable<string> GetDisplayEdgeRequirements(DisplayEdge displayEdge)
		{
			switch (displayEdge.displayEdgeKind)
			{
			case DisplayEdgeKind.Normal:
				try
				{
					string[] capturedRequirements = displayEdge.Label.CapturedRequirements;
					for (int i = 0; i < capturedRequirements.Length; i++)
					{
						yield return capturedRequirements[i];
					}
				}
				finally
				{
				}
				try
				{
					string[] assumeCapturedRequirements = displayEdge.Label.AssumeCapturedRequirements;
					for (int j = 0; j < assumeCapturedRequirements.Length; j++)
					{
						yield return assumeCapturedRequirements[j];
					}
				}
				finally
				{
				}
				break;
			case DisplayEdgeKind.Hyper:
			case DisplayEdgeKind.Collapsed:
				foreach (DisplayEdge de in displayEdge.subEdges)
				{
					foreach (string displayEdgeRequirement in GetDisplayEdgeRequirements(de))
					{
						yield return displayEdgeRequirement;
					}
				}
				break;
			}
		}

		public void GetTransitionSystemStatus(out int initialStateCount, out int stateCount, out int errorStateCount, out int nonAcceptingEndStateCount, out int boundHitStateCount, out int stepCount, out int requirementCount)
		{
			stateCount = 0;
			errorStateCount = 0;
			nonAcceptingEndStateCount = 0;
			boundHitStateCount = 0;
			State[] states = TransitionSystem.States;
			foreach (State state in states)
			{
				if (!state.IsVirtual && (state.RepresentativeState == null || state.RelationKind != StateRelationKind.Equivalent))
				{
					stateCount++;
					if ((state.Flags & StateFlags.Error) != 0)
					{
						errorStateCount++;
					}
					if ((state.Flags & StateFlags.NonAcceptingEnd) != 0)
					{
						nonAcceptingEndStateCount++;
					}
					if ((state.Flags & StateFlags.BoundStopped) != 0)
					{
						boundHitStateCount++;
					}
				}
			}
			initialStateCount = TransitionSystem.InitialStates.Length;
			stepCount = 0;
			HashSet<string> hashSet = new HashSet<string>();
			Transition[] transitions = TransitionSystem.Transitions;
			foreach (Transition transition in transitions)
			{
				stepCount++;
				string[] capturedRequirements = transition.CapturedRequirements;
				foreach (string item in capturedRequirements)
				{
					hashSet.Add(item);
				}
				string[] assumeCapturedRequirements = transition.AssumeCapturedRequirements;
				foreach (string item2 in assumeCapturedRequirements)
				{
					hashSet.Add(item2);
				}
			}
			requirementCount = hashSet.Count;
		}

		private void DisplayStatus()
		{
			int displayInitialStateCount;
			int displayStateCount;
			int displayStepCount;
			int displayRequirementCount;
			int displayBoundCount;
			string boundKind;
			int displayErrorCount;
			GetCurrentDisplayGraphStatus(out displayInitialStateCount, out displayStateCount, out displayStepCount, out displayRequirementCount, out displayBoundCount, out boundKind, out displayErrorCount);
			toolStripStatusLabel.Text = string.Format("Initial State(s): {0}/{1}   |   States:  {2}/{3}   |   Steps: {4}/{5}   |    Requirements: {6}/{7}   |", displayInitialStateCount, initialStateCount, displayStateCount, stateCount, displayStepCount, stepCount, displayRequirementCount, requirementCount);
			toolStripStatusLabel1.Text = string.Format("Bounds:  {0}/{1} {2}", displayBoundCount, boundHitStateCount, boundKind);
			System.Drawing.Color backColor = toolStripStatusLabel.BackColor;
			if (boundHitStateCount > 0)
			{
				toolStripStatusLabel1.BackColor = System.Drawing.Color.Orange;
			}
			else
			{
				toolStripStatusLabel1.BackColor = backColor;
			}
			toolStripStatusLabel2.Text = "|";
			toolStripStatusLabel3.Text = string.Format("Errors:  {0}/{1}", displayErrorCount, errorStateCount);
			if (displayErrorCount > 0)
			{
				toolStripStatusLabel3.BackColor = System.Drawing.Color.Red;
			}
			else
			{
				toolStripStatusLabel3.BackColor = backColor;
			}
		}

		private void UpdateFindStateFormSettings()
		{
			if (searchControlPanel.Visible)
			{
				FindStateSettings findStateSettings = GetFindStateSettings(vdm.CurrentView);
				findStateControl.UpdateSettings(findStateSettings);
			}
			findState.Enabled = true;
		}

		private void SaveFindStateFormSettings()
		{
			if (nodeSearchControlInialized)
			{
				findStateControl.SaveSettings();
			}
			findState.Enabled = false;
		}

		private FindStateSettings GetFindStateSettings(IViewDefinition viewDefinition)
		{
			GViewer value = null;
			if (!viewers.TryGetValue(viewDefinition, out value))
			{
				throw new InvalidOperationException("Can not find states before set transition system");
			}
			if (cachedFindStateSettings == null)
			{
				cachedFindStateSettings = new Dictionary<IViewDefinition, FindStateSettings>();
			}
			FindStateSettings value2 = null;
			if (cachedFindStateSettings.TryGetValue(viewDefinition, out value2))
			{
				return value2;
			}
			value2 = new FindStateSettings(value);
			SortedList<int, DisplayNode> sortedList = new SortedList<int, DisplayNode>();
			foreach (DisplayNode node in currentDisplayGraph.Nodes)
			{
				int result = int.MaxValue;
				if (node.DisplayNodeKind == DisplayNodeKind.Hyper)
				{
					if (node.SubNodes == null)
					{
						continue;
					}
					foreach (DisplayNode subNode in node.SubNodes)
					{
						int result2 = 0;
						if (int.TryParse(subNode.Label.Label.Substring(1), out result2) && result2 < result)
						{
							result = result2;
						}
					}
					sortedList.Add(result, node);
				}
				else if (int.TryParse(node.Label.Label.Substring(1), out result))
				{
					sortedList.Add(result, node);
				}
			}
			value2.OrderedDisplayNode = sortedList.Values.ToList();
			cachedFindStateSettings[viewDefinition] = value2;
			return value2;
		}

		public static Graph PersistenceFileToGraphForTest(string fileName, ViewDefinition viewDefinition)
		{
			ExplorationResultLoader explorationResultLoader = new ExplorationResultLoader(fileName);
			TransitionSystem transitionSystem;
			try
			{
				transitionSystem = explorationResultLoader.LoadTransitionSystem();
			}
			catch (ExplorationResultLoadingException)
			{
				throw;
			}
			DisplayGraphBuilder displayGraphBuilder = new DisplayGraphBuilder(transitionSystem);
			DisplayGraph displayGraph = displayGraphBuilder.BuildDisplayGraph(viewDefinition);
			GViewerControlBuilder gViewerControlBuilder = new GViewerControlBuilder();
			return gViewerControlBuilder.BuildControlGraph(displayGraph);
		}

		protected override void Dispose(bool disposing)
		{
			if (viewerBuilder != null)
			{
				viewerBuilder.Dispose();
				viewerBuilder = null;
			}
			if (currentGViewer != null && viewers != null && !viewers.ContainsValue(currentGViewer))
			{
				currentGViewer.AbortAsyncLayout();
				currentGViewer.Dispose();
				currentGViewer = null;
			}
			if (viewers != null)
			{
				foreach (GViewer value in viewers.Values)
				{
					value.Dispose();
				}
			}
			if (vdm != null)
			{
				vdm.ViewDefinitionUpdate -= OnViewDefinitionUpdated;
			}
			if (disposing && components != null)
			{
				components.Dispose();
				components = null;
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			viewDocumentTool = new System.Windows.Forms.ToolStrip();
			zoomIn = new System.Windows.Forms.ToolStripButton();
			zoomOut = new System.Windows.Forms.ToolStripButton();
			modeSplitButton = new System.Windows.Forms.ToolStripSplitButton();
			magnifyModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			moveModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			panModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			saveImage = new System.Windows.Forms.ToolStripButton();
			fullScreen = new System.Windows.Forms.ToolStripButton();
			fitToScreen = new System.Windows.Forms.ToolStripButton();
			manageViews = new System.Windows.Forms.ToolStripButton();
			findState = new System.Windows.Forms.ToolStripButton();
			comboBox = new System.Windows.Forms.ComboBox();
			statusStrip = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
			toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
			gViewerRenderingTextBox = new System.Windows.Forms.TextBox();
			comboBoxLabel = new System.Windows.Forms.Label();
			searchControlPanel = new System.Windows.Forms.Panel();
			findStateControl = new Microsoft.SpecExplorer.Viewer.FindState();
			displayPanel = new System.Windows.Forms.Panel();
			viewDocumentTool.SuspendLayout();
			statusStrip.SuspendLayout();
			searchControlPanel.SuspendLayout();
			SuspendLayout();
			viewDocumentTool.AccessibleName = "viewDocumentTool";
			viewDocumentTool.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			viewDocumentTool.Items.AddRange(new System.Windows.Forms.ToolStripItem[8] { zoomIn, zoomOut, modeSplitButton, saveImage, fullScreen, fitToScreen, manageViews, findState });
			viewDocumentTool.Location = new System.Drawing.Point(0, 0);
			viewDocumentTool.Name = "viewDocumentTool";
			viewDocumentTool.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			viewDocumentTool.Size = new System.Drawing.Size(546, 25);
			viewDocumentTool.TabIndex = 1;
			viewDocumentTool.Text = "toolStripExplorerGraph";
			zoomIn.AccessibleName = "ZoomIn";
			zoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			zoomIn.Image = Microsoft.SpecExplorer.Resource.ZoomIn;
			zoomIn.ImageTransparentColor = System.Drawing.Color.White;
			zoomIn.Name = "zoomIn";
			zoomIn.Size = new System.Drawing.Size(23, 22);
			zoomIn.Text = "ZoomIn";
			zoomIn.ToolTipText = "Zoom In";
			zoomOut.AccessibleName = "ZoomOut";
			zoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			zoomOut.Image = Microsoft.SpecExplorer.Resource.ZoomOut;
			zoomOut.ImageTransparentColor = System.Drawing.Color.White;
			zoomOut.Name = "zoomOut";
			zoomOut.Size = new System.Drawing.Size(23, 22);
			zoomOut.Text = "ZoomOut";
			zoomOut.ToolTipText = "Zoom Out";
			modeSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			modeSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { magnifyModeToolStripMenuItem, moveModeToolStripMenuItem, panModeToolStripMenuItem });
			modeSplitButton.Image = Microsoft.SpecExplorer.Resource.Move;
			modeSplitButton.ImageTransparentColor = System.Drawing.Color.White;
			modeSplitButton.Name = "modeSplitButton";
			modeSplitButton.Size = new System.Drawing.Size(32, 22);
			modeSplitButton.Text = "Mode";
			magnifyModeToolStripMenuItem.Name = "magnifyModeToolStripMenuItem";
			magnifyModeToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			magnifyModeToolStripMenuItem.Text = "Magnify";
			moveModeToolStripMenuItem.Name = "moveModeToolStripMenuItem";
			moveModeToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			moveModeToolStripMenuItem.Text = "Move";
			panModeToolStripMenuItem.Name = "panModeToolStripMenuItem";
			panModeToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
			panModeToolStripMenuItem.Text = "Pan";
			saveImage.AccessibleName = "Save Image";
			saveImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			saveImage.Image = Microsoft.SpecExplorer.Resource.SaveImage;
			saveImage.ImageTransparentColor = System.Drawing.Color.White;
			saveImage.Name = "saveImage";
			saveImage.Size = new System.Drawing.Size(23, 22);
			saveImage.Text = "SaveImage";
			saveImage.ToolTipText = "Save Graph Image";
			fullScreen.AccessibleName = "Full Screen";
			fullScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			fullScreen.Image = Microsoft.SpecExplorer.Resource.FullScreen;
			fullScreen.ImageTransparentColor = System.Drawing.Color.White;
			fullScreen.Name = "fullScreen";
			fullScreen.Size = new System.Drawing.Size(23, 22);
			fullScreen.Text = "FullScreen";
			fullScreen.ToolTipText = "Full Screen";
			fitToScreen.AccessibleName = "Fit to Screen";
			fitToScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			fitToScreen.Image = Microsoft.SpecExplorer.Resource.Fixtoscreen;
			fitToScreen.ImageTransparentColor = System.Drawing.Color.Magenta;
			fitToScreen.Name = "fitToScreen";
			fitToScreen.Size = new System.Drawing.Size(23, 22);
			fitToScreen.Text = "FitToScreen";
			fitToScreen.ToolTipText = "Fit to Screen";
			manageViews.AccessibleName = "Manage Views";
			manageViews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			manageViews.Image = Microsoft.SpecExplorer.Resource.ManageView;
			manageViews.ImageTransparentColor = System.Drawing.Color.White;
			manageViews.Name = "manageViews";
			manageViews.Size = new System.Drawing.Size(23, 22);
			manageViews.Text = "Manage Views";
			manageViews.ToolTipText = "Manage Views";
			findState.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			findState.Image = Microsoft.SpecExplorer.Resource.FindState;
			findState.ImageTransparentColor = System.Drawing.Color.White;
			findState.Name = "findState";
			findState.Size = new System.Drawing.Size(23, 22);
			findState.Text = "Find States";
			comboBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			comboBox.Location = new System.Drawing.Point(425, 0);
			comboBox.Name = "comboBox";
			comboBox.Size = new System.Drawing.Size(121, 21);
			comboBox.TabIndex = 2;
			statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { toolStripStatusLabel, toolStripStatusLabel1, toolStripStatusLabel2, toolStripStatusLabel3 });
			statusStrip.Location = new System.Drawing.Point(0, 289);
			statusStrip.Name = "statusStrip";
			statusStrip.Size = new System.Drawing.Size(546, 22);
			statusStrip.TabIndex = 0;
			statusStrip.Text = "statusStrip1";
			toolStripStatusLabel.Name = "toolStripStatusLabel";
			toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
			toolStripStatusLabel2.Name = "toolStripStatusLabel2";
			toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
			toolStripStatusLabel3.Name = "toolStripStatusLabel3";
			toolStripStatusLabel3.Size = new System.Drawing.Size(0, 17);
			gViewerRenderingTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			gViewerRenderingTextBox.Location = new System.Drawing.Point(0, 0);
			gViewerRenderingTextBox.Multiline = true;
			gViewerRenderingTextBox.Name = "gViewerRenderingTextBox";
			gViewerRenderingTextBox.ReadOnly = true;
			gViewerRenderingTextBox.Size = new System.Drawing.Size(0, 20);
			gViewerRenderingTextBox.TabIndex = 0;
			gViewerRenderingTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			comboBoxLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			comboBoxLabel.AutoSize = true;
			comboBoxLabel.Location = new System.Drawing.Point(344, 3);
			comboBoxLabel.Name = "comboBoxLabel";
			comboBoxLabel.Size = new System.Drawing.Size(75, 13);
			comboBoxLabel.TabIndex = 4;
			comboBoxLabel.Text = "Selected View";
			searchControlPanel.Controls.Add(findStateControl);
			searchControlPanel.Dock = System.Windows.Forms.DockStyle.Top;
			searchControlPanel.Location = new System.Drawing.Point(0, 25);
			searchControlPanel.Name = "searchControlPanel";
			searchControlPanel.Size = new System.Drawing.Size(546, 61);
			searchControlPanel.TabIndex = 5;
			searchControlPanel.Visible = false;
			findStateControl.Location = new System.Drawing.Point(3, 0);
			findStateControl.Name = "nodeSearchControl";
			findStateControl.Size = new System.Drawing.Size(531, 61);
			findStateControl.TabIndex = 0;
			displayPanel.AccessibleName = "DisplayPanel";
			displayPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			displayPanel.Location = new System.Drawing.Point(0, 86);
			displayPanel.Name = "displayPanel";
			displayPanel.Size = new System.Drawing.Size(546, 203);
			displayPanel.TabIndex = 6;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(displayPanel);
			base.Controls.Add(searchControlPanel);
			base.Controls.Add(comboBoxLabel);
			base.Controls.Add(comboBox);
			base.Controls.Add(viewDocumentTool);
			base.Controls.Add(statusStrip);
			base.Name = "ViewDocumentControl";
			base.Size = new System.Drawing.Size(546, 311);
			viewDocumentTool.ResumeLayout(false);
			viewDocumentTool.PerformLayout();
			statusStrip.ResumeLayout(false);
			statusStrip.PerformLayout();
			searchControlPanel.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}
	}
}
