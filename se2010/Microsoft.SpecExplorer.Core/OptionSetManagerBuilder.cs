using System;
using System.ComponentModel;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	internal class OptionSetManagerBuilder
	{
		private TransitionSystem transitionSystem;

		internal OptionSetManagerBuilder(TransitionSystem transitionSystem)
		{
			if (transitionSystem == null)
			{
				throw new ArgumentNullException("transitionSystem");
			}
			this.transitionSystem = transitionSystem;
		}

		internal OptionSetManager CreateOptionSetManager()
		{
			OptionSetManager optionSetManager = new OptionSetManager();
			optionSetManager.Initialize();
			IOptionSet optionSet = optionSetManager.CreateDefaultOptionSet();
			ConfigSwitch[] configSwitches = transitionSystem.ConfigSwitches;
			foreach (ConfigSwitch configSwitch in configSwitches)
			{
				PropertyDescriptor property = optionSetManager.FindProperty(configSwitch.Name, Visibility.Configurable);
				optionSet.SetValue(property, configSwitch.Value);
			}
			optionSetManager.CurrentOptionSet = optionSet;
			return optionSetManager;
		}
	}
}
