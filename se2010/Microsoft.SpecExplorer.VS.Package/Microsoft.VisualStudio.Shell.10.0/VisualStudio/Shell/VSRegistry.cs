// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.VSRegistry
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public static class VSRegistry
  {
    private static ServiceProvider GlobalProvider
    {
      get
      {
        return ServiceProvider.GlobalProvider;
      }
    }

    public static RegistryKey RegistryRoot(__VsLocalRegistryType registryType)
    {
      return VSRegistry.RegistryRoot((IServiceProvider) VSRegistry.GlobalProvider, registryType, false);
    }

    public static RegistryKey RegistryRoot(
      __VsLocalRegistryType registryType,
      bool writable)
    {
      return VSRegistry.RegistryRoot((IServiceProvider) VSRegistry.GlobalProvider, registryType, writable);
    }

    public static RegistryKey RegistryRoot(
      IServiceProvider provider,
      __VsLocalRegistryType registryType,
      bool writable)
    {
      if (provider == null)
        throw new ArgumentNullException(nameof (provider));
      if (__VsLocalRegistryType.RegType_UserSettings != registryType && __VsLocalRegistryType.RegType_Configuration != registryType)
        throw new NotSupportedException();
      ILocalRegistry4 service1 = provider.GetService(typeof (SLocalRegistry)) as ILocalRegistry4;
      uint pdwRegRootHandle;
      string pbstrRoot1;
      if (service1 != null && ErrorHandler.Succeeded(service1.GetLocalRegistryRootEx((uint) registryType, out pdwRegRootHandle, out pbstrRoot1)))
      {
        __VsLocalRegistryRootHandle registryRootHandle = (__VsLocalRegistryRootHandle) pdwRegRootHandle;
        if (!string.IsNullOrEmpty(pbstrRoot1) && registryRootHandle != __VsLocalRegistryRootHandle.RegHandle_Invalid)
          return (__VsLocalRegistryRootHandle.RegHandle_LocalMachine == registryRootHandle ? Registry.LocalMachine : Registry.CurrentUser).OpenSubKey(pbstrRoot1, writable);
      }
      ILocalRegistry2 service2 = provider.GetService(typeof (SLocalRegistry)) as ILocalRegistry2;
      if (service2 == null)
        return (RegistryKey) null;
      string pbstrRoot2;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service2.GetLocalRegistryRoot(out pbstrRoot2));
      if (string.IsNullOrEmpty(pbstrRoot2))
        return (RegistryKey) null;
      return (__VsLocalRegistryType.RegType_Configuration == registryType ? Registry.LocalMachine : Registry.CurrentUser).OpenSubKey(pbstrRoot2, writable);
    }
  }
}
