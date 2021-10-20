namespace Microsoft.SpecExplorer.VSService
{
	public interface IVSServiceProvider
	{
		bool TryGetExtensionData(string key, object inputValue, out object outputValue);
	}
}
