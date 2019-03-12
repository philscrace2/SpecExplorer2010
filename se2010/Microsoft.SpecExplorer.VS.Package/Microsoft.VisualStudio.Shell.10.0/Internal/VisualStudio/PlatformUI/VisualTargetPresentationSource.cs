// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.VisualTargetPresentationSource
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  public class VisualTargetPresentationSource : PresentationSource
  {
    private VisualTarget visualTarget;

    public VisualTargetPresentationSource(HostVisual hostVisual)
    {
      if (hostVisual == null)
        throw new ArgumentNullException(nameof (hostVisual));
      this.visualTarget = new VisualTarget(hostVisual);
    }

    public override Visual RootVisual
    {
      get
      {
        return this.visualTarget.RootVisual;
      }
      [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows), UIPermission(SecurityAction.InheritanceDemand, Window = UIPermissionWindow.AllWindows)] set
      {
        Visual rootVisual = this.visualTarget.RootVisual;
        this.visualTarget.RootVisual = value;
        this.RootChanged(rootVisual, value);
        System.Windows.UIElement uiElement = value as System.Windows.UIElement;
        if (uiElement == null)
          return;
        uiElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        uiElement.Arrange(new Rect(uiElement.DesiredSize));
      }
    }

    public override bool IsDisposed
    {
      get
      {
        return false;
      }
    }

    protected override CompositionTarget GetCompositionTargetCore()
    {
      return (CompositionTarget) this.visualTarget;
    }
  }
}
