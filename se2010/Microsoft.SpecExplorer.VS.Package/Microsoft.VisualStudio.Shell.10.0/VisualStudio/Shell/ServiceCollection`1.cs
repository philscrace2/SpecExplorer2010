// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ServiceCollection`1
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Shell
{
  public class ServiceCollection<T> : Dictionary<Type, T>
  {
    private static ServiceCollection<T>.EmbeddedTypeAwareTypeComparer serviceTypeComparer = new ServiceCollection<T>.EmbeddedTypeAwareTypeComparer();

    public ServiceCollection()
      : base((IEqualityComparer<Type>) ServiceCollection<T>.serviceTypeComparer)
    {
    }

    private class EmbeddedTypeAwareTypeComparer : IEqualityComparer<Type>
    {
      public bool Equals(Type x, Type y)
      {
        return x.GUID == y.GUID;
      }

      public int GetHashCode(Type obj)
      {
        return obj.GUID.GetHashCode();
      }
    }
  }
}
