// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.CodeElementItem
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using EnvDTE80;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.SpecExplorer.VS
{
  public class CodeElementItem : INotifyPropertyChanged
  {
    private Func<CodeElementItem, CodeElement, bool> childElementValidator;
    private Func<CodeElementItem, string> nameFabricator;
    private ObservableCollection<CodeElementItem> children;
    private bool isSelected;

    public CodeElementItem(
      CodeElement rootElement,
      Func<CodeElementItem, CodeElement, bool> childElementValidator,
      Func<CodeElementItem, string> nameFabricator)
      : this(rootElement, (CodeElementItem) null, childElementValidator, nameFabricator)
    {
    }

    public CodeElementItem(
      CodeElement rootElement,
      CodeElementItem parent,
      Func<CodeElementItem, CodeElement, bool> childElementValidator,
      Func<CodeElementItem, string> nameFabricator)
    {
      if (rootElement == null && parent == null)
        throw new ArgumentNullException(nameof (rootElement), "rootElement and parent cannot be null at the same time.");
      this.RootElement = rootElement;
      this.Parent = parent;
      this.nameFabricator = nameFabricator;
      this.childElementValidator = childElementValidator;
    }

    public CodeElement RootElement { get; private set; }

    public CodeElementItem Parent { get; private set; }

    public ObservableCollection<CodeElementItem> Children
    {
      get
      {
        if (this.children == null)
        {
          this.children = new ObservableCollection<CodeElementItem>();
          if (this.Kind != CodeElementItemType.BaseContainer)
          {
            this.AddChildren(this.RootElement, false);
            this.AddBaseTypesAsChildren();
          }
          else
            this.AddChildren(this.Parent.RootElement, true);
        }
        return this.children;
      }
    }

    public bool IsStatic
    {
      get
      {
        switch (this.Kind)
        {
          case CodeElementItemType.Class:
            return (this.RootElement as CodeClass2).IsShared;
          case CodeElementItemType.Event:
            return (this.RootElement as CodeEvent).IsShared;
          case CodeElementItemType.Function:
            return (this.RootElement as CodeFunction).IsShared;
          default:
            return false;
        }
      }
    }

    public string FullName
    {
      get
      {
        string prototype = this.GetPrototype(true);
        switch (this.Kind)
        {
          case CodeElementItemType.Namespace:
            return "namespace " + prototype;
          case CodeElementItemType.Class:
            return (this.IsStatic ? "static " : string.Empty) + "class " + prototype;
          case CodeElementItemType.Interface:
            return "interface " + prototype;
          case CodeElementItemType.Event:
            return (this.IsStatic ? "static " : string.Empty) + "event " + prototype;
          case CodeElementItemType.Function:
            return (this.IsStatic ? "static " : string.Empty) + prototype;
          case CodeElementItemType.BaseContainer:
            return "Base Types";
          default:
            return (string) null;
        }
      }
    }

    public string Name
    {
      get
      {
        if (this.nameFabricator == null)
          return this.GetPrototype(false);
        return this.nameFabricator(this);
      }
    }

    public CodeElementItemType Kind
    {
      get
      {
        if (this.RootElement == null)
          return CodeElementItemType.BaseContainer;
        switch (this.RootElement.Kind)
        {
          case vsCMElement.vsCMElementClass:
            return CodeElementItemType.Class;
          case vsCMElement.vsCMElementFunction:
            return CodeElementItemType.Function;
          case vsCMElement.vsCMElementNamespace:
            return CodeElementItemType.Namespace;
          case vsCMElement.vsCMElementInterface:
            return CodeElementItemType.Interface;
          case vsCMElement.vsCMElementEvent:
            return CodeElementItemType.Event;
          default:
            return CodeElementItemType.None;
        }
      }
    }

    public bool IsSelected
    {
      get
      {
        return this.isSelected;
      }
      set
      {
        this.isSelected = value;
        this.SendNotification(nameof (IsSelected));
      }
    }

    public string GetPrototype(bool enableFullName)
    {
      switch (this.Kind)
      {
        case CodeElementItemType.Namespace:
          return this.RootElement.FullName;
        case CodeElementItemType.Class:
        case CodeElementItemType.Interface:
          if (!enableFullName)
            return this.RootElement[];
          return this.RootElement.FullName;
        case CodeElementItemType.Event:
          CodeEvent rootElement1 = this.RootElement as CodeEvent;
          return string.Format("{0} {1}", (object) rootElement1.Type.AsString, enableFullName ? (object) rootElement1.FullName : (object) rootElement1[]);
        case CodeElementItemType.Function:
          CodeFunction rootElement2 = this.RootElement as CodeFunction;
          return rootElement2.get_Prototype((rootElement2.FunctionKind == vsCMFunction.vsCMFunctionConstructor || rootElement2.FunctionKind == vsCMFunction.vsCMFunctionDestructor ? 0 : 128) | 16 | 8 | (enableFullName ? 1 : 0));
        case CodeElementItemType.BaseContainer:
          return "Base Types";
        default:
          return (string) null;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void SendNotification(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    private void AddBaseTypesAsChildren()
    {
      if (this.Kind != CodeElementItemType.Class && this.Kind != CodeElementItemType.Interface)
        return;
      foreach (CodeElement codeElement in (this.RootElement as CodeType).Bases)
      {
        if (this.childElementValidator(this, codeElement))
        {
          this.AddChild((CodeElement) null);
          break;
        }
      }
    }

    private void AddChildren(CodeElement childrenProvider, bool baseClassAsChildren)
    {
      switch (childrenProvider.Kind)
      {
        case vsCMElement.vsCMElementClass:
        case vsCMElement.vsCMElementInterface:
          CodeType codeType = childrenProvider as CodeType;
          if (codeType == null)
            break;
          if (baseClassAsChildren)
          {
            if (codeType.Bases == null)
              break;
            IEnumerator enumerator = codeType.Bases.GetEnumerator();
            try
            {
              while (enumerator.MoveNext())
                this.AddChild((CodeElement) enumerator.Current);
              break;
            }
            finally
            {
              (enumerator as IDisposable)?.Dispose();
            }
          }
          else
          {
            using (IEnumerator<CodeElement> enumerator = codeType.GetAllMembers().GetEnumerator())
            {
              while (enumerator.MoveNext())
                this.AddChild(enumerator.Current);
              break;
            }
          }
        case vsCMElement.vsCMElementNamespace:
          CodeNamespace codeNamespace = childrenProvider as CodeNamespace;
          if (codeNamespace.Members == null)
            break;
          IEnumerator enumerator1 = codeNamespace.Members.GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
              this.AddChild((CodeElement) enumerator1.Current);
            break;
          }
          finally
          {
            (enumerator1 as IDisposable)?.Dispose();
          }
      }
    }

    private bool AddChild(CodeElement element)
    {
      if (element == null || !this.childElementValidator(this, element))
        return false;
      CodeElementItem codeElementItem = new CodeElementItem(element, this, this.childElementValidator, this.nameFabricator);
      codeElementItem.PropertyChanged += this.PropertyChanged;
      this.children.Add(codeElementItem);
      return true;
    }
  }
}
