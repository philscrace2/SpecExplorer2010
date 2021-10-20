using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	internal class PtfTestAttributeProvider : VsTestAttributeProvider
	{
		internal PtfTestAttributeProvider(TransitionSystem transitionSystem)
			: base(transitionSystem)
		{
		}

		public override IEnumerable<CodeAttributeDeclaration> CreateTestCaseInitializeAttributes()
		{
			return Enumerable.Empty<CodeAttributeDeclaration>();
		}

		public override IEnumerable<CodeAttributeDeclaration> CreateTestCaseCleanupAttributes()
		{
			return Enumerable.Empty<CodeAttributeDeclaration>();
		}

		public override IEnumerable<CodeAttributeDeclaration> CreateTestClassInitializeAttributes()
		{
			string classInitializeAttribute = variableResolver.Resolve("classinitializeattribute", transitionSystem.GetSwitch("classinitializeattribute"));
			if (classInitializeAttribute.IsNoneOrEmptyValue())
			{
				yield break;
			}
			foreach (CodeAttributeDeclaration item in classInitializeAttribute.SplitAttributes())
			{
				yield return item;
			}
		}

		public override IEnumerable<CodeAttributeDeclaration> CreateTestClassCleanupAttributes()
		{
			string classCleanupAttribute = variableResolver.Resolve("classcleanupattribute", transitionSystem.GetSwitch("classcleanupattribute"));
			if (classCleanupAttribute.IsNoneOrEmptyValue())
			{
				yield break;
			}
			foreach (CodeAttributeDeclaration item in classCleanupAttribute.SplitAttributes())
			{
				yield return item;
			}
		}
	}
}
