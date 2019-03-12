// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.MruCache`2
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [DebuggerDisplay("Count = {Count}, Capacity = {Capacity}, IsFull = {IsFull}")]
  internal class MruCache<TKey, TValue>
  {
    private Dictionary<TKey, MruCache<TKey, TValue>.Entry> map;
    private LinkedList<TKey> mruList;

    public MruCache(int capacity)
    {
      if (capacity < 1)
        throw new ArgumentOutOfRangeException(Resources.Error_CapacityMustBeGreaterThanZero);
      this.Capacity = capacity;
      this.map = new Dictionary<TKey, MruCache<TKey, TValue>.Entry>(capacity);
      this.mruList = new LinkedList<TKey>();
    }

    public int Capacity { get; private set; }

    public int Count
    {
      get
      {
        return this.map.Count;
      }
    }

    public void Clear()
    {
      this.map.Clear();
      this.mruList.Clear();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      if ((object) key == null)
        throw new ArgumentNullException(nameof (key), Resources.Error_KeyCannotBeNull);
      MruCache<TKey, TValue>.Entry entry;
      if (!this.map.TryGetValue(key, out entry))
      {
        this.OnCacheMiss(key);
        value = default (TValue);
        return false;
      }
      if (this.mruList.First != entry.node)
      {
        this.mruList.Remove(entry.node);
        this.mruList.AddFirst(entry.node);
      }
      value = entry.value;
      return true;
    }

    public void Add(TKey key, TValue value)
    {
      if ((object) key == null)
        throw new ArgumentNullException(nameof (key), Resources.Error_KeyCannotBeNull);
      if (this.IsFull)
        this.DiscardOldest();
      MruCache<TKey, TValue>.Entry entry = new MruCache<TKey, TValue>.Entry()
      {
        value = value,
        node = this.mruList.AddFirst(key)
      };
      try
      {
        this.map.Add(key, entry);
      }
      catch
      {
        this.mruList.RemoveFirst();
        throw;
      }
    }

    protected virtual void OnCacheMiss(TKey key)
    {
    }

    protected virtual void OnDiscard(TKey key)
    {
    }

    private void DiscardOldest()
    {
      LinkedListNode<TKey> last = this.mruList.Last;
      this.OnDiscard(last.Value);
      this.map.Remove(last.Value);
      this.mruList.RemoveLast();
    }

    [Conditional("DEBUG")]
    private void ConsistencyCheck()
    {
      for (LinkedListNode<TKey> linkedListNode = this.mruList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
      {
        MruCache<TKey, TValue>.Entry entry;
        if (!this.map.TryGetValue(linkedListNode.Value, out entry))
          throw new MruCache<TKey, TValue>.ConsistencyCheckException("Key in linked list was not found in the dictionary");
        if (!object.ReferenceEquals((object) entry.node, (object) linkedListNode))
          throw new MruCache<TKey, TValue>.ConsistencyCheckException("The value in the dictionary did not point back to the corresponding linked list node");
      }
      foreach (KeyValuePair<TKey, MruCache<TKey, TValue>.Entry> keyValuePair in this.map)
      {
        if (!object.ReferenceEquals((object) keyValuePair.Value.node, (object) this.mruList.Find(keyValuePair.Key)))
          throw new MruCache<TKey, TValue>.ConsistencyCheckException("One of the values in the dictionary is not in the linked list");
      }
    }

    private bool IsFull
    {
      get
      {
        return this.Count == this.Capacity;
      }
    }

    [Serializable]
    private class ConsistencyCheckException : Exception
    {
      public ConsistencyCheckException(string message)
        : base(message)
      {
      }

      protected ConsistencyCheckException(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
      }
    }

    private struct Entry
    {
      public TValue value;
      public LinkedListNode<TKey> node;
    }
  }
}
