namespace Microsoft.SpecExplorer
{
	public enum ExplorationState
	{
		Created,
		Building,
		FailedBuilding,
		FinishedBuilding,
		Exploring,
		SuspendedExploring,
		FinishedExploring,
		Aborted
	}
}
