using System;
using System.Runtime.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	[Serializable]
	public class GuidanceException : Exception
	{
		public GuidanceException()
		{
		}

		public GuidanceException(string message)
			: base(message)
		{
		}

		public GuidanceException(string message, Exception exc)
			: base(message, exc)
		{
		}

		protected GuidanceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
