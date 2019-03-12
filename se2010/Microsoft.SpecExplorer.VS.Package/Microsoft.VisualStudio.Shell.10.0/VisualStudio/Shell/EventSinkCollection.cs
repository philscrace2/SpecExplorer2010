// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.EventSinkCollection
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public class EventSinkCollection : IEnumerable
  {
    private ArrayList map;

    private ArrayList GetMap()
    {
      if (this.map == null)
        this.map = new ArrayList();
      return this.map;
    }

    public int Count
    {
      get
      {
        if (this.map != null)
          return this.map.Count;
        return 0;
      }
    }

    public uint Add(object o)
    {
      if (o == null)
        throw new ArgumentNullException(nameof (o));
      int index = 0;
      for (int count = this.GetMap().Count; index < count; ++index)
      {
        if (this.map[index] == null)
        {
          this.map[index] = o;
          return (uint) (index + 1);
        }
      }
      this.map.Add(o);
      return (uint) this.map.Count;
    }

    public void Remove(object obj)
    {
      if (obj == null)
        throw new ArgumentNullException(nameof (obj));
      if (this.map != null)
      {
        int index = 0;
        for (int count = this.map.Count; index < count; ++index)
        {
          if (this.map[index] == obj)
          {
            this.map[index] = (object) null;
            if (index != count - 1)
              return;
            while (index > 0 && this.map[index - 1] == null)
              --index;
            this.map.RemoveRange(index, count - index);
            return;
          }
        }
      }
      throw new ArgumentOutOfRangeException(nameof (obj));
    }

    public void RemoveAt(uint cookie)
    {
      if (this.map == null)
        return;
      this.map[(int) cookie - 1] = (object) null;
    }

    public void SetAt(uint cookie, object value)
    {
      this.GetMap()[(int) cookie - 1] = value;
    }

    public object this[uint cookie]
    {
      get
      {
        if (this.map == null || cookie <= 0U || (long) cookie > (long) this.map.Count)
          return (object) null;
        return this.map[(int) cookie - 1];
      }
      set
      {
        this.GetMap()[(int) cookie - 1] = value;
      }
    }

    public void Clear()
    {
      if (this.map == null)
        return;
      this.map.Clear();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new EventSinkCollection.EventSinkEnumerator(this.map);
    }

    internal class EventSinkEnumerator : IEnumerator
    {
      private ArrayList map;
      private int pos;

      public EventSinkEnumerator(ArrayList map)
      {
        this.map = map;
        this.pos = -1;
      }

      object IEnumerator.Current
      {
        get
        {
          if (this.map == null || this.pos < 0 || this.pos >= this.map.Count)
            return (object) null;
          return this.map[this.pos];
        }
      }

      bool IEnumerator.MoveNext()
      {
        if (this.map == null)
          return false;
        int count = this.map.Count;
        if (this.pos >= count)
          return false;
        ++this.pos;
        while (this.pos < count && this.map[this.pos] == null)
          ++this.pos;
        return this.pos < count;
      }

      void IEnumerator.Reset()
      {
        this.pos = -1;
      }
    }
  }
}
