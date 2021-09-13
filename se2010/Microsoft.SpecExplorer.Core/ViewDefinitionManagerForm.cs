// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ViewDefinitionManagerForm
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.Properties;
using Microsoft.SpecExplorer.Viewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows.Forms;

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
      this.manager = ivd;
      this.InitializeComponent();
      this.viewDefinitionsControl.Host = host;
      this.viewDefinitionsControl.ViewDefinitionList = (IEnumerable<IViewDefinition>) new List<IViewDefinition>(ivd.Views);
      this.viewDefinitionsControl.IsDirty = false;
      this.viewDefinitionsControl.CurrentViewDefinition = this.manager.CurrentView;
      this.updatedViewDefinitiions = new HashSet<IViewDefinition>();
      this.InitializeUI();
    }

    private void InitializeUI()
    {
      this.FormClosing += new FormClosingEventHandler(this.OnClose);
      this.New.Click += new EventHandler(this.OnNew);
      this.Delete.Click += new EventHandler(this.OnDelete);
      this.Import.Click += new EventHandler(this.OnImport);
      this.Export.Click += new EventHandler(this.OnExport);
      this.OK.Click += new EventHandler(this.OnOK);
      this.Cancel.Click += (EventHandler) ((sender, e) => this.Close());
      this.Apply.Click += new EventHandler(this.OnApply);
      this.viewDefinitionsControl.propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(this.OnUpdate);
      if (!string.IsNullOrEmpty(this.fileName))
        return;
      this.New.Enabled = false;
      this.Delete.Enabled = false;
      this.Import.Enabled = false;
      this.Export.Enabled = false;
      this.Apply.Enabled = false;
    }

    private void OnNew(object sender, EventArgs e)
    {
      ViewDefinition viewDefinition1 = new ViewDefinition();
      string str = "View1";
      HashSet<string> stringSet = new HashSet<string>();
      foreach (ViewDefinition viewDefinition2 in this.viewDefinitionsControl.ViewDefinitionList)
        stringSet.Add(viewDefinition2.Name);
      int num = 2;
      while (stringSet.Contains(str))
      {
        str = "View" + num.ToString();
        ++num;
      }
      viewDefinition1.Name = str;
      this.viewDefinitionsControl.Add((IViewDefinition) viewDefinition1);
      this.viewDefinitionsControl.CurrentViewDefinition = (IViewDefinition) viewDefinition1;
      this.Apply.Enabled = true;
    }

    private void OnDelete(object sender, EventArgs e)
    {
      if (this.viewDefinitionsControl.CurrentViewDefinition.IsDefault)
      {
        this.host.NotificationDialog(Resources.SpecExplorer, "Default view cannot be deleted.");
      }
      else
      {
        this.viewDefinitionsControl.RemoveCurrentViewDefinition();
        this.Apply.Enabled = true;
      }
    }

    private void OnApply(object sender, EventArgs e) => this.ApplyChange();

    private bool ApplyChange()
    {
      try
      {
        FileInfo fileInfo = new FileInfo(this.fileName);
        if (fileInfo.Exists && fileInfo.IsReadOnly)
        {
          switch (this.host.DecisionDialog(Resources.SpecExplorer, "The view definition file is read-only, do you want to set it to writable?", MessageButton.YESNOCANCEL))
          {
            case MessageResult.YES:
              try
              {
                fileInfo.IsReadOnly = false;
                break;
              }
              catch (UnauthorizedAccessException ex)
              {
                ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
                return false;
              }
            case MessageResult.NO:
              return true;
            default:
              return false;
          }
        }
        using (FileStream fileStream = new FileStream(this.fileName, FileMode.Create))
        {
          try
          {
            this.manager.Store(this.viewDefinitionsControl.ViewDefinitionList, (Stream) fileStream);
          }
          catch (ViewDefinitionManagerException ex)
          {
            this.host.NotificationDialog(Resources.SpecExplorer, string.Format("Error occured while storing view definitions:\n{0}", (object) ex.Message));
            return false;
          }
        }
        this.manager.Views = (IEnumerable<IViewDefinition>) new List<IViewDefinition>(this.viewDefinitionsControl.ViewDefinitionList);
      }
      catch (SecurityException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
      catch (FileNotFoundException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
      catch (DirectoryNotFoundException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
      catch (PathTooLongException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
      catch (UnauthorizedAccessException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
      this.viewDefinitionsControl.IsDirty = false;
      this.Apply.Enabled = false;
      return true;
    }

    private void OnImport(object sender, EventArgs e)
    {
      string fileName;
      using (OpenFileDialog openFileDialog = new OpenFileDialog())
      {
        openFileDialog.Multiselect = false;
        openFileDialog.Filter = "View definition file (*.sevu)|*.sevu|All (*.*)|*.*";
        int num = (int) openFileDialog.ShowDialog();
        fileName = openFileDialog.FileName;
      }
      this.ImportFile(fileName);
      this.viewDefinitionsControl.CurrentViewDefinition = this.manager.DefaultViews.First<IViewDefinition>();
      this.Apply.Enabled = true;
    }

    private void ImportFile(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        return;
      try
      {
        using (FileStream fileStream = File.OpenRead(fileName))
        {
          try
          {
            this.manager.Load((Stream) fileStream);
          }
          catch (ViewDefinitionManagerException ex)
          {
            this.host.NotificationDialog(Resources.SpecExplorer, string.Format("Error occured while loading view definitions:\n{0}", (object) ex.Message));
            return;
          }
        }
      }
      catch (SecurityException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
      catch (FileNotFoundException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
      catch (DirectoryNotFoundException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
      catch (PathTooLongException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
      this.viewDefinitionsControl.ViewDefinitionList = (IEnumerable<IViewDefinition>) new List<IViewDefinition>(this.manager.Views);
    }

    private void OnExport(object sender, EventArgs e)
    {
      string fileName;
      using (SaveFileDialog saveFileDialog = new SaveFileDialog())
      {
        saveFileDialog.Filter = "View definition file (*.sevu)|*.sevu|All (*.*)|*.*";
        int num = (int) saveFileDialog.ShowDialog();
        fileName = saveFileDialog.FileName;
      }
      if (string.IsNullOrEmpty(fileName))
        return;
      try
      {
        using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
          this.manager.Store(this.viewDefinitionsControl.ViewDefinitionList, (Stream) fileStream);
      }
      catch (SecurityException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
      catch (FileNotFoundException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
      catch (DirectoryNotFoundException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
      catch (PathTooLongException ex)
      {
        ViewDefinitionManagerForm.ShowFileStreamExcptionMessageBox((Exception) ex);
      }
    }

    private void OnOK(object sender, EventArgs e)
    {
      if (this.Apply.Enabled)
      {
        if (!this.ApplyChange())
          return;
        this.manager.CurrentView = this.viewDefinitionsControl.CurrentViewDefinition;
        this.manager.UpdateEventRaise((IEnumerable<IViewDefinition>) this.updatedViewDefinitiions);
        this.Close();
      }
      else
      {
        this.manager.CurrentView = this.viewDefinitionsControl.CurrentViewDefinition;
        this.manager.UpdateEventRaise((IEnumerable<IViewDefinition>) this.updatedViewDefinitiions);
        this.Close();
      }
    }

    private void OnUpdate(object sender, PropertyValueChangedEventArgs e)
    {
      this.Apply.Enabled = true;
      this.updatedViewDefinitiions.Add(this.viewDefinitionsControl.CurrentViewDefinition);
    }

    private void OnClose(object sender, EventArgs e)
    {
      if (!this.viewDefinitionsControl.IsDirty)
        return;
      this.manager.Reset();
      if (File.Exists(this.fileName))
        this.ImportFile(this.fileName);
      this.viewDefinitionsControl.IsDirty = false;
    }

    private static void ShowFileStreamExcptionMessageBox(Exception e)
    {
      int num = (int) MessageBox.Show(e.Message, "File IO error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
    }

    private void ViewDefinitionManagerForm_HelpButtonClicked(object sender, CancelEventArgs e)
    {
      if (this.ShowHelp == null)
        return;
      this.ShowHelp(sender, (EventArgs) e);
    }

    private void ViewDefinitionManagerForm_HelpRequested(object sender, HelpEventArgs hlpevent)
    {
      if (this.ShowHelp == null)
        return;
      this.ShowHelp(sender, (EventArgs) hlpevent);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.viewDefinitionsControl = new ViewDefinitionsControl();
      this.New = new Button();
      this.Delete = new Button();
      this.Import = new Button();
      this.Export = new Button();
      this.OK = new Button();
      this.Cancel = new Button();
      this.Apply = new Button();
      this.groupBox1 = new GroupBox();
      this.SuspendLayout();
      this.viewDefinitionsControl.BackColor = SystemColors.ButtonFace;
      this.viewDefinitionsControl.CurrentViewDefinition = (IViewDefinition) null;
      this.viewDefinitionsControl.IsDirty = false;
      this.viewDefinitionsControl.Location = new Point(12, 12);
      this.viewDefinitionsControl.Name = "viewDefinitionsControl";
      this.viewDefinitionsControl.Size = new Size(513, 382);
      this.viewDefinitionsControl.TabIndex = 0;
      this.New.Location = new Point(12, 400);
      this.New.Name = "New";
      this.New.Size = new Size(75, 23);
      this.New.TabIndex = 1;
      this.New.Text = "&New";
      this.New.UseVisualStyleBackColor = true;
      this.Delete.Location = new Point(93, 400);
      this.Delete.Name = "Delete";
      this.Delete.Size = new Size(75, 23);
      this.Delete.TabIndex = 2;
      this.Delete.Text = "&Delete";
      this.Delete.UseVisualStyleBackColor = true;
      this.Import.Location = new Point(12, 448);
      this.Import.Name = "Import";
      this.Import.Size = new Size(75, 23);
      this.Import.TabIndex = 3;
      this.Import.Text = "&Import...";
      this.Import.UseVisualStyleBackColor = true;
      this.Export.Location = new Point(93, 448);
      this.Export.Name = "Export";
      this.Export.Size = new Size(75, 23);
      this.Export.TabIndex = 4;
      this.Export.Text = "&Export...";
      this.Export.UseVisualStyleBackColor = true;
      this.OK.Location = new Point(288, 448);
      this.OK.Name = "OK";
      this.OK.Size = new Size(75, 23);
      this.OK.TabIndex = 5;
      this.OK.Text = "&OK";
      this.OK.UseVisualStyleBackColor = true;
      this.Cancel.DialogResult = DialogResult.Cancel;
      this.Cancel.Location = new Point(369, 448);
      this.Cancel.Name = "Cancel";
      this.Cancel.Size = new Size(75, 23);
      this.Cancel.TabIndex = 6;
      this.Cancel.Text = "&Cancel";
      this.Cancel.UseVisualStyleBackColor = true;
      this.Apply.Enabled = false;
      this.Apply.Location = new Point(450, 448);
      this.Apply.Name = "Apply";
      this.Apply.Size = new Size(75, 23);
      this.Apply.TabIndex = 7;
      this.Apply.Text = "&Apply";
      this.Apply.UseVisualStyleBackColor = true;
      this.groupBox1.Location = new Point(12, 429);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(513, 8);
      this.groupBox1.TabIndex = 8;
      this.groupBox1.TabStop = false;
      this.AcceptButton = (IButtonControl) this.Apply;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.Cancel;
      this.ClientSize = new Size(537, 478);
      this.Controls.Add((Control) this.groupBox1);
      this.Controls.Add((Control) this.Apply);
      this.Controls.Add((Control) this.Cancel);
      this.Controls.Add((Control) this.OK);
      this.Controls.Add((Control) this.Export);
      this.Controls.Add((Control) this.Import);
      this.Controls.Add((Control) this.Delete);
      this.Controls.Add((Control) this.New);
      this.Controls.Add((Control) this.viewDefinitionsControl);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = nameof (ViewDefinitionManagerForm);
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Spec Explorer View Definitions";
      this.HelpButtonClicked += new CancelEventHandler(this.ViewDefinitionManagerForm_HelpButtonClicked);
      this.HelpRequested += new HelpEventHandler(this.ViewDefinitionManagerForm_HelpRequested);
      this.ResumeLayout(false);
    }
  }
}
