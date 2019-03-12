// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.RegisterEditorLogicalViewAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [Obsolete("RegisterEditorLogicalViewAttribute has been deprecated. Please use ProvideEditorLogicalViewAttribute instead.")]
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class RegisterEditorLogicalViewAttribute : RegistrationAttribute
  {
    private Guid factory;
    private Guid logicalView;

    public RegisterEditorLogicalViewAttribute(object factoryType, string logicalViewGuid)
    {
      this.logicalView = new Guid(logicalViewGuid);
      if (factoryType is string)
        this.factory = new Guid((string) factoryType);
      else if ((object) (factoryType as Type) != null)
        this.factory = ((Type) factoryType).GUID;
      else if (factoryType is Guid)
        this.factory = (Guid) factoryType;
      else
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Attributes_InvalidFactoryType, factoryType));
    }

    public Guid FactoryType
    {
      get
      {
        return this.factory;
      }
    }

    public Guid LogicalView
    {
      get
      {
        return this.logicalView;
      }
    }

    private string LogicalViewPath
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Editors\\{0}\\LogicalViews", (object) this.factory.ToString("B"));
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyEditorView, (object) this.logicalView.ToString("B")));
      using (RegistrationAttribute.Key key = context.CreateKey(this.LogicalViewPath))
        key.SetValue(this.logicalView.ToString("B"), (object) "");
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveValue(this.LogicalViewPath, this.logicalView.ToString("B"));
    }
  }
}
