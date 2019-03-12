// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.WpfLoaderPrivate
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  internal class WpfLoaderPrivate : IVsUIWpfLoader
  {
    public int CreateUIElement(string elementFQN, string codeBase, out IVsUIElement ppUIElement)
    {
      ppUIElement = (IVsUIElement) null;
      try
      {
        if (string.IsNullOrEmpty(elementFQN))
          return -2147024809;
        Type type;
        if (string.IsNullOrEmpty(codeBase))
        {
          type = Type.GetType(elementFQN);
        }
        else
        {
          string name = elementFQN;
          int length = name.IndexOf(',');
          AssemblyName assemblyRef;
          if (length != -1)
          {
            string str = name.Substring(length + 1);
            name = name.Substring(0, length);
            assemblyRef = new AssemblyName(str.Trim());
          }
          else
            assemblyRef = new AssemblyName();
          assemblyRef.CodeBase = codeBase;
          type = AppDomain.CurrentDomain.Load(assemblyRef).GetType(name);
        }
        ppUIElement = (IVsUIElement) new WpfUIElement(type);
        return 0;
      }
      catch (Exception ex)
      {
        return Marshal.GetHRForException(ex);
      }
    }

    public int CreateUIElementOfType(object pUnkElementType, out IVsUIElement ppUIElement)
    {
      ppUIElement = (IVsUIElement) null;
      if (pUnkElementType == null)
        return -2147024809;
      try
      {
        Type frameworkElementType = (Type) pUnkElementType;
        ppUIElement = (IVsUIElement) new WpfUIElement(frameworkElementType);
        return 0;
      }
      catch (Exception ex)
      {
        return Marshal.GetHRForException(ex);
      }
    }

    public int ShowModalElement(IVsUIElement pUIElement, IntPtr hWndParent, out int pResult)
    {
      try
      {
        pResult = WindowHelper.ShowModalElement(pUIElement, hWndParent);
        return 0;
      }
      catch (Exception ex)
      {
        pResult = -1;
        return Marshal.GetHRForException(ex);
      }
    }
  }
}
