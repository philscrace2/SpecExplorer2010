using System;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	[Serializable]
	public class ActivityImpl : IActivity
	{
		[XmlAttribute("Id")]
		public string Id { get; set; }

		[XmlAttribute("Description")]
		public string Description { get; set; }

		[XmlElement("Explanation")]
		public string Explanation { get; set; }

		[XmlIgnore]
		public IInstructions Instructions
		{
			get
			{
				return IntructionsField;
			}
		}

		[XmlElement("Instructions")]
		public InstructionsImpl IntructionsField { get; set; }
	}
}
