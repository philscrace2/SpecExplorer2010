using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.ActionMachines.Cord;

namespace Microsoft.SpecExplorer.VS
{
	internal class CodeElementTypeResolver
	{
		private HashSet<CodeElement> candidates = new HashSet<CodeElement>();

		private IList<Namespace> imports;

		private Project project;

		internal CodeElementTypeResolver(IList<Namespace> imports, Project project)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			this.project = project;
			this.imports = imports;
		}

		internal CodeElement ResolveTypeUnique(Microsoft.ActionMachines.Cord.Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			IList<CodeElement> list = ResolveType(type);
			if (list != null && list.Count == 1)
			{
				return list.First();
			}
			return null;
		}

		internal IList<CodeElement> ResolveType(Microsoft.ActionMachines.Cord.Type type)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Expected O, but got Unknown
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			candidates.Clear();
			foreach (CodeElement codeElement in project.CodeModel.CodeElements)
			{
				CodeElement element = codeElement;
				ResolveType(type, element);
			}
			return candidates.ToList();
		}

		private void ResolveType(Microsoft.ActionMachines.Cord.Type type, CodeElement element)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Invalid comparison between Unknown and I4
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Invalid comparison between Unknown and I4
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			if (element == null)
			{
				return;
			}
			vsCMElement kind = element.Kind;
			if ((int)kind != 1)
			{
				if ((int)kind != 5)
				{
					if ((int)kind == 8)
					{
						CodeInterface2 val = (CodeInterface2)(object)((element is CodeInterface2) ? element : null);
						if (val != null && IsTypeMatch(type, val.FullName))
						{
							candidates.Add(element);
						}
					}
					return;
				}
				CodeNamespace val2 = (CodeNamespace)(object)((element is CodeNamespace) ? element : null);
				if (val2 == null || val2.Members == null)
				{
					return;
				}
				foreach (CodeElement member in val2.Members)
				{
					CodeElement element2 = member;
					ResolveType(type, element2);
				}
				return;
			}
			CodeClass2 val3 = (CodeClass2)(object)((element is CodeClass2) ? element : null);
			if (val3 != null && IsTypeMatch(type, val3.FullName))
			{
				candidates.Add(element);
			}
			foreach (CodeElement allMember in val3.GetAllMembers())
			{
				ResolveType(type, allMember);
			}
		}

		private bool IsTypeMatch(Microsoft.ActionMachines.Cord.Type type, string fullName)
		{
			string typeName = type.Flatten();
			if (fullName == typeName)
			{
				return true;
			}
			if (imports.Any((Namespace import) => import != null && import.Name != null && fullName.Equals(import.Name.Flatten() + "." + typeName)))
			{
				return true;
			}
			return false;
		}
	}
}
