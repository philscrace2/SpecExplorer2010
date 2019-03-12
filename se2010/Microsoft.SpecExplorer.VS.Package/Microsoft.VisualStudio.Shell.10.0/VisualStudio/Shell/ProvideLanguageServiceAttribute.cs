// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideLanguageServiceAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideLanguageServiceAttribute : RegistrationAttribute
  {
    private Guid languageServiceGuid;
    private string strLanguageName;
    private int languageResourceID;
    private Hashtable optionsTable;
    private ProvideLanguageServiceAttribute.DebuggerLanguages debuggerLanguages;

    public ProvideLanguageServiceAttribute(
      object languageService,
      string strLanguageName,
      int languageResourceID)
    {
      if ((object) (languageService as Type) != null)
      {
        this.languageServiceGuid = ((Type) languageService).GUID;
      }
      else
      {
        if (!(languageService is string))
          throw new ArgumentException();
        this.languageServiceGuid = new Guid((string) languageService);
      }
      this.strLanguageName = strLanguageName;
      this.languageResourceID = languageResourceID;
      this.debuggerLanguages = new ProvideLanguageServiceAttribute.DebuggerLanguages(strLanguageName);
      this.optionsTable = new Hashtable();
    }

    public Guid LanguageServiceSid
    {
      get
      {
        return this.languageServiceGuid;
      }
    }

    public string LanguageName
    {
      get
      {
        return this.strLanguageName;
      }
    }

    public int LanguageResourceID
    {
      get
      {
        return this.languageResourceID;
      }
    }

    public string DebuggerLanguageExpressionEvaluator
    {
      get
      {
        return this.debuggerLanguages.ExpressionEvaluator.ToString("B");
      }
      set
      {
        this.debuggerLanguages.ExpressionEvaluator = new Guid(value);
      }
    }

    public bool ShowCompletion
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.showCompletion];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.showCompletion] = (object) value;
      }
    }

    public bool ShowSmartIndent
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.showIndentOptions];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.showIndentOptions] = (object) value;
      }
    }

    public bool RequestStockColors
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.useStockColors];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.useStockColors] = (object) value;
      }
    }

    public bool ShowHotURLs
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.showHotURLs];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.showHotURLs] = (object) value;
      }
    }

    public bool DefaultToNonHotURLs
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.nonHotURLs];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.nonHotURLs] = (object) value;
      }
    }

    public bool DefaultToInsertSpaces
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.insertSpaces];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.insertSpaces] = (object) value;
      }
    }

    public bool ShowDropDownOptions
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.showDropDownBar];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.showDropDownBar] = (object) value;
      }
    }

    public bool SingleCodeWindowOnly
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.disableWindowNewWindow];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.disableWindowNewWindow] = (object) value;
      }
    }

    public bool EnableAdvancedMembersOption
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.enableAdvMembersOption];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.enableAdvMembersOption] = (object) value;
      }
    }

    public bool SupportCopyPasteOfHTML
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.supportCF_HTML];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.supportCF_HTML] = (object) value;
      }
    }

    public bool EnableLineNumbers
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.enableLineNumbersOption];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.enableLineNumbersOption] = (object) value;
      }
    }

    public bool HideAdvancedMembersByDefault
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.hideAdvancedMembersByDefault];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.hideAdvancedMembersByDefault] = (object) value;
      }
    }

    public bool CodeSense
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.codeSense];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.codeSense] = (object) value;
      }
    }

    public bool MatchBraces
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.matchBraces];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.matchBraces] = (object) value;
      }
    }

    public bool QuickInfo
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.quickInfo];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.quickInfo] = (object) value;
      }
    }

    public bool ShowMatchingBrace
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.showMatchingBrace];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.showMatchingBrace] = (object) value;
      }
    }

    public bool MatchBracesAtCaret
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.matchBracesAtCaret];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.matchBracesAtCaret] = (object) value;
      }
    }

    public int MaxErrorMessages
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.maxErrorMessages];
        if (obj != null)
          return (int) obj;
        return 0;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.maxErrorMessages] = (object) value;
      }
    }

    public int CodeSenseDelay
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.codeSenseDelay];
        if (obj != null)
          return (int) obj;
        return 0;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.codeSenseDelay] = (object) value;
      }
    }

    public bool EnableAsyncCompletion
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.enableAsyncCompletion];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.enableAsyncCompletion] = (object) value;
      }
    }

    public bool EnableCommenting
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.enableCommenting];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.enableCommenting] = (object) value;
      }
    }

    public bool EnableFormatSelection
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.enableFormatSelection];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.enableFormatSelection] = (object) value;
      }
    }

    public bool AutoOutlining
    {
      get
      {
        object obj = this.optionsTable[(object) LanguageOptionKeys.autoOutlining];
        if (obj != null)
          return (bool) obj;
        return false;
      }
      set
      {
        this.optionsTable[(object) LanguageOptionKeys.autoOutlining] = (object) value;
      }
    }

    private string LanguageServicesKeyName
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) RegistryPaths.languageServices, (object) this.LanguageName);
      }
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyLanguageService, (object) this.LanguageName, (object) this.LanguageServiceSid.ToString("B")));
      using (RegistrationAttribute.Key key1 = context.CreateKey(this.LanguageServicesKeyName))
      {
        key1.SetValue(string.Empty, (object) this.LanguageServiceSid.ToString("B"));
        key1.SetValue(RegistryPaths.package, (object) context.ComponentType.GUID.ToString("B"));
        key1.SetValue(RegistryPaths.languageResourceId, (object) this.languageResourceID);
        foreach (object key2 in (IEnumerable) this.optionsTable.Keys)
        {
          string valueName = key2.ToString();
          if (this.optionsTable[key2] is bool)
          {
            int num = 0;
            if ((bool) this.optionsTable[key2])
              num = 1;
            key1.SetValue(valueName, (object) num);
          }
          else if (this.optionsTable[key2] is int)
          {
            key1.SetValue(valueName, (object) (int) this.optionsTable[key2]);
          }
          else
          {
            string str = this.optionsTable[key2].ToString();
            key1.SetValue(valueName, (object) str);
          }
        }
        if (!this.debuggerLanguages.IsValid())
          return;
        string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\{1}", (object) RegistryPaths.debuggerLanguages, (object) this.debuggerLanguages.ExpressionEvaluator.ToString("B"));
        using (RegistrationAttribute.Key subkey = key1.CreateSubkey(name))
          subkey.SetValue((string) null, (object) this.debuggerLanguages.LanguageName);
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.LanguageServicesKeyName);
    }

    private class DebuggerLanguages
    {
      private Guid guidEE;
      private string languageName;

      public DebuggerLanguages(string languageName)
      {
        this.languageName = languageName;
        this.guidEE = Guid.Empty;
      }

      public Guid ExpressionEvaluator
      {
        get
        {
          return this.guidEE;
        }
        set
        {
          this.guidEE = value;
        }
      }

      public string LanguageName
      {
        get
        {
          return this.languageName;
        }
      }

      public bool IsValid()
      {
        return this.guidEE != Guid.Empty;
      }
    }
  }
}
