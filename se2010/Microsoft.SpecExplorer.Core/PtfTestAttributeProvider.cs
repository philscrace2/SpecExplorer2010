// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.PtfTestAttributeProvider
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer
{
  internal class PtfTestAttributeProvider : VsTestAttributeProvider
  {
    internal PtfTestAttributeProvider(TransitionSystem transitionSystem)
      : base(transitionSystem)
    {
    }

    public override IEnumerable<CodeAttributeDeclaration> CreateTestCaseInitializeAttributes() => Enumerable.Empty<CodeAttributeDeclaration>();

    public override IEnumerable<CodeAttributeDeclaration> CreateTestCaseCleanupAttributes() => Enumerable.Empty<CodeAttributeDeclaration>();

    public override IEnumerable<CodeAttributeDeclaration> CreateTestClassInitializeAttributes()
    {
      string classInitializeAttribute = this.variableResolver.Resolve("classinitializeattribute", this.transitionSystem.GetSwitch("classinitializeattribute"));
      if (!classInitializeAttribute.IsNoneOrEmptyValue())
      {
        foreach (CodeAttributeDeclaration splitAttribute in classInitializeAttribute.SplitAttributes())
          yield return splitAttribute;
      }
    }

    public override IEnumerable<CodeAttributeDeclaration> CreateTestClassCleanupAttributes()
    {
      string classCleanupAttribute = this.variableResolver.Resolve("classcleanupattribute", this.transitionSystem.GetSwitch("classcleanupattribute"));
      if (!classCleanupAttribute.IsNoneOrEmptyValue())
      {
        foreach (CodeAttributeDeclaration splitAttribute in classCleanupAttribute.SplitAttributes())
          yield return splitAttribute;
      }
    }
  }
}
