// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.UIDispatch
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  [CLSCompliant(false)]
  public abstract class UIDispatch : IVsUIDispatch, IUIDispatch
  {
    private IDictionary<string, UIDataSourceVerb> verbs = (IDictionary<string, UIDataSourceVerb>) new Dictionary<string, UIDataSourceVerb>();

    public int EnumVerbs(out IVsUIEnumDataSourceVerbs ppEnum)
    {
      ppEnum = (IVsUIEnumDataSourceVerbs) new UIDataSourceVerbEnumerator(this.verbs.Values);
      return 0;
    }

    public int Invoke(string verbName, object pvaIn, out object pvaOut)
    {
      if (verbName == null)
        throw new ArgumentNullException(nameof (verbName));
      if (verbName.Length == 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_EmptyString, (object) nameof (verbName)));
      UIDataSourceVerb uiDataSourceVerb;
      lock (this.verbs)
      {
        if (!this.verbs.TryGetValue(verbName, out uiDataSourceVerb))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_VerbNotDefined, (object) verbName));
      }
      uiDataSourceVerb.Handler((IVsUIDispatch) this, verbName, pvaIn, out pvaOut);
      return 0;
    }

    public object Invoke(string verbName, object parameter)
    {
      object pvaOut;
      this.Invoke(verbName, parameter, out pvaOut);
      return pvaOut;
    }

    public IEnumerable<IVerbDescription> Verbs
    {
      get
      {
        lock (this.verbs)
        {
          IList<IVerbDescription> verbDescriptionList = (IList<IVerbDescription>) new List<IVerbDescription>();
          foreach (UIDataSourceVerb uiDataSourceVerb in (IEnumerable<UIDataSourceVerb>) this.verbs.Values)
            verbDescriptionList.Add((IVerbDescription) uiDataSourceVerb);
          return (IEnumerable<IVerbDescription>) verbDescriptionList;
        }
      }
    }

    public void AddCommand(string name, CommandHandler handler)
    {
      lock (this.verbs)
      {
        UIDataSourceVerb uiDataSourceVerb;
        if (this.verbs.TryGetValue(name, out uiDataSourceVerb))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Error_DuplicateVerb, (object) name));
        uiDataSourceVerb = new UIDataSourceVerb(name, handler);
        this.verbs[name] = uiDataSourceVerb;
      }
    }
  }
}
