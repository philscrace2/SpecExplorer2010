// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.AnnotationFormatter
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Text;

namespace Microsoft.SpecExplorer.Viewer
{
  internal class AnnotationFormatter
  {
    internal static readonly string TabPadding = "  ";
    private static readonly string LineBreakIndent = string.Empty;
    internal static readonly string SoftLineBreakIndent = AnnotationFormatter.LineBreakIndent + AnnotationFormatter.TabPadding;
    internal static readonly int DefaultLineWidth = 15;

    public static string Format(string text, int maxWidth)
    {
      maxWidth = maxWidth > AnnotationFormatter.DefaultLineWidth ? maxWidth : AnnotationFormatter.DefaultLineWidth;
      text = text.Replace("\r", string.Empty);
      StringBuilder stringBuilder = new StringBuilder();
      string[] strArray = text.Split(new char[1]{ '\n' }, StringSplitOptions.None);
      for (int index = 0; index < strArray.Length; ++index)
      {
        string str1 = AnnotationFormatter.LineBreakIndent;
        if (index > 0)
          stringBuilder.AppendLine();
        string str2 = strArray[index];
        int num1;
        if (!str2.Contains("\t") && str2.Length <= maxWidth)
          stringBuilder.Append(str2);
        else if (str2.Contains("\t"))
        {
          do
          {
            for (int length = str2.IndexOf('\t'); length >= 0 && length < maxWidth; length = str2.IndexOf('\t'))
            {
              string str3 = length < maxWidth - 1 ? AnnotationFormatter.TabPadding : " ";
              str2 = str2.Substring(0, length) + str3 + str2.Substring(length + 1);
            }
            if (str2.Length > maxWidth)
            {
              int num2 = str2.LastIndexOf(' ', maxWidth - 1, maxWidth - str1.Length);
              str1 = AnnotationFormatter.SoftLineBreakIndent;
              int num3 = num2 > 0 ? num2 + 1 : maxWidth;
              stringBuilder.AppendLine(str2.Substring(0, num3));
              str2 = AnnotationFormatter.SoftLineBreakIndent + str2.Substring(num3);
            }
            else
              goto label_11;
          }
          while (str2.Length > 0);
          continue;
label_11:
          stringBuilder.Append(str2);
        }
        else
        {
          for (; str2.Length > 0; str2 = AnnotationFormatter.SoftLineBreakIndent + str2.Substring(num1))
          {
            if (str2.Length > maxWidth)
            {
              int num2 = str2.LastIndexOf(' ', maxWidth - 1, maxWidth - str1.Length);
              str1 = AnnotationFormatter.SoftLineBreakIndent;
              num1 = num2 > 0 ? num2 + 1 : maxWidth;
              stringBuilder.AppendLine(str2.Substring(0, num1));
            }
            else
            {
              stringBuilder.Append(str2);
              break;
            }
          }
        }
      }
      return stringBuilder.ToString();
    }
  }
}
