// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideAutomationObjectAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideAutomationObjectAttribute : RegistrationAttribute
  {
    private string name;
    private string description;

    public ProvideAutomationObjectAttribute(string objectName)
    {
      if (objectName == null)
        throw new ArgumentNullException("ObjectName");
      this.name = objectName;
    }

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public string Description
    {
      get
      {
        return this.description;
      }
      set
      {
        this.description = value;
      }
    }

    private string GetAutomationRegKey(Guid packageGuid)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Packages\\{0}\\Automation", (object) packageGuid.ToString("B"));
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      using (RegistrationAttribute.Key key = context.CreateKey(this.GetAutomationRegKey(context.ComponentType.GUID)))
      {
        string str = this.Description == null ? "" : this.Description;
        key.SetValue(this.Name, (object) str);
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.GetAutomationRegKey(context.ComponentType.GUID));
    }
  }
}
