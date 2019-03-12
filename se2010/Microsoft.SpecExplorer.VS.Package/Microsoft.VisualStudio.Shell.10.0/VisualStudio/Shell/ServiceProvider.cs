// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ServiceProvider
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.VisualStudio.Shell
{
  [ComVisible(true)]
  [CLSCompliant(false)]
  public sealed class ServiceProvider : System.IServiceProvider, IDisposable, IObjectWithSite
  {
    private static TraceSwitch TRACESERVICE = new TraceSwitch(nameof (TRACESERVICE), "ServiceProvider: Trace service provider requests.");
    private Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider;
    private bool defaultServices;
    private static ServiceProvider globalProvider;
    private static Thread threadOwningGlobalProvider;

    public ServiceProvider(Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp)
      : this(sp, true)
    {
    }

    public ServiceProvider(Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp, bool defaultServices)
    {
      if (sp == null)
        throw new ArgumentNullException(nameof (sp));
      this.serviceProvider = sp;
      this.defaultServices = defaultServices;
    }

    private ServiceProvider()
    {
    }

    public void Dispose()
    {
      if (this.serviceProvider == null)
        return;
      this.serviceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider) null;
    }

    public object GetService(Type serviceType)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      if (this.serviceProvider == null)
        return (object) null;
      return this.GetService(serviceType.GUID, serviceType);
    }

    public object GetService(Guid guid)
    {
      if (this.serviceProvider == null)
        return (object) null;
      return this.GetService(guid, (Type) null);
    }

    private object GetService(Guid guid, Type serviceType)
    {
      object obj = (object) null;
      if (guid.Equals(Guid.Empty))
        return (object) null;
      if (this.defaultServices)
      {
        if (guid.Equals(Microsoft.VisualStudio.NativeMethods.IID_IServiceProvider))
          return (object) this.serviceProvider;
        if (guid.Equals(Microsoft.VisualStudio.NativeMethods.IID_IObjectWithSite))
          return (object) this;
      }
      IntPtr ppvObject = IntPtr.Zero;
      Guid iidIunknown = Microsoft.VisualStudio.NativeMethods.IID_IUnknown;
      if (Microsoft.VisualStudio.NativeMethods.Succeeded(this.serviceProvider.QueryService(ref guid, ref iidIunknown, out ppvObject)))
      {
        if (IntPtr.Zero != ppvObject)
        {
          try
          {
            obj = Marshal.GetObjectForIUnknown(ppvObject);
            goto label_12;
          }
          finally
          {
            Marshal.Release(ppvObject);
          }
        }
      }
      obj = (object) null;
label_12:
      return obj;
    }

    internal TInterfaceType GetService<TInterfaceType>(Type serviceType) where TInterfaceType : class
    {
      TInterfaceType service = this.GetService(serviceType) as TInterfaceType;
      if ((object) service == null)
        throw new InvalidOperationException(string.Format((IFormatProvider) Resources.Culture, Resources.General_MissingService, (object) serviceType.FullName));
      return service;
    }

    void IObjectWithSite.GetSite(ref Guid riid, out IntPtr ppv)
    {
      object service = this.GetService(riid);
      if (service == null)
        Marshal.ThrowExceptionForHR(-2147467262);
      IntPtr iunknownForObject = Marshal.GetIUnknownForObject(service);
      int num = Marshal.QueryInterface(iunknownForObject, ref riid, out ppv);
      Marshal.Release(iunknownForObject);
      if (!Microsoft.VisualStudio.NativeMethods.Failed(num))
        return;
      Marshal.ThrowExceptionForHR(num);
    }

    void IObjectWithSite.SetSite(object pUnkSite)
    {
      if (!(pUnkSite is Microsoft.VisualStudio.OLE.Interop.IServiceProvider))
        return;
      this.serviceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider) pUnkSite;
    }

    private static void SetGlobalProvider(ServiceProvider sp)
    {
      ServiceProvider.globalProvider = sp;
      ServiceProvider.threadOwningGlobalProvider = Thread.CurrentThread;
    }

    internal static bool CheckServiceProviderThreadAccess()
    {
      return ServiceProvider.threadOwningGlobalProvider == Thread.CurrentThread;
    }

    private static bool IsNullOrUnsited(ServiceProvider sp)
    {
      if (sp != null)
        return sp.serviceProvider == null;
      return true;
    }

    public static ServiceProvider GlobalProvider
    {
      get
      {
        if (ServiceProvider.IsNullOrUnsited(ServiceProvider.globalProvider))
        {
          Microsoft.VisualStudio.OLE.Interop.IServiceProvider globalProvider = OleServiceProvider.GlobalProvider;
          if (globalProvider != null)
            ServiceProvider.SetGlobalProvider(new ServiceProvider(globalProvider));
          else if (ServiceProvider.globalProvider == null)
            ServiceProvider.SetGlobalProvider(new ServiceProvider());
        }
        return ServiceProvider.globalProvider;
      }
    }

    public static ServiceProvider CreateFromSetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp)
    {
      ServiceProvider sp1 = new ServiceProvider(sp);
      if (ServiceProvider.IsNullOrUnsited(ServiceProvider.globalProvider))
        ServiceProvider.SetGlobalProvider(sp1);
      return sp1;
    }
  }
}
