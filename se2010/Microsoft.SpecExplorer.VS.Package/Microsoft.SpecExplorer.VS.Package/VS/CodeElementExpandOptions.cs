using System;

namespace Microsoft.SpecExplorer.VS
{
	[Flags]
	public enum CodeElementExpandOptions
	{
		None = 0x0,
		ExpandToNamespaces = 0x1,
		ExpandToInterfaces = 0x2,
		ExpandToClasses = 0x4,
		ExpandToFunctions = 0x8,
		ExpandToEvents = 0x10,
		ExpandAll = 0x1F
	}
}
