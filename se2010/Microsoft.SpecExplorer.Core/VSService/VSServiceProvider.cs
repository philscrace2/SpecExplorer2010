// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VSService.VSServiceProvider
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.Xrt;

namespace Microsoft.SpecExplorer.VSService
{
  public class VSServiceProvider : ComponentBase, IVSServiceProvider
  {
    private ExplorerMediator explorerMediator;

    public VSServiceProvider(ExplorerMediator explorerMediator) => this.explorerMediator = explorerMediator;

    public bool TryGetExtensionData(string key, object inputValue, out object outputValue) => this.explorerMediator.TryGetExtensionData(key, inputValue, out outputValue);
  }
}
