using System;
using System.Windows.Forms;

namespace Microsoft.SpecExplorer
{
	public interface IHost
	{
		bool Logging { get; }

		VerbosityLevel Verbosity { get; }

		IWin32Window DialogOwner { get; }

		Exception FatalError(string message, params Exception[] exceptions);

		void RecoverFromFatalError(Exception exception);

		void RunProtected(ProtectedAction action);

		EventHandler Protect(EventHandler handler);

		void Log(string line);

		void ProgressMessage(VerbosityLevel verbosity, string message);

		void DiagMessage(DiagnosisKind kind, string message, object location);

		void NotificationDialog(string title, string message);

		MessageResult DecisionDialog(string title, string message, MessageButton messageButton);

		DialogResult ModalDialog(Form form);

		object GetService(Type type);

		bool TryFindLocation(MemberInfo member, out TextLocation location);

		bool TryGetExtensionData(string key, object inputValue, out object outputValue);

		void NavigateTo(string fileName, int line, int column);
	}
}
