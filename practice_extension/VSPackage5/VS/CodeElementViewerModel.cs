// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.CodeElementViewerModel
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.SpecExplorer.VS
{
  public class CodeElementViewerModel : INotifyPropertyChanged
  {
    private Stack<CodeElementItem> propagationStack = new Stack<CodeElementItem>();
    private HashSet<CodeElementItem> downwardSelectionList = new HashSet<CodeElementItem>();
    private HashSet<CodeElementItem> upwardSelectionList = new HashSet<CodeElementItem>();
    private bool isMultipleSelectionAllowed;
    private bool isBaseTypeFolderAllowed;
    private CodeElementExpandOptions expandOptions;
    private HashSet<CodeElementItem> selectedElements;

    public CodeElementViewerModel()
    {
      this.CodeElements = new ObservableCollection<CodeElementItem>();
      this.selectedElements = new HashSet<CodeElementItem>();
    }

    public ObservableCollection<CodeElementItem> CodeElements { get; private set; }

    public void LoadCodeElements(
      IEnumerable codeElements,
      bool allowMultipleSelection,
      CodeElementExpandOptions expandOptions,
      bool allowBaseTypeFolder)
    {
      this.isBaseTypeFolderAllowed = allowBaseTypeFolder;
      this.IsMultipleSelectionAllowed = allowMultipleSelection;
      this.expandOptions = expandOptions;
      this.selectedElements.Clear();
      this.CodeElements.Clear();
      foreach (CodeElement codeElement in codeElements)
      {
        if (this.IsCodeElementValid((CodeElementItem) null, codeElement))
        {
          CodeElementItem codeElementItem = new CodeElementItem(codeElement, new Func<CodeElementItem, CodeElement, bool>(this.IsCodeElementValid), this.CodeElementDisplayTextFabricator);
          codeElementItem.PropertyChanged += new PropertyChangedEventHandler(this.CodeElementItemEventHandler);
          this.CodeElements.Add(codeElementItem);
        }
      }
    }

    public ICollection<CodeElementAndContainerPair> RetrieveSelectedItems(
      params vsCMElement[] elementKinds)
    {
      if (elementKinds == null || elementKinds.Length == 0)
        return (ICollection<CodeElementAndContainerPair>) null;
      ICollection<CodeElementAndContainerPair> andContainerPairs = (ICollection<CodeElementAndContainerPair>) new List<CodeElementAndContainerPair>();
      foreach (CodeElementItem selectedElement in this.selectedElements)
      {
        if (selectedElement.Kind != CodeElementItemType.BaseContainer && ((IEnumerable<vsCMElement>) elementKinds).Contains<vsCMElement>(selectedElement.RootElement.Kind))
        {
          CodeType container = selectedElement.Kind != CodeElementItemType.Function || (selectedElement.RootElement as CodeFunction).FunctionKind != vsCMFunction.vsCMFunctionConstructor && (selectedElement.RootElement as CodeFunction).FunctionKind != vsCMFunction.vsCMFunctionDestructor ? this.LocateTopmostAncestor(selectedElement).RootElement as CodeType : (selectedElement.RootElement as CodeFunction).Parent as CodeType;
          andContainerPairs.Add(new CodeElementAndContainerPair(selectedElement.RootElement, container));
        }
      }
      return andContainerPairs;
    }

    public bool IsMultipleSelectionAllowed
    {
      get
      {
        return this.isMultipleSelectionAllowed;
      }
      set
      {
        this.isMultipleSelectionAllowed = value;
        this.SendNotification(IsMultipleSelectionAllowed.ToString());
      }
    }

    public Func<CodeElementItem, string> CodeElementDisplayTextFabricator { get; set; }

    public Func<CodeElement, bool> CodeElementContentValidator { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    private void SendNotification(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void PropagateSelectionStatus(CodeElementItem element)
    {
      if (this.downwardSelectionList.Contains(element) || this.propagationStack.Count == 0)
      {
        this.propagationStack.Push(element);
        foreach (CodeElementItem child in (Collection<CodeElementItem>) element.Children)
        {
          this.downwardSelectionList.Add(child);
          child.IsSelected = element.IsSelected;
          this.downwardSelectionList.Remove(child);
        }
        this.propagationStack.Pop();
      }
      CodeElementItem parent = element.Parent;
      if (parent == null || !this.upwardSelectionList.Contains(element) && this.propagationStack.Count != 0)
        return;
      this.propagationStack.Push(element);
      bool flag = true;
      foreach (CodeElementItem child in (Collection<CodeElementItem>) parent.Children)
        flag = flag && child.IsSelected;
      this.upwardSelectionList.Add(parent);
      parent.IsSelected = flag;
      this.upwardSelectionList.Remove(parent);
      this.propagationStack.Pop();
    }

    private void CodeElementItemEventHandler(object sender, PropertyChangedEventArgs evtArgs)
    {
      CodeElementItem element = sender as CodeElementItem;
      if (!(evtArgs.PropertyName == "IsSelected"))
        return;
      if (element.IsSelected)
        this.selectedElements.Add(element);
      else
        this.selectedElements.Remove(element);
      if (!this.IsMultipleSelectionAllowed)
        return;
      this.PropagateSelectionStatus(element);
    }

    private bool IsCodeElementValid(CodeElementItem parent, CodeElement codeElement)
    {
      if (codeElement == null)
        return this.isBaseTypeFolderAllowed;
      if (!this.ValidateExpansionLimit(parent, codeElement))
        return false;
      if (this.CodeElementContentValidator == null)
        return true;
      return this.CodeElementContentValidator(codeElement);
    }

    private bool ValidateExpansionLimit(CodeElementItem parent, CodeElement codeElement)
    {
      switch (codeElement.Kind)
      {
        case vsCMElement.vsCMElementClass:
          return (this.expandOptions & CodeElementExpandOptions.ExpandToClasses) != CodeElementExpandOptions.None;
        case vsCMElement.vsCMElementFunction:
          if ((this.expandOptions & CodeElementExpandOptions.ExpandToFunctions) == CodeElementExpandOptions.None)
            return false;
          CodeFunction codeFunction = codeElement as CodeFunction;
          if (codeFunction.FunctionKind == vsCMFunction.vsCMFunctionConstructor || codeFunction.FunctionKind == vsCMFunction.vsCMFunctionDestructor)
            return !this.IsUnderBaseTypeFolder(parent);
          return true;
        case vsCMElement.vsCMElementNamespace:
          return (this.expandOptions & CodeElementExpandOptions.ExpandToNamespaces) != CodeElementExpandOptions.None;
        case vsCMElement.vsCMElementInterface:
          return (this.expandOptions & CodeElementExpandOptions.ExpandToInterfaces) != CodeElementExpandOptions.None;
        case vsCMElement.vsCMElementEvent:
          return (this.expandOptions & CodeElementExpandOptions.ExpandToEvents) != CodeElementExpandOptions.None;
        default:
          return false;
      }
    }

    private CodeElementItem LocateTopmostAncestor(CodeElementItem codeElement)
    {
      CodeElementItem parent1 = codeElement.Parent;
      for (CodeElementItem parent2 = codeElement.Parent; parent2 != null; parent2 = parent2.Parent)
      {
        if (parent2.Kind == CodeElementItemType.BaseContainer)
          parent1 = parent2.Parent;
      }
      return parent1;
    }

    private bool IsUnderBaseTypeFolder(CodeElementItem codeElement)
    {
      for (CodeElementItem codeElementItem = codeElement; codeElementItem != null; codeElementItem = codeElementItem.Parent)
      {
        if (codeElementItem.Kind == CodeElementItemType.BaseContainer)
          return true;
      }
      return false;
    }
  }
}
