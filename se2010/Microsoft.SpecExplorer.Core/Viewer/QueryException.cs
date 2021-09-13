// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.QueryException
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.SpecExplorer.Viewer
{
  [Serializable]
  public class QueryException : Exception
  {
    public QueryException()
    {
    }

    protected QueryException(SerializationInfo info, StreamingContext sctx)
      : base(info, sctx)
    {
    }

    public QueryException(string msg)
      : base(msg)
    {
    }

    public QueryException(string msg, Exception e)
      : base(msg, e)
    {
    }
  }
}
