// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.DataSourceCollection
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class DataSourceCollection : IDataSourceCollection, IUIDispatch, IVsUICollection, IVsUISimpleDataSource, IVsUIDispatch, IList<IDataSource>, ICollection<IDataSource>, IEnumerable<IDataSource>, INotifyPropertyChanged, INotifyCollectionChanged, IList, ICollection, IEnumerable, IDisposable
  {
    private IVsUICollection innerCollection;
    private readonly DataSourceParameters parameters;
    private readonly bool readOnly;
    private IList<IDataSource> items;
    private uint collectionChangeEventsCookie;
    private bool _disposed;

    private IVsUIDynamicCollection MutableCollection
    {
      get
      {
        this.ThrowIfDisposed();
        return this.innerCollection as IVsUIDynamicCollection;
      }
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals(obj, (object) this))
        return true;
      if (obj == null)
        return false;
      if ((object) (obj as DataSourceCollection) != null)
        return this.RootInnerCollection == ((DataSourceCollection) obj).RootInnerCollection;
      if (obj is IVsUICollection)
        return this.RootInnerCollection == (IVsUICollection) obj;
      return false;
    }

    public static bool operator ==(DataSourceCollection left, DataSourceCollection right)
    {
      if ((object) left == null)
        return (object) right == null;
      return left.Equals((object) right);
    }

    public static bool operator ==(DataSourceCollection left, IVsUICollection right)
    {
      if ((object) left == null)
        return right == null;
      return left.Equals((object) right);
    }

    public static bool operator ==(IVsUICollection left, DataSourceCollection right)
    {
      if ((object) right == null)
        return left == null;
      return right.Equals((object) left);
    }

    public static bool operator !=(DataSourceCollection left, DataSourceCollection right)
    {
      return !(left == right);
    }

    public static bool operator !=(DataSourceCollection left, IVsUICollection right)
    {
      return !(left == right);
    }

    public static bool operator !=(IVsUICollection left, DataSourceCollection right)
    {
      return !(left == right);
    }

    private IVsUICollection RootInnerCollection
    {
      get
      {
        IVsUICollection innerCollection = this.innerCollection;
        while ((object) (innerCollection as DataSourceCollection) != null)
          innerCollection = ((DataSourceCollection) innerCollection).innerCollection;
        return innerCollection;
      }
    }

    public override int GetHashCode()
    {
      if (this.innerCollection == null)
        return 0;
      return this.innerCollection.GetHashCode();
    }

    public static DataSourceCollection CreateInstance(
      IVsUICollection uiCollection)
    {
      return DataSourceCollection.CreateInstance(uiCollection, new DataSourceParameters());
    }

    public static DataSourceCollection CreateInstance(
      IVsUICollection uiCollection,
      DataSourceParameters parameters)
    {
      return new DataSourceCollection(uiCollection, parameters, !(uiCollection is IVsUIDynamicCollection));
    }

    public static DataSourceCollection CreateReadOnlyInstance(
      IVsUICollection uiCollection)
    {
      return DataSourceCollection.CreateReadOnlyInstance(uiCollection, new DataSourceParameters());
    }

    public static DataSourceCollection CreateReadOnlyInstance(
      IVsUICollection uiCollection,
      DataSourceParameters parameters)
    {
      return new DataSourceCollection(uiCollection, parameters, true);
    }

    private DataSourceCollection(
      IVsUICollection uiCollection,
      DataSourceParameters parameters,
      bool readOnly)
    {
      if (uiCollection == null)
        throw new ArgumentNullException(nameof (uiCollection));
      if (parameters == null)
        throw new ArgumentNullException(nameof (parameters));
      this.innerCollection = uiCollection;
      this.parameters = parameters;
      this.readOnly = readOnly;
      this.InitializeLocalCache();
      this.SubscribeToVisualElementEvents();
      this.SubscribeInnerCollectionChangeEvents();
    }

    private void InitializeLocalCache()
    {
      uint pnCount = 0;
      // ISSUE: reference to a compiler-generated method
      DataSourceCollection.ThrowIfFailed(this.innerCollection.get_Count(out pnCount));
      this.items = (IList<IDataSource>) new List<IDataSource>((int) pnCount);
      for (uint index = 0; (int) index != (int) pnCount; ++index)
        this.items.Add((IDataSource) null);
    }

    ~DataSourceCollection()
    {
      this.Dispose(false);
    }

    private void visualWindow_Closed(object sender, EventArgs e)
    {
      this.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public bool IsDisposed
    {
      get
      {
        return this._disposed;
      }
    }

    private void ThrowIfDisposed()
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!this.IsDisposed)
      {
        this.UnsubscribeFromVisualElementEvents();
        this.UnsubscribeInnerCollectionChangeEvents();
        this.innerCollection = (IVsUICollection) null;
        if (disposing)
        {
          foreach (IDataSource dataSource in (IEnumerable<IDataSource>) this.items)
            (dataSource as IDisposable)?.Dispose();
        }
      }
      this._disposed = true;
    }

    public int IndexOf(IDataSource item)
    {
      this.ThrowIfDisposed();
      int count = this.Count;
      for (int index = 0; index < count; ++index)
      {
        if ((IVsUIDataSource) item == (DataSource) this.GetCachedItem(index))
          return index;
      }
      return -1;
    }

    public void Insert(int index, IDataSource item)
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly();
      // ISSUE: reference to a compiler-generated method
      DataSourceCollection.ThrowIfFailed(this.MutableCollection.InsertItem((uint) index, (IVsUIDataSource) item));
    }

    public void RemoveAt(int index)
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly();
      // ISSUE: reference to a compiler-generated method
      DataSourceCollection.ThrowIfFailed(this.MutableCollection.RemoveItem((uint) index));
    }

    public IDataSource this[int index]
    {
      get
      {
        this.ThrowIfDisposed();
        return this.GetCachedItem(index);
      }
      set
      {
        this.ThrowIfDisposed();
        this.ThrowIfReadOnly();
        DataSourceCollection.ThrowIfFailed(this.MutableCollection.ReplaceItem((uint) index, (IVsUIDataSource) value));
      }
    }

    int IList.Add(object value)
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly();
      IDataSource dataSource = value as IDataSource;
      if (dataSource == null)
        return -1;
      this.Add(dataSource);
      return this.Count - 1;
    }

    bool IList.Contains(object value)
    {
      this.ThrowIfDisposed();
      IDataSource dataSource = value as IDataSource;
      if (dataSource == null)
        return false;
      return this.Contains(dataSource);
    }

    int IList.IndexOf(object value)
    {
      this.ThrowIfDisposed();
      IDataSource dataSource = value as IDataSource;
      if (dataSource == null)
        return -1;
      return this.IndexOf(dataSource);
    }

    void IList.Insert(int index, object value)
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly();
      IDataSource dataSource = value as IDataSource;
      if (dataSource == null)
        return;
      this.Insert(index, dataSource);
    }

    bool IList.IsFixedSize
    {
      get
      {
        this.ThrowIfDisposed();
        return this.readOnly;
      }
    }

    public void Remove(object value)
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly();
      IDataSource dataSource = value as IDataSource;
      if (dataSource == null)
        return;
      this.Remove(dataSource);
    }

    object IList.this[int index]
    {
      get
      {
        return (object) this[index];
      }
      set
      {
        this.ThrowIfDisposed();
        this.ThrowIfReadOnly();
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        IDataSource dataSource = value as IDataSource;
        if (dataSource == null)
          throw new ArgumentException(Resources.Error_CollectionElementIsNotDataSource);
        this[index] = dataSource;
      }
    }

    public void Add(IDataSource item)
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly();
      this.Insert(this.Count, item);
    }

    public void Clear()
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly();
      // ISSUE: reference to a compiler-generated method
      DataSourceCollection.ThrowIfFailed(this.MutableCollection.ClearItems());
    }

    public bool Contains(IDataSource item)
    {
      this.ThrowIfDisposed();
      return this.IndexOf(item) != -1;
    }

    public void CopyTo(IDataSource[] array, int arrayIndex)
    {
      this.ThrowIfDisposed();
      this.CopyToHelper((Array) array, arrayIndex);
    }

    public int Count
    {
      get
      {
        if (this.IsDisposed)
          return 0;
        return this.items.Count;
      }
    }

    public bool IsReadOnly
    {
      get
      {
        this.ThrowIfDisposed();
        return this.readOnly;
      }
    }

    public bool Remove(IDataSource item)
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly();
      int index = this.IndexOf(item);
      if (index == -1)
        return false;
      this.RemoveAt(index);
      return true;
    }

    int ICollection.Count
    {
      get
      {
        if (this.IsDisposed)
          return 0;
        return this.Count;
      }
    }

    public void CopyTo(Array array, int index)
    {
      this.ThrowIfDisposed();
      this.CopyToHelper(array, index);
    }

    public bool IsSynchronized
    {
      get
      {
        this.ThrowIfDisposed();
        return false;
      }
    }

    public object SyncRoot
    {
      get
      {
        this.ThrowIfDisposed();
        return (object) this.items;
      }
    }

    public IEnumerator<IDataSource> GetEnumerator()
    {
      this.ThrowIfDisposed();
      return this.EnumeratorHelper();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      this.ThrowIfDisposed();
      return (IEnumerator) this.EnumeratorHelper();
    }

    private void CopyToHelper(Array array, int index)
    {
      for (int index1 = 0; index1 != this.items.Count; ++index1)
        array.SetValue((object) this.GetCachedItem(index1), index + index1);
    }

    private IDataSource GetCachedItem(int index)
    {
      IDataSource innerItem = this.items[index];
      if (innerItem == null)
      {
        innerItem = this.GetInnerItem((uint) index);
        this.items[index] = innerItem;
      }
      return innerItem;
    }

    private IDataSource RemoveCachedItem(int index)
    {
      IDataSource dataSource = this.items[index];
      this.items.RemoveAt(index);
      return dataSource;
    }

    private IEnumerator<IDataSource> EnumeratorHelper()
    {
      int nItems = this.Count;
      for (int i = 0; i != nItems; ++i)
        yield return this.GetCachedItem(i);
    }

    private void ThrowIfReadOnly()
    {
      if (this.readOnly)
        throw new NotSupportedException(Resources.Error_ReadOnlyCollection);
    }

    private static void ThrowIfFailed(int hr)
    {
      Marshal.ThrowExceptionForHR(hr);
    }

    public int EnumVerbs(out IVsUIEnumDataSourceVerbs ppEnum)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      return this.innerCollection.EnumVerbs(out ppEnum);
    }

    public int Invoke(string verb, object pvaIn, out object pvaOut)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      return this.innerCollection.Invoke(verb, pvaIn, out pvaOut);
    }

    public int get_Count(out uint pnCount)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      return this.innerCollection.get_Count(out pnCount);
    }

    public int GetItem(uint nItem, out IVsUIDataSource pVsUIDataSource)
    {
      this.ThrowIfDisposed();
      pVsUIDataSource = (IVsUIDataSource) this.GetCachedItem((int) nItem);
      return 0;
    }

    public int AdviseCollectionChangeEvents(IVsUICollectionChangeEvents pAdvise, out uint pCookie)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      return this.innerCollection.AdviseCollectionChangeEvents(pAdvise, out pCookie);
    }

    public int UnadviseCollectionChangeEvents(uint cookie)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      return this.innerCollection.UnadviseCollectionChangeEvents(cookie);
    }

    private IDataSource GetInnerItem(uint nItem)
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIDataSource pVsUIDataSource;
      // ISSUE: reference to a compiler-generated method
      DataSourceCollection.ThrowIfFailed(this.innerCollection.GetItem(nItem, out pVsUIDataSource));
      return (IDataSource) new DataSource(pVsUIDataSource, this.parameters);
    }

    public int Close()
    {
      this.ThrowIfDisposed();
      this.Dispose();
      return 0;
    }

    public object Invoke(string verbName, object parameter)
    {
      this.ThrowIfDisposed();
      object pvaOut;
      this.Invoke(verbName, parameter, out pvaOut);
      return pvaOut;
    }

    public IEnumerable<IVerbDescription> Verbs
    {
      get
      {
        this.ThrowIfDisposed();
        IVsUIEnumDataSourceVerbs ppEnum;
        DataSourceCollection.ThrowIfFailed(this.innerCollection.EnumVerbs(out ppEnum));
        if (ppEnum == null)
          throw new InvalidOperationException();
        return DataSourceCollection.GetCOMVerbEnumerator(ppEnum);
      }
    }

    private static IEnumerable<IVerbDescription> GetCOMVerbEnumerator(
      IVsUIEnumDataSourceVerbs verbEnumerator)
    {
      foreach (string name in ComUtilities.EnumerableFrom(verbEnumerator))
        yield return (IVerbDescription) new VerbDescription(name);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    private void SubscribeToVisualElementEvents()
    {
      Window visualElement = this.parameters.visualElement as Window;
      if (visualElement == null)
        return;
      visualElement.Closed += new EventHandler(this.visualWindow_Closed);
    }

    private void UnsubscribeFromVisualElementEvents()
    {
      if (this.parameters == null)
        return;
      Window visualElement = this.parameters.visualElement as Window;
      if (visualElement == null)
        return;
      visualElement.Closed -= new EventHandler(this.visualWindow_Closed);
    }

    private void SubscribeInnerCollectionChangeEvents()
    {
      this.AdviseCollectionChangeEvents((IVsUICollectionChangeEvents) new DataSourceCollection.VsUICollectionChangeEvents(this), out this.collectionChangeEventsCookie);
    }

    private void UnsubscribeInnerCollectionChangeEvents()
    {
      if (this.collectionChangeEventsCookie == 0U)
        return;
      try
      {
        this.UnadviseCollectionChangeEvents(this.collectionChangeEventsCookie);
        this.collectionChangeEventsCookie = 0U;
      }
      catch (Exception ex)
      {
      }
    }

    private void RaiseOnAfterItemAdded(uint nItem)
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIDataSource pVsUIDataSource;
      // ISSUE: reference to a compiler-generated method
      this.innerCollection.GetItem(nItem, out pVsUIDataSource);
      DataSource dataSource = new DataSource(pVsUIDataSource, this.parameters);
      this.items.Insert((int) nItem, (IDataSource) dataSource);
      this.FirePropertyChangedEvent(new PropertyChangedEventArgs("Count"));
      this.FirePropertyChangedEvent(new PropertyChangedEventArgs("Item[]"));
      this.FireCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (object) dataSource, (int) nItem));
    }

    private void RaiseOnAfterItemRemoved(IVsUIDataSource pRemovedItem, uint nItem)
    {
      if ((long) nItem >= (long) this.items.Count)
        throw new ArgumentOutOfRangeException(nameof (nItem));
      IDataSource dataSource = this.RemoveCachedItem((int) nItem) ?? (IDataSource) new DataSource(pRemovedItem, this.parameters);
      using (dataSource as IDisposable)
      {
        this.FirePropertyChangedEvent(new PropertyChangedEventArgs("Count"));
        this.FirePropertyChangedEvent(new PropertyChangedEventArgs("Item[]"));
        this.FireCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (object) dataSource, (int) nItem));
        // ISSUE: reference to a compiler-generated method
        ((IVsUIDataSource) dataSource).Close();
      }
    }

    private void RaiseOnAfterItemReplaced(
      IVsUIDataSource pNewItem,
      IVsUIDataSource pOldItem,
      uint nItem)
    {
      if ((long) nItem >= (long) this.items.Count)
        throw new ArgumentOutOfRangeException(nameof (nItem));
      IDataSource cachedItem = this.GetCachedItem((int) nItem);
      using (cachedItem as IDisposable)
      {
        IDataSource dataSource = (IDataSource) new DataSource(pNewItem, this.parameters);
        this.items[(int) nItem] = dataSource;
        this.FirePropertyChangedEvent(new PropertyChangedEventArgs("Item[]"));
        this.FireCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, (object) dataSource, (object) cachedItem, (int) nItem));
        // ISSUE: reference to a compiler-generated method
        ((IVsUIDataSource) cachedItem).Close();
      }
    }

    private void RaiseOnInvalidateAllItems()
    {
      IList<IDataSource> items = this.items;
      this.InitializeLocalCache();
      this.FirePropertyChangedEvent(new PropertyChangedEventArgs("Count"));
      this.FirePropertyChangedEvent(new PropertyChangedEventArgs("Item[]"));
      this.FireCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      foreach (IDataSource dataSource in (IEnumerable<IDataSource>) items)
      {
        if (dataSource != null)
        {
          using (dataSource as IDisposable)
          {
            // ISSUE: reference to a compiler-generated method
            ((IVsUIDataSource) dataSource).Close();
          }
        }
      }
    }

    private void FirePropertyChangedEvent(PropertyChangedEventArgs args)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, args);
    }

    private void FireCollectionChangedEvent(NotifyCollectionChangedEventArgs args)
    {
      if (this.CollectionChanged == null)
        return;
      this.CollectionChanged((object) this, args);
    }

    private class VsUICollectionChangeEvents : IVsUICollectionChangeEvents, IVsUIEventSink
    {
      private DataSourceCollection owner;

      public VsUICollectionChangeEvents(DataSourceCollection owner)
      {
        this.owner = owner;
      }

      public int OnAfterItemAdded(IVsUICollection coll, uint nItem)
      {
        this.owner.RaiseOnAfterItemAdded(nItem);
        return 0;
      }

      public int OnAfterItemRemoved(IVsUICollection coll, IVsUIDataSource pRemovedItem, uint nItem)
      {
        this.owner.RaiseOnAfterItemRemoved(pRemovedItem, nItem);
        return 0;
      }

      public int OnAfterItemReplaced(
        IVsUICollection coll,
        IVsUIDataSource pNewItem,
        IVsUIDataSource pOldItem,
        uint nItem)
      {
        this.owner.RaiseOnAfterItemReplaced(pNewItem, pOldItem, nItem);
        return 0;
      }

      public int OnInvalidateAllItems(IVsUICollection coll)
      {
        this.owner.RaiseOnInvalidateAllItems();
        return 0;
      }

      public int Disconnect(IVsUISimpleDataSource pSource)
      {
        this.owner.UnsubscribeInnerCollectionChangeEvents();
        return 0;
      }
    }
  }
}
