using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using EnvDTE;
using EnvDTE80;

namespace Microsoft.SpecExplorer.VS
{
	public class CodeElementItem : INotifyPropertyChanged
	{
		private Func<CodeElementItem, CodeElement, bool> childElementValidator;

		private Func<CodeElementItem, string> nameFabricator;

		private ObservableCollection<CodeElementItem> children;

		private bool isSelected;

		public CodeElement RootElement { get; private set; }

		public CodeElementItem Parent { get; private set; }

		public ObservableCollection<CodeElementItem> Children
		{
			get
			{
				if (children == null)
				{
					children = new ObservableCollection<CodeElementItem>();
					if (Kind != CodeElementItemType.BaseContainer)
					{
						AddChildren(RootElement, false);
						AddBaseTypesAsChildren();
					}
					else
					{
						AddChildren(Parent.RootElement, true);
					}
				}
				return children;
			}
		}

		public bool IsStatic
		{
			get
			{
				switch (Kind)
				{
				case CodeElementItemType.Function:
				{
					CodeElement rootElement3 = RootElement;
					return ((CodeFunction)((rootElement3 is CodeFunction) ? rootElement3 : null)).IsShared;
				}
				case CodeElementItemType.Event:
				{
					CodeElement rootElement2 = RootElement;
					return ((CodeEvent)((rootElement2 is CodeEvent) ? rootElement2 : null)).IsShared;
				}
				case CodeElementItemType.Class:
				{
					CodeElement rootElement = RootElement;
					return ((CodeClass2)((rootElement is CodeClass2) ? rootElement : null)).IsShared;
				}
				default:
					return false;
				}
			}
		}

		public string FullName
		{
			get
			{
				string prototype = GetPrototype(true);
				switch (Kind)
				{
				case CodeElementItemType.BaseContainer:
					return "Base Types";
				case CodeElementItemType.Class:
					return (IsStatic ? "static " : string.Empty) + "class " + prototype;
				case CodeElementItemType.Interface:
					return "interface " + prototype;
				case CodeElementItemType.Function:
					return (IsStatic ? "static " : string.Empty) + prototype;
				case CodeElementItemType.Event:
					return (IsStatic ? "static " : string.Empty) + "event " + prototype;
				case CodeElementItemType.Namespace:
					return "namespace " + prototype;
				default:
					return null;
				}
			}
		}

		public string Name
		{
			get
			{
				if (nameFabricator == null)
				{
					return GetPrototype(false);
				}
				return nameFabricator(this);
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
				return isSelected;
			}
			set
			{
				isSelected = value;
				SendNotification("IsSelected");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public CodeElementItem(CodeElement rootElement, Func<CodeElementItem, CodeElement, bool> childElementValidator, Func<CodeElementItem, string> nameFabricator)
			: this(rootElement, null, childElementValidator, nameFabricator)
		{
		}

		public CodeElementItem(CodeElement rootElement, CodeElementItem parent, Func<CodeElementItem, CodeElement, bool> childElementValidator, Func<CodeElementItem, string> nameFabricator)
		{
			if (rootElement == null && parent == null)
			{
				throw new ArgumentNullException("rootElement", "rootElement and parent cannot be null at the same time.");
			}
			RootElement = rootElement;
			Parent = parent;
			this.nameFabricator = nameFabricator;
			this.childElementValidator = childElementValidator;
		}

		public string GetPrototype(bool enableFullName)
		{
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Invalid comparison between Unknown and I4
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Invalid comparison between Unknown and I4
			switch (Kind)
			{
			case CodeElementItemType.BaseContainer:
				return "Base Types";
			case CodeElementItemType.Class:
			case CodeElementItemType.Interface:
				if (!enableFullName)
				{
					return RootElement.Name;
				}
				return RootElement.FullName;
			case CodeElementItemType.Namespace:
				return RootElement.FullName;
			case CodeElementItemType.Event:
			{
				CodeElement rootElement2 = RootElement;
				CodeEvent val2 = (CodeEvent)(object)((rootElement2 is CodeEvent) ? rootElement2 : null);
				return string.Format("{0} {1}", val2.Type.AsString, enableFullName ? val2.FullName : val2.Name);
			}
			case CodeElementItemType.Function:
			{
				CodeElement rootElement = RootElement;
				CodeFunction val = (CodeFunction)(object)((rootElement is CodeFunction) ? rootElement : null);
				return val.get_Prototype((((int)val.FunctionKind != 1 && (int)val.FunctionKind != 512) ? 128 : 0) | 0x10 | 8 | (enableFullName ? 1 : 0));
			}
			default:
				return null;
			}
		}

		private void SendNotification(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void AddBaseTypesAsChildren()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			if (Kind != CodeElementItemType.Class && Kind != CodeElementItemType.Interface)
			{
				return;
			}
			CodeElement rootElement = RootElement;
			foreach (CodeElement basis in ((CodeType)((rootElement is CodeType) ? rootElement : null)).Bases)
			{
				CodeElement arg = basis;
				if (childElementValidator(this, arg))
				{
					AddChild(null);
					break;
				}
			}
		}

		private void AddChildren(CodeElement childrenProvider, bool baseClassAsChildren)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Invalid comparison between Unknown and I4
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Invalid comparison between Unknown and I4
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Expected O, but got Unknown
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Expected O, but got Unknown
			vsCMElement kind = childrenProvider.Kind;
			if ((int)kind != 1)
			{
				if ((int)kind == 5)
				{
					CodeNamespace val = (CodeNamespace)(object)((childrenProvider is CodeNamespace) ? childrenProvider : null);
					if (val.Members == null)
					{
						return;
					}
					foreach (CodeElement member in val.Members)
					{
						CodeElement element = member;
						AddChild(element);
					}
					return;
				}
				if ((int)kind != 8)
				{
					return;
				}
			}
			CodeType val2 = (CodeType)(object)((childrenProvider is CodeType) ? childrenProvider : null);
			if (val2 == null)
			{
				return;
			}
			if (baseClassAsChildren)
			{
				if (val2.Bases == null)
				{
					return;
				}
				foreach (CodeElement basis in val2.Bases)
				{
					CodeElement element2 = basis;
					AddChild(element2);
				}
				return;
			}
			foreach (CodeElement allMember in val2.GetAllMembers())
			{
				AddChild(allMember);
			}
		}

		private bool AddChild(CodeElement element)
		{
			if (element != null && childElementValidator(this, element))
			{
				CodeElementItem codeElementItem = new CodeElementItem(element, this, childElementValidator, nameFabricator);
				codeElementItem.PropertyChanged += this.PropertyChanged;
				children.Add(codeElementItem);
				return true;
			}
			return false;
		}
	}
}
