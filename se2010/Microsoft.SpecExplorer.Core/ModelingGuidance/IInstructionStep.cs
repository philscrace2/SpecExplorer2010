namespace Microsoft.SpecExplorer.ModelingGuidance
{
	public interface IInstructionStep
	{
		string Title { get; }

		string StepDetails { get; }

		ICodeBlock Code { get; }

		bool IsOptional { get; }

		bool HasContent { get; }

		int Index { get; }

		bool IsInstructive { get; }
	}
}
