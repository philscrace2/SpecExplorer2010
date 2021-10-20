using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer.Viewer
{
	public struct BrowserEdge
	{
		public string Text { get; private set; }

		public string ActionText { get; private set; }

		public State Source { get; private set; }

		public State Target { get; private set; }

		public string[] PreConstraints { get; private set; }

		public string[] PostConstraints { get; private set; }

		public string[] unboundVariables { get; private set; }

		public string[] CapturedRequirements { get; private set; }

		public string[] AssumeCapturedRequirements { get; private set; }

		public BrowserEdge(string text, string actionText, State source, State target, string[] preConstraints, string[] postConstraints, string[] unboundVariables, string[] capturedRequirements, string[] assumeCapturedRequirements)
		{
			this = default(BrowserEdge);
			Text = text;
			ActionText = actionText;
			Source = source;
			Target = target;
			PreConstraints = preConstraints;
			PostConstraints = postConstraints;
			this.unboundVariables = unboundVariables;
			CapturedRequirements = capturedRequirements;
			AssumeCapturedRequirements = assumeCapturedRequirements;
		}
	}
}
