using System;
using Microsoft.ActionMachines.Cord;

namespace Microsoft.SpecExplorer.VS
{
	internal class ActionDeclaration
	{
		internal MethodDescriptor Method { get; private set; }

		internal string Name
		{
			get
			{
				if (Method.MethodName == null)
				{
					return Method.ResultType.Flatten();
				}
				return Method.MethodName.Name;
			}
		}

		internal string FullName
		{
			get
			{
				return Method.ToString();
			}
		}

		internal ActionDeclaration(MethodDescriptor method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			Method = method;
		}

		public override bool Equals(object obj)
		{
			ActionDeclaration actionDeclaration = obj as ActionDeclaration;
			if (actionDeclaration != null)
			{
				return Name.Equals(actionDeclaration.Name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}
}
