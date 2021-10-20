namespace Microsoft.SpecExplorer.ModelingGuidance
{
	public interface IActivity
	{
		string Id { get; }

		string Description { get; }

		string Explanation { get; }

		IInstructions Instructions { get; }
	}
}
