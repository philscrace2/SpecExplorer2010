namespace Microsoft.SpecExplorer.DiffAlgorithm
{
	internal sealed class RunPair
	{
		public int LeftBegin { get; private set; }

		public int LeftEnd { get; private set; }

		public int RightBegin { get; private set; }

		public int RightEnd { get; private set; }

		public bool IsIdentical { get; private set; }

		internal RunPair(int leftBegin, int leftEnd, int rightBegin, int rightEnd, bool isIdentical)
		{
			LeftBegin = leftBegin;
			RightBegin = rightBegin;
			LeftEnd = leftEnd;
			RightEnd = rightEnd;
			IsIdentical = isIdentical;
		}

		public override string ToString()
		{
			return "[" + LeftBegin + "," + LeftEnd + ") <==> [" + RightBegin + "," + RightEnd + ")";
		}
	}
}
