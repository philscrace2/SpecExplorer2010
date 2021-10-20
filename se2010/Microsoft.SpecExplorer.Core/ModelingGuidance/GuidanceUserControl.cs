using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public class GuidanceUserControl : UserControl, IComponentConnector, IStyleConnector
	{
		internal GuidanceUserControl userControl;

		internal GuidanceControlModel viewerModel;

		internal ComboBox guidanceComboBox;

		internal ListBox activitiesPanel;

		private bool _contentLoaded;

		public GuidanceControlModel ControlModel
		{
			get
			{
				return viewerModel;
			}
		}

		public GuidanceUserControl()
		{
			InitializeComponent();
			RoutedEventHandler value = delegate
			{
				PresentationSource presentationSource = PresentationSource.FromVisual(this);
				if (presentationSource != null)
				{
					HwndTarget hwndTarget = presentationSource.CompositionTarget as HwndTarget;
					if (hwndTarget != null)
					{
						hwndTarget.RenderMode = RenderMode.SoftwareOnly;
					}
				}
			};
			base.Loaded += value;
		}

		private void FlowDocumentLoaded(object sender, RoutedEventArgs evtArgs)
		{
			FlowDocument flowDocument = sender as FlowDocument;
			if (flowDocument.Tag != null)
			{
				using (Stream stream = new MemoryStream(new ASCIIEncoding().GetBytes(flowDocument.Tag.ToString())))
				{
					new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd).Load(stream, DataFormats.Xaml);
				}
			}
		}

		[DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri resourceLocator = new Uri("/Microsoft.SpecExplorer.Core;V2.2.0.0;component/modelingguidance/guidanceusercontrol.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		[DebuggerNonUserCode]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				userControl = (GuidanceUserControl)target;
				break;
			case 2:
				viewerModel = (GuidanceControlModel)target;
				break;
			case 4:
				guidanceComboBox = (ComboBox)target;
				break;
			case 5:
				activitiesPanel = (ListBox)target;
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerNonUserCode]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 3)
			{
				((FlowDocument)target).Loaded += FlowDocumentLoaded;
			}
		}
	}
}
