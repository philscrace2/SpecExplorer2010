using System;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer.DiffAlgorithm
{
	internal static class LongestCommonSubsequenceAlgorithm
	{
		private sealed class IncreasingList
		{
			internal sealed class LinkedPair
			{
				public LinkedPair LastPair { get; private set; }

				public int Left { get; private set; }

				public int Right { get; private set; }

				internal LinkedPair(LinkedPair lastPair, int left, int right)
				{
					LastPair = lastPair;
					Left = left;
					Right = right;
				}

				public override string ToString()
				{
					return "(" + Left + "," + Right + ")";
				}
			}

			private LinkedPair[] currentList;

			private int leftMax;

			private int rightMax;

			private IEnumerable<LinkedPair> Pairs
			{
				get
				{
					LinkedList<LinkedPair> linkedList = new LinkedList<LinkedPair>();
					if (Count > 0)
					{
						for (LinkedPair linkedPair = currentList[Count - 1]; linkedPair != null; linkedPair = linkedPair.LastPair)
						{
							linkedList.AddFirst(linkedPair);
						}
					}
					return linkedList;
				}
			}

			public int Count { get; private set; }

			public IEnumerable<RunPair> RunPairs
			{
				get
				{
					int left = 0;
					int right = 0;
					int lastLeft = 0;
					int lastRight = 0;
					foreach (LinkedPair pair in Pairs)
					{
						if (pair.Left > left || pair.Right > right)
						{
							if (left > 0)
							{
								yield return new RunPair(lastLeft, left, lastRight, right, true);
							}
							yield return new RunPair(left, pair.Left, right, pair.Right, false);
							lastLeft = pair.Left;
							lastRight = pair.Right;
						}
						left = pair.Left + 1;
						right = pair.Right + 1;
					}
					if (lastLeft < leftMax || lastRight < rightMax)
					{
						if (left > lastLeft && right > lastRight)
						{
							yield return new RunPair(lastLeft, left, lastRight, right, true);
						}
						if (left < leftMax || right < rightMax)
						{
							yield return new RunPair(left, leftMax, right, rightMax, false);
						}
					}
				}
			}

			public IncreasingList(int leftMax, int rightMax)
			{
				this.leftMax = leftMax;
				this.rightMax = rightMax;
				currentList = new LinkedPair[rightMax];
			}

			public void Increase(int leftIndex, int[] rightIndics)
			{
				if (rightIndics == null)
				{
					throw new ArgumentNullException("rightIndics");
				}
				if (rightIndics.Length == 0)
				{
					return;
				}
				if (Count == 0)
				{
					currentList[Count++] = new LinkedPair(null, leftIndex, rightIndics[0]);
					return;
				}
				if (rightIndics[rightIndics.Length - 1] <= currentList[0].Right)
				{
					currentList[0] = new LinkedPair(null, leftIndex, rightIndics[0]);
					return;
				}
				int num = Count - 1;
				int num2 = rightIndics.Length - 1;
				while (num2 >= 0)
				{
					if (rightIndics[num2] == currentList[num].Right)
					{
						num2--;
					}
					else if (rightIndics[num2] > currentList[num].Right)
					{
						if (num == Count - 1)
						{
							while (num2 > 0 && rightIndics[num2 - 1] > currentList[num].Right)
							{
								num2--;
							}
							currentList[Count++] = new LinkedPair(currentList[num], leftIndex, rightIndics[num2]);
						}
						else
						{
							while (num2 > 0 && rightIndics[num2 - 1] > currentList[num].Right)
							{
								num2--;
							}
							currentList[num + 1] = new LinkedPair(currentList[num], leftIndex, rightIndics[num2]);
						}
						num2--;
					}
					else
					{
						num--;
						if (num == -1)
						{
							currentList[0] = new LinkedPair(null, leftIndex, rightIndics[0]);
							break;
						}
					}
				}
			}
		}

		public static PairSequence CalculateLongestCommonSubsequence<Element>(IEnumerable<Element> left, IEnumerable<Element> right, Func<Element, Element, bool> approximateMatcher)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			Dictionary<Element, List<int>> dictionary = new Dictionary<Element, List<int>>();
			int num = 0;
			foreach (Element item in left)
			{
				num++;
				if (!dictionary.ContainsKey(item))
				{
					dictionary[item] = new List<int>();
				}
			}
			if (num == 0)
			{
				return new PairSequence();
			}
			int num2 = EnlistMatchedIndics(right, approximateMatcher, dictionary);
			if (num2 == 0)
			{
				return new PairSequence();
			}
			Dictionary<Element, int[]> dictionary2 = new Dictionary<Element, int[]>();
			foreach (KeyValuePair<Element, List<int>> item2 in dictionary)
			{
				if (item2.Value.Count > 0)
				{
					dictionary2[item2.Key] = item2.Value.ToArray();
				}
			}
			IncreasingList increasingList = new IncreasingList(num, num2);
			int num3 = 0;
			foreach (Element item3 in left)
			{
				int[] value;
				if (dictionary2.TryGetValue(item3, out value))
				{
					increasingList.Increase(num3, value);
				}
				num3++;
			}
			return new PairSequence(increasingList.RunPairs, increasingList.Count);
		}

		public static PairSequence CalculateLongestCommonSubsequence<Element>(IEnumerable<Element> left, IEnumerable<Element> right)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			return CalculateLongestCommonSubsequence(left, right, null);
		}

		private static int EnlistMatchedIndics<Element>(IEnumerable<Element> right, Func<Element, Element, bool> equator, Dictionary<Element, List<int>> matchedIndexLists)
		{
			int num = 0;
			if (equator == null)
			{
				foreach (Element item in right)
				{
					List<int> value;
					if (matchedIndexLists.TryGetValue(item, out value))
					{
						value.Add(num);
					}
					num++;
				}
				return num;
			}
			foreach (Element item2 in right)
			{
				foreach (KeyValuePair<Element, List<int>> matchedIndexList in matchedIndexLists)
				{
					if (equator(matchedIndexList.Key, item2))
					{
						matchedIndexList.Value.Add(num);
					}
				}
				num++;
			}
			return num;
		}
	}
}
