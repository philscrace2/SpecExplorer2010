using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer.DiffAlgorithm
{
	internal sealed class PairSequence
	{
		public IEnumerable<RunPair> RunPairs { get; private set; }

		public int Count { get; private set; }

		internal PairSequence()
			: this(Enumerable.Empty<RunPair>(), 0)
		{
		}

		internal PairSequence(IEnumerable<RunPair> runPairs, int count)
		{
			RunPairs = runPairs;
			Count = count;
		}
	}
}
