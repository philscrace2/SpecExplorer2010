// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.LanguageToolsOptionCreator
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  internal class LanguageToolsOptionCreator
  {
    private LanguageToolsOptionCreator()
    {
    }

    private static string FormatRegKey(string languageName, string categoryName)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}\\{2}\\{3}", (object) RegistryPaths.languageServices, (object) languageName, (object) RegistryPaths.editorToolsOptions, (object) categoryName);
    }

    internal static void CreateRegistryEntries(
      RegistrationAttribute.RegistrationContext context,
      string languageName,
      string categoryName,
      string categoryResourceId,
      Guid pageGuid)
    {
      using (RegistrationAttribute.Key key = context.CreateKey(LanguageToolsOptionCreator.FormatRegKey(languageName, categoryName)))
      {
        key.SetValue(string.Empty, (object) categoryResourceId);
        key.SetValue(RegistryPaths.package, (object) context.ComponentType.GUID.ToString("B"));
        if (!(pageGuid != Guid.Empty))
          return;
        key.SetValue(RegistryPaths.page, (object) pageGuid.ToString("B"));
      }
    }

    internal static void RemoveRegistryEntries(
      RegistrationAttribute.RegistrationContext context,
      string languageName,
      string categoryName)
    {
      context.RemoveKey(LanguageToolsOptionCreator.FormatRegKey(languageName, categoryName));
    }
  }
}
