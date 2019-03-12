// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.SyncObjectLock
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Threading;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  internal class SyncObjectLock : IDisposable
  {
    private object syncObject;

    public SyncObjectLock(object syncObject)
    {
      if (syncObject == null)
        throw new ArgumentNullException(nameof (syncObject));
      this.syncObject = syncObject;
      bool lockTaken = false;
      Monitor.Enter(this.syncObject, ref lockTaken);
    }

    ~SyncObjectLock()
    {
      throw new InvalidOperationException("You forgot to call Microsoft.Internal.VisualStudio.PlatformUI.SyncObjectLock.Dispose! This is a shell bug.");
    }

    public void Dispose()
    {
      if (this.syncObject == null)
        return;
      Monitor.Exit(this.syncObject);
      this.syncObject = (object) null;
      GC.SuppressFinalize((object) this);
    }
  }
}
