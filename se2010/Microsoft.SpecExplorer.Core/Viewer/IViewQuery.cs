using System.Collections.Generic;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer.Viewer
{
	public interface IViewQuery
	{
		string GetLabel(State state);

		IEnumerable<DisplayNode> GetHyperNodes(ICollection<DisplayNode> nodes);

		void DivideHyperNodes(DisplayNode parentNode);

		bool ValidateViewQuery(TransitionSystem transitionSystem, out string errorMessage);
	}
}
