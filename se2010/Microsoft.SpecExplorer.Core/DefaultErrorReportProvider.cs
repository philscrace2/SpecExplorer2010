using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	internal class DefaultErrorReportProvider : ComponentBase, IErrorReportProvider
	{
		private EventAdapter eventAdapter;

		internal DefaultErrorReportProvider(EventAdapter eventAdapter)
		{
			this.eventAdapter = eventAdapter;
		}

		public void ReportError(string message)
		{
			eventAdapter.DiagMessage(DiagnosisKind.Error, message, null);
		}

		public void ReportWarning(string message)
		{
			eventAdapter.DiagMessage(DiagnosisKind.Warning, message, null);
		}

		public void ReportHint(string message)
		{
			eventAdapter.DiagMessage(DiagnosisKind.Hint, message, null);
		}
	}
}
