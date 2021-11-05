using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.SpecExplorer.Viewer;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.SpecExplorer.VS
{
	[Guid("1079EAE0-5880-4dc0-88FF-139EDF582BCA")]
	public class StateBrowserToolWindow : ToolWindowPane
	{
		private ExplorationStateView control;

		public override IWin32Window Window
		{
			get
			{
				return control;
			}
		}

		public StateBrowserToolWindow()
			: base((IServiceProvider)null)
		{
			control = new ExplorationStateView();
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		public void SetHost(IHost host)
		{
			control.Host = host;
		}

		public void LoadStates(
	  string fileName,
	  IEnumerable<Microsoft.SpecExplorer.ObjectModel.State> states,
	  bool shouldDisplayLeftTree,
	  string label)
		{
			this.Caption = string.Format("{0} - {1}", (object)Microsoft.SpecExplorer.Resources.StatesBrowserToolWindowTitle, (object)label);
			((Control)this.control).SuspendLayout();
			this.control.LoadStates(fileName, states, shouldDisplayLeftTree);
			((Control)this.control).ResumeLayout();
		}
	}
}
