using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	public static class ExplorationUtility
	{
		public static bool NeedsReExploration(string currentStamp, string explorationResultFilePath)
		{
			if (File.Exists(explorationResultFilePath))
			{
				try
				{
					ExplorationResultLoader explorationResultLoader = new ExplorationResultLoader(explorationResultFilePath);
					ExplorationResultExtensions explorationResultExtensions = explorationResultLoader.LoadExtensions();
					return explorationResultExtensions.IgnoreSignature || string.Compare(currentStamp, explorationResultExtensions.Signature, StringComparison.InvariantCulture) != 0;
				}
				catch (ExplorationResultLoadingException)
				{
					return true;
				}
			}
			return true;
		}

		public static string GetSourceFilesStamp(ICollection<string> assemblies, ICollection<string> scripts)
		{
			List<string> list = new List<string>(assemblies);
			List<string> list2 = new List<string>(scripts);
			list.Sort();
			list2.Sort();
			MD5 mD = MD5.Create();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string item in list)
			{
				byte[] data = mD.ComputeHash(File.OpenRead(item));
				ProcessBytes(stringBuilder, data);
				stringBuilder.Append(item);
			}
			foreach (string item2 in list2)
			{
				byte[] data2 = mD.ComputeHash(File.OpenRead(item2));
				ProcessBytes(stringBuilder, data2);
				stringBuilder.Append(item2);
			}
			byte[] data3 = mD.ComputeHash(Encoding.Unicode.GetBytes(stringBuilder.ToString() + "3.5.3146.0"));
			mD.Dispose();
			stringBuilder.Clear();
			ProcessBytes(stringBuilder, data3);
			return stringBuilder.ToString();
		}

		private static void ProcessBytes(StringBuilder builder, byte[] data)
		{
			foreach (byte b in data)
			{
				builder.Append(b.ToString("x2"));
			}
		}
	}
}
