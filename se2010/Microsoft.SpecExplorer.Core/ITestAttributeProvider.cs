// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ITestAttributeProvider
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

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

    IEnumerable<CodeAttributeDeclaration> CreateTestCaseAttributes(
      string testCaseID);

    IEnumerable<CodeAttributeDeclaration> CreateDynamicTraversalTestCaseAttributes();
  }
}
