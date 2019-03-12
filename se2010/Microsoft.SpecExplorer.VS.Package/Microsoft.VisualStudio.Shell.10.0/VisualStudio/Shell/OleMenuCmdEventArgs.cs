// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.OleMenuCmdEventArgs
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public class OleMenuCmdEventArgs : EventArgs
  {
    private object inParam;
    private IntPtr outParam;
    private OLECMDEXECOPT execOptions;

    public OleMenuCmdEventArgs(object inParam, IntPtr outParam)
      : this(inParam, outParam, OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT)
    {
    }

    public OleMenuCmdEventArgs(object inParam, IntPtr outParam, OLECMDEXECOPT options)
    {
      this.execOptions = options;
      this.inParam = inParam;
      this.outParam = outParam;
    }

    public object InValue
    {
      get
      {
        return this.inParam;
      }
    }

    public OLECMDEXECOPT Options
    {
      get
      {
        return this.execOptions;
      }
    }

    public IntPtr OutValue
    {
      get
      {
        return this.outParam;
      }
    }
  }
}
