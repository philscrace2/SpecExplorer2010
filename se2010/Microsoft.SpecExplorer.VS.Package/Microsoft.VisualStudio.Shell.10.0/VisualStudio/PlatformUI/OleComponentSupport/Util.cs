﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.OleComponentSupport.Util
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

namespace Microsoft.VisualStudio.PlatformUI.OleComponentSupport
{
  internal static class Util
  {
    internal static System.Windows.Interop.MSG TranslateMessageStruct(Microsoft.VisualStudio.OLE.Interop.MSG message)
    {
      return new System.Windows.Interop.MSG()
      {
        hwnd = message.hwnd,
        message = (int) message.message,
        wParam = message.wParam,
        lParam = message.lParam,
        time = (int) message.time,
        pt_x = message.pt.x,
        pt_y = message.pt.y
      };
    }
  }
}
