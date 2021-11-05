using System.IO;

namespace Microsoft.SpecExplorer.VS
{
	public class ScriptInfo : ICordSyntaxElementInfo
	{
		public string ScriptName { get; private set; }

		public string ContainerProject { get; private set; }

		public string DisplayText { get; private set; }

		public ScriptInfo()
		{
			DisplayText = "<New Script>";
		}

		public ScriptInfo(string containerProject, string scriptName)
		{
			ScriptName = scriptName;
			ContainerProject = containerProject;
			DisplayText = string.Format("{0} :: {1}", Path.GetFileNameWithoutExtension(ContainerProject), Path.GetFileName(ScriptName));
		}
	}
}
