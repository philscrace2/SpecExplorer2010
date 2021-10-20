namespace Microsoft.SpecExplorer.ModelingGuidance
{
	public interface IGuidance
	{
		string Id { get; }

		string Description { get; }

		string Explanation { get; }

		IActivity[] Activities { get; }

		IActivityReference[] Structure { get; }
	}
}
