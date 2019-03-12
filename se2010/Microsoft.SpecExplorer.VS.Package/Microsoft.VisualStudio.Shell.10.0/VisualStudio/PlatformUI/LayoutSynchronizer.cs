// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.LayoutSynchronizer
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public static class LayoutSynchronizer
  {
    private static int layoutSynchronizationRefCount = 0;
    private static HashSet<PresentationSource> elementsToUpdate = new HashSet<PresentationSource>();
    private static bool isUpdatingLayout;

    public static IDisposable BeginLayoutSynchronization()
    {
      return (IDisposable) new LayoutSynchronizer.LayoutSynchronizationScope();
    }

    public static bool IsSynchronizing
    {
      get
      {
        return LayoutSynchronizer.layoutSynchronizationRefCount > 0;
      }
    }

    public static void Update(Visual element)
    {
      if (!LayoutSynchronizer.IsSynchronizing || LayoutSynchronizer.isUpdatingLayout)
        return;
      PresentationSource presentationSource = PresentationSource.FromVisual(element);
      if (presentationSource == null)
        return;
      LayoutSynchronizer.elementsToUpdate.Add(presentationSource);
    }

    private static void Synchronize()
    {
      if (LayoutSynchronizer.isUpdatingLayout)
        return;
      LayoutSynchronizer.isUpdatingLayout = true;
      try
      {
        foreach (PresentationSource presentationSource in LayoutSynchronizer.elementsToUpdate)
          (presentationSource.RootVisual as UIElement)?.UpdateLayout();
        LayoutSynchronizer.elementsToUpdate.Clear();
      }
      finally
      {
        LayoutSynchronizer.isUpdatingLayout = false;
      }
    }

    private class LayoutSynchronizationScope : DisposableObject
    {
      public LayoutSynchronizationScope()
      {
        ++LayoutSynchronizer.layoutSynchronizationRefCount;
      }

      protected override void DisposeManagedResources()
      {
        --LayoutSynchronizer.layoutSynchronizationRefCount;
        if (LayoutSynchronizer.layoutSynchronizationRefCount != 0)
          return;
        LayoutSynchronizer.Synchronize();
      }
    }
  }
}
