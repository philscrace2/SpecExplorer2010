using System.IO;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer.VS
{
	public class ConfigInfo : ICordSyntaxElementInfo
	{
		public string ContainerProject { get; private set; }

		public string ContainerScript { get; private set; }

		public string ConfigName { get; private set; }

		public string DisplayText { get; private set; }

		public ConfigInfo()
		{
			DisplayText = "<New Configuration>";
		}

		public ConfigInfo(string containerProject, string containerScript, string configName)
		{
			ContainerProject = containerProject;
			ContainerScript = containerScript;
			ConfigName = configName;
			DisplayText = string.Format("{0} [{1} :: {2}]", ConfigName, Path.GetFileNameWithoutExtension(ContainerProject), Path.GetFileName(ContainerScript));
		}

		public override bool Equals(object obj)
		{
			ConfigInfo configInfo = obj as ConfigInfo;
			if (configInfo == null)
			{
				return false;
			}
			if (ConfigName == configInfo.ConfigName && ContainerScript == configInfo.ContainerScript)
			{
				return ContainerProject == configInfo.ContainerProject;
			}
			return false;
		}

		public override int GetHashCode()
		{
			Hash32Builder hash32Builder = default(Hash32Builder);
			if (ConfigName != null)
			{
				hash32Builder.Add(ConfigName.GetHashCode());
			}
			if (ContainerScript != null)
			{
				hash32Builder.Add(ContainerScript.GetHashCode());
			}
			if (ContainerProject != null)
			{
				hash32Builder.Add(ContainerProject.GetHashCode());
			}
			return hash32Builder.Result;
		}
	}
}
