using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	public class ExplorerMediator : DisposableMarshalByRefObject
	{
		public Session session;

		public string InstallDir
		{
			get
			{
				return session.ConfigurationDir;
			}
		}

		internal ExplorerMediator(Session session)
		{
			this.session = session;
		}

		public bool TryFindLocation(MemberInfo member, out TextLocation location)
		{
			return session.Host.TryFindLocation(member, out location);
		}

		public bool TryGetExtensionData(string key, object inputValue, out object outputValue)
		{
			return session.Host.TryGetExtensionData(key, inputValue, out outputValue);
		}

		public void DiagMessage(DiagnosisKind kind, string message, object location)
		{
			session.Host.DiagMessage(kind, message, location);
		}
	}
}
