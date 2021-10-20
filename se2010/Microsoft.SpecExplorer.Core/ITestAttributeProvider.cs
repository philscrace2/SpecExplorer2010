using System.CodeDom;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer
{
	public interface ITestAttributeProvider
	{
		IEnumerable<CodeAttributeDeclaration> CreateTestAssemblyAttributes();

		IEnumerable<CodeAttributeDeclaration> CreateTestClassAttributes();

		IEnumerable<CodeAttributeDeclaration> CreateTestClassInitializeAttributes();

		IEnumerable<CodeAttributeDeclaration> CreateTestClassCleanupAttributes();

		IEnumerable<CodeAttributeDeclaration> CreateTestCaseInitializeAttributes();

		IEnumerable<CodeAttributeDeclaration> CreateTestCaseCleanupAttributes();

		IEnumerable<CodeAttributeDeclaration> CreateTestCaseAttributes(string testCaseID);

		IEnumerable<CodeAttributeDeclaration> CreateDynamicTraversalTestCaseAttributes();
	}
}
