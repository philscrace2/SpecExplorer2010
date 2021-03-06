﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.VerbDescription
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  public class VerbDescription : IVerbDescription
  {
    private string name;

    public VerbDescription(string name)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_EmptyString, (object) nameof (name)));
      this.name = name;
    }

    public string Name
    {
      get
      {
        return this.name;
      }
    }
  }
}
