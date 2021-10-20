using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer.DiffAlgorithm
{
	[Serializable]
	internal sealed class DiffBlockPair
	{
		internal DiffType Type { get; private set; }

		internal string Left { get; private set; }

		internal string Right { get; private set; }

		internal IEnumerable<DiffTokenPair> TokenPairs { get; set; }

		internal DiffBlockPair(string left, string right, DiffType type, IEnumerable<DiffTokenPair> elements)
		{
			Left = left;
			Right = right;
			Type = type;
			TokenPairs = elements;
		}

		internal DiffBlockPair(string left, string right, DiffType type)
			: this(left, right, type, Enumerable.Empty<DiffTokenPair>())
		{
		}
	}
}
