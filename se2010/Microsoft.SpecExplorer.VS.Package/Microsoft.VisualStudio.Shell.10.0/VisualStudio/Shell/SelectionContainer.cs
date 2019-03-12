// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.SelectionContainer
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  public class SelectionContainer : ISelectionContainer
  {
    private static ICollection _emptyCollection = (ICollection) new object[0];
    public const uint ALL = 1;
    public const uint SELECTED = 2;
    private const int SELOBJ_ACTIVATE_WINDOW = 1;
    private ICollection _selectableObjects;
    private ICollection _selectedObjects;
    private readonly bool _selectableReadOnly;
    private readonly bool _selectedReadOnly;

    public SelectionContainer()
    {
      this._selectableObjects = SelectionContainer._emptyCollection;
      this._selectedObjects = SelectionContainer._emptyCollection;
    }

    public SelectionContainer(bool selectableReadOnly, bool selectedReadOnly)
      : this()
    {
      this._selectableReadOnly = selectableReadOnly;
      this._selectedReadOnly = selectedReadOnly;
    }

    public ICollection SelectableObjects
    {
      get
      {
        return this._selectableObjects;
      }
      set
      {
        if (value == null)
          value = SelectionContainer._emptyCollection;
        this._selectableObjects = value;
      }
    }

    public ICollection SelectedObjects
    {
      get
      {
        return this._selectedObjects;
      }
      set
      {
        if (value == null)
          value = SelectionContainer._emptyCollection;
        this._selectedObjects = value;
      }
    }

    public event EventHandler SelectedObjectsChanged;

    protected virtual void ActivateObjects()
    {
    }

    private void ChangeSelection(object[] prgUnkObjects, int dwFlags)
    {
      if (this._selectedReadOnly)
        throw new InvalidOperationException();
      this.SelectedObjects = (ICollection) prgUnkObjects;
      if (this.SelectedObjectsChanged != null)
        this.SelectedObjectsChanged((object) this, EventArgs.Empty);
      if ((dwFlags & 1) == 0)
        return;
      this.ActivateObjects();
    }

    int ISelectionContainer.CountObjects(uint dwFlags, out uint pc)
    {
      switch (dwFlags)
      {
        case 1:
          pc = (uint) this.SelectableObjects.Count;
          break;
        case 2:
          pc = (uint) this.SelectedObjects.Count;
          break;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.General_UnsupportedValue, (object) dwFlags), nameof (dwFlags));
      }
      return 0;
    }

    int ISelectionContainer.GetObjects(
      uint dwFlags,
      uint cObjects,
      object[] apUnkObjects)
    {
      ICollection collection;
      switch (dwFlags)
      {
        case 1:
          collection = this.SelectableObjects;
          break;
        case 2:
          collection = this.SelectedObjects;
          break;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.General_UnsupportedValue, (object) dwFlags), nameof (dwFlags));
      }
      int num = 0;
      foreach (object obj in (IEnumerable) collection)
      {
        if ((long) num < (long) cObjects)
        {
          if (num < apUnkObjects.Length)
            apUnkObjects[num++] = obj;
          else
            break;
        }
        else
          break;
      }
      return 0;
    }

    int ISelectionContainer.SelectObjects(
      uint cSelect,
      object[] apUnkSelect,
      uint dwFlags)
    {
      this.ChangeSelection(apUnkSelect, (int) dwFlags);
      return 0;
    }
  }
}
