// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideObjectAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideObjectAttribute : RegistrationAttribute
  {
    private Type _objectType;
    private RegistrationMethod registrationMethod;

    public ProvideObjectAttribute(Type objectType)
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

    private string CLSIDRegKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CLSID\\{0}", (object) this.ObjectType.GUID.ToString("B"));
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyCreateObject, (object) this.ObjectType.Name));
      using (RegistrationAttribute.Key key = context.CreateKey(this.CLSIDRegKey))
      {
        key.SetValue(string.Empty, (object) this.ObjectType.FullName);
        key.SetValue("InprocServer32", (object) context.InprocServerPath);
        key.SetValue("Class", (object) this.ObjectType.FullName);
        if (context.RegistrationMethod != RegistrationMethod.Default)
          this.registrationMethod = context.RegistrationMethod;
        switch (this.registrationMethod)
        {
          case RegistrationMethod.Default:
          case RegistrationMethod.Assembly:
            key.SetValue("Assembly", (object) this.ObjectType.Assembly.FullName);
            break;
          case RegistrationMethod.CodeBase:
            key.SetValue("CodeBase", (object) context.CodeBase);
            break;
        }
        key.SetValue("ThreadingModel", (object) "Both");
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.CLSIDRegKey);
    }
  }
}
