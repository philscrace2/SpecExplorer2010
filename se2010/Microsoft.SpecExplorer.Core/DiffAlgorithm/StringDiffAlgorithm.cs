using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.SpecExplorer.DiffAlgorithm
{
	internal sealed class StringDiffAlgorithm
	{
		private const int PartialMatchLimit = 1000;

		private const double PartialSameLimit = 0.9;

		private const double RawLinesGroupSimilarityLimit = 0.6;

		private const double HalfRawLinesGroupSimilarityLimit = 0.3;

		private string[] leftLines;

		private string[] rightLines;

		private bool intraLines;

		internal StringDiffAlgorithm(string left, string right, bool calcIntraLines)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			intraLines = calcIntraLines;
			leftLines = Array.ConvertAll(left.Split('\n'), (string text) => text.Trim('\r'));
			rightLines = Array.ConvertAll(right.Split('\n'), (string text) => text.Trim('\r'));
		}

		public IEnumerable<DiffBlockPair> Execute()
		{
			bool partialCompare = intraLines && leftLines.Length + rightLines.Length < 1000;
			IEnumerable<RunPair> runPairs = ((!partialCompare) ? LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence(leftLines, rightLines).RunPairs : LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence(leftLines, rightLines, StringRawSimilarity).RunPairs);
			foreach (RunPair pair in runPairs)
			{
				if (pair.IsIdentical)
				{
					int m = pair.LeftBegin;
					int n = pair.RightBegin;
					while (m < pair.LeftEnd)
					{
						if (!partialCompare || leftLines[m] == rightLines[n])
						{
							yield return new DiffBlockPair(leftLines[m], rightLines[n], DiffType.Identical);
						}
						else
						{
							foreach (DiffBlockPair item in MatchIntraLines(leftLines[m], rightLines[n]))
							{
								yield return item;
							}
						}
						m++;
						n++;
					}
				}
				else if (pair.LeftBegin == pair.LeftEnd)
				{
					for (int l = pair.RightBegin; l < pair.RightEnd; l++)
					{
						yield return new DiffBlockPair(string.Empty, rightLines[l], DiffType.Inserted);
					}
				}
				else if (pair.RightBegin == pair.RightEnd)
				{
					for (int k = pair.LeftBegin; k < pair.LeftEnd; k++)
					{
						yield return new DiffBlockPair(leftLines[k], string.Empty, DiffType.Deleted);
					}
				}
				else if (intraLines)
				{
					foreach (DiffBlockPair item2 in LinesCompare(pair.LeftBegin, pair.LeftEnd, pair.RightBegin, pair.RightEnd))
					{
						yield return item2;
					}
				}
				else
				{
					for (int j = pair.LeftBegin; j < pair.LeftEnd; j++)
					{
						yield return new DiffBlockPair(leftLines[j], string.Empty, DiffType.Deleted);
					}
					for (int i = pair.RightBegin; i < pair.RightEnd; i++)
					{
						yield return new DiffBlockPair(string.Empty, rightLines[i], DiffType.Inserted);
					}
				}
			}
		}

		private static IEnumerable<string> Tokenize(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				yield break;
			}
			bool lastCharIsAlphaNum = false;
			bool beginningOfWord = true;
			StringBuilder tokenConnector = new StringBuilder();
			try
			{
				foreach (char ch in text)
				{
					if (ch != '\r')
					{
						if (!beginningOfWord && (!char.IsLetterOrDigit(ch) || !lastCharIsAlphaNum))
						{
							yield return tokenConnector.ToString();
							tokenConnector.Length = 0;
						}
						beginningOfWord = false;
						lastCharIsAlphaNum = char.IsLetterOrDigit(ch);
						tokenConnector.Append(ch);
					}
				}
			}
			finally
			{
			}
			yield return tokenConnector.ToString();
		}

		private static string Combine(List<string> list, int from, int to)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = from; i < to; i++)
			{
				stringBuilder.Append(list[i]);
			}
			return stringBuilder.ToString();
		}

		private static DiffBlockPair ConvertTokensToBlock(List<DiffTokenPair> Elements)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			DiffType diffType = Elements[0].Type;
			foreach (DiffTokenPair Element in Elements)
			{
				stringBuilder.Append(Element.Left);
				stringBuilder2.Append(Element.Right);
				if (diffType != Element.Type)
				{
					diffType = DiffType.Changed;
				}
			}
			return new DiffBlockPair(stringBuilder.ToString(), stringBuilder2.ToString(), diffType, Elements);
		}

		private static IEnumerable<DiffBlockPair> MatchIntraLines(string leftLine, string rightLine)
		{
			if (leftLine == null)
			{
				throw new ArgumentNullException("leftLine");
			}
			if (rightLine == null)
			{
				throw new ArgumentNullException("rightLine");
			}
			List<string> left = new List<string>(Tokenize(leftLine));
			List<string> right = new List<string>(Tokenize(rightLine));
			IEnumerable<RunPair> ilist = LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence(left, right).RunPairs;
			List<DiffTokenPair> subTokens = new List<DiffTokenPair>();
			foreach (RunPair pair in ilist)
			{
				if (pair.IsIdentical)
				{
					int k = pair.LeftBegin;
					int l = pair.RightBegin;
					while (k < pair.LeftEnd)
					{
						if (left[k] == "\n")
						{
							if (subTokens.Count > 0)
							{
								yield return ConvertTokensToBlock(subTokens);
								subTokens = new List<DiffTokenPair>();
							}
						}
						else
						{
							subTokens.Add(new DiffTokenPair(left[k], right[l], DiffType.Identical));
						}
						k++;
						l++;
					}
				}
				else if (pair.LeftBegin == pair.LeftEnd)
				{
					for (int j = pair.RightBegin; j < pair.RightEnd; j++)
					{
						if (right[j] == "\n")
						{
							if (subTokens.Count > 0)
							{
								yield return ConvertTokensToBlock(subTokens);
								subTokens = new List<DiffTokenPair>();
							}
						}
						else
						{
							subTokens.Add(new DiffTokenPair("", right[j], DiffType.Inserted));
						}
					}
				}
				else if (pair.RightBegin == pair.RightEnd)
				{
					for (int i = pair.LeftBegin; i < pair.LeftEnd; i++)
					{
						if (left[i] == "\n")
						{
							if (subTokens.Count > 0)
							{
								yield return ConvertTokensToBlock(subTokens);
								subTokens = new List<DiffTokenPair>();
							}
						}
						else
						{
							subTokens.Add(new DiffTokenPair(left[i], "", DiffType.Deleted));
						}
					}
				}
				else
				{
					subTokens.Add(new DiffTokenPair(Combine(left, pair.LeftBegin, pair.LeftEnd), Combine(right, pair.RightBegin, pair.RightEnd), DiffType.Changed));
				}
			}
			if (subTokens.Count > 0)
			{
				yield return ConvertTokensToBlock(subTokens);
			}
		}

		private IEnumerable<DiffBlockPair> LinesCompare(int leftStart, int leftStop, int rightStart, int rightStop)
		{
			StringBuilder leftStringBuilder = new StringBuilder();
			StringBuilder rightStringBuilder = new StringBuilder();
			for (int i = leftStart; i < leftStop; i++)
			{
				leftStringBuilder.Append(leftLines[i]);
				leftStringBuilder.Append("\n");
			}
			for (int j = rightStart; j < rightStop; j++)
			{
				rightStringBuilder.Append(rightLines[j]);
				rightStringBuilder.Append("\n");
			}
			foreach (DiffBlockPair pair in MatchIntraLines(leftStringBuilder.ToString(), rightStringBuilder.ToString()))
			{
				if (pair.Type == DiffType.Changed)
				{
					int max = LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence(pair.Left, pair.Right).Count;
					if ((double)max <= 0.3 * (double)(pair.Left.Length + pair.Right.Length))
					{
						try
						{
							string[] array = pair.Left.Split('\n');
							foreach (string leftLine in array)
							{
								yield return new DiffBlockPair(leftLine, "", DiffType.Deleted);
							}
						}
						finally
						{
						}
						try
						{
							string[] array2 = pair.Right.Split('\n');
							foreach (string rightLine in array2)
							{
								yield return new DiffBlockPair("", rightLine, DiffType.Inserted);
							}
						}
						finally
						{
						}
					}
					else
					{
						yield return pair;
					}
				}
				else
				{
					yield return pair;
				}
			}
		}

		private static bool StringRawSimilarity(string left, string right)
		{
			if (left.Length == 0)
			{
				return right.Length == 0;
			}
			if (right.Length == 0)
			{
				return false;
			}
			if ((double)Math.Abs(left.Length - right.Length) > 0.099999999999999978 * (double)(left.Length + right.Length))
			{
				return false;
			}
			int count = LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence(left.Substring(0, left.Length / 2), right.Substring(0, right.Length / 2)).Count;
			int count2 = LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence(left.Substring(left.Length / 2, left.Length - left.Length / 2), right.Substring(right.Length / 2, right.Length - right.Length / 2)).Count;
			int num = count + count2;
			return (double)num > 0.45 * (double)(left.Length + right.Length);
		}
	}
}
