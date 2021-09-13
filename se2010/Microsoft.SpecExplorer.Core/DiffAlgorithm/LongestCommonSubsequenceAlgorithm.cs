// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.DiffAlgorithm.LongestCommonSubsequenceAlgorithm
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer.DiffAlgorithm
{
  internal static class LongestCommonSubsequenceAlgorithm
  {
    public static PairSequence CalculateLongestCommonSubsequence<Element>(
      IEnumerable<Element> left,
      IEnumerable<Element> right,
      Func<Element, Element, bool> approximateMatcher)
    {
      if (left == null)
        throw new ArgumentNullException(nameof (left));
      if (right == null)
        throw new ArgumentNullException(nameof (right));
      Dictionary<Element, List<int>> matchedIndexLists = new Dictionary<Element, List<int>>();
      int leftMax = 0;
      foreach (Element key in left)
      {
        ++leftMax;
        if (!matchedIndexLists.ContainsKey(key))
          matchedIndexLists[key] = new List<int>();
      }
      if (leftMax == 0)
        return new PairSequence();
      int rightMax = LongestCommonSubsequenceAlgorithm.EnlistMatchedIndics<Element>(right, approximateMatcher, matchedIndexLists);
      if (rightMax == 0)
        return new PairSequence();
      Dictionary<Element, int[]> dictionary = new Dictionary<Element, int[]>();
      foreach (KeyValuePair<Element, List<int>> keyValuePair in matchedIndexLists)
      {
        if (keyValuePair.Value.Count > 0)
          dictionary[keyValuePair.Key] = keyValuePair.Value.ToArray();
      }
      LongestCommonSubsequenceAlgorithm.IncreasingList increasingList = new LongestCommonSubsequenceAlgorithm.IncreasingList(leftMax, rightMax);
      int leftIndex = 0;
      foreach (Element key in left)
      {
        int[] rightIndics;
        if (dictionary.TryGetValue(key, out rightIndics))
          increasingList.Increase(leftIndex, rightIndics);
        ++leftIndex;
      }
      return new PairSequence(increasingList.RunPairs, increasingList.Count);
    }

    public static PairSequence CalculateLongestCommonSubsequence<Element>(
      IEnumerable<Element> left,
      IEnumerable<Element> right)
    {
      if (left == null)
        throw new ArgumentNullException(nameof (left));
      return right != null ? LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence<Element>(left, right, (Func<Element, Element, bool>) null) : throw new ArgumentNullException(nameof (right));
    }

    private static int EnlistMatchedIndics<Element>(
      IEnumerable<Element> right,
      Func<Element, Element, bool> equator,
      Dictionary<Element, List<int>> matchedIndexLists)
    {
      int num = 0;
      if (equator == null)
      {
        foreach (Element key in right)
        {
          List<int> intList;
          if (matchedIndexLists.TryGetValue(key, out intList))
            intList.Add(num);
          ++num;
        }
      }
      else
      {
        foreach (Element element in right)
        {
          foreach (KeyValuePair<Element, List<int>> matchedIndexList in matchedIndexLists)
          {
            if (equator(matchedIndexList.Key, element))
              matchedIndexList.Value.Add(num);
          }
          ++num;
        }
      }
      return num;
    }

    private sealed class IncreasingList
    {
      private LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair[] currentList;
      private int leftMax;
      private int rightMax;

      public IncreasingList(int leftMax, int rightMax)
      {
        this.leftMax = leftMax;
        this.rightMax = rightMax;
        this.currentList = new LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair[rightMax];
      }

      public void Increase(int leftIndex, int[] rightIndics)
      {
        if (rightIndics == null)
          throw new ArgumentNullException(nameof (rightIndics));
        if (rightIndics.Length == 0)
          return;
        if (this.Count == 0)
          this.currentList[this.Count++] = new LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair((LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair) null, leftIndex, rightIndics[0]);
        else if (rightIndics[rightIndics.Length - 1] <= this.currentList[0].Right)
        {
          this.currentList[0] = new LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair((LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair) null, leftIndex, rightIndics[0]);
        }
        else
        {
          int index1 = this.Count - 1;
          int index2 = rightIndics.Length - 1;
          while (index2 >= 0)
          {
            if (rightIndics[index2] == this.currentList[index1].Right)
              --index2;
            else if (rightIndics[index2] > this.currentList[index1].Right)
            {
              if (index1 == this.Count - 1)
              {
                while (index2 > 0 && rightIndics[index2 - 1] > this.currentList[index1].Right)
                  --index2;
                this.currentList[this.Count++] = new LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair(this.currentList[index1], leftIndex, rightIndics[index2]);
              }
              else
              {
                while (index2 > 0 && rightIndics[index2 - 1] > this.currentList[index1].Right)
                  --index2;
                this.currentList[index1 + 1] = new LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair(this.currentList[index1], leftIndex, rightIndics[index2]);
              }
              --index2;
            }
            else
            {
              --index1;
              if (index1 == -1)
              {
                this.currentList[0] = new LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair((LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair) null, leftIndex, rightIndics[0]);
                break;
              }
            }
          }
        }
      }

      private IEnumerable<LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair> Pairs
      {
        get
        {
          LinkedList<LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair> linkedList = new LinkedList<LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair>();
          if (this.Count > 0)
          {
            for (LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair linkedPair = this.currentList[this.Count - 1]; linkedPair != null; linkedPair = linkedPair.LastPair)
              linkedList.AddFirst(linkedPair);
          }
          return (IEnumerable<LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair>) linkedList;
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
          foreach (LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair pair in this.Pairs)
          {
            if (pair.Left > left || pair.Right > right)
            {
              if (left > 0)
                yield return new RunPair(lastLeft, left, lastRight, right, true);
              yield return new RunPair(left, pair.Left, right, pair.Right, false);
              lastLeft = pair.Left;
              lastRight = pair.Right;
            }
            left = pair.Left + 1;
            right = pair.Right + 1;
          }
          if (lastLeft < this.leftMax || lastRight < this.rightMax)
          {
            if (left > lastLeft && right > lastRight)
              yield return new RunPair(lastLeft, left, lastRight, right, true);
            if (left < this.leftMax || right < this.rightMax)
              yield return new RunPair(left, this.leftMax, right, this.rightMax, false);
          }
        }
      }

      internal sealed class LinkedPair
      {
        public LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair LastPair { get; private set; }

        public int Left { get; private set; }

        public int Right { get; private set; }

        internal LinkedPair(
          LongestCommonSubsequenceAlgorithm.IncreasingList.LinkedPair lastPair,
          int left,
          int right)
        {
          this.LastPair = lastPair;
          this.Left = left;
          this.Right = right;
        }

        public override string ToString() => "(" + (object) this.Left + "," + (object) this.Right + ")";
      }
    }
  }
}
