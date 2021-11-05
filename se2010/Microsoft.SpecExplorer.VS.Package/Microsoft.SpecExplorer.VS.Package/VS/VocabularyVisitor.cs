using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.ActionMachines.Cord;

namespace Microsoft.SpecExplorer.VS
{
	internal class VocabularyVisitor : SyntaxVisitor
	{
		private Dictionary<Config, VisitingState> visitingState = new Dictionary<Config, VisitingState>();

		private Stack<Config> visitingConfigs = new Stack<Config>();

		private ActionDeclarationBuilder actionBuilder;

		private IList<Namespace> imports;

		private Project project;

		private CoordinationScript ast;

		public IEnumerable<ActionDeclaration> ImportActions { get; private set; }

		public IEnumerable<ActionDeclaration> ExcludedActions { get; private set; }

		internal VocabularyVisitor(CoordinationScript ast, IList<Namespace> imports, Project project)
		{
			this.ast = ast;
			this.imports = imports;
			this.project = project;
			ImportActions = Enumerable.Empty<ActionDeclaration>();
			ExcludedActions = Enumerable.Empty<ActionDeclaration>();
		}

		public override void VisitVocabulary(Config voc)
		{
			if (visitingState.ContainsKey(voc))
			{
				switch (visitingState[voc])
				{
				default:
					return;
				case VisitingState.Unvisited:
					break;
				}
			}
			visitingState[voc] = VisitingState.Visiting;
			visitingConfigs.Push(voc);
			HashSet<ActionDeclaration> hashSet = new HashSet<ActionDeclaration>();
			if (voc.Clauses != null)
			{
				ConfigClause.IncludeConfig baseVoc;
				foreach (ConfigClause.IncludeConfig item in from c in voc.Clauses.OfType<ConfigClause.IncludeConfig>()
					where c.Vocabulary != null
					select c)
				{
					baseVoc = item;
					Config config = ast.Configs.FirstOrDefault((Config c) => c.Name == baseVoc.Vocabulary.Name);
					if (config != null)
					{
						config.Accept(this);
						hashSet.UnionWith(ImportActions);
						hashSet.ExceptWith(ExcludedActions);
						ImportActions = Enumerable.Empty<ActionDeclaration>();
						ExcludedActions = Enumerable.Empty<ActionDeclaration>();
					}
				}
			}
			ActionDeclarationBuilder actionDeclarationBuilder = actionBuilder;
			actionBuilder = new ActionDeclarationBuilder();
			voc.AcceptOnChildren(this);
			hashSet.UnionWith(actionBuilder.ImportActions);
			ImportActions = hashSet;
			ExcludedActions = actionBuilder.ExcludedActions;
			visitingState[voc] = VisitingState.Visited;
			visitingConfigs.Pop();
			actionBuilder = actionDeclarationBuilder;
		}

		public override void VisitImportAllFromScope(ConfigClause.ImportAllFromScope ua)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Invalid comparison between Unknown and I4
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Invalid comparison between Unknown and I4
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Expected O, but got Unknown
			if (ua == null || ua.FromType == null)
			{
				return;
			}
			CodeElementTypeResolver codeElementTypeResolver = new CodeElementTypeResolver(imports, project);
			CodeElement val = codeElementTypeResolver.ResolveTypeUnique(ua.FromType);
			if (val == null)
			{
				return;
			}
			CordSyntaxElementBuilder cordSyntaxElementBuilder = new CordSyntaxElementBuilder(null, null, null);
			if ((int)val.Kind == 1)
			{
				CodeClass2 val2 = (CodeClass2)(object)((val is CodeClass2) ? val : null);
				if (val2 == null)
				{
					return;
				}
				foreach (CodeElement allMember in val2.GetAllMembers())
				{
					if (allMember != null)
					{
						ConfigClause.ImportMethod importMethod = cordSyntaxElementBuilder.CreateActionImport(allMember, project.CodeModel.CodeTypeFromFullName(val2.FullName));
						if (importMethod != null && importMethod.Method != null)
						{
							actionBuilder.AddAction(importMethod.Method);
						}
					}
				}
			}
			else
			{
				if ((int)val.Kind != 8)
				{
					return;
				}
				CodeInterface2 val3 = (CodeInterface2)(object)((val is CodeInterface2) ? val : null);
				if (val3 == null)
				{
					return;
				}
				foreach (CodeElement member in val3.Members)
				{
					CodeElement val4 = member;
					if (val4 != null)
					{
						ConfigClause.ImportMethod importMethod2 = cordSyntaxElementBuilder.CreateActionImport(val4, project.CodeModel.CodeTypeFromFullName(val3.FullName));
						if (importMethod2 != null && importMethod2.Method != null)
						{
							actionBuilder.AddAction(importMethod2.Method);
						}
					}
				}
			}
		}

		public override void VisitImportEvent(ConfigClause.ImportMethod um)
		{
			if (um != null && um.Method != null)
			{
				actionBuilder.AddAction(um.Method);
			}
		}

		public override void VisitImportMethod(ConfigClause.ImportMethod um)
		{
			if (um != null && um.Method != null)
			{
				actionBuilder.AddAction(um.Method);
			}
		}

		public override void VisitExcludeMethod(ConfigClause.ExcludeMethod um)
		{
			if (um != null && um.Method != null)
			{
				actionBuilder.ExcludeAction(um.Method);
			}
		}

		public override void VisitDeclareEvent(ConfigClause.DeclareMethod dm)
		{
			if (dm != null && dm.Method != null)
			{
				actionBuilder.AddAction(dm.Method);
			}
		}

		public override void VisitDeclareMethod(ConfigClause.DeclareMethod dm)
		{
			if (dm != null && dm.Method != null)
			{
				actionBuilder.AddAction(dm.Method);
			}
		}
	}
}
