using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.ActionMachines.Cord;
using Microsoft.VisualStudio;

namespace Microsoft.SpecExplorer.VS
{
	internal class ProjectUtils
	{
		internal static IEnumerable<Project> GetAllRealProjects(DTE dte)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			List<Project> list = new List<Project>();
			if (((_DTE)dte).Solution != null && ((_Solution)((_DTE)dte).Solution).Projects != null)
			{
				foreach (Project project in ((_Solution)((_DTE)dte).Solution).Projects)
				{
					Project val = project;
					if (val != null)
					{
						CollectProjects(val, list);
					}
				}
				return list;
			}
			return list;
		}

		internal static IEnumerable<Project> GetProjectsContainingCordScript(DTE dte, ICordDesignTimeScopeManager scopeManager)
		{
			List<Project> list = new List<Project>();
			foreach (Project allRealProject in GetAllRealProjects(dte))
			{
				if (allRealProject != null && allRealProject.UniqueName != null)
				{
					ICordDesignTimeManager cordDesignTimeManager = scopeManager.GetCordDesignTimeManager(allRealProject.UniqueName);
					if (cordDesignTimeManager != null && cordDesignTimeManager.ManagedScripts.Count > 0)
					{
						list.Add(allRealProject);
					}
				}
			}
			return list;
		}

		private static void CollectProjects(Project project, IList<Project> projects)
		{
			if (project.Kind == "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}")
			{
				if (project.ProjectItems == null)
					return;
				foreach (ProjectItem projectItem in project.ProjectItems)
				{
					if (projectItem != null && projectItem.SubProject != null)
						ProjectUtils.CollectProjects(projectItem.SubProject, projects);
				}
			}
			else
			{
				if (string.Compare("{67294A52-A4F0-11D2-AA88-00C04F688DDE}", project.Kind, StringComparison.OrdinalIgnoreCase) == 0)
					return;
				projects.Add(project);
			}
		}

		internal static Project GetProjectOfFile(string filePath, DTE dte)
		{
			foreach (Project allRealProject in GetAllRealProjects(dte))
			{
				if (allRealProject != null && IsFileInProject(filePath, allRealProject))
				{
					return allRealProject;
				}
			}
			return null;
		}

		private static bool IsFileUnderProjectItem(string filePath, ProjectItem projectItem)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			Guid guid = new Guid(projectItem.Kind);
			if (guid == VSConstants.GUID_ItemType_PhysicalFile)
			{
				string text = projectItem.get_FileNames((short)1);
				if (text != null && string.Compare(text, filePath, true) == 0)
				{
					return true;
				}
			}
			else if (guid == VSConstants.GUID_ItemType_PhysicalFolder)
			{
				foreach (ProjectItem projectItem3 in projectItem.ProjectItems)
				{
					ProjectItem projectItem2 = projectItem3;
					if (IsFileUnderProjectItem(filePath, projectItem2))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool IsFileInProject(string filePath, Project project)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			if (project.ProjectItems != null)
			{
				foreach (ProjectItem projectItem in project.ProjectItems)
				{
					ProjectItem val = projectItem;
					if (val != null && IsFileUnderProjectItem(filePath, val))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static IList<string> GetDocumentsInProject(Project project, string fileExtension)
		{
			return GetDocumentsInProjectItems(project.ProjectItems, fileExtension);
		}

		private static IList<string> GetDocumentsInProjectItems(ProjectItems projectItems, string fileExtension)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			List<string> list = new List<string>();
			if (projectItems != null)
			{
				foreach (ProjectItem projectItem in projectItems)
				{
					ProjectItem val = projectItem;
					if (val != null)
					{
						Guid guid = new Guid(val.Kind);
						if (guid == VSConstants.GUID_ItemType_PhysicalFile)
						{
							string name = val.Name;
							string text = val.get_FileNames((short)0);
							if (!string.IsNullOrEmpty(name) && name.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(text))
							{
								list.Add(text);
							}
						}
						else if (guid == VSConstants.GUID_ItemType_PhysicalFolder)
						{
							list.AddRange(GetDocumentsInProjectItems(val.ProjectItems, fileExtension));
						}
					}
				}
				return list;
			}
			return list;
		}
	}
}
