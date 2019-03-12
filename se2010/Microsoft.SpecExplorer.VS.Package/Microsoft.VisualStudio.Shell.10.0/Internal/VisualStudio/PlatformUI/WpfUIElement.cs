// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.WpfUIElement
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class WpfUIElement : UIElement, IVsUIWpfElement
  {
    private Type frameworkElementType;
    private FrameworkElement frameworkElement;

    public WpfUIElement(Type frameworkElementType)
    {
      if (frameworkElementType == (Type) null)
        throw new ArgumentNullException(nameof (frameworkElementType));
      this.frameworkElementType = frameworkElementType;
    }

    public override int TranslateAccelerator(IVsUIAccelerator pAccel)
    {
      return -2147467263;
    }

    public override int GetUIObject(out object ppUnk)
    {
      ppUnk = (object) this;
      return 0;
    }

    public int CreateFrameworkElement(out object ppUnkElement)
    {
      if (this.frameworkElement != null)
        throw new InvalidOperationException(Resources.Error_DoubleCreateElementCall);
      object instance = Activator.CreateInstance(this.frameworkElementType);
      this.frameworkElement = instance as FrameworkElement;
      if (this.frameworkElement == null)
      {
        System.IServiceProvider serviceProvider = instance as System.IServiceProvider;
        if (serviceProvider != null)
          this.frameworkElement = serviceProvider.GetService(typeof (FrameworkElement)) as FrameworkElement;
      }
      if (this.frameworkElement == null)
      {
        ppUnkElement = (object) null;
        return -2147467259;
      }
      DataSourceParameters parameters = new DataSourceParameters(this.frameworkElement, this.ServiceProvider);
      if (this.DataSource is IVsUIDataSource)
        this.frameworkElement.DataContext = (object) new DataSource(this.DataSource as IVsUIDataSource, parameters);
      else if (this.DataSource is IVsUICollection)
        this.frameworkElement.DataContext = (object) DataSourceCollection.CreateInstance(this.DataSource as IVsUICollection, parameters);
      else if (this.frameworkElement.DataContext == null)
        this.frameworkElement.DataContext = (object) this.DataSource;
      (this.frameworkElement as IObjectWithSite)?.SetSite((object) this.ServiceProvider);
      ppUnkElement = (object) this.frameworkElement;
      return 0;
    }

    public int GetFrameworkElement(out object ppUnkElement)
    {
      if (this.frameworkElement == null)
        throw new InvalidOperationException(Resources.Error_GetFrameworkElementBeforeCreate);
      ppUnkElement = (object) this.frameworkElement;
      return 0;
    }
  }
}
