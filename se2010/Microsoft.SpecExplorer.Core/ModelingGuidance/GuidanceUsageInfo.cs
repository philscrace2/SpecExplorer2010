// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ModelingGuidance.GuidanceUsageInfo
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
  public class GuidanceUsageInfo
  {
    public string GuidanceId { get; private set; }

    public IEnumerable<string> CompletedActivityIds { get; private set; }

    public static GuidanceUsageInfo Parse(string usageInfoString)
    {
      if (string.IsNullOrEmpty(usageInfoString))
        return (GuidanceUsageInfo) null;
      string[] strArray = usageInfoString.Split(':');
      return strArray.Length == 2 ? new GuidanceUsageInfo()
      {
        GuidanceId = strArray[0],
        CompletedActivityIds = ((IEnumerable<string>) strArray[1].Split(' ')).Where<string>((Func<string, bool>) (id => !string.IsNullOrEmpty(id)))
      } : throw new GuidanceException("Invalid format for guidance usage string");
    }

    public static IEnumerable<GuidanceUsageInfo> ParseForMultipleGuidance(
      string combinedUsageInfo)
    {
      if (string.IsNullOrEmpty(combinedUsageInfo))
        return (IEnumerable<GuidanceUsageInfo>) null;
      return ((IEnumerable<string>) combinedUsageInfo.Split(';')).Select<string, GuidanceUsageInfo>((Func<string, GuidanceUsageInfo>) (parsedString => GuidanceUsageInfo.Parse(parsedString))).Where<GuidanceUsageInfo>((Func<GuidanceUsageInfo, bool>) (usageInfo => usageInfo != null));
    }

    public static string PackGuidanceUsageToString(IGuidance guidance)
    {
      if (guidance == null)
        return string.Empty;
      IEnumerable<string> source = ((IEnumerable<IActivityReference>) guidance.Structure).Where<IActivityReference>((Func<IActivityReference, bool>) (actRef => actRef.IsCompleted)).Select<IActivityReference, string>((Func<IActivityReference, string>) (actRef => actRef.Activity.Id));
      return string.Format("{0}: {1}", (object) guidance.Id, source == null || source.Count<string>() == 0 ? (object) string.Empty : (object) source.Aggregate<string>((Func<string, string, string>) ((combined, next) => next + " " + combined)));
    }

    public static string PackMultipleGuidanceUsageToString(IEnumerable<IGuidance> guidanceList) => guidanceList == null || guidanceList.Count<IGuidance>() == 0 ? string.Empty : guidanceList.Select<IGuidance, string>((Func<IGuidance, string>) (guidance => GuidanceUsageInfo.PackGuidanceUsageToString(guidance))).Aggregate<string>((Func<string, string, string>) ((combined, next) => next + ";" + combined));
  }
}
