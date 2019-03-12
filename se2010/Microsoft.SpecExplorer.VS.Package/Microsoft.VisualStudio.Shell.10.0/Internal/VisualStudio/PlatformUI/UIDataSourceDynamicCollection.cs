// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.UIDataSourceDynamicCollection
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class UIDataSourceDynamicCollection : UIDataSourceCollection, IVsUIDynamicCollection, IVsUICollection, IVsUISimpleDataSource, IVsUIDispatch
  {
    public override void Insert(int index, IVsUIDataSource item)
    {
      this.InsertItem((uint) index, item);
    }

    public override void RemoveAt(int index)
    {
      this.RemoveItem((uint) index);
    }

    public override IVsUIDataSource this[int index]
    {
      get
      {
        return this.DataCollection[index];
      }
      set
      {
        this.ReplaceItem((uint) index, value);
      }
    }

    public override bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public override void Add(IVsUIDataSource item)
    {
      int num = (int) this.AddItem(item);
    }

    public override bool Remove(IVsUIDataSource item)
    {
      if (!this.DataCollection.Contains(item))
        return false;
      this.RemoveItem((uint) this.DataCollection.IndexOf(item));
      return true;
    }

    public override void Clear()
    {
      this.ClearItems();
    }

    public override uint AddItem(IVsUIDataSource item)
    {
      uint pIndex = 0;
      this.AddItem(item, out pIndex);
      return pIndex;
    }

    public virtual int AddItem(IVsUIDataSource pItem, out uint pIndex)
    {
      if (pItem == null)
        throw new ArgumentNullException(nameof (pItem));
      lock (this.DataCollection)
      {
        this.DataCollection.Add(pItem);
        pIndex = (uint) (this.DataCollection.Count - 1);
        this.FireOnAfterItemAdded(pIndex);
      }
      return 0;
    }

    public virtual int ClearItems()
    {
      lock (this.DataCollection)
      {
        this.DataCollection.Clear();
        this.FireOnInvalidateAllItems();
      }
      return 0;
    }

    public virtual int InsertCollection(uint nIndex, IVsUICollection pCollection)
    {
      if (pCollection == null)
        throw new ArgumentNullException(nameof (pCollection));
      if (pCollection.Equals((object) this))
        throw new InvalidOperationException(Resources.Error_CannotInsertCollectionIntoItself);
      lock (this.DataCollection)
      {
        if (nIndex < 0U || (long) nIndex >= (long) this.DataCollection.Count)
          throw new ArgumentOutOfRangeException(nameof (nIndex));
        uint pnCount = 0;
        // ISSUE: reference to a compiler-generated method
        pCollection.get_Count(out pnCount);
        for (uint nItem = 0; nItem < pnCount; ++nItem)
        {
          // ISSUE: variable of a compiler-generated type
          IVsUIDataSource pVsUIDataSource;
          // ISSUE: reference to a compiler-generated method
          pCollection.GetItem(nItem, out pVsUIDataSource);
          this.DataCollection.Insert((int) nIndex, pVsUIDataSource);
          this.FireOnAfterItemAdded(nIndex);
          ++nIndex;
        }
      }
      return 0;
    }

    public virtual int InsertItem(uint nIndex, IVsUIDataSource pItem)
    {
      if (pItem == null)
        throw new ArgumentNullException(nameof (pItem));
      lock (this.DataCollection)
      {
        if (nIndex < 0U || (long) nIndex > (long) this.DataCollection.Count)
          throw new ArgumentOutOfRangeException(nameof (nIndex));
        this.DataCollection.Insert((int) nIndex, pItem);
        this.FireOnAfterItemAdded(nIndex);
      }
      return 0;
    }

    public virtual int RemoveItem(uint nIndex)
    {
      lock (this.DataCollection)
      {
        if (nIndex < 0U || (long) nIndex >= (long) this.DataCollection.Count)
          throw new ArgumentOutOfRangeException(nameof (nIndex));
        // ISSUE: variable of a compiler-generated type
        IVsUIDataSource data = this.DataCollection[(int) nIndex];
        this.DataCollection.RemoveAt((int) nIndex);
        this.FireOnAfterItemRemoved(data, nIndex);
      }
      return 0;
    }

    public virtual int ReplaceItem(uint nIndex, IVsUIDataSource pItem)
    {
      lock (this.DataCollection)
      {
        if (nIndex < 0U || (long) nIndex >= (long) this.DataCollection.Count)
          throw new ArgumentOutOfRangeException(nameof (nIndex));
        if (pItem == null)
          throw new ArgumentNullException(nameof (pItem));
        // ISSUE: variable of a compiler-generated type
        IVsUIDataSource data = this.DataCollection[(int) nIndex];
        this.DataCollection[(int) nIndex] = pItem;
        this.FireOnAfterItemReplaced(pItem, data, nIndex);
      }
      return 0;
    }

    public override int Close()
    {
      return base.Close();
    }
  }
}
