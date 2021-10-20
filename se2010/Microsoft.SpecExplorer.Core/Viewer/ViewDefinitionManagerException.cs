using System;
using System.Runtime.Serialization;

namespace Microsoft.SpecExplorer.Viewer
{
	[Serializable]
	public class ViewDefinitionManagerException : Exception
	{
		public ViewDefinitionManagerException()
		{
		}

		public ViewDefinitionManagerException(string message)
			: base(message)
		{
		}

		public ViewDefinitionManagerException(string message, Exception exc)
			: base(message, exc)
		{
		}

		protected ViewDefinitionManagerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
