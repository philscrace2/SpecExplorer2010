// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.DataSourceParameters
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell;
using System;
using System.Windows;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  public sealed class DataSourceParameters
  {
    internal FrameworkElement visualElement;
    internal IServiceProvider serviceProvider;
    internal bool isPropertyAccessSynchronized;

    public DataSourceParameters()
      : this((FrameworkElement) null, (IServiceProvider) ServiceProvider.GlobalProvider, false)
    {
    }

    public DataSourceParameters(FrameworkElement visualElement)
      : this(visualElement, (IServiceProvider) ServiceProvider.GlobalProvider, false)
    {
    }

    public DataSourceParameters(IServiceProvider serviceProvider)
      : this((FrameworkElement) null, serviceProvider, false)
    {
    }

    public DataSourceParameters(bool isPropertyAccessSynchronized)
      : this((FrameworkElement) null, (IServiceProvider) ServiceProvider.GlobalProvider, isPropertyAccessSynchronized)
    {
    }

    public DataSourceParameters(FrameworkElement visualElement, IServiceProvider serviceProvider)
      : this(visualElement, serviceProvider, false)
    {
    }

    public DataSourceParameters(
      FrameworkElement visualElement,
      IServiceProvider serviceProvider,
      bool isPropertyAccessSynchronized)
    {
      this.visualElement = visualElement;
      this.serviceProvider = serviceProvider;
      this.isPropertyAccessSynchronized = isPropertyAccessSynchronized;
    }
  }
}
