using System;

namespace Microsoft.SpecExplorer.VS
{
	public sealed class SolutionBuildEventArgs : EventArgs
	{
		public bool IsCanceled { get; private set; }

		public bool IsSucceeded { get; private set; }

		public SolutionBuildEventArgs(bool isCanceled, bool isSucceeded)
		{
			IsCanceled = isCanceled;
			IsSucceeded = isSucceeded;
		}
	}
}
