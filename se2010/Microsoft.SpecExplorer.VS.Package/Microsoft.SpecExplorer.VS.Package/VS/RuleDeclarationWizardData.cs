using System.Collections.Generic;
using EnvDTE;
using Microsoft.ActionMachines.Cord;

namespace Microsoft.SpecExplorer.VS
{
	public class RuleDeclarationWizardData
	{
		public ConfigInfo ConfigInfo { get; set; }

		public Dictionary<ProcedureType, HashSet<MethodDescriptor>> MethodDescriptors { get; set; }

		public HashSet<ProcedureType> AdapterTypes { get; set; }

		public Dictionary<ProcedureType, CodeClass> HostClassMap { get; private set; }

		public Dictionary<ProcedureType, CodeClass> TypeBindingMap { get; private set; }

		public RuleDeclarationWizardData()
		{
			HostClassMap = new Dictionary<ProcedureType, CodeClass>();
			TypeBindingMap = new Dictionary<ProcedureType, CodeClass>();
		}
	}
}
