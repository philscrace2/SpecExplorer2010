// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.ExtensionMethods
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public static class ExtensionMethods
  {
    public static void ThrowIfNullOrEmpty(this string value, string message)
    {
      if (value == null)
        throw new ArgumentNullException(message);
      if (string.IsNullOrEmpty(value))
        throw new ArgumentException(message);
    }

    public static void CopyTo(this Stream sourceStream, Stream targetStream)
    {
      byte[] buffer = new byte[4096];
      int count;
      while ((count = sourceStream.Read(buffer, 0, 4096)) > 0)
        targetStream.Write(buffer, 0, count);
    }

    public static DependencyObject GetVisualOrLogicalParent(
      this DependencyObject sourceElement)
    {
      if (sourceElement is Visual)
        return VisualTreeHelper.GetParent(sourceElement) ?? LogicalTreeHelper.GetParent(sourceElement);
      return LogicalTreeHelper.GetParent(sourceElement);
    }

    public static TAncestorType FindAncestorOrSelf<TAncestorType>(this DependencyObject obj) where TAncestorType : DependencyObject
    {
      TAncestorType ancestorType = obj as TAncestorType;
      if ((object) ancestorType != null)
        return ancestorType;
      return obj.FindAncestor<TAncestorType, DependencyObject>(new Func<DependencyObject, DependencyObject>(ExtensionMethods.GetVisualOrLogicalParent));
    }

    public static bool IsLogicalAncestorOf(this DependencyObject element, DependencyObject other)
    {
      if (other == null)
        return false;
      return element.IsAncestorOf<DependencyObject>(other, new Func<DependencyObject, DependencyObject>(ExtensionMethods.GetVisualOrLogicalParent));
    }

    public static TAncestorType FindAncestor<TAncestorType>(this DependencyObject obj) where TAncestorType : DependencyObject
    {
      return obj.FindAncestor<TAncestorType, DependencyObject>(new Func<DependencyObject, DependencyObject>(ExtensionMethods.GetVisualOrLogicalParent));
    }

    public static bool IsAncestorOf<TElementType>(
      this TElementType element,
      TElementType other,
      Func<TElementType, TElementType> parentEvaluator)
      where TElementType : class
    {
      for (TElementType elementType = parentEvaluator(other); (object) elementType != null; elementType = parentEvaluator(elementType))
      {
        if ((object) elementType == (object) element)
          return true;
      }
      return false;
    }

    public static TAncestorType FindAncestor<TAncestorType, TElementType>(
      this TElementType obj,
      Func<TElementType, TElementType> parentEvaluator)
      where TAncestorType : class
    {
      for (TElementType elementType = parentEvaluator(obj); (object) elementType != null; elementType = parentEvaluator(elementType))
      {
        TAncestorType ancestorType = (object) elementType as TAncestorType;
        if ((object) ancestorType != null)
          return ancestorType;
      }
      return default (TAncestorType);
    }

    public static T FindDescendant<T>(this DependencyObject obj) where T : class
    {
      T obj1 = default (T);
      for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(obj); ++childIndex)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, childIndex);
        obj1 = child as T;
        if ((object) obj1 == null)
        {
          obj1 = child.FindDescendant<T>();
          if ((object) obj1 != null)
            break;
        }
        else
          break;
      }
      return obj1;
    }

    public static IEnumerable<T> FindDescendants<T>(this DependencyObject obj) where T : class
    {
      List<T> descendants = new List<T>();
      obj.TraverseVisualTree<T>((Action<T>) (child => descendants.Add(child)));
      return (IEnumerable<T>) descendants;
    }

    public static void TraverseVisualTree<T>(this DependencyObject obj, Action<T> action) where T : class
    {
      for (int childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(obj); ++childIndex)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, childIndex);
        T obj1 = child as T;
        child.TraverseVisualTreeReverse<T>(action);
        if ((object) obj1 != null)
          action(obj1);
      }
    }

    public static IEnumerable<T> FindDescendantsReverse<T>(this DependencyObject obj) where T : class
    {
      List<T> descendants = new List<T>();
      obj.TraverseVisualTreeReverse<T>((Action<T>) (child => descendants.Add(child)));
      return (IEnumerable<T>) descendants;
    }

    public static void TraverseVisualTreeReverse<T>(this DependencyObject obj, Action<T> action) where T : class
    {
      for (int childIndex = VisualTreeHelper.GetChildrenCount(obj) - 1; childIndex >= 0; --childIndex)
      {
        DependencyObject child = VisualTreeHelper.GetChild(obj, childIndex);
        T obj1 = child as T;
        child.TraverseVisualTreeReverse<T>(action);
        if ((object) obj1 != null)
          action(obj1);
      }
    }

    public static DependencyObject FindCommonAncestor(
      this DependencyObject obj1,
      DependencyObject obj2)
    {
      return obj1.FindCommonAncestor<DependencyObject>(obj2, new Func<DependencyObject, DependencyObject>(ExtensionMethods.GetVisualOrLogicalParent));
    }

    public static T FindCommonAncestor<T>(this T obj1, T obj2, Func<T, T> parentEvaluator) where T : DependencyObject
    {
      if ((object) obj1 == null || (object) obj2 == null)
        return default (T);
      HashSet<T> objSet = new HashSet<T>();
      for (obj1 = parentEvaluator(obj1); (object) obj1 != null; obj1 = parentEvaluator(obj1))
        objSet.Add(obj1);
      for (obj2 = parentEvaluator(obj2); (object) obj2 != null; obj2 = parentEvaluator(obj2))
      {
        if (objSet.Contains(obj2))
          return obj2;
      }
      return default (T);
    }

    public static bool IsTopmost(IntPtr hWnd)
    {
      Microsoft.VisualStudio.NativeMethods.WINDOWINFO pwi = new Microsoft.VisualStudio.NativeMethods.WINDOWINFO();
      pwi.cbSize = Marshal.SizeOf((object) pwi);
      if (!Microsoft.VisualStudio.NativeMethods.GetWindowInfo(hWnd, ref pwi))
        return false;
      return (pwi.dwExStyle & 8) == 8;
    }

    public static POINTL ToPOINTL(this Point point)
    {
      return new POINTL()
      {
        x = (int) point.X,
        y = (int) point.Y
      };
    }

    public static void RaiseEvent<TEventArgs>(
      this EventHandler<TEventArgs> eventHandler,
      object source,
      TEventArgs args)
      where TEventArgs : EventArgs
    {
      if (eventHandler == null)
        return;
      eventHandler(source, args);
    }

    public static void RaiseEvent(this EventHandler eventHandler, object source)
    {
      eventHandler.RaiseEvent(source, EventArgs.Empty);
    }

    public static void RaiseEvent(this EventHandler eventHandler, object source, EventArgs args)
    {
      if (eventHandler == null)
        return;
      eventHandler(source, args);
    }

    public static void RaiseEvent(
      this CancelEventHandler eventHandler,
      object source,
      CancelEventArgs args)
    {
      if (eventHandler == null)
        return;
      eventHandler(source, args);
    }

    public static bool IsNonreal(this double value)
    {
      if (!double.IsNaN(value))
        return double.IsInfinity(value);
      return true;
    }

    public static bool IsNearlyEqual(this double value1, double value2)
    {
      if (value1.IsNonreal() || value2.IsNonreal())
        return value1.CompareTo(value2) == 0;
      return Math.Abs(value1 - value2) < 1E-05;
    }

    public static bool IsSignificantlyGreater(this double value1, double value2)
    {
      if (value1.IsNearlyEqual(value2))
        return false;
      return value2 > value1;
    }

    public static bool IsConnectedToPresentationSource(this DependencyObject obj)
    {
      return PresentationSource.FromDependencyObject(obj) != null;
    }

    public static bool AcquireWin32Focus(this DependencyObject obj, out IntPtr previousFocus)
    {
      HwndSource hwndSource = PresentationSource.FromDependencyObject(obj) as HwndSource;
      if (hwndSource != null)
      {
        previousFocus = Microsoft.VisualStudio.NativeMethods.GetFocus();
        if (previousFocus != hwndSource.Handle)
        {
          Microsoft.VisualStudio.NativeMethods.SetFocus(hwndSource.Handle);
          return true;
        }
      }
      previousFocus = IntPtr.Zero;
      return false;
    }
  }
}
