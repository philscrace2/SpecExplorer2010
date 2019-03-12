// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.AssemblyEnumerationService
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio
{
  internal sealed class AssemblyEnumerationService
  {
    private IServiceProvider provider;
    private IVsComponentEnumeratorFactory enumFactory;
    private Hashtable enumCache;
    private IEnumerable enumAllItems;

    internal AssemblyEnumerationService(IServiceProvider provider)
    {
      this.provider = provider;
    }

    private IVsComponentEnumeratorFactory EnumFactory
    {
      get
      {
        if (this.enumFactory == null && this.provider != null)
          this.enumFactory = (IVsComponentEnumeratorFactory) this.provider.GetService(typeof (SCompEnumService));
        return this.enumFactory;
      }
    }

    internal IEnumerable GetAssemblyNames()
    {
      IVsComponentEnumeratorFactory enumFactory = this.EnumFactory;
      if (enumFactory == null)
        return (IEnumerable) new AssemblyName[0];
      if (this.enumAllItems == null)
        this.enumAllItems = (IEnumerable) new AssemblyEnumerationService.VSAssemblyEnumerator(enumFactory, (string) null);
      return this.enumAllItems;
    }

    internal IEnumerable GetAssemblyNames(string name)
    {
      IVsComponentEnumeratorFactory enumFactory = this.EnumFactory;
      if (enumFactory == null)
        return (IEnumerable) new AssemblyName[0];
      IEnumerable enumerable = (IEnumerable) null;
      if (this.enumCache != null)
        enumerable = (IEnumerable) this.enumCache[(object) name];
      else
        this.enumCache = new Hashtable();
      if (enumerable == null)
      {
        enumerable = (IEnumerable) new AssemblyEnumerationService.VSAssemblyEnumerator(enumFactory, name);
        this.enumCache[(object) name] = (object) enumerable;
      }
      return enumerable;
    }

    private class VSAssemblyEnumerator : IEnumerable, IEnumerator
    {
      private IVsComponentEnumeratorFactory enumFactory;
      private string name;
      private AssemblyName currentName;
      private int current;
      private IEnumComponents componentEnum;
      private VSCOMPONENTSELECTORDATA[] selector;
      private ArrayList cachedNames;

      public VSAssemblyEnumerator(IVsComponentEnumeratorFactory enumFactory, string name)
      {
        this.enumFactory = enumFactory;
        this.current = -1;
        if (name == null)
          return;
        this.name = name + ".dll";
        Path.GetExtension(this.name);
      }

      object IEnumerator.Current
      {
        get
        {
          if (this.currentName == null)
            throw new InvalidOperationException();
          return (object) this.currentName;
        }
      }

      bool IEnumerator.MoveNext()
      {
        this.currentName = (AssemblyName) null;
        string assemblyFile = (string) null;
        if (this.cachedNames != null && this.current < this.cachedNames.Count - 1)
        {
          ++this.current;
          this.currentName = (AssemblyName) this.cachedNames[this.current];
          return true;
        }
        while (assemblyFile == null || this.currentName == null)
        {
          if (this.name == null)
          {
            if (this.componentEnum == null)
            {
              NativeMethods.ThrowOnFailure(this.enumFactory.GetComponents((string) null, 101, 0, out this.componentEnum));
              this.selector = new VSCOMPONENTSELECTORDATA[1];
              this.selector[0] = new VSCOMPONENTSELECTORDATA();
              this.selector[0].dwSize = (uint) Marshal.SizeOf(typeof (VSCOMPONENTSELECTORDATA));
            }
            uint pceltFetched;
            NativeMethods.ThrowOnFailure(this.componentEnum.Next(1U, this.selector, out pceltFetched));
            if (pceltFetched == 0U)
              return false;
            assemblyFile = this.selector[0].bstrFile;
          }
          else
          {
            if (this.componentEnum == null)
            {
              NativeMethods.ThrowOnFailure(this.enumFactory.GetComponents((string) null, 102, 0, out this.componentEnum));
              this.selector = new VSCOMPONENTSELECTORDATA[1];
              this.selector[0] = new VSCOMPONENTSELECTORDATA();
              this.selector[0].dwSize = (uint) Marshal.SizeOf(typeof (VSCOMPONENTSELECTORDATA));
            }
            string path;
            do
            {
              uint pceltFetched;
              NativeMethods.ThrowOnFailure(this.componentEnum.Next(1U, this.selector, out pceltFetched));
              if (pceltFetched == 0U)
                return false;
              path = Path.Combine(this.selector[0].bstrFile, this.name);
            }
            while (!File.Exists(path));
            assemblyFile = path;
          }
          try
          {
            this.currentName = AssemblyName.GetAssemblyName(assemblyFile);
          }
          catch
          {
            assemblyFile = (string) null;
          }
        }
        if (this.cachedNames == null)
          this.cachedNames = new ArrayList();
        this.cachedNames.Add((object) this.currentName);
        ++this.current;
        return true;
      }

      void IEnumerator.Reset()
      {
        this.currentName = (AssemblyName) null;
        this.current = -1;
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        ((IEnumerator) this).Reset();
        return (IEnumerator) this;
      }
    }
  }
}
