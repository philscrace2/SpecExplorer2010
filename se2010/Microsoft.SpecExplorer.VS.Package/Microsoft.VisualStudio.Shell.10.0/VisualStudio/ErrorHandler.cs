// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ErrorHandler
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio
{
  public sealed class ErrorHandler
  {
    private static HashSet<Type> criticalExceptions = new HashSet<Type>();

    private ErrorHandler()
    {
    }

    public static bool Succeeded(int hr)
    {
      return hr >= 0;
    }

    public static bool Failed(int hr)
    {
      return hr < 0;
    }

    public static int ThrowOnFailure(int hr)
    {
      return ErrorHandler.ThrowOnFailure(hr, (int[]) null);
    }

    public static int ThrowOnFailure(int hr, params int[] expectedHRFailure)
    {
      if (ErrorHandler.Failed(hr) && (expectedHRFailure == null || Array.IndexOf<int>(expectedHRFailure, hr) < 0))
        Marshal.ThrowExceptionForHR(hr);
      return hr;
    }

    static ErrorHandler()
    {
      ErrorHandler.criticalExceptions.Add(typeof (OutOfMemoryException));
      ErrorHandler.criticalExceptions.Add(typeof (StackOverflowException));
      ErrorHandler.criticalExceptions.Add(typeof (AccessViolationException));
      ErrorHandler.criticalExceptions.Add(typeof (AppDomainUnloadedException));
      ErrorHandler.criticalExceptions.Add(typeof (BadImageFormatException));
      ErrorHandler.criticalExceptions.Add(typeof (DivideByZeroException));
    }

    public static bool IsCriticalException(Exception ex)
    {
      return ErrorHandler.criticalExceptions.Contains(ex.GetType());
    }

    public static int CallWithCOMConvention(Func<int> method)
    {
      try
      {
        return method();
      }
      catch (Exception ex)
      {
        if (!ErrorHandler.IsCriticalException(ex))
          return Marshal.GetHRForException(ex);
        throw;
      }
    }

    public static int CallWithCOMConvention(Action method)
    {
      return ErrorHandler.CallWithCOMConvention((Func<int>) (() =>
      {
        method();
        return 0;
      }));
    }
  }
}
