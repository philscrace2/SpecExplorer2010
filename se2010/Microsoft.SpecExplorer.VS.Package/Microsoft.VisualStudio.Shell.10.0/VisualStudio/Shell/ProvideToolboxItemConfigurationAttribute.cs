// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideToolboxItemConfigurationAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideToolboxItemConfigurationAttribute : RegistrationAttribute
  {
    private Type _objectType;

    public ProvideToolboxItemConfigurationAttribute(Type objectType)
    {
      if (objectType == (Type) null)
        throw new ArgumentNullException(nameof (objectType));
      this._objectType = objectType;
    }

    public Type ObjectType
    {
      get
      {
        return this._objectType;
      }
    }

    private string CLSIDRegKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CLSID\\{0}", (object) this.ObjectType.GUID.ToString("B"));
      }
    }

    private string GetItemCfgFilterKey(string filter)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ToolboxItemConfiguration\\{0}", (object) filter);
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyToolboxItemConfiguration, (object) this.ObjectType.Name));
      using (RegistrationAttribute.Key key = context.CreateKey(this.CLSIDRegKey))
      {
        key.SetValue(string.Empty, (object) this.ObjectType.FullName);
        key.SetValue("InprocServer32", (object) context.InprocServerPath);
        key.SetValue("Class", (object) this.ObjectType.FullName);
        if (context.RegistrationMethod == RegistrationMethod.CodeBase)
          key.SetValue("Codebase", (object) context.CodeBase);
        else
          key.SetValue("Assembly", (object) this.ObjectType.Assembly.FullName);
        key.SetValue("ThreadingModel", (object) "Both");
      }
      string str = this.ObjectType.GUID.ToString("B");
      foreach (ProvideAssemblyFilterAttribute customAttribute in this.ObjectType.GetCustomAttributes(typeof (ProvideAssemblyFilterAttribute), true))
      {
        context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyToolboxItemFilter, (object) customAttribute.AssemblyFilter));
        using (RegistrationAttribute.Key key = context.CreateKey(this.GetItemCfgFilterKey(customAttribute.AssemblyFilter)))
          key.SetValue(this.ObjectType.FullName, (object) str);
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.CLSIDRegKey);
      foreach (ProvideAssemblyFilterAttribute customAttribute in this.ObjectType.GetCustomAttributes(typeof (ProvideAssemblyFilterAttribute), true))
        context.RemoveKey(this.GetItemCfgFilterKey(customAttribute.AssemblyFilter));
    }
  }
}
