// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Settings.ShellSettingsStore
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
  internal class ShellSettingsStore : SettingsStore
  {
    private IVsSettingsStore settingsStore;

    public override bool GetBoolean(string collectionPath, string propertyName)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      int num;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetBool(collectionPath, propertyName, out num));
      return Convert.ToBoolean(num);
    }

    public override bool GetBoolean(string collectionPath, string propertyName, bool defaultValue)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      int num;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetBoolOrDefault(collectionPath, propertyName, Convert.ToInt32(defaultValue), out num));
      return Convert.ToBoolean(num);
    }

    public override int GetInt32(string collectionPath, string propertyName)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      int num;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetInt(collectionPath, propertyName, out num));
      return num;
    }

    public override int GetInt32(string collectionPath, string propertyName, int defaultValue)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      int num;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetIntOrDefault(collectionPath, propertyName, defaultValue, out num));
      return num;
    }

    public override uint GetUInt32(string collectionPath, string propertyName)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      uint num;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetUnsignedInt(collectionPath, propertyName, out num));
      return num;
    }

    public override uint GetUInt32(string collectionPath, string propertyName, uint defaultValue)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      uint num;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetUnsignedIntOrDefault(collectionPath, propertyName, defaultValue, out num));
      return num;
    }

    public override long GetInt64(string collectionPath, string propertyName)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      long num;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetInt64(collectionPath, propertyName, out num));
      return num;
    }

    public override long GetInt64(string collectionPath, string propertyName, long defaultValue)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      long num;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetInt64OrDefault(collectionPath, propertyName, defaultValue, out num));
      return num;
    }

    public override ulong GetUInt64(string collectionPath, string propertyName)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      ulong num;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetUnsignedInt64(collectionPath, propertyName, out num));
      return num;
    }

    public override ulong GetUInt64(string collectionPath, string propertyName, ulong defaultValue)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      ulong num;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetUnsignedInt64OrDefault(collectionPath, propertyName, defaultValue, out num));
      return num;
    }

    public override string GetString(string collectionPath, string propertyName)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      string str;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetString(collectionPath, propertyName, out str));
      return str;
    }

    public override string GetString(
      string collectionPath,
      string propertyName,
      string defaultValue)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      string str;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetStringOrDefault(collectionPath, propertyName, defaultValue, out str));
      return str;
    }

    public override MemoryStream GetMemoryStream(
      string collectionPath,
      string propertyName)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      uint[] actualByteLength = new uint[1];
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetBinary(collectionPath, propertyName, 0U, (byte[]) null, actualByteLength));
      uint byteLength = actualByteLength[0];
      byte[] numArray = new byte[(IntPtr) byteLength];
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetBinary(collectionPath, propertyName, byteLength, numArray, (uint[]) null));
      return new MemoryStream(numArray);
    }

    public override SettingsType GetPropertyType(
      string collectionPath,
      string propertyName)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      uint type;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetPropertyType(collectionPath, propertyName, out type));
      return (SettingsType) type;
    }

    public override bool PropertyExists(string collectionPath, string propertyName)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      HelperMethods.CheckNullArgument((object) propertyName, nameof (propertyName));
      int pfExists;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.PropertyExists(collectionPath, propertyName, out pfExists));
      return Convert.ToBoolean(pfExists);
    }

    public override bool CollectionExists(string collectionPath)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      int pfExists;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.CollectionExists(collectionPath, out pfExists));
      return Convert.ToBoolean(pfExists);
    }

    public override DateTime GetLastWriteTime(string collectionPath)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      SYSTEMTIME[] lastWriteTime = new SYSTEMTIME[1];
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetLastWriteTime(collectionPath, lastWriteTime));
      return new DateTime((int) lastWriteTime[0].wYear, (int) lastWriteTime[0].wMonth, (int) lastWriteTime[0].wDay, (int) lastWriteTime[0].wHour, (int) lastWriteTime[0].wMinute, (int) lastWriteTime[0].wSecond, (int) lastWriteTime[0].wMilliseconds, DateTimeKind.Local);
    }

    public override int GetSubCollectionCount(string collectionPath)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      uint subCollectionCount;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetSubCollectionCount(collectionPath, out subCollectionCount));
      return Convert.ToInt32(subCollectionCount);
    }

    public override int GetPropertyCount(string collectionPath)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      uint propertyCount;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetPropertyCount(collectionPath, out propertyCount));
      return Convert.ToInt32(propertyCount);
    }

    public override IEnumerable<string> GetSubCollectionNames(string collectionPath)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      uint subCollectionCount;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetSubCollectionCount(collectionPath, out subCollectionCount));
      string[] strArray = new string[(IntPtr) subCollectionCount];
      for (uint index = 0; index < subCollectionCount; ++index)
      {
        string subCollectionName;
        // ISSUE: reference to a compiler-generated method
        Marshal.ThrowExceptionForHR(this.settingsStore.GetSubCollectionName(collectionPath, index, out subCollectionName));
        strArray[(IntPtr) index] = subCollectionName;
      }
      return (IEnumerable<string>) strArray;
    }

    public override IEnumerable<string> GetPropertyNames(string collectionPath)
    {
      HelperMethods.CheckNullArgument((object) collectionPath, nameof (collectionPath));
      uint propertyCount;
      // ISSUE: reference to a compiler-generated method
      Marshal.ThrowExceptionForHR(this.settingsStore.GetPropertyCount(collectionPath, out propertyCount));
      string[] strArray = new string[(IntPtr) propertyCount];
      for (uint index = 0; index < propertyCount; ++index)
      {
        string propertyName;
        // ISSUE: reference to a compiler-generated method
        Marshal.ThrowExceptionForHR(this.settingsStore.GetPropertyName(collectionPath, index, out propertyName));
        strArray[(IntPtr) index] = propertyName;
      }
      return (IEnumerable<string>) strArray;
    }

    internal ShellSettingsStore(IVsSettingsStore settingsStore)
    {
      HelperMethods.CheckNullArgument((object) settingsStore, nameof (settingsStore));
      this.settingsStore = settingsStore;
    }
  }
}
