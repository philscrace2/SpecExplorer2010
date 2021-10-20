using System;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	[Serializable]
	[XmlRoot("Guidance")]
	public class GuidanceImpl : IGuidance
	{
		private ActivityReferenceImpl[] structure;

		[XmlAttribute("Id")]
		public string Id { get; set; }

		[XmlAttribute("Description")]
		public string Description { get; set; }

		[XmlElement("Explanation")]
		public string Explanation { get; set; }

		[XmlIgnore]
		public IActivity[] Activities
		{
			get
			{
				return ActivitiesField;
			}
		}

		[XmlIgnore]
		public IActivityReference[] Structure
		{
			get
			{
				return structure;
			}
		}

		[XmlArrayItem("Activity")]
		[XmlArray("Activities")]
		public ActivityImpl[] ActivitiesField { get; set; }

		[XmlArray("Structure")]
		[XmlArrayItem("Activity")]
		public ActivityReferenceImpl[] StructureField
		{
			get
			{
				return structure;
			}
			set
			{
				structure = value;
				LoadReferences();
			}
		}

		private void LoadReferences()
		{
			if (structure == null || Activities == null)
			{
				return;
			}
			Array.ForEach(structure, delegate(ActivityReferenceImpl actRef)
			{
				actRef.Activity = Activities.First((IActivity act) => act.Id == actRef.RefId);
				actRef.Index = Array.IndexOf(structure, actRef) + 1;
			});
		}
	}
}
