// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.PackageRegistrationAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class PackageRegistrationAttribute : RegistrationAttribute
  {
    private RegistrationMethod registrationMethod;
    private bool useManagedResources;
    private string satellitePath;

    public RegistrationMethod RegisterUsing
    {
      get
      {
        return this.registrationMethod;
      }
      set
      {
        this.registrationMethod = value;
      }
    }

    public bool UseManagedResourcesOnly
    {
      get
      {
        return this.useManagedResources;
      }
      set
      {
        this.useManagedResources = value;
      }
    }

    public string SatellitePath
    {
      get
      {
        return this.satellitePath;
      }
      set
      {
        this.satellitePath = value;
      }
    }

    private string RegKeyName(RegistrationAttribute.RegistrationContext context)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Packages\\{0}", (object) context.ComponentType.GUID.ToString("B"));
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      Type componentType = context.ComponentType;
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyPackage, (object) componentType.Name, (object) componentType.GUID.ToString("B")));
      RegistrationAttribute.Key key1 = (RegistrationAttribute.Key) null;
      try
      {
        key1 = context.CreateKey(this.RegKeyName(context));
        DescriptionAttribute attribute = TypeDescriptor.GetAttributes(componentType)[typeof (DescriptionAttribute)] as DescriptionAttribute;
        if (attribute != null && !string.IsNullOrEmpty(attribute.Description))
          key1.SetValue(string.Empty, (object) attribute.Description);
        else
          key1.SetValue(string.Empty, (object) componentType.AssemblyQualifiedName);
        key1.SetValue("InprocServer32", (object) context.InprocServerPath);
        key1.SetValue("Class", (object) componentType.FullName);
        if (context.RegistrationMethod != RegistrationMethod.Default)
          this.registrationMethod = context.RegistrationMethod;
        switch (this.registrationMethod)
        {
          case RegistrationMethod.Default:
          case RegistrationMethod.Assembly:
            key1.SetValue("Assembly", (object) componentType.Assembly.FullName);
            break;
          case RegistrationMethod.CodeBase:
            key1.SetValue("CodeBase", (object) context.CodeBase);
            break;
        }
        RegistrationAttribute.Key key2 = (RegistrationAttribute.Key) null;
        if (this.useManagedResources)
          return;
        try
        {
          key2 = key1.CreateSubkey("SatelliteDll");
          string str = this.SatellitePath == null ? context.ComponentPath : context.EscapePath(this.SatellitePath);
          key2.SetValue("Path", (object) str);
          key2.SetValue("DllName", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}UI.dll", (object) Path.GetFileNameWithoutExtension(componentType.Assembly.ManifestModule.Name)));
        }
        finally
        {
          key2?.Close();
        }
      }
      finally
      {
        key1?.Close();
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.RegKeyName(context));
    }
  }
}
