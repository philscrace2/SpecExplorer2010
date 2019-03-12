// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.Performance.CodeMarkerEvent
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

namespace Microsoft.Internal.Performance
{
  internal enum CodeMarkerEvent
  {
    perfShellUI_VSStatusBarReady = 7001, // 0x00001B59
    perfGel_ShapeIdentifierCacheMiss = 18100, // 0x000046B4
    perfGel_ShapeIdentifierCacheDiscard = 18101, // 0x000046B5
    perfShellUI_MainWindowCtor = 18150, // 0x000046E6
    perfShellUI_MainWindowOnSourceInitialized = 18151, // 0x000046E7
    perfShellUI_LoadProfileFromLocalStorageBegin = 18152, // 0x000046E8
    perfShellUI_LoadProfileFromLocalStorageEnd = 18153, // 0x000046E9
    perfShellUI_SaveProfileToLocalStorageBegin = 18154, // 0x000046EA
    perfShellUI_SaveProfileToLocalStorageEnd = 18155, // 0x000046EB
    perfShellUI_LoadDefaultWindowProfileBegin = 18156, // 0x000046EC
    perfShellUI_LoadDefaultWindowProfileEnd = 18157, // 0x000046ED
    perfShellUI_CreateProfileFromRegistryBegin = 18158, // 0x000046EE
    perfShellUI_CreateProfileFromRegistryEnd = 18159, // 0x000046EF
    perfShellUI_ViewManagerInitializeBegin = 18160, // 0x000046F0
    perfShellUI_ViewManagerInitializeEnd = 18161, // 0x000046F1
    perfShellUI_BaseToolbarHostAddToolbarBegin = 18162, // 0x000046F2
    perfShellUI_BaseToolbarHostAddToolbarEnd = 18163, // 0x000046F3
    perfShellUI_UtilitySelectStyleForItemBegin = 18164, // 0x000046F4
    perfShellUI_UtilitySelectStyleForItemEnd = 18165, // 0x000046F5
    perfShellUI_GrayscaleImageConvertValueBegin = 18166, // 0x000046F6
    perfShellUI_GrayscaleImageConvertValueEnd = 18167, // 0x000046F7
    perfShellUI_UtilityResolveDeferredStyleForItem = 18168, // 0x000046F8
  }
}
