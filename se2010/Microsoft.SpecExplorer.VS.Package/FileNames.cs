// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.FileNames
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System;

namespace Microsoft.SpecExplorer
{
  internal class FileNames
  {
    internal const string ExplorationResultsFolder = "ExplorationResults";
    internal const string OnTheFlyTestResultsFolder = "TestResults";
    internal const string ScriptExtension = ".cord";
    internal const string CSharpExtension = ".cs";
    internal const string CSharpProjectExtension = ".csproj";
    internal const string TasksDefinitionExtension = ".setasks";
    internal const string ExplorationResultExtension = ".seexpl";
    internal const string ViewDefintionExtension = ".sevu";
    internal const string SummaryFileExtension = ".sesum";

    internal static bool HasScriptExtension(string fileName)
    {
      if (!string.IsNullOrEmpty(fileName))
        return fileName.EndsWith(".cord", StringComparison.OrdinalIgnoreCase);
      return false;
    }

    internal static bool HasCSharpExtension(string fileName)
    {
      if (!string.IsNullOrEmpty(fileName))
        return fileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
      return false;
    }
  }
}
