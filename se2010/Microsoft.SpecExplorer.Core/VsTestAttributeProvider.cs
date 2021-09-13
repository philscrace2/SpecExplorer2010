// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VsTestAttributeProvider
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.Properties;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer
{
  internal class VsTestAttributeProvider : ITestAttributeProvider
  {
    protected TransitionSystem transitionSystem;
    protected VariableResolver variableResolver;

    internal VsTestAttributeProvider(TransitionSystem transitionSystem)
    {
      this.transitionSystem = transitionSystem;
      this.variableResolver = new VariableResolver(transitionSystem);
    }

    public virtual IEnumerable<CodeAttributeDeclaration> CreateTestAssemblyAttributes()
    {
      string testAssemblyAttribute = this.variableResolver.Resolve("testassemblyattribute");
      if (!testAssemblyAttribute.IsNoneOrEmptyValue())
      {
        foreach (CodeAttributeDeclaration splitAttribute in testAssemblyAttribute.SplitAttributes())
          yield return splitAttribute;
      }
    }

    public virtual IEnumerable<CodeAttributeDeclaration> CreateTestClassAttributes()
    {
      yield return new CodeAttributeDeclaration(typeof (GeneratedCodeAttribute).FullName, new CodeAttributeArgument[2]
      {
        new CodeAttributeArgument((string) null, (CodeExpression) new CodePrimitiveExpression((object) Resources.SpecExplorer)),
        new CodeAttributeArgument((string) null, (CodeExpression) new CodePrimitiveExpression((object) "3.5.3146.0"))
      });
      string testClassAttribute = this.variableResolver.Resolve("testclassattribute");
      if (!testClassAttribute.IsNoneOrEmptyValue())
      {
        foreach (CodeAttributeDeclaration splitAttribute in testClassAttribute.SplitAttributes())
          yield return splitAttribute;
      }
    }

    public virtual IEnumerable<CodeAttributeDeclaration> CreateTestCaseInitializeAttributes()
    {
      string testInitializeAttribute = this.variableResolver.Resolve("testinitializeattribute");
      if (!testInitializeAttribute.IsNoneOrEmptyValue())
      {
        foreach (CodeAttributeDeclaration splitAttribute in testInitializeAttribute.SplitAttributes())
          yield return splitAttribute;
      }
    }

    public virtual IEnumerable<CodeAttributeDeclaration> CreateTestCaseCleanupAttributes()
    {
      string testCleanupAttribute = this.variableResolver.Resolve("testcleanupattribute");
      if (!testCleanupAttribute.IsNoneOrEmptyValue())
      {
        foreach (CodeAttributeDeclaration splitAttribute in testCleanupAttribute.SplitAttributes())
          yield return splitAttribute;
      }
    }

    public virtual IEnumerable<CodeAttributeDeclaration> CreateTestClassInitializeAttributes() => Enumerable.Empty<CodeAttributeDeclaration>();

    public virtual IEnumerable<CodeAttributeDeclaration> CreateTestClassCleanupAttributes() => Enumerable.Empty<CodeAttributeDeclaration>();
    
    public virtual IEnumerable<CodeAttributeDeclaration> CreateTestCaseAttributes(
      string testCaseID)
    {
      string testMethodAttribute = this.transitionSystem.GetSwitch("testmethodattribute");
      if (!testMethodAttribute.IsNoneOrEmptyValue())
      {
        testMethodAttribute = this.variableResolver.Resolve("testmethodattribute", testCaseID);
        foreach (CodeAttributeDeclaration splitAttribute in testMethodAttribute.SplitAttributes())
          yield return splitAttribute;
      }
    }

    public virtual IEnumerable<CodeAttributeDeclaration> CreateDynamicTraversalTestCaseAttributes()
    {
      string testMethodAttribute = this.transitionSystem.GetSwitch("testmethodattribute");
      if (!testMethodAttribute.IsNoneOrEmptyValue())
      {
        testMethodAttribute = this.variableResolver.Resolve("testmethodattribute");
        foreach (CodeAttributeDeclaration splitAttribute in testMethodAttribute.SplitAttributes())
          yield return splitAttribute;
      }
    }
  }
}
