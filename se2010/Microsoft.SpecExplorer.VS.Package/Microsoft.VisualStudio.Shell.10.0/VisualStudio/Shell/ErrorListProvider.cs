// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ErrorListProvider
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows.Forms.Design;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public class ErrorListProvider : TaskProvider
  {
    private IVsErrorList errorList;

    public ErrorListProvider(IServiceProvider provider)
      : base(provider)
    {
    }

    ~ErrorListProvider()
    {
      this.Dispose(false);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      this.errorList = (IVsErrorList) null;
    }

    protected override IVsTaskList VsTaskList
    {
      get
      {
        if (this.taskList == null)
        {
          this.errorList = this.GetService(typeof (SVsErrorList)) as IVsErrorList;
          if (this.errorList == null)
            return base.VsTaskList;
          this.taskList = this.errorList as IVsTaskList;
          if (this.taskList == null)
            throw new InvalidOperationException(string.Format((IFormatProvider) Resources.Culture, Resources.General_MissingService, (object) typeof (IVsTaskList).FullName));
          NativeMethods.ThrowOnFailure(this.taskList.RegisterTaskProvider((IVsTaskProvider) this, out this.taskListCookie));
        }
        return this.taskList;
      }
    }

    public void BringToFront()
    {
      IVsTaskList vsTaskList = this.VsTaskList;
      NativeMethods.ThrowOnFailure(this.errorList.BringToFront());
    }

    public void ForceShowErrors()
    {
      IVsTaskList vsTaskList = this.VsTaskList;
      NativeMethods.ThrowOnFailure(this.errorList.ForceShowErrors());
    }

    public override void Show()
    {
      IUIService service = this.GetService(typeof (IUIService)) as IUIService;
      if (service == null)
        return;
      Guid toolWindow = new Guid("{D78612C7-9962-4B83-95D9-268046DAD23A}");
      service.ShowToolWindow(toolWindow);
    }
  }
}
