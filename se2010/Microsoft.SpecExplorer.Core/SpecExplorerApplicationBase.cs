using System;
using System.IO;
using System.Xml;
using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.ActionMachines.Cord.Construct;
using Microsoft.SpecExplorer.Extensions;
using Microsoft.SpecExplorer.VSService;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	internal class SpecExplorerApplicationBase : ActionMachineApplicationBase, ISourceLocationProvider
	{
		private string baseDir;

		private ExplorerMediator explorerMediator;

		public override string PersistentConfigurationBaseDir
		{
			get
			{
				return baseDir;
			}
		}

		internal SpecExplorerApplicationBase(string baseDir, bool allowLoadingRuntimeAssemblies, ExplorerMediator explorerMediator)
			: this(baseDir, allowLoadingRuntimeAssemblies, explorerMediator, null, null, null)
		{
		}

		internal SpecExplorerApplicationBase(string baseDir, bool allowLoadingRuntimeAssemblies, ExplorerMediator explorerMediator, Version targetPlatformVersion, string extensionAssemblyFolder, int? constraintSolverTimeout)
		{
			this.baseDir = baseDir;
			this.explorerMediator = explorerMediator;
			base.Configuration.Options.TargetPlatformVersion = targetPlatformVersion;
			base.Configuration.Options.ExtensionAssemblyFolder = extensionAssemblyFolder;
			base.Configuration.Options.AllowLoadingRuntimeAssemblies = allowLoadingRuntimeAssemblies;
			if (constraintSolverTimeout.HasValue)
			{
				base.Configuration.Options.ConstraintSolverTimeout = constraintSolverTimeout.Value;
			}
			AddPersistentOptions();
			InstallActionMachineComponents();
			InstallAdditionalServices(allowLoadingRuntimeAssemblies);
		}

		private void InstallAdditionalServices(bool allowLoadingRuntimeAssemblies)
		{
			base.Setup.Add(new CordProvider());
			base.Setup.Add(new CodeGeneratorProvider());
			base.Setup.Add(new RuleMachineConstructProvider());
			base.Setup.Add(new EdgeCoverageMachineConstructProvider());
			base.Setup.Add(new PointShootMachinesConstructProvider());
			base.Setup.Add(new BoundedExplorationMachinesConstructProvider());
			base.Setup.Add(new AcceptCompletionMachinesConstructProvider());
			base.Setup.Add(new AcceptingTraversalMachinesProvider());
			base.Setup.Add(new DeadPathEliminationMachineConstructProvider());
			base.Setup.Add(new ParameterExpansionMachinesConstructProvider());
			base.Setup.Add(new ConstructManager());
			base.Setup.Add(new VSServiceProvider(explorerMediator));
			ExtensionLoader extensionLoader = new ExtensionLoader(base.Setup, explorerMediator);
			extensionLoader.LoadExtension(baseDir, !allowLoadingRuntimeAssemblies);
		}

		public override XmlDocument LoadPersistentConfiguration()
		{
			string filename = Path.Combine(PersistentConfigurationBaseDir, "xrt.config");
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			return xmlDocument;
		}

		public bool TryFindLocation(IMember member, out SourceLocation location)
		{
			TextLocation location2 = default(TextLocation);
			bool flag = false;
			if (member is IMethod)
			{
				IMethod method = member as IMethod;
				MemberInfo memberInfo = null;
				if (method.AssociationReferences != null && method.AssociationReferences.Length == 1 && method.AssociationReferences[0].Association.Kind == AssociationKind.Property)
				{
					IAssociation association = method.AssociationReferences[0].Association;
					memberInfo = new MemberInfo(method.DeclaringType.FullName, MemberKind.Property, association.ShortName);
				}
				else
				{
					string name = (method.IsConstructor ? method.DeclaringType.ShortName : method.ShortName);
					memberInfo = new MemberInfo(method.DeclaringType.FullName, MemberKind.Method, name, method.Parameters);
				}
				flag = explorerMediator.TryFindLocation(memberInfo, out location2);
			}
			else if (member is IField)
			{
				IField field = member as IField;
				flag = explorerMediator.TryFindLocation(new MemberInfo(field.DeclaringType.FullName, MemberKind.Field, field.ShortName), out location2);
			}
			else if (member is IType)
			{
				IType type = member as IType;
				flag = explorerMediator.TryFindLocation(new MemberInfo(type.FullName, MemberKind.Type, type.ShortName), out location2);
			}
			if (flag)
			{
				IProgram requiredService = GetRequiredService<IProgram>();
				location = requiredService.MakeTextLocation(new Uri(location2.FileName), location2.FirstLine, location2.FirstColumn, location2.LastLine, location2.LastColumn);
			}
			else
			{
				location = default(SourceLocation);
			}
			return flag;
		}
	}
}
