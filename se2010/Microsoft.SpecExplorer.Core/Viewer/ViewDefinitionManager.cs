using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.SpecExplorer.Properties;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer.Viewer
{
	public class ViewDefinitionManager : ComponentBase, IViewDefinitionManager
	{
		private Dictionary<string, IViewDefinition> viewDefinitionDict;

		private ViewDefinition defaultView;

		private IHost host;

		private Func<Stream> deferredLoadingStreamProvider;

		public IEnumerable<IViewDefinition> Views
		{
			get
			{
				if (deferredLoadingStreamProvider != null)
				{
					DoDeferredLoading();
				}
				return viewDefinitionDict.Values;
			}
			set
			{
				viewDefinitionDict.Clear();
				Add(value);
			}
		}

		public IViewDefinition CurrentView { get; set; }

		public IEnumerable<IViewDefinition> CustomizedViews
		{
			get
			{
				if (deferredLoadingStreamProvider != null)
				{
					DoDeferredLoading();
				}
				List<IViewDefinition> list = new List<IViewDefinition>();
				foreach (IViewDefinition value in viewDefinitionDict.Values)
				{
					if (!value.IsDefault)
					{
						list.Add(value);
					}
				}
				return list;
			}
		}

		public IEnumerable<IViewDefinition> DefaultViews
		{
			get
			{
				if (deferredLoadingStreamProvider != null)
				{
					DoDeferredLoading();
				}
				List<IViewDefinition> list = new List<IViewDefinition>();
				foreach (IViewDefinition value in viewDefinitionDict.Values)
				{
					if (value.IsDefault)
					{
						list.Add(value);
					}
				}
				return list;
			}
		}

		public event EventHandler<ViewDefinitionUpdateEventArgs> ViewDefinitionUpdate;

		public ViewDefinitionManager(IHost host)
		{
			defaultView = new ViewDefinition();
			defaultView.Name = "Default";
			defaultView.IsDefault = true;
			defaultView.ViewCollapseSteps = true;
			this.host = host;
			viewDefinitionDict = new Dictionary<string, IViewDefinition>();
			Add(defaultView);
			CurrentView = defaultView;
		}

		public void Add(IViewDefinition viewDefinition)
		{
			IViewDefinition value;
			if (viewDefinitionDict.TryGetValue(viewDefinition.Name, out value))
			{
				throw new InvalidOperationException("view definition already in view definition manager.");
			}
			viewDefinitionDict.Add(viewDefinition.Name, viewDefinition);
		}

		public void Add(IEnumerable<IViewDefinition> viewDefinitions)
		{
			foreach (IViewDefinition viewDefinition in viewDefinitions)
			{
				Add(viewDefinition);
			}
		}

		public void Remove(IViewDefinition viewDefinition)
		{
			viewDefinitionDict.Remove(viewDefinition.Name);
		}

		public void Reset()
		{
			viewDefinitionDict.Clear();
			Add(defaultView);
		}

		public bool TryGetViewDefinition(string name, out IViewDefinition viewDefinition)
		{
			return viewDefinitionDict.TryGetValue(name, out viewDefinition);
		}

		public void SetDeferredLoading(Func<Stream> streamProvider)
		{
			deferredLoadingStreamProvider = streamProvider;
		}

		private void DoDeferredLoading()
		{
			try
			{
				Stream s = deferredLoadingStreamProvider();
				if (s == null)
				{
					return;
				}
				using (s)
				{
					host.RunProtected(delegate
					{
						Load(s);
					});
				}
			}
			finally
			{
				deferredLoadingStreamProvider = null;
			}
		}

		public void UpdateEventRaise(IEnumerable<IViewDefinition> updatedViewDefinitions)
		{
			if (this.ViewDefinitionUpdate != null)
			{
				this.ViewDefinitionUpdate(this, new ViewDefinitionUpdateEventArgs(updatedViewDefinitions));
			}
		}

		public void Store(Stream outputStream)
		{
			Store(viewDefinitionDict.Values, outputStream);
		}

		public void Store(IEnumerable<IViewDefinition> viewDefinitions, Stream outputStream)
		{
			Views views = new Views();
			HashSet<string> hashSet = new HashSet<string>();
			List<ViewDefinition> list = new List<ViewDefinition>();
			foreach (ViewDefinition viewDefinition in viewDefinitions)
			{
				if (!hashSet.Add(viewDefinition.Name))
				{
					host.NotificationDialog(Resources.SpecExplorer, string.Format("Duplicate view name: {0}.", viewDefinition.Name));
					return;
				}
				if (!viewDefinition.IsDefault)
				{
					list.Add(viewDefinition);
				}
			}
			views.ViewList = list.ToArray();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Views));
			try
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(outputStream, Encoding.UTF8))
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlSerializer.Serialize(xmlTextWriter, views);
					xmlTextWriter.Flush();
					xmlTextWriter.Close();
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new ViewDefinitionManagerException(ex.Message, ex);
			}
		}

		public void Load(Stream inputStream)
		{
			List<IViewDefinition> list = new List<IViewDefinition>();
			list.Add(defaultView);
			XmlReader xmlReader = null;
			XmlReader xmlReader2 = null;
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Views));
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReader2 = XmlReader.Create(new StringReader(Resources.ViewDefinitionSchema));
				xmlReaderSettings.Schemas.Add(null, xmlReader2);
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.ValidationEventHandler += ViewDefinitionValidationEventHandler;
				xmlReaderSettings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
				if (inputStream.Length > 0)
				{
					xmlReader = XmlReader.Create(inputStream, xmlReaderSettings);
					Views views = (Views)xmlSerializer.Deserialize(xmlReader);
					list.AddRange(views.ViewList);
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new ViewDefinitionManagerException(ex.Message);
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
				if (xmlReader2 != null)
				{
					xmlReader2.Close();
				}
			}
			viewDefinitionDict.Clear();
			Add(list);
		}

		private void ViewDefinitionValidationEventHandler(object sender, ValidationEventArgs e)
		{
			string message = string.Format("Schema Validation Failed: {0} at line {1} column {2}.", e.Message, e.Exception.LineNumber, e.Exception.LinePosition);
			host.DiagMessage(DiagnosisKind.Warning, message, null);
			throw new InvalidOperationException(e.Message);
		}
	}
}
