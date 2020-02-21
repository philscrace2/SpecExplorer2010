// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ProvideSolutionProps
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.VisualStudio.Shell;
using System;
using System.Globalization;

namespace Microsoft.SpecExplorer.VS
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  internal sealed class ProvideSolutionProps : RegistrationAttribute
  {
    public ProvideSolutionProps(string propName)
    {
      this.PropertyName = propName;
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ProvideSolutionProps: ({0} = {1})", (object) context.ComponentType.GUID.ToString("B"), (object) this.PropertyName));
      RegistrationAttribute.Key key = (RegistrationAttribute.Key) null;
      try
      {
        key = context.CreateKey(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) "SolutionPersistence", (object) this.PropertyName));
        key.SetValue(string.Empty, (object) context.ComponentType.GUID.ToString("B"));
      }
      finally
      {
        key.Close();
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) "SolutionPersistence", (object) this.PropertyName));
    }

    public string PropertyName { get; private set; }
  }
}
