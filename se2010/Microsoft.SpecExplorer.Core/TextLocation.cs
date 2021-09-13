// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.TextLocation
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;

namespace Microsoft.SpecExplorer
{
  [Serializable]
  public struct TextLocation
  {
    public string FileName { get; private set; }

    public short FirstLine { get; private set; }

    public short LastLine { get; private set; }

    public short FirstColumn { get; private set; }

    public short LastColumn { get; private set; }

    public TextLocation(string fileName, short line)
      : this()
    {
      this.FileName = fileName;
      this.FirstLine = this.LastLine = line;
      this.FirstColumn = this.LastColumn = (short) 0;
    }

    public TextLocation(string fileName, short line, short column)
      : this()
    {
      this.FileName = fileName;
      this.FirstLine = this.LastLine = line;
      this.FirstColumn = this.LastColumn = column;
    }

    public TextLocation(
      string fileName,
      short firstLine,
      short firstColumn,
      short lastLine,
      short lastColumn)
      : this()
    {
      this.FileName = fileName;
      this.FirstLine = firstLine;
      this.FirstColumn = firstColumn;
      this.LastLine = lastLine;
      this.LastColumn = lastColumn;
    }

    public override string ToString()
    {
      string str1 = this.FirstColumn != (short) 0 ? this.FirstLine.ToString() + "." + (object) this.FirstColumn : this.FirstLine.ToString();
      string str2 = this.LastColumn != (short) 0 ? this.LastLine.ToString() + "." + (object) this.LastColumn : this.LastLine.ToString();
      return string.Format("{0}({1})", (object) this.FileName, str1 == str2 ? (object) str1 : (object) (str1 + "-" + str2));
    }
  }
}
