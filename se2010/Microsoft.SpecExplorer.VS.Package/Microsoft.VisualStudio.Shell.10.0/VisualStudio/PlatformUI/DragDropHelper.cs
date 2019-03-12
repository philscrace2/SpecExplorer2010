// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.DragDropHelper
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;

namespace Microsoft.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class DragDropHelper
  {
    private bool _dragDropInProgress;
    private Microsoft.VisualStudio.OLE.Interop.IDropTarget _dropTarget;
    private bool _usingDefaultDropTarget;
    private readonly IInputElement targetInputElement;

    private Microsoft.VisualStudio.OLE.Interop.IDropTarget DropTarget
    {
      get
      {
        if (this._dropTarget == null)
        {
          this._dropTarget = ServiceProvider.GlobalProvider.GetService(typeof (SVsMainWindowDropTarget)) as Microsoft.VisualStudio.OLE.Interop.IDropTarget;
          this._usingDefaultDropTarget = true;
        }
        return this._dropTarget;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException(nameof (value));
        if (this._dragDropInProgress)
          throw new InvalidOperationException(Microsoft.VisualStudio.PlatformUI.Common.Resources.Error_CannotChangeDropTargetDuringDragAndDrop);
        this._dropTarget = value;
        this._usingDefaultDropTarget = false;
      }
    }

    public DragDropHelper(IInputElement targetInputElement)
    {
      this.targetInputElement = targetInputElement;
    }

    public DragDropHelper(IInputElement targetInputElement, Microsoft.VisualStudio.OLE.Interop.IDropTarget dropTarget)
    {
      this.targetInputElement = targetInputElement;
      this.DropTarget = dropTarget;
    }

    public void OnDragEnter(System.Windows.DragEventArgs e)
    {
      if (e.Handled)
        return;
      try
      {
        uint pdwEffect = 0;
        Point position = e.GetPosition(this.targetInputElement);
        this.DropTarget.DragEnter(DragDropHelper.ConvertToOleDataObject(e.Data), (uint) e.KeyStates, position.ToPOINTL(), ref pdwEffect);
        e.Effects = (System.Windows.DragDropEffects) pdwEffect;
      }
      catch (COMException ex)
      {
        e.Effects = System.Windows.DragDropEffects.None;
      }
      finally
      {
        this._dragDropInProgress = true;
      }
      e.Handled = true;
    }

    public void OnDragLeave(RoutedEventArgs e)
    {
      if (e.Handled)
        return;
      try
      {
        this.DropTarget.DragLeave();
      }
      catch (COMException ex)
      {
      }
      finally
      {
        this.OnDragDropComplete();
      }
      e.Handled = true;
    }

    public void OnDragOver(System.Windows.DragEventArgs e)
    {
      if (e.Handled)
        return;
      try
      {
        uint pdwEffect = 0;
        Point position = e.GetPosition(this.targetInputElement);
        this.DropTarget.DragOver((uint) e.KeyStates, position.ToPOINTL(), ref pdwEffect);
        e.Effects = (System.Windows.DragDropEffects) pdwEffect;
      }
      catch (COMException ex)
      {
        e.Effects = System.Windows.DragDropEffects.None;
      }
      e.Handled = true;
    }

    public void OnDrop(System.Windows.DragEventArgs e)
    {
      if (e.Handled)
        return;
      try
      {
        uint pdwEffect = 0;
        Point position = e.GetPosition(this.targetInputElement);
        this.DropTarget.Drop(DragDropHelper.ConvertToOleDataObject(e.Data), (uint) e.KeyStates, position.ToPOINTL(), ref pdwEffect);
        e.Effects = (System.Windows.DragDropEffects) pdwEffect;
      }
      catch (COMException ex)
      {
        VsShellUtilities.ShowMessageBox((System.IServiceProvider) ServiceProvider.GlobalProvider, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.PlatformUI.Common.Resources.Error_CannotCompleteDragAndDrop, (object) ex.Message), string.Empty, OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
      }
      finally
      {
        this.OnDragDropComplete();
      }
      e.Handled = true;
    }

    private void OnDragDropComplete()
    {
      this._dragDropInProgress = false;
      if (!this._usingDefaultDropTarget)
        return;
      this._dropTarget = (Microsoft.VisualStudio.OLE.Interop.IDropTarget) null;
    }

    public static Microsoft.VisualStudio.OLE.Interop.IDataObject ConvertToOleDataObject(
      System.Windows.IDataObject source)
    {
      System.Windows.Forms.DataObject dataObject = new System.Windows.Forms.DataObject();
      foreach (string format in source.GetFormats())
      {
        object data = (object) null;
        try
        {
          data = source.GetData(format);
        }
        catch (COMException ex)
        {
        }
        if (data != null)
          dataObject.SetData(format, data);
      }
      return (Microsoft.VisualStudio.OLE.Interop.IDataObject) new OleDataObject((System.Windows.Forms.IDataObject) dataObject);
    }
  }
}
