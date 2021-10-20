using System;

namespace Microsoft.SpecExplorer.DiffAlgorithm
{
	[Serializable]
	internal sealed class DiffTokenPair
	{
		public DiffType Type { get; private set; }

		public string Left { get; private set; }

		public string Right { get; private set; }

		internal DiffTokenPair(string left, string right, DiffType type)
		{
			Left = left;
			Right = right;
			Type = type;
		}
	}
}
