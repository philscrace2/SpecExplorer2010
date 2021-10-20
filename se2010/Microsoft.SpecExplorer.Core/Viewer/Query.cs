using System;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.Viewer
{
	[Serializable]
	public class Query
	{
		[XmlIgnore]
		public QueryType Type { get; set; }

		[XmlElement("Param")]
		public string Param { get; set; }

		public Query()
		{
			Param = "";
		}
	}
}
