using System;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer.Viewer
{
	public sealed class ViewDefinitionUpdateEventArgs : EventArgs
	{
		public IEnumerable<IViewDefinition> UpdatedViewDefinitions;

		public ViewDefinitionUpdateEventArgs(IEnumerable<IViewDefinition> updatedViewDefinitions)
		{
			UpdatedViewDefinitions = updatedViewDefinitions;
		}
	}
}
