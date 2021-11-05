using Microsoft.Xrt;

namespace Microsoft.SpecExplorer.VS
{
	public class ProcedureType
	{
		public string FullName { get; private set; }

		public string ShortName { get; private set; }

		public bool IsAdapter { get; private set; }

		public ProcedureType(string fullName, string shortName, bool isAdapter)
		{
			FullName = fullName;
			ShortName = shortName;
			IsAdapter = isAdapter;
		}

		public ProcedureType(string fullName, string shortName)
			: this(fullName, shortName, false)
		{
			FullName = fullName;
			ShortName = shortName;
		}

		public ProcedureType(IType type)
			: this((type.IsAddressType || type.IsArrayType) ? type.ElementType.FullName : type.FullName, type.ShortName, type.IsAdapter)
		{
		}

		public override int GetHashCode()
		{
			Hash32Builder hash32Builder = default(Hash32Builder);
			hash32Builder.Add(typeof(ProcedureType).GetHashCode());
			hash32Builder.Add(FullName.GetHashCode());
			hash32Builder.Add(ShortName.GetHashCode());
			hash32Builder.Add(IsAdapter.GetHashCode());
			return hash32Builder.Result;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ProcedureType))
			{
				return false;
			}
			ProcedureType procedureType = (ProcedureType)obj;
			if (FullName == procedureType.FullName && ShortName == procedureType.ShortName)
			{
				return IsAdapter == procedureType.IsAdapter;
			}
			return false;
		}
	}
}
