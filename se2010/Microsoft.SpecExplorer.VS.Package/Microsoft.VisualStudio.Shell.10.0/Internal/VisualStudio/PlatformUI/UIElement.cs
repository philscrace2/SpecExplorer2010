// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.UIElement
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public abstract class UIElement : IVsUIElement, IObjectWithSite
  {
    private System.IServiceProvider serviceProvider;
    private IVsUISimpleDataSource dataSource;

    int IVsUIElement.get_DataSource(out IVsUISimpleDataSource ppDataSource)
    {
      ppDataSource = this.DataSource;
      return 0;
    }

    int IVsUIElement.put_DataSource(IVsUISimpleDataSource pDataSource)
    {
      this.DataSource = pDataSource;
      return 0;
    }

    public abstract int GetUIObject(out object ppUnk);

    public abstract int TranslateAccelerator(IVsUIAccelerator pAccel);

    public void GetSite(ref Guid riid, out IntPtr ppvSite)
    {
      Microsoft.VisualStudio.OLE.Interop.IServiceProvider service = this.ServiceProvider.GetService(typeof (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
      if (service == null)
        throw new ArgumentException(Resources.Error_ProviderCannotBeObtainedFromSite);
      IntPtr iunknownForObject = Marshal.GetIUnknownForObject((object) service);
      int errorCode = Marshal.QueryInterface(iunknownForObject, ref riid, out ppvSite);
      Marshal.Release(iunknownForObject);
      Marshal.ThrowExceptionForHR(errorCode);
    }

    public void SetSite(object pUnkSite)
    {
      if (pUnkSite == null)
        throw new ArgumentNullException(nameof (pUnkSite));
      System.IServiceProvider serviceProvider = pUnkSite as System.IServiceProvider;
      if (serviceProvider == null)
      {
        Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp = pUnkSite as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
        if (sp == null)
          throw new ArgumentException(Resources.Error_SiteIsNotProvider);
        this.serviceProvider = (System.IServiceProvider) new Microsoft.VisualStudio.Shell.ServiceProvider(sp);
      }
      else
      {
        if (!(serviceProvider.GetService(typeof (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) is Microsoft.VisualStudio.OLE.Interop.IServiceProvider))
          throw new ArgumentException(Resources.Error_ProviderCannotBeObtainedFromSite);
        this.serviceProvider = serviceProvider;
      }
    }

    public virtual IVsUISimpleDataSource DataSource
    {
      get
      {
        return this.dataSource;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        this.dataSource = value;
      }
    }

    public System.IServiceProvider ServiceProvider
    {
      get
      {
        return this.serviceProvider;
      }
    }
  }
}
