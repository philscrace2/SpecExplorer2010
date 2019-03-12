// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.Package
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Microsoft.VisualStudio.Shell
{
  [CLSCompliant(false)]
  [ComVisible(true)]
  [PackageRegistration]
  public abstract class Package : IVsPackage, Microsoft.VisualStudio.OLE.Interop.IServiceProvider, Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget, IVsPersistSolutionOpts, IServiceContainer, System.IServiceProvider, IVsUserSettings, IVsUserSettingsMigration, IVsToolWindowFactory, IVsToolboxItemProvider
  {
    private ServiceCollection<object> _services = new ServiceCollection<object>();
    private Dictionary<string, System.Windows.Forms.IDataObject> _tbxItemDataCache = new Dictionary<string, System.Windows.Forms.IDataObject>();
    private ServiceProvider _provider;
    private Hashtable _editorFactories;
    private Hashtable _projectFactories;
    private ToolWindowCollection _toolWindows;
    private Container _componentToolWindows;
    private Container _pagesAndProfiles;
    private ArrayList _optionKeys;
    private bool zombie;

    protected Package()
    {
      ServiceCreatorCallback callback = new ServiceCreatorCallback(this.OnCreateService);
      ((IServiceContainer) this).AddService(typeof (IMenuCommandService), callback);
      ((IServiceContainer) this).AddService(typeof (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget), callback);
    }

    protected event EventHandler ToolboxInitialized;

    protected event EventHandler ToolboxUpgraded;

    public RegistryKey ApplicationRegistryRoot
    {
      get
      {
        return VSRegistry.RegistryRoot((System.IServiceProvider) this._provider, __VsLocalRegistryType.RegType_Configuration, false);
      }
    }

    public string UserDataPath
    {
      get
      {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), this.GetRegistryRoot().Substring("SOFTWARE\\".Length));
      }
    }

    public string UserLocalDataPath
    {
      get
      {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), this.GetRegistryRoot().Substring("SOFTWARE\\".Length));
      }
    }

    public RegistryKey UserRegistryRoot
    {
      get
      {
        return VSRegistry.RegistryRoot((System.IServiceProvider) this._provider, __VsLocalRegistryType.RegType_UserSettings, true);
      }
    }

    protected void AddOptionKey(string name)
    {
      if (this.zombie)
        Marshal.ThrowExceptionForHR(-2147418113);
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      if (name.IndexOf('.') != -1 || name.Length > 31)
        throw new ArgumentException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Package_BadOptionName, (object) name));
      if (this._optionKeys == null)
        this._optionKeys = new ArrayList();
      else if (this._optionKeys.Contains((object) name))
        throw new ArgumentException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Package_OptionNameUsed, (object) name));
      this._optionKeys.Add((object) name);
    }

    int IVsUserSettings.ExportSettings(string strPageGuid, IVsSettingsWriter writer)
    {
      this.GetProfileManager(new Guid(strPageGuid), Package.ProfileManagerLoadAction.LoadPropsFromRegistry)?.SaveSettingsToXml(writer);
      return 0;
    }

    int IVsUserSettingsMigration.MigrateSettings(
      IVsSettingsReader reader,
      IVsSettingsWriter writer,
      string strPageGuid)
    {
      Guid objectGuid = Guid.Empty;
      try
      {
        objectGuid = new Guid(strPageGuid);
      }
      catch (FormatException ex)
      {
      }
      (!(objectGuid == Guid.Empty) ? this.GetProfileManager(objectGuid, Package.ProfileManagerLoadAction.None) as IProfileMigrator : this.GetAutomationObject(strPageGuid) as IProfileMigrator)?.MigrateSettings(reader, writer);
      return 0;
    }

    int IVsUserSettings.ImportSettings(
      string strPageGuid,
      IVsSettingsReader reader,
      uint flags,
      ref int restartRequired)
    {
      if (restartRequired > 0)
        restartRequired = 0;
      bool flag = ((int) flags & 1) == 0;
      IProfileManager profileManager = this.GetProfileManager(new Guid(strPageGuid), flag ? Package.ProfileManagerLoadAction.LoadPropsFromRegistry : Package.ProfileManagerLoadAction.ResetSettings);
      if (profileManager != null)
      {
        profileManager.LoadSettingsFromXml(reader);
        profileManager.SaveSettingsToStorage();
      }
      return 0;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this._editorFactories != null)
      {
        Hashtable editorFactories = this._editorFactories;
        this._editorFactories = (Hashtable) null;
        try
        {
          IVsRegisterEditors service = this.GetService(typeof (SVsRegisterEditors)) as IVsRegisterEditors;
          foreach (DictionaryEntry dictionaryEntry in editorFactories)
          {
            try
            {
              service?.UnregisterEditor((uint) dictionaryEntry.Value);
            }
            catch (Exception ex)
            {
            }
            finally
            {
              (dictionaryEntry.Key as IDisposable)?.Dispose();
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
      if (this._projectFactories != null)
      {
        Hashtable projectFactories = this._projectFactories;
        this._projectFactories = (Hashtable) null;
        try
        {
          IVsRegisterProjectTypes service = this.GetService(typeof (SVsRegisterProjectTypes)) as IVsRegisterProjectTypes;
          foreach (DictionaryEntry dictionaryEntry in projectFactories)
          {
            try
            {
              service?.UnregisterProjectType((uint) dictionaryEntry.Value);
            }
            finally
            {
              (dictionaryEntry.Key as IDisposable)?.Dispose();
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
      if (this._componentToolWindows != null)
      {
        Container componentToolWindows = this._componentToolWindows;
        this._componentToolWindows = (Container) null;
        try
        {
          componentToolWindows.Dispose();
        }
        catch (Exception ex)
        {
        }
      }
      if (this._pagesAndProfiles != null)
      {
        Container pagesAndProfiles = this._pagesAndProfiles;
        this._pagesAndProfiles = (Container) null;
        try
        {
          pagesAndProfiles.Dispose();
        }
        catch (Exception ex)
        {
        }
      }
      if (this._services != null)
      {
        if (this._services.Count > 0)
        {
          try
          {
            IProfferService service = (IProfferService) this.GetService(typeof (SProfferService));
            ServiceCollection<object> services = this._services;
            this._services = (ServiceCollection<object>) null;
            foreach (object obj1 in services.Values)
            {
              object obj2 = obj1;
              Package.ProfferedService profferedService = obj2 as Package.ProfferedService;
              try
              {
                if (profferedService != null)
                {
                  obj2 = profferedService.Instance;
                  if (profferedService.Cookie != 0U)
                  {
                    if (service != null)
                    {
                      if (Microsoft.VisualStudio.NativeMethods.Failed(service.RevokeService(profferedService.Cookie)))
                        Trace.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Failed to unregister service {0}", (object) obj2.GetType().FullName));
                    }
                  }
                }
              }
              finally
              {
                if (obj2 is IDisposable)
                  ((IDisposable) obj2).Dispose();
              }
            }
          }
          catch (Exception ex)
          {
          }
        }
      }
      if (this._provider != null)
      {
        try
        {
          this._provider.Dispose();
        }
        catch (Exception ex)
        {
        }
        this._provider = (ServiceProvider) null;
      }
      if (this._toolWindows != null)
      {
        this._toolWindows.Dispose();
        this._toolWindows = (ToolWindowCollection) null;
      }
      if (this._optionKeys != null)
        this._optionKeys = (ArrayList) null;
      SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(this.OnUserPreferenceChanged);
    }

    protected virtual object GetAutomationObject(string name)
    {
      if (this.zombie)
        Marshal.ThrowExceptionForHR(-2147418113);
      if (name == null)
        return (object) null;
      string[] strArray = name.Split('.');
      if (strArray.Length != 2)
        return (object) null;
      strArray[0] = strArray[0].Trim();
      strArray[1] = strArray[1].Trim();
      foreach (Attribute attribute in TypeDescriptor.GetAttributes((object) this))
      {
        ProvideOptionPageAttribute optionPageAttribute = attribute as ProvideOptionPageAttribute;
        if (optionPageAttribute != null && optionPageAttribute.SupportsAutomation && (string.Compare(optionPageAttribute.CategoryName, strArray[0], StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(optionPageAttribute.PageName, strArray[1], StringComparison.OrdinalIgnoreCase) == 0))
          return this.GetDialogPage(optionPageAttribute.PageType).AutomationObject;
      }
      return (object) null;
    }

    protected DialogPage GetDialogPage(System.Type dialogPageType)
    {
      if (this.zombie)
        Marshal.ThrowExceptionForHR(-2147418113);
      if (dialogPageType == (System.Type) null)
        throw new ArgumentNullException(nameof (dialogPageType));
      if (!typeof (DialogPage).IsAssignableFrom(dialogPageType))
        throw new ArgumentException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Package_BadDialogPageType, (object) dialogPageType.FullName));
      if (this._pagesAndProfiles != null)
      {
        foreach (object component in (ReadOnlyCollectionBase) this._pagesAndProfiles.Components)
        {
          if (component.GetType() == dialogPageType)
            return (DialogPage) component;
        }
      }
      ConstructorInfo constructor = dialogPageType.GetConstructor(new System.Type[0]);
      if (constructor == (ConstructorInfo) null)
        throw new ArgumentException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Package_PageCtorMissing, (object) dialogPageType.FullName));
      DialogPage dialogPage = (DialogPage) constructor.Invoke(new object[0]);
      if (this._pagesAndProfiles == null)
        this._pagesAndProfiles = (Container) new Package.PackageContainer((System.IServiceProvider) this);
      this._pagesAndProfiles.Add((IComponent) dialogPage);
      return dialogPage;
    }

    private IProfileManager GetProfileManager(
      Guid objectGuid,
      Package.ProfileManagerLoadAction loadAction)
    {
      IProfileManager profileManager = (IProfileManager) null;
      if (objectGuid == Guid.Empty)
        throw new ArgumentNullException(nameof (objectGuid));
      if (this._pagesAndProfiles != null)
      {
        foreach (object component in (ReadOnlyCollectionBase) this._pagesAndProfiles.Components)
        {
          if (component.GetType().GUID.Equals(objectGuid))
          {
            if (component is IProfileManager)
            {
              profileManager = component as IProfileManager;
              if (profileManager != null)
              {
                switch (loadAction)
                {
                  case Package.ProfileManagerLoadAction.LoadPropsFromRegistry:
                    profileManager.LoadSettingsFromStorage();
                    goto label_15;
                  case Package.ProfileManagerLoadAction.ResetSettings:
                    profileManager.ResetSettings();
                    goto label_15;
                  default:
                    goto label_15;
                }
              }
              else
                break;
            }
            else
              break;
          }
        }
      }
label_15:
      if (profileManager == null)
      {
        foreach (Attribute attribute in TypeDescriptor.GetAttributes((object) this))
        {
          if (attribute is ProvideProfileAttribute)
          {
            System.Type objectType = ((ProvideProfileAttribute) attribute).ObjectType;
            if (objectType.GUID.Equals(objectGuid))
            {
              ConstructorInfo constructor = objectType.GetConstructor(new System.Type[0]);
              if (constructor == (ConstructorInfo) null)
                throw new ArgumentException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Package_PageCtorMissing, (object) objectType.FullName));
              profileManager = (IProfileManager) constructor.Invoke(new object[0]);
              if (profileManager != null)
              {
                if (this._pagesAndProfiles == null)
                  this._pagesAndProfiles = (Container) new Package.PackageContainer((System.IServiceProvider) this);
                this._pagesAndProfiles.Add((IComponent) profileManager);
                break;
              }
              break;
            }
          }
        }
      }
      return profileManager;
    }

    private string GetRegistryRoot()
    {
      IVsShell service = (IVsShell) this.GetService(typeof (SVsShell));
      string str;
      if (service == null)
      {
        DefaultRegistryRootAttribute attribute = (DefaultRegistryRootAttribute) TypeDescriptor.GetAttributes(this.GetType())[typeof (DefaultRegistryRootAttribute)];
        if (attribute == null)
          throw new NotSupportedException();
        str = "SOFTWARE\\Microsoft\\VisualStudio\\" + attribute.Root;
      }
      else
      {
        object pvar;
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service.GetProperty(-9002, out pvar));
        str = pvar.ToString();
      }
      return str;
    }

    protected object GetService(System.Type serviceType)
    {
      if (this.zombie)
        return (object) null;
      if (serviceType == (System.Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      if (serviceType.IsEquivalentTo(typeof (IServiceContainer)) || serviceType.IsEquivalentTo(typeof (Package)) || serviceType.IsEquivalentTo(this.GetType()))
        return (object) this;
      object obj = (object) null;
      if (this._services != null && this._services.Count > 0)
      {
        lock (serviceType)
        {
          if (this._services.ContainsKey(serviceType))
            obj = this._services[serviceType];
          if (obj is Package.ProfferedService)
            obj = ((Package.ProfferedService) obj).Instance;
          if (obj is ServiceCreatorCallback)
          {
            this._services[serviceType] = (object) null;
            obj = ((ServiceCreatorCallback) obj)((IServiceContainer) this, serviceType);
            if (obj == null)
            {
              string text = "An object was not returned from a service creator callback for the registered type of " + serviceType.Name + ".  This may mean that it failed a type equivalence comparison.  To compare type objects you must use Type.IsEquivalentTo(Type).  Do not use .Equals or the == operator.";
              IVsAppCommandLine service = this.GetService(typeof (SVsAppCommandLine)) as IVsAppCommandLine;
              if (service != null)
              {
                int pfPresent = 0;
                string pbstrOptionValue;
                service.GetOption("RootSuffix", out pfPresent, out pbstrOptionValue);
                if (pfPresent == 1 && string.Compare(pbstrOptionValue, "Exp", StringComparison.OrdinalIgnoreCase) == 0)
                {
                  int num = (int) MessageBox.Show(text);
                }
              }
            }
            else if (!obj.GetType().IsCOMObject && !serviceType.IsAssignableFrom(obj.GetType()))
              obj = (object) null;
            this._services[serviceType] = obj;
          }
        }
      }
      if (obj == null && this._provider != null && (this._services == null || this._services.Count == 0 || !this._services.ContainsKey(serviceType)))
        obj = this._provider.GetService(serviceType);
      return obj;
    }

    protected virtual void Initialize()
    {
      if (this._services.Count > 0)
      {
        IProfferService service1 = (IProfferService) this.GetService(typeof (SProfferService));
        if (service1 != null)
        {
          foreach (KeyValuePair<System.Type, object> service2 in (Dictionary<System.Type, object>) this._services)
          {
            Package.ProfferedService profferedService = service2.Value as Package.ProfferedService;
            if (profferedService != null)
            {
              Guid guid = service2.Key.GUID;
              uint pdwCookie;
              Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service1.ProfferService(ref guid, (Microsoft.VisualStudio.OLE.Interop.IServiceProvider) this, out pdwCookie));
              profferedService.Cookie = pdwCookie;
            }
          }
        }
      }
      Thread.CurrentThread.CurrentUICulture = new CultureInfo(this.GetProviderLocale());
      SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(this.OnUserPreferenceChanged);
      if (this._optionKeys == null)
        return;
      try
      {
        IVsSolutionPersistence service = (IVsSolutionPersistence) this.GetService(typeof (SVsSolutionPersistence));
        if (service == null)
          return;
        foreach (string optionKey in this._optionKeys)
          service.LoadPackageUserOpts((IVsPersistSolutionOpts) this, optionKey);
      }
      catch (Exception ex)
      {
      }
    }

    protected virtual int QueryClose(out bool canClose)
    {
      canClose = true;
      return 0;
    }

    public int GetProviderLocale()
    {
      int num = CultureInfo.CurrentCulture.LCID;
      IUIHostLocale service = (IUIHostLocale) this.GetService(typeof (IUIHostLocale));
      if (service != null)
      {
        uint plcid;
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service.GetUILocale(out plcid));
        num = (int) plcid;
      }
      return num;
    }

    public object CreateInstance(ref Guid clsid, ref Guid iid, System.Type type)
    {
      object obj = (object) null;
      IntPtr instance = this.CreateInstance(ref clsid, ref iid);
      if (instance != IntPtr.Zero)
      {
        try
        {
          obj = Marshal.GetTypedObjectForIUnknown(instance, type);
        }
        finally
        {
          Marshal.Release(instance);
        }
      }
      else
        obj = Activator.CreateInstance(type);
      return obj;
    }

    private IntPtr CreateInstance(ref Guid clsid, ref Guid iid)
    {
      IntPtr ppvObj;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure((this.GetService(typeof (SLocalRegistry)) as ILocalRegistry3).CreateInstance(clsid, (object) null, ref iid, 1U, out ppvObj));
      return ppvObj;
    }

    public IVsOutputWindowPane GetOutputPane(Guid page, string caption)
    {
      IVsOutputWindow service = this.GetService(typeof (SVsOutputWindow)) as IVsOutputWindow;
      IVsOutputWindowPane ppPane = (IVsOutputWindowPane) null;
      if (Microsoft.VisualStudio.NativeMethods.Failed(service.GetPane(ref page, out ppPane)) && caption != null && Microsoft.VisualStudio.NativeMethods.Succeeded(service.CreatePane(ref page, caption, 1, 1)))
        service.GetPane(ref page, out ppPane);
      if (ppPane != null)
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(ppPane.Activate());
      return ppPane;
    }

    private object OnCreateService(IServiceContainer container, System.Type serviceType)
    {
      if (serviceType.IsEquivalentTo(typeof (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget)))
      {
        object service = this.GetService(typeof (IMenuCommandService));
        if (service is Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget)
          return service;
      }
      else if (serviceType.IsEquivalentTo(typeof (IMenuCommandService)))
        return (object) new OleMenuCommandService((System.IServiceProvider) this);
      return (object) null;
    }

    protected virtual void OnLoadOptions(string key, Stream stream)
    {
    }

    protected virtual void OnSaveOptions(string key, Stream stream)
    {
    }

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
      if (e.Category != UserPreferenceCategory.Locale)
        return;
      CultureInfo.CurrentCulture.ClearCachedData();
    }

    protected void ParseToolboxResource(
      TextReader resourceData,
      ResourceManager localizedCategories)
    {
      this.ParseToolboxResource(resourceData, localizedCategories, Guid.Empty);
    }

    protected void ParseToolboxResource(TextReader resourceData, Guid packageGuid)
    {
      this.ParseToolboxResource(resourceData, (ResourceManager) null, packageGuid);
    }

    private void ParseToolboxResource(
      TextReader resourceData,
      ResourceManager localizedCategories,
      Guid packageGuid)
    {
      if (resourceData == null)
        throw new ArgumentNullException(nameof (resourceData));
      IToolboxService service1 = this.GetService(typeof (IToolboxService)) as IToolboxService;
      if (service1 == null)
        throw new InvalidOperationException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.General_MissingService, (object) typeof (IToolboxService).FullName));
      IVsToolbox service2 = this.GetService(typeof (SVsToolbox)) as IVsToolbox;
      IVsToolbox2 vsToolbox2 = service2 as IVsToolbox2;
      IVsToolbox3 vsToolbox3 = service2 as IVsToolbox3;
      if (vsToolbox3 == null)
        throw new InvalidOperationException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.General_MissingService, (object) typeof (SVsToolbox).FullName));
      string str1 = resourceData.ReadLine();
      string str2 = (string) null;
      string str3 = (string) null;
      for (; str1 != null; str1 = resourceData.ReadLine())
      {
        try
        {
          string str4 = str1.Trim();
          if (str4.Length != 0)
          {
            if (!str4.StartsWith(";", StringComparison.OrdinalIgnoreCase))
            {
              if (str4.StartsWith("[", StringComparison.OrdinalIgnoreCase) && str4.EndsWith("]", StringComparison.OrdinalIgnoreCase))
              {
                str2 = str4.Trim('[', ']').Trim();
                string lpszTabID = str2;
                if (localizedCategories != null)
                {
                  string str5 = localizedCategories.GetString(str2);
                  if (str5 != null)
                    str2 = str5;
                }
                bool flag = false;
                if (!string.IsNullOrEmpty(str2))
                {
                  if (packageGuid != Guid.Empty && vsToolbox2 != null)
                  {
                    vsToolbox2.AddTab2(str2, ref packageGuid);
                    if (!string.IsNullOrEmpty(lpszTabID) && vsToolbox3 != null)
                    {
                      vsToolbox3.SetIDOfTab(str2, packageGuid.ToString("B") + "-" + lpszTabID);
                      lpszTabID = (string) null;
                    }
                    flag = true;
                  }
                  else if (service2 != null)
                  {
                    service2.AddTab(str2);
                    flag = true;
                  }
                  if (flag)
                  {
                    if (!string.IsNullOrEmpty(lpszTabID))
                    {
                      if (vsToolbox3 != null)
                      {
                        vsToolbox3.SetIDOfTab(str2, lpszTabID);
                        str3 = (string) null;
                      }
                    }
                  }
                }
              }
              else
              {
                int length = str4.IndexOf(",");
                if (length != -1)
                {
                  string name = str4.Substring(0, length).Trim();
                  string str5 = str4.Substring(length + 1).Trim();
                  if (str5.IndexOf(",") == -1)
                  {
                    IEnumerator enumerator = new AssemblyEnumerationService((System.IServiceProvider) this).GetAssemblyNames(str5).GetEnumerator();
                    try
                    {
                      if (enumerator.MoveNext())
                        str5 = ((AssemblyName) enumerator.Current).FullName;
                    }
                    finally
                    {
                      (enumerator as IDisposable)?.Dispose();
                    }
                  }
                  Assembly assembly = Assembly.Load(str5);
                  if (assembly != (Assembly) null)
                  {
                    System.Type type = assembly.GetType(name);
                    if (type != (System.Type) null)
                    {
                      ToolboxItem toolboxItem = ToolboxService.GetToolboxItem(type);
                      if (toolboxItem != null)
                      {
                        if (str2 == null)
                          service1.AddToolboxItem(toolboxItem);
                        else
                          service1.AddToolboxItem(toolboxItem, str2);
                      }
                    }
                  }
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
        }
      }
    }

    protected void RegisterEditorFactory(IVsEditorFactory factory)
    {
      IVsRegisterEditors service = this.GetService(typeof (SVsRegisterEditors)) as IVsRegisterEditors;
      if (service == null)
        throw new InvalidOperationException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Package_MissingService, (object) typeof (SVsRegisterEditors).FullName));
      Guid guid = factory.GetType().GUID;
      uint pdwCookie;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service.RegisterEditor(ref guid, factory, out pdwCookie));
      if (this._editorFactories == null)
        this._editorFactories = new Hashtable();
      this._editorFactories[(object) factory] = (object) pdwCookie;
    }

    protected void RegisterProjectFactory(IVsProjectFactory factory)
    {
      IVsRegisterProjectTypes service = this.GetService(typeof (SVsRegisterProjectTypes)) as IVsRegisterProjectTypes;
      if (service == null)
        throw new InvalidOperationException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Package_MissingService, (object) typeof (SVsRegisterProjectTypes).FullName));
      Guid guid = factory.GetType().GUID;
      uint pdwCookie;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service.RegisterProjectType(ref guid, factory, out pdwCookie));
      if (this._projectFactories == null)
        this._projectFactories = new Hashtable();
      this._projectFactories[(object) factory] = (object) pdwCookie;
    }

    public void ShowOptionPage(System.Type optionsPageType)
    {
      if (optionsPageType == (System.Type) null)
        throw new ArgumentNullException(nameof (optionsPageType));
      MenuCommandService service = this.GetService(typeof (IMenuCommandService)) as MenuCommandService;
      if (service == null)
        return;
      CommandID commandId = new CommandID(Microsoft.VisualStudio.NativeMethods.GUID_VSStandardCommandSet97, 264);
      service.GlobalInvoke(commandId, (object) optionsPageType.GUID.ToString());
    }

    int Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget.Exec(
      ref Guid guidGroup,
      uint nCmdId,
      uint nCmdExcept,
      IntPtr pIn,
      IntPtr vOut)
    {
      Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget service = (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget) this.GetService(typeof (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget));
      if (service != null)
        return service.Exec(ref guidGroup, nCmdId, nCmdExcept, pIn, vOut);
      return -2147221248;
    }

    int Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget.QueryStatus(
      ref Guid guidGroup,
      uint nCmdId,
      Microsoft.VisualStudio.OLE.Interop.OLECMD[] oleCmd,
      IntPtr oleText)
    {
      Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget service = (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget) this.GetService(typeof (Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget));
      if (service != null)
        return service.QueryStatus(ref guidGroup, nCmdId, oleCmd, oleText);
      return -2147221248;
    }

    int Microsoft.VisualStudio.OLE.Interop.IServiceProvider.QueryService(
      ref Guid sid,
      ref Guid iid,
      out IntPtr ppvObj)
    {
      ppvObj = (IntPtr) 0;
      int num = 0;
      object o = (object) null;
      if (this._services != null && this._services.Count > 0)
      {
        foreach (System.Type key in this._services.Keys)
        {
          if (key.GUID.Equals(sid))
          {
            o = this.GetService(key);
            break;
          }
        }
      }
      if (o == null)
        num = -2147467262;
      else if (iid.Equals(Microsoft.VisualStudio.NativeMethods.IID_IUnknown))
      {
        ppvObj = Marshal.GetIUnknownForObject(o);
      }
      else
      {
        IntPtr iunknownForObject = Marshal.GetIUnknownForObject(o);
        num = Marshal.QueryInterface(iunknownForObject, ref iid, out ppvObj);
        Marshal.Release(iunknownForObject);
      }
      return num;
    }

    void IServiceContainer.AddService(System.Type serviceType, object serviceInstance)
    {
      ((IServiceContainer) this).AddService(serviceType, serviceInstance, false);
    }

    void IServiceContainer.AddService(
      System.Type serviceType,
      object serviceInstance,
      bool promote)
    {
      if (serviceType == (System.Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      if (serviceInstance == null)
        throw new ArgumentNullException(nameof (serviceInstance));
      if (this._services.ContainsKey(serviceType))
        throw new InvalidOperationException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Package_DuplicateService, (object) serviceType.FullName));
      if (promote)
      {
        Package.ProfferedService profferedService = new Package.ProfferedService();
        profferedService.Instance = serviceInstance;
        if (this._provider == null)
          return;
        IProfferService service = (IProfferService) this.GetService(typeof (SProfferService));
        if (service == null)
          return;
        Guid guid = serviceType.GUID;
        uint pdwCookie;
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service.ProfferService(ref guid, (Microsoft.VisualStudio.OLE.Interop.IServiceProvider) this, out pdwCookie));
        profferedService.Cookie = pdwCookie;
        this._services[serviceType] = (object) profferedService;
      }
      else
        this._services[serviceType] = serviceInstance;
    }

    void IServiceContainer.AddService(
      System.Type serviceType,
      ServiceCreatorCallback callback)
    {
      ((IServiceContainer) this).AddService(serviceType, callback, false);
    }

    void IServiceContainer.AddService(
      System.Type serviceType,
      ServiceCreatorCallback callback,
      bool promote)
    {
      if (serviceType == (System.Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      if (callback == null)
        throw new ArgumentNullException(nameof (callback));
      if (this._services.ContainsKey(serviceType))
        throw new InvalidOperationException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Package_DuplicateService, (object) serviceType.FullName));
      if (promote)
      {
        Package.ProfferedService profferedService = new Package.ProfferedService();
        this._services[serviceType] = (object) profferedService;
        profferedService.Instance = (object) callback;
        if (this._provider == null)
          return;
        IProfferService service = (IProfferService) this.GetService(typeof (SProfferService));
        if (service == null)
          return;
        Guid guid = serviceType.GUID;
        uint pdwCookie;
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service.ProfferService(ref guid, (Microsoft.VisualStudio.OLE.Interop.IServiceProvider) this, out pdwCookie));
        profferedService.Cookie = pdwCookie;
      }
      else
        this._services[serviceType] = (object) callback;
    }

    void IServiceContainer.RemoveService(System.Type serviceType)
    {
      ((IServiceContainer) this).RemoveService(serviceType, false);
    }

    void IServiceContainer.RemoveService(System.Type serviceType, bool promote)
    {
      if (serviceType == (System.Type) null)
        throw new ArgumentNullException(nameof (serviceType));
      if (this._services == null || this._services.Count <= 0)
        return;
      object obj = (object) null;
      if (this._services.ContainsKey(serviceType))
        obj = this._services[serviceType];
      if (obj == null)
        return;
      this._services.Remove(serviceType);
      try
      {
        Package.ProfferedService profferedService = obj as Package.ProfferedService;
        if (profferedService == null)
          return;
        obj = profferedService.Instance;
        if (profferedService.Cookie == 0U)
          return;
        IProfferService service = (IProfferService) this.GetService(typeof (SProfferService));
        if (service != null)
          Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service.RevokeService(profferedService.Cookie));
        profferedService.Cookie = 0U;
      }
      finally
      {
        if (obj is IDisposable)
          ((IDisposable) obj).Dispose();
      }
    }

    object System.IServiceProvider.GetService(System.Type serviceType)
    {
      return this.GetService(serviceType);
    }

    int IVsPackage.Close()
    {
      if (!this.zombie)
        this.Dispose(true);
      this.zombie = true;
      return 0;
    }

    public int CreateTool(ref Guid persistenceSlot)
    {
      if (this.zombie)
        Marshal.ThrowExceptionForHR(-2147418113);
      return ((IVsToolWindowFactory) this).CreateToolWindow(ref persistenceSlot, 0U);
    }

    int IVsToolWindowFactory.CreateToolWindow(ref Guid toolWindowType, uint id)
    {
      if (id > (uint) int.MaxValue)
        throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Instance ID cannot be more then {0}", (object) int.MaxValue));
      int id1 = (int) id;
      foreach (Attribute customAttribute in Attribute.GetCustomAttributes((MemberInfo) this.GetType()))
      {
        if (customAttribute is ProvideToolWindowAttribute)
        {
          ProvideToolWindowAttribute tool = (ProvideToolWindowAttribute) customAttribute;
          if (tool.ToolType.GUID == toolWindowType)
          {
            this.FindToolWindow(tool.ToolType, id1, true, tool);
            break;
          }
        }
      }
      return 0;
    }

    protected WindowPane CreateToolWindow(System.Type toolWindowType, int id)
    {
      if (toolWindowType == (System.Type) null)
        throw new ArgumentNullException(nameof (toolWindowType));
      if (id < 0)
        throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Package_InvalidInstanceID, (object) id));
      if (!toolWindowType.IsSubclassOf(typeof (WindowPane)))
        throw new ArgumentException(Microsoft.VisualStudio.Shell.Resources.Package_InvalidToolWindowClass);
      foreach (Attribute customAttribute in Attribute.GetCustomAttributes((MemberInfo) this.GetType()))
      {
        if (customAttribute is ProvideToolWindowAttribute)
        {
          ProvideToolWindowAttribute tool = (ProvideToolWindowAttribute) customAttribute;
          if (tool.ToolType == toolWindowType)
            return this.CreateToolWindow(toolWindowType, id, tool);
        }
      }
      return (WindowPane) null;
    }

    private WindowPane CreateToolWindow(
      System.Type toolWindowType,
      int id,
      ProvideToolWindowAttribute tool)
    {
      if (toolWindowType == (System.Type) null)
        throw new ArgumentNullException(nameof (toolWindowType));
      if (id < 0)
        throw new ArgumentOutOfRangeException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Package_InvalidInstanceID, (object) id));
      if (!toolWindowType.IsSubclassOf(typeof (WindowPane)))
        throw new ArgumentException(Microsoft.VisualStudio.Shell.Resources.Package_InvalidToolWindowClass);
      if (tool == null)
        throw new ArgumentNullException(nameof (tool));
      WindowPane instance = (WindowPane) Activator.CreateInstance(toolWindowType);
      ToolWindowPane pane = instance as ToolWindowPane;
      bool flag = false;
      Guid empty = Guid.Empty;
      Guid rclsidTool = Guid.Empty;
      string pszCaption = (string) null;
      if (pane != null)
      {
        rclsidTool = pane.ToolClsid;
        pszCaption = pane.Caption;
        flag = pane.ToolBar != null;
        pane.Package = (object) this;
      }
      uint grfCTW = 65536;
      if (!tool.Transient)
        grfCTW |= 524288U;
      if (flag)
        grfCTW |= 4194304U;
      if (tool.MultiInstances)
        grfCTW |= 2097152U;
      object punkTool = (object) null;
      if (rclsidTool.CompareTo(Guid.Empty) == 0)
        punkTool = pane == null ? (object) instance : pane.GetIVsWindowPane();
      Guid guid1 = toolWindowType.GUID;
      IVsUIShell service = (IVsUIShell) this.GetService(typeof (SVsUIShell));
      if (service == null)
        throw new Exception(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.General_MissingService, (object) typeof (SVsUIShell).FullName));
      IVsWindowFrame ppWindowFrame;
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(service.CreateToolWindow(grfCTW, (uint) id, punkTool, ref rclsidTool, ref guid1, ref empty, (Microsoft.VisualStudio.OLE.Interop.IServiceProvider) null, pszCaption, (int[]) null, out ppWindowFrame));
      IComponent component = (instance.Content == null ? instance.Window as IComponent : instance.Content as IComponent) ?? punkTool as IComponent;
      if (component != null)
      {
        if (this._componentToolWindows == null)
          this._componentToolWindows = (Container) new Package.PackageContainer((System.IServiceProvider) this);
        this._componentToolWindows.Add(component);
      }
      if (pane != null)
        pane.Frame = (object) ppWindowFrame;
      if (flag && ppWindowFrame != null && pane != null)
      {
        object pvar;
        Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(ppWindowFrame.GetProperty(-5008, out pvar));
        // ISSUE: variable of a compiler-generated type
        IVsToolWindowToolbarHost2 windowToolbarHost2 = (IVsToolWindowToolbarHost2) pvar;
        if (windowToolbarHost2 != null)
        {
          Guid guid2 = pane.ToolBar.Guid;
          // ISSUE: reference to a compiler-generated method
          Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(windowToolbarHost2.AddToolbar2((VSTWT_LOCATION) pane.ToolBarLocation, ref guid2, (uint) pane.ToolBar.ID, pane.ToolBarDropTarget));
        }
        pane.OnToolBarAdded();
      }
      if (pane != null)
      {
        if (this._toolWindows == null)
          this._toolWindows = new ToolWindowCollection();
        this._toolWindows.Add(toolWindowType.GUID, id, pane);
      }
      return instance;
    }

    public ToolWindowPane FindToolWindow(System.Type toolWindowType, int id, bool create)
    {
      return this.FindToolWindow(toolWindowType, id, create, (ProvideToolWindowAttribute) null) as ToolWindowPane;
    }

    public WindowPane FindWindowPane(System.Type toolWindowType, int id, bool create)
    {
      return this.FindToolWindow(toolWindowType, id, create, (ProvideToolWindowAttribute) null);
    }

    private WindowPane FindToolWindow(
      System.Type toolWindowType,
      int id,
      bool create,
      ProvideToolWindowAttribute tool)
    {
      if (toolWindowType == (System.Type) null)
        throw new ArgumentNullException(nameof (toolWindowType));
      WindowPane windowPane = (WindowPane) null;
      if (this._toolWindows != null)
        windowPane = (WindowPane) this._toolWindows.GetToolWindowPane(toolWindowType.GUID, id);
      if (windowPane == null && create)
        windowPane = tool == null ? this.CreateToolWindow(toolWindowType, id) : this.CreateToolWindow(toolWindowType, id, tool);
      return windowPane;
    }

    int IVsPackage.GetAutomationObject(string propName, out object auto)
    {
      if (this.zombie)
        Marshal.ThrowExceptionForHR(-2147418113);
      auto = this.GetAutomationObject(propName);
      if (auto == null)
        Marshal.ThrowExceptionForHR(-2147467263);
      return 0;
    }

    int IVsPackage.GetPropertyPage(ref Guid rguidPage, VSPROPSHEETPAGE[] ppage)
    {
      if (ppage == null || ppage.Length < 1)
        throw new ArgumentException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.General_ArraySizeShouldBeAtLeast1), nameof (ppage));
      if (this.zombie)
        Marshal.ThrowExceptionForHR(-2147418113);
      IWin32Window win32Window1 = (IWin32Window) null;
      if (this._pagesAndProfiles != null)
      {
        foreach (object component in (ReadOnlyCollectionBase) this._pagesAndProfiles.Components)
        {
          if (component.GetType().GUID.Equals(rguidPage))
          {
            IWin32Window win32Window2 = component as IWin32Window;
            if (win32Window2 != null)
            {
              if (win32Window2 is DialogPage)
                ((DialogPage) win32Window2).ResetContainer();
              win32Window1 = win32Window2;
              break;
            }
          }
        }
      }
      if (win32Window1 == null)
      {
        DialogPage dialogPage = (DialogPage) null;
        foreach (Attribute attribute in TypeDescriptor.GetAttributes((object) this))
        {
          if (attribute is ProvideOptionDialogPageAttribute)
          {
            System.Type pageType = ((ProvideOptionDialogPageAttribute) attribute).PageType;
            if (pageType.GUID.Equals(rguidPage))
            {
              win32Window1 = (IWin32Window) this.GetDialogPage(pageType);
              break;
            }
          }
          if (dialogPage != null)
          {
            if (this._pagesAndProfiles == null)
              this._pagesAndProfiles = (Container) new Package.PackageContainer((System.IServiceProvider) this);
            this._pagesAndProfiles.Add((IComponent) dialogPage);
            break;
          }
        }
      }
      if (win32Window1 == null)
        Marshal.ThrowExceptionForHR(-2147467263);
      ppage[0].dwSize = (uint) Marshal.SizeOf(typeof (VSPROPSHEETPAGE));
      ppage[0].hwndDlg = win32Window1.Handle;
      ppage[0].dwFlags = 0U;
      ppage[0].HINSTANCE = 0U;
      ppage[0].dwTemplateSize = 0U;
      ppage[0].pTemplate = IntPtr.Zero;
      ppage[0].pfnDlgProc = IntPtr.Zero;
      ppage[0].lParam = IntPtr.Zero;
      ppage[0].pfnCallback = IntPtr.Zero;
      ppage[0].pcRefParent = IntPtr.Zero;
      ppage[0].dwReserved = 0U;
      ppage[0].wTemplateId = (ushort) 0;
      return 0;
    }

    int IVsPackage.QueryClose(out int close)
    {
      close = 1;
      bool canClose = true;
      int num = this.QueryClose(out canClose);
      if (!canClose)
        close = 0;
      return num;
    }

    int IVsPackage.ResetDefaults(uint grfFlags)
    {
      if (this.zombie)
        Marshal.ThrowExceptionForHR(-2147418113);
      switch (grfFlags)
      {
        case 1:
          if (this.ToolboxInitialized != null)
          {
            this.ToolboxInitialized((object) this, EventArgs.Empty);
            break;
          }
          break;
        case 2:
          if (this.ToolboxUpgraded != null)
          {
            this.ToolboxUpgraded((object) this, EventArgs.Empty);
            break;
          }
          break;
      }
      return 0;
    }

    int IVsPackage.SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp)
    {
      if (this.zombie)
        Marshal.ThrowExceptionForHR(-2147418113);
      if (sp != null)
      {
        if (this._provider != null)
          throw new InvalidOperationException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Package_SiteAlreadySet, (object) this.GetType().FullName));
        this._provider = ServiceProvider.CreateFromSetSite(sp);
        this.Initialize();
      }
      else if (this._provider != null)
        this.Dispose(true);
      return 0;
    }

    int IVsPersistSolutionOpts.LoadUserOptions(
      IVsSolutionPersistence pPersistance,
      uint options)
    {
      int hr = 0;
      if (((int) options & 1) != 0)
        return hr;
      if (this._optionKeys != null)
      {
        foreach (string optionKey in this._optionKeys)
        {
          hr = pPersistance.LoadPackageUserOpts((IVsPersistSolutionOpts) this, optionKey);
          if (Microsoft.VisualStudio.NativeMethods.Failed(hr))
            break;
        }
      }
      Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(hr);
      return hr;
    }

    int IVsPersistSolutionOpts.ReadUserOptions(Microsoft.VisualStudio.OLE.Interop.IStream pStream, string pszKey)
    {
      Microsoft.VisualStudio.NativeMethods.DataStreamFromComStream streamFromComStream = new Microsoft.VisualStudio.NativeMethods.DataStreamFromComStream(pStream);
      using (streamFromComStream)
        this.OnLoadOptions(pszKey, (Stream) streamFromComStream);
      return 0;
    }

    int IVsPersistSolutionOpts.SaveUserOptions(
      IVsSolutionPersistence pPersistance)
    {
      if (this._optionKeys != null)
      {
        foreach (string optionKey in this._optionKeys)
          Microsoft.VisualStudio.NativeMethods.ThrowOnFailure(pPersistance.SavePackageUserOpts((IVsPersistSolutionOpts) this, optionKey));
      }
      return 0;
    }

    int IVsPersistSolutionOpts.WriteUserOptions(
      Microsoft.VisualStudio.OLE.Interop.IStream pStream,
      string pszKey)
    {
      Microsoft.VisualStudio.NativeMethods.DataStreamFromComStream streamFromComStream = new Microsoft.VisualStudio.NativeMethods.DataStreamFromComStream(pStream);
      using (streamFromComStream)
        this.OnSaveOptions(pszKey, (Stream) streamFromComStream);
      return 0;
    }

    int IVsToolboxItemProvider.GetItemContent(
      string itemId,
      ushort format,
      out IntPtr global)
    {
      if (this.zombie)
        Marshal.ThrowExceptionForHR(-2147418113);
      object toolboxItemData = this.GetToolboxItemData(itemId, DataFormats.GetFormat((int) format));
      if (toolboxItemData == null)
      {
        global = IntPtr.Zero;
      }
      else
      {
        OleDataObject oleDataObject = new OleDataObject();
        oleDataObject.SetData(DataFormats.GetFormat((int) format).Name, toolboxItemData);
        FORMATETC[] pformatetcIn = new FORMATETC[1]
        {
          new FORMATETC()
        };
        pformatetcIn[0].cfFormat = format;
        pformatetcIn[0].dwAspect = 1U;
        pformatetcIn[0].lindex = -1;
        pformatetcIn[0].tymed = 1U;
        STGMEDIUM[] pRemoteMedium = new STGMEDIUM[1]
        {
          new STGMEDIUM()
        };
        pRemoteMedium[0].tymed = 1U;
        ((Microsoft.VisualStudio.OLE.Interop.IDataObject) oleDataObject).GetData(pformatetcIn, pRemoteMedium);
        global = pRemoteMedium[0].unionmember;
      }
      return 0;
    }

    protected virtual object GetToolboxItemData(string itemId, DataFormats.Format format)
    {
      if (string.IsNullOrEmpty(itemId))
        throw new ArgumentNullException(nameof (itemId));
      System.Windows.Forms.IDataObject dataObject;
      if (this._tbxItemDataCache.TryGetValue(itemId, out dataObject))
      {
        if (dataObject.GetDataPresent(format.Name))
          return dataObject.GetData(format.Name);
        throw new InvalidOperationException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Toolbox_UnsupportedFormat, (object) format.Name));
      }
      int length = itemId.IndexOf(",");
      if (length == -1)
        throw new InvalidOperationException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Toolbox_InvalidItemId, (object) itemId));
      string name = itemId.Substring(0, length).Trim();
      string str = itemId.Substring(length + 1).Trim();
      if (str.IndexOf(",") == -1)
      {
        IEnumerator enumerator = new AssemblyEnumerationService((System.IServiceProvider) this).GetAssemblyNames(str).GetEnumerator();
        try
        {
          if (enumerator.MoveNext())
            str = ((AssemblyName) enumerator.Current).FullName;
        }
        finally
        {
          (enumerator as IDisposable)?.Dispose();
        }
      }
      Assembly assembly = Assembly.Load(str);
      if (assembly != (Assembly) null)
      {
        System.Type type = assembly.GetType(name);
        if (type != (System.Type) null)
        {
          ToolboxItem toolboxItem = ToolboxService.GetToolboxItem(type);
          if (toolboxItem != null)
          {
            System.Windows.Forms.IDataObject toolboxData = new ToolboxItemContainer(toolboxItem).ToolboxData;
            this._tbxItemDataCache.Add(itemId, toolboxData);
            if (toolboxData.GetDataPresent(format.Name))
              return toolboxData.GetData(format.Name);
            throw new InvalidOperationException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Toolbox_UnsupportedFormat, (object) format.Name));
          }
        }
      }
      throw new InvalidOperationException(string.Format((IFormatProvider) Microsoft.VisualStudio.Shell.Resources.Culture, Microsoft.VisualStudio.Shell.Resources.Toolbox_ItemNotFound, (object) itemId));
    }

    public static object GetGlobalService(System.Type serviceType)
    {
      object obj = (object) null;
      ServiceProvider globalProvider = ServiceProvider.GlobalProvider;
      if (globalProvider != null)
        obj = globalProvider.GetService(serviceType);
      return obj;
    }

    public bool Zombied
    {
      get
      {
        return this.zombie;
      }
    }

    private enum ProfileManagerLoadAction
    {
      None,
      LoadPropsFromRegistry,
      ResetSettings,
    }

    private sealed class PackageContainer : Container
    {
      private IUIService _uis;
      private AmbientProperties _ambientProperties;
      private System.IServiceProvider _provider;

      internal PackageContainer(System.IServiceProvider provider)
      {
        this._provider = provider;
      }

      protected override object GetService(System.Type serviceType)
      {
        if (serviceType == (System.Type) null)
          throw new ArgumentNullException(nameof (serviceType));
        if (this._provider != null)
        {
          if (serviceType.IsEquivalentTo(typeof (AmbientProperties)))
          {
            if (this._uis == null)
              this._uis = (IUIService) this._provider.GetService(typeof (IUIService));
            if (this._ambientProperties == null)
              this._ambientProperties = new AmbientProperties();
            if (this._uis != null)
              this._ambientProperties.Font = (Font) this._uis.Styles[(object) "DialogFont"];
            return (object) this._ambientProperties;
          }
          object service = this._provider.GetService(serviceType);
          if (service != null)
            return service;
        }
        return base.GetService(serviceType);
      }
    }

    private sealed class ProfferedService
    {
      public object Instance;
      public uint Cookie;
    }
  }
}
