// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.WorkerThreadElementContainer
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  public abstract class WorkerThreadElementContainer : FrameworkElement
  {
    private object rootPanelSync = new object();
    private HostVisual hostVisual = new HostVisual();
    private Panel rootPanel;
    private Dictionary<DependencyProperty, object> propertyCache;

    protected WorkerThreadElementContainer()
    {
      this.AddVisualChild((Visual) this.hostVisual);
      this.CreateUIOnWorkerThread();
    }

    protected abstract System.Windows.UIElement CreateRootUIElement();

    protected virtual int StackSize
    {
      get
      {
        return 0;
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (this.rootPanel == null)
        return Size.Empty;
      this.rootPanel.Dispatcher.BeginInvoke((Delegate) (() =>
      {
        Size desiredSize = this.rootPanel.DesiredSize;
        this.rootPanel.Measure(availableSize);
        if (!(desiredSize != this.rootPanel.DesiredSize))
          return;
        this.Dispatcher.BeginInvoke((Delegate) (() => this.InvalidateMeasure()), DispatcherPriority.Render);
      }), DispatcherPriority.Render);
      return this.rootPanel.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      if (this.rootPanel == null)
        return finalSize;
      this.rootPanel.Dispatcher.BeginInvoke((Delegate) (() =>
      {
        Size renderSize = this.rootPanel.RenderSize;
        this.rootPanel.Arrange(new Rect(finalSize));
        if (!(renderSize != this.rootPanel.RenderSize))
          return;
        this.Dispatcher.BeginInvoke((Delegate) (() => this.InvalidateVisual()), DispatcherPriority.Render);
      }), DispatcherPriority.Render);
      return finalSize;
    }

    protected override HitTestResult HitTestCore(
      PointHitTestParameters hitTestParameters)
    {
      return (HitTestResult) new PointHitTestResult((Visual) this.hostVisual, hitTestParameters.HitPoint);
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (e.Property.ReadOnly)
        return;
      FrameworkPropertyMetadata metadata = e.Property.GetMetadata(typeof (FrameworkElement)) as FrameworkPropertyMetadata;
      if (metadata == null || !metadata.Inherits)
        return;
      lock (this.rootPanelSync)
      {
        if (this.rootPanel == null)
        {
          if (this.propertyCache == null)
            this.propertyCache = new Dictionary<DependencyProperty, object>();
          if (e.NewValue == DependencyProperty.UnsetValue)
            this.propertyCache.Remove(e.Property);
          else
            this.propertyCache[e.Property] = e.NewValue;
        }
        else
          this.rootPanel.Dispatcher.BeginInvoke((Delegate) (() => this.rootPanel.SetValue(e.Property, e.NewValue)), DispatcherPriority.DataBind);
      }
    }

    protected override Visual GetVisualChild(int index)
    {
      if (index == 0)
        return (Visual) this.hostVisual;
      throw new ArgumentOutOfRangeException(nameof (index));
    }

    protected override int VisualChildrenCount
    {
      get
      {
        return 1;
      }
    }

    private void CreateUIOnWorkerThread()
    {
      Thread thread = new Thread(new ParameterizedThreadStart(this.UIWorkerThreadStart), this.StackSize);
      thread.SetApartmentState(ApartmentState.STA);
      thread.IsBackground = true;
      thread.CurrentCulture = CultureInfo.CurrentCulture;
      thread.CurrentUICulture = CultureInfo.CurrentUICulture;
      thread.Start();
    }

    private void UIWorkerThreadStart(object arg)
    {
      lock (this.rootPanelSync)
      {
        this.rootPanel = (Panel) new Grid();
        if (this.propertyCache != null)
        {
          foreach (KeyValuePair<DependencyProperty, object> keyValuePair in this.propertyCache)
            this.rootPanel.SetValue(keyValuePair.Key, keyValuePair.Value);
          this.propertyCache.Clear();
          this.propertyCache = (Dictionary<DependencyProperty, object>) null;
        }
      }
      this.rootPanel.Children.Add(this.CreateRootUIElement());
      new VisualTargetPresentationSource(this.hostVisual).RootVisual = (Visual) this.rootPanel;
      Dispatcher.Run();
    }
  }
}
