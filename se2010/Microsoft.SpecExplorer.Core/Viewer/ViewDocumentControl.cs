// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.ViewDocumentControl
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.SpecExplorer.Viewer
{
  public class ViewDocumentControl : UserControl
  {
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
      get => this.displayGraphBuilder.TransitionSystem;
      set
      {
        this.displayPanel.Controls.Clear();
        this.displayGraphBuilder = new DisplayGraphBuilder(value);
        this.displayGraphs.Clear();
        foreach (GViewer gviewer in this.viewers.Values)
          gviewer?.Dispose();
        this.viewers.Clear();
        this.SetRecommendedViews();
        this.RefreshViewsAndViewComboBox();
        this.GetTransitionSystemStatus(out this.initialStateCount, out this.stateCount, out this.errorStateCount, out this.nonAcceptingEndStateCount, out this.boundHitStateCount, out this.stepCount, out this.requirementCount);
        this.SetDisplayRecommendedView();
        if (this.cachedFindStateSettings == null)
          return;
        this.cachedFindStateSettings.Clear();
      }
    }

    public ViewDocumentControl(IViewDefinitionManager vdm, IHost host)
    {
      this.InitializeComponent();
      this.vdm = vdm;
      this.host = host;
      this.displayGraphs = new Dictionary<IViewDefinition, DisplayGraph>();
      this.viewers = new Dictionary<IViewDefinition, GViewer>();
      this.viewerBuilder = new GViewerControlBuilder();
      this.comboBox.SelectedIndexChanged += new EventHandler(this.OnComboBoxChanged);
      this.comboBox.DropDown += new EventHandler(this.OnComboBoxDropDown);
      this.zoomIn.Click += new EventHandler(this.OnZoomInClick);
      this.zoomOut.Click += new EventHandler(this.OnZoomOutClick);
      this.magnifyModeToolStripMenuItem.Click += new EventHandler(this.OnMagnifyMode);
      this.moveModeToolStripMenuItem.Click += new EventHandler(this.OnMoveMode);
      this.panModeToolStripMenuItem.Click += new EventHandler(this.OnPanMode);
      this.saveImage.Click += new EventHandler(this.OnSaveImageClick);
      this.fullScreen.Click += new EventHandler(this.OnFullScreenClick);
      this.manageViews.Click += new EventHandler(this.OnInvokeViewDefinitionManager);
      this.fitToScreen.Click += new EventHandler(this.OnFitScreenClick);
      this.findState.Click += new EventHandler(this.OnFindStateClick);
      this.contextMenuStrip = new ContextMenuStrip();
      this.contextMenuStrip.Items.Add("Compare with selected state");
      this.contextMenuStrip.Opening += (CancelEventHandler) ((sender, e) =>
      {
        if (this.contextMenuStrip.Visible)
          e.Cancel = true;
        if (this.lastSelectedNode != null && this.selectingNode != null)
          this.contextMenuStrip.Items[0].Enabled = this.lastSelectedNode.Attr.Id != this.selectingNode.Attr.Id;
        else
          this.contextMenuStrip.Items[0].Enabled = false;
      });
      this.contextMenuStrip.Click += (EventHandler) ((sender, e) =>
      {
        if (this.lastSelectedNode == null || this.selectingNode == null || !(this.selectingNode.Attr.Id != this.lastSelectedNode.Attr.Id))
          return;
        DisplayNode nodeById1 = this.currentDisplayGraph.GetNodeById(this.lastSelectedNode.Id);
        DisplayNode nodeById2 = this.currentDisplayGraph.GetNodeById(this.selectingNode.Id);
        if (nodeById1.DisplayNodeKind == DisplayNodeKind.Hyper || nodeById2.DisplayNodeKind == DisplayNodeKind.Hyper)
        {
          this.host.NotificationDialog("Spec Explorer", "Can not compare hyper node");
        }
        else
        {
          if (this.CompareStates == null)
            return;
          this.CompareStates((object) this, new CompareStateEventArgs(nodeById1.Label, nodeById2.Label));
        }
      });
      this.toolTip = new ToolTip();
      vdm.ViewDefinitionUpdate += new EventHandler<ViewDefinitionUpdateEventArgs>(this.OnViewDefinitionUpdated);
    }

    public event EventHandler<StatesBrowserEventArgs> BrowseStates;

    public event EventHandler<CompareStateEventArgs> CompareStates;

    public event EventHandler<StepBrowserEventArgs> BrowseStep;

    private void OnGViewerClick(object sender, MouseEventArgs e)
    {
      if (this.currentGViewer.SelectedObject == null || e.Button == System.Windows.Forms.MouseButtons.Right)
        return;
      if (this.lastSelectedNode != null && this.currentGViewer.SelectedObject != this.lastSelectedNode)
      {
        this.lastSelectedNode.Attr.Color = this.lastSelectedRestoreColor;
        this.lastSelectedNode = (Microsoft.Msagl.Drawing.Node) null;
      }
      if (this.lastSelectedEdge != null && this.currentGViewer.SelectedObject != this.lastSelectedEdge)
      {
        ViewDocumentControl.SetEdgeAndLabelColor(this.lastSelectedEdge, this.lastSelectedRestoreColor);
        this.lastSelectedEdge = (Edge) null;
      }
      this.lastSelectionEdge = (Edge) null;
      this.lastSelectionNode = (Microsoft.Msagl.Drawing.Node) null;
      if (this.currentGViewer.SelectedObject is Microsoft.Msagl.Drawing.Node)
      {
        Microsoft.Msagl.Drawing.Node selectedObject = this.currentGViewer.SelectedObject as Microsoft.Msagl.Drawing.Node;
        if (e.Button == System.Windows.Forms.MouseButtons.Left)
        {
          if (this.lastSelectedNode != this.currentGViewer.SelectedObject)
          {
            this.lastSelectedNode = selectedObject;
            this.lastSelectedRestoreColor = this.lastSelectionRestoreColor;
            selectedObject.Attr.Color = Microsoft.Msagl.Drawing.Color.Blue;
          }
          StatesBrowserEventArgs e1 = new StatesBrowserEventArgs(this.currentDisplayGraph.GetNodeById(selectedObject.Id));
          if (this.BrowseStates != null)
            this.BrowseStates((object) this, e1);
        }
        this.currentGViewer.Invalidate();
      }
      else
      {
        if (!(this.currentGViewer.SelectedObject is Edge))
          return;
        Edge selectedObject = this.currentGViewer.SelectedObject as Edge;
        if (selectedObject.UserData == null)
        {
          this.lastSelectionEdge = selectedObject;
        }
        else
        {
          if (this.lastSelectedEdge != this.currentGViewer.SelectedObject)
          {
            this.lastSelectedEdge = selectedObject;
            this.lastSelectedRestoreColor = this.lastSelectionRestoreColor;
            ViewDocumentControl.SetEdgeAndLabelColor(selectedObject, Microsoft.Msagl.Drawing.Color.Blue);
          }
          DisplayEdge edgeById = this.currentDisplayGraph.GetEdgeById(selectedObject.UserData as string);
          List<BrowserEdge> browserEdgeList = new List<BrowserEdge>();
          switch (edgeById.displayEdgeKind)
          {
            case DisplayEdgeKind.Normal:
            case DisplayEdgeKind.Collapsed:
              browserEdgeList.Add(this.BuildBrowserEdge(edgeById));
              break;
          }
          if (this.BrowseStep == null || browserEdgeList.Count <= 0)
            return;
          this.BrowseStep((object) this, new StepBrowserEventArgs((IEnumerable<BrowserEdge>) browserEdgeList, edgeById.Text));
        }
      }
    }

    private BrowserEdge BuildBrowserEdge(DisplayEdge displayEdge)
    {
      switch (displayEdge.displayEdgeKind)
      {
        case DisplayEdgeKind.Normal:
          return new BrowserEdge(displayEdge.Text, displayEdge.ActionText, displayEdge.Source.Label, displayEdge.Target.Label, this.TranslateConstraints((IEnumerable<Constraint>) displayEdge.Label.PreConstraints), this.TranslateConstraints((IEnumerable<Constraint>) displayEdge.Label.PostConstraints), displayEdge.Label.VariablesToUnbindKeys, displayEdge.CapturedRequirements.ToArray<string>(), displayEdge.AssumeCapturedRequirements.ToArray<string>());
        case DisplayEdgeKind.Collapsed:
          List<string> stringList = new List<string>();
          stringList.AddRange((IEnumerable<string>) displayEdge.subEdges[0].Label.VariablesToUnbindKeys);
          stringList.AddRange((IEnumerable<string>) displayEdge.subEdges[1].Label.VariablesToUnbindKeys);
          return new BrowserEdge(displayEdge.Text, displayEdge.ActionText, displayEdge.Source.Label, displayEdge.Target.Label, this.TranslateConstraints((IEnumerable<Constraint>) displayEdge.subEdges.Where<DisplayEdge>((Func<DisplayEdge, bool>) (edge => edge.Kind == ActionSymbolKind.Call)).FirstOrDefault<DisplayEdge>().Label.PreConstraints), this.TranslateConstraints((IEnumerable<Constraint>) displayEdge.subEdges.Where<DisplayEdge>((Func<DisplayEdge, bool>) (edge => edge.Kind == ActionSymbolKind.Call)).FirstOrDefault<DisplayEdge>().Label.PostConstraints), stringList.ToArray(), displayEdge.CapturedRequirements.ToArray<string>(), displayEdge.AssumeCapturedRequirements.ToArray<string>());
        default:
          throw new InvalidOperationException("Can not create a browser edge from a hyper edge.");
      }
    }

    private string[] TranslateConstraints(IEnumerable<Constraint> constraints) => constraints.Select<Constraint, string>((Func<Constraint, string>) (t => t.Text)).ToArray<string>();

    private void OnSelectionChanged(object sender, EventArgs args)
    {
      if (this.contextMenuStrip != null)
        this.contextMenuStrip.Items[0].Enabled = false;
      if (this.lastSelectionNode != null)
      {
        this.lastSelectionNode.Attr.Color = this.lastSelectionRestoreColor;
        this.lastSelectionNode = (Microsoft.Msagl.Drawing.Node) null;
      }
      if (this.lastSelectionEdge != null)
      {
        ViewDocumentControl.SetEdgeAndLabelColor(this.lastSelectionEdge, this.lastSelectionRestoreColor);
        this.lastSelectionEdge = (Edge) null;
      }
      if (this.currentGViewer.SelectedObject is Microsoft.Msagl.Drawing.Node)
      {
        Microsoft.Msagl.Drawing.Node selectedObject = this.currentGViewer.SelectedObject as Microsoft.Msagl.Drawing.Node;
        this.lastSelectionRestoreColor = selectedObject.Attr.Color;
        selectedObject.Attr.Color = Microsoft.Msagl.Drawing.Color.Magenta;
        if (this.currentGViewer.ContextMenuStrip != null)
          this.currentGViewer.ContextMenuStrip.Items[0].Enabled = true;
        this.lastSelectionNode = selectedObject;
        this.toolTip.SetToolTip(this.currentGViewer.DrawingPanel, this.GetStateDisplay(selectedObject.Id));
        this.selectingNode = selectedObject;
      }
      else if (this.currentGViewer.SelectedObject is Edge)
      {
        Edge selectedObject = this.currentGViewer.SelectedObject as Edge;
        this.lastSelectionRestoreColor = selectedObject.Attr.Color;
        ViewDocumentControl.SetEdgeAndLabelColor(selectedObject, Microsoft.Msagl.Drawing.Color.Red);
        this.lastSelectionEdge = selectedObject;
        this.selectingNode = (Microsoft.Msagl.Drawing.Node) null;
      }
      else
        this.selectingNode = (Microsoft.Msagl.Drawing.Node) null;
      this.currentGViewer.Invalidate();
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
      if (Control.ModifierKeys == Keys.Control)
      {
        if (e.X >= 0 && (e.X <= this.ClientRectangle.Width && e.Y >= 0) && (e.Y <= this.ClientRectangle.Height && this.currentGViewer != null))
        {
          if (e.Delta < 0)
            this.OnZoomOutClick((object) this, (EventArgs) e);
          else
            this.OnZoomInClick((object) this, (EventArgs) e);
        }
      }
      else if (this.currentGViewer != null)
        this.currentGViewer.Pan(0.0f, (float) e.Delta);
      base.OnMouseWheel(e);
    }

    public event EventHandler InvokeViewDefinitionManager;

    private void OnComboBoxChanged(object sender, EventArgs e)
    {
      if (this.displayGraphBuilder == null)
        throw new InvalidOperationException("Can not display graph before set transition system");
      if (this.comboBox.SelectedIndex == this.comboxSplitterIndex)
      {
        this.comboBox.SelectedIndex = 0;
      }
      else
      {
        string selectedItem = (string) this.comboBox.SelectedItem;
        if (selectedItem == null)
          return;
        this.SaveFindStateFormSettings();
        IViewDefinition viewDefinition1;
        this.vdm.TryGetViewDefinition(selectedItem, out viewDefinition1);
        ViewDefinition viewDefinition2 = viewDefinition1 as ViewDefinition;
        this.vdm.CurrentView = (IViewDefinition) viewDefinition2;
        if (this.currentGViewer != null)
          this.currentGViewer.AbortAsyncLayout();
        this.ResetGviewer();
        this.displayPanel.Controls.Add((Control) this.gViewerRenderingTextBox);
        if (!this.displayGraphs.TryGetValue(viewDefinition1, out this.currentDisplayGraph))
        {
          try
          {
            this.currentDisplayGraph = this.displayGraphBuilder.BuildDisplayGraph(viewDefinition2);
            this.displayGraphs[viewDefinition1] = this.currentDisplayGraph;
          }
          catch (QueryException ex)
          {
            this.gViewerRenderingTextBox.Text = ex.Message;
            return;
          }
        }
        if (!this.viewers.TryGetValue(viewDefinition1, out this.currentGViewer))
          this.currentGViewer = this.viewerBuilder.BuildGViewerControl(viewDefinition2.RenderingTimeOut);
        this.SetGViewer();
        this.RenderingAndSetGViewer(selectedItem);
        this.DisplayStatus();
      }
    }

    private void OnComboBoxDropDown(object sender, EventArgs e)
    {
      if (this.displayGraphBuilder == null)
        throw new InvalidOperationException("Can not display graph before set transition system");
      string selectedItem = this.comboBox.SelectedItem as string;
      this.comboBox.SelectedIndexChanged -= new EventHandler(this.OnComboBoxChanged);
      this.RefreshViewsAndViewComboBox();
      this.comboBox.SelectedItem = (object) selectedItem;
      this.comboBox.SelectedIndexChanged += new EventHandler(this.OnComboBoxChanged);
      if (this.comboBox.SelectedIndex >= 0)
        return;
      this.comboBox.SelectedIndex = 0;
    }

    private void OnZoomInClick(object sender, EventArgs e)
    {
      if (this.currentGViewer == null)
        return;
      this.currentGViewer.ZoomInPressed();
    }

    private void OnZoomOutClick(object sender, EventArgs e)
    {
      if (this.currentGViewer == null)
        return;
      this.currentGViewer.ZoomOutPressed();
    }

    private void OnMagnifyMode(object sender, EventArgs e)
    {
      if (this.currentGViewer == null)
        return;
      this.magnifyModeToolStripMenuItem.Checked = true;
      this.moveModeToolStripMenuItem.Checked = false;
      this.panModeToolStripMenuItem.Checked = false;
      this.currentGViewer.WindowZoomButtonPressed = true;
      this.currentGViewer.PanButtonPressed = false;
    }

    private void OnMoveMode(object sender, EventArgs e)
    {
      if (this.currentGViewer == null)
        return;
      this.magnifyModeToolStripMenuItem.Checked = false;
      this.moveModeToolStripMenuItem.Checked = true;
      this.panModeToolStripMenuItem.Checked = false;
      this.currentGViewer.WindowZoomButtonPressed = false;
      this.currentGViewer.PanButtonPressed = false;
    }

    private void OnPanMode(object sender, EventArgs e)
    {
      if (this.currentGViewer == null)
        return;
      this.magnifyModeToolStripMenuItem.Checked = false;
      this.moveModeToolStripMenuItem.Checked = false;
      this.panModeToolStripMenuItem.Checked = true;
      this.currentGViewer.WindowZoomButtonPressed = false;
      this.currentGViewer.PanButtonPressed = true;
    }

    private void OnSaveImageClick(object sender, EventArgs e)
    {
      if (this.currentGViewer == null)
        return;
      this.currentGViewer.SaveButtonPressed();
    }

    private void OnFullScreenClick(object sender, EventArgs e)
    {
      if (this.FullScreen == null)
        return;
      this.FullScreen(sender, e);
    }

    private void OnInvokeViewDefinitionManager(object sender, EventArgs e) => this.InvokeViewDefinitionManager(sender, new EventArgs());

    private void OnFitScreenClick(object sender, EventArgs e)
    {
      if (this.currentGViewer == null)
        return;
      this.currentGViewer.ZoomF = 1.0;
    }

    private void OnFindStateClick(object sender, EventArgs e)
    {
      if (!this.searchControlPanel.Visible)
      {
        if (!this.nodeSearchControlInialized)
        {
          this.findStateControl.UpdateSettings(this.GetFindStateSettings(this.vdm.CurrentView));
          this.findStateControl.SetHost(this.host);
          this.nodeSearchControlInialized = true;
        }
        this.searchControlPanel.Visible = true;
      }
      else
        this.searchControlPanel.Visible = false;
    }

    private void OnViewDefinitionUpdated(object sender, ViewDefinitionUpdateEventArgs args)
    {
      foreach (IViewDefinition updatedViewDefinition in args.UpdatedViewDefinitions)
      {
        this.displayGraphs.Remove(updatedViewDefinition);
        GViewer gviewer;
        if (this.viewers.TryGetValue(updatedViewDefinition, out gviewer) && gviewer != null)
        {
          this.viewers.Remove(updatedViewDefinition);
          if (gviewer == this.currentGViewer)
          {
            this.lastSelectedEdge = (Edge) null;
            this.lastSelectedNode = (Microsoft.Msagl.Drawing.Node) null;
            this.lastSelectionEdge = (Edge) null;
            this.lastSelectionNode = (Microsoft.Msagl.Drawing.Node) null;
          }
          gviewer.Dispose();
        }
      }
      this.RefreshViewsAndViewComboBox();
      this.comboBox.SelectedItem = (object) this.vdm.CurrentView.Name;
    }

    private void RefreshViewsAndViewComboBox()
    {
      this.comboBox.Items.Clear();
      foreach (IViewDefinition defaultView in this.vdm.DefaultViews)
        this.comboBox.Items.Add((object) defaultView.Name);
      HashSet<string> source = new HashSet<string>();
      foreach (IViewDefinition customizedView in this.vdm.CustomizedViews)
        source.Add(customizedView.Name);
      foreach (string recommendedView in this.recommendedViews)
      {
        if (source.Contains(recommendedView))
        {
          this.comboBox.Items.Add((object) recommendedView);
          source.Remove(recommendedView);
        }
      }
      if (source.Count > 0)
      {
        this.comboxSplitterIndex = this.comboBox.Items.Add((object) "------------------------");
        this.comboBox.Items.AddRange((object[]) source.OrderBy<string, string>((Func<string, string>) (item => item)).ToArray<string>());
      }
      else
        this.comboxSplitterIndex = this.comboBox.Items.Count;
    }

    private void SetRecommendedViews()
    {
      this.recommendedViews = (IEnumerable<string>) this.TransitionSystem.GetSwitch("RecommendedViews").Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries);
      if (this.recommendedViews.Count<string>() <= 0)
        return;
      List<string> stringList = new List<string>();
      foreach (string recommendedView in this.recommendedViews)
        stringList.Add(recommendedView.Trim());
      this.recommendedViews = (IEnumerable<string>) stringList;
    }

    private void SetDisplayRecommendedView()
    {
      int selectedIndex = this.comboBox.SelectedIndex;
      if (selectedIndex <= 0)
      {
        foreach (object recommendedView in this.recommendedViews)
        {
          this.comboBox.SelectedItem = recommendedView;
          if (this.comboBox.SelectedIndex != -1)
            break;
        }
        if (this.comboBox.SelectedIndex != -1)
          return;
        this.comboBox.SelectedIndex = 0;
      }
      else
        this.comboBox.SelectedIndex = selectedIndex;
    }

    private void SetGViewer()
    {
      if (this.currentGViewer == null)
        return;
      this.currentGViewer.Dock = DockStyle.Fill;
      this.currentGViewer.SetContextMenumStrip(this.contextMenuStrip);
      this.currentGViewer.MouseClick += new MouseEventHandler(this.OnGViewerClick);
      this.currentGViewer.ObjectUnderMouseCursorChanged += new EventHandler<ObjectUnderMouseCursorChangedEventArgs>(this.OnSelectionChanged);
      if (this.currentGViewer.PanButtonPressed)
        this.OnPanMode((object) this, new EventArgs());
      else if (this.currentGViewer.WindowZoomButtonPressed)
        this.OnMagnifyMode((object) this, new EventArgs());
      else
        this.OnMoveMode((object) this, new EventArgs());
      this.lastSelectionEdge = (Edge) null;
      this.lastSelectionNode = (Microsoft.Msagl.Drawing.Node) null;
    }

    private void DisplayGviewer()
    {
      this.displayPanel.Controls.Remove((Control) this.gViewerRenderingTextBox);
      this.displayPanel.Controls.Add((Control) this.currentGViewer);
      this.currentGViewer.Focus();
    }

    private void ResetGviewer()
    {
      if (this.currentGViewer == null)
        return;
      this.displayPanel.Controls.Remove((Control) this.currentGViewer);
      this.currentGViewer.MouseClick -= new MouseEventHandler(this.OnGViewerClick);
      this.currentGViewer.ObjectUnderMouseCursorChanged -= new EventHandler<ObjectUnderMouseCursorChangedEventArgs>(this.OnSelectionChanged);
      this.currentGViewer.PanButtonPressed = false;
      if (this.lastSelectionEdge != null)
        ViewDocumentControl.SetEdgeAndLabelColor(this.lastSelectionEdge, this.lastSelectionRestoreColor);
      if (this.lastSelectionNode == null)
        return;
      this.lastSelectionNode.Attr.Color = this.lastSelectionRestoreColor;
    }

    private static void SetEdgeAndLabelColor(Edge edge, Microsoft.Msagl.Drawing.Color color)
    {
      edge.Attr.Color = color;
      if (edge.Label == null)
        return;
      edge.Label.FontColor = color;
    }

    private void SetGViewerRenderingText(string str) => this.gViewerRenderingTextBox.Text = str;

    private void RenderingAndSetGViewer(string viewName)
    {
      if (this.currentGViewer.Graph == null)
      {
        IViewDefinition ivd;
        this.vdm.TryGetViewDefinition(viewName, out ivd);
        if (this.currentGViewer.AsyncLayout)
        {
          this.currentGViewer.AsyncLayoutProgress += (EventHandler<LayoutProgressEventArgs>) ((s, args) =>
          {
            if (!this.Created)
              return;
            ViewDocumentControl.SetGViewerRenderingStatus gviewerRenderingStatus = new ViewDocumentControl.SetGViewerRenderingStatus(this.SetGViewerRenderingText);
            switch (args.Progress)
            {
              case LayoutProgress.LayingOut:
              case LayoutProgress.Rendering:
                string str1 = "Rendering ...";
                this.BeginInvoke((Delegate) gviewerRenderingStatus, (object) str1);
                break;
              case LayoutProgress.Finished:
                this.viewers[ivd] = this.currentGViewer;
                this.DisplayGviewer();
                this.UpdateFindStateFormSettings();
                break;
              case LayoutProgress.Aborted:
                if (string.IsNullOrEmpty(args.Diagnostics))
                {
                  string str2 = string.Format("Rendering aborted after {0} seconds. Increase timeout (view definition parameter Timeout) if you want to wait longer.", (object) ivd.RenderingTimeOut.ToString());
                  this.BeginInvoke((Delegate) gviewerRenderingStatus, (object) str2);
                }
                else
                  this.BeginInvoke((Delegate) gviewerRenderingStatus, (object) args.Diagnostics);
                this.ResetGviewer();
                break;
            }
          });
        }
        else
        {
          this.viewers[ivd] = this.currentGViewer;
          this.DisplayGviewer();
          this.UpdateFindStateFormSettings();
        }
        this.currentGViewer.Graph = this.viewerBuilder.BuildControlGraph(this.currentDisplayGraph);
      }
      else
      {
        this.DisplayGviewer();
        this.UpdateFindStateFormSettings();
      }
    }

    private string GetStateDisplay(string nodeId)
    {
      DisplayNode nodeById = this.currentDisplayGraph.GetNodeById(nodeId);
      if (nodeById == null)
        return (string) null;
      StringBuilder stringBuilder = new StringBuilder();
      if ((nodeById.StateFlags & ObjectModel.StateFlags.PathDepthBoundStopped) != null)
        stringBuilder.Append("[path depth bound hit]");
      if ((nodeById.StateFlags & ObjectModel.StateFlags.StateBoundStopped) != null)
        stringBuilder.Append("[state bound hit]");
      if ((nodeById.StateFlags & ObjectModel.StateFlags.StepBoundStopped) != null)
        stringBuilder.Append("[step bound hit]");
      if ((nodeById.StateFlags & ObjectModel.StateFlags.ExplorationErrorBoundStopped) != null)
        stringBuilder.Append("[exploration error bound hit]");
      if ((nodeById.StateFlags & ObjectModel.StateFlags.StepsPerStateBoundStopped) != null)
        stringBuilder.Append("[steps per state bound hit]");
      if ((nodeById.StateFlags & ObjectModel.StateFlags.UserStopped) != null)
        stringBuilder.Append("[user stopped]");
      if ((nodeById.StateFlags & ObjectModel.StateFlags.NonAcceptingEnd) != null)
        stringBuilder.Append("[non-accepting end]");
      if ((nodeById.StateFlags & ObjectModel.StateFlags.Accepting) != null)
      {
        switch (nodeById.DisplayNodeKind)
        {
          case DisplayNodeKind.Normal:
            stringBuilder.Append(nodeById.Label.Description);
            break;
          case DisplayNodeKind.Hyper:
            using (List<State>.Enumerator enumerator = nodeById.LeafNodeStates.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                State current = enumerator.Current;
                stringBuilder.Append(current.Description);
                if (current != ((IEnumerable<State>) nodeById.LeafNodeStates).Last<State>())
                  stringBuilder.Append(", ");
              }
              break;
            }
        }
      }
      return stringBuilder.ToString();
    }

    private void GetCurrentDisplayGraphStatus(
      out int displayInitialStateCount,
      out int displayStateCount,
      out int displayStepCount,
      out int displayRequirementCount,
      out int displayBoundCount,
      out string boundKind,
      out int displayErrorCount)
    {
      displayInitialStateCount = 0;
      displayStateCount = 0;
      displayStepCount = 0;
      displayRequirementCount = 0;
      displayBoundCount = 0;
      boundKind = "";
      displayErrorCount = 0;
      if (this.currentDisplayGraph == null)
        return;
      displayInitialStateCount = this.currentDisplayGraph.StartNodes.Count<Microsoft.GraphTraversal.Node<State>>();
      displayStateCount = this.currentDisplayGraph.Nodes.Count<Microsoft.GraphTraversal.Node<State>>();
      HashSet<string> stringSet = new HashSet<string>();
      foreach (DisplayEdge edge in this.currentDisplayGraph.Edges)
      {
        ++displayStepCount;
        foreach (string displayEdgeRequirement in this.GetDisplayEdgeRequirements(edge))
          stringSet.Add(displayEdgeRequirement);
      }
      displayRequirementCount = !(this.vdm.CurrentView as ViewDefinition).DisplayRequirements ? 0 : stringSet.Count;
      StateFlags stateFlags = (StateFlags) 0;
      foreach (DisplayNode node in this.currentDisplayGraph.Nodes)
      {
        if ((node.StateFlags & ObjectModel.StateFlags.Accepting) != null)
          ++displayErrorCount;
        if ((node.StateFlags & ObjectModel.StateFlags.BoundStopped) != null)
        {
          ++displayBoundCount;
          stateFlags = stateFlags | node.StateFlags;
        }
      }
      if (stateFlags == null)
        return;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[");
      if ((stateFlags & ObjectModel.StateFlags.StateBoundStopped) != null)
        stringBuilder.Append("State, ");
      if ((stateFlags & ObjectModel.StateFlags.StepBoundStopped) != null)
        stringBuilder.Append("Step, ");
      if ((stateFlags & ObjectModel.StateFlags.PathDepthBoundStopped) != null)
        stringBuilder.Append("Path depth, ");
      if ((stateFlags & ObjectModel.StateFlags.StepsPerStateBoundStopped) != null)
        stringBuilder.Append("Steps per state, ");
      if ((stateFlags & ObjectModel.StateFlags.ExplorationErrorBoundStopped) != null)
        stringBuilder.Append("Exploration error, ");
      stringBuilder.Remove(stringBuilder.Length - 2, 2);
      stringBuilder.Append("]");
      boundKind = stringBuilder.ToString();
    }

    private IEnumerable<string> GetDisplayEdgeRequirements(DisplayEdge displayEdge)
    {
      switch (displayEdge.displayEdgeKind)
      {
        case DisplayEdgeKind.Normal:
          foreach (string capturedRequirement in displayEdge.Label.CapturedRequirements)
            yield return capturedRequirement;
          foreach (string capturedRequirement in displayEdge.Label.AssumeCapturedRequirements)
            yield return capturedRequirement;
          break;
        case DisplayEdgeKind.Hyper:
        case DisplayEdgeKind.Collapsed:
          using (List<DisplayEdge>.Enumerator enumerator = displayEdge.subEdges.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              DisplayEdge de = enumerator.Current;
              foreach (string displayEdgeRequirement in this.GetDisplayEdgeRequirements(de))
                yield return displayEdgeRequirement;
            }
            break;
          }
      }
    }

    public void GetTransitionSystemStatus(
      out int initialStateCount,
      out int stateCount,
      out int errorStateCount,
      out int nonAcceptingEndStateCount,
      out int boundHitStateCount,
      out int stepCount,
      out int requirementCount)
    {
      stateCount = 0;
      errorStateCount = 0;
      nonAcceptingEndStateCount = 0;
      boundHitStateCount = 0;
      foreach (State state in this.TransitionSystem.States)
      {
        if (!state.IsVirtual && (state.RepresentativeState == null || state.RelationKind != StateRelationKind.Equivalent))
        {
          ++stateCount;
          if ((state.Flags & StateFlags.Error) != null)
            ++errorStateCount;
          if ((state.Flags & StateFlags.NonAcceptingEnd) != null)
            ++nonAcceptingEndStateCount;
          if ((state.Flags & StateFlags.BoundStopped) != null)
            ++boundHitStateCount;
        }
      }
      initialStateCount = this.TransitionSystem.InitialStates.Length;
      stepCount = 0;
      HashSet<string> stringSet = new HashSet<string>();
      foreach (Transition transition in this.TransitionSystem.Transitions)
      {
        ++stepCount;
        foreach (string capturedRequirement in transition.CapturedRequirements)
          stringSet.Add(capturedRequirement);
        foreach (string capturedRequirement in transition.AssumeCapturedRequirements)
          stringSet.Add(capturedRequirement);
      }
      requirementCount = stringSet.Count;
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
      this.GetCurrentDisplayGraphStatus(out displayInitialStateCount, out displayStateCount, out displayStepCount, out displayRequirementCount, out displayBoundCount, out boundKind, out displayErrorCount);
      this.toolStripStatusLabel.Text = string.Format("Initial State(s): {0}/{1}   |   States:  {2}/{3}   |   Steps: {4}/{5}   |    Requirements: {6}/{7}   |", (object) displayInitialStateCount, (object) this.initialStateCount, (object) displayStateCount, (object) this.stateCount, (object) displayStepCount, (object) this.stepCount, (object) displayRequirementCount, (object) this.requirementCount);
      this.toolStripStatusLabel1.Text = string.Format("Bounds:  {0}/{1} {2}", (object) displayBoundCount, (object) this.boundHitStateCount, (object) boundKind);
      System.Drawing.Color backColor = this.toolStripStatusLabel.BackColor;
      if (this.boundHitStateCount > 0)
        this.toolStripStatusLabel1.BackColor = System.Drawing.Color.Orange;
      else
        this.toolStripStatusLabel1.BackColor = backColor;
      this.toolStripStatusLabel2.Text = "|";
      this.toolStripStatusLabel3.Text = string.Format("Errors:  {0}/{1}", (object) displayErrorCount, (object) this.errorStateCount);
      if (displayErrorCount > 0)
        this.toolStripStatusLabel3.BackColor = System.Drawing.Color.Red;
      else
        this.toolStripStatusLabel3.BackColor = backColor;
    }

    private void UpdateFindStateFormSettings()
    {
      if (this.searchControlPanel.Visible)
        this.findStateControl.UpdateSettings(this.GetFindStateSettings(this.vdm.CurrentView));
      this.findState.Enabled = true;
    }

    private void SaveFindStateFormSettings()
    {
      if (this.nodeSearchControlInialized)
        this.findStateControl.SaveSettings();
      this.findState.Enabled = false;
    }

    private FindStateSettings GetFindStateSettings(IViewDefinition viewDefinition)
    {
      GViewer viewer = (GViewer) null;
      if (!this.viewers.TryGetValue(viewDefinition, out viewer))
        throw new InvalidOperationException("Can not find states before set transition system");
      if (this.cachedFindStateSettings == null)
        this.cachedFindStateSettings = new Dictionary<IViewDefinition, FindStateSettings>();
      FindStateSettings findStateSettings1 = (FindStateSettings) null;
      if (this.cachedFindStateSettings.TryGetValue(viewDefinition, out findStateSettings1))
        return findStateSettings1;
      FindStateSettings findStateSettings2 = new FindStateSettings(viewer);
      SortedList<int, DisplayNode> sortedList = new SortedList<int, DisplayNode>();
      foreach (DisplayNode node in this.currentDisplayGraph.Nodes)
      {
        int result1 = int.MaxValue;
        if (node.DisplayNodeKind == DisplayNodeKind.Hyper)
        {
          if (node.SubNodes != null)
          {
            foreach (DisplayNode subNode in node.SubNodes)
            {
              int result2 = 0;
              if (int.TryParse(subNode.Label.Label.Substring(1), out result2) && result2 < result1)
                result1 = result2;
            }
            sortedList.Add(result1, node);
          }
        }
        else if (int.TryParse(node.Label.Label.Substring(1), out result1))
          sortedList.Add(result1, node);
      }
      findStateSettings2.OrderedDisplayNode = sortedList.Values.ToList<DisplayNode>();
      this.cachedFindStateSettings[viewDefinition] = findStateSettings2;
      return findStateSettings2;
    }

    public static Graph PersistenceFileToGraphForTest(
      string fileName,
      ViewDefinition viewDefinition)
    {
      ExplorationResultLoader explorationResultLoader = new ExplorationResultLoader(fileName);
      TransitionSystem transitionSystem;
      try
      {
        transitionSystem = explorationResultLoader.LoadTransitionSystem();
      }
      catch (ExplorationResultLoadingException ex)
      {
        throw;
      }
      return new GViewerControlBuilder().BuildControlGraph(new DisplayGraphBuilder(transitionSystem).BuildDisplayGraph(viewDefinition));
    }

    protected override void Dispose(bool disposing)
    {
      if (this.viewerBuilder != null)
      {
        this.viewerBuilder.Dispose();
        this.viewerBuilder = (GViewerControlBuilder) null;
      }
      if (this.currentGViewer != null && this.viewers != null && !this.viewers.ContainsValue(this.currentGViewer))
      {
        this.currentGViewer.AbortAsyncLayout();
        this.currentGViewer.Dispose();
        this.currentGViewer = (GViewer) null;
      }
      if (this.viewers != null)
      {
        foreach (Component component in this.viewers.Values)
          component.Dispose();
      }
      if (this.vdm != null)
        this.vdm.ViewDefinitionUpdate -= new EventHandler<ViewDefinitionUpdateEventArgs>(this.OnViewDefinitionUpdated);
      if (disposing && this.components != null)
      {
        this.components.Dispose();
        this.components = (IContainer) null;
      }
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.viewDocumentTool = new ToolStrip();
      this.zoomIn = new ToolStripButton();
      this.zoomOut = new ToolStripButton();
      this.modeSplitButton = new ToolStripSplitButton();
      this.magnifyModeToolStripMenuItem = new ToolStripMenuItem();
      this.moveModeToolStripMenuItem = new ToolStripMenuItem();
      this.panModeToolStripMenuItem = new ToolStripMenuItem();
      this.saveImage = new ToolStripButton();
      this.fullScreen = new ToolStripButton();
      this.fitToScreen = new ToolStripButton();
      this.manageViews = new ToolStripButton();
      this.findState = new ToolStripButton();
      this.comboBox = new ComboBox();
      this.statusStrip = new StatusStrip();
      this.toolStripStatusLabel = new ToolStripStatusLabel();
      this.toolStripStatusLabel1 = new ToolStripStatusLabel();
      this.toolStripStatusLabel2 = new ToolStripStatusLabel();
      this.toolStripStatusLabel3 = new ToolStripStatusLabel();
      this.gViewerRenderingTextBox = new TextBox();
      this.comboBoxLabel = new System.Windows.Forms.Label();
      this.searchControlPanel = new Panel();
      this.findStateControl = new FindState();
      this.displayPanel = new Panel();
      this.viewDocumentTool.SuspendLayout();
      this.statusStrip.SuspendLayout();
      this.searchControlPanel.SuspendLayout();
      this.SuspendLayout();
      this.viewDocumentTool.AccessibleName = "viewDocumentTool";
      this.viewDocumentTool.GripStyle = ToolStripGripStyle.Hidden;
      this.viewDocumentTool.Items.AddRange(new ToolStripItem[8]
      {
        (ToolStripItem) this.zoomIn,
        (ToolStripItem) this.zoomOut,
        (ToolStripItem) this.modeSplitButton,
        (ToolStripItem) this.saveImage,
        (ToolStripItem) this.fullScreen,
        (ToolStripItem) this.fitToScreen,
        (ToolStripItem) this.manageViews,
        (ToolStripItem) this.findState
      });
      this.viewDocumentTool.Location = new Point(0, 0);
      this.viewDocumentTool.Name = "viewDocumentTool";
      this.viewDocumentTool.RenderMode = ToolStripRenderMode.System;
      this.viewDocumentTool.Size = new System.Drawing.Size(546, 25);
      this.viewDocumentTool.TabIndex = 1;
      this.viewDocumentTool.Text = "toolStripExplorerGraph";
      this.zoomIn.AccessibleName = "ZoomIn";
      this.zoomIn.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.zoomIn.Image = (Image) Resources.ZoomIn;
      this.zoomIn.ImageTransparentColor = System.Drawing.Color.White;
      this.zoomIn.Name = "zoomIn";
      this.zoomIn.Size = new System.Drawing.Size(23, 22);
      this.zoomIn.Text = "ZoomIn";
      this.zoomIn.ToolTipText = "Zoom In";
      this.zoomOut.AccessibleName = "ZoomOut";
      this.zoomOut.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.zoomOut.Image = (Image) Resources.ZoomOut;
      this.zoomOut.ImageTransparentColor = System.Drawing.Color.White;
      this.zoomOut.Name = "zoomOut";
      this.zoomOut.Size = new System.Drawing.Size(23, 22);
      this.zoomOut.Text = "ZoomOut";
      this.zoomOut.ToolTipText = "Zoom Out";
      this.modeSplitButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.modeSplitButton.DropDownItems.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.magnifyModeToolStripMenuItem,
        (ToolStripItem) this.moveModeToolStripMenuItem,
        (ToolStripItem) this.panModeToolStripMenuItem
      });
      this.modeSplitButton.Image = (Image) Resources.Move;
      this.modeSplitButton.ImageTransparentColor = System.Drawing.Color.White;
      this.modeSplitButton.Name = "modeSplitButton";
      this.modeSplitButton.Size = new System.Drawing.Size(32, 22);
      this.modeSplitButton.Text = "Mode";
      this.magnifyModeToolStripMenuItem.Name = "magnifyModeToolStripMenuItem";
      this.magnifyModeToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
      this.magnifyModeToolStripMenuItem.Text = "Magnify";
      this.moveModeToolStripMenuItem.Name = "moveModeToolStripMenuItem";
      this.moveModeToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
      this.moveModeToolStripMenuItem.Text = "Move";
      this.panModeToolStripMenuItem.Name = "panModeToolStripMenuItem";
      this.panModeToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
      this.panModeToolStripMenuItem.Text = "Pan";
      this.saveImage.AccessibleName = "Save Image";
      this.saveImage.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.saveImage.Image = (Image) Resources.SaveImage;
      this.saveImage.ImageTransparentColor = System.Drawing.Color.White;
      this.saveImage.Name = "saveImage";
      this.saveImage.Size = new System.Drawing.Size(23, 22);
      this.saveImage.Text = "SaveImage";
      this.saveImage.ToolTipText = "Save Graph Image";
      this.fullScreen.AccessibleName = "Full Screen";
      this.fullScreen.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.fullScreen.Image = (Image) Resources.FullScreen;
      this.fullScreen.ImageTransparentColor = System.Drawing.Color.White;
      this.fullScreen.Name = "fullScreen";
      this.fullScreen.Size = new System.Drawing.Size(23, 22);
      this.fullScreen.Text = "FullScreen";
      this.fullScreen.ToolTipText = "Full Screen";
      this.fitToScreen.AccessibleName = "Fit to Screen";
      this.fitToScreen.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.fitToScreen.Image = (Image) Resources.Fixtoscreen;
      this.fitToScreen.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.fitToScreen.Name = "fitToScreen";
      this.fitToScreen.Size = new System.Drawing.Size(23, 22);
      this.fitToScreen.Text = "FitToScreen";
      this.fitToScreen.ToolTipText = "Fit to Screen";
      this.manageViews.AccessibleName = "Manage Views";
      this.manageViews.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.manageViews.Image = (Image) Resources.ManageView;
      this.manageViews.ImageTransparentColor = System.Drawing.Color.White;
      this.manageViews.Name = "manageViews";
      this.manageViews.Size = new System.Drawing.Size(23, 22);
      this.manageViews.Text = "Manage Views";
      this.manageViews.ToolTipText = "Manage Views";
      this.findState.DisplayStyle = ToolStripItemDisplayStyle.Image;
      this.findState.Image = (Image) Resources.FindState;
      this.findState.ImageTransparentColor = System.Drawing.Color.White;
      this.findState.Name = "findState";
      this.findState.Size = new System.Drawing.Size(23, 22);
      this.findState.Text = "Find States";
      this.comboBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
      this.comboBox.Location = new Point(425, 0);
      this.comboBox.Name = "comboBox";
      this.comboBox.Size = new System.Drawing.Size(121, 21);
      this.comboBox.TabIndex = 2;
      this.statusStrip.Items.AddRange(new ToolStripItem[4]
      {
        (ToolStripItem) this.toolStripStatusLabel,
        (ToolStripItem) this.toolStripStatusLabel1,
        (ToolStripItem) this.toolStripStatusLabel2,
        (ToolStripItem) this.toolStripStatusLabel3
      });
      this.statusStrip.Location = new Point(0, 289);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new System.Drawing.Size(546, 22);
      this.statusStrip.TabIndex = 0;
      this.statusStrip.Text = "statusStrip1";
      this.toolStripStatusLabel.Name = "toolStripStatusLabel";
      this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
      this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
      this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
      this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
      this.toolStripStatusLabel3.Size = new System.Drawing.Size(0, 17);
      this.gViewerRenderingTextBox.Dock = DockStyle.Fill;
      this.gViewerRenderingTextBox.Location = new Point(0, 0);
      this.gViewerRenderingTextBox.Multiline = true;
      this.gViewerRenderingTextBox.Name = "gViewerRenderingTextBox";
      this.gViewerRenderingTextBox.ReadOnly = true;
      this.gViewerRenderingTextBox.Size = new System.Drawing.Size(0, 20);
      this.gViewerRenderingTextBox.TabIndex = 0;
      this.gViewerRenderingTextBox.TextAlign = HorizontalAlignment.Center;
      this.comboBoxLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.comboBoxLabel.AutoSize = true;
      this.comboBoxLabel.Location = new Point(344, 3);
      this.comboBoxLabel.Name = "comboBoxLabel";
      this.comboBoxLabel.Size = new System.Drawing.Size(75, 13);
      this.comboBoxLabel.TabIndex = 4;
      this.comboBoxLabel.Text = "Selected View";
      this.searchControlPanel.Controls.Add((Control) this.findStateControl);
      this.searchControlPanel.Dock = DockStyle.Top;
      this.searchControlPanel.Location = new Point(0, 25);
      this.searchControlPanel.Name = "searchControlPanel";
      this.searchControlPanel.Size = new System.Drawing.Size(546, 61);
      this.searchControlPanel.TabIndex = 5;
      this.searchControlPanel.Visible = false;
      this.findStateControl.Location = new Point(3, 0);
      this.findStateControl.Name = "nodeSearchControl";
      this.findStateControl.Size = new System.Drawing.Size(531, 61);
      this.findStateControl.TabIndex = 0;
      this.displayPanel.AccessibleName = "DisplayPanel";
      this.displayPanel.Dock = DockStyle.Fill;
      this.displayPanel.Location = new Point(0, 86);
      this.displayPanel.Name = "displayPanel";
      this.displayPanel.Size = new System.Drawing.Size(546, 203);
      this.displayPanel.TabIndex = 6;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.Controls.Add((Control) this.displayPanel);
      this.Controls.Add((Control) this.searchControlPanel);
      this.Controls.Add((Control) this.comboBoxLabel);
      this.Controls.Add((Control) this.comboBox);
      this.Controls.Add((Control) this.viewDocumentTool);
      this.Controls.Add((Control) this.statusStrip);
      this.Name = nameof (ViewDocumentControl);
      this.Size = new System.Drawing.Size(546, 311);
      this.viewDocumentTool.ResumeLayout(false);
      this.viewDocumentTool.PerformLayout();
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
      this.searchControlPanel.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    private delegate void SetGViewerRenderingStatus(string str);
  }
}
