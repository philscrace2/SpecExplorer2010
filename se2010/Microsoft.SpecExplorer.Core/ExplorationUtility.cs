// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ExplorationUtility
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.SpecExplorer
{
  public static class ExplorationUtility
  {
    public static bool NeedsReExploration(string currentStamp, string explorationResultFilePath)
    {
      if (!File.Exists(explorationResultFilePath))
        return true;
      try
      {
        ExplorationResultExtensions resultExtensions = new ExplorationResultLoader(explorationResultFilePath).LoadExtensions();
        return resultExtensions.IgnoreSignature != null || string.Compare(currentStamp, (string) resultExtensions.Signature, StringComparison.InvariantCulture) != 0;
      }
      catch (ExplorationResultLoadingException ex)
      {
        return true;
      }
    }

    public static string GetSourceFilesStamp(
      ICollection<string> assemblies,
      ICollection<string> scripts)
    {
      List<string> stringList1 = new List<string>((IEnumerable<string>) assemblies);
      List<string> stringList2 = new List<string>((IEnumerable<string>) scripts);
      stringList1.Sort();
      stringList2.Sort();
      MD5 md5 = MD5.Create();
      StringBuilder builder = new StringBuilder();
      foreach (string path in stringList1)
      {
        byte[] hash = md5.ComputeHash((Stream) File.OpenRead(path));
        ExplorationUtility.ProcessBytes(builder, hash);
        builder.Append(path);
      }
      foreach (string path in stringList2)
      {
        byte[] hash = md5.ComputeHash((Stream) File.OpenRead(path));
        ExplorationUtility.ProcessBytes(builder, hash);
        builder.Append(path);
      }
      byte[] hash1 = md5.ComputeHash(Encoding.Unicode.GetBytes(builder.ToString() + "3.5.3146.0"));
      md5.Dispose();
      builder.Clear();
      ExplorationUtility.ProcessBytes(builder, hash1);
      return builder.ToString();
    }

    private static void ProcessBytes(StringBuilder builder, byte[] data)
    {
      foreach (byte num in data)
        builder.Append(num.ToString("x2"));
    }
  }
}
