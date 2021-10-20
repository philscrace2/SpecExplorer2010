namespace Microsoft.SpecExplorer.ModelingGuidance
{
	public interface IInstructions
	{
		string Prerequisites { get; }

		IInstructionStep[] Steps { get; }
	}
}
