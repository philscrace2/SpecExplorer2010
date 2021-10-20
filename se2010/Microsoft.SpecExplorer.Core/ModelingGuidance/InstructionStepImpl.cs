using System;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	[Serializable]
	[XmlType("InstructionStep")]
	public class InstructionStepImpl : IInstructionStep
	{
		[XmlAttribute("Header")]
		public string Title { get; set; }

		[XmlElement("StepDetails")]
		public string StepDetails { get; set; }

		[XmlAttribute("Optional")]
		public bool IsOptional { get; set; }

		[XmlIgnore]
		public ICodeBlock Code
		{
			get
			{
				return CodeField;
			}
		}

		[XmlIgnore]
		public bool HasContent
		{
			get
			{
				if (StepDetails == null)
				{
					return Code != null;
				}
				return true;
			}
		}

		[XmlIgnore]
		public int Index { get; set; }

		[XmlAttribute("Instructive")]
		public bool IsInstructive { get; set; }

		[XmlElement("Code")]
		public CodeBlockImpl CodeField { get; set; }
	}
}
