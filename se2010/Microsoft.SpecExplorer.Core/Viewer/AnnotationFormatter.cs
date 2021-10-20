using System;
using System.Text;

namespace Microsoft.SpecExplorer.Viewer
{
	internal class AnnotationFormatter
	{
		internal static readonly string TabPadding = "  ";

		private static readonly string LineBreakIndent = string.Empty;

		internal static readonly string SoftLineBreakIndent = LineBreakIndent + TabPadding;

		internal static readonly int DefaultLineWidth = 15;

		public static string Format(string text, int maxWidth)
		{
			maxWidth = ((maxWidth > DefaultLineWidth) ? maxWidth : DefaultLineWidth);
			text = text.Replace("\r", string.Empty);
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = text.Split(new char[1] { '\n' }, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = LineBreakIndent;
				if (i > 0)
				{
					stringBuilder.AppendLine();
				}
				string text3 = array[i];
				if (!text3.Contains("\t") && text3.Length <= maxWidth)
				{
					stringBuilder.Append(text3);
					continue;
				}
				if (text3.Contains("\t"))
				{
					do
					{
						int num = text3.IndexOf('\t');
						while (num >= 0 && num < maxWidth)
						{
							string text4 = ((num < maxWidth - 1) ? TabPadding : " ");
							text3 = text3.Substring(0, num) + text4 + text3.Substring(num + 1);
							num = text3.IndexOf('\t');
						}
						if (text3.Length > maxWidth)
						{
							num = text3.LastIndexOf(' ', maxWidth - 1, maxWidth - text2.Length);
							text2 = SoftLineBreakIndent;
							int num2 = ((num > 0) ? (num + 1) : maxWidth);
							stringBuilder.AppendLine(text3.Substring(0, num2));
							text3 = SoftLineBreakIndent + text3.Substring(num2);
							continue;
						}
						stringBuilder.Append(text3);
						break;
					}
					while (text3.Length > 0);
					continue;
				}
				while (text3.Length > 0)
				{
					if (text3.Length > maxWidth)
					{
						int num3 = text3.LastIndexOf(' ', maxWidth - 1, maxWidth - text2.Length);
						text2 = SoftLineBreakIndent;
						int num4 = ((num3 > 0) ? (num3 + 1) : maxWidth);
						stringBuilder.AppendLine(text3.Substring(0, num4));
						text3 = SoftLineBreakIndent + text3.Substring(num4);
						continue;
					}
					stringBuilder.Append(text3);
					break;
				}
			}
			return stringBuilder.ToString();
		}
	}
}
