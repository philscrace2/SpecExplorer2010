using System.Drawing;

namespace Microsoft.SpecExplorer.Viewer
{
	public interface IViewDefinition
	{
		string Name { get; }

		Color NodeFillColor { get; set; }

		Color EdgeColor { get; set; }

		bool ViewCollapseLabels { get; set; }

		bool ViewCollapseSteps { get; set; }

		bool IsDefault { get; }

		int RenderingTimeOut { get; set; }
	}
}
