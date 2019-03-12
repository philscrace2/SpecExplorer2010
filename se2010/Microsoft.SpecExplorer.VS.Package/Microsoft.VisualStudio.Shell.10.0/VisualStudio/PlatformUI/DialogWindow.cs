// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.DialogWindow
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.VSHelp;
using System;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class DialogWindow : DialogWindowBase
  {
    private string helpTopic;

    public DialogWindow()
    {
    }

    public DialogWindow(string helpTopic)
    {
      if (helpTopic == null)
        throw new ArgumentNullException(nameof (helpTopic));
      this.helpTopic = helpTopic;
      this.HasHelpButton = true;
    }

    protected override void InvokeDialogHelp()
    {
      if (this.helpTopic == null || !this.HasHelpButton)
        return;
      (ServiceProvider.GlobalProvider.GetService(typeof (Help)) as Help).DisplayTopicFromF1Keyword(this.helpTopic);
    }

    public bool? ShowModal()
    {
      int num = WindowHelper.ShowModal((Window) this);
      if (num == 0)
        return new bool?();
      return new bool?(num == 1);
    }
  }
}
