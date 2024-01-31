using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	internal class VsTestAttributeProvider : ITestAttributeProvider
	{
		protected TransitionSystem transitionSystem;

		protected VariableResolver variableResolver;

		internal VsTestAttributeProvider(TransitionSystem transitionSystem)
		{
			this.transitionSystem = transitionSystem;
			variableResolver = new VariableResolver(transitionSystem);
		}

		public virtual IEnumerable<CodeAttributeDeclaration> CreateTestAssemblyAttributes()
		{
			string testAssemblyAttribute = variableResolver.Resolve("testassemblyattribute");
			if (testAssemblyAttribute.IsNoneOrEmptyValue())
			{
				yield break;
			}
			foreach (CodeAttributeDeclaration item in testAssemblyAttribute.SplitAttributes())
			{
				yield return item;
			}
		}

		public virtual IEnumerable<CodeAttributeDeclaration> CreateTestClassAttributes()
		{
			yield return new CodeAttributeDeclaration(typeof(GeneratedCodeAttribute).FullName, new CodeAttributeArgument(null, new CodePrimitiveExpression(Resource.SpecExplorer)), new CodeAttributeArgument(null, new CodePrimitiveExpression("3.5.3146.0")));
			string testClassAttribute = variableResolver.Resolve("testclassattribute");
			if (testClassAttribute.IsNoneOrEmptyValue())
			{
				yield break;
			}
			foreach (CodeAttributeDeclaration item in testClassAttribute.SplitAttributes())
			{
				yield return item;
			}
		}

		public virtual IEnumerable<CodeAttributeDeclaration> CreateTestCaseInitializeAttributes()
		{
			string testInitializeAttribute = variableResolver.Resolve("testinitializeattribute");
			if (testInitializeAttribute.IsNoneOrEmptyValue())
			{
				yield break;
			}
			foreach (CodeAttributeDeclaration item in testInitializeAttribute.SplitAttributes())
			{
				yield return item;
			}
		}

		public virtual IEnumerable<CodeAttributeDeclaration> CreateTestCaseCleanupAttributes()
		{
			string testCleanupAttribute = variableResolver.Resolve("testcleanupattribute");
			if (testCleanupAttribute.IsNoneOrEmptyValue())
			{
				yield break;
			}
			foreach (CodeAttributeDeclaration item in testCleanupAttribute.SplitAttributes())
			{
				yield return item;
			}
		}

		public virtual IEnumerable<CodeAttributeDeclaration> CreateTestClassInitializeAttributes()
		{
			return Enumerable.Empty<CodeAttributeDeclaration>();
		}

		public virtual IEnumerable<CodeAttributeDeclaration> CreateTestClassCleanupAttributes()
		{
			return Enumerable.Empty<CodeAttributeDeclaration>();
		}

		public virtual IEnumerable<CodeAttributeDeclaration> CreateTestCaseAttributes(string testCaseID)
		{
			string testMethodAttribute2 = transitionSystem.GetSwitch("testmethodattribute");
			if (testMethodAttribute2.IsNoneOrEmptyValue())
			{
				yield break;
			}
			testMethodAttribute2 = variableResolver.Resolve("testmethodattribute", testCaseID);
			foreach (CodeAttributeDeclaration item in testMethodAttribute2.SplitAttributes())
			{
				yield return item;
			}
		}

		public virtual IEnumerable<CodeAttributeDeclaration> CreateDynamicTraversalTestCaseAttributes()
		{
			string testMethodAttribute2 = transitionSystem.GetSwitch("testmethodattribute");
			if (testMethodAttribute2.IsNoneOrEmptyValue())
			{
				yield break;
			}
			testMethodAttribute2 = variableResolver.Resolve("testmethodattribute");
			foreach (CodeAttributeDeclaration item in testMethodAttribute2.SplitAttributes())
			{
				yield return item;
			}
		}
	}
}
