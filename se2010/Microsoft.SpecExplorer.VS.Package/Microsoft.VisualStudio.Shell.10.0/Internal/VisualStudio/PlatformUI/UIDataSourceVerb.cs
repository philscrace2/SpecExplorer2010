// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.UIDataSourceVerb
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class UIDataSourceVerb : VerbDescription
  {
    private CommandHandler handler;

    public UIDataSourceVerb(string name, CommandHandler handler)
      : base(name)
    {
      if (handler == null)
        throw new ArgumentNullException(nameof (handler));
      this.handler = handler;
    }

    public CommandHandler Handler
    {
      get
      {
        return this.handler;
      }
    }
  }
}
