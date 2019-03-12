// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.DataSource
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public class DataSource : IDataSource, IUIDataSource, IUIDispatch, IVsUIDataSource, IVsUISimpleDataSource, IVsUIDispatch, INotifyPropertyChanged, ICustomTypeDescriptor, IDisposable
  {
    private static ShapeIdentifierMap shapeIdentifierMap = new ShapeIdentifierMap();
    internal VsUICookieTable<IVsUIDataSourcePropertyChangeEvents> eventSubscribers = new VsUICookieTable<IVsUIDataSourcePropertyChangeEvents>();
    private Dictionary<string, IVsUIObject> propertyCache = new Dictionary<string, IVsUIObject>();
    private IVsUIDataSource innerSource;
    private DataSourceParameters parameters;
    private uint cookiePropertyChangeHandler;
    private IEnumerable<IPropertyDescription> propertyList;
    private bool _disposed;

    internal uint EventSubscribersSize
    {
      get
      {
        return this.eventSubscribers.Size;
      }
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals(obj, (object) this))
        return true;
      if (obj == null)
        return false;
      if ((object) (obj as DataSource) != null)
        return this.RootInnerSource == ((DataSource) obj).RootInnerSource;
      if (obj is IVsUIDataSource)
        return this.RootInnerSource == (IVsUIDataSource) obj;
      return false;
    }

    public static bool operator ==(DataSource left, DataSource right)
    {
      if ((object) left == null)
        return (object) right == null;
      return left.Equals((object) right);
    }

    public static bool operator ==(DataSource left, IVsUIDataSource right)
    {
      if ((object) left == null)
        return right == null;
      return left.Equals((object) right);
    }

    public static bool operator ==(IVsUIDataSource left, DataSource right)
    {
      if ((object) right == null)
        return left == null;
      return right.Equals((object) left);
    }

    public static bool operator !=(DataSource left, DataSource right)
    {
      return !(left == right);
    }

    public static bool operator !=(DataSource left, IVsUIDataSource right)
    {
      return !(left == right);
    }

    public static bool operator !=(IVsUIDataSource left, DataSource right)
    {
      return !(left == right);
    }

    private IVsUIDataSource RootInnerSource
    {
      get
      {
        IVsUIDataSource innerSource = this.innerSource;
        while ((object) (innerSource as DataSource) != null)
          innerSource = ((DataSource) innerSource).innerSource;
        return innerSource;
      }
    }

    public override int GetHashCode()
    {
      if (this.innerSource == null)
        return 0;
      return this.innerSource.GetHashCode();
    }

    public DataSource(IVsUIDataSource uiDataSource)
      : this(uiDataSource, new DataSourceParameters())
    {
    }

    public DataSource(IVsUIDataSource uiDataSource, DataSourceParameters parameters)
    {
      if (uiDataSource == null)
        throw new ArgumentNullException(nameof (uiDataSource));
      if (parameters == null)
        throw new ArgumentNullException(nameof (parameters));
      this.innerSource = uiDataSource;
      this.parameters = parameters;
      this.SubscribeToVisualElementEvents();
      this.SubscribeInnerSourcePropertyChangeEvents();
    }

    ~DataSource()
    {
      this.Dispose(false);
    }

    private void visualWindow_Closed(object sender, EventArgs e)
    {
      this.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public bool IsDisposed
    {
      get
      {
        return this._disposed;
      }
    }

    private void ThrowIfDisposed()
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!this.IsDisposed)
      {
        this.UnsubscribeFromVisualElementEvents();
        this.DisconnectEventSubscribers();
        this.UnsubscribeInnerSourcePropertyChangeEvents();
        this.innerSource = (IVsUIDataSource) null;
        if (disposing)
        {
          using (this.GetPropertyCacheSynchronizer())
          {
            foreach (IVsUIObject vsUiObject in this.propertyCache.Values)
            {
              if (vsUiObject != null)
              {
                using (Utilities.GetObjectData(vsUiObject) as IDisposable)
                  ;
              }
            }
          }
        }
      }
      this._disposed = true;
    }

    public int AdvisePropertyChangeEvents(
      IVsUIDataSourcePropertyChangeEvents pAdvise,
      out uint pCookie)
    {
      this.ThrowIfDisposed();
      if (pAdvise == null)
        throw new ArgumentNullException(nameof (pAdvise));
      pCookie = this.eventSubscribers.Insert(pAdvise);
      return 0;
    }

    public int UnadvisePropertyChangeEvents(uint cookie)
    {
      this.ThrowIfDisposed();
      if (!this.eventSubscribers.Remove(cookie))
        throw new ArgumentException(Resources.Error_InvalidCookieValue, nameof (cookie));
      return 0;
    }

    public int EnumProperties(out IVsUIEnumDataSourceProperties ppEnum)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      return this.innerSource.EnumProperties(out ppEnum);
    }

    public int EnumVerbs(out IVsUIEnumDataSourceVerbs ppEnum)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      return this.innerSource.EnumVerbs(out ppEnum);
    }

    public int Invoke(string verb, object pvaIn, out object pvaOut)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      return this.innerSource.Invoke(verb, pvaIn, out pvaOut);
    }

    public int GetValue(string prop, out IVsUIObject ppValue)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      return this.innerSource.GetValue(prop, out ppValue);
    }

    int IVsUIDataSource.SetValue(string prop, IVsUIObject pValue)
    {
      this.ThrowIfDisposed();
      string pTypeName;
      // ISSUE: reference to a compiler-generated method
      int num = pValue.get_Type(out pTypeName);
      if (num == 0 && pTypeName.Equals("VsUI.Collection"))
        throw new ArgumentException(Resources.Error_CannotChangeDataSourceCollection);
      if (num == 0)
      {
        using (this.GetPropertyCacheSynchronizer())
        {
          // ISSUE: variable of a compiler-generated type
          IVsUIObject cachedValue = this.RemoveCachedValue(prop);
          num = -2147467259;
          try
          {
            // ISSUE: reference to a compiler-generated method
            num = this.innerSource.SetValue(prop, pValue);
          }
          finally
          {
            if (cachedValue != null)
            {
              if (num != 0)
              {
                this.propertyCache[prop] = cachedValue;
              }
              else
              {
                using (this.GetDisposableItemFromCachedValue(cachedValue))
                  ;
              }
            }
          }
        }
      }
      return num;
    }

    public int GetShapeIdentifier(out Guid guid, out uint dw)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      return this.innerSource.GetShapeIdentifier(out guid, out dw);
    }

    public int QueryValue(string prop, string[] pTypeName, uint[] pDataFormat, object[] pValue)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      return this.innerSource.QueryValue(prop, pTypeName, pDataFormat, pValue);
    }

    public int ResetValue(string prop)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      return this.innerSource.ResetValue(prop);
    }

    IVsUIObject IUIDataSource.GetValue(string name)
    {
      this.ThrowIfDisposed();
      // ISSUE: variable of a compiler-generated type
      IVsUIObject ppValue;
      // ISSUE: reference to a compiler-generated method
      this.GetValue(name, out ppValue);
      return ppValue;
    }

    void IUIDataSource.SetValue(string name, IVsUIObject value)
    {
      this.ThrowIfDisposed();
      // ISSUE: reference to a compiler-generated method
      ((IVsUIDataSource) this).SetValue(name, value);
    }

    IVsUIObject IUIDataSource.this[string name]
    {
      get
      {
        this.ThrowIfDisposed();
        return ((IUIDataSource) this).GetValue(name);
      }
      set
      {
        this.ThrowIfDisposed();
        ((IUIDataSource) this).SetValue(name, value);
      }
    }

    public object Invoke(string verbName, object parameter)
    {
      this.ThrowIfDisposed();
      object pvaOut;
      // ISSUE: reference to a compiler-generated method
      int errorCode = this.innerSource.Invoke(verbName, parameter, out pvaOut);
      if (errorCode != 0)
        Marshal.ThrowExceptionForHR(errorCode);
      return pvaOut;
    }

    public ShapeIdentifier ShapeIdentifier
    {
      get
      {
        this.ThrowIfDisposed();
        return new ShapeIdentifier(this.innerSource);
      }
    }

    public IEnumerable<IPropertyDescription> Properties
    {
      get
      {
        this.ThrowIfDisposed();
        if (this.propertyList == null)
          this.BuildPropertyList();
        return this.propertyList;
      }
    }

    public IEnumerable<IVerbDescription> Verbs
    {
      get
      {
        this.ThrowIfDisposed();
        IVsUIEnumDataSourceVerbs ppEnum;
        Marshal.ThrowExceptionForHR(this.innerSource.EnumVerbs(out ppEnum));
        if (ppEnum == null)
          throw new InvalidOperationException();
        return DataSource.GetCOMVerbEnumerator(ppEnum);
      }
    }

    private static IEnumerable<IVerbDescription> GetCOMVerbEnumerator(
      IVsUIEnumDataSourceVerbs verbEnumerator)
    {
      foreach (string name in ComUtilities.EnumerableFrom(verbEnumerator))
        yield return (IVerbDescription) new VerbDescription(name);
    }

    public int Close()
    {
      this.ThrowIfDisposed();
      this.Dispose();
      return 0;
    }

    AttributeCollection ICustomTypeDescriptor.GetAttributes()
    {
      return new AttributeCollection((Attribute[]) null);
    }

    string ICustomTypeDescriptor.GetClassName()
    {
      return typeof (DataSource).Name;
    }

    string ICustomTypeDescriptor.GetComponentName()
    {
      return (string) null;
    }

    TypeConverter ICustomTypeDescriptor.GetConverter()
    {
      return (TypeConverter) null;
    }

    EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
    {
      return (EventDescriptor) null;
    }

    PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
    {
      return (PropertyDescriptor) null;
    }

    object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
    {
      return (object) null;
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents(
      Attribute[] attributes)
    {
      return (EventDescriptorCollection) null;
    }

    EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
    {
      return (EventDescriptorCollection) null;
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(
      Attribute[] attributes)
    {
      return ((ICustomTypeDescriptor) this).GetProperties();
    }

    PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
    {
      if (this.IsDisposed)
        return new PropertyDescriptorCollection((PropertyDescriptor[]) null);
      return DataSource.shapeIdentifierMap.GetPropertyDescriptors((IUIDataSource) this);
    }

    object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
    {
      return (object) this;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void SubscribeToVisualElementEvents()
    {
      Window visualElement = this.parameters.visualElement as Window;
      if (visualElement == null)
        return;
      visualElement.Closed += new EventHandler(this.visualWindow_Closed);
    }

    private void UnsubscribeFromVisualElementEvents()
    {
      if (this.parameters == null)
        return;
      Window visualElement = this.parameters.visualElement as Window;
      if (visualElement == null)
        return;
      visualElement.Closed -= new EventHandler(this.visualWindow_Closed);
    }

    private void SubscribeInnerSourcePropertyChangeEvents()
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIDataSourcePropertyChangeEvents pAdvise = (IVsUIDataSourcePropertyChangeEvents) new DataSource.VsUIDataSourcePropertyChangeEvents(this);
      // ISSUE: reference to a compiler-generated method
      this.innerSource.AdvisePropertyChangeEvents(pAdvise, out this.cookiePropertyChangeHandler);
    }

    private void UnsubscribeInnerSourcePropertyChangeEvents()
    {
      if (this.cookiePropertyChangeHandler == 0U)
        return;
      try
      {
        // ISSUE: reference to a compiler-generated method
        this.innerSource.UnadvisePropertyChangeEvents(this.cookiePropertyChangeHandler);
        this.cookiePropertyChangeHandler = 0U;
      }
      catch (Exception ex)
      {
      }
    }

    private void DisconnectEventSubscribers()
    {
      this.eventSubscribers.ForEach((CookieTableCallback<uint, IVsUIDataSourcePropertyChangeEvents>) ((cookie, subscriber) =>
      {
        this.ThrowIfDisposed();
        // ISSUE: reference to a compiler-generated method
        subscriber.Disconnect((IVsUISimpleDataSource) this);
      }));
      this.eventSubscribers.Clear();
    }

    private void RaisePropertyChanged(string propertyName, IVsUIObject varOld, IVsUIObject varNew)
    {
      using (this.GetPropertyCacheSynchronizer())
      {
        using (this.GetDisposableItemFromCachedValue(this.RemoveCachedValue(propertyName)))
        {
          if (this.PropertyChanged != null)
            this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
          // ISSUE: reference to a compiler-generated method
          this.eventSubscribers.ForEach((CookieTableCallback<uint, IVsUIDataSourcePropertyChangeEvents>) ((cookie, subscriber) => DataSource.ThrowIfFailed(subscriber.OnPropertyChanged((IVsUIDataSource) this, propertyName, varOld, varNew))));
        }
      }
    }

    public void SetValue(string name, object value)
    {
      this.ThrowIfDisposed();
      if ((object) (value as DataSourceCollection) != null)
        throw new ArgumentException(Resources.Error_CannotChangeDataSourceCollection);
      // ISSUE: variable of a compiler-generated type
      IVsUIObject vsUiObject1 = value as IVsUIObject;
      if (vsUiObject1 != null)
      {
        ((IUIDataSource) this).SetValue(name, vsUiObject1);
      }
      else
      {
        // ISSUE: variable of a compiler-generated type
        IVsUIObject ppValue;
        // ISSUE: reference to a compiler-generated method
        this.innerSource.GetValue(name, out ppValue);
        string objectType = Utilities.GetObjectType(ppValue);
        if (Utilities.GetObjectFormat(ppValue) != __VSUIDATAFORMAT.VSDF_BUILTIN)
          throw new ArgumentException(Resources.Error_PropertyValueNeedsToBeVsUIObject);
        // ISSUE: variable of a compiler-generated type
        IVsUIObject vsUiObject2 = (IVsUIObject) new BuiltInPropertyValue(!(value is DataSource) ? value : (object) ((DataSource) value).innerSource);
        if (!Utilities.GetObjectType(vsUiObject2).Equals(objectType, StringComparison.Ordinal))
          throw new ArgumentException(Resources.Error_PropertyValueTypeMismatch);
        Utilities.SetValue((IVsUIDataSource) this, name, vsUiObject2);
      }
    }

    public object GetValue(string name)
    {
      this.ThrowIfDisposed();
      // ISSUE: variable of a compiler-generated type
      IVsUIObject valueForPresentation;
      using (this.GetPropertyCacheSynchronizer())
      {
        if (!this.propertyCache.TryGetValue(name, out valueForPresentation))
        {
          object varData = (object) null;
          bool dataRetrieved = false;
          valueForPresentation = this.GetValueForPresentation(name, out varData, out dataRetrieved);
          this.propertyCache[name] = valueForPresentation;
          if (dataRetrieved)
            return varData;
        }
      }
      return Utilities.GetObjectData(valueForPresentation);
    }

    private IVsUIObject RemoveCachedValue(string name)
    {
      // ISSUE: variable of a compiler-generated type
      IVsUIObject vsUiObject;
      if (this.propertyCache.TryGetValue(name, out vsUiObject))
        this.propertyCache.Remove(name);
      return vsUiObject;
    }

    private IVsUIObject GetValueForPresentation(
      string name,
      out object varData,
      out bool dataRetrieved)
    {
      varData = (object) null;
      dataRetrieved = false;
      // ISSUE: variable of a compiler-generated type
      IVsUIObject ppValue;
      // ISSUE: reference to a compiler-generated method
      DataSource.ThrowIfFailed(this.innerSource.GetValue(name, out ppValue));
      // ISSUE: variable of a compiler-generated type
      __VSUIDATAFORMAT objectFormat = Utilities.GetObjectFormat(ppValue);
      switch (objectFormat)
      {
        case __VSUIDATAFORMAT.VSDF_BUILTIN:
          varData = Utilities.GetObjectData(ppValue);
          if (varData != null)
          {
            string objectType = Utilities.GetObjectType(ppValue);
            if (objectType.Equals("VsUI.DataSource") && varData is IVsUIDataSource)
            {
              if ((object) (varData as DataSource) == null)
                return (IVsUIObject) new BuiltInPropertyValue((object) new DataSource((IVsUIDataSource) varData, this.parameters));
            }
            else if (objectType.Equals("VsUI.Collection") && varData is IVsUICollection && (object) (varData as DataSourceCollection) == null)
              return (IVsUIObject) new BuiltInPropertyValue((object) DataSourceCollection.CreateInstance((IVsUICollection) varData, this.parameters));
          }
          dataRetrieved = true;
          goto case __VSUIDATAFORMAT.VSDF_WPF;
        case __VSUIDATAFORMAT.VSDF_WPF:
          return ppValue;
        default:
          if (this.parameters.serviceProvider == null)
            throw new ArgumentException(Resources.Error_NoServiceProvider);
          // ISSUE: variable of a compiler-generated type
          IVsUIDataConverterManager service = this.parameters.serviceProvider.GetService(typeof (SVsUIDataConverters)) as IVsUIDataConverterManager;
          if (service == null)
            throw new ArgumentException(Resources.Error_NoConverterManager);
          // ISSUE: variable of a compiler-generated type
          IVsUIDataConverter ppConverter;
          // ISSUE: reference to a compiler-generated method
          if (service.GetObjectConverter(ppValue, 3U, out ppConverter) != 0 || ppConverter == null)
            throw new ArgumentException(Resources.Error_NoConverterAvailable);
          // ISSUE: variable of a compiler-generated type
          IVsUIObject ppConvertedObject;
          // ISSUE: reference to a compiler-generated method
          if (ppConverter.Convert(ppValue, out ppConvertedObject) != 0 || ppConvertedObject == null)
            throw new ArgumentException(Resources.Error_ConversionFailed);
          return ppConvertedObject;
      }
    }

    private IDisposable GetDisposableItemFromCachedValue(IVsUIObject cachedValue)
    {
      if (cachedValue == null)
        return (IDisposable) null;
      return Utilities.GetObjectData(cachedValue) as IDisposable;
    }

    private void BuildPropertyList()
    {
      UIDataSource innerSource = this.innerSource as UIDataSource;
      if (innerSource != null)
      {
        this.propertyList = innerSource.Properties;
      }
      else
      {
        // ISSUE: variable of a compiler-generated type
        IVsUIEnumDataSourceProperties ppEnum;
        // ISSUE: reference to a compiler-generated method
        this.innerSource.EnumProperties(out ppEnum);
        if (ppEnum == null)
          throw new InvalidOperationException();
        List<IPropertyDescription> propertyDescriptionList = new List<IPropertyDescription>();
        foreach (VsUIPropertyDescriptor propertyDescriptor in ComUtilities.EnumerableFrom(ppEnum))
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          PropertyDescription propertyDescription = new PropertyDescription(propertyDescriptor.name, propertyDescriptor.type);
          propertyDescriptionList.Add((IPropertyDescription) propertyDescription);
        }
        this.propertyList = (IEnumerable<IPropertyDescription>) propertyDescriptionList;
      }
    }

    private static void ThrowIfFailed(int hr)
    {
      Marshal.ThrowExceptionForHR(hr);
    }

    private IDisposable GetPropertyCacheSynchronizer()
    {
      if (this.parameters.isPropertyAccessSynchronized)
        return (IDisposable) new SyncObjectLock(((ICollection) this.propertyCache).SyncRoot);
      return (IDisposable) null;
    }

    internal class DataSourcePropertyDescriptor : PropertyDescriptor
    {
      private IPropertyDescription propertyDescription;

      public DataSourcePropertyDescriptor(IPropertyDescription propertyDescription)
        : base(propertyDescription.Name, (Attribute[]) null)
      {
        this.propertyDescription = propertyDescription;
      }

      public override Type ComponentType
      {
        get
        {
          return typeof (DataSource);
        }
      }

      public override bool IsReadOnly
      {
        get
        {
          return false;
        }
      }

      public override Type PropertyType
      {
        get
        {
          if (this.propertyDescription.Type.Equals("VsUI.DataSource", StringComparison.Ordinal))
            return typeof (DataSource);
          if (this.propertyDescription.Type.Equals("VsUI.Collection", StringComparison.Ordinal))
            return typeof (DataSourceCollection);
          Type type = PropertyDescription.TypeFromVsUIType(this.propertyDescription.Type);
          if (type != (Type) null)
            return type;
          return typeof (IVsUIObject);
        }
      }

      public override bool CanResetValue(object component)
      {
        return false;
      }

      public override void ResetValue(object component)
      {
        throw new NotSupportedException();
      }

      public override bool ShouldSerializeValue(object component)
      {
        return true;
      }

      public override object GetValue(object component)
      {
        return ((DataSource) component).GetValue(this.propertyDescription.Name);
      }

      public override void SetValue(object component, object value)
      {
        if (value == null)
        {
          if (!PropertyDescription.IsBuiltInUIType(this.propertyDescription.Type))
            throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_CannotSetPropertyNullValue_Format, (object) this.propertyDescription.Name, (object) this.propertyDescription.Type));
          value = this.propertyDescription.Type.Equals("VsUI.DataSource", StringComparison.Ordinal) || this.propertyDescription.Type.Equals("VsUI.Collection", StringComparison.Ordinal) ? (object) BuiltInPropertyValue.CreateEmptyValue(this.propertyDescription.Type) : (!this.propertyDescription.Type.Equals("VsUI.Dispatch", StringComparison.Ordinal) ? (!this.propertyDescription.Type.Equals("VsUI.Unknown", StringComparison.Ordinal) ? (!this.propertyDescription.Type.Equals("VsUI.String", StringComparison.Ordinal) ? Activator.CreateInstance(this.PropertyType) : (object) "") : (object) BuiltInPropertyValue.CreateUnknownValue((object) new UnknownWrapper((object) null))) : (object) BuiltInPropertyValue.CreateDispatchValue((object) new DispatchWrapper((object) null)));
        }
        ((DataSource) component).SetValue(this.propertyDescription.Name, value);
      }
    }

    private class VsUIDataSourcePropertyChangeEvents : IVsUIDataSourcePropertyChangeEvents, IVsUIEventSink
    {
      private DataSource owner;

      public VsUIDataSourcePropertyChangeEvents(DataSource owner)
      {
        this.owner = owner;
      }

      public int OnPropertyChanged(
        IVsUIDataSource ds,
        string prop,
        IVsUIObject varOld,
        IVsUIObject varNew)
      {
        this.owner.RaisePropertyChanged(prop, varOld, varNew);
        return 0;
      }

      public int Disconnect(IVsUISimpleDataSource pSource)
      {
        this.owner.UnsubscribeInnerSourcePropertyChangeEvents();
        return 0;
      }
    }
  }
}
