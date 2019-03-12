// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.EnumerableComCollection`2
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public abstract class EnumerableComCollection<TComEnumerator, TEnumerated> : IEnumerable<TEnumerated>, IEnumerable, IComEnumeratorRelay<TComEnumerator, TEnumerated>
  {
    protected const int DefaultCacheSize = 8;
    private readonly TComEnumerator _wrappedComEnumerator;
    private readonly int _cacheSize;

    protected EnumerableComCollection(TComEnumerator enumerator)
      : this(enumerator, 8)
    {
    }

    protected EnumerableComCollection(TComEnumerator enumerator, int cacheSize)
    {
      if ((object) enumerator == null)
        throw new ArgumentNullException(nameof (enumerator));
      if (cacheSize <= 0)
        throw new ArgumentOutOfRangeException(nameof (cacheSize));
      this._cacheSize = cacheSize;
      this._wrappedComEnumerator = this.Clone(enumerator);
    }

    private IEnumerator<TEnumerated> CreateEnumerator()
    {
      return (IEnumerator<TEnumerated>) new EnumerableComCollection<TComEnumerator, TEnumerated>.Enumerator((IComEnumeratorRelay<TComEnumerator, TEnumerated>) this, this.Clone(this._wrappedComEnumerator), this._cacheSize);
    }

    private TComEnumerator Clone(TComEnumerator original)
    {
      TComEnumerator clone;
      ErrorHandler.ThrowOnFailure(this.Clone(original, out clone));
      return clone;
    }

    public abstract int Clone(TComEnumerator enumerator, out TComEnumerator clone);

    public abstract int NextItems(
      TComEnumerator enumerator,
      uint count,
      TEnumerated[] items,
      out uint fetched);

    public abstract int Reset(TComEnumerator enumerator);

    public abstract int Skip(TComEnumerator enumerator, uint count);

    public IEnumerator<TEnumerated> GetEnumerator()
    {
      return this.CreateEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.CreateEnumerator();
    }

    private class Enumerator : IEnumerator<TEnumerated>, IDisposable, IEnumerator
    {
      private readonly IComEnumeratorRelay<TComEnumerator, TEnumerated> _relay;
      private readonly TComEnumerator _wrappedComEnumerator;
      private readonly TEnumerated[] _cache;
      private int _currentIndex;
      private int _validCachedItemCount;
      private bool _fetchAgain;

      public Enumerator(
        IComEnumeratorRelay<TComEnumerator, TEnumerated> relay,
        TComEnumerator enumerator,
        int cacheSize)
      {
        if (relay == null)
          throw new ArgumentNullException(nameof (relay));
        if ((object) enumerator == null)
          throw new ArgumentNullException(nameof (enumerator));
        if (cacheSize <= 0)
          throw new ArgumentOutOfRangeException(nameof (cacheSize));
        this._relay = relay;
        this._fetchAgain = true;
        this._wrappedComEnumerator = enumerator;
        this._cache = new TEnumerated[cacheSize];
        this._currentIndex = cacheSize;
        this._validCachedItemCount = 0;
      }

      public void Dispose()
      {
        GC.SuppressFinalize((object) this);
      }

      public TEnumerated Current
      {
        get
        {
          return this._cache[this._currentIndex];
        }
      }

      object IEnumerator.Current
      {
        get
        {
          return (object) this.Current;
        }
      }

      public bool MoveNext()
      {
        if (!this.MoreItems)
          return false;
        if (this.NeedToFetch)
          this.Fetch();
        else if (this.NextIndex < this.CacheSize)
          ++this._currentIndex;
        return this.MoreItems;
      }

      public void Reset()
      {
        ErrorHandler.ThrowOnFailure(this._relay.Reset(this._wrappedComEnumerator));
        this.ResetCache(0);
        this._fetchAgain = true;
      }

      private bool MoreItems
      {
        get
        {
          if (!this._fetchAgain)
            return this.NextIndex <= this._validCachedItemCount;
          return true;
        }
      }

      private bool NeedToFetch
      {
        get
        {
          if (this._fetchAgain)
            return this.NextIndex >= this._validCachedItemCount;
          return false;
        }
      }

      private int NextIndex
      {
        get
        {
          return this._currentIndex + 1;
        }
      }

      private int CacheSize
      {
        get
        {
          return this._cache.Length;
        }
      }

      private bool Fetch()
      {
        uint fetched = 0;
        ErrorHandler.ThrowOnFailure(this._relay.NextItems(this._wrappedComEnumerator, (uint) this.CacheSize, this._cache, out fetched));
        this.ResetCache((int) fetched);
        this._fetchAgain = this._validCachedItemCount == this.CacheSize;
        return this._validCachedItemCount > 0;
      }

      private void ResetCache(int validCachedItemCount)
      {
        this._currentIndex = 0;
        this._validCachedItemCount = validCachedItemCount;
        for (int validCachedItemCount1 = this._validCachedItemCount; validCachedItemCount1 < this.CacheSize; ++validCachedItemCount1)
          this._cache[validCachedItemCount1] = default (TEnumerated);
      }
    }
  }
}
