using System.Collections.Generic;
using Microsoft.ActionMachines.Cord;

namespace Microsoft.SpecExplorer.VS
{
	internal class ActionDeclarationBuilder
	{
		private HashSet<ActionDeclaration> importActions = new HashSet<ActionDeclaration>();

		private HashSet<ActionDeclaration> excludedActions = new HashSet<ActionDeclaration>();

		internal IEnumerable<ActionDeclaration> ImportActions
		{
			get
			{
				return importActions;
			}
		}

		internal IEnumerable<ActionDeclaration> ExcludedActions
		{
			get
			{
				return excludedActions;
			}
		}

		internal void AddAction(MethodDescriptor method)
		{
			importActions.Add(new ActionDeclaration(method));
		}

		internal void ExcludeAction(MethodDescriptor method)
		{
			excludedActions.Add(new ActionDeclaration(method));
		}
	}
}
