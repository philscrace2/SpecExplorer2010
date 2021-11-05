using System;
using EnvDTE;

namespace Microsoft.SpecExplorer.VS
{
	public class CodeElementAndContainerPair
	{
		public CodeType Container { get; private set; }

		public CodeElement Element { get; private set; }

		public CodeElementAndContainerPair(CodeElement element, CodeType container)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			Element = element;
			Container = container;
		}
	}
}
