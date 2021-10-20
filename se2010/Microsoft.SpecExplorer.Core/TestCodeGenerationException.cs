using System;
using System.Runtime.Serialization;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	public sealed class TestCodeGenerationException : Exception
	{
		public TestCodeGenerationException()
		{
		}

		public TestCodeGenerationException(string message)
			: base(message)
		{
		}

		public TestCodeGenerationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		private TestCodeGenerationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
