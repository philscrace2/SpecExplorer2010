using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.ActionMachines.Cord;
using Microsoft.SpecExplorer.VS.Common;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer.VS
{
	internal class CordCompletionProvider : ComponentBase, ICordCompletionProvider
	{
		private SpecExplorerPackage package;

		internal CordCompletionProvider(SpecExplorerPackage package)
		{
			if (package == null)
			{
				throw new ArgumentNullException("package");
			}
			this.package = package;
		}

		private IEnumerable<Completion> GetTypeCompletions(CodeElement element, IList<Namespace> imports)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Invalid comparison between Unknown and I4
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Invalid comparison between Unknown and I4
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Invalid comparison between Unknown and I4
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Expected O, but got Unknown
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Expected O, but got Unknown
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (imports == null)
			{
				throw new ArgumentNullException("imports");
			}
			HashSet<Completion> hashSet = new HashSet<Completion>();
			if (element != null)
			{
				vsCMElement kind = element.Kind;
				if ((int)kind != 1)
				{
					if ((int)kind != 5)
					{
						if ((int)kind == 8)
						{
							CodeInterface2 codeInterface = (CodeInterface2)(object)((element is CodeInterface2) ? element : null);
							if (codeInterface != null && codeInterface.Namespace != null && imports.Any((Namespace import) => import != null && import.Name != null && import.Name.Flatten().Equals(codeInterface.Namespace.FullName)))
							{
								hashSet.Add(new Completion(codeInterface.Name, codeInterface.Name, "interface " + codeInterface.FullName, CompletionResources.InterfaceCompletionIcon, "Interface"));
							}
						}
					}
					else
					{
						CodeNamespace val = (CodeNamespace)(object)((element is CodeNamespace) ? element : null);
						if (val != null && val.Members != null)
						{
							foreach (CodeElement member in val.Members)
							{
								CodeElement val2 = member;
								if (val2 != null)
								{
									hashSet.UnionWith(GetTypeCompletions(val2, imports));
								}
							}
							return hashSet;
						}
					}
				}
				else
				{
					CodeClass2 codeClass = (CodeClass2)(object)((element is CodeClass2) ? element : null);
					if (codeClass != null && codeClass.Namespace != null && imports.Any((Namespace import) => import != null && import.Name != null && import.Name.Flatten().Equals(codeClass.Namespace.FullName)))
					{
						hashSet.Add(new Completion(codeClass.Name, codeClass.Name, "class " + codeClass.FullName, CompletionResources.ClassCompletionIcon, "Class"));
					}
				}
			}
			return hashSet;
		}

		private IEnumerable<Completion> GetTypeCompletions(CodeModel codeModel, IList<Namespace> imports)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Expected O, but got Unknown
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Expected O, but got Unknown
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Invalid comparison between Unknown and I4
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Expected O, but got Unknown
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Invalid comparison between Unknown and I4
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Expected O, but got Unknown
			if (codeModel == null)
			{
				throw new ArgumentNullException("codeModel");
			}
			if (imports == null)
			{
				throw new ArgumentNullException("imports");
			}
			HashSet<Completion> hashSet = new HashSet<Completion>();
			foreach (CodeElement codeElement in codeModel.CodeElements)
			{
				CodeElement val = codeElement;
				if (val == null)
				{
					continue;
				}
				if ((int)val.Kind == 5)
				{
					CodeNamespace val2 = (CodeNamespace)(object)((val is CodeNamespace) ? val : null);
					if (val2 == null)
					{
						continue;
					}
					hashSet.Add(new Completion(val2.Name, val2.Name, "namespace " + val2.Name, CompletionResources.NamespaceCompletionIcon, "Namespace"));
					if (val2.Members == null)
					{
						continue;
					}
					foreach (CodeElement member in val2.Members)
					{
						CodeElement element = member;
						hashSet.UnionWith(GetTypeCompletions(element, imports));
					}
				}
				else if ((int)val.Kind == 1)
				{
					CodeClass2 val3 = (CodeClass2)(object)((val is CodeClass2) ? val : null);
					if (val3 != null)
					{
						hashSet.Add(new Completion(val3.Name, val3.Name, "class " + val3.FullName, CompletionResources.ClassCompletionIcon, "Class"));
					}
				}
				else if ((int)val.Kind == 8)
				{
					CodeInterface2 val4 = (CodeInterface2)(object)((val is CodeInterface2) ? val : null);
					if (val4 != null)
					{
						hashSet.Add(new Completion(val4.Name, val4.Name, "interface " + val4.FullName, CompletionResources.InterfaceCompletionIcon, "Interface"));
					}
				}
			}
			return hashSet;
		}

		private IEnumerable<Completion> GetPrimitiveTypeCompletions()
		{
			yield return new Completion("sbyte", "sbyte", "struct System.SByte\r\nRepresents an 8-bit signed integer.", CompletionResources.StructureCompletionIcon, "Structure");
			yield return new Completion("byte", "byte", "struct System.Byte\r\nRepresents an 8-bit unsigned integer.", CompletionResources.StructureCompletionIcon, "Structure");
			yield return new Completion("short", "short", "struct System.Int16\r\nRepresents a 16-bit signed integer.", CompletionResources.StructureCompletionIcon, "Structure");
			yield return new Completion("ushort", "ushort", "struct System.UInt16\r\nRepresents a 16-bit unsigned integer.", CompletionResources.StructureCompletionIcon, "Structure");
			yield return new Completion("int", "int", "struct System.Int32\r\nRepresents a 32-bit signed integer.", CompletionResources.StructureCompletionIcon, "Structure");
			yield return new Completion("uint", "uint", "struct System.UInt32\r\nRepresents a 32-bit unsigned integer.", CompletionResources.StructureCompletionIcon, "Structure");
			yield return new Completion("long", "long", "struct System.Int64\r\nRepresents a 64-bit signed integer.", CompletionResources.StructureCompletionIcon, "Structure");
			yield return new Completion("ulong", "ulong", "struct System.UInt64\r\nRepresents a 64-bit unsigned integer.", CompletionResources.StructureCompletionIcon, "Structure");
			yield return new Completion("char", "char", "struct System.Char\r\nRepresents a Unicode character.", CompletionResources.StructureCompletionIcon, "Structure");
			yield return new Completion("float", "float", "struct System.Single\r\nRepresents a single-precision floating-point number.", CompletionResources.StructureCompletionIcon, "Structure");
			yield return new Completion("double", "double", "struct System.Double\r\nRepresents a double-precision floating-point number.", CompletionResources.StructureCompletionIcon, "Structure");
			yield return new Completion("bool", "bool", "struct System.Boolean\r\nRepresents a Boolean value.", CompletionResources.StructureCompletionIcon, "Structure");
			yield return new Completion("object", "object", "class System.Object\r\nSupports all classes in the .NET Framework class hierarchy and provides low-level\r\nservices to derived classes. This is the ultimate base class of all classes\r\nin the .NET Framework; it is the root of the type hierarchy.", CompletionResources.ClassCompletionIcon, "Class");
			yield return new Completion("string", "string", "class System.String\r\nRepresents text as a series of Unicode characters.", CompletionResources.ClassCompletionIcon, "Class");
		}

		public ICordDesignTimeManager GetCordDesignTimeManager(string scriptFilePath)
		{
			if (string.IsNullOrEmpty(scriptFilePath))
			{
				throw new ArgumentNullException("cannot be null or empty.", "scriptFilePath");
			}
			ICordDesignTimeManager result = null;
			Project projectOfFile = ProjectUtils.GetProjectOfFile(scriptFilePath, package.DTE);
			if (projectOfFile != null && package.CordScopeManager != null)
			{
				result = package.CordScopeManager.GetCordDesignTimeManager(projectOfFile.UniqueName);
			}
			return result;
		}

		public IEnumerable<Completion> GetTypeCompletions(string scriptFilePath)
		{
			if (scriptFilePath == null)
			{
				throw new ArgumentNullException("scriptFilePath");
			}
			HashSet<Completion> hashSet = new HashSet<Completion>();
			Project projectOfFile = ProjectUtils.GetProjectOfFile(scriptFilePath, package.DTE);
			if (projectOfFile != null && package.CordScopeManager != null)
			{
				ICordDesignTimeManager cordDesignTimeManager = package.CordScopeManager.GetCordDesignTimeManager(projectOfFile.UniqueName);
				if (cordDesignTimeManager != null)
				{
					CoordinationScript scriptSyntax = cordDesignTimeManager.GetScriptSyntax(scriptFilePath);
					if (scriptSyntax != null)
					{
						hashSet.UnionWith(GetTypeCompletions(projectOfFile.CodeModel, scriptSyntax.GlobalNamespaces));
					}
				}
			}
			hashSet.UnionWith(GetPrimitiveTypeCompletions());
			return hashSet;
		}

		public IEnumerable<Completion> GetMemberCompletions(string parentText, string scriptFilePath, bool includeMembersOfType)
		{
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Expected O, but got Unknown
			if (string.IsNullOrEmpty(parentText))
			{
				throw new ArgumentException("parentText cannot be null or empty.", "parentText");
			}
			if (scriptFilePath == null)
			{
				throw new ArgumentNullException("scriptFilePath");
			}
			HashSet<Completion> hashSet = new HashSet<Completion>();
			Project projectOfFile = ProjectUtils.GetProjectOfFile(scriptFilePath, package.DTE);
			if (projectOfFile != null && package.CordScopeManager != null)
			{
				ICordDesignTimeManager cordDesignTimeManager = package.CordScopeManager.GetCordDesignTimeManager(projectOfFile.UniqueName);
				if (cordDesignTimeManager != null)
				{
					CoordinationScript scriptSyntax = cordDesignTimeManager.GetScriptSyntax(scriptFilePath);
					if (scriptSyntax != null && projectOfFile.CodeModel != null)
					{
						CodeType val = projectOfFile.CodeModel.CodeTypeFromFullName(parentText);
						if (val != null)
						{
							hashSet.UnionWith(GetCodeTypeMemberCompletions(val, includeMembersOfType));
						}
						if (scriptSyntax.GlobalNamespaces != null)
						{
							foreach (Namespace globalNamespace in scriptSyntax.GlobalNamespaces)
							{
								if (globalNamespace != null && globalNamespace.Name != null)
								{
									val = projectOfFile.CodeModel.CodeTypeFromFullName(globalNamespace.Name.Flatten() + "." + parentText);
									if (val != null)
									{
										hashSet.UnionWith(GetCodeTypeMemberCompletions(val, includeMembersOfType));
									}
								}
							}
						}
						if (projectOfFile.CodeModel.CodeElements != null)
						{
							foreach (CodeElement codeElement in projectOfFile.CodeModel.CodeElements)
							{
								CodeElement element = codeElement;
								CodeNamespace val2 = MatchNamespace(parentText, element);
								if (val2 != null)
								{
									hashSet.UnionWith(GetNamespaceMemberCompletions(val2));
									return hashSet;
								}
							}
							return hashSet;
						}
					}
				}
			}
			return hashSet;
		}

		private IEnumerable<Completion> GetCodeTypeMemberCompletions(
	 CodeType codeType,
	 bool includeMembersOfType)
		{
			HashSet<Completion> completionSet = new HashSet<Completion>();
			if (codeType != null && codeType.Members != null)
			{
				foreach (CodeElement allMember in codeType.Members)
				{
					if (allMember != null)
					{
						switch (allMember.Kind)
						{
							case vsCMElement.vsCMElementClass:
								CodeClass2 codeClass2 = allMember as CodeClass2;
								if (codeClass2 != null)
								{
									completionSet.Add(new Completion(codeClass2.FullName, codeClass2.Name, "class " + codeClass2.FullName, CompletionResources.ClassCompletionIcon, "Class"));
									continue;
								}
								continue;
							case vsCMElement.vsCMElementFunction:
								CodeFunction2 codeFunction2 = allMember as CodeFunction2;
								if (codeFunction2 != null && includeMembersOfType)
								{
									completionSet.Add(new Completion(codeFunction2.FullName, codeFunction2.Name, codeFunction2.FullName, CompletionResources.MethodCompletionIcon, "Method"));
									continue;
								}
								continue;
							case vsCMElement.vsCMElementEvent:
								CodeEvent codeEvent = allMember as CodeEvent;
								if (codeEvent != null && includeMembersOfType)
								{
									completionSet.Add(new Completion(codeEvent.FullName, codeEvent.Name, codeEvent.FullName, CompletionResources.EventCompletionIcon, "Event"));
									continue;
								}
								continue;
							default:
								continue;
						}
					}
				}
			}
			return (IEnumerable<Completion>)completionSet;
		}

		private CodeNamespace MatchNamespace(string namespaceName, CodeElement element)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Invalid comparison between Unknown and I4
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected O, but got Unknown
			if (element != null && (int)element.Kind == 5)
			{
				CodeNamespace val = (CodeNamespace)(object)((element is CodeNamespace) ? element : null);
				if (val != null)
				{
					if (val.FullName.Equals(namespaceName))
					{
						return val;
					}
					if (val.Members != null)
					{
						foreach (CodeElement member in val.Members)
						{
							CodeElement element2 = member;
							CodeNamespace val2 = MatchNamespace(namespaceName, element2);
							if (val2 != null)
							{
								return val2;
							}
						}
					}
				}
			}
			return null;
		}

		private IEnumerable<Completion> GetNamespaceMemberCompletions(CodeNamespace codeNamespace)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Invalid comparison between Unknown and I4
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Invalid comparison between Unknown and I4
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Invalid comparison between Unknown and I4
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected O, but got Unknown
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Expected O, but got Unknown
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Expected O, but got Unknown
			HashSet<Completion> hashSet = new HashSet<Completion>();
			if (codeNamespace != null && codeNamespace.Members != null)
			{
				foreach (CodeElement member in codeNamespace.Members)
				{
					CodeElement val = member;
					if (val != null)
					{
						vsCMElement kind = val.Kind;
						if ((int)kind != 1)
						{
							if ((int)kind != 5)
							{
								if ((int)kind == 8)
								{
									CodeInterface2 val2 = (CodeInterface2)(object)((val is CodeInterface2) ? val : null);
									if (val2 != null)
									{
										hashSet.Add(new Completion(val2.Name, val2.Name, "interface " + val2.FullName, CompletionResources.InterfaceCompletionIcon, "Interface"));
									}
								}
							}
							else
							{
								CodeNamespace val3 = (CodeNamespace)(object)((val is CodeNamespace) ? val : null);
								if (val3 != null)
								{
									hashSet.Add(new Completion(val3.Name, val3.Name, "namespace " + val3.Name, CompletionResources.NamespaceCompletionIcon, "Namespace"));
								}
							}
						}
						else
						{
							CodeClass2 val4 = (CodeClass2)(object)((val is CodeClass2) ? val : null);
							if (val4 != null)
							{
								hashSet.Add(new Completion(val4.Name, val4.Name, "class " + val4.FullName, CompletionResources.ClassCompletionIcon, "Class"));
							}
						}
					}
				}
				return hashSet;
			}
			return hashSet;
		}

		public IEnumerable<Completion> GetBehaviorCompletions(MachineDefinition machine, string scriptFilePath)
		{
			if (machine == null)
			{
				throw new ArgumentNullException("machine");
			}
			if (scriptFilePath == null)
			{
				throw new ArgumentNullException("scriptFilePath");
			}
			HashSet<Completion> hashSet = new HashSet<Completion>();
			Project projectOfFile = ProjectUtils.GetProjectOfFile(scriptFilePath, package.DTE);
			if (projectOfFile != null && package.CordScopeManager != null)
			{
				ICordDesignTimeManager cordDesignTimeManager = package.CordScopeManager.GetCordDesignTimeManager(projectOfFile.UniqueName);
				if (cordDesignTimeManager != null)
				{
					CoordinationScript scriptSyntax = cordDesignTimeManager.GetScriptSyntax(scriptFilePath);
					if (scriptSyntax != null)
					{
						hashSet.UnionWith(GetMachineCompletions(machine, scriptSyntax));
						hashSet.UnionWith(GetActionCompletions(machine, projectOfFile, scriptSyntax));
					}
				}
			}
			return hashSet;
		}

		private static IEnumerable<Completion> GetActionCompletions(MachineDefinition machine, Project project, CoordinationScript ast)
		{
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Expected O, but got Unknown
			HashSet<Completion> hashSet = new HashSet<Completion>();
			if (machine != null && machine.Vocabularies != null && ast != null)
			{
				List<ConfigClause.IncludeConfig> list = new List<ConfigClause.IncludeConfig>();
				ConfigReference[] vocabularies = machine.Vocabularies;
				foreach (ConfigReference configReference in vocabularies)
				{
					list.Add(new ConfigClause.IncludeConfig(new ConfigReference(configReference.Name, machine.Location)));
				}
				Config voc = new Config(null, machine.Location, "", list);
				VocabularyVisitor vocabularyVisitor = new VocabularyVisitor(ast, ast.GlobalNamespaces, project);
				vocabularyVisitor.VisitVocabulary(voc);
				{
					foreach (ActionDeclaration item in vocabularyVisitor.ImportActions.Except(vocabularyVisitor.ExcludedActions))
					{
						hashSet.Add(new Completion(item.Name, item.Name, "action " + item.FullName, CompletionResources.MethodCompletionIcon, "Method"));
					}
					return hashSet;
				}
			}
			return hashSet;
		}

		public IEnumerable<Completion> GetMachineCompletions(MachineDefinition machine, string scriptFilePath)
		{
			if (machine == null)
			{
				throw new ArgumentNullException("machine");
			}
			if (scriptFilePath == null)
			{
				throw new ArgumentNullException("scriptFilePath");
			}
			HashSet<Completion> hashSet = new HashSet<Completion>();
			Project projectOfFile = ProjectUtils.GetProjectOfFile(scriptFilePath, package.DTE);
			if (projectOfFile != null && package.CordScopeManager != null)
			{
				ICordDesignTimeManager cordDesignTimeManager = package.CordScopeManager.GetCordDesignTimeManager(projectOfFile.UniqueName);
				if (cordDesignTimeManager != null)
				{
					CoordinationScript scriptSyntax = cordDesignTimeManager.GetScriptSyntax(scriptFilePath);
					if (scriptSyntax != null)
					{
						hashSet.UnionWith(GetMachineCompletions(machine, scriptSyntax));
					}
				}
			}
			return hashSet;
		}

		private static IEnumerable<Completion> GetMachineCompletions(MachineDefinition machine, CoordinationScript ast)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Expected O, but got Unknown
			HashSet<Completion> hashSet = new HashSet<Completion>();
			if (ast != null && ast.Machines != null)
			{
				foreach (MachineDefinition machine2 in ast.Machines)
				{
					if (machine2 != null && machine2.Name != machine.Name)
					{
						hashSet.Add(new Completion(machine2.Name, machine2.Name, "machine " + machine2.Name, CompletionResources.ClassCompletionIcon, "Class"));
					}
				}
				return hashSet;
			}
			return hashSet;
		}

		public void NavigateTo(string fileName, int line, int column)
		{
			package.NavigateTo(fileName, line, column);
		}
	}
}
