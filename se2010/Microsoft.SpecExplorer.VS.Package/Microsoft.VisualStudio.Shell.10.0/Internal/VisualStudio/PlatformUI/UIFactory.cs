// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.UIFactory
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public abstract class UIFactory : IVsUIFactory
  {
    private System.IServiceProvider serviceProvider;

    protected UIFactory(System.IServiceProvider serviceProvider)
    {
      if (serviceProvider == null)
        throw new ArgumentNullException(nameof (serviceProvider));
      if (!(serviceProvider.GetService(typeof (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) is Microsoft.VisualStudio.OLE.Interop.IServiceProvider))
        throw new ArgumentException(Resources.Error_ProviderCannotBeObtainedFromSite);
      this.serviceProvider = serviceProvider;
    }

    public System.IServiceProvider ServiceProvider
    {
      get
      {
        return this.serviceProvider;
      }
    }

    public int CreateUIElement(ref Guid factory, uint elementId, out IVsUIElement ppUIElement)
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIElement uiElement = this.CreateUIElement(ref factory, elementId);
      if (uiElement == null)
        throw new InvalidComObjectException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CannotCreateUIElement, (object) factory.ToString("B"), (object) elementId));
      IObjectWithSite objectWithSite = uiElement as IObjectWithSite;
      if (objectWithSite != null)
      {
        Microsoft.VisualStudio.OLE.Interop.IServiceProvider service = this.ServiceProvider.GetService(typeof (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)) as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
        objectWithSite.SetSite((object) service);
      }
      ppUIElement = uiElement;
      return 0;
    }

    protected abstract IVsUIElement CreateUIElement(ref Guid factory, uint elementId);
  }
}
