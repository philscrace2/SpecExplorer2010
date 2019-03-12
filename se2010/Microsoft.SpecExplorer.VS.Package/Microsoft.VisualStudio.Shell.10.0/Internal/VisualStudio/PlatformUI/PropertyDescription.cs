// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.PropertyDescription
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  public class PropertyDescription : IPropertyDescription
  {
    private static IDictionary<System.Type, string> typeMap = (IDictionary<System.Type, string>) new Dictionary<System.Type, string>((IEqualityComparer<System.Type>) new PropertyDescription.EmbeddedTypeAwareTypeComparer());
    private static IDictionary<string, System.Type> vsuiTypeMap = (IDictionary<string, System.Type>) new Dictionary<string, System.Type>();
    private string name;
    private string type;

    public PropertyDescription(string name, string type)
    {
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.Length == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_EmptyString, (object) nameof (name)));
      if (type == null)
        throw new ArgumentNullException(nameof (type));
      if (type.Length == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_EmptyString, (object) nameof (type)));
      this.name = name;
      this.type = type;
    }

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public string Type
    {
      get
      {
        return this.type;
      }
    }

    public static string VsUITypeFromType(System.Type type)
    {
      PropertyDescription.BuildTypeMaps();
      if (type == (System.Type) null)
        throw new ArgumentNullException(nameof (type));
      string str;
      PropertyDescription.typeMap.TryGetValue(type, out str);
      if (string.IsNullOrEmpty(str))
      {
        if (typeof (IVsUIDataSource).IsAssignableFrom(type))
          str = "VsUI.DataSource";
        else if (typeof (IVsUICollection).IsAssignableFrom(type))
          str = "VsUI.Collection";
        else if (typeof (DispatchWrapper).IsAssignableFrom(type))
          str = "VsUI.Dispatch";
        else if (typeof (UnknownWrapper).IsAssignableFrom(type))
          str = "VsUI.Unknown";
      }
      return str;
    }

    public static System.Type TypeFromVsUIType(string typeName)
    {
      PropertyDescription.BuildTypeMaps();
      if (typeName == null)
        throw new ArgumentNullException(nameof (typeName));
      System.Type type;
      PropertyDescription.vsuiTypeMap.TryGetValue(typeName, out type);
      return type;
    }

    public static bool IsBuiltInUIType(string typeName)
    {
      return PropertyDescription.TypeFromVsUIType(typeName) != (System.Type) null;
    }

    private static void BuildTypeMaps()
    {
      lock (PropertyDescription.typeMap)
      {
        if (PropertyDescription.typeMap.Count != 0)
          return;
        PropertyDescription.typeMap[typeof (bool)] = "VsUI.Boolean";
        PropertyDescription.typeMap[typeof (sbyte)] = "VsUI.Char";
        PropertyDescription.typeMap[typeof (short)] = "VsUI.Int16";
        PropertyDescription.typeMap[typeof (int)] = "VsUI.Int32";
        PropertyDescription.typeMap[typeof (long)] = "VsUI.Int64";
        PropertyDescription.typeMap[typeof (byte)] = "VsUI.Byte";
        PropertyDescription.typeMap[typeof (ushort)] = "VsUI.Word";
        PropertyDescription.typeMap[typeof (uint)] = "VsUI.DWord";
        PropertyDescription.typeMap[typeof (ulong)] = "VsUI.QWord";
        PropertyDescription.typeMap[typeof (float)] = "VsUI.Single";
        PropertyDescription.typeMap[typeof (double)] = "VsUI.Double";
        PropertyDescription.typeMap[typeof (string)] = "VsUI.String";
        PropertyDescription.typeMap[typeof (DateTime)] = "VsUI.DateTime";
        PropertyDescription.typeMap[typeof (Decimal)] = "VsUI.Decimal";
        PropertyDescription.typeMap[typeof (UnknownWrapper)] = "VsUI.Unknown";
        PropertyDescription.typeMap[typeof (DispatchWrapper)] = "VsUI.Dispatch";
        PropertyDescription.typeMap[typeof (IVsUIDataSource)] = "VsUI.DataSource";
        PropertyDescription.typeMap[typeof (IVsUICollection)] = "VsUI.Collection";
        foreach (System.Type key in (IEnumerable<System.Type>) PropertyDescription.typeMap.Keys)
          PropertyDescription.vsuiTypeMap[PropertyDescription.typeMap[key]] = key;
      }
    }

    private class EmbeddedTypeAwareTypeComparer : IEqualityComparer<System.Type>
    {
      public bool Equals(System.Type x, System.Type y)
      {
        return x.IsEquivalentTo(y);
      }

      public int GetHashCode(System.Type obj)
      {
        return obj.GUID.GetHashCode();
      }
    }
  }
}
