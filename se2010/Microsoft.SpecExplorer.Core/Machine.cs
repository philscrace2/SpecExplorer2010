namespace Microsoft.SpecExplorer
{
	public class Machine
	{
		public string Name { get; set; }

		public MachineContainer Container { get; set; }

		public string TestEnabled { get; set; }

		public string Description { get; set; }

		public string Group { get; set; }

		public string RecommendedViews { get; set; }
	}
}
