using System;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	[Serializable]
	public class CodeBlockImpl : ICodeBlock
	{
		[XmlElement("FormattedText")]
		public string FormattedText { get; set; }

		[XmlElement("RawText")]
		public string RawText { get; set; }

		[XmlAttribute("Language")]
		public string Language { get; set; }
	}
}
