using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.SpecExplorer.Viewer;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.SpecExplorer.VS
{
	[Guid("7E4F0150-06DA-4084-8F5C-A3A76A70E7D7")]
	public class StepBrowserToolWindow : ToolWindowPane
	{
		private StepBrowserControl control;

		public override IWin32Window Window
		{
			get
			{
				return control;
			}
		}

		public StepBrowserToolWindow()
	  : base((System.IServiceProvider)null)
		{
			this.Caption = Microsoft.SpecExplorer.Resources.StepBrowserToolWindow;
			this.control = new StepBrowserControl();
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		public void LoadSteps(IEnumerable<BrowserEdge> browserEdges, string label)
		{
			this.Caption = string.Format("{0} - {1}", (object)Microsoft.SpecExplorer.Resources.StepBrowserToolWindow, (object)label);
			this.control.LoadSteps(browserEdges);
		}
	}
}
