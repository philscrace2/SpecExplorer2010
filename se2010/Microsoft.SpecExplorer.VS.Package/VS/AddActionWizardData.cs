// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.AddActionWizardData
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System.Collections.Generic;

namespace Microsoft.SpecExplorer.VS
{
  internal class AddActionWizardData
  {
    public string ConfigName { get; set; }

    public string ScriptName { get; set; }

    public string ProjectName { get; set; }

    public bool IsConfigToBeCreated { get; set; }

    public bool IsScriptToBeCreated { get; set; }

    public IEnumerable<CodeElementAndContainerPair> CodeElementsToBeImported { get; set; }
  }
}
