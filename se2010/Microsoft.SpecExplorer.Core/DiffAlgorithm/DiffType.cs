using System;

namespace Microsoft.SpecExplorer.DiffAlgorithm
{
	[Serializable]
	internal enum DiffType
	{
		Identical,
		Deleted,
		Inserted,
		Changed
	}
}
