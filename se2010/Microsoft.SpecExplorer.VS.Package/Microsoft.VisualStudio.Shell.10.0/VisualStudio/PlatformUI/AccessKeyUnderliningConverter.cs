// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.AccessKeyUnderliningConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Globalization;
using System.Windows.Documents;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class AccessKeyUnderliningConverter : ValueConverter<string, object>
  {
    protected override object Convert(string fullText, object parameter, CultureInfo culture)
    {
      char ch1 = HelperMethods.AccessKeySpecifierFromObject(parameter);
      Span span = new Span();
      while (!string.IsNullOrEmpty(fullText))
      {
        int length = fullText.IndexOf(ch1);
        if (length == -1)
        {
          span.Inlines.Add((Inline) new Run(fullText));
          fullText = string.Empty;
        }
        else
        {
          if (length > 0)
            span.Inlines.Add((Inline) new Run(fullText.Substring(0, length)));
          if (length < fullText.Length - 1)
          {
            char ch2 = fullText[length + 1];
            Run run = new Run(ch2.ToString());
            if ((int) ch2 == (int) ch1)
              span.Inlines.Add((Inline) run);
            else
              span.Inlines.Add((Inline) new Underline((Inline) run));
            fullText = fullText.Substring(length + 2);
          }
          else
          {
            span.Inlines.Add((Inline) new Run(ch1.ToString()));
            fullText = string.Empty;
          }
        }
      }
      return (object) span;
    }
  }
}
