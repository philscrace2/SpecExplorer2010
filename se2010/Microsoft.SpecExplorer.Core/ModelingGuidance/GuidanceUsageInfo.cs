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
			{
				return null;
			}
			string[] array = usageInfoString.Split(':');
			if (array.Length == 2)
			{
				GuidanceUsageInfo guidanceUsageInfo = new GuidanceUsageInfo();
				guidanceUsageInfo.GuidanceId = array[0];
				guidanceUsageInfo.CompletedActivityIds = from id in array[1].Split(' ')
					where !string.IsNullOrEmpty(id)
					select id;
				return guidanceUsageInfo;
			}
			throw new GuidanceException("Invalid format for guidance usage string");
		}

		public static IEnumerable<GuidanceUsageInfo> ParseForMultipleGuidance(string combinedUsageInfo)
		{
			if (string.IsNullOrEmpty(combinedUsageInfo))
			{
				return null;
			}
			return from parsedString in combinedUsageInfo.Split(';')
				select Parse(parsedString) into usageInfo
				where usageInfo != null
				select usageInfo;
		}

		public static string PackGuidanceUsageToString(IGuidance guidance)
		{
			if (guidance == null)
			{
				return string.Empty;
			}
			IEnumerable<string> enumerable = from actRef in guidance.Structure
				where actRef.IsCompleted
				select actRef.Activity.Id;
			return string.Format("{0}: {1}", guidance.Id, (enumerable == null || enumerable.Count() == 0) ? string.Empty : enumerable.Aggregate((string combined, string next) => next + " " + combined));
		}

		public static string PackMultipleGuidanceUsageToString(IEnumerable<IGuidance> guidanceList)
		{
			if (guidanceList == null || guidanceList.Count() == 0)
			{
				return string.Empty;
			}
			return guidanceList.Select((IGuidance guidance) => PackGuidanceUsageToString(guidance)).Aggregate((string combined, string next) => next + ";" + combined);
		}
	}
}
