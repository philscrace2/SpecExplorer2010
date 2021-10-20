using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.SpecExplorer.ObjectModel;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer
{
	internal class MarshalPostProcesser : DisposableMarshalByRefObject
	{
		private IPostProcessor processor;

		private string[] fileNames;

		internal string Description
		{
			get
			{
				return processor.Description;
			}
		}

		internal Exception Exception { get; private set; }

		internal void Initial(string[] fileNames, string assemblyPath, string typeName)
		{
			Assembly assembly = Assembly.LoadFrom(assemblyPath);
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (type.FullName.Equals(typeName))
				{
					processor = type.GetConstructor(new Type[0]).Invoke(new object[0]) as IPostProcessor;
				}
			}
			this.fileNames = fileNames;
		}

		internal void Invoke(IDictionary<string, object> environment)
		{
			try
			{
				List<TransitionSystem> list = new List<TransitionSystem>();
				string[] array = fileNames;
				foreach (string path in array)
				{
					ExplorationResultLoader explorationResultLoader = new ExplorationResultLoader(path);
					list.Add(explorationResultLoader.LoadTransitionSystem());
				}
				processor.PostProcess(list, environment);
			}
			catch (Exception ex)
			{
				Exception ex3 = (Exception = ex);
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			processor.Dispose();
		}
	}
}
