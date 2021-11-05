using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.ActionMachines;
using Microsoft.ActionMachines.Cord;
using Microsoft.Modeling;

namespace Microsoft.SpecExplorer.VS
{
	internal class CordSyntaxElementBuilder
	{
		private Project containerProject;

		private IEnumerable<string> importedNamespaces;

		public CordSyntaxElementBuilder(SpecExplorerPackage package, string containerProjectName, string containerScriptPath)
		{
			containerProject = package.GetProjectByUniqueName(containerProjectName);
			CoordinationScript scriptSyntax = package.CordScopeManager.GetCordDesignTimeManager(containerProjectName).GetScriptSyntax(containerScriptPath);
			importedNamespaces = scriptSyntax.GlobalNamespaces.Select((Namespace import) => import.Name.Flatten());
		}

		public Config CreateConfig(string configName)
		{
			return new Config(null, Location.None, configName, null);
		}

		public ConfigClause.ImportMethod CreateActionImport(CodeElement codeElement, CodeType container)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Invalid comparison between Unknown and I4
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			vsCMElement kind = codeElement.Kind;
			MethodDescriptor methodDescriptor = (((int)kind == 2) ? CreateMethodDescriptor((CodeFunction)(object)((codeElement is CodeFunction) ? codeElement : null), container) : (((int)kind != 38) ? null : CreateMethodDescriptor((CodeEvent)(object)((codeElement is CodeEvent) ? codeElement : null), container)));
			if (methodDescriptor != null)
			{
				return new ConfigClause.ImportMethod(Location.None, methodDescriptor);
			}
			return null;
		}

		public MethodDescriptor CreateMethodDescriptor(CodeFunction codeFunction, CodeType container)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Invalid comparison between Unknown and I4
			Parameter[] array = CreateParameters(codeFunction.Parameters, false);
			string name = EnvDTEUtils.ShortenTypeName(container.FullName, importedNamespaces, containerProject);
			InstantiatedName prefix = new InstantiatedName(Location.None, null, name, null);
			Type.Simple simple;
			InstantiatedName methodName;
			if ((int)codeFunction.FunctionKind == 1)
			{
				object parent = codeFunction.Parent;
				simple = CreateType(((CodeElement)((parent is CodeElement) ? parent : null)).FullName);
				methodName = null;
			}
			else
			{
				simple = CreateType(codeFunction.Type);
				methodName = new InstantiatedName(Location.None, prefix, codeFunction.Name, null);
			}
			if (array == null || simple == null)
			{
				return null;
			}
			return new MethodDescriptor(methodName, codeFunction.IsShared, array, simple, ActionKind.MethodCompound, null, null);
		}

		public MethodDescriptor CreateMethodDescriptor(CodeEvent codeEvent, CodeType container)
		{
			CodeType codeType = codeEvent.Type.CodeType;
			CodeDelegate val = (CodeDelegate)(object)((codeType is CodeDelegate) ? codeType : null);
			Type.Simple simple = CreateType(val.Type);
			Parameter[] array = CreateParameters(val.Parameters, true);
			string name = EnvDTEUtils.ShortenTypeName(container.FullName, importedNamespaces, containerProject);
			InstantiatedName prefix = new InstantiatedName(Location.None, null, name, null);
			if (simple == null || !simple.IsVoid || array == null)
			{
				return null;
			}
			return new MethodDescriptor(new InstantiatedName(Location.None, prefix, codeEvent.Name, null), codeEvent.IsShared, array, simple, ActionKind.Event, null, null);
		}

		public Parameter[] CreateParameters(CodeElements parametersCol, bool isForEvent)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected O, but got Unknown
			Parameter[] array = new Parameter[parametersCol.Count];
			int num = 0;
			foreach (CodeElement item in parametersCol)
			{
				CodeElement val = item;
				array[num] = CreateParameter((CodeParameter2)val, isForEvent);
				if (array[num] == null)
				{
					return null;
				}
				num++;
			}
			return array;
		}

		public Parameter CreateParameter(CodeParameter2 param, bool isForEvent)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected I4, but got Unknown
			Location none = Location.None;
			vsCMParameterKind parameterKind = param.ParameterKind;
			ParameterKind kind;
			switch ((int)parameterKind)
			{
			case 2:
				if (isForEvent)
				{
					return null;
				}
				kind = ParameterKind.Ref;
				break;
			case 4:
				if (isForEvent)
				{
					return null;
				}
				kind = ParameterKind.Out;
				break;
			case 0:
			case 1:
				kind = ParameterKind.In;
				break;
			default:
				return null;
			}
			Type type = CreateType(param.Type);
			if (type != null)
			{
				return new Parameter(none, kind, type, param.Name);
			}
			return null;
		}

		public Type.Simple CreateType(CodeTypeRef typeRef)
		{
			switch (typeRef.TypeKind)
			{
				case vsCMTypeRef.vsCMTypeRefArray:
					return (Type.Simple)null;
				case vsCMTypeRef.vsCMTypeRefVoid:
					return this.CreateType("void");
				default:
					return this.CreateType(typeRef.AsString);
			}
		}

		public Type.Simple CreateType(string typeString)
		{
			string name = EnvDTEUtils.ShortenTypeName(typeString, importedNamespaces, containerProject);
			return new Type.Simple(new InstantiatedName(Location.None, null, name, null));
		}

		public static bool IsFunctionValid(CodeFunction codeFunction)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)codeFunction.Access == 1 && !codeFunction.Type.AsString.Contains("[]") && !((IEnumerable)codeFunction.Attributes).Cast<CodeAttribute2>().Any((CodeAttribute2 attr) => attr.FullName == typeof(RuleAttribute).FullName))
			{
				return !((IEnumerable)codeFunction.Parameters).Cast<CodeParameter2>().Any((CodeParameter2 param) => param.Type.AsString.Contains("[]"));
			}
			return false;
		}

		public static bool IsEventValid(CodeEvent codeEvent)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Invalid comparison between Unknown and I4
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Invalid comparison between Unknown and I4
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Invalid comparison between Unknown and I4
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Invalid comparison between Unknown and I4
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Invalid comparison between Unknown and I4
			if ((int)codeEvent.Access != 1)
			{
				return false;
			}
			CodeType codeType = codeEvent.Type.CodeType;
			CodeDelegate val = (CodeDelegate)(object)((codeType is CodeDelegate) ? codeType : null);
			if ((int)val.Type.TypeKind != 3)
			{
				return false;
			}
			foreach (CodeParameter2 parameter in val.Parameters)
			{
				CodeParameter2 val2 = parameter;
				if ((int)val2.Type.TypeKind == 2 || (int)val2.ParameterKind == 4 || (int)val2.ParameterKind == 2 || (int)val2.ParameterKind == 16 || val2.Type.AsString.Contains("[]"))
				{
					return false;
				}
			}
			return true;
		}
	}
}
