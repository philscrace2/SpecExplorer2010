using System;
using System.Runtime.Serialization;

namespace Microsoft.SpecExplorer.Viewer
{
	[Serializable]
	public class QueryException : Exception
	{
		public QueryException()
		{
		}

		protected QueryException(SerializationInfo info, StreamingContext sctx)
			: base(info, sctx)
		{
		}

		public QueryException(string msg)
			: base(msg)
		{
		}

		public QueryException(string msg, Exception e)
			: base(msg, e)
		{
		}
	}
}
