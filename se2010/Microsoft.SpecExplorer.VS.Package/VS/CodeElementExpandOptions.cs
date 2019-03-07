// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.CodeElementExpandOptions
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System;

namespace Microsoft.SpecExplorer.VS
{
  [Flags]
  public enum CodeElementExpandOptions
  {
    None = 0,
    ExpandToNamespaces = 1,
    ExpandToInterfaces = 2,
    ExpandToClasses = 4,
    ExpandToFunctions = 8,
    ExpandToEvents = 16, // 0x00000010
    ExpandAll = ExpandToEvents | ExpandToFunctions | ExpandToClasses | ExpandToInterfaces | ExpandToNamespaces, // 0x0000001F
  }
}
