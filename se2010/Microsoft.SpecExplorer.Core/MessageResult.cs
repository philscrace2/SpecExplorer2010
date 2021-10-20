using System;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	public enum MessageResult
	{
		None,
		OK,
		CANCEL,
		ABORT,
		RETRY,
		IGNORE,
		YES,
		NO
	}
}
