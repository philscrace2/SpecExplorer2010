// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.ViewDefinition
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.Viewer
{
  [Serializable]
  public class ViewDefinition : IViewDefinition
  {
    [Description("The name of selected view")]
    [Category("Name")]
    [XmlElement("Name")]
    public string Name { get; set; }

    [XmlElement("SelectQuery")]
    [Browsable(false)]
    public SelectQuery SelectQuery { get; set; }

    [XmlIgnore]
    [Browsable(false)]
    public bool SelectQuerySpecified => !string.IsNullOrEmpty(this.SelectQuery.Query.Param);

    [Browsable(false)]
    public Query[] GroupQuery { get; set; }

    [Browsable(false)]
    [XmlIgnore]
    public bool GroupQuerySpecified => this.GroupQuery.Length > 1 || !string.IsNullOrEmpty(this.GroupQuery[0].Param);

    [DisplayName("GroupQuery")]
    [XmlIgnore]
    [Category("Query")]
    [Description("Group query will group nodes with same value (after applying this query) to one node.\r\nCurrent support query is full or short name of probe defined in model program.")]
    public string GroupQueryParam
    {
      get => ((IEnumerable<Query>) this.GroupQuery).First<Query>().Param;
      set => ((IEnumerable<Query>) this.GroupQuery).First<Query>().Param = value;
    }

    [Description("True to collapse steps with same source and target node to one step, default is false.")]
    [DisplayName("ViewCollapseLabels")]
    [XmlElement("ViewCollapseLabels")]
    public bool ViewCollapseLabels { get; set; }

    [Browsable(false)]
    [XmlIgnore]
    public bool ViewCollapseLabelsSpecified => this.ViewCollapseLabels;

    [Description("True to collapse call and return step into one compound step, default is true.")]
    [DisplayName("ViewCollapseSteps")]
    [XmlElement("ViewCollapseSteps")]
    public bool ViewCollapseSteps { get; set; }

    [XmlIgnore]
    [Browsable(false)]
    public bool ViewCollapseStepsSpecified => !this.ViewCollapseSteps;

    [XmlElement("NodeFillColor")]
    [Browsable(false)]
    public int NodeFillColorRgb { get; set; }

    [XmlIgnore]
    [Browsable(false)]
    public bool NodeFillColorRgbSpecified => this.NodeFillColorRgb != Color.White.ToArgb();

    [DisplayName("NodeFillColor")]
    [XmlIgnore]
    [Description("Specify fill color for normal node")]
    [Browsable(false)]
    [Category("Appearance")]
    public Color NodeFillColor
    {
      get => Color.FromArgb(this.NodeFillColorRgb);
      set => this.NodeFillColorRgb = value.ToArgb();
    }

    [XmlElement("EdgeColor")]
    [Browsable(false)]
    public int EdgeColorRgb { get; set; }

    [Browsable(false)]
    [XmlIgnore]
    public bool EdgeColorRgbSpecified => this.EdgeColorRgb != Color.Black.ToArgb();

    [DisplayName("EdgeColor")]
    [Browsable(false)]
    [Category("Appearance")]
    [Description("Specify color for normal edge")]
    [XmlIgnore]
    public Color EdgeColor
    {
      get => Color.FromArgb(this.EdgeColorRgb);
      set => this.EdgeColorRgb = value.ToArgb();
    }

    [Browsable(false)]
    [XmlIgnore]
    public bool IsDefault { get; internal set; }

    [Description("If set to none, states are labeled in the graph with an auto-generated unique identifier (e.g. S0, S1, etc.). In order to obtain more meaningful state labels, this switch must be set to the (fully qualified or short) identifier of a probe (a static property or parameter-less method with the Probe attribute) returning a string, defined in an assembly referenced by the project containing the Cord script with the explored machine")]
    [DisplayName("StateDescription")]
    [XmlIgnore]
    public string StateDescriptionParam
    {
      get => this.StateDescription.Param;
      set => this.StateDescription.Param = value;
    }

    [XmlElement("StateDescription")]
    [Browsable(false)]
    public Query StateDescription { get; set; }

    [Browsable(false)]
    [XmlIgnore]
    public bool StateDescriptionSpecified => !string.IsNullOrEmpty(this.StateDescription.Param);

    [DisplayName("RenderingTimeOut")]
    [XmlElement("RenderingTimeOut")]
    [Category("Rendering")]
    [Description("View display rendering time out(in seconds). If this value is less than or equals 0, rendering will never be aborted.")]
    public int RenderingTimeOut { get; set; }

    [XmlIgnore]
    [Browsable(false)]
    public bool RenderingTimeOutSpecified => this.RenderingTimeOut != 25;

    [XmlElement("DisplayRequirements")]
    [Description("True to display requirements on step, default is false.")]
    [DisplayName("DisplayRequirements")]
    public bool DisplayRequirements { get; set; }

    [XmlIgnore]
    [Browsable(false)]
    public bool DisplayRequirementsSpecified => this.DisplayRequirements;

    [XmlElement("ShowErrorPathsOnly")]
    [Description("When ShowErrorPath is True and in the case of an error condition(s), the display graph will be filtered to display only one or more paths from initial states to each of the error states, all other states will be suppressed.  The default value is False.")]
    [DisplayName("ShowErrorPathsOnly")]
    public bool ShowErrorPathsOnly { get; set; }

    [XmlIgnore]
    [Browsable(false)]
    public bool ShowErrorPathsOnlySpecified => this.ShowErrorPathsOnly;

    [Description("When ShowParameters is True, parameters will be displayed, otherwise only the action name is shown.  The default value is True.")]
    [DisplayName("ShowParameters")]
    [XmlElement("ShowParameters")]
    public bool ShowParameters { get; set; }

    [Browsable(false)]
    [XmlIgnore]
    public bool ShowParametersSpecified => !this.ShowParameters;

    [Browsable(false)]
    [XmlElement("HideQuery")]
    public HideQuery HideQuery { get; set; }

    [Browsable(false)]
    [XmlIgnore]
    public bool HideQuerySpecified => !string.IsNullOrEmpty(this.HideQuery.Query.Param);

    [Category("Query")]
    [DisplayName("HideQuery")]
    [Description("HideQuery specifies the name of a Boolean probe. States for which the probe is true are suppressed from display. Paths between remaining visible states that run through hidden states are displayed as dashed lines.")]
    [XmlIgnore]
    public string HideQueryParam
    {
      get => this.HideQuery.Query.Param;
      set => this.HideQuery.Query.Param = value;
    }

    public ViewDefinition()
    {
      this.NodeFillColor = Color.White;
      this.EdgeColor = Color.Black;
      this.SelectQuery = new SelectQuery();
      this.GroupQuery = new Query[1];
      this.StateDescription = new Query();
      this.GroupQuery[0] = new Query();
      this.ViewCollapseSteps = true;
      this.IsDefault = false;
      this.RenderingTimeOut = 25;
      this.DisplayRequirements = false;
      this.ShowErrorPathsOnly = false;
      this.ShowParameters = true;
      this.HideQuery = new HideQuery();
    }
  }
}
