namespace Microsoft.SpecExplorer.ModelingGuidance
{
	public interface ICodeBlock
	{
		string FormattedText { get; }

		string RawText { get; }

		string Language { get; }
	}
}
