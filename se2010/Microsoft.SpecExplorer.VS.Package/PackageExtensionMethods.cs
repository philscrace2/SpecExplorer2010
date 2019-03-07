// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.PackageExtensionMethods
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer
{
  public static class PackageExtensionMethods
  {
    public static IEnumerable<CodeElement> GetAllMembers(
      this CodeType codeType)
    {
      if (codeType != null)
      {
        CodeClass2 codeClass = codeType as CodeClass2;
        if (codeClass != null)
        {
          foreach (CodeElement allMember in codeClass.GetAllMembers())
            yield return allMember;
        }
        else if (codeType.Members != null)
        {
          IEnumerator enumerator = codeType.Members.GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
            {
              CodeElement member = (CodeElement) enumerator.Current;
              yield return member;
            }
          }
          finally
          {
            IDisposable disposable = enumerator as IDisposable;
            disposable?.Dispose();
          }
        }
      }
    }

    public static IEnumerable<CodeElement> GetAllMembers(
      this CodeClass2 codeClass)
    {
      if (codeClass != null)
      {
        if (codeClass.InfoLocation == vsCMInfoLocation.vsCMInfoLocationProject)
        {
          if (codeClass.Parts != null)
          {
            IEnumerator enumerator1 = codeClass.Parts.GetEnumerator();
            try
            {
              while (enumerator1.MoveNext())
              {
                object p = enumerator1.Current;
                CodeClass2 part = p as CodeClass2;
                if (part != null && part.Members != null)
                {
                  IEnumerator enumerator2 = part.Members.GetEnumerator();
                  try
                  {
                    while (enumerator2.MoveNext())
                    {
                      CodeElement member = (CodeElement) enumerator2.Current;
                      yield return member;
                    }
                  }
                  finally
                  {
                    IDisposable disposable = enumerator2 as IDisposable;
                    disposable?.Dispose();
                  }
                }
              }
            }
            finally
            {
              IDisposable disposable = enumerator1 as IDisposable;
              disposable?.Dispose();
            }
          }
        }
        else if (codeClass.Members != null)
        {
          IEnumerator enumerator = codeClass.Members.GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
            {
              CodeElement member = (CodeElement) enumerator.Current;
              yield return member;
            }
          }
          finally
          {
            IDisposable disposable = enumerator as IDisposable;
            disposable?.Dispose();
          }
        }
      }
    }

    public static OLEMSGBUTTON ToOleMessageButton(this MessageButton messageButton)
    {
      switch ((int) messageButton)
      {
        case 0:
          return OLEMSGBUTTON.OLEMSGBUTTON_OK;
        case 1:
          return OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL;
        case 2:
          return OLEMSGBUTTON.OLEMSGBUTTON_ABORTRETRYIGNORE;
        case 4:
          return OLEMSGBUTTON.OLEMSGBUTTON_YESNO;
        case 5:
          return OLEMSGBUTTON.OLEMSGBUTTON_RETRYCANCEL;
        case 6:
          return OLEMSGBUTTON.OLEMSGBUTTON_YESALLNOCANCEL;
        default:
          return OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL;
      }
    }

    public static bool IsValid(this CodeNamespace codeNamespace)
    {
      if (codeNamespace != null && !codeNamespace.Equals("System"))
        return !codeNamespace.Equals("MS");
      return false;
    }
  }
}
