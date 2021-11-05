using System.Collections.Generic;
using Microsoft.ActionMachines.Cord;

namespace Microsoft.SpecExplorer.VS
{
	public delegate bool ActionConfigClauseResolver(IEnumerable<ConfigClause> selectedActions, out Dictionary<ProcedureType, HashSet<MethodDescriptor>> methodDescriptors, out HashSet<ProcedureType> adapterTypes);
}
