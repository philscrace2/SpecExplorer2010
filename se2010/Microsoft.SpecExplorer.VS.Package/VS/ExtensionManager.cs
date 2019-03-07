// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ExtensionManager
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.SpecExplorer.VS.Common;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer.VS
{
  internal class ExtensionManager : IExtensionManager
  {
    private Dictionary<string, IExtensionService> extensionServices = new Dictionary<string, IExtensionService>();

    internal ExtensionManager()
    {
    }

    public void RegisterExtension(string key, IExtensionService service)
    {
      this.extensionServices[key] = service;
    }

    public bool TyrGetExtensionData(string key, object inputValue, out object outputValue)
    {
      IExtensionService iextensionService;
      if (this.extensionServices.TryGetValue(key, out iextensionService))
        return iextensionService.TryGetExtensionData(inputValue, ref outputValue);
      outputValue = (object) null;
      return false;
    }
  }
}
