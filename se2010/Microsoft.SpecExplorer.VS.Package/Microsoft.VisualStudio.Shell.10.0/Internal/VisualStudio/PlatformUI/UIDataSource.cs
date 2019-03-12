// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.UIDataSource
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class UIDataSource : UISimpleDataSource, IVsUIDataSource, IVsUISimpleDataSource, IVsUIDispatch, IUIDataSource, IUIDispatch
  {
    internal IDictionary<string, UIDataSourceProperty> properties = (IDictionary<string, UIDataSourceProperty>) new Dictionary<string, UIDataSourceProperty>();
    internal VsUICookieTable<IVsUIDataSourcePropertyChangeEvents> eventSubscribers = new VsUICookieTable<IVsUIDataSourcePropertyChangeEvents>();
    private Guid guidMPFShapes = Guid.NewGuid();

    internal uint EventSubscribersSize
    {
      get
      {
        return this.eventSubscribers.Size;
      }
    }

    public int GetValue(string prop, out IVsUIObject ppValue)
    {
      if (prop == null)
        throw new ArgumentNullException(nameof (prop));
      if (prop.Length == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_EmptyString, (object) nameof (prop)));
      lock (this.properties)
      {
        UIDataSourceProperty dataSourceProperty;
        if (!this.properties.TryGetValue(prop, out dataSourceProperty))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_PropertyNotDefined, (object) prop));
        ppValue = dataSourceProperty.Value;
        return 0;
      }
    }

    public void SetValue(string prop, object newValue)
    {
      Utilities.SetValue((IVsUIDataSource) this, prop, newValue);
    }

    int IVsUIDataSource.SetValue(string prop, IVsUIObject newValue)
    {
      if (newValue == null)
        throw new ArgumentNullException(nameof (newValue));
      if (prop == null)
        throw new ArgumentNullException(nameof (prop));
      if (prop.Length == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_EmptyString, (object) nameof (prop)));
      this.ValidatePropertyValue(prop, newValue);
      UIDataSourceProperty dataSourceProperty;
      lock (this.properties)
      {
        if (!this.properties.TryGetValue(prop, out dataSourceProperty))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_PropertyNotDefined, (object) prop));
      }
      // ISSUE: variable of a compiler-generated type
      IVsUIObject vsUiObject = this.GetValue(prop);
      bool pfAreEqual;
      int num;
      // ISSUE: reference to a compiler-generated method
      if ((num = newValue.Equals(vsUiObject, out pfAreEqual)) != 0)
        return num;
      dataSourceProperty.Value = newValue;
      if (object.ReferenceEquals((object) vsUiObject, (object) newValue))
        return 1;
      // ISSUE: reference to a compiler-generated method
      if (newValue.Equals(vsUiObject, out pfAreEqual) != 0)
        throw new ArgumentException(Resources.Error_CannotCheckEqual);
      if (pfAreEqual)
        return 1;
      this.NotifyEventSubscribers(prop, vsUiObject, newValue);
      return 0;
    }

    public int EnumProperties(out IVsUIEnumDataSourceProperties ppEnum)
    {
      ppEnum = (IVsUIEnumDataSourceProperties) new UIDataSourcePropertyEnumerator(this.properties.Values);
      return 0;
    }

    public int AdvisePropertyChangeEvents(
      IVsUIDataSourcePropertyChangeEvents pAdvise,
      out uint pCookie)
    {
      if (pAdvise == null)
        throw new ArgumentNullException(nameof (pAdvise));
      pCookie = this.eventSubscribers.Insert(pAdvise);
      return 0;
    }

    public int UnadvisePropertyChangeEvents(uint cookie)
    {
      if (!this.eventSubscribers.Remove(cookie))
        throw new ArgumentException(Resources.Error_InvalidCookieValue, nameof (cookie));
      return 0;
    }

    public int GetShapeIdentifier(out Guid guid, out uint dw)
    {
      guid = this.guidMPFShapes;
      dw = (uint) this.GetHashCode();
      return 0;
    }

    public int QueryValue(string prop, string[] pTypeName, uint[] pDataFormat, object[] pValue)
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIObject ppValue;
      int num = this.GetValue(prop, out ppValue);
      if (num != 0)
        return num;
      if (pTypeName != null)
        pTypeName[0] = Utilities.GetObjectType(ppValue);
      if (pDataFormat != null)
        pDataFormat[0] = (uint) Utilities.GetObjectFormat(ppValue);
      if (pValue != null)
        pValue[0] = Utilities.GetObjectData(ppValue);
      return 0;
    }

    public int ResetValue(string prop)
    {
      return -2147467263;
    }

    private bool IsClosableType(string type)
    {
      switch (type)
      {
        case "VsUI.Char":
        case "VsUI.Int16":
        case "VsUI.Int32":
        case "VsUI.Int64":
        case "VsUI.Byte":
        case "VsUI.Word":
        case "VsUI.DWord":
        case "VsUI.QWord":
        case "VsUI.Boolean":
        case "VsUI.String":
        case "VsUI.DateTime":
        case "VsUI.Single":
        case "VsUI.Double":
        case "VsUI.Decimal":
        case "VsUI.Bitmap":
        case "VsUI.Color":
        case "VsUI.Icon":
        case "VsUI.ImageList":
          return false;
        default:
          return true;
      }
    }

    public override int Close()
    {
      // ISSUE: reference to a compiler-generated method
      this.eventSubscribers.ForEach((CookieTableCallback<uint, IVsUIDataSourcePropertyChangeEvents>) ((cookie, subscriber) => subscriber.Disconnect((IVsUISimpleDataSource) this)));
      this.eventSubscribers.Clear();
      lock (this.properties)
      {
        foreach (UIDataSourceProperty dataSourceProperty in (IEnumerable<UIDataSourceProperty>) this.properties.Values)
        {
          if (this.IsClosableType(dataSourceProperty.Type))
          {
            // ISSUE: variable of a compiler-generated type
            IVsUIObject vsUiObject = dataSourceProperty.Value;
            object pVar;
            // ISSUE: reference to a compiler-generated method
            if (vsUiObject.get_Data(out pVar) == 0)
            {
              // ISSUE: variable of a compiler-generated type
              IVsUISimpleDataSource simpleDataSource = pVar as IVsUISimpleDataSource;
              // ISSUE: reference to a compiler-generated method
              simpleDataSource?.Close();
            }
          }
        }
      }
      return 0;
    }

    public T GetValue<T>(string name)
    {
      return (T) Utilities.GetValue((IVsUIDataSource) this, name);
    }

    public IVsUIObject GetValue(string name)
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIObject ppValue;
      this.GetValue(name, out ppValue);
      return ppValue;
    }

    public void SetValue(string name, IVsUIObject value)
    {
      // ISSUE: reference to a compiler-generated method
      ((IVsUIDataSource) this).SetValue(name, value);
    }

    public IVsUIObject this[string name]
    {
      get
      {
        return this.GetValue(name);
      }
      set
      {
        this.SetValue(name, value);
      }
    }

    public ShapeIdentifier ShapeIdentifier
    {
      get
      {
        return new ShapeIdentifier((IVsUIDataSource) this);
      }
    }

    public IEnumerable<IPropertyDescription> Properties
    {
      get
      {
        lock (this.properties)
        {
          IList<IPropertyDescription> propertyDescriptionList = (IList<IPropertyDescription>) new List<IPropertyDescription>();
          foreach (UIDataSourceProperty dataSourceProperty in (IEnumerable<UIDataSourceProperty>) this.properties.Values)
            propertyDescriptionList.Add((IPropertyDescription) dataSourceProperty);
          return (IEnumerable<IPropertyDescription>) propertyDescriptionList;
        }
      }
    }

    private void NotifyEventSubscribers(string prop, IVsUIObject oldValue, IVsUIObject newValue)
    {
      // ISSUE: reference to a compiler-generated method
      this.eventSubscribers.ForEach((CookieTableCallback<uint, IVsUIDataSourcePropertyChangeEvents>) ((cookie, subscriber) => UIDataSource.ThrowIfFailed(subscriber.OnPropertyChanged((IVsUIDataSource) this, prop, oldValue, newValue))));
    }

    public void AddBuiltInProperty(string name, object value)
    {
      if (value is IVsUIObject)
        throw new ArgumentException(Resources.Error_AddBuiltInPropertyUsedForUIObject, nameof (value));
      // ISSUE: variable of a compiler-generated type
      IVsUIObject initialValue = (IVsUIObject) new BuiltInPropertyValue(value);
      this.AddProperty(name, initialValue);
    }

    public void AddUnknownProperty(string name, object value)
    {
      if (value is IVsUIObject)
        throw new ArgumentException(Resources.Error_AddBuiltInPropertyUsedForUIObject, nameof (value));
      // ISSUE: variable of a compiler-generated type
      IVsUIObject unknownValue = BuiltInPropertyValue.CreateUnknownValue(value);
      this.AddProperty(name, unknownValue);
    }

    public void AddDispatchProperty(string name, object value)
    {
      if (value is IVsUIObject)
        throw new ArgumentException(Resources.Error_AddBuiltInPropertyUsedForUIObject, nameof (value));
      // ISSUE: variable of a compiler-generated type
      IVsUIObject dispatchValue = BuiltInPropertyValue.CreateDispatchValue(value);
      this.AddProperty(name, dispatchValue);
    }

    public void AddIndirectProperty<T>(string name, string type, GetterThunk<T> getterThunk)
    {
      if (string.IsNullOrEmpty(type))
        throw new ArgumentNullException(nameof (type));
      if (getterThunk == null)
        throw new ArgumentNullException(nameof (getterThunk));
      // ISSUE: variable of a compiler-generated type
      IVsUIObject initialValue = (IVsUIObject) new IndirectPropertyValue<T>(getterThunk, type, 0U);
      this.AddProperty(name, initialValue);
    }

    public void AddProperty(string name, IVsUIObject initialValue)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      lock (this.properties)
      {
        UIDataSourceProperty dataSourceProperty;
        if (this.properties.TryGetValue(name, out dataSourceProperty))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_DuplicateProperty, (object) name));
        dataSourceProperty = new UIDataSourceProperty(name, initialValue);
        this.properties[name] = dataSourceProperty;
      }
    }

    public virtual void ValidatePropertyValue(string name, IVsUIObject value)
    {
    }

    private static void ThrowIfFailed(int hr)
    {
      Marshal.ThrowExceptionForHR(hr);
    }
  }
}
