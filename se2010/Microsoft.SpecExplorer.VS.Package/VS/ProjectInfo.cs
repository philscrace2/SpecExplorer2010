// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ProjectInfo
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System.IO;

namespace Microsoft.SpecExplorer.VS
{
  public class ProjectInfo : ICordSyntaxElementInfo
  {
    public string ProjectName { get; private set; }

    public string DisplayText { get; private set; }

    public ProjectInfo(string projectName)
    {
      this.ProjectName = projectName;
      this.DisplayText = Path.GetFileNameWithoutExtension(this.ProjectName);
    }
  }
}
