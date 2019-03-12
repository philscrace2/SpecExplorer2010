// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.IEventHandler
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio
{
  [Guid("9BDA66AE-CA28-4e22-AA27-8A7218A0E3FA")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [CLSCompliant(false)]
  [ComImport]
  public interface IEventHandler
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int AddHandler(string bstrEventName);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int RemoveHandler(string bstrEventName);

    IVsEnumBSTR GetHandledEvents();

    bool HandlesEvent(string bstrEventName);
  }
}
