// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.OleComponentSupport.ActivationChangeEventArgs
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;

namespace Microsoft.VisualStudio.PlatformUI.OleComponentSupport
{
  [CLSCompliant(false)]
  public class ActivationChangeEventArgs : EventArgs
  {
    public ActivationChangeEventArgs(
      IOleComponent activatedComponent,
      bool calleeIsActivating,
      OLECRINFO? activatingComponentsInfo,
      bool hostIsActivating,
      OLECHOSTINFO? hostInfo)
    {
      this.ActivatedComponent = activatedComponent;
      this.CalleeIsActivating = calleeIsActivating;
      this.ActivatingComponentsInfo = activatingComponentsInfo;
      this.HostIsActivating = hostIsActivating;
      this.HostInfo = hostInfo;
    }

    public IOleComponent ActivatedComponent { get; private set; }

    public bool CalleeIsActivating { get; private set; }

    public OLECRINFO? ActivatingComponentsInfo { get; private set; }

    public bool HostIsActivating { get; private set; }

    public OLECHOSTINFO? HostInfo { get; private set; }
  }
}
