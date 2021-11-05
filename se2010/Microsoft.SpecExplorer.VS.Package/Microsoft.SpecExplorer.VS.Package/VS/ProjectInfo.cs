using System.IO;

namespace Microsoft.SpecExplorer.VS
{
	public class ProjectInfo : ICordSyntaxElementInfo
	{
		public string ProjectName { get; private set; }

		public string DisplayText { get; private set; }

		public ProjectInfo(string projectName)
		{
			ProjectName = projectName;
			DisplayText = Path.GetFileNameWithoutExtension(ProjectName);
		}
	}
}
