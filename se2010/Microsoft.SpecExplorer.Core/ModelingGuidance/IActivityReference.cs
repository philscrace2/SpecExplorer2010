using System.ComponentModel;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	public interface IActivityReference : INotifyPropertyChanged
	{
		IActivity Activity { get; }

		bool IsCompleted { get; set; }

		bool IsSelected { get; set; }

		int Index { get; }

		bool IsOptional { get; }
	}
}
