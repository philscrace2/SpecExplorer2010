// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.DiffAlgorithm.StringDiffAlgorithm
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

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
        throw new ArgumentNullException(nameof (left));
      if (right == null)
        throw new ArgumentNullException(nameof (right));
      this.intraLines = calcIntraLines;
      this.leftLines = Array.ConvertAll<string, string>(left.Split('\n'), (Converter<string, string>) (text => text.Trim('\r')));
      this.rightLines = Array.ConvertAll<string, string>(right.Split('\n'), (Converter<string, string>) (text => text.Trim('\r')));
    }

    public IEnumerable<DiffBlockPair> Execute()
    {
      bool partialCompare = this.intraLines && this.leftLines.Length + this.rightLines.Length < 1000;
      IEnumerable<RunPair> runPairs = !partialCompare ? LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence<string>((IEnumerable<string>) this.leftLines, (IEnumerable<string>) this.rightLines).RunPairs : LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence<string>((IEnumerable<string>) this.leftLines, (IEnumerable<string>) this.rightLines, new Func<string, string, bool>(StringDiffAlgorithm.StringRawSimilarity)).RunPairs;
      foreach (RunPair runPair in runPairs)
      {
        if (runPair.IsIdentical)
        {
          int i = runPair.LeftBegin;
          int j = runPair.RightBegin;
          while (i < runPair.LeftEnd)
          {
            if (!partialCompare || this.leftLines[i] == this.rightLines[j])
            {
              yield return new DiffBlockPair(this.leftLines[i], this.rightLines[j], DiffType.Identical);
            }
            else
            {
              foreach (DiffBlockPair matchIntraLine in StringDiffAlgorithm.MatchIntraLines(this.leftLines[i], this.rightLines[j]))
                yield return matchIntraLine;
            }
            ++i;
            ++j;
          }
        }
        else if (runPair.LeftBegin == runPair.LeftEnd)
        {
          for (int i = runPair.RightBegin; i < runPair.RightEnd; ++i)
            yield return new DiffBlockPair(string.Empty, this.rightLines[i], DiffType.Inserted);
        }
        else if (runPair.RightBegin == runPair.RightEnd)
        {
          for (int i = runPair.LeftBegin; i < runPair.LeftEnd; ++i)
            yield return new DiffBlockPair(this.leftLines[i], string.Empty, DiffType.Deleted);
        }
        else if (this.intraLines)
        {
          foreach (DiffBlockPair diffBlockPair in this.LinesCompare(runPair.LeftBegin, runPair.LeftEnd, runPair.RightBegin, runPair.RightEnd))
            yield return diffBlockPair;
        }
        else
        {
          for (int i = runPair.LeftBegin; i < runPair.LeftEnd; ++i)
            yield return new DiffBlockPair(this.leftLines[i], string.Empty, DiffType.Deleted);
          for (int i = runPair.RightBegin; i < runPair.RightEnd; ++i)
            yield return new DiffBlockPair(string.Empty, this.rightLines[i], DiffType.Inserted);
        }
      }
    }

    private static IEnumerable<string> Tokenize(string text)
    {
      if (!string.IsNullOrEmpty(text))
      {
        bool lastCharIsAlphaNum = false;
        bool beginningOfWord = true;
        StringBuilder tokenConnector = new StringBuilder();
        foreach (char c in text)
        {
          if (c != '\r')
          {
            if (!beginningOfWord && (!char.IsLetterOrDigit(c) || !lastCharIsAlphaNum))
            {
              yield return tokenConnector.ToString();
              beginningOfWord = true;
              tokenConnector.Length = 0;
            }
            beginningOfWord = false;
            lastCharIsAlphaNum = char.IsLetterOrDigit(c);
            tokenConnector.Append(c);
          }
        }
        yield return tokenConnector.ToString();
      }
    }

    private static string Combine(List<string> list, int from, int to)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = from; index < to; ++index)
        stringBuilder.Append(list[index]);
      return stringBuilder.ToString();
    }

    private static DiffBlockPair ConvertTokensToBlock(List<DiffTokenPair> Elements)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      DiffType type = Elements[0].Type;
      foreach (DiffTokenPair element in Elements)
      {
        stringBuilder1.Append(element.Left);
        stringBuilder2.Append(element.Right);
        if (type != element.Type)
          type = DiffType.Changed;
      }
      return new DiffBlockPair(stringBuilder1.ToString(), stringBuilder2.ToString(), type, (IEnumerable<DiffTokenPair>) Elements);
    }

    private static IEnumerable<DiffBlockPair> MatchIntraLines(
      string leftLine,
      string rightLine)
    {
      if (leftLine == null)
        throw new ArgumentNullException(nameof (leftLine));
      if (rightLine == null)
        throw new ArgumentNullException(nameof (rightLine));
      List<string> left = new List<string>(StringDiffAlgorithm.Tokenize(leftLine));
      List<string> right = new List<string>(StringDiffAlgorithm.Tokenize(rightLine));
      IEnumerable<RunPair> ilist = LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence<string>((IEnumerable<string>) left, (IEnumerable<string>) right).RunPairs;
      List<DiffTokenPair> subTokens = new List<DiffTokenPair>();
      foreach (RunPair runPair in ilist)
      {
        if (runPair.IsIdentical)
        {
          int i = runPair.LeftBegin;
          int j = runPair.RightBegin;
          while (i < runPair.LeftEnd)
          {
            if (left[i] == "\n")
            {
              if (subTokens.Count > 0)
              {
                yield return StringDiffAlgorithm.ConvertTokensToBlock(subTokens);
                subTokens = new List<DiffTokenPair>();
              }
            }
            else
              subTokens.Add(new DiffTokenPair(left[i], right[j], DiffType.Identical));
            ++i;
            ++j;
          }
        }
        else if (runPair.LeftBegin == runPair.LeftEnd)
        {
          for (int i = runPair.RightBegin; i < runPair.RightEnd; ++i)
          {
            if (right[i] == "\n")
            {
              if (subTokens.Count > 0)
              {
                yield return StringDiffAlgorithm.ConvertTokensToBlock(subTokens);
                subTokens = new List<DiffTokenPair>();
              }
            }
            else
              subTokens.Add(new DiffTokenPair("", right[i], DiffType.Inserted));
          }
        }
        else if (runPair.RightBegin == runPair.RightEnd)
        {
          for (int i = runPair.LeftBegin; i < runPair.LeftEnd; ++i)
          {
            if (left[i] == "\n")
            {
              if (subTokens.Count > 0)
              {
                yield return StringDiffAlgorithm.ConvertTokensToBlock(subTokens);
                subTokens = new List<DiffTokenPair>();
              }
            }
            else
              subTokens.Add(new DiffTokenPair(left[i], "", DiffType.Deleted));
          }
        }
        else
          subTokens.Add(new DiffTokenPair(StringDiffAlgorithm.Combine(left, runPair.LeftBegin, runPair.LeftEnd), StringDiffAlgorithm.Combine(right, runPair.RightBegin, runPair.RightEnd), DiffType.Changed));
      }
      if (subTokens.Count > 0)
        yield return StringDiffAlgorithm.ConvertTokensToBlock(subTokens);
    }

    private IEnumerable<DiffBlockPair> LinesCompare(
      int leftStart,
      int leftStop,
      int rightStart,
      int rightStop)
    {
      StringBuilder leftStringBuilder = new StringBuilder();
      StringBuilder rightStringBuilder = new StringBuilder();
      for (int index = leftStart; index < leftStop; ++index)
      {
        leftStringBuilder.Append(this.leftLines[index]);
        leftStringBuilder.Append("\n");
      }
      for (int index = rightStart; index < rightStop; ++index)
      {
        rightStringBuilder.Append(this.rightLines[index]);
        rightStringBuilder.Append("\n");
      }
      foreach (DiffBlockPair matchIntraLine in StringDiffAlgorithm.MatchIntraLines(leftStringBuilder.ToString(), rightStringBuilder.ToString()))
      {
        if (matchIntraLine.Type == DiffType.Changed)
        {
          int max = LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence<char>((IEnumerable<char>) matchIntraLine.Left, (IEnumerable<char>) matchIntraLine.Right).Count;
          if ((double) max <= 0.3 * (double) (matchIntraLine.Left.Length + matchIntraLine.Right.Length))
          {
            string left1 = matchIntraLine.Left;
            char[] chArray1 = new char[1]{ '\n' };
            foreach (string left2 in left1.Split(chArray1))
              yield return new DiffBlockPair(left2, "", DiffType.Deleted);
            string right1 = matchIntraLine.Right;
            char[] chArray2 = new char[1]{ '\n' };
            foreach (string right2 in right1.Split(chArray2))
              yield return new DiffBlockPair("", right2, DiffType.Inserted);
          }
          else
            yield return matchIntraLine;
        }
        else
          yield return matchIntraLine;
      }
    }

    private static bool StringRawSimilarity(string left, string right)
    {
      if (left.Length == 0)
        return right.Length == 0;
      return right.Length != 0 && (double) Math.Abs(left.Length - right.Length) <= 1.0 / 10.0 * (double) (left.Length + right.Length) && (double) (LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence<char>((IEnumerable<char>) left.Substring(0, left.Length / 2), (IEnumerable<char>) right.Substring(0, right.Length / 2)).Count + LongestCommonSubsequenceAlgorithm.CalculateLongestCommonSubsequence<char>((IEnumerable<char>) left.Substring(left.Length / 2, left.Length - left.Length / 2), (IEnumerable<char>) right.Substring(right.Length / 2, right.Length - right.Length / 2)).Count) > 0.45 * (double) (left.Length + right.Length);
    }
  }
}
