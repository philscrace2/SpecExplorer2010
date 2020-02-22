// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.SummaryDocumentControl
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.VisualStudio.Shell;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Microsoft.SpecExplorer.VS
{
  public class SummaryDocumentControl : UserControl
  {
    private WebBrowser webBrowser1;
    private SpecExplorerPackage package;

    internal SummaryDocumentControl(SpecExplorerPackage package, string filePath)
    {
      this.InitializeComponent();
      this.package = package;
      using (StreamReader streamReader = new StreamReader(filePath))
      {
        try
        {
          this.webBrowser1.DocumentText = streamReader.ReadToEnd();
        }
        catch (IOException ex)
        {
          this.package.DecisionDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, string.Format("Invalid summary file: {0}", (object) filePath), (MessageButton) 0);
        }
      }
    }

    private void InitializeComponent()
    {
      this.webBrowser1 = new WebBrowser();
      this.SuspendLayout();
      this.webBrowser1.Dock = DockStyle.Fill;
      this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
      this.webBrowser1.WebBrowserShortcutsEnabled = false;
      this.webBrowser1.Location = new Point(0, 0);
      this.webBrowser1.MinimumSize = new Size(20, 20);
      this.webBrowser1.Name = "webBrowser1";
      this.webBrowser1.Size = new Size(629, 442);
      this.webBrowser1.TabIndex = 0;
      this.webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
      this.Controls.Add((Control) this.webBrowser1);
      this.Name = "Changed in souce by phil scrace";
      this.Size = new Size(629, 442);
      this.ResumeLayout(false);
    }

    private void webBrowser1_DocumentCompleted(
      object sender,
      WebBrowserDocumentCompletedEventArgs e)
    {
      this.webBrowser1.Navigating += new WebBrowserNavigatingEventHandler(this.webBrowser1_Navigating);
    }

    private string GetFilePath(string filePath)
    {
      if (string.IsNullOrEmpty(filePath))
        return string.Empty;
      string str = "about:_file:///";
      if (filePath.StartsWith(str, StringComparison.InvariantCultureIgnoreCase))
        filePath = filePath.Substring(str.Length);
      try
      {
        return new Uri(filePath).LocalPath;
      }
      catch (UriFormatException ex)
      {
        return string.Empty;
      }
    }

    private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
    {
      string filePath = this.GetFilePath(e.Url.ToString());
      if (File.Exists(filePath))
        VsShellUtilities.OpenDocument((System.IServiceProvider) this.package, filePath);
      else
        this.package.DecisionDialog(Microsoft.SpecExplorer.Resources.SpecExplorer, string.Format("\"{0}\" does not exist.", (object) filePath), (MessageButton) 0);
      e.Cancel = true;
    }
  }
}
