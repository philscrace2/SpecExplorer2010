// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Flavor.FlavoredProjectFactory
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell.Flavor
{
  [CLSCompliant(false)]
  public abstract class FlavoredProjectFactory : IVsAggregatableProjectFactory, IVsProjectFactory
  {
    private ServiceProvider _serviceProvider;

    protected ServiceProvider serviceProvider
    {
      get
      {
        return this._serviceProvider;
      }
    }

    int IVsProjectFactory.CanCreateProject(
      string fileName,
      uint flags,
      out int canCreate)
    {
      canCreate = this.CanCreateProject(fileName, flags) ? 1 : 0;
      return 0;
    }

    protected virtual bool CanCreateProject(string fileName, uint flags)
    {
      return !string.IsNullOrEmpty(fileName) | !PackageUtilities.ContainsInvalidFileNameChars(fileName);
    }

    int IVsProjectFactory.CreateProject(
      string fileName,
      string location,
      string name,
      uint flags,
      ref Guid projectGuid,
      out IntPtr project,
      out int canceled)
    {
      this.CreateProject(fileName, location, name, flags, ref projectGuid, out project, out canceled);
      return 0;
    }

    protected virtual void CreateProject(
      string fileName,
      string location,
      string name,
      uint flags,
      ref Guid projectGuid,
      out IntPtr project,
      out int canceled)
    {
      project = IntPtr.Zero;
      canceled = 0;
    }

    int IVsProjectFactory.Close()
    {
      this.Dispose(true);
      return 0;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this._serviceProvider == null)
        return;
      this._serviceProvider.Dispose();
      this._serviceProvider = (ServiceProvider) null;
    }

    int IVsProjectFactory.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider provider)
    {
      this._serviceProvider = new ServiceProvider(provider);
      this.Initialize();
      return 0;
    }

    protected virtual void Initialize()
    {
    }

    int IVsAggregatableProjectFactory.GetAggregateProjectType(
      string fileName,
      out string projectTypeGuid)
    {
      projectTypeGuid = this.ProjectTypeGuids(fileName);
      return 0;
    }

    int IVsAggregatableProjectFactory.PreCreateForOuter(
      object outerProject,
      out object project)
    {
      project = (object) null;
      project = this.PreCreateForOuter(outerProject);
      if (!(project is FlavoredProject))
        Trace.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Expected to recieve a FlavoredProject from PreCreateForOuter.\n Recieved a {0}", (object) project.GetType().FullName));
      return project == null ? -2147467259 : 0;
    }

    protected abstract object PreCreateForOuter(object outerProject);

    protected virtual string ProjectTypeGuids(string file)
    {
      throw new NotImplementedException();
    }
  }
}
