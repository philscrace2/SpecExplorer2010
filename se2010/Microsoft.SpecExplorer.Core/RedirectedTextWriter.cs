// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.RedirectedTextWriter
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System.IO;
using System.Text;

namespace Microsoft.SpecExplorer
{
  internal class RedirectedTextWriter : TextWriter
  {
    private StringBuilder lineBuffer = new StringBuilder();
    private EventAdapter eventAdapter;

    internal RedirectedTextWriter(EventAdapter eventAdapter) => this.eventAdapter = eventAdapter;

    public override Encoding Encoding => Encoding.Default;

    public override void Write(string s) => this.Add(s);

    public override void Write(char c) => this.Add(new string(c, 1));

    public override void WriteLine() => this.Add(this.NewLine);

    public override void WriteLine(string s)
    {
      this.Add(s);
      this.Add(this.NewLine);
    }

    private void Add(string s)
    {
      if (s == null)
        return;
      int num;
      for (int startIndex = 0; startIndex < s.Length; startIndex = num + this.NewLine.Length)
      {
        num = s.IndexOf(this.NewLine, startIndex);
        if (num >= 0)
        {
          this.lineBuffer.Append(s.Substring(startIndex, num - startIndex));
          this.eventAdapter.Log(this.lineBuffer.ToString());
          this.lineBuffer.Length = 0;
        }
        else
        {
          this.lineBuffer.Append(s.Substring(startIndex));
          break;
        }
      }
    }
  }
}
