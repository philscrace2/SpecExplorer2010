using System;

namespace Microsoft.SpecExplorer.Extensions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class SpecExplorerExtensionAttribute : Attribute
	{
	}
}
