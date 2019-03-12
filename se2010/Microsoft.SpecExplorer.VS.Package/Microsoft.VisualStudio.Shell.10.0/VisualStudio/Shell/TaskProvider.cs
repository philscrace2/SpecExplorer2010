// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.TaskProvider
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Microsoft.VisualStudio.Shell
{
  [ComVisible(true)]
  [CLSCompliant(false)]
  public class TaskProvider : IVsTaskProvider2, IVsTaskProvider, IVsTaskProvider3, IDisposable
  {
    internal System.IServiceProvider provider;
    internal ImageList imageList;
    internal IVsTaskList taskList;
    internal uint taskListCookie;
    internal TaskProvider.TaskCollection tasks;
    internal StringCollection subCategories;
    internal int suspended;
    internal bool dirty;
    internal Guid providerGuid;
    internal string name;
    internal bool alwaysVisible;
    internal bool disableAutoRoute;
    internal Guid toolbarGroup;
    internal int toolbarId;
    internal bool maintainOrder;
    private bool inFinalRelease;

    public TaskProvider(System.IServiceProvider provider)
    {
      this.provider = provider;
    }

    ~TaskProvider()
    {
      this.Dispose(false);
    }

    public bool MaintainInitialTaskOrder
    {
      get
      {
        return this.maintainOrder;
      }
      set
      {
        this.maintainOrder = value;
      }
    }

    public Guid ProviderGuid
    {
      get
      {
        return this.providerGuid;
      }
      set
      {
        this.providerGuid = value;
      }
    }

    public string ProviderName
    {
      get
      {
        return this.name;
      }
      set
      {
        this.name = value;
      }
    }

    public bool AlwaysVisible
    {
      get
      {
        return this.alwaysVisible;
      }
      set
      {
        this.alwaysVisible = value;
      }
    }

    public bool DisableAutoRoute
    {
      get
      {
        return this.disableAutoRoute;
      }
      set
      {
        this.disableAutoRoute = value;
      }
    }

    public Guid ToolbarGroup
    {
      get
      {
        return this.toolbarGroup;
      }
      set
      {
        this.toolbarGroup = value;
      }
    }

    public int ToolbarId
    {
      get
      {
        return this.toolbarId;
      }
      set
      {
        this.toolbarId = value;
      }
    }

    public ImageList ImageList
    {
      get
      {
        return this.imageList;
      }
      set
      {
        if (this.imageList == value)
          return;
        this.imageList = value;
        this.UpdateProviderInfo();
      }
    }

    public StringCollection Subcategories
    {
      get
      {
        if (this.subCategories == null)
          this.subCategories = new StringCollection();
        return this.subCategories;
      }
    }

    public TaskProvider.TaskCollection Tasks
    {
      get
      {
        if (this.tasks == null)
          this.tasks = new TaskProvider.TaskCollection(this);
        return this.tasks;
      }
    }

    protected virtual IVsTaskList VsTaskList
    {
      get
      {
        if (this.taskList == null)
        {
          this.taskList = this.GetService(typeof (SVsTaskList)) as IVsTaskList;
          if (this.taskList == null)
            throw new InvalidOperationException(string.Format((IFormatProvider) Resources.Culture, Resources.General_MissingService, (object) typeof (IVsTaskList).FullName));
          Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.taskList.RegisterTaskProvider((IVsTaskProvider) this, out this.taskListCookie));
        }
        return this.taskList;
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.tasks != null && !this.inFinalRelease)
      {
        this.tasks.Clear();
        this.tasks = (TaskProvider.TaskCollection) null;
      }
      if (this.taskList != null)
      {
        try
        {
          this.taskList.UnregisterTaskProvider(this.taskListCookie);
        }
        catch (Exception ex)
        {
        }
        this.taskList = (IVsTaskList) null;
      }
      if (this.imageList == null)
        return;
      this.imageList.Dispose();
      this.imageList = (ImageList) null;
    }

    protected internal object GetService(System.Type serviceType)
    {
      if (this.provider != null)
        return this.provider.GetService(serviceType);
      return (object) null;
    }

    public bool Navigate(Task task, Guid logicalView)
    {
      if (task == null)
        throw new ArgumentNullException(nameof (task));
      if (task.Document == null || task.Document.Length == 0)
        return false;
      IVsUIShellOpenDocument service1 = this.GetService(typeof (IVsUIShellOpenDocument)) as IVsUIShellOpenDocument;
      if (service1 == null)
        return false;
      Guid rguidLogicalView = logicalView;
      Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP;
      IVsUIHierarchy ppHier;
      uint pitemid;
      IVsWindowFrame ppWindowFrame;
      if (Microsoft.VisualStudio.NativeMethods.Failed(service1.OpenDocumentViaProject(task.Document, ref rguidLogicalView, out ppSP, out ppHier, out pitemid, out ppWindowFrame)) || ppWindowFrame == null)
        return false;
      object pvar;
      ppWindowFrame.GetProperty(-4004, out pvar);
      Microsoft.VisualStudio.TextManager.Interop.VsTextBuffer vsTextBuffer = pvar as Microsoft.VisualStudio.TextManager.Interop.VsTextBuffer;
      if (vsTextBuffer == null)
      {
        IVsTextBufferProvider textBufferProvider = pvar as IVsTextBufferProvider;
        if (textBufferProvider != null)
        {
          IVsTextLines ppTextBuffer;
          Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(textBufferProvider.GetTextBuffer(out ppTextBuffer));
          vsTextBuffer = ppTextBuffer as Microsoft.VisualStudio.TextManager.Interop.VsTextBuffer;
          if (vsTextBuffer == null)
            return false;
        }
      }
      IVsTextManager service2 = this.GetService(typeof (VsTextManagerClass)) as IVsTextManager;
      if (service2 == null)
        return false;
      int line = task.Line;
      if (line > 0)
        --line;
      service2.NavigateToLineAndColumn((IVsTextBuffer) vsTextBuffer, ref logicalView, line, 0, line, 0);
      return true;
    }

    public void Refresh()
    {
      if (this.suspended == 0)
      {
        this.dirty = false;
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.VsTaskList.RefreshTasks(this.taskListCookie));
      }
      else
        this.dirty = true;
    }

    public virtual void Show()
    {
      (this.GetService(typeof (IUIService)) as IUIService)?.ShowToolWindow(new Guid("{4A9B7E51-AA16-11D0-A8C5-00A0C921A4D2}"));
    }

    public void SuspendRefresh()
    {
      if (this.suspended >= int.MaxValue)
        return;
      ++this.suspended;
    }

    public void ResumeRefresh()
    {
      if (this.suspended <= 0)
        return;
      --this.suspended;
      if (this.suspended != 0 || !this.dirty)
        return;
      this.Refresh();
    }

    private void TasksChanged()
    {
      this.Refresh();
    }

    private void UpdateProviderInfo()
    {
      if (this.taskList == null)
        return;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(this.taskList.UpdateProviderInfo(this.taskListCookie));
    }

    int IVsTaskProvider.EnumTaskItems(out IVsEnumTaskItems items)
    {
      items = (IVsEnumTaskItems) new TaskProvider.VsEnumTaskItems(this.Tasks);
      return 0;
    }

    int IVsTaskProvider.ImageList(out IntPtr himagelist)
    {
      if (this.ImageList != null)
      {
        HandleRef himl = new HandleRef((object) null, this.ImageList.Handle);
        himagelist = Microsoft.VisualStudio.UnsafeNativeMethods.ImageList_Duplicate(himl);
      }
      else
        himagelist = IntPtr.Zero;
      return 0;
    }

    int IVsTaskProvider.OnTaskListFinalRelease(IVsTaskList taskList)
    {
      this.inFinalRelease = true;
      this.Dispose(true);
      return 0;
    }

    int IVsTaskProvider.ReRegistrationKey(out string key)
    {
      key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) this.GetType().Name, (object) this.GetHashCode());
      return 0;
    }

    int IVsTaskProvider.SubcategoryList(uint cbstr, string[] rgbstr, out uint cnt)
    {
      cnt = 0U;
      if (rgbstr == null)
      {
        if (cbstr != 0U)
          throw new ArgumentNullException(nameof (rgbstr));
        cnt = this.subCategories == null ? 0U : (uint) this.subCategories.Count;
        return 0;
      }
      if (this.subCategories != null)
      {
        cnt = 0U;
        while (cnt < cbstr && (long) cnt < (long) this.subCategories.Count)
        {
          rgbstr[(int) cnt] = this.subCategories[(int) cnt];
          ++cnt;
        }
      }
      return 0;
    }

    int IVsTaskProvider2.EnumTaskItems(out IVsEnumTaskItems items)
    {
      return ((IVsTaskProvider) this).EnumTaskItems(out items);
    }

    int IVsTaskProvider2.ImageList(out IntPtr himagelist)
    {
      return ((IVsTaskProvider) this).ImageList(out himagelist);
    }

    int IVsTaskProvider2.OnTaskListFinalRelease(IVsTaskList taskList)
    {
      return ((IVsTaskProvider) this).OnTaskListFinalRelease(taskList);
    }

    int IVsTaskProvider2.ReRegistrationKey(out string key)
    {
      return ((IVsTaskProvider) this).ReRegistrationKey(out key);
    }

    int IVsTaskProvider2.SubcategoryList(uint cbstr, string[] rgbstr, out uint cnt)
    {
      return ((IVsTaskProvider) this).SubcategoryList(cbstr, rgbstr, out cnt);
    }

    int IVsTaskProvider2.MaintainInitialTaskOrder(out int fMaintainOrder)
    {
      fMaintainOrder = this.maintainOrder ? 1 : 0;
      return 0;
    }

    int IVsTaskProvider3.GetProviderFlags(out uint tpfFlags)
    {
      tpfFlags = this.alwaysVisible ? 1U : 0U;
      if (this.disableAutoRoute)
        tpfFlags |= 2U;
      return 0;
    }

    int IVsTaskProvider3.GetProviderName(out string pbstrName)
    {
      pbstrName = this.name;
      return 0;
    }

    int IVsTaskProvider3.GetProviderGuid(out Guid pguidProvider)
    {
      pguidProvider = this.GetType().GUID;
      return 0;
    }

    int IVsTaskProvider3.GetProviderToolbar(out Guid pguidGroup, out uint pdwID)
    {
      pguidGroup = this.toolbarGroup;
      pdwID = (uint) this.toolbarId;
      return 0;
    }

    int IVsTaskProvider3.GetColumnCount(out int count)
    {
      count = 0;
      return -2147467259;
    }

    int IVsTaskProvider3.GetColumn(int iColumn, VSTASKCOLUMN[] pColumn)
    {
      return -2147467259;
    }

    int IVsTaskProvider3.GetSurrogateProviderGuid(out Guid guid)
    {
      guid = Guid.Empty;
      return -2147467263;
    }

    int IVsTaskProvider3.OnBeginTaskEdit(IVsTaskItem item)
    {
      return 0;
    }

    int IVsTaskProvider3.OnEndTaskEdit(
      IVsTaskItem item,
      int fCommitChanges,
      out int fAllowChanges)
    {
      fAllowChanges = 1;
      return 0;
    }

    public sealed class TaskCollection : IList, ICollection, IEnumerable
    {
      private TaskProvider owner;
      private ArrayList list;

      public TaskCollection(TaskProvider owner)
      {
        if (owner == null)
          throw new ArgumentNullException(nameof (owner));
        this.owner = owner;
        this.list = new ArrayList();
      }

      public int Count
      {
        get
        {
          return this.list.Count;
        }
      }

      public Task this[int index]
      {
        get
        {
          return (Task) this.list[index];
        }
        set
        {
          if (value == null)
            throw new ArgumentNullException(nameof (value));
          Task task = this[index];
          if (task != null)
            task.Owner = (TaskProvider) null;
          this.list[index] = (object) value;
          value.Owner = this.owner;
          this.owner.TasksChanged();
        }
      }

      public int Add(Task task)
      {
        if (task == null)
          throw new ArgumentNullException(nameof (task));
        int num = this.list.Add((object) task);
        task.Owner = this.owner;
        this.owner.TasksChanged();
        return num;
      }

      public void Clear()
      {
        if (this.list.Count <= 0)
          return;
        foreach (Task task in this.list)
          task.Owner = (TaskProvider) null;
        this.list.Clear();
        this.owner.TasksChanged();
      }

      public bool Contains(Task task)
      {
        return this.list.Contains((object) task);
      }

      private void EnsureTask(object obj)
      {
        if (!(obj is Task))
          throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.General_InvalidType, (object) typeof (Task).FullName), nameof (obj));
      }

      public IEnumerator GetEnumerator()
      {
        return this.list.GetEnumerator();
      }

      public int IndexOf(Task task)
      {
        return this.list.IndexOf((object) task);
      }

      public void Insert(int index, Task task)
      {
        if (task == null)
          throw new ArgumentNullException(nameof (task));
        this.list.Insert(index, (object) task);
        task.Owner = this.owner;
        this.owner.TasksChanged();
      }

      public void Remove(Task task)
      {
        if (task == null)
          throw new ArgumentNullException(nameof (task));
        this.list.Remove((object) task);
        task.Owner = (TaskProvider) null;
        this.owner.TasksChanged();
      }

      public void RemoveAt(int index)
      {
        this[index].Owner = (TaskProvider) null;
        this.list.RemoveAt(index);
        this.owner.TasksChanged();
      }

      void ICollection.CopyTo(Array array, int index)
      {
        this.list.CopyTo(array, index);
      }

      bool ICollection.IsSynchronized
      {
        get
        {
          return false;
        }
      }

      object ICollection.SyncRoot
      {
        get
        {
          return (object) this;
        }
      }

      bool IList.IsFixedSize
      {
        get
        {
          return false;
        }
      }

      bool IList.IsReadOnly
      {
        get
        {
          return false;
        }
      }

      object IList.this[int index]
      {
        get
        {
          return (object) this[index];
        }
        set
        {
          this.EnsureTask(value);
          this[index] = (Task) value;
        }
      }

      int IList.Add(object obj)
      {
        this.EnsureTask(obj);
        return this.Add((Task) obj);
      }

      void IList.Clear()
      {
        this.Clear();
      }

      bool IList.Contains(object obj)
      {
        this.EnsureTask(obj);
        return this.Contains((Task) obj);
      }

      int IList.IndexOf(object obj)
      {
        this.EnsureTask(obj);
        return this.IndexOf((Task) obj);
      }

      void IList.Insert(int index, object obj)
      {
        this.EnsureTask(obj);
        this.Insert(index, (Task) obj);
      }

      void IList.Remove(object obj)
      {
        this.EnsureTask(obj);
        this.Remove((Task) obj);
      }

      void IList.RemoveAt(int index)
      {
        this.RemoveAt(index);
      }
    }

    private class VsEnumTaskItems : IVsEnumTaskItems
    {
      private TaskProvider.TaskCollection tasks;
      private IEnumerator taskEnum;

      internal VsEnumTaskItems(TaskProvider.TaskCollection tasks)
      {
        this.tasks = tasks;
        this.taskEnum = tasks.GetEnumerator();
      }

      public int Clone(out IVsEnumTaskItems newItems)
      {
        newItems = (IVsEnumTaskItems) new TaskProvider.VsEnumTaskItems(this.tasks);
        return 0;
      }

      public int Next(uint celt, IVsTaskItem[] items, uint[] pceltFetched)
      {
        if (items == null || (long) items.Length < (long) celt)
          throw new ArgumentException(string.Empty, nameof (items));
        uint num = 0;
        while (num < celt && this.taskEnum.MoveNext())
          items[(IntPtr) num++] = (IVsTaskItem) this.taskEnum.Current;
        if (pceltFetched != null && pceltFetched.Length > 0)
          pceltFetched[0] = num;
        return num == 0U && celt > 0U ? 1 : 0;
      }

      public int Reset()
      {
        this.taskEnum.Reset();
        return 0;
      }

      public int Skip(uint count)
      {
        while (count != 0U)
        {
          --count;
          if (!this.taskEnum.MoveNext() && count != 0U)
            return 1;
        }
        return 0;
      }
    }
  }
}
