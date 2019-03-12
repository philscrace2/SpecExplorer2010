// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.Performance.CodeMarkers
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Internal.Performance
{
  internal sealed class CodeMarkers
  {
    public static readonly CodeMarkers Instance = new CodeMarkers();
    private const string AtomName = "VSCodeMarkersEnabled";
    private const string DllName = "Microsoft.Internal.Performance.CodeMarkers.dll";
    private bool fUseCodeMarkers;

    private CodeMarkers()
    {
      this.fUseCodeMarkers = CodeMarkers.NativeMethods.FindAtom("VSCodeMarkersEnabled") != (ushort) 0;
    }

    public void CodeMarker(CodeMarkerEvent nTimerID)
    {
      if (!this.fUseCodeMarkers)
        return;
      try
      {
        CodeMarkers.NativeMethods.DllPerfCodeMarker((int) nTimerID, (byte[]) null, 0);
      }
      catch (DllNotFoundException ex)
      {
        this.fUseCodeMarkers = false;
      }
    }

    public void CodeMarkerEx(CodeMarkerEvent nTimerID, byte[] aBuff)
    {
      if (aBuff == null)
        throw new ArgumentNullException(nameof (aBuff));
      if (!this.fUseCodeMarkers)
        return;
      try
      {
        CodeMarkers.NativeMethods.DllPerfCodeMarker((int) nTimerID, aBuff, aBuff.Length);
      }
      catch (DllNotFoundException ex)
      {
        this.fUseCodeMarkers = false;
      }
    }

    private static class NativeMethods
    {
      [DllImport("Microsoft.Internal.Performance.CodeMarkers.dll", EntryPoint = "PerfCodeMarker")]
      public static extern void DllPerfCodeMarker(int nTimerID, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] aUserParams, int cbParams);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
      public static extern ushort FindAtom(string lpString);
    }
  }
}
