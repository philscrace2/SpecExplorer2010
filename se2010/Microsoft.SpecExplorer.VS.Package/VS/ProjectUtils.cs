// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ProjectUtils
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using Microsoft.ActionMachines.Cord;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;
using System;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer.VS
{
  internal class ProjectUtils
  {
    internal static IEnumerable<Project> GetAllRealProjects(DTE dte)
    {
      List<Project> projectList = new List<Project>();
      if (dte.Solution != null && dte.Solution.Projects != null)
      {
        foreach (Project project in dte.Solution.Projects)
        {
          if (project != null)
            ProjectUtils.CollectProjects(project, (IList<Project>) projectList);
        }
      }
      return (IEnumerable<Project>) projectList;
    }

    internal static IEnumerable<Project> GetProjectsContainingCordScript(
      DTE dte,
      ICordDesignTimeScopeManager scopeManager)
    {
      List<Project> projectList = new List<Project>();
      foreach (Project allRealProject in ProjectUtils.GetAllRealProjects(dte))
      {
        if (allRealProject != null && allRealProject.UniqueName != null)
        {
          ICordDesignTimeManager designTimeManager = scopeManager.GetCordDesignTimeManager(allRealProject.UniqueName);
          if (designTimeManager != null && designTimeManager.ManagedScripts.Count > 0)
            projectList.Add(allRealProject);
        }
      }
      return (IEnumerable<Project>) projectList;
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
      foreach (Project allRealProject in ProjectUtils.GetAllRealProjects(dte))
      {
        if (allRealProject != null && ProjectUtils.IsFileInProject(filePath, allRealProject))
          return allRealProject;
      }
      return (Project) null;
    }

    private static bool IsFileUnderProjectItem(string filePath, ProjectItem projectItem)
    {
      Guid guid = new Guid(projectItem.Kind);
      if (guid == VSConstants.GUID_ItemType_PhysicalFile)
      {
        string strA = projectItem.FileNames((short) 1);
        if (strA != null && string.Compare(strA, filePath, true) == 0)
          return true;
      }
      else if (guid == VSConstants.GUID_ItemType_PhysicalFolder)
      {
        foreach (ProjectItem projectItem1 in projectItem.ProjectItems)
        {
          if (ProjectUtils.IsFileUnderProjectItem(filePath, projectItem1))
            return true;
        }
      }
      return false;
    }

    private static bool IsFileInProject(string filePath, Project project)
    {
      if (project.ProjectItems != null)
      {
        foreach (ProjectItem projectItem in project.ProjectItems)
        {
          if (projectItem != null && ProjectUtils.IsFileUnderProjectItem(filePath, projectItem))
            return true;
        }
      }
      return false;
    }

    internal static IList<string> GetDocumentsInProject(Project project, string fileExtension)
    {
      return ProjectUtils.GetDocumentsInProjectItems(project.ProjectItems, fileExtension);
    }

    private static IList<string> GetDocumentsInProjectItems(
      ProjectItems projectItems,
      string fileExtension)
    {
      List<string> stringList = new List<string>();
      if (projectItems != null)
      {
        foreach (ProjectItem projectItem in projectItems)
        {
          if (projectItem != null)
          {
            Guid guid = new Guid(projectItem.Kind);
            if (guid == VSConstants.GUID_ItemType_PhysicalFile)
            {
              string str1 = projectItem.Name;
              string str2 = projectItem.get_FileNames((short) 0);
              if (!string.IsNullOrEmpty(str1) && str1.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(str2))
                stringList.Add(str2);
            }
            else if (guid == VSConstants.GUID_ItemType_PhysicalFolder)
              stringList.AddRange((IEnumerable<string>) ProjectUtils.GetDocumentsInProjectItems(projectItem.ProjectItems, fileExtension));
          }
        }
      }
      return (IList<string>) stringList;
    }
  }
}
