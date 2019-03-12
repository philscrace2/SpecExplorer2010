// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.EnvironmentRenderCapabilities
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.PlatformUI
{
  public sealed class EnvironmentRenderCapabilities : DisposableObject, INotifyPropertyChanged, IVsShellPropertyEvents
  {
    private static EnvironmentRenderCapabilities current;
    private int visualEffectsAllowed;
    private bool areGradientsAllowed;
    private bool areAnimationsAllowed;
    private uint shellPropertyChangesCookie;

    public event EventHandler RenderCapabilitiesChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    public static EnvironmentRenderCapabilities Current
    {
      get
      {
        return EnvironmentRenderCapabilities.current ?? (EnvironmentRenderCapabilities.current = new EnvironmentRenderCapabilities());
      }
    }

    private EnvironmentRenderCapabilities()
    {
      IVsShell service = ServiceProvider.GlobalProvider.GetService(typeof (SVsShell)) as IVsShell;
      if (service == null)
        return;
      object pvar;
      if (ErrorHandler.Succeeded(service.GetProperty(-9061, out pvar)))
        this.VisualEffectsAllowed = (int) pvar;
      service.AdviseShellPropertyChanges((IVsShellPropertyEvents) this, out this.shellPropertyChangesCookie);
    }

    protected override void DisposeManagedResources()
    {
      if (this.shellPropertyChangesCookie != 0U)
      {
        (ServiceProvider.GlobalProvider.GetService(typeof (SVsShell)) as IVsShell)?.UnadviseShellPropertyChanges(this.shellPropertyChangesCookie);
        this.shellPropertyChangesCookie = 0U;
      }
      base.DisposeManagedResources();
    }

    public int OnShellPropertyChange(int propid, object var)
    {
      this.ThrowIfDisposed();
      if (propid == -9061)
        this.VisualEffectsAllowed = (int) var;
      return 0;
    }

    public int VisualEffectsAllowed
    {
      get
      {
        return this.visualEffectsAllowed;
      }
      set
      {
        if (this.visualEffectsAllowed == value)
          return;
        this.visualEffectsAllowed = value;
        this.AreGradientsAllowed = (value & 2) == 2;
        this.AreAnimationsAllowed = (value & 1) == 1;
        this.RaisePropertyChanged(nameof (VisualEffectsAllowed));
        this.RenderCapabilitiesChanged.RaiseEvent((object) this, EventArgs.Empty);
      }
    }

    public bool AreGradientsAllowed
    {
      get
      {
        return this.areGradientsAllowed;
      }
      private set
      {
        if (this.areGradientsAllowed == value)
          return;
        this.areGradientsAllowed = value;
        this.RaisePropertyChanged(nameof (AreGradientsAllowed));
      }
    }

    public bool AreAnimationsAllowed
    {
      get
      {
        return this.areAnimationsAllowed;
      }
      private set
      {
        if (this.areAnimationsAllowed == value)
          return;
        this.areAnimationsAllowed = value;
        this.RaisePropertyChanged(nameof (AreAnimationsAllowed));
      }
    }

    private void RaisePropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
