// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.UIDataSourceCollection
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class UIDataSourceCollection : UISimpleDataSource, IVsUICollection, IVsUISimpleDataSource, IVsUIDispatch, IUIDispatch, IList<IVsUIDataSource>, ICollection<IVsUIDataSource>, IEnumerable<IVsUIDataSource>, IEnumerable
  {
    private List<IVsUIDataSource> dataCollection = new List<IVsUIDataSource>();
    private VsUICookieTable<IVsUICollectionChangeEvents> collectionEventSubscribers = new VsUICookieTable<IVsUICollectionChangeEvents>();

    protected IList<IVsUIDataSource> DataCollection
    {
      get
      {
        return (IList<IVsUIDataSource>) this.dataCollection;
      }
    }

    public override int Close()
    {
      // ISSUE: reference to a compiler-generated method
      this.collectionEventSubscribers.ForEach((CookieTableCallback<uint, IVsUICollectionChangeEvents>) ((cookie, subscriber) => subscriber.Disconnect((IVsUISimpleDataSource) this)));
      this.collectionEventSubscribers.Clear();
      lock (this.DataCollection)
      {
        foreach (IVsUIDataSource data in (IEnumerable<IVsUIDataSource>) this.DataCollection)
        {
          // ISSUE: reference to a compiler-generated method
          data.Close();
        }
      }
      return 0;
    }

    public int get_Count(out uint pnCount)
    {
      pnCount = this.Count;
      return 0;
    }

    public int GetItem(uint nItem, out IVsUIDataSource pVsUIDataSource)
    {
      lock (this.DataCollection)
      {
        if (nItem < 0U || (long) nItem >= (long) this.DataCollection.Count)
          throw new ArgumentOutOfRangeException(nameof (nItem));
        pVsUIDataSource = this.DataCollection[(int) nItem];
      }
      return 0;
    }

    public int AdviseCollectionChangeEvents(IVsUICollectionChangeEvents pAdvise, out uint pCookie)
    {
      if (pAdvise == null)
        throw new ArgumentNullException(nameof (pAdvise));
      pCookie = this.collectionEventSubscribers.Insert(pAdvise);
      return 0;
    }

    public int UnadviseCollectionChangeEvents(uint cookie)
    {
      if (!this.collectionEventSubscribers.Remove(cookie))
        throw new ArgumentException(Resources.Error_InvalidCookieValue, nameof (cookie));
      return 0;
    }

    public int IndexOf(IVsUIDataSource item)
    {
      return this.DataCollection.IndexOf(item);
    }

    public virtual void Insert(int index, IVsUIDataSource item)
    {
      throw new NotSupportedException();
    }

    public virtual void RemoveAt(int index)
    {
      throw new NotSupportedException();
    }

    public virtual IVsUIDataSource this[int index]
    {
      get
      {
        return this.DataCollection[index];
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public virtual void Add(IVsUIDataSource item)
    {
      throw new NotSupportedException();
    }

    public virtual void Clear()
    {
      throw new NotSupportedException();
    }

    public bool Contains(IVsUIDataSource item)
    {
      return this.DataCollection.Contains(item);
    }

    public void CopyTo(IVsUIDataSource[] array, int arrayIndex)
    {
      this.DataCollection.CopyTo(array, arrayIndex);
    }

    int ICollection<IVsUIDataSource>.Count
    {
      get
      {
        return this.DataCollection.Count;
      }
    }

    public virtual bool IsReadOnly
    {
      get
      {
        return true;
      }
    }

    public virtual bool Remove(IVsUIDataSource item)
    {
      throw new NotSupportedException();
    }

    public IEnumerator<IVsUIDataSource> GetEnumerator()
    {
      return this.DataCollection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.DataCollection.GetEnumerator();
    }

    public uint Count
    {
      get
      {
        lock (this.DataCollection)
          return (uint) this.DataCollection.Count;
      }
    }

    public virtual uint AddItem(IVsUIDataSource item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof (item));
      lock (this.DataCollection)
      {
        this.DataCollection.Add(item);
        return (uint) (this.DataCollection.Count - 1);
      }
    }

    protected void FireOnAfterItemAdded(uint itemIndex)
    {
      // ISSUE: reference to a compiler-generated method
      this.collectionEventSubscribers.ForEach((CookieTableCallback<uint, IVsUICollectionChangeEvents>) ((cookie, subscriber) => subscriber.OnAfterItemAdded((IVsUICollection) this, itemIndex)));
    }

    protected void FireOnAfterItemRemoved(IVsUIDataSource removedItem, uint itemIndex)
    {
      // ISSUE: reference to a compiler-generated method
      this.collectionEventSubscribers.ForEach((CookieTableCallback<uint, IVsUICollectionChangeEvents>) ((cookie, subscriber) => subscriber.OnAfterItemRemoved((IVsUICollection) this, removedItem, itemIndex)));
    }

    protected void FireOnAfterItemReplaced(
      IVsUIDataSource newItem,
      IVsUIDataSource oldItem,
      uint itemIndex)
    {
      // ISSUE: reference to a compiler-generated method
      this.collectionEventSubscribers.ForEach((CookieTableCallback<uint, IVsUICollectionChangeEvents>) ((cookie, subscriber) => subscriber.OnAfterItemReplaced((IVsUICollection) this, newItem, oldItem, itemIndex)));
    }

    protected void FireOnInvalidateAllItems()
    {
      // ISSUE: reference to a compiler-generated method
      this.collectionEventSubscribers.ForEach((CookieTableCallback<uint, IVsUICollectionChangeEvents>) ((cookie, subscriber) => subscriber.OnInvalidateAllItems((IVsUICollection) this)));
    }
  }
}
