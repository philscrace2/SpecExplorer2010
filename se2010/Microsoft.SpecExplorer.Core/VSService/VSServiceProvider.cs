using Microsoft.Xrt;

namespace Microsoft.SpecExplorer.VSService
{
	public class VSServiceProvider : ComponentBase, IVSServiceProvider
	{
		private ExplorerMediator explorerMediator;

		public VSServiceProvider(ExplorerMediator explorerMediator)
		{
			this.explorerMediator = explorerMediator;
		}

		public bool TryGetExtensionData(string key, object inputValue, out object outputValue)
		{
			return explorerMediator.TryGetExtensionData(key, inputValue, out outputValue);
		}
	}
}
