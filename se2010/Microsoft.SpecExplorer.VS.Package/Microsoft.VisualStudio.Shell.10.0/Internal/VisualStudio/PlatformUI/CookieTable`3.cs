// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.CookieTable`3
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class CookieTable<TCookie, TCookieTraits, TValue>
    where TCookie : IComparable<TCookie>
    where TCookieTraits : CookieTraits<TCookie>, new()
  {
    private TCookieTraits _traits = new TCookieTraits();
    private IDictionary<TCookie, TValue> _table = (IDictionary<TCookie, TValue>) new Dictionary<TCookie, TValue>();
    private CookieTable<TCookie, TCookieTraits, TValue>.PendingMods _pendingMods = new CookieTable<TCookie, TCookieTraits, TValue>.PendingMods();
    private object _syncLock = new object();
    private TCookie _currentCookie;
    private uint _lockCount;

    public CookieTable()
    {
      this._currentCookie = this._traits.InvalidCookie;
    }

    private TCookie NextCookie
    {
      get
      {
        lock (this._syncLock)
        {
          if (this._table.Count + this._pendingMods._pendingInsert.Count == (int) this._traits.UniqueCookies)
            throw new ApplicationException(Resources.Error_CookieTable_NoMoreCookies);
          TCookie nextCookie;
          do
          {
            nextCookie = this._traits.GetNextCookie(this._currentCookie);
            this._currentCookie = nextCookie;
          }
          while (this._table.ContainsKey(nextCookie) || this._pendingMods._pendingInsert.ContainsKey(nextCookie));
          return nextCookie;
        }
      }
    }

    public TCookie Insert(TValue value)
    {
      lock (this._syncLock)
      {
        using (new CookieTable<TCookie, TCookieTraits, TValue>.CookieTableLock(this))
        {
          TCookie nextCookie = this.NextCookie;
          this._pendingMods._pendingInsert.Add(nextCookie, value);
          return nextCookie;
        }
      }
    }

    public bool Remove(TCookie cookie)
    {
      lock (this._syncLock)
      {
        bool flag = false;
        using (new CookieTable<TCookie, TCookieTraits, TValue>.CookieTableLock(this))
        {
          if (this._table.ContainsKey(cookie) && !this.IsPendingDelete(cookie))
          {
            this._pendingMods._pendingDelete.Add(cookie);
            flag = true;
          }
          if (!flag)
          {
            if (this._pendingMods._pendingInsert.ContainsKey(cookie))
            {
              this._pendingMods._pendingInsert.Remove(cookie);
              flag = true;
            }
          }
        }
        return flag;
      }
    }

    public void Clear()
    {
      lock (this._syncLock)
      {
        using (new CookieTable<TCookie, TCookieTraits, TValue>.CookieTableLock(this))
        {
          this._pendingMods.Reset();
          this._pendingMods._pendingClear = true;
        }
      }
    }

    public void ForEach(CookieTableCallback<TCookie, TValue> callback, bool skipRemoved)
    {
      lock (this._syncLock)
      {
        using (new CookieTable<TCookie, TCookieTraits, TValue>.CookieTableLock(this))
        {
          foreach (KeyValuePair<TCookie, TValue> keyValuePair in (IEnumerable<KeyValuePair<TCookie, TValue>>) this._table)
          {
            if (!this.IsPendingDelete(keyValuePair.Key) || !skipRemoved)
              callback(keyValuePair.Key, keyValuePair.Value);
          }
        }
      }
    }

    public void ForEach(CookieTableCallback<TCookie, TValue> callback)
    {
      this.ForEach(callback, true);
    }

    private bool IsPendingDelete(TCookie cookie)
    {
      if (this._pendingMods._pendingClear)
        return true;
      if (this._pendingMods._pendingDelete.Count > 0)
        return this._pendingMods._pendingDelete.Contains(cookie);
      return false;
    }

    public bool TryGetValue(TCookie cookie, out TValue value)
    {
      lock (this._syncLock)
      {
        bool flag = this._table.TryGetValue(cookie, out value);
        if (flag && this.IsLocked && this.IsPendingDelete(cookie))
          flag = false;
        return flag;
      }
    }

    public uint Size
    {
      get
      {
        return (uint) this._table.Count;
      }
    }

    public uint PendingSize
    {
      get
      {
        lock (this._syncLock)
        {
          uint num = this.Size;
          if (this.IsLocked)
            num = (!this._pendingMods._pendingClear ? num - (uint) this._pendingMods._pendingDelete.Count : 0U) + (uint) this._pendingMods._pendingInsert.Count;
          return num;
        }
      }
    }

    public uint MaxSize
    {
      get
      {
        return this._traits.UniqueCookies;
      }
    }

    public void Lock()
    {
      lock (this._syncLock)
        ++this._lockCount;
    }

    public void Unlock()
    {
      lock (this._syncLock)
      {
        if (this._lockCount == 0U)
          throw new InvalidOperationException(Resources.Error_CookieTable_Unlocked);
        --this._lockCount;
        if (this._lockCount != 0U)
          return;
        if (this._pendingMods._pendingClear)
        {
          this._table = (IDictionary<TCookie, TValue>) new Dictionary<TCookie, TValue>();
        }
        else
        {
          foreach (TCookie key in (IEnumerable<TCookie>) this._pendingMods._pendingDelete)
            this._table.Remove(key);
        }
        foreach (KeyValuePair<TCookie, TValue> keyValuePair in (IEnumerable<KeyValuePair<TCookie, TValue>>) this._pendingMods._pendingInsert)
          this._table.Add(keyValuePair);
        this._pendingMods.Reset();
      }
    }

    public bool IsLocked
    {
      get
      {
        return this._lockCount > 0U;
      }
    }

    private class CookieTableLock : IDisposable
    {
      private CookieTable<TCookie, TCookieTraits, TValue> _cookieTable;

      public CookieTableLock(
        CookieTable<TCookie, TCookieTraits, TValue> cookieTable)
      {
        this._cookieTable = cookieTable;
        this._cookieTable.Lock();
      }

      public void Dispose()
      {
        this._cookieTable.Unlock();
        GC.SuppressFinalize((object) this);
      }
    }

    private class PendingMods
    {
      public IDictionary<TCookie, TValue> _pendingInsert = (IDictionary<TCookie, TValue>) new Dictionary<TCookie, TValue>();
      public IList<TCookie> _pendingDelete = (IList<TCookie>) new List<TCookie>();
      public bool _pendingClear;

      public void Reset()
      {
        this._pendingClear = false;
        this._pendingDelete.Clear();
        this._pendingInsert.Clear();
      }
    }
  }
}
