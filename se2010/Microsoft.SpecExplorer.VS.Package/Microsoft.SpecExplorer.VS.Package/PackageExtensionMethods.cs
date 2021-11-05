using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.SpecExplorer
{
	public static class PackageExtensionMethods
	{
		public static IEnumerable<CodeElement> GetAllMembers(this CodeType codeType)
		{
			if (codeType == null)
			{
				yield break;
			}
			CodeClass2 codeClass = (CodeClass2)(object)((codeType is CodeClass2) ? codeType : null);
			if (codeClass != null)
			{
				foreach (CodeElement allMember in codeClass.GetAllMembers())
				{
					yield return allMember;
				}
			}
			else
			{
				if (codeType.Members == null)
				{
					yield break;
				}
				foreach (CodeElement member in codeType.Members)
				{
					yield return member;
				}
			}
		}

		public static IEnumerable<CodeElement> GetAllMembers(this CodeClass2 codeClass)
		{
			if (codeClass == null)
			{
				yield break;
			}
			if ((int)codeClass.InfoLocation == 1)
			{
				if (codeClass.Parts == null)
				{
					yield break;
				}
				foreach (object p in codeClass.Parts)
				{
					CodeClass2 part = (CodeClass2)((p is CodeClass2) ? p : null);
					if (part == null || part.Members == null)
					{
						continue;
					}
					foreach (CodeElement member in part.Members)
					{
						yield return member;
					}
				}
			}
			else
			{
				if (codeClass.Members == null)
				{
					yield break;
				}
				foreach (CodeElement member2 in codeClass.Members)
				{
					yield return member2;
				}
			}
		}

		public static OLEMSGBUTTON ToOleMessageButton(this MessageButton messageButton)
		{
			switch (messageButton)
			{
			case MessageButton.ABORTRETRYIGNORE:
				return (OLEMSGBUTTON)2;
			case MessageButton.OK:
				return (OLEMSGBUTTON)0;
			case MessageButton.OKCANCEL:
				return (OLEMSGBUTTON)1;
			case MessageButton.RETRYCANCEL:
				return (OLEMSGBUTTON)5;
			case MessageButton.YESALLNOCANCEL:
				return (OLEMSGBUTTON)6;
			case MessageButton.YESNO:
				return (OLEMSGBUTTON)4;
			default:
				return (OLEMSGBUTTON)3;
			}
		}

		public static bool IsValid(this CodeNamespace codeNamespace)
		{
			if (codeNamespace != null && !codeNamespace.Name.Equals("System"))
			{
				return !codeNamespace.Name.Equals("MS");
			}
			return false;
		}
	}
}
