// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ServiceProviderHierarchy
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public sealed class ServiceProviderHierarchy : SortedList<int, System.IServiceProvider>, System.IServiceProvider
  {
    public object GetService(Type serviceType)
    {
      if (serviceType == (Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      if (serviceType.IsEquivalentTo(typeof (IObjectWithSite)))
        throw new InvalidOperationException("can not re-site chained services providers");
      object obj = (object) null;
      if (serviceType.IsEquivalentTo(typeof (ServiceProviderHierarchy)))
      {
        obj = (object) this;
      }
      else
      {
        foreach (System.IServiceProvider serviceProvider in (IEnumerable<System.IServiceProvider>) this.Values)
        {
          obj = serviceProvider.GetService(serviceType);
          if (obj != null)
            break;
        }
      }
      return obj;
    }
  }
}
