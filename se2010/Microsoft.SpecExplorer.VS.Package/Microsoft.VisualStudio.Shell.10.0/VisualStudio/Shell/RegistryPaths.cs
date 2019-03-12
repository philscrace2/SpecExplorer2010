// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.RegistryPaths
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

namespace Microsoft.VisualStudio.Shell
{
  internal class RegistryPaths
  {
    internal static string package = "Package";
    internal static string displayName = "DisplayName";
    internal static string languageStringId = "LangStringID";
    internal static string languageResourceId = "LangResID";
    internal static string showRoots = "ShowRoots";
    internal static string indexPath = "IndexPath";
    internal static string paths = "Paths";
    internal static string languages = "Languages";
    internal static string languageServices = RegistryPaths.languages + "\\Language Services";
    internal static string codeExpansion = RegistryPaths.languages + "\\CodeExpansions";
    internal static string forceCreateDirs = "ForceCreateDirs";
    internal static string debuggerLanguages = "Debugger Languages";
    internal static string editorToolsOptions = "EditorToolsOptions";
    internal static string page = "Page";

    private RegistryPaths()
    {
    }
  }
}
