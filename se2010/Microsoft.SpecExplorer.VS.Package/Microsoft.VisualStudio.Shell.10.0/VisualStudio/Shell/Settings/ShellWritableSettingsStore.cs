// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Settings.ShellWritableSettingsStore
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell.Settings
{
  [CLSCompliant(false)]
  internal sealed class ShellWritableSettingsStore : WritableSettingsStore
  {
    private IVsWritableSettingsStore writableSettingsStore;
    private SettingsStore settingsStore;

    public override bool GetBoolean(string collectionPath, string propertyName)
    {
      return this.settingsStore.GetBoolean(collectionPath, propertyName);
    }

    public override bool GetBoolean(string collectionPath, string propertyName, bool defaultValue)
    {
      return this.settingsStore.GetBoolean(collectionPath, propertyName, defaultValue);
    }

    public override int GetInt32(string collectionPath, string propertyName)
    {
      return this.settingsStore.GetInt32(collectionPath, propertyName);
    }

    public override int GetInt32(string collectionPath, string propertyName, int defaultValue)
    {
      return this.settingsStore.GetInt32(collectionPath, propertyName, defaultValue);
    }

    public override uint GetUInt32(string collectionPath, string propertyName)
    {
      return this.settingsStore.GetUInt32(collectionPath, propertyName);
    }

    public override uint GetUInt32(string collectionPath, string propertyName, uint defaultValue)
    {
      return this.settingsStore.GetUInt32(collectionPath, propertyName, defaultValue);
    }

    public override long GetInt64(string collectionPath, string propertyName)
    {
      return this.settingsStore.GetInt64(collectionPath, propertyName);
    }

    public override long GetInt64(string collectionPath, string propertyName, long defaultValue)
    {
      return this.settingsStore.GetInt64(collectionPath, propertyName, defaultValue);
    }

    public override ulong GetUInt64(string collectionPath, string propertyName)
    {
      return this.settingsStore.GetUInt64(collectionPath, propertyName);
    }

    public override ulong GetUInt64(string collectionPath, string propertyName, ulong defaultValue)
    {
      return this.settingsStore.GetUInt64(collectionPath, propertyName, defaultValue);
    }

    public override string GetString(string collectionPath, string propertyName)
    {
      return this.settingsStore.GetString(collectionPath, propertyName);
    }

    public override string GetString(
      string collectionPath,
      string propertyName,
      string defaultValue)
    {
      return this.settingsStore.GetString(collectionPath, propertyName, defaultValue);
    }

    public override MemoryStream GetMemoryStream(
      string collectionPath,
      string propertyName)
    {
      return this.settingsStore.GetMemoryStream(collectionPath, propertyName);
    }

    public override SettingsType GetPropertyType(
      string collectionPath,
      string propertyName)
    {
      return this.settingsStore.GetPropertyType(collectionPath, propertyName);
    }

    public override bool PropertyExists(string collectionPath, string propertyName)
    {
      return this.settingsStore.PropertyExists(collectionPath, propertyName);
    }

    public override bool CollectionExists(string collectionPath)
    {
      return this.settingsStore.CollectionExists(collectionPath);
    }

    public override DateTime GetLastWriteTime(string collectionPath)
    {
      return this.settingsStore.GetLastWriteTime(collectionPath);
    }

    public override int GetSubCollectionCount(string collectionPath)
    {
      return this.settingsStore.GetSubCollectionCount(collectionPath);
    }

    public override int GetPropertyCount(string collectionPath)
    {
      return this.settingsStore.GetPropertyCount(collectionPath);
    }

    public override IEnumerable<string> GetSubCollectionNames(string collectionPath)
    {
      return this.settingsStore.GetSubCollectionNames(collectionPath);
    }

    public override IEnumerable<string> GetPropertyNames(string collectionPath)
    {
      return this.settingsStore.GetPropertyNames(collectionPath);
    }

    public override void SetBoolean(string collectionPath, string propertyName, bool value)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.writableSettingsStore.SetBool(collectionPath, propertyName, Convert.ToInt32(value)));
    }

    public override void SetInt32(string collectionPath, string propertyName, int value)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.writableSettingsStore.SetInt(collectionPath, propertyName, value));
    }

    public override void SetUInt32(string collectionPath, string propertyName, uint value)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.writableSettingsStore.SetUnsignedInt(collectionPath, propertyName, value));
    }

    public override void SetInt64(string collectionPath, string propertyName, long value)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.writableSettingsStore.SetInt64(collectionPath, propertyName, value));
    }

    public override void SetUInt64(string collectionPath, string propertyName, ulong value)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.writableSettingsStore.SetUnsignedInt64(collectionPath, propertyName, value));
    }

    public override void SetString(string collectionPath, string propertyName, string value)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      HelperMethods.CheckNullArgument((object) value, nameof (value));
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.writableSettingsStore.SetString(collectionPath, propertyName, value));
    }

    public override void SetMemoryStream(
      string collectionPath,
      string propertyName,
      MemoryStream value)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      HelperMethods.CheckNullArgument((object) value, nameof (value));
      byte[] array = value.ToArray();
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.writableSettingsStore.SetBinary(collectionPath, propertyName, (uint) array.Length, array));
    }

    public override void CreateCollection(string collectionPath)
    {
      HelperMethods.CheckNullOrEmptyString(collectionPath, nameof (collectionPath));
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.writableSettingsStore.CreateCollection(collectionPath));
    }

    public override bool DeleteCollection(string collectionPath)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      // ISSUE: reference to a compiler-generated method
      int errorCode = this.writableSettingsStore.DeleteCollection(collectionPath);
      Marshal.ThrowExceptionForHR(errorCode);
      return errorCode == 0;
    }

    public override bool DeleteProperty(string collectionPath, string propertyName)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      // ISSUE: reference to a compiler-generated method
      int errorCode = this.writableSettingsStore.DeleteProperty(collectionPath, propertyName);
      Marshal.ThrowExceptionForHR(errorCode);
      return errorCode == 0;
    }

    internal ShellWritableSettingsStore(IVsWritableSettingsStore writableSettingsStore)
    {
      HelperMethods.CheckNullArgument((object) writableSettingsStore, nameof (writableSettingsStore));
      this.writableSettingsStore = writableSettingsStore;
      this.settingsStore = (SettingsStore) new ShellSettingsStore((IVsSettingsStore) this.writableSettingsStore);
    }
  }
}
