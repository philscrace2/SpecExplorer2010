// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.WpfUIFactoryElement
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class WpfUIFactoryElement
  {
    private Guid factory;
    private uint elementId;
    private Type elementType;

    public WpfUIFactoryElement(Guid factory, uint elementId, Type elementType)
    {
      this.factory = factory;
      this.elementId = elementId;
      this.elementType = elementType;
    }

    public Guid Factory
    {
      get
      {
        return this.factory;
      }
    }

    public uint ElementId
    {
      get
      {
        return this.elementId;
      }
    }

    public Type ElementType
    {
      get
      {
        return this.elementType;
      }
    }
  }
}
