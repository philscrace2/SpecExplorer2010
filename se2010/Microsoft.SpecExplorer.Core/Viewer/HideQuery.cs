using System;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.Viewer
{
	[Serializable]
	public class HideQuery
	{
		[XmlElement("Query")]
		public Query Query { get; set; }

		public HideQuery()
		{
			Query = new Query();
		}
	}
}
