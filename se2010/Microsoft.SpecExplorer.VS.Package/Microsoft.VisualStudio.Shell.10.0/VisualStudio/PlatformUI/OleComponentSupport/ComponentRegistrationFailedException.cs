// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.OleComponentSupport.ComponentRegistrationFailedException
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.PlatformUI.OleComponentSupport
{
  [Serializable]
  public class ComponentRegistrationFailedException : Exception
  {
    public ComponentRegistrationFailedException()
    {
    }

    public ComponentRegistrationFailedException(string message)
      : base(message)
    {
    }

    public ComponentRegistrationFailedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ComponentRegistrationFailedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
