// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideServiceAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideServiceAttribute : RegistrationAttribute
  {
    private string _name;
    private Guid _serviceGuid;
    private Type _serviceType;

    public ProvideServiceAttribute(object serviceType)
    {
      this._serviceType = (Type) null;
      if (serviceType is string)
        this._serviceGuid = new Guid((string) serviceType);
      else if ((object) (serviceType as Type) != null)
      {
        this._serviceType = (Type) serviceType;
        this._serviceGuid = this._serviceType.GUID;
        this._name = this._serviceType.Name;
      }
      else if (serviceType is Guid)
        this._serviceGuid = (Guid) serviceType;
      else
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Attributes_InvalidFactoryType, serviceType));
    }

    public string ServiceName
    {
      get
      {
        return this._name;
      }
      set
      {
        this._name = value;
      }
    }

    public Type Service
    {
      get
      {
        return this._serviceType;
      }
    }

    public Guid ServiceType
    {
      get
      {
        return this._serviceGuid;
      }
    }

    private string ServiceRegKey
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Services\\{0}", (object) this.ServiceType.ToString("B"));
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyService, (object) this.ServiceName));
      using (RegistrationAttribute.Key key = context.CreateKey(this.ServiceRegKey))
      {
        key.SetValue(string.Empty, (object) context.ComponentType.GUID.ToString("B"));
        key.SetValue("Name", (object) this.ServiceName);
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.ServiceRegKey);
    }
  }
}
