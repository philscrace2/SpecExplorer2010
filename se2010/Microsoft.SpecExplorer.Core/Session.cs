// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Session
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines.Cord;
using Microsoft.SpecExplorer.ModelingGuidance;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.Viewer;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Microsoft.SpecExplorer
{
  public class Session : ComponentBase, ISession
  {
    private IHost host;
    private string installDir;
    private static bool resolving;

    public Session(IHost host)
    {
      this.host = host != null ? host : throw new ArgumentNullException(nameof (host));
      this.Application = (ApplicationBase) new SpecExplorerApplicationBase(this.ConfigurationDir, false, new ExplorerMediator(this));
      this.Application.Setup.Add((IComponent) this);
      this.RegisterServices();
      AppDomain.CurrentDomain.AssemblyResolve += (ResolveEventHandler) ((sender, args) => Session.ResolveAssemblyFromPotentialOtherLoadContext(args.Name));
    }

    private void RegisterServices()
    {
      this.Application.Setup.Add((IComponent) new CordDesignTimeScopeManager((IEnumerable<string>) new string[1]
      {
        Path.Combine(this.ConfigurationDir, this.Application.Configuration.DefaultCordPreludeFileName)
      }));
      this.Application.Setup.Add((IComponent) new ViewDefinitionManager(this.host));
      this.Application.Setup.Add((IComponent) new GuidanceLoaderImpl());
    }

    internal static System.Reflection.Assembly ResolveAssemblyFromPotentialOtherLoadContext(
      string name)
    {
      if (Session.resolving)
        return (System.Reflection.Assembly) null;
      try
      {
        Session.resolving = true;
        foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          if (assembly.FullName == name)
            return assembly;
        }
        return System.Reflection.Assembly.Load(name);
      }
      catch
      {
        return (System.Reflection.Assembly) null;
      }
      finally
      {
        Session.resolving = false;
      }
    }

    public ApplicationBase Application { get; private set; }

    public IHost Host => this.host;

    public IExplorer CreateExplorer(
      ICollection<string> assemblies,
      ICollection<string> scripts,
      ExplorationMode explorationMode,
      string machine,
      string outputDir,
      string replayPath,
      int? onTheFlyMaximumExperimentCount,
      IDictionary<string, string> machineSwitches,
      bool allowEndingAtEventStates)
    {
      return (IExplorer) new Explorer(this, new ExplorerConfiguration(assemblies, scripts, explorationMode, machine, machineSwitches, outputDir, replayPath, onTheFlyMaximumExperimentCount, allowEndingAtEventStates));
    }

    public ITestCodeGenerator CreateTestCodeGenerator(
      TransitionSystem transitionSystem)
    {
      return (ITestCodeGenerator) new StaticTestCodeGenerator(this.Host, transitionSystem);
    }

    public string InstallDir
    {
      get
      {
        if (this.installDir == null)
        {
          string empty = string.Empty;
          string directoryName = Path.GetDirectoryName(new Uri(typeof (Session).Assembly.CodeBase).LocalPath);
          this.installDir = directoryName;
          while (!string.IsNullOrEmpty(this.installDir))
          {
            if (!File.Exists(Path.Combine(this.installDir, "xrt.config")))
            {
              try
              {
                this.installDir = Path.GetDirectoryName(this.installDir);
              }
              catch (Exception ex)
              {
                empty += string.Format("Failed to get directory name from '{0}' : {1}", (object) this.installDir, (object) ex.Message);
                this.installDir = (string) null;
              }
            }
            else
              break;
          }
          if (this.installDir == null)
            this.Host.FatalError("Cannot retrieve installation directory." + (empty + string.Format("Failed to find xrt.config file in path '{0}'.", (object) directoryName)));
        }
        return this.installDir;
      }
    }

    public string ConfigurationDir => this.InstallDir;

    public override void Dispose()
    {
      if (!this.IsDisposed && this.Application != null)
      {
        this.Application.Dispose();
        this.Application = (ApplicationBase) null;
      }
      base.Dispose();
      GC.SuppressFinalize((object) this);
    }
  }
}
