// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.QueryFactory
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

namespace Microsoft.SpecExplorer.Viewer
{
  internal static class QueryFactory
  {
    internal static IViewQuery GetViewQuery(Query query)
    {
      if (string.IsNullOrEmpty(query.Param))
        return (IViewQuery) null;
      if (query.Type == QueryType.Probe)
        return (IViewQuery) new ProbeQuery(query.Param);
      return query.Type == QueryType.Probe ? (IViewQuery) new ProbeQuery(query.Param) : (IViewQuery) new ProbeQuery(query.Param);
    }
  }
}
