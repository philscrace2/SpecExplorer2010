// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideOptionDialogPageAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public abstract class ProvideOptionDialogPageAttribute : RegistrationAttribute
  {
    private Type _pageType;
    private string _pageNameResourceId;

    public ProvideOptionDialogPageAttribute(Type pageType, string pageNameResourceId)
    {
      if (pageType == (Type) null)
        throw new ArgumentNullException(nameof (pageType));
      if (!typeof (DialogPage).IsAssignableFrom(pageType))
        throw new ArgumentException(string.Format((IFormatProvider) Resources.Culture, Resources.Package_PageNotDialogPage, (object) pageType.FullName));
      this._pageType = pageType;
      this._pageNameResourceId = pageNameResourceId;
    }

    public Type PageType
    {
      get
      {
        return this._pageType;
      }
    }

    public string PageNameResourceId
    {
      get
      {
        return this._pageNameResourceId;
      }
    }
  }
}
