// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.CommandLine.ExtensionMethods
// Assembly: SpecExplorer, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7507FD4E-0ABD-4C37-958B-5FF2521D030B
// Assembly location: C:\Users\pls2\OneDrive\source code\SourceCode\spec_explorer\original_files\Spec Explorer 2010\SpecExplorer.exe

namespace Microsoft.SpecExplorer.CommandLine
{
  public static class ExtensionMethods
  {
    public static string Description(this TaskType task)
    {
      switch (task)
      {
        case TaskType.GenerateTests:
          return "Generating test suite";
        case TaskType.PersistExploration:
          return "Persisting exploration";
        case TaskType.PerformUserTasks:
          return "Performing user tasks";
        case TaskType.DoOnTheFlyTests:
          return "Performing on-the-fly tests";
        default:
          return string.Empty;
      }
    }

    public static string ToSwitchNameDescription(this string switchName)
    {
      if (string.IsNullOrEmpty(switchName))
        return string.Empty;
      return switchName.EndsWith(":") ? switchName.Substring(0, switchName.Length - 1) : switchName;
    }
  }
}
