using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.SpecExplorer.Viewer
{
	public interface IViewDefinitionManager
	{
		IEnumerable<IViewDefinition> Views { get; set; }

		IViewDefinition CurrentView { get; set; }

		IEnumerable<IViewDefinition> CustomizedViews { get; }

		IEnumerable<IViewDefinition> DefaultViews { get; }

		event EventHandler<ViewDefinitionUpdateEventArgs> ViewDefinitionUpdate;

		void Load(Stream inputStream);

		void SetDeferredLoading(Func<Stream> streamProvider);

		void Store(IEnumerable<IViewDefinition> viewDefinitions, Stream outputStream);

		void Store(Stream outputStream);

		void Add(IViewDefinition viewDefinition);

		void Add(IEnumerable<IViewDefinition> viewDefinitions);

		void Remove(IViewDefinition viewDefinition);

		void Reset();

		bool TryGetViewDefinition(string name, out IViewDefinition viewDefinition);

		void UpdateEventRaise(IEnumerable<IViewDefinition> updatedViewDefinitions);
	}
}
