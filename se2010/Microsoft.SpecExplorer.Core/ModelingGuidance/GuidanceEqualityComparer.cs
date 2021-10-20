using System.Collections.Generic;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	public class GuidanceEqualityComparer : IEqualityComparer<IGuidance>
	{
		public bool Equals(IGuidance x, IGuidance y)
		{
			return x.Id.Equals(y.Id);
		}

		public int GetHashCode(IGuidance obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}
