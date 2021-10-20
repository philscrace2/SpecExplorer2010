using System;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.Viewer
{
	[Serializable]
	public class SelectQuery
	{
		[XmlElement("Query")]
		public Query Query { get; set; }

		public SelectQuery()
		{
			Query = new Query();
		}
	}
}
