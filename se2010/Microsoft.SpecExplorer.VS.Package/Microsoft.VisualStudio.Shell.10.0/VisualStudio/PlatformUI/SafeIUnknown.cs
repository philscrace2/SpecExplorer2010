// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SafeIUnknown
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.PlatformUI
{
  [DebuggerDisplay("{Value}")]
  public class SafeIUnknown : SafeHandle
  {
    public SafeIUnknown()
      : this(IntPtr.Zero)
    {
    }

    public SafeIUnknown(SafeIUnknown other)
      : this(other.handle)
    {
      if (!(this.handle != IntPtr.Zero))
        return;
      Marshal.AddRef(this.handle);
    }

    public SafeIUnknown(IntPtr punk)
      : base(IntPtr.Zero, true)
    {
      this.handle = punk;
    }

    public static SafeIUnknown FromObject(object o)
    {
      if (o is SafeIUnknown)
        return new SafeIUnknown((SafeIUnknown) o);
      return new SafeIUnknown(Marshal.GetIUnknownForObject(o));
    }

    public object ToObject()
    {
      if (this.IsInvalid)
        return (object) null;
      return Marshal.GetObjectForIUnknown(this.handle);
    }

    public T ToObject<T>() where T : class
    {
      return this.ToObject() as T;
    }

    public IntPtr Value
    {
      get
      {
        return this.handle;
      }
    }

    protected override bool ReleaseHandle()
    {
      Marshal.Release(this.handle);
      this.handle = IntPtr.Zero;
      return true;
    }

    public override bool IsInvalid
    {
      get
      {
        return this.handle == IntPtr.Zero;
      }
    }
  }
}
