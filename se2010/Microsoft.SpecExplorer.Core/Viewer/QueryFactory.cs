namespace Microsoft.SpecExplorer.Viewer
{
	internal static class QueryFactory
	{
		internal static IViewQuery GetViewQuery(Query query)
		{
			if (string.IsNullOrEmpty(query.Param))
			{
				return null;
			}
			if (query.Type == QueryType.Probe)
			{
				return new ProbeQuery(query.Param);
			}
			if (query.Type == QueryType.Probe)
			{
				return new ProbeQuery(query.Param);
			}
			return new ProbeQuery(query.Param);
		}
	}
}
