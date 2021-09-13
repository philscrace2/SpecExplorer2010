// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.LogProbesHelper
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer
{
  public class LogProbesHelper
  {
    private List<string> probes = new List<string>();
    private bool hasValidProbes;
    private bool hasCheckedProbes;

    internal void CheckLogProbesSwitchValue(
      TransitionSystem transitionSystem,
      IHost host,
      string machineName)
    {
      this.probes.Clear();
      this.hasCheckedProbes = true;
      string strB = transitionSystem.GetSwitch("LogProbes");
      if (strB == null)
        return;
      if (string.IsNullOrEmpty(strB))
        host.DiagMessage(DiagnosisKind.Error, string.Format("[{0}]:Please use 'none' or just remove this switch.", (object) machineName), (object) null);
      else if (string.Compare("none", strB, true) == 0)
        return;
      List<string> stringList = new List<string>();
      if (transitionSystem.States.Length > 0)
      {
        foreach (Probe probe in transitionSystem.States[0].Probes)
          stringList.Add(probe.Name);
      }
      string[] strArray = strB.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string str1 in strArray)
      {
        string probe = str1;
        List<string> all = stringList.FindAll((Predicate<string>) (s => ("." + s).EndsWith("." + probe.Trim())));
        int count = all.Count;
        if (count == 0)
          host.DiagMessage(DiagnosisKind.Error, string.Format("[{0}]:Switch \"LogProbes\" is invalid, cannot find the probe: '{1}'", (object) machineName, (object) probe.Trim()), (object) null);
        else if (count > 1)
        {
          string str2 = string.Join(",", (IEnumerable<string>) all);
          host.DiagMessage(DiagnosisKind.Error, string.Format("[{0}]:The probe '{1}' is ambiguous for {2}...  Please use full probe names for switch \"LogProbes\".", (object) machineName, (object) str2, (object) probe.Trim()), (object) null);
        }
        else
          this.probes.Add(probe.Trim());
      }
      this.hasValidProbes = true;
    }

    internal List<string> GetCommentsForLogProbes(State currentState, IHost host)
    {
      if (!this.hasCheckedProbes)
        host.FatalError("Please call CheckLogProbesSwitchValue method before call this method.");
      if (!this.hasValidProbes)
        return new List<string>();
      List<string> stringList = new List<string>();
      using (List<string>.Enumerator enumerator = this.probes.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          string probe = enumerator.Current;
          Probe probe1 = ((IEnumerable<Probe>) currentState.Probes).FirstOrDefault<Probe>((Func<Probe, bool>) (x => ("." + x.Name).EndsWith("." + probe.Trim())));
          if (probe1 != null)
            stringList.Add(string.Format("- probe: {0}->{1}", (object) probe.Trim(), (object) probe1.Value));
        }
      }
      return stringList;
    }
  }
}
