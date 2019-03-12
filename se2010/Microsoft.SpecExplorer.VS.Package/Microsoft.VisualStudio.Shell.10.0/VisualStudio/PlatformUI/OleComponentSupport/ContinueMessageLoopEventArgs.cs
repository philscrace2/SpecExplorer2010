// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.OleComponentSupport.ContinueMessageLoopEventArgs
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.PlatformUI.OleComponentSupport
{
  [CLSCompliant(false)]
  public class ContinueMessageLoopEventArgs : EventArgs
  {
    public ContinueMessageLoopEventArgs(uint reasonCode, IntPtr privateData, Microsoft.VisualStudio.OLE.Interop.MSG? message)
    {
      this.ReasonCode = reasonCode;
      this.PrivateData = privateData;
      this.Message = !message.HasValue ? new System.Windows.Interop.MSG?() : new System.Windows.Interop.MSG?(Util.TranslateMessageStruct(message.Value));
      this.ContinuePumping = true;
    }

    public uint ReasonCode { get; private set; }

    public IntPtr PrivateData { get; private set; }

    public bool ContinuePumping { get; set; }

    public System.Windows.Interop.MSG? Message { get; private set; }
  }
}
