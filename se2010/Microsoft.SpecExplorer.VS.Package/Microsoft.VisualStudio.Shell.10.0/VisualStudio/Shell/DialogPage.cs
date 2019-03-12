// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.DialogPage
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Microsoft.VisualStudio.Shell
{
  [ComVisible(true)]
  [CLSCompliant(false)]
  public class DialogPage : Component, IWin32Window, IProfileManager
  {
    private IWin32Window _window;
    private DialogPage.DialogSubclass _subclass;
    private DialogPage.DialogContainer _container;
    private string _settingsPath;
    private bool _initializing;
    private bool _uiActive;
    private bool _propertyChangedHooked;
    private EventHandler _onPropertyChanged;

    public DialogPage()
    {
      this.HookProperties(true);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual object AutomationObject
    {
      get
      {
        return (object) this;
      }
    }

    public override ISite Site
    {
      get
      {
        return base.Site;
      }
      set
      {
        if (value == null)
        {
          ISite site = base.Site;
        }
        base.Site = value;
        if (value == null)
          return;
        this.LoadSettingsFromStorage();
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    protected virtual IWin32Window Window
    {
      get
      {
        PropertyGrid propertyGrid = new PropertyGrid();
        propertyGrid.Location = new Point(0, 0);
        propertyGrid.ToolbarVisible = false;
        propertyGrid.CommandsVisibleIfAvailable = false;
        propertyGrid.SelectedObject = this.AutomationObject;
        return (IWin32Window) propertyGrid;
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this._container != null)
        {
          try
          {
            this._container.Dispose();
          }
          catch (Exception ex)
          {
          }
          this._container = (DialogPage.DialogContainer) null;
        }
        if (this._window != null)
        {
          if (this._window is IDisposable)
          {
            try
            {
              ((IDisposable) this._window).Dispose();
            }
            catch (Exception ex)
            {
            }
            this._window = (IWin32Window) null;
          }
        }
        if (this._subclass != null)
          this._subclass = (DialogPage.DialogSubclass) null;
        this.HookProperties(false);
      }
      base.Dispose(disposing);
    }

    public virtual void LoadSettingsFromStorage()
    {
      this._initializing = true;
      try
      {
        Package service = (Package) this.GetService(typeof (Package));
        if (service != null)
        {
          using (RegistryKey userRegistryRoot = service.UserRegistryRoot)
          {
            string settingsRegistryPath = this.SettingsRegistryPath;
            object automationObject = this.AutomationObject;
            RegistryKey registryKey = userRegistryRoot.OpenSubKey(settingsRegistryPath, false);
            if (registryKey != null)
            {
              using (registryKey)
              {
                string[] valueNames = registryKey.GetValueNames();
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(automationObject);
                foreach (string name in valueNames)
                {
                  string text = registryKey.GetValue(name).ToString();
                  PropertyDescriptor propertyDescriptor = properties[name];
                  if (propertyDescriptor != null && propertyDescriptor.Converter.CanConvertFrom(typeof (string)))
                    propertyDescriptor.SetValue(automationObject, propertyDescriptor.Converter.ConvertFromInvariantString(text));
                }
              }
            }
          }
        }
      }
      finally
      {
        this._initializing = false;
      }
      this.HookProperties(true);
    }

    public virtual void LoadSettingsFromXml(IVsSettingsReader reader)
    {
      this._initializing = true;
      try
      {
        object automationObject = this.AutomationObject;
        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(automationObject, new Attribute[1]
        {
          (Attribute) DesignerSerializationVisibilityAttribute.Visible
        }))
        {
          TypeConverter converter = property.Converter;
          if (converter.CanConvertTo(typeof (string)) && converter.CanConvertFrom(typeof (string)))
          {
            string pbstrSettingValue = (string) null;
            object obj = (object) null;
            try
            {
              if (Microsoft.VisualStudio.NativeMethods.Succeeded(reader.ReadSettingString(property.Name, out pbstrSettingValue)))
              {
                if (pbstrSettingValue != null)
                  obj = property.Converter.ConvertFromInvariantString(pbstrSettingValue);
              }
            }
            catch (Exception ex)
            {
            }
            if (obj != null)
              property.SetValue(automationObject, obj);
          }
        }
      }
      finally
      {
        this._initializing = false;
      }
      this.HookProperties(true);
    }

    public virtual void ResetSettings()
    {
    }

    private void HookProperties(bool hook)
    {
      if (this._propertyChangedHooked == hook)
        return;
      if (this._onPropertyChanged == null)
        this._onPropertyChanged = new EventHandler(this.OnPropertyChanged);
      object component = (object) null;
      try
      {
        component = this.AutomationObject;
      }
      catch (Exception ex)
      {
      }
      if (component == null)
        return;
      foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(component, new Attribute[1]
      {
        (Attribute) DesignerSerializationVisibilityAttribute.Visible
      }))
      {
        if (hook)
          property.AddValueChanged(component, this._onPropertyChanged);
        else
          property.RemoveValueChanged(component, this._onPropertyChanged);
      }
      this._propertyChangedHooked = hook;
    }

    private void OnPropertyChanged(object sender, EventArgs e)
    {
      if (this._initializing || this._uiActive)
        return;
      this.SaveSettingsToStorage();
    }

    protected virtual void OnActivate(CancelEventArgs e)
    {
      this._uiActive = true;
    }

    protected virtual void OnClosed(EventArgs e)
    {
      this._uiActive = false;
      this.LoadSettingsFromStorage();
    }

    protected virtual void OnDeactivate(CancelEventArgs e)
    {
    }

    protected virtual void OnApply(DialogPage.PageApplyEventArgs e)
    {
      this.SaveSettingsToStorage();
    }

    public virtual void SaveSettingsToStorage()
    {
      Package service = (Package) this.GetService(typeof (Package));
      if (service == null)
        return;
      using (RegistryKey userRegistryRoot = service.UserRegistryRoot)
      {
        string settingsRegistryPath = this.SettingsRegistryPath;
        object automationObject = this.AutomationObject;
        RegistryKey registryKey = userRegistryRoot.OpenSubKey(settingsRegistryPath, true) ?? userRegistryRoot.CreateSubKey(settingsRegistryPath);
        using (registryKey)
        {
          foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(automationObject, new Attribute[1]
          {
            (Attribute) DesignerSerializationVisibilityAttribute.Visible
          }))
          {
            TypeConverter converter = property.Converter;
            if (converter.CanConvertTo(typeof (string)) && converter.CanConvertFrom(typeof (string)))
              registryKey.SetValue(property.Name, (object) converter.ConvertToInvariantString(property.GetValue(automationObject)));
          }
        }
      }
    }

    public virtual void SaveSettingsToXml(IVsSettingsWriter writer)
    {
      object automationObject = this.AutomationObject;
      PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(automationObject, new Attribute[1]
      {
        (Attribute) DesignerSerializationVisibilityAttribute.Visible
      });
      ArrayList arrayList = new ArrayList();
      foreach (PropertyDescriptor propertyDescriptor in properties)
        arrayList.Add((object) propertyDescriptor.Name);
      arrayList.Sort();
      foreach (string index in arrayList)
      {
        PropertyDescriptor propertyDescriptor = properties[index];
        TypeConverter converter = propertyDescriptor.Converter;
        if (converter.CanConvertTo(typeof (string)) && converter.CanConvertFrom(typeof (string)))
          Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(writer.WriteSettingString(propertyDescriptor.Name, converter.ConvertToInvariantString(propertyDescriptor.GetValue(automationObject))));
      }
    }

    protected string SettingsRegistryPath
    {
      get
      {
        if (this._settingsPath == null)
          this._settingsPath = "DialogPage\\" + this.AutomationObject.GetType().FullName;
        return this._settingsPath;
      }
      set
      {
        this._settingsPath = value;
      }
    }

    IntPtr IWin32Window.Handle
    {
      get
      {
        if (this._window == null)
        {
          this._window = this.Window;
          if (this._window is IComponent)
          {
            if (this._container == null)
              this._container = new DialogPage.DialogContainer((System.IServiceProvider) this.Site);
            this._container.Add((IComponent) this._window);
          }
          if (this._subclass == null)
            this._subclass = new DialogPage.DialogSubclass(this);
        }
        if (this._subclass.Handle != this._window.Handle)
          this._subclass.AssignHandle(this._window.Handle);
        return this._window.Handle;
      }
    }

    internal void ResetContainer()
    {
      if (this._container == null || !(this._window is IComponent))
        return;
      this._container._ambientProperties = (AmbientProperties) null;
      this._container.Remove((IComponent) this._window);
      this._container.Add((IComponent) this._window);
    }

    public enum ApplyKind
    {
      Apply,
      Cancel,
      CancelNoNavigate,
    }

    protected class PageApplyEventArgs : EventArgs
    {
      private DialogPage.ApplyKind _apply;

      public DialogPage.ApplyKind ApplyBehavior
      {
        get
        {
          return this._apply;
        }
        set
        {
          this._apply = value;
        }
      }
    }

    private sealed class DialogContainer : Container
    {
      private System.IServiceProvider _provider;
      internal AmbientProperties _ambientProperties;

      public DialogContainer(System.IServiceProvider provider)
      {
        this._provider = provider;
      }

      protected override object GetService(System.Type serviceType)
      {
        if (serviceType == (System.Type) null)
          throw new ArgumentNullException(nameof (serviceType));
        if (serviceType.IsEquivalentTo(typeof (AmbientProperties)))
        {
          if (this._ambientProperties == null)
          {
            IUIService service = this.GetService(typeof (IUIService)) as IUIService;
            this._ambientProperties = new AmbientProperties();
            this._ambientProperties.Font = (Font) service.Styles[(object) "DialogFont"];
          }
          return (object) this._ambientProperties;
        }
        if (this._provider != null)
        {
          object service = this._provider.GetService(serviceType);
          if (service != null)
            return service;
        }
        return base.GetService(serviceType);
      }
    }

    private sealed class DialogSubclass : NativeWindow
    {
      private DialogPage _page;
      private bool _closeCalled;

      internal DialogSubclass(DialogPage page)
      {
        this._page = page;
        this._closeCalled = false;
      }

      protected override void WndProc(ref Message m)
      {
        switch (m.Msg)
        {
          case 2:
            if (!this._closeCalled && this._page != null)
            {
              this._page.OnClosed(EventArgs.Empty);
              break;
            }
            break;
          case 78:
            switch (((Microsoft.VisualStudio.NativeMethods.NMHDR) Marshal.PtrToStructure(m.LParam, typeof (Microsoft.VisualStudio.NativeMethods.NMHDR))).code)
            {
              case -203:
                this._closeCalled = true;
                this._page.OnClosed(EventArgs.Empty);
                return;
              case -202:
                DialogPage.PageApplyEventArgs e1 = new DialogPage.PageApplyEventArgs();
                this._page.OnApply(e1);
                switch (e1.ApplyBehavior)
                {
                  case DialogPage.ApplyKind.Cancel:
                    m.Result = (IntPtr) 1;
                    break;
                  case DialogPage.ApplyKind.CancelNoNavigate:
                    m.Result = (IntPtr) 2;
                    break;
                  default:
                    m.Result = IntPtr.Zero;
                    break;
                }
                Microsoft.VisualStudio.UnsafeNativeMethods.SetWindowLong(m.HWnd, 0, m.Result);
                return;
              case -201:
                CancelEventArgs e2 = new CancelEventArgs();
                this._page.OnDeactivate(e2);
                m.Result = (IntPtr) (e2.Cancel ? 1 : 0);
                Microsoft.VisualStudio.UnsafeNativeMethods.SetWindowLong(m.HWnd, 0, m.Result);
                return;
              case -200:
                this._closeCalled = false;
                CancelEventArgs e3 = new CancelEventArgs();
                this._page.OnActivate(e3);
                m.Result = (IntPtr) (e3.Cancel ? -1 : 0);
                Microsoft.VisualStudio.UnsafeNativeMethods.SetWindowLong(m.HWnd, 0, m.Result);
                return;
            }
        }
        base.WndProc(ref m);
      }
    }
  }
}
