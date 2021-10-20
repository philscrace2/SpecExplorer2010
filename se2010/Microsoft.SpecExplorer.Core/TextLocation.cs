using System;

namespace Microsoft.SpecExplorer
{
	[Serializable]
	public struct TextLocation
	{
		public string FileName { get; private set; }

		public short FirstLine { get; private set; }

		public short LastLine { get; private set; }

		public short FirstColumn { get; private set; }

		public short LastColumn { get; private set; }

		public TextLocation(string fileName, short line)
		{
			this = default(TextLocation);
			FileName = fileName;
			short firstLine = (LastLine = line);
			FirstLine = firstLine;
			short firstColumn = (LastColumn = 0);
			FirstColumn = firstColumn;
		}

		public TextLocation(string fileName, short line, short column)
		{
			this = default(TextLocation);
			FileName = fileName;
			short firstLine = (LastLine = line);
			FirstLine = firstLine;
			short firstColumn = (LastColumn = column);
			FirstColumn = firstColumn;
		}

		public TextLocation(string fileName, short firstLine, short firstColumn, short lastLine, short lastColumn)
		{
			this = default(TextLocation);
			FileName = fileName;
			FirstLine = firstLine;
			FirstColumn = firstColumn;
			LastLine = lastLine;
			LastColumn = lastColumn;
		}

		public override string ToString()
		{
			string text = ((FirstColumn != 0) ? (FirstLine + "." + FirstColumn) : FirstLine.ToString());
			string text2 = ((LastColumn != 0) ? (LastLine + "." + LastColumn) : LastLine.ToString());
			return string.Format("{0}({1})", FileName, (text == text2) ? text : (text + "-" + text2));
		}
	}
}
