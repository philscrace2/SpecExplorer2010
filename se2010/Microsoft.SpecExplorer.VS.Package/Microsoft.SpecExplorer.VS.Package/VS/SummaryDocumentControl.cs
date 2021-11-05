using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.SpecExplorer.VS
{
	public class SummaryDocumentControl : UserControl
	{
		private WebBrowser webBrowser1;

		private SpecExplorerPackage package;

		internal SummaryDocumentControl(SpecExplorerPackage package, string filePath)
		{
			InitializeComponent();
			this.package = package;
			using (StreamReader streamReader = new StreamReader(filePath))
			{
				try
				{
					webBrowser1.DocumentText = streamReader.ReadToEnd();
				}
				catch (IOException)
				{
					this.package.DecisionDialog(Resources.SpecExplorer, string.Format("Invalid summary file: {0}", filePath), MessageButton.OK);
				}
			}
		}

		private void InitializeComponent()
		{
			webBrowser1 = new System.Windows.Forms.WebBrowser();
			SuspendLayout();
			webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			webBrowser1.IsWebBrowserContextMenuEnabled = false;
			webBrowser1.WebBrowserShortcutsEnabled = false;
			webBrowser1.Location = new System.Drawing.Point(0, 0);
			webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			webBrowser1.Name = "webBrowser1";
			webBrowser1.Size = new System.Drawing.Size(629, 442);
			webBrowser1.TabIndex = 0;
			webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
			base.Controls.Add(webBrowser1);
			base.Name = "SummaryDocumentControl";
			base.Size = new System.Drawing.Size(629, 442);
			ResumeLayout(false);
		}

		private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			webBrowser1.Navigating += webBrowser1_Navigating;
		}

		private string GetFilePath(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return string.Empty;
			}
			string text = "about:_file:///";
			if (filePath.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
			{
				filePath = filePath.Substring(text.Length);
			}
			Uri uri = null;
			try
			{
				uri = new Uri(filePath);
				return uri.LocalPath;
			}
			catch (UriFormatException)
			{
				return string.Empty;
			}
		}

		private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			string filePath = GetFilePath(e.Url.ToString());
			if (File.Exists(filePath))
			{
				VsShellUtilities.OpenDocument((IServiceProvider)(object)package, filePath);
			}
			else
			{
				package.DecisionDialog(Resources.SpecExplorer, string.Format("\"{0}\" does not exist.", filePath), MessageButton.OK);
			}
			e.Cancel = true;
		}
	}
}
