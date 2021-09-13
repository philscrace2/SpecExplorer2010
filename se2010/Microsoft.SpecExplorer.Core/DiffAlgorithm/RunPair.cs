// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.DiffAlgorithm.RunPair
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

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
      this.LeftBegin = leftBegin;
      this.RightBegin = rightBegin;
      this.LeftEnd = leftEnd;
      this.RightEnd = rightEnd;
      this.IsIdentical = isIdentical;
    }

    public override string ToString() => "[" + (object) this.LeftBegin + "," + (object) this.LeftEnd + ") <==> [" + (object) this.RightBegin + "," + (object) this.RightEnd + ")";
  }
}
