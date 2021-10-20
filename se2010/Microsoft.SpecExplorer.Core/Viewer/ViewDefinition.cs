using System;
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
		public bool SelectQuerySpecified
		{
			get
			{
				return !string.IsNullOrEmpty(SelectQuery.Query.Param);
			}
		}

		[Browsable(false)]
		public Query[] GroupQuery { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public bool GroupQuerySpecified
		{
			get
			{
				if (GroupQuery.Length <= 1)
				{
					return !string.IsNullOrEmpty(GroupQuery[0].Param);
				}
				return true;
			}
		}

		[DisplayName("GroupQuery")]
		[XmlIgnore]
		[Category("Query")]
		[Description("Group query will group nodes with same value (after applying this query) to one node.\r\nCurrent support query is full or short name of probe defined in model program.")]
		public string GroupQueryParam
		{
			get
			{
				return GroupQuery.First().Param;
			}
			set
			{
				GroupQuery.First().Param = value;
			}
		}

		[Description("True to collapse steps with same source and target node to one step, default is false.")]
		[DisplayName("ViewCollapseLabels")]
		[XmlElement("ViewCollapseLabels")]
		public bool ViewCollapseLabels { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public bool ViewCollapseLabelsSpecified
		{
			get
			{
				return ViewCollapseLabels;
			}
		}

		[Description("True to collapse call and return step into one compound step, default is true.")]
		[DisplayName("ViewCollapseSteps")]
		[XmlElement("ViewCollapseSteps")]
		public bool ViewCollapseSteps { get; set; }

		[XmlIgnore]
		[Browsable(false)]
		public bool ViewCollapseStepsSpecified
		{
			get
			{
				return !ViewCollapseSteps;
			}
		}

		[XmlElement("NodeFillColor")]
		[Browsable(false)]
		public int NodeFillColorRgb { get; set; }

		[XmlIgnore]
		[Browsable(false)]
		public bool NodeFillColorRgbSpecified
		{
			get
			{
				return NodeFillColorRgb != Color.White.ToArgb();
			}
		}

		[DisplayName("NodeFillColor")]
		[XmlIgnore]
		[Description("Specify fill color for normal node")]
		[Browsable(false)]
		[Category("Appearance")]
		public Color NodeFillColor
		{
			get
			{
				return Color.FromArgb(NodeFillColorRgb);
			}
			set
			{
				NodeFillColorRgb = value.ToArgb();
			}
		}

		[XmlElement("EdgeColor")]
		[Browsable(false)]
		public int EdgeColorRgb { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public bool EdgeColorRgbSpecified
		{
			get
			{
				return EdgeColorRgb != Color.Black.ToArgb();
			}
		}

		[DisplayName("EdgeColor")]
		[Browsable(false)]
		[Category("Appearance")]
		[Description("Specify color for normal edge")]
		[XmlIgnore]
		public Color EdgeColor
		{
			get
			{
				return Color.FromArgb(EdgeColorRgb);
			}
			set
			{
				EdgeColorRgb = value.ToArgb();
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool IsDefault { get; internal set; }

		[Description("If set to none, states are labeled in the graph with an auto-generated unique identifier (e.g. S0, S1, etc.). In order to obtain more meaningful state labels, this switch must be set to the (fully qualified or short) identifier of a probe (a static property or parameter-less method with the Probe attribute) returning a string, defined in an assembly referenced by the project containing the Cord script with the explored machine")]
		[DisplayName("StateDescription")]
		[XmlIgnore]
		public string StateDescriptionParam
		{
			get
			{
				return StateDescription.Param;
			}
			set
			{
				StateDescription.Param = value;
			}
		}

		[XmlElement("StateDescription")]
		[Browsable(false)]
		public Query StateDescription { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public bool StateDescriptionSpecified
		{
			get
			{
				return !string.IsNullOrEmpty(StateDescription.Param);
			}
		}

		[DisplayName("RenderingTimeOut")]
		[XmlElement("RenderingTimeOut")]
		[Category("Rendering")]
		[Description("View display rendering time out(in seconds). If this value is less than or equals 0, rendering will never be aborted.")]
		public int RenderingTimeOut { get; set; }

		[XmlIgnore]
		[Browsable(false)]
		public bool RenderingTimeOutSpecified
		{
			get
			{
				return RenderingTimeOut != 25;
			}
		}

		[XmlElement("DisplayRequirements")]
		[Description("True to display requirements on step, default is false.")]
		[DisplayName("DisplayRequirements")]
		public bool DisplayRequirements { get; set; }

		[XmlIgnore]
		[Browsable(false)]
		public bool DisplayRequirementsSpecified
		{
			get
			{
				return DisplayRequirements;
			}
		}

		[XmlElement("ShowErrorPathsOnly")]
		[Description("When ShowErrorPath is True and in the case of an error condition(s), the display graph will be filtered to display only one or more paths from initial states to each of the error states, all other states will be suppressed.  The default value is False.")]
		[DisplayName("ShowErrorPathsOnly")]
		public bool ShowErrorPathsOnly { get; set; }

		[XmlIgnore]
		[Browsable(false)]
		public bool ShowErrorPathsOnlySpecified
		{
			get
			{
				return ShowErrorPathsOnly;
			}
		}

		[Description("When ShowParameters is True, parameters will be displayed, otherwise only the action name is shown.  The default value is True.")]
		[DisplayName("ShowParameters")]
		[XmlElement("ShowParameters")]
		public bool ShowParameters { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public bool ShowParametersSpecified
		{
			get
			{
				return !ShowParameters;
			}
		}

		[Browsable(false)]
		[XmlElement("HideQuery")]
		public HideQuery HideQuery { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public bool HideQuerySpecified
		{
			get
			{
				return !string.IsNullOrEmpty(HideQuery.Query.Param);
			}
		}

		[Category("Query")]
		[DisplayName("HideQuery")]
		[Description("HideQuery specifies the name of a Boolean probe. States for which the probe is true are suppressed from display. Paths between remaining visible states that run through hidden states are displayed as dashed lines.")]
		[XmlIgnore]
		public string HideQueryParam
		{
			get
			{
				return HideQuery.Query.Param;
			}
			set
			{
				HideQuery.Query.Param = value;
			}
		}

		public ViewDefinition()
		{
			NodeFillColor = Color.White;
			EdgeColor = Color.Black;
			SelectQuery = new SelectQuery();
			GroupQuery = new Query[1];
			StateDescription = new Query();
			GroupQuery[0] = new Query();
			ViewCollapseSteps = true;
			IsDefault = false;
			RenderingTimeOut = 25;
			DisplayRequirements = false;
			ShowErrorPathsOnly = false;
			ShowParameters = true;
			HideQuery = new HideQuery();
		}
	}
}
