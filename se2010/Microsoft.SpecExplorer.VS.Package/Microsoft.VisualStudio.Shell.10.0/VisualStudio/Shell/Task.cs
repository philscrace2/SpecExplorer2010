// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Task
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.CodeDom;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.InteropServices;
using System.Xml;

namespace Microsoft.VisualStudio.Shell
{
  [ComVisible(true)]
  [CLSCompliant(false)]
  public class Task : IVsTaskItem, IVsProvideUserContext
  {
    public const string contextNameKeyword = "Keyword";
    private TaskProvider owner;
    private bool canDelete;
    private bool checkedEditable;
    private bool priorityEditable;
    private bool textEditable;
    private TaskPriority priority;
    private TaskCategory category;
    private int subCategoryIndex;
    private string text;
    private string document;
    private string caption;
    private string helpKeyword;
    private int line;
    private int imageIndex;
    private int column;
    private bool isChecked;
    private IVsUserContext context;

    public Task()
    {
      this.priority = TaskPriority.Normal;
      this.subCategoryIndex = -1;
      this.imageIndex = -1;
      this.line = -1;
      this.column = -1;
      this.text = string.Empty;
      this.helpKeyword = string.Empty;
      this.document = string.Empty;
    }

    public Task(Exception error)
      : this()
    {
      if (error == null)
        throw new ArgumentNullException(nameof (error));
      this.Text = error.Message;
      this.HelpKeyword = error.HelpLink;
      if (this.Text.Length == 0)
        this.Text = error.ToString();
      for (; error != null; error = error.InnerException)
      {
        if (error is CodeDomSerializerException serializerException)
        {
          CodeLinePragma linePragma = serializerException.LinePragma;
          if (linePragma == null)
            break;
          this.Document = linePragma.FileName;
          this.Line = linePragma.LineNumber;
          break;
        }
        if (error is XmlException xmlException)
        {
          this.Line = xmlException.LineNumber - 1;
          break;
        }
      }
    }

    public bool CanDelete
    {
      get
      {
        return this.canDelete;
      }
      set
      {
        if (this.canDelete == value)
          return;
        this.canDelete = value;
        this.UpdateOwner();
      }
    }

    public TaskCategory Category
    {
      get
      {
        return this.category;
      }
      set
      {
        if (this.category == value)
          return;
        this.category = value;
        this.UpdateOwner();
      }
    }

    public bool Checked
    {
      get
      {
        return this.isChecked;
      }
      set
      {
        if (this.isChecked == value)
          return;
        this.isChecked = value;
        this.UpdateOwner();
      }
    }

    public int Column
    {
      get
      {
        return this.column;
      }
      set
      {
        if (this.column == value)
          return;
        this.column = value;
        this.UpdateOwner();
      }
    }

    public string Document
    {
      get
      {
        return this.document;
      }
      set
      {
        if (value == null)
          value = string.Empty;
        if (!(this.document != value))
          return;
        this.document = value;
        this.UpdateOwner();
      }
    }

    public string HelpKeyword
    {
      get
      {
        return this.helpKeyword;
      }
      set
      {
        if (value == null)
          value = string.Empty;
        if (!(this.helpKeyword != value))
          return;
        this.helpKeyword = value;
        this.UpdateOwner();
      }
    }

    public int ImageIndex
    {
      get
      {
        return this.imageIndex;
      }
      set
      {
        if (this.imageIndex == value)
          return;
        this.imageIndex = value;
        this.UpdateOwner();
      }
    }

    public bool IsCheckedEditable
    {
      get
      {
        return this.checkedEditable;
      }
      set
      {
        if (this.checkedEditable == value)
          return;
        this.checkedEditable = value;
        this.UpdateOwner();
      }
    }

    public bool IsPriorityEditable
    {
      get
      {
        return this.priorityEditable;
      }
      set
      {
        if (this.priorityEditable == value)
          return;
        this.priorityEditable = value;
        this.UpdateOwner();
      }
    }

    public bool IsTextEditable
    {
      get
      {
        return this.textEditable;
      }
      set
      {
        if (this.textEditable == value)
          return;
        this.textEditable = value;
        this.UpdateOwner();
      }
    }

    public int Line
    {
      get
      {
        return this.line;
      }
      set
      {
        if (this.line == value)
          return;
        this.line = value;
        this.UpdateOwner();
      }
    }

    internal TaskProvider Owner
    {
      get
      {
        return this.owner;
      }
      set
      {
        if (this.owner != null && value == null)
          this.OnRemoved(EventArgs.Empty);
        this.owner = value;
      }
    }

    public TaskPriority Priority
    {
      get
      {
        return this.priority;
      }
      set
      {
        if (this.priority == value)
          return;
        this.priority = value;
        this.UpdateOwner();
      }
    }

    public int SubcategoryIndex
    {
      get
      {
        return this.subCategoryIndex;
      }
      set
      {
        if (this.subCategoryIndex == value)
          return;
        this.subCategoryIndex = value;
        this.UpdateOwner();
      }
    }

    public string Text
    {
      get
      {
        return this.text;
      }
      set
      {
        if (value == null)
          value = string.Empty;
        if (!(this.text != value))
          return;
        this.text = value;
        this.UpdateOwner();
      }
    }

    public event EventHandler Deleted;

    public event EventHandler Removed;

    public event EventHandler Help;

    public event EventHandler Navigate;

    protected virtual void OnDeleted(EventArgs e)
    {
      if (this.Deleted == null)
        return;
      this.Deleted((object) this, e);
    }

    protected virtual void OnRemoved(EventArgs e)
    {
      if (this.Removed == null)
        return;
      this.Removed((object) this, e);
    }

    protected virtual void OnHelp(EventArgs e)
    {
      if (this.HelpKeyword.Length > 0 && this.owner != null)
        (this.owner.GetService(typeof (IHelpService)) as IHelpService)?.ShowHelpFromKeyword(this.HelpKeyword);
      if (this.Help == null)
        return;
      this.Help((object) this, e);
    }

    protected virtual void OnNavigate(EventArgs e)
    {
      if (this.Navigate == null)
        return;
      this.Navigate((object) this, e);
    }

    private void UpdateOwner()
    {
      if (this.owner == null)
        return;
      this.owner.Refresh();
    }

    private string GetDisplayName(string fileName)
    {
      if (this.owner != null)
      {
        IVsRunningDocumentTable service = (IVsRunningDocumentTable) this.owner.GetService(typeof (SVsRunningDocumentTable));
        if (service != null)
        {
          uint[] numArray = new uint[1];
          IVsHierarchy ppHier;
          IntPtr ppunkDocData;
          uint pdwCookie;
          if (Microsoft.VisualStudio.NativeMethods.Succeeded(service.FindAndLockDocument(0U, fileName, out ppHier, out numArray[0], out ppunkDocData, out pdwCookie)) && ppunkDocData != IntPtr.Zero && ppHier != null)
          {
            Marshal.Release(ppunkDocData);
            object pvar1;
            if (Microsoft.VisualStudio.NativeMethods.Succeeded(ppHier.GetProperty(numArray[0], -2057, out pvar1)) && (bool) pvar1)
            {
              object pvar2;
              int property = ppHier.GetProperty(numArray[0], -2003, out pvar2);
              string str = pvar2 as string;
              if (Microsoft.VisualStudio.NativeMethods.Succeeded(property) && !string.IsNullOrEmpty(str))
                return str;
            }
          }
        }
      }
      return fileName;
    }

    private string GetCaption()
    {
      if (this.caption == null)
        this.caption = this.GetDisplayName(this.document);
      return this.caption;
    }

    int IVsTaskItem.CanDelete(out int fdelete)
    {
      fdelete = this.CanDelete ? 1 : 0;
      return 0;
    }

    int IVsTaskItem.Category(VSTASKCATEGORY[] cat)
    {
      if (cat != null)
        cat[0] = (VSTASKCATEGORY) this.Category;
      return 0;
    }

    int IVsTaskItem.Column(out int col)
    {
      col = this.Column;
      return 0;
    }

    int IVsTaskItem.Document(out string doc)
    {
      doc = this.GetCaption();
      return 0;
    }

    int IVsTaskItem.HasHelp(out int fHelp)
    {
      fHelp = this.Help != null || this.HelpKeyword != null && this.owner != null ? 1 : 0;
      return 0;
    }

    int IVsTaskItem.ImageListIndex(out int index)
    {
      index = this.ImageIndex;
      return 0;
    }

    int IVsTaskItem.IsReadOnly(VSTASKFIELD field, out int fReadOnly)
    {
      bool flag = true;
      switch (field)
      {
        case VSTASKFIELD.FLD_PRIORITY:
          flag = !this.IsPriorityEditable;
          break;
        case VSTASKFIELD.FLD_CHECKED:
          flag = !this.IsCheckedEditable;
          break;
        case VSTASKFIELD.FLD_DESCRIPTION:
          flag = !this.IsTextEditable;
          break;
      }
      fReadOnly = flag ? 1 : 0;
      return 0;
    }

    int IVsTaskItem.Line(out int line)
    {
      line = this.Line;
      return 0;
    }

    int IVsTaskItem.NavigateTo()
    {
      this.OnNavigate(EventArgs.Empty);
      return 0;
    }

    int IVsTaskItem.NavigateToHelp()
    {
      this.OnHelp(EventArgs.Empty);
      return 0;
    }

    int IVsTaskItem.OnDeleteTask()
    {
      this.OnDeleted(EventArgs.Empty);
      return 0;
    }

    int IVsTaskItem.OnFilterTask(int f)
    {
      return 0;
    }

    int IVsTaskItem.SubcategoryIndex(out int index)
    {
      index = this.SubcategoryIndex;
      return index < 0 ? -2147467259 : 0;
    }

    int IVsTaskItem.get_Checked(out int f)
    {
      f = this.Checked ? 1 : 0;
      return 0;
    }

    int IVsTaskItem.get_Priority(VSTASKPRIORITY[] pri)
    {
      if (pri != null)
        pri[0] = (VSTASKPRIORITY) this.Priority;
      return 0;
    }

    int IVsTaskItem.get_Text(out string text)
    {
      text = this.Text;
      return 0;
    }

    int IVsTaskItem.put_Checked(int f)
    {
      this.isChecked = f != 0;
      return 0;
    }

    int IVsTaskItem.put_Priority(VSTASKPRIORITY pri)
    {
      this.priority = (TaskPriority) pri;
      return 0;
    }

    int IVsTaskItem.put_Text(string t)
    {
      this.text = t;
      return 0;
    }

    public int GetUserContext(out IVsUserContext ppctx)
    {
      int num = 0;
      if (this.context == null)
      {
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure((this.owner.GetService(typeof (SVsMonitorUserContext)) as IVsMonitorUserContext).CreateEmptyContext(out this.context));
        num = this.context.AddAttribute(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1, "Keyword", this.HelpKeyword);
      }
      ppctx = this.context;
      return num;
    }
  }
}
