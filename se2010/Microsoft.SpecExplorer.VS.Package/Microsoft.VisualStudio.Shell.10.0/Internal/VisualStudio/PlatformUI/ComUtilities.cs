﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.ComUtilities
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public static class ComUtilities
  {
    public static IEnumerable<VsUIPropertyDescriptor> EnumerableFrom(
      IVsUIEnumDataSourceProperties enumerator)
    {
      return (IEnumerable<VsUIPropertyDescriptor>) new EnumerablePropertiesCollection(enumerator);
    }

    public static IEnumerable<string> EnumerableFrom(IVsUIEnumDataSourceVerbs enumerator)
    {
      return (IEnumerable<string>) new EnumerableVerbsCollection(enumerator);
    }
  }
}