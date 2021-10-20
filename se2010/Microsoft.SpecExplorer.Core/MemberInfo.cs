using System;
using System.Collections.Generic;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	public class MemberInfo
	{
		public string Name { get; private set; }

		public MemberKind Kind { get; private set; }

		public string TypeName { get; private set; }

		public IList<string> ParameterTypes { get; private set; }

		public MemberInfo(string typeName, MemberKind kind, string name)
			: this(typeName, kind, name, null)
		{
		}

		public MemberInfo(string typeName, MemberKind kind, string name, ILocal[] parameters)
		{
			TypeName = typeName;
			Kind = kind;
			Name = name;
			if (kind == MemberKind.Method)
			{
				if (parameters == null)
				{
					throw new ArgumentNullException("parameters");
				}
				BuildParameters(parameters);
			}
		}

		private void BuildParameters(ILocal[] parameters)
		{
			List<string> list = new List<string>();
			foreach (ILocal local in parameters)
			{
				if (local.Name != "this")
				{
					IType type = local.Type;
					if (type.IsAddressType || type.IsArrayType)
					{
						type = type.ElementType;
					}
					list.Add(type.FullName);
				}
			}
			ParameterTypes = list;
		}
	}
}
