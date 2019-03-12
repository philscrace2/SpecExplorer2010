// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideToolWindowAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Drawing;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideToolWindowAttribute : RegistrationAttribute
  {
    private Guid dockedWith = Guid.Empty;
    private Rectangle position = Rectangle.Empty;
    private Type tool;
    private string name;
    private ToolWindowOrientation orientation;
    private VsDockStyle style;
    private bool multiInstances;
    private bool transient;

    public ProvideToolWindowAttribute(Type toolType)
    {
      this.tool = toolType;
      this.name = this.tool.FullName;
    }

    public VsDockStyle Style
    {
      get
      {
        return this.style;
      }
      set
      {
        this.style = value;
      }
    }

    public int PositionX
    {
      get
      {
        return this.position.X;
      }
      set
      {
        this.position.X = value;
      }
    }

    public int PositionY
    {
      get
      {
        return this.position.Y;
      }
      set
      {
        this.position.Y = value;
      }
    }

    public int Width
    {
      get
      {
        return this.position.Width;
      }
      set
      {
        this.position.Width = value;
      }
    }

    public int Height
    {
      get
      {
        return this.position.Height;
      }
      set
      {
        this.position.Height = value;
      }
    }

    public ToolWindowOrientation Orientation
    {
      get
      {
        return this.orientation;
      }
      set
      {
        this.orientation = value;
      }
    }

    public Type ToolType
    {
      get
      {
        return this.tool;
      }
    }

    public string Window
    {
      get
      {
        return this.dockedWith.ToString();
      }
      set
      {
        this.dockedWith = new Guid(value);
      }
    }

    public bool MultiInstances
    {
      get
      {
        return this.multiInstances;
      }
      set
      {
        this.multiInstances = value;
      }
    }

    public bool Transient
    {
      get
      {
        return this.transient;
      }
      set
      {
        this.transient = value;
      }
    }

    private string RegKeyName
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ToolWindows\\{0}", (object) this.tool.GUID.ToString("B"));
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyToolResource, (object) this.name, (object) this.tool.GUID.ToString("B")));
      using (RegistrationAttribute.Key key = context.CreateKey(this.RegKeyName))
      {
        key.SetValue(string.Empty, (object) context.ComponentType.GUID.ToString("B"));
        if (this.name != null)
          key.SetValue("Name", (object) this.name);
        if (this.orientation != ToolWindowOrientation.none)
          key.SetValue("Orientation", (object) this.OrientationToString(this.orientation));
        if (this.style != VsDockStyle.none)
          key.SetValue("Style", (object) this.StyleToString(this.style));
        if (this.dockedWith != Guid.Empty)
          key.SetValue("Window", (object) this.dockedWith.ToString("B"));
        if (this.position.Width != 0 && this.position.Height != 0)
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", (object) this.position.Left, (object) this.position.Top, (object) this.position.Right, (object) this.position.Bottom);
          key.SetValue("Float", (object) str);
        }
        if (!this.transient)
          return;
        key.SetValue("DontForceCreate", (object) 1);
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.RegKeyName);
    }

    private string StyleToString(VsDockStyle style)
    {
      switch (style)
      {
        case VsDockStyle.none:
          return string.Empty;
        case VsDockStyle.MDI:
          return "MDI";
        case VsDockStyle.Float:
          return "Float";
        case VsDockStyle.Linked:
          return "Linked";
        case VsDockStyle.Tabbed:
          return "Tabbed";
        case VsDockStyle.AlwaysFloat:
          return "AlwaysFloat";
        default:
          throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Attributes_UnknownDockingStyle, (object) style));
      }
    }

    private string OrientationToString(ToolWindowOrientation position)
    {
      switch (position)
      {
        case ToolWindowOrientation.none:
          return string.Empty;
        case ToolWindowOrientation.Top:
          return "Top";
        case ToolWindowOrientation.Left:
          return "Left";
        case ToolWindowOrientation.Right:
          return "Right";
        case ToolWindowOrientation.Bottom:
          return "Bottom";
        default:
          throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Attributes_UnknownPosition, (object) position));
      }
    }
  }
}
