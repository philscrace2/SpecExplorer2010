// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.DiffAlgorithm.DiffBlockPair
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

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

    internal DiffBlockPair(
      string left,
      string right,
      DiffType type,
      IEnumerable<DiffTokenPair> elements)
    {
      this.Left = left;
      this.Right = right;
      this.Type = type;
      this.TokenPairs = elements;
    }

    internal DiffBlockPair(string left, string right, DiffType type)
      : this(left, right, type, Enumerable.Empty<DiffTokenPair>())
    {
    }
  }
}
