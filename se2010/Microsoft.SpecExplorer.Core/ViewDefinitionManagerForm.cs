using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using Microsoft.SpecExplorer.Properties;
using Microsoft.SpecExplorer.Viewer;

namespace Microsoft.SpecExplorer
{
	public class ViewDefinitionManagerForm : Form
	{
		private const string viewDefinitionExtetionFilter = "View definition file (*.sevu)|*.sevu|All (*.*)|*.*";

		private IHost host;

		private IViewDefinitionManager manager;

		private string fileName;

		private HashSet<IViewDefinition> updatedViewDefinitiions;

		private IContainer components;

		internal ViewDefinitionsControl viewDefinitionsControl;

		private Button New;

		private Button Delete;

		private Button Import;

		private Button Export;

		private Button OK;

		private Button Cancel;

		private Button Apply;

		private GroupBox groupBox1;

		public event EventHandler ShowHelp;

		public ViewDefinitionManagerForm(IHost host, IViewDefinitionManager ivd, string fileName)
		{
			this.host = host;
			this.fileName = fileName;
			manager = ivd;
			InitializeComponent();
			viewDefinitionsControl.Host = host;
			viewDefinitionsControl.ViewDefinitionList = new List<IViewDefinition>(ivd.Views);
			viewDefinitionsControl.IsDirty = false;
			viewDefinitionsControl.CurrentViewDefinition = manager.CurrentView;
			updatedViewDefinitiions = new HashSet<IViewDefinition>();
			InitializeUI();
		}

		private void InitializeUI()
		{
			base.FormClosing += OnClose;
			New.Click += OnNew;
			Delete.Click += OnDelete;
			Import.Click += OnImport;
			Export.Click += OnExport;
			OK.Click += OnOK;
			Cancel.Click += delegate
			{
				Close();
			};
			Apply.Click += OnApply;
			viewDefinitionsControl.propertyGrid.PropertyValueChanged += OnUpdate;
			if (string.IsNullOrEmpty(fileName))
			{
				New.Enabled = false;
				Delete.Enabled = false;
				Import.Enabled = false;
				Export.Enabled = false;
				Apply.Enabled = false;
			}
		}

		private void OnNew(object sender, EventArgs e)
		{
			ViewDefinition viewDefinition = new ViewDefinition();
			string text = "View1";
			HashSet<string> hashSet = new HashSet<string>();
			foreach (ViewDefinition viewDefinition2 in viewDefinitionsControl.ViewDefinitionList)
			{
				hashSet.Add(viewDefinition2.Name);
			}
			int num = 2;
			while (hashSet.Contains(text))
			{
				text = "View" + num;
				num++;
			}
			viewDefinition.Name = text;
			viewDefinitionsControl.Add(viewDefinition);
			viewDefinitionsControl.CurrentViewDefinition = viewDefinition;
			Apply.Enabled = true;
		}

		private void OnDelete(object sender, EventArgs e)
		{
			IViewDefinition currentViewDefinition = viewDefinitionsControl.CurrentViewDefinition;
			if (currentViewDefinition.IsDefault)
			{
				host.NotificationDialog(Resources.SpecExplorer, "Default view cannot be deleted.");
				return;
			}
			viewDefinitionsControl.RemoveCurrentViewDefinition();
			Apply.Enabled = true;
		}

		private void OnApply(object sender, EventArgs e)
		{
			ApplyChange();
		}

		private bool ApplyChange()
		{
			try
			{
				FileInfo fileInfo = new FileInfo(fileName);
				if (fileInfo.Exists && fileInfo.IsReadOnly)
				{
					switch (host.DecisionDialog(Resources.SpecExplorer, "The view definition file is read-only, do you want to set it to writable?", MessageButton.YESNOCANCEL))
					{
					case MessageResult.YES:
						break;
					case MessageResult.NO:
						return true;
					default:
						return false;
					}
					try
					{
						fileInfo.IsReadOnly = false;
					}
					catch (UnauthorizedAccessException e)
					{
						ShowFileStreamExcptionMessageBox(e);
						return false;
					}
				}
				using (FileStream outputStream = new FileStream(fileName, FileMode.Create))
				{
					try
					{
						manager.Store(viewDefinitionsControl.ViewDefinitionList, outputStream);
					}
					catch (ViewDefinitionManagerException ex)
					{
						host.NotificationDialog(Resources.SpecExplorer, string.Format("Error occured while storing view definitions:\n{0}", ex.Message));
						return false;
					}
				}
				manager.Views = new List<IViewDefinition>(viewDefinitionsControl.ViewDefinitionList);
			}
			catch (SecurityException e2)
			{
				ShowFileStreamExcptionMessageBox(e2);
			}
			catch (FileNotFoundException e3)
			{
				ShowFileStreamExcptionMessageBox(e3);
			}
			catch (DirectoryNotFoundException e4)
			{
				ShowFileStreamExcptionMessageBox(e4);
			}
			catch (PathTooLongException e5)
			{
				ShowFileStreamExcptionMessageBox(e5);
			}
			catch (UnauthorizedAccessException e6)
			{
				ShowFileStreamExcptionMessageBox(e6);
			}
			viewDefinitionsControl.IsDirty = false;
			Apply.Enabled = false;
			return true;
		}

		private void OnImport(object sender, EventArgs e)
		{
			string text;
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Multiselect = false;
				openFileDialog.Filter = "View definition file (*.sevu)|*.sevu|All (*.*)|*.*";
				openFileDialog.ShowDialog();
				text = openFileDialog.FileName;
			}
			ImportFile(text);
			viewDefinitionsControl.CurrentViewDefinition = manager.DefaultViews.First();
			Apply.Enabled = true;
		}

		private void ImportFile(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return;
			}
			try
			{
				using (FileStream inputStream = File.OpenRead(fileName))
				{
					try
					{
						manager.Load(inputStream);
					}
					catch (ViewDefinitionManagerException ex)
					{
						host.NotificationDialog(Resources.SpecExplorer, string.Format("Error occured while loading view definitions:\n{0}", ex.Message));
						return;
					}
				}
			}
			catch (SecurityException e)
			{
				ShowFileStreamExcptionMessageBox(e);
			}
			catch (FileNotFoundException e2)
			{
				ShowFileStreamExcptionMessageBox(e2);
			}
			catch (DirectoryNotFoundException e3)
			{
				ShowFileStreamExcptionMessageBox(e3);
			}
			catch (PathTooLongException e4)
			{
				ShowFileStreamExcptionMessageBox(e4);
			}
			viewDefinitionsControl.ViewDefinitionList = new List<IViewDefinition>(manager.Views);
		}

		private void OnExport(object sender, EventArgs e)
		{
			string text;
			using (SaveFileDialog saveFileDialog = new SaveFileDialog())
			{
				saveFileDialog.Filter = "View definition file (*.sevu)|*.sevu|All (*.*)|*.*";
				saveFileDialog.ShowDialog();
				text = saveFileDialog.FileName;
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			try
			{
				using (FileStream outputStream = new FileStream(text, FileMode.Create))
				{
					manager.Store(viewDefinitionsControl.ViewDefinitionList, outputStream);
				}
			}
			catch (SecurityException e2)
			{
				ShowFileStreamExcptionMessageBox(e2);
			}
			catch (FileNotFoundException e3)
			{
				ShowFileStreamExcptionMessageBox(e3);
			}
			catch (DirectoryNotFoundException e4)
			{
				ShowFileStreamExcptionMessageBox(e4);
			}
			catch (PathTooLongException e5)
			{
				ShowFileStreamExcptionMessageBox(e5);
			}
		}

		private void OnOK(object sender, EventArgs e)
		{
			if (Apply.Enabled)
			{
				if (ApplyChange())
				{
					manager.CurrentView = viewDefinitionsControl.CurrentViewDefinition;
					manager.UpdateEventRaise(updatedViewDefinitiions);
					Close();
				}
			}
			else
			{
				manager.CurrentView = viewDefinitionsControl.CurrentViewDefinition;
				manager.UpdateEventRaise(updatedViewDefinitiions);
				Close();
			}
		}

		private void OnUpdate(object sender, PropertyValueChangedEventArgs e)
		{
			Apply.Enabled = true;
			updatedViewDefinitiions.Add(viewDefinitionsControl.CurrentViewDefinition);
		}

		private void OnClose(object sender, EventArgs e)
		{
			if (viewDefinitionsControl.IsDirty)
			{
				manager.Reset();
				if (File.Exists(fileName))
				{
					ImportFile(fileName);
				}
				viewDefinitionsControl.IsDirty = false;
			}
		}

		private static void ShowFileStreamExcptionMessageBox(Exception e)
		{
			MessageBox.Show(e.Message, "File IO error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}

		private void ViewDefinitionManagerForm_HelpButtonClicked(object sender, CancelEventArgs e)
		{
			if (this.ShowHelp != null)
			{
				this.ShowHelp(sender, e);
			}
		}

		private void ViewDefinitionManagerForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			if (this.ShowHelp != null)
			{
				this.ShowHelp(sender, hlpevent);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			viewDefinitionsControl = new Microsoft.SpecExplorer.Viewer.ViewDefinitionsControl();
			New = new System.Windows.Forms.Button();
			Delete = new System.Windows.Forms.Button();
			Import = new System.Windows.Forms.Button();
			Export = new System.Windows.Forms.Button();
			OK = new System.Windows.Forms.Button();
			Cancel = new System.Windows.Forms.Button();
			Apply = new System.Windows.Forms.Button();
			groupBox1 = new System.Windows.Forms.GroupBox();
			SuspendLayout();
			viewDefinitionsControl.BackColor = System.Drawing.SystemColors.ButtonFace;
			viewDefinitionsControl.CurrentViewDefinition = null;
			viewDefinitionsControl.IsDirty = false;
			viewDefinitionsControl.Location = new System.Drawing.Point(12, 12);
			viewDefinitionsControl.Name = "viewDefinitionsControl";
			viewDefinitionsControl.Size = new System.Drawing.Size(513, 382);
			viewDefinitionsControl.TabIndex = 0;
			New.Location = new System.Drawing.Point(12, 400);
			New.Name = "New";
			New.Size = new System.Drawing.Size(75, 23);
			New.TabIndex = 1;
			New.Text = "&New";
			New.UseVisualStyleBackColor = true;
			Delete.Location = new System.Drawing.Point(93, 400);
			Delete.Name = "Delete";
			Delete.Size = new System.Drawing.Size(75, 23);
			Delete.TabIndex = 2;
			Delete.Text = "&Delete";
			Delete.UseVisualStyleBackColor = true;
			Import.Location = new System.Drawing.Point(12, 448);
			Import.Name = "Import";
			Import.Size = new System.Drawing.Size(75, 23);
			Import.TabIndex = 3;
			Import.Text = "&Import...";
			Import.UseVisualStyleBackColor = true;
			Export.Location = new System.Drawing.Point(93, 448);
			Export.Name = "Export";
			Export.Size = new System.Drawing.Size(75, 23);
			Export.TabIndex = 4;
			Export.Text = "&Export...";
			Export.UseVisualStyleBackColor = true;
			OK.Location = new System.Drawing.Point(288, 448);
			OK.Name = "OK";
			OK.Size = new System.Drawing.Size(75, 23);
			OK.TabIndex = 5;
			OK.Text = "&OK";
			OK.UseVisualStyleBackColor = true;
			Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Cancel.Location = new System.Drawing.Point(369, 448);
			Cancel.Name = "Cancel";
			Cancel.Size = new System.Drawing.Size(75, 23);
			Cancel.TabIndex = 6;
			Cancel.Text = "&Cancel";
			Cancel.UseVisualStyleBackColor = true;
			Apply.Enabled = false;
			Apply.Location = new System.Drawing.Point(450, 448);
			Apply.Name = "Apply";
			Apply.Size = new System.Drawing.Size(75, 23);
			Apply.TabIndex = 7;
			Apply.Text = "&Apply";
			Apply.UseVisualStyleBackColor = true;
			groupBox1.Location = new System.Drawing.Point(12, 429);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(513, 8);
			groupBox1.TabIndex = 8;
			groupBox1.TabStop = false;
			base.AcceptButton = Apply;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = Cancel;
			base.ClientSize = new System.Drawing.Size(537, 478);
			base.Controls.Add(groupBox1);
			base.Controls.Add(Apply);
			base.Controls.Add(Cancel);
			base.Controls.Add(OK);
			base.Controls.Add(Export);
			base.Controls.Add(Import);
			base.Controls.Add(Delete);
			base.Controls.Add(New);
			base.Controls.Add(viewDefinitionsControl);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.HelpButton = true;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ViewDefinitionManagerForm";
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Spec Explorer View Definitions";
			base.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(ViewDefinitionManagerForm_HelpButtonClicked);
			base.HelpRequested += new System.Windows.Forms.HelpEventHandler(ViewDefinitionManagerForm_HelpRequested);
			ResumeLayout(false);
		}
	}
}
