using System.Collections.Generic;
using EnvDTE;
using Microsoft.ActionMachines.Cord;

namespace Microsoft.SpecExplorer.VS
{
	public delegate HashSet<ProcedureType> SourceBindingTypeProvider(Dictionary<ProcedureType, HashSet<MethodDescriptor>> methodDescriptors, IEnumerable<CodeElement> codeElements, out Dictionary<ProcedureType, CodeClass> existTypeBinding);
}
