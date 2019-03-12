// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ErrorTask
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Shell
{
  [ComVisible(true)]
  [CLSCompliant(false)]
  public class ErrorTask : Task, IVsErrorItem
  {
    private IVsHierarchy item;
    private TaskErrorCategory category;

    public ErrorTask()
    {
      this.category = TaskErrorCategory.Error;
    }

    public ErrorTask(Exception error)
      : base(error)
    {
    }

    public TaskErrorCategory ErrorCategory
    {
      get
      {
        return this.category;
      }
      set
      {
        this.category = value;
      }
    }

    public IVsHierarchy HierarchyItem
    {
      get
      {
        return this.item;
      }
      set
      {
        this.item = value;
      }
    }

    int IVsErrorItem.GetHierarchy(out IVsHierarchy ppHier)
    {
      ppHier = this.item;
      return 0;
    }

    int IVsErrorItem.GetCategory(out uint pCategory)
    {
      pCategory = (uint) this.ErrorCategory;
      return 0;
    }
  }
}
