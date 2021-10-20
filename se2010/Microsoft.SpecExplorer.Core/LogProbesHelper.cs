using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	public class LogProbesHelper
	{
		private List<string> probes = new List<string>();

		private bool hasValidProbes;

		private bool hasCheckedProbes;

		internal void CheckLogProbesSwitchValue(TransitionSystem transitionSystem, IHost host, string machineName)
		{
			probes.Clear();
			hasCheckedProbes = true;
			string @switch = transitionSystem.GetSwitch("LogProbes");
			if (@switch == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(@switch))
			{
				host.DiagMessage(DiagnosisKind.Error, string.Format("[{0}]:Please use 'none' or just remove this switch.", machineName), null);
			}
			else if (string.Compare("none", @switch, true) == 0)
			{
				return;
			}
			List<string> list = new List<string>();
			if (transitionSystem.States.Length > 0)
			{
				Probe[] array = transitionSystem.States[0].Probes;
				foreach (Probe probe2 in array)
				{
					list.Add(probe2.Name);
				}
			}
			string[] array2 = @switch.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			string[] array3 = array2;
			string probe;
			for (int j = 0; j < array3.Length; j++)
			{
				probe = array3[j];
				List<string> list2 = list.FindAll((string s) => ("." + s).EndsWith("." + probe.Trim()));
				int count = list2.Count;
				if (count == 0)
				{
					host.DiagMessage(DiagnosisKind.Error, string.Format("[{0}]:Switch \"LogProbes\" is invalid, cannot find the probe: '{1}'", machineName, probe.Trim()), null);
				}
				else if (count > 1)
				{
					string arg = string.Join(",", list2);
					host.DiagMessage(DiagnosisKind.Error, string.Format("[{0}]:The probe '{1}' is ambiguous for {2}...  Please use full probe names for switch \"LogProbes\".", machineName, arg, probe.Trim()), null);
				}
				else
				{
					probes.Add(probe.Trim());
				}
				list2 = null;
			}
			hasValidProbes = true;
		}

		internal List<string> GetCommentsForLogProbes(State currentState, IHost host)
		{
			if (!hasCheckedProbes)
			{
				host.FatalError("Please call CheckLogProbesSwitchValue method before call this method.");
			}
			if (!hasValidProbes)
			{
				return new List<string>();
			}
			List<string> list = new List<string>();
			string probe;
			foreach (string probe3 in probes)
			{
				probe = probe3;
				Probe probe2 = currentState.Probes.FirstOrDefault((Probe x) => ("." + x.Name).EndsWith("." + probe.Trim()));
				if (probe2 != null)
				{
					list.Add(string.Format("- probe: {0}->{1}", probe.Trim(), probe2.Value));
				}
			}
			return list;
		}
	}
}
