using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Microsoft.SpecExplorer.VS
{
	internal static class EnvDTEUtils
	{
		public static IEnumerable<CodeImport> RetrieveNamespaceImports(CodeClass codeClass)
		{
			IEnumerable<CodeImport> enumerable = ((IEnumerable)codeClass.ProjectItem.FileCodeModel.CodeElements).OfType<CodeImport>();
			CodeNamespace val = codeClass.Namespace;
			while (val != null)
			{
				enumerable.Union(((IEnumerable)val.Members).OfType<CodeImport>());
				object parent = val.Parent;
				val = (CodeNamespace)((parent is CodeNamespace) ? parent : null);
			}
			return enumerable;
		}

		public static string ShortenTypeName(string typeName, IEnumerable<string> importedNamespaces, Project containerProject)
		{
			if (typeName == null || !typeName.Contains('.'))
			{
				return typeName;
			}
			CodeType val = containerProject.CodeModel.CodeTypeFromFullName(typeName);
			if (val == null)
			{
				return typeName;
			}
			string typeNameV2 = (importedNamespaces.Contains(val.Namespace.FullName) ? typeName.Substring(val.Namespace.FullName.Length + 1) : typeName);
			string text = ((importedNamespaces.Count((string ns) => containerProject.CodeModel.CodeTypeFromFullName(ns + "." + typeNameV2) != null) != 1) ? typeName : typeNameV2);
			if (text.Contains('<'))
			{
				int num = text.IndexOf('<');
				int num2 = text.LastIndexOf('>');
				IEnumerable<string> source = SplitIntoTypes(text.Substring(num + 1, num2 - num - 1));
				IEnumerable<string> source2 = source.Select((string name) => ShortenTypeName(name, importedNamespaces, containerProject));
				return string.Format("{0}<{1}>", text.Substring(0, num), source2.Aggregate((string combined, string next) => combined + ", " + next));
			}
			return text;
		}

		private static IEnumerable<string> SplitIntoTypes(string typeString)
		{
			int start = 0;
			int bracketBalance = 0;
			for (int i = 0; i < typeString.Length; i++)
			{
				switch (typeString[i])
				{
				case '<':
					bracketBalance++;
					break;
				case '>':
					bracketBalance--;
					break;
				case ' ':
				case ',':
					if (bracketBalance == 0)
					{
						string type = typeString.Substring(start, i - start);
						start = i + 1;
						if (!string.IsNullOrEmpty(type))
						{
							yield return type;
						}
					}
					break;
				}
			}
			string remaining = typeString.Substring(start, typeString.Length - start);
			if (!string.IsNullOrEmpty(remaining))
			{
				yield return remaining;
			}
		}
	}
}
