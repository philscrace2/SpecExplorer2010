using System.Collections.Generic;

namespace Microsoft.SpecExplorer.VS
{
	public class ScriptManipulationReport
	{
		public ICollection<string> InsertedClauses { get; private set; }

		public ScriptManipulationReport()
		{
			InsertedClauses = new List<string>();
		}
	}
}
