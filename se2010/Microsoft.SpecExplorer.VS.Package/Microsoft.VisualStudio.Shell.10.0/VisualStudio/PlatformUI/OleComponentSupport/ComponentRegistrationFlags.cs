// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.OleComponentSupport.ComponentRegistrationFlags
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.PlatformUI.OleComponentSupport
{
  [Flags]
  public enum ComponentRegistrationFlags
  {
    NeedIdleTime = 1,
    PreTranslateKeys = 4,
    PreTranslateAll = 8,
    NeedSpecialActivationNotifications = 16, // 0x00000010
    NeedAllActivationNotifications = 32, // 0x00000020
    ExclusiveBorderSpace = 64, // 0x00000040
    ExclusiveActivation = 128, // 0x00000080
    NeedAllMacEvents = 256, // 0x00000100
    Master = 512, // 0x00000200
  }
}
