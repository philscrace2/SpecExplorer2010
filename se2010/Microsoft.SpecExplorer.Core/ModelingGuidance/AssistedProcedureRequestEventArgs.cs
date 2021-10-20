using System;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	public class AssistedProcedureRequestEventArgs : EventArgs
	{
		public uint ProcedureId { get; private set; }

		public AssistedProcedureRequestEventArgs(uint procedureId)
		{
			ProcedureId = procedureId;
		}
	}
}
