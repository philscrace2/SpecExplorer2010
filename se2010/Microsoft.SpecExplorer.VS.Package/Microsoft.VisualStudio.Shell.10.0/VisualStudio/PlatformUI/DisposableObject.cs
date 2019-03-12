// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.DisposableObject
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.PlatformUI
{
  [ComVisible(true)]
  public class DisposableObject : IDisposable
  {
    private EventHandler _disposing;

    ~DisposableObject()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public bool IsDisposed { get; private set; }

    public event EventHandler Disposing
    {
      add
      {
        this.ThrowIfDisposed();
        this._disposing += value;
      }
      remove
      {
        this.ThrowIfDisposed();
        this._disposing -= value;
      }
    }

    protected void ThrowIfDisposed()
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    protected void Dispose(bool disposing)
    {
      if (!this.IsDisposed)
      {
        this._disposing.RaiseEvent((object) this);
        this._disposing = (EventHandler) null;
        if (disposing)
          this.DisposeManagedResources();
        this.DisposeNativeResources();
      }
      this.IsDisposed = true;
    }

    protected virtual void DisposeManagedResources()
    {
    }

    protected virtual void DisposeNativeResources()
    {
    }
  }
}
