using System.IO;
using System.Text;

namespace Microsoft.SpecExplorer
{
	internal class RedirectedTextWriter : TextWriter
	{
		private StringBuilder lineBuffer = new StringBuilder();

		private EventAdapter eventAdapter;

		public override Encoding Encoding
		{
			get
			{
				return Encoding.Default;
			}
		}

		internal RedirectedTextWriter(EventAdapter eventAdapter)
		{
			this.eventAdapter = eventAdapter;
		}

		public override void Write(string s)
		{
			Add(s);
		}

		public override void Write(char c)
		{
			Add(new string(c, 1));
		}

		public override void WriteLine()
		{
			Add(NewLine);
		}

		public override void WriteLine(string s)
		{
			Add(s);
			Add(NewLine);
		}

		private void Add(string s)
		{
			if (s == null)
			{
				return;
			}
			int num = 0;
			while (num < s.Length)
			{
				int num2 = s.IndexOf(NewLine, num);
				if (num2 >= 0)
				{
					lineBuffer.Append(s.Substring(num, num2 - num));
					eventAdapter.Log(lineBuffer.ToString());
					lineBuffer.Length = 0;
					num = num2 + NewLine.Length;
					continue;
				}
				lineBuffer.Append(s.Substring(num));
				break;
			}
		}
	}
}
