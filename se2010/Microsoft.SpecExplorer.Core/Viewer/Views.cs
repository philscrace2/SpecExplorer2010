using System;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.Viewer
{
	[Serializable]
	public class Views
	{
		[XmlAttribute("Version")]
		public string Version { get; set; }

		[XmlArray("ViewList")]
		public ViewDefinition[] ViewList { get; set; }

		public Views()
		{
			Version = "1.0";
			ViewList = null;
		}
	}
}
