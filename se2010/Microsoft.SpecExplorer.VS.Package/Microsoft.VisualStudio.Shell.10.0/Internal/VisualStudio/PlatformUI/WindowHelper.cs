// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.WindowHelper
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.VSHelp;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public static class WindowHelper
  {
    private static IServiceProvider serviceProvider;

    internal static IServiceProvider ServiceProvider
    {
      get
      {
        if (WindowHelper.serviceProvider == null)
          WindowHelper.serviceProvider = (IServiceProvider) Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider;
        return WindowHelper.serviceProvider;
      }
      set
      {
        WindowHelper.serviceProvider = value;
      }
    }

    public static void AddHelpTopic(UIDataSource dataSource, string topic)
    {
      if (dataSource == null)
        throw new ArgumentNullException(nameof (dataSource));
      dataSource.AddBuiltInProperty("HelpTopic", (object) topic);
      dataSource.AddCommand("Help", (CommandHandler) ((IVsUIDispatch sender, string verb, object argumentIn, out object argumentOut) =>
      {
        argumentOut = (object) null;
        if (dataSource != sender)
          throw new InvalidOperationException(Resources.Error_CannotInvokeVerbOnDataSource);
        string pszKeyword = argumentIn as string;
        if (string.IsNullOrEmpty(pszKeyword))
        {
          UIObject uiObject = new UIObject(dataSource["HelpTopic"]);
          pszKeyword = uiObject.Data as string;
          if (uiObject.Format != __VSUIDATAFORMAT.VSDF_BUILTIN || uiObject.Type != "VsUI.String" || string.IsNullOrEmpty(pszKeyword))
            throw new InvalidOperationException(Resources.Error_NoValidHelpTopic);
        }
        (WindowHelper.ServiceProvider.GetService(typeof (Help)) as Help).DisplayTopicFromF1Keyword(pszKeyword);
      }));
    }

    public static IVsUIWpfLoader CreateWpfLoader()
    {
      Guid clsid = new Guid("0B127700-143C-4AB5-9D39-BFF47151B563");
      Guid guid = typeof (IVsUIWpfLoader).GUID;
      IntPtr ppvObj;
      int instance = (WindowHelper.ServiceProvider.GetService(typeof (SLocalRegistry)) as ILocalRegistry).CreateInstance(clsid, (object) null, ref guid, 1U, out ppvObj);
      if (instance != 0)
        throw new COMException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CannotCreateWPFLoader), instance);
      // ISSUE: variable of a compiler-generated type
      IVsUIWpfLoader objectForIunknown = Marshal.GetObjectForIUnknown(ppvObj) as IVsUIWpfLoader;
      Marshal.Release(ppvObj);
      return objectForIunknown;
    }

    public static IVsUIElement CreateUIElement(Guid factory, uint elementId)
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIFactory service = WindowHelper.ServiceProvider.GetService(typeof (SVsUIFactory)) as IVsUIFactory;
      // ISSUE: variable of a compiler-generated type
      IVsUIElement ppUIElement;
      // ISSUE: reference to a compiler-generated method
      int uiElement = service.CreateUIElement(ref factory, elementId, out ppUIElement);
      if (uiElement != 0)
        throw new COMException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CannotCreateUIElement, (object) factory.ToString("B"), (object) elementId), uiElement);
      if (ppUIElement == null)
        throw new COMException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CannotCreateUIElement, (object) factory.ToString("B"), (object) elementId), -2147467259);
      return ppUIElement;
    }

    public static IntPtr GetDialogOwnerHandle()
    {
      IVsUIShell service = WindowHelper.ServiceProvider.GetService(typeof (SVsUIShell)) as IVsUIShell;
      if (service == null)
        throw new COMException(Resources.Error_CannotGetUIShellService, -2147467259);
      IntPtr phwnd;
      int dialogOwnerHwnd = service.GetDialogOwnerHwnd(out phwnd);
      if (dialogOwnerHwnd != 0)
        throw new COMException(Resources.Error_CannotGetParent, dialogOwnerHwnd);
      return phwnd;
    }

    public static int ShowModalElement(IVsUIElement element, IntPtr parent)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      object ppUnk;
      // ISSUE: reference to a compiler-generated method
      int uiObject = element.GetUIObject(out ppUnk);
      if (uiObject != 0)
        throw new COMException(Resources.Error_CannotGetUIObject, uiObject);
      // ISSUE: variable of a compiler-generated type
      IVsUIWin32Element vsUiWin32Element = ppUnk as IVsUIWin32Element;
      if (vsUiWin32Element != null)
      {
        IVsUIShell service = WindowHelper.ServiceProvider.GetService(typeof (SVsUIShell)) as IVsUIShell;
        if (service == null)
          throw new COMException(Resources.Error_CannotGetUIShellService, -2147467259);
        int errorCode1 = service.EnableModeless(0);
        if (errorCode1 != 0)
          throw new COMException(Resources.Error_CannotEnterModal, errorCode1);
        try
        {
          int pDlgResult;
          // ISSUE: reference to a compiler-generated method
          int errorCode2 = vsUiWin32Element.ShowModal(parent, out pDlgResult);
          if (errorCode2 != 0)
            throw new COMException(Resources.Error_CannotCreateWindow, errorCode2);
          if (pDlgResult <= 0)
            throw new COMException(Resources.Error_CannotCreateWindow, -2147467259);
          return pDlgResult;
        }
        finally
        {
          service.EnableModeless(1);
        }
      }
      else
      {
        // ISSUE: variable of a compiler-generated type
        IVsUIWpfElement vsUiWpfElement = ppUnk as IVsUIWpfElement;
        if (vsUiWpfElement != null)
        {
          object ppUnkElement;
          // ISSUE: reference to a compiler-generated method
          int frameworkElement = vsUiWpfElement.CreateFrameworkElement(out ppUnkElement);
          if (frameworkElement != 0)
            throw new COMException(Resources.Error_CannotCreateWindow, frameworkElement);
          Window window = ppUnkElement as Window;
          if (window == null)
            throw new COMException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InvalidUIObjectType, (object) typeof (Window).FullName), -2147467259);
          return WindowHelper.ShowModal(window, parent);
        }
        throw new COMException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_UnknownUIObjectType, (object) ppUnk.GetType().FullName), -2147467259);
      }
    }

    public static int ShowModal(Window window, IntPtr parent)
    {
      if (window == null)
        throw new ArgumentNullException(nameof (window));
      IVsUIShell service = WindowHelper.ServiceProvider.GetService(typeof (SVsUIShell)) as IVsUIShell;
      if (service == null)
        throw new COMException(Resources.Error_CannotGetUIShellService, -2147467259);
      int errorCode = service.EnableModeless(0);
      if (errorCode != 0)
        throw new COMException(Resources.Error_CannotEnterModal, errorCode);
      try
      {
        WindowInteropHelper helper = new WindowInteropHelper(window);
        helper.Owner = parent;
        if (window.WindowStartupLocation == WindowStartupLocation.CenterOwner)
          window.SourceInitialized += (EventHandler) delegate
          {
            Microsoft.VisualStudio.NativeMethods.RECT lpRect = new Microsoft.VisualStudio.NativeMethods.RECT();
            if (!Microsoft.VisualStudio.NativeMethods.GetWindowRect(parent, out lpRect))
              return;
            HwndSource hwndSource = HwndSource.FromHwnd(helper.Handle);
            if (hwndSource == null)
              return;
            Point point1 = hwndSource.CompositionTarget.TransformToDevice.Transform(new Point(window.ActualWidth, window.ActualHeight));
            Microsoft.VisualStudio.NativeMethods.RECT rect = WindowHelper.CenterRectOnSingleMonitor(lpRect, (int) point1.X, (int) point1.Y);
            Point point2 = hwndSource.CompositionTarget.TransformFromDevice.Transform(new Point((double) rect.Left, (double) rect.Top));
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = point2.X;
            window.Top = point2.Y;
          };
        bool? nullable = window.ShowDialog();
        return nullable.HasValue ? (nullable.Value ? 1 : 2) : 0;
      }
      finally
      {
        service.EnableModeless(1);
      }
    }

    public static int ShowModal(Window window)
    {
      IntPtr dialogOwnerHandle = WindowHelper.GetDialogOwnerHandle();
      return WindowHelper.ShowModal(window, dialogOwnerHandle);
    }

    private static Microsoft.VisualStudio.NativeMethods.RECT CenterInRect(
      Microsoft.VisualStudio.NativeMethods.RECT parentRect,
      int childWidth,
      int childHeight,
      Microsoft.VisualStudio.NativeMethods.RECT monitorClippingRect)
    {
      Microsoft.VisualStudio.NativeMethods.RECT rect = new Microsoft.VisualStudio.NativeMethods.RECT()
      {
        Left = parentRect.Left + (parentRect.Width - childWidth) / 2,
        Top = parentRect.Top + (parentRect.Height - childHeight) / 2,
        Width = childWidth,
        Height = childHeight
      };
      rect.Left = Math.Min(rect.Right, monitorClippingRect.Right) - rect.Width;
      rect.Top = Math.Min(rect.Bottom, monitorClippingRect.Bottom) - rect.Height;
      rect.Left = Math.Max(rect.Left, monitorClippingRect.Left);
      rect.Top = Math.Max(rect.Top, monitorClippingRect.Top);
      return rect;
    }

    private static Microsoft.VisualStudio.NativeMethods.RECT CenterRectOnSingleMonitor(
      Microsoft.VisualStudio.NativeMethods.RECT parentRect,
      int childWidth,
      int childHeight)
    {
      Microsoft.VisualStudio.NativeMethods.RECT screenSubRect;
      Microsoft.VisualStudio.NativeMethods.RECT monitorRect;
      Microsoft.VisualStudio.NativeMethods.FindMaximumSingleMonitorRectangle(parentRect, out screenSubRect, out monitorRect);
      return WindowHelper.CenterInRect(screenSubRect, childWidth, childHeight, monitorRect);
    }

    public static int ShowModalElement(IVsUIElement element)
    {
      IntPtr dialogOwnerHandle = WindowHelper.GetDialogOwnerHandle();
      return WindowHelper.ShowModalElement(element, dialogOwnerHandle);
    }

    public static int ShowModalElement(
      Guid factory,
      uint elementId,
      IVsUISimpleDataSource dataSource)
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIElement uiElement = WindowHelper.CreateUIElement(factory, elementId);
      // ISSUE: reference to a compiler-generated method
      int errorCode = uiElement.put_DataSource(dataSource);
      if (errorCode != 0)
        throw new COMException(Resources.Error_CannotSetDataSource, errorCode);
      return WindowHelper.ShowModalElement(uiElement);
    }

    public static IntPtr GetHwndFromUIElement(IVsUIElement element, IntPtr parent)
    {
      IntPtr pHandle = IntPtr.Zero;
      object ppUnk;
      // ISSUE: reference to a compiler-generated method
      element.GetUIObject(out ppUnk);
      // ISSUE: variable of a compiler-generated type
      IVsUIWin32Element vsUiWin32Element = ppUnk as IVsUIWin32Element;
      if (vsUiWin32Element != null)
      {
        // ISSUE: reference to a compiler-generated method
        if (vsUiWin32Element.GetHandle(out pHandle) != 0 || pHandle == IntPtr.Zero)
        {
          // ISSUE: reference to a compiler-generated method
          int errorCode = vsUiWin32Element.Create(parent, out pHandle);
          if (errorCode != 0)
            throw new COMException(Resources.Error_CannotCreateWindow, errorCode);
        }
      }
      else
      {
        // ISSUE: variable of a compiler-generated type
        IVsUIWpfElement vsUiWpfElement = ppUnk as IVsUIWpfElement;
        if (vsUiWpfElement != null)
        {
          object ppUnkElement;
          // ISSUE: reference to a compiler-generated method
          int frameworkElement1 = vsUiWpfElement.CreateFrameworkElement(out ppUnkElement);
          if (frameworkElement1 != 0)
            throw new COMException(Resources.Error_CannotCreateWindow, frameworkElement1);
          FrameworkElement frameworkElement2 = ppUnkElement as FrameworkElement;
          if (frameworkElement2 == null)
            throw new COMException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InvalidUIObjectType, (object) typeof (FrameworkElement).FullName), -2147467259);
          HwndSourceParameters parameters = new HwndSourceParameters();
          parameters.WindowName = frameworkElement2.Name;
          parameters.WindowStyle = 1140850688;
          parameters.ParentWindow = parent;
          Microsoft.VisualStudio.NativeMethods.RECT rect = new Microsoft.VisualStudio.NativeMethods.RECT();
          Microsoft.VisualStudio.NativeMethods.GetClientRect(parent, ref rect);
          parameters.PositionX = rect.Left;
          parameters.PositionY = rect.Top;
          parameters.Width = rect.Width;
          parameters.Height = rect.Height;
          HwndSource hwndSource = new HwndSource(parameters);
          hwndSource.RootVisual = (Visual) frameworkElement2;
          pHandle = hwndSource.Handle;
        }
      }
      if (pHandle == IntPtr.Zero)
        throw new COMException(Resources.Error_CannotCreateWindow, -2147467259);
      return pHandle;
    }

    public static IntPtr CreateChildElement(IVsUIElement element, IntPtr parent)
    {
      IntPtr hwndFromUiElement = WindowHelper.GetHwndFromUIElement(element, parent);
      Microsoft.VisualStudio.NativeMethods.ShowWindow(hwndFromUiElement, 5);
      return hwndFromUiElement;
    }

    public static IntPtr CreateChildElement(
      Guid factory,
      uint elementId,
      IVsUISimpleDataSource dataSource,
      IntPtr parent)
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIElement uiElement = WindowHelper.CreateUIElement(factory, elementId);
      // ISSUE: reference to a compiler-generated method
      int errorCode = uiElement.put_DataSource(dataSource);
      if (errorCode != 0)
        throw new COMException(Resources.Error_CannotSetDataSource, errorCode);
      return WindowHelper.CreateChildElement(uiElement, parent);
    }

    public static int ShowModalElement(IVsUIElement element, Window owner)
    {
      object ppUnk;
      // ISSUE: reference to a compiler-generated method
      element.GetUIObject(out ppUnk);
      // ISSUE: variable of a compiler-generated type
      IVsUIWpfElement vsUiWpfElement = ppUnk as IVsUIWpfElement;
      if (vsUiWpfElement == null)
        return WindowHelper.ShowModalElement(element);
      IVsUIShell service = WindowHelper.ServiceProvider.GetService(typeof (SVsUIShell)) as IVsUIShell;
      if (service == null)
        throw new COMException(Resources.Error_CannotGetUIShellService, -2147467259);
      int errorCode = service.EnableModeless(0);
      if (errorCode != 0)
        throw new COMException(Resources.Error_CannotEnterModal, errorCode);
      try
      {
        object ppUnkElement;
        // ISSUE: reference to a compiler-generated method
        int frameworkElement = vsUiWpfElement.CreateFrameworkElement(out ppUnkElement);
        if (frameworkElement != 0)
          throw new COMException(Resources.Error_CannotCreateWindow, frameworkElement);
        Window window = ppUnkElement as Window;
        if (window == null)
          throw new COMException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InvalidUIObjectType, (object) typeof (Window).FullName), -2147467259);
        window.Owner = owner;
        bool? nullable = window.ShowDialog();
        return nullable.HasValue ? (nullable.Value ? 1 : 2) : 0;
      }
      finally
      {
        service.EnableModeless(1);
      }
    }

    public static FrameworkElement GetFrameworkElementFromUIElement(
      IVsUIElement element)
    {
      object ppUnk;
      // ISSUE: reference to a compiler-generated method
      element.GetUIObject(out ppUnk);
      // ISSUE: variable of a compiler-generated type
      IVsUIWpfElement vsUiWpfElement = ppUnk as IVsUIWpfElement;
      if (vsUiWpfElement != null)
      {
        object ppUnkElement;
        // ISSUE: reference to a compiler-generated method
        int frameworkElement1 = vsUiWpfElement.CreateFrameworkElement(out ppUnkElement);
        if (frameworkElement1 != 0)
          throw new COMException(Resources.Error_CannotCreateWindow, frameworkElement1);
        FrameworkElement frameworkElement2 = ppUnkElement as FrameworkElement;
        if (frameworkElement2 == null)
          throw new COMException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_InvalidUIObjectType, (object) typeof (FrameworkElement).FullName), -2147467259);
        return frameworkElement2;
      }
      // ISSUE: variable of a compiler-generated type
      IVsUIWin32Element win32Element = ppUnk as IVsUIWin32Element;
      if (win32Element != null)
        return (FrameworkElement) new WindowHelper.NativeWindowHost(win32Element);
      throw new COMException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_UnknownUIObjectType, (object) ppUnk.GetType().FullName), -2147467259);
    }

    public static bool CreateChildElement(
      IVsUIElement element,
      FrameworkElement parent,
      out FrameworkElement frameworkElement)
    {
      frameworkElement = WindowHelper.GetFrameworkElementFromUIElement(element);
      frameworkElement.Visibility = Visibility.Visible;
      Decorator decorator = parent as Decorator;
      if (decorator != null)
      {
        decorator.Child = (System.Windows.UIElement) frameworkElement;
        return true;
      }
      ContentControl contentControl = parent as ContentControl;
      if (contentControl != null)
      {
        contentControl.Content = (object) frameworkElement;
        return true;
      }
      Panel panel = parent as Panel;
      if (panel == null)
        return false;
      panel.Children.Add((System.Windows.UIElement) frameworkElement);
      return true;
    }

    public static bool CreateChildElement(
      Guid factory,
      uint elementId,
      IVsUISimpleDataSource dataSource,
      FrameworkElement parent,
      out FrameworkElement frameworkElement)
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIElement uiElement = WindowHelper.CreateUIElement(factory, elementId);
      // ISSUE: reference to a compiler-generated method
      int errorCode = uiElement.put_DataSource(dataSource);
      if (errorCode != 0)
        throw new COMException(Resources.Error_CannotSetDataSource, errorCode);
      return WindowHelper.CreateChildElement(uiElement, parent, out frameworkElement);
    }

    private class NativeWindowHost : HwndHost
    {
      private IVsUIWin32Element win32Element;

      public NativeWindowHost(IVsUIWin32Element win32Element)
      {
        if (win32Element == null)
          throw new ArgumentNullException(nameof (win32Element));
        this.win32Element = win32Element;
      }

      protected override HandleRef BuildWindowCore(HandleRef hwndParent)
      {
        IntPtr pHandle;
        // ISSUE: reference to a compiler-generated method
        int errorCode = this.win32Element.Create(hwndParent.Handle, out pHandle);
        if (errorCode != 0)
          throw new COMException(Resources.Error_CannotCreateChildWindow, errorCode);
        return new HandleRef((object) this, pHandle);
      }

      protected override void DestroyWindowCore(HandleRef hwnd)
      {
        IntPtr pHandle;
        // ISSUE: reference to a compiler-generated method
        if (this.win32Element.GetHandle(out pHandle) != 0 || pHandle != hwnd.Handle || this != hwnd.Wrapper)
          throw new COMException(Resources.Error_UnrecognizedWindowHandle, -2147467259);
        // ISSUE: reference to a compiler-generated method
        int errorCode = this.win32Element.Destroy();
        if (errorCode != 0)
          throw new COMException(Resources.Error_CannotDestroyWindow, errorCode);
      }
    }
  }
}
