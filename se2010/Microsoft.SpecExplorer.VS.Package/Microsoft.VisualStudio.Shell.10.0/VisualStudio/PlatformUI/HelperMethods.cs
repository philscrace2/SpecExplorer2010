// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.HelperMethods
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System.Text;

namespace Microsoft.VisualStudio.PlatformUI
{
  public static class HelperMethods
  {
    public static string StripAccelerators(string input)
    {
      return HelperMethods.StripAccelerators(input, '&');
    }

    public static string StripAccelerators(string input, char accessSpecifier)
    {
      if (string.IsNullOrEmpty(input))
        return input;
      StringBuilder stringBuilder = new StringBuilder(input.Length);
      for (int index1 = 0; index1 < input.Length; ++index1)
      {
        if ((int) input[index1] == (int) accessSpecifier)
          ++index1;
        else if (input[index1] == '(')
        {
          bool flag = false;
          int num = 0;
          for (int index2 = index1 + 1; index2 < input.Length; ++index2)
          {
            if (input[index2] == ')')
            {
              num = index2 + 1;
              break;
            }
            if ((int) input[index2] == (int) accessSpecifier)
            {
              if (index2 == input.Length - 1 || (int) input[index2 + 1] == (int) accessSpecifier)
                ++index2;
              else
                flag = true;
            }
          }
          if (flag)
            index1 = num;
        }
        if (index1 < input.Length)
          stringBuilder.Append(input[index1]);
      }
      return stringBuilder.ToString().TrimEnd();
    }

    public static string StripAccelerators(string input, object accessKeySpecifier)
    {
      return HelperMethods.StripAccelerators(input, HelperMethods.AccessKeySpecifierFromObject(accessKeySpecifier));
    }

    public static char AccessKeySpecifierFromObject(object accessKeySpecifier)
    {
      char ch = '&';
      if (accessKeySpecifier is char)
      {
        ch = (char) accessKeySpecifier;
      }
      else
      {
        string str = accessKeySpecifier as string;
        if (str != null && str.Length == 1)
          ch = str[0];
      }
      return ch;
    }
  }
}
