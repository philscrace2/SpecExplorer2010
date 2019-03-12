// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Settings.ShellSettingsManager
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Settings
{
  [CLSCompliant(false)]
  public sealed class ShellSettingsManager : SettingsManager
  {
    private IVsSettingsManager settingsManager;

    public ShellSettingsManager(IServiceProvider serviceProvider)
    {
      HelperMethods.CheckNullArgument((object) serviceProvider, nameof (serviceProvider));
      this.settingsManager = serviceProvider.GetService(typeof (SVsSettingsManager)) as IVsSettingsManager;
      if (this.settingsManager == null)
        throw new NotSupportedException(typeof (SVsSettingsManager).FullName);
    }

    public override EnclosingScopes GetCollectionScopes(string collectionPath)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      uint scopes;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsManager.GetCollectionScopes(collectionPath, out scopes));
      return (EnclosingScopes) scopes;
    }

    public override EnclosingScopes GetPropertyScopes(
      string collectionPath,
      string propertyName)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      uint scopes;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsManager.GetPropertyScopes(collectionPath, propertyName, out scopes));
      return (EnclosingScopes) scopes;
    }

    public override SettingsStore GetReadOnlySettingsStore(SettingsScope scope)
    {
      // ISSUE: variable of a compiler-generated type
      IVsSettingsStore store;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsManager.GetReadOnlySettingsStore((uint) scope, out store));
      return (SettingsStore) new ShellSettingsStore(store);
    }

    public override WritableSettingsStore GetWritableSettingsStore(
      SettingsScope scope)
    {
      // ISSUE: variable of a compiler-generated type
      IVsWritableSettingsStore writableStore;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsManager.GetWritableSettingsStore((uint) scope, out writableStore));
      return (WritableSettingsStore) new ShellWritableSettingsStore(writableStore);
    }

    public override string GetApplicationDataFolder(ApplicationDataFolder folder)
    {
      string folderPath;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsManager.GetApplicationDataFolder((uint) folder, out folderPath));
      return folderPath;
    }

    public override IEnumerable<string> GetCommonExtensionsSearchPaths()
    {
      uint actualPaths;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsManager.GetCommonExtensionsSearchPaths(0U, (string[]) null, out actualPaths));
      string[] commonExtensionsPaths = new string[(IntPtr) actualPaths];
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsManager.GetCommonExtensionsSearchPaths((uint) commonExtensionsPaths.Length, commonExtensionsPaths, out actualPaths));
      return (IEnumerable<string>) commonExtensionsPaths;
    }
  }
}
