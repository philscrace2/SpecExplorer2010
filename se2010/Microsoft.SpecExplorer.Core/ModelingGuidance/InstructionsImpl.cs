using System;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	[Serializable]
	public class InstructionsImpl : IInstructions
	{
		private InstructionStepImpl[] steps;

		[XmlElement("Prerequisites")]
		public string Prerequisites { get; set; }

		[XmlIgnore]
		public IInstructionStep[] Steps
		{
			get
			{
				return steps;
			}
		}

		[XmlElement("Step")]
		public InstructionStepImpl[] StepsField
		{
			get
			{
				return steps;
			}
			set
			{
				steps = value;
				IndexSteps();
			}
		}

		private void IndexSteps()
		{
			int index = 0;
			Array.ForEach(steps, delegate(InstructionStepImpl step)
			{
				step.Index = (step.IsInstructive ? (++index) : 0);
			});
		}
	}
}
