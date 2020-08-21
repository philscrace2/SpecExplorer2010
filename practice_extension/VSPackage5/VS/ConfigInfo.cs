// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ConfigInfo
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.Xrt;
using System.IO;

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
      this.DisplayText = "<New Configuration>";
    }

    public ConfigInfo(string containerProject, string containerScript, string configName)
    {
      this.ContainerProject = containerProject;
      this.ContainerScript = containerScript;
      this.ConfigName = configName;
      this.DisplayText = string.Format("{0} [{1} :: {2}]", (object) this.ConfigName, (object) Path.GetFileNameWithoutExtension(this.ContainerProject), (object) Path.GetFileName(this.ContainerScript));
    }

    public override bool Equals(object obj)
    {
      ConfigInfo configInfo = obj as ConfigInfo;
      if (configInfo == null || !(this.ConfigName == configInfo.ConfigName) || !(this.ContainerScript == configInfo.ContainerScript))
        return false;
      return this.ContainerProject == configInfo.ContainerProject;
    }

    public override int GetHashCode()
    {
        Hash32Builder hash32Builder = new Hash32Builder(); ;
      if (this.ConfigName != null)
        ((Hash32Builder) hash32Builder).Add(this.ConfigName.GetHashCode());
      if (this.ContainerScript != null)
        ((Hash32Builder) hash32Builder).Add(this.ContainerScript.GetHashCode());
      if (this.ContainerProject != null)
        ((Hash32Builder) hash32Builder).Add(this.ContainerProject.GetHashCode());
      return ((Hash32Builder) hash32Builder).Result;
    }
  }
}
