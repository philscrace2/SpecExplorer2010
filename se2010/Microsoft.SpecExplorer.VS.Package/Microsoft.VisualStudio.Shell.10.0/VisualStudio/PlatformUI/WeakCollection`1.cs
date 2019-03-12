// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.WeakCollection`1
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class WeakCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    where T : class
  {
    private List<WeakReference> innerList = new List<WeakReference>();

    public void Add(T item)
    {
      this.innerList.Add(new WeakReference((object) item));
    }

    public void Clear()
    {
      this.innerList.Clear();
    }

    public bool Contains(T item)
    {
      return this.GetStrongList().Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      this.GetStrongList().CopyTo(array, arrayIndex);
    }

    public T Find(Predicate<T> match)
    {
      return this.GetStrongList().Find(match);
    }

    public bool Remove(T item)
    {
      for (int index = 0; index < this.innerList.Count; ++index)
      {
        if (object.ReferenceEquals(this.innerList[index].Target, (object) item))
        {
          this.innerList.RemoveAt(index);
          return true;
        }
      }
      return false;
    }

    public int Count
    {
      get
      {
        return this.innerList.Count;
      }
    }

    bool ICollection<T>.IsReadOnly
    {
      get
      {
        return ((ICollection<T>) this.innerList).IsReadOnly;
      }
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return (IEnumerator<T>) this.GetStrongList().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable) this.GetStrongList()).GetEnumerator();
    }

    private List<T> GetStrongList()
    {
      List<T> objList = new List<T>(this.innerList.Count);
      for (int index = 0; index < this.innerList.Count; ++index)
      {
        T target = this.innerList[index].Target as T;
        if ((object) target == null)
        {
          this.innerList.RemoveAt(index);
          --index;
        }
        else
          objList.Add(target);
      }
      return objList;
    }
  }
}
