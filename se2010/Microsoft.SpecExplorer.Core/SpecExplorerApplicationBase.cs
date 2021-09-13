// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.SpecExplorerApplicationBase
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.ActionMachines.Cord.Construct;
using Microsoft.SpecExplorer.Extensions;
using Microsoft.SpecExplorer.VSService;
using Microsoft.Xrt;
using System;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace Microsoft.SpecExplorer
{
  internal class SpecExplorerApplicationBase : ActionMachineApplicationBase, ISourceLocationProvider
  {
    private string baseDir;
    private ExplorerMediator explorerMediator;

    internal SpecExplorerApplicationBase(
      string baseDir,
      bool allowLoadingRuntimeAssemblies,
      ExplorerMediator explorerMediator)
      : this(baseDir, allowLoadingRuntimeAssemblies, explorerMediator, (Version) null, (string) null, new int?())
    {
    }

    internal SpecExplorerApplicationBase(
      string baseDir,
      bool allowLoadingRuntimeAssemblies,
      ExplorerMediator explorerMediator,
      Version targetPlatformVersion,
      string extensionAssemblyFolder,
      int? constraintSolverTimeout)
    {
      this.baseDir = baseDir;
      this.explorerMediator = explorerMediator;
      this.Configuration.Options.TargetPlatformVersion = targetPlatformVersion;
      this.Configuration.Options.ExtensionAssemblyFolder = extensionAssemblyFolder;
      this.Configuration.Options.AllowLoadingRuntimeAssemblies = allowLoadingRuntimeAssemblies;
      if (constraintSolverTimeout.HasValue)
        this.Configuration.Options.ConstraintSolverTimeout = constraintSolverTimeout.Value;
      this.AddPersistentOptions();
      this.InstallActionMachineComponents();
      this.InstallAdditionalServices(allowLoadingRuntimeAssemblies);
    }

    private void InstallAdditionalServices(bool allowLoadingRuntimeAssemblies)
    {
      this.Setup.Add((IComponent) new CordProvider());
      this.Setup.Add((IComponent) new CodeGeneratorProvider());
      this.Setup.Add((IComponent) new RuleMachineConstructProvider());
      this.Setup.Add((IComponent) new EdgeCoverageMachineConstructProvider());
      this.Setup.Add((IComponent) new PointShootMachinesConstructProvider());
      this.Setup.Add((IComponent) new BoundedExplorationMachinesConstructProvider());
      this.Setup.Add((IComponent) new AcceptCompletionMachinesConstructProvider());
      this.Setup.Add((IComponent) new AcceptingTraversalMachinesProvider());
      this.Setup.Add((IComponent) new DeadPathEliminationMachineConstructProvider());
      this.Setup.Add((IComponent) new ParameterExpansionMachinesConstructProvider());
      this.Setup.Add((IComponent) new ConstructManager());
      this.Setup.Add((IComponent) new VSServiceProvider(this.explorerMediator));
      new ExtensionLoader(this.Setup, this.explorerMediator).LoadExtension(this.baseDir, !allowLoadingRuntimeAssemblies);
    }

    public override string PersistentConfigurationBaseDir => this.baseDir;

    public override XmlDocument LoadPersistentConfiguration()
    {
      string filename = Path.Combine(this.PersistentConfigurationBaseDir, "xrt.config");
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(filename);
      return xmlDocument;
    }

    public bool TryFindLocation(IMember member, out SourceLocation location)
    {
      TextLocation location1 = new TextLocation();
      bool flag = false;
      switch (member)
      {
        case IMethod _:
          IMethod method = member as IMethod;
          MemberInfo member1;
          if (method.AssociationReferences != null && method.AssociationReferences.Length == 1 && method.AssociationReferences[0].Association.Kind == AssociationKind.Property)
          {
            IAssociation association = method.AssociationReferences[0].Association;
            member1 = new MemberInfo(method.DeclaringType.FullName, MemberKind.Property, association.ShortName);
          }
          else
          {
            string name = method.IsConstructor ? method.DeclaringType.ShortName : method.ShortName;
            member1 = new MemberInfo(method.DeclaringType.FullName, MemberKind.Method, name, method.Parameters);
          }
          flag = this.explorerMediator.TryFindLocation(member1, out location1);
          break;
        case IField _:
          IField field = member as IField;
          flag = this.explorerMediator.TryFindLocation(new MemberInfo(field.DeclaringType.FullName, MemberKind.Field, field.ShortName), out location1);
          break;
        case IType _:
          IType type = member as IType;
          flag = this.explorerMediator.TryFindLocation(new MemberInfo(type.FullName, MemberKind.Type, type.ShortName), out location1);
          break;
      }
      if (flag)
      {
        IProgram requiredService = this.GetRequiredService<IProgram>();
        location = requiredService.MakeTextLocation(new Uri(location1.FileName), (int) location1.FirstLine, (int) location1.FirstColumn, (int) location1.LastLine, (int) location1.LastColumn);
      }
      else
        location = new SourceLocation();
      return flag;
    }
  }
}
