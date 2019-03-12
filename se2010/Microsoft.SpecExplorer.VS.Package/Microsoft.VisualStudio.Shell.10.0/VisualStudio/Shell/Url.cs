// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Url
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Shell
{
  public class Url
  {
    private const char c_DummyChar = '\xFFFF';
    private Uri uri;
    private bool isFile;
    private string cachedAbsUrl;

    public Url(string path)
    {
      this.Init(path);
    }

    private void Init(string path)
    {
      if (path == null)
        return;
      if (!Uri.TryCreate(path, UriKind.Absolute, out this.uri))
        Uri.TryCreate(path, UriKind.Relative, out this.uri);
      this.CheckIsFile();
    }

    private void CheckIsFile()
    {
      this.isFile = false;
      if (!(this.uri != (Uri) null))
        return;
      if (this.uri.IsAbsoluteUri)
      {
        this.isFile = this.uri.IsFile;
      }
      else
      {
        int num1 = this.uri.OriginalString.Count<char>((Func<char, bool>) (ch => ch == '/'));
        int num2 = this.uri.OriginalString.Count<char>((Func<char, bool>) (ch => ch == '\\'));
        if (num1 != 0 && num1 >= num2)
          return;
        this.isFile = true;
      }
    }

    public Url(Url baseUrl, string relpath)
    {
      if (baseUrl.uri == (Uri) null || !baseUrl.uri.IsAbsoluteUri)
        this.Init(relpath);
      else if (string.IsNullOrEmpty(relpath))
        this.uri = baseUrl.uri;
      else
        Uri.TryCreate(baseUrl.uri, relpath, out this.uri);
      this.CheckIsFile();
    }

    public string AbsoluteUrl
    {
      get
      {
        if (this.uri == (Uri) null)
          return (string) null;
        if (this.cachedAbsUrl != null)
          return this.cachedAbsUrl;
        this.cachedAbsUrl = !this.uri.IsAbsoluteUri ? this.uri.OriginalString : (!this.isFile ? this.uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped) : this.uri.LocalPath);
        return this.cachedAbsUrl;
      }
    }

    public string Directory
    {
      get
      {
        string absoluteUrl = this.AbsoluteUrl;
        if (absoluteUrl == null)
          return (string) null;
        if (this.isFile)
          return Path.GetDirectoryName(absoluteUrl);
        int num = absoluteUrl.LastIndexOf('/');
        int length = num > 0 ? num : absoluteUrl.Length;
        return absoluteUrl.Substring(0, length);
      }
    }

    public bool IsFile
    {
      get
      {
        return this.isFile;
      }
    }

    public Url Move(Url oldBase, Url newBase)
    {
      if (this.uri == (Uri) null || oldBase.uri == (Uri) null)
        return (Url) null;
      string relpath = oldBase.uri.MakeRelativeUri(this.uri).ToString();
      return new Url(newBase, relpath);
    }

    public string MakeRelative(Url url)
    {
      if (this.uri == (Uri) null || url.uri == (Uri) null)
        return (string) null;
      if (this.uri.Scheme != url.uri.Scheme || this.uri.Host != url.uri.Host)
        return url.AbsoluteUrl;
      return Url.Unescape(this.uri.MakeRelativeUri(url.uri).ToString(), this.isFile);
    }

    private static char EscapedAscii(char digit, char next)
    {
      if ((digit < '0' || digit > '9') && (digit < 'A' || digit > 'F') && (digit < 'a' || digit > 'f'))
        return char.MaxValue;
      int num1 = digit > '9' ? (digit > 'F' ? (int) digit - 97 + 10 : (int) digit - 65 + 10) : (int) digit - 48;
      if ((next < '0' || next > '9') && (next < 'A' || next > 'F') && (next < 'a' || next > 'f'))
        return char.MaxValue;
      int num2 = num1 << 4;
      return next > '9' ? (digit > 'F' ? (char) (num2 + ((int) next - 97 + 10)) : (char) (num2 + ((int) next - 65 + 10))) : (char) (num2 + ((int) next - 48));
    }

    public static string Unescape(string escaped, bool isFile)
    {
      if (string.IsNullOrEmpty(escaped))
        return string.Empty;
      byte[] bytes = (byte[]) null;
      char[] chars = new char[escaped.Length];
      int num1 = 0;
      int index1 = 0;
      for (int length = escaped.Length; index1 < length; ++index1)
      {
        char directorySeparatorChar = escaped[index1];
        switch (directorySeparatorChar)
        {
          case '%':
            int num2 = 0;
            if (bytes == null)
              bytes = new byte[length - index1];
            char ch;
            while ((ch = escaped[index1]) == '%' && length - index1 >= 3)
            {
              ch = Url.EscapedAscii(escaped[index1 + 1], escaped[index1 + 2]);
              if (ch == char.MaxValue)
              {
                ch = '%';
                break;
              }
              if (ch < '\x0080')
              {
                index1 += 2;
                break;
              }
              bytes[num2++] = (byte) ch;
              index1 += 3;
              if (index1 >= length)
                break;
            }
            if (num2 != 0)
            {
              int charCount = Encoding.UTF8.GetCharCount(bytes, 0, num2);
              if (charCount != 0)
              {
                Encoding.UTF8.GetChars(bytes, 0, num2, chars, num1);
                num1 += charCount;
              }
              else
              {
                for (int index2 = 0; index2 < num2; ++index2)
                  chars[num1++] = (char) bytes[index2];
              }
            }
            if (index1 < length)
            {
              chars[num1++] = ch;
              break;
            }
            break;
          case '/':
            if (isFile)
            {
              directorySeparatorChar = Path.DirectorySeparatorChar;
              goto default;
            }
            else
              goto default;
          default:
            chars[num1++] = directorySeparatorChar;
            break;
        }
      }
      return new string(chars, 0, num1);
    }

    public Uri Uri
    {
      get
      {
        return this.uri;
      }
    }

    public string GetPartial(int i)
    {
      return this.GetPartial(0, i);
    }

    public string GetPartial(int i, int j)
    {
      string str = this.JoinSegments(i, j);
      if (i == 0)
      {
        if (!this.isFile)
          str = this.uri.Scheme + "://" + this.uri.Host + (object) '/' + str;
        else if (this.uri.IsAbsoluteUri && this.uri.IsUnc && this.AbsoluteUrl.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase))
          str = "\\\\" + str;
      }
      return str;
    }

    public string GetRemainder(int i)
    {
      return this.JoinSegments(i, -1);
    }

    public string[] Segments
    {
      get
      {
        if (this.uri == (Uri) null)
          return (string[]) null;
        string str1 = this.AbsoluteUrl;
        if (this.isFile)
        {
          if (str1.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
            str1 = str1.Substring(0, str1.Length - 1);
          if (this.uri.IsAbsoluteUri && this.uri.IsUnc && str1.StartsWith("\\\\", StringComparison.OrdinalIgnoreCase))
            str1 = str1.Substring(2);
          return str1.Split(Path.DirectorySeparatorChar);
        }
        string str2 = str1.Substring(this.uri.Scheme.Length + 3 + this.uri.Host.Length + 1);
        if (str2.EndsWith("/", StringComparison.OrdinalIgnoreCase))
          str2 = str2.Substring(0, str2.Length - 1);
        return str2.Split('/');
      }
    }

    public string JoinSegments(int i, int j)
    {
      if (i < 0)
        throw new ArgumentOutOfRangeException(nameof (i));
      StringBuilder stringBuilder = new StringBuilder();
      string[] segments = this.Segments;
      if (segments == null)
        return (string) null;
      if (j < 0)
        j = segments.Length;
      for (int length = segments.Length; i < j && i < length; ++i)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(this.isFile ? Path.DirectorySeparatorChar : '/');
        string str = segments[i];
        stringBuilder.Append(str);
      }
      return Url.Unescape(stringBuilder.ToString(), this.isFile);
    }
  }
}
