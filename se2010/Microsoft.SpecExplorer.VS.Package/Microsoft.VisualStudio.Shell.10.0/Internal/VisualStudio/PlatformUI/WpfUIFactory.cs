// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.WpfUIFactory
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public abstract class WpfUIFactory : UIFactory
  {
    private object syncObject = new object();
    private WpfUIFactoryElement[] elements;
    private IVsUIWpfLoader wpfLoader;

    protected WpfUIFactory(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    protected virtual IVsUIWpfLoader WpfLoader
    {
      get
      {
        if (this.wpfLoader == null)
        {
          lock (this.syncObject)
            this.wpfLoader = WindowHelper.CreateWpfLoader();
        }
        return this.wpfLoader;
      }
    }

    protected override IVsUIElement CreateUIElement(ref Guid factory, uint elementId)
    {
      foreach (WpfUIFactoryElement factoryElement in this.FactoryElements)
      {
        if (factory.Equals(factoryElement.Factory) && elementId.Equals(factoryElement.ElementId))
        {
          // ISSUE: variable of a compiler-generated type
          IVsUIElement ppUIElement;
          // ISSUE: reference to a compiler-generated method
          int uiElementOfType = this.WpfLoader.CreateUIElementOfType((object) factoryElement.ElementType, out ppUIElement);
          if (uiElementOfType != 0)
            throw new COMException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CannotCreateUIElementType, (object) factoryElement.ElementType.AssemblyQualifiedName), uiElementOfType);
          // ISSUE: variable of a compiler-generated type
          IVsUIElement vsUiElement = ppUIElement;
          return vsUiElement;
        }
      }
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CannotCreateUIElement, (object) factory.ToString("B"), (object) elementId));
    }

    private WpfUIFactoryElement[] FactoryElements
    {
      get
      {
        if (this.elements == null)
        {
          lock (this.syncObject)
            this.elements = this.GetFactoryElements();
        }
        return this.elements;
      }
    }

    protected abstract WpfUIFactoryElement[] GetFactoryElements();
  }
}
