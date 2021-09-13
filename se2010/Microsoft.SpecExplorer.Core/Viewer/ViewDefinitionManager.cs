// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.ViewDefinitionManager
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.Properties;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.Viewer
{
  public class ViewDefinitionManager : ComponentBase, IViewDefinitionManager
  {
    private Dictionary<string, IViewDefinition> viewDefinitionDict;
    private ViewDefinition defaultView;
    private IHost host;
    private Func<Stream> deferredLoadingStreamProvider;

    public event EventHandler<ViewDefinitionUpdateEventArgs> ViewDefinitionUpdate;

    public ViewDefinitionManager(IHost host)
    {
      this.defaultView = new ViewDefinition();
      this.defaultView.Name = "Default";
      this.defaultView.IsDefault = true;
      this.defaultView.ViewCollapseSteps = true;
      this.host = host;
      this.viewDefinitionDict = new Dictionary<string, IViewDefinition>();
      this.Add((IViewDefinition) this.defaultView);
      this.CurrentView = (IViewDefinition) this.defaultView;
    }

    public IEnumerable<IViewDefinition> Views
    {
      get
      {
        if (this.deferredLoadingStreamProvider != null)
          this.DoDeferredLoading();
        return (IEnumerable<IViewDefinition>) this.viewDefinitionDict.Values;
      }
      set
      {
        this.viewDefinitionDict.Clear();
        this.Add(value);
      }
    }

    public IViewDefinition CurrentView { get; set; }

    public IEnumerable<IViewDefinition> CustomizedViews
    {
      get
      {
        if (this.deferredLoadingStreamProvider != null)
          this.DoDeferredLoading();
        List<IViewDefinition> viewDefinitionList = new List<IViewDefinition>();
        foreach (IViewDefinition viewDefinition in this.viewDefinitionDict.Values)
        {
          if (!viewDefinition.IsDefault)
            viewDefinitionList.Add(viewDefinition);
        }
        return (IEnumerable<IViewDefinition>) viewDefinitionList;
      }
    }

    public IEnumerable<IViewDefinition> DefaultViews
    {
      get
      {
        if (this.deferredLoadingStreamProvider != null)
          this.DoDeferredLoading();
        List<IViewDefinition> viewDefinitionList = new List<IViewDefinition>();
        foreach (IViewDefinition viewDefinition in this.viewDefinitionDict.Values)
        {
          if (viewDefinition.IsDefault)
            viewDefinitionList.Add(viewDefinition);
        }
        return (IEnumerable<IViewDefinition>) viewDefinitionList;
      }
    }

    public void Add(IViewDefinition viewDefinition)
    {
      if (this.viewDefinitionDict.TryGetValue(viewDefinition.Name, out IViewDefinition _))
        throw new InvalidOperationException("view definition already in view definition manager.");
      this.viewDefinitionDict.Add(viewDefinition.Name, viewDefinition);
    }

    public void Add(IEnumerable<IViewDefinition> viewDefinitions)
    {
      foreach (IViewDefinition viewDefinition in viewDefinitions)
        this.Add(viewDefinition);
    }

    public void Remove(IViewDefinition viewDefinition) => this.viewDefinitionDict.Remove(viewDefinition.Name);

    public void Reset()
    {
      this.viewDefinitionDict.Clear();
      this.Add((IViewDefinition) this.defaultView);
    }

    public bool TryGetViewDefinition(string name, out IViewDefinition viewDefinition) => this.viewDefinitionDict.TryGetValue(name, out viewDefinition);

    public void SetDeferredLoading(Func<Stream> streamProvider) => this.deferredLoadingStreamProvider = streamProvider;

    private void DoDeferredLoading()
    {
      try
      {
        Stream s = this.deferredLoadingStreamProvider();
        if (s == null)
          return;
        using (s)
          this.host.RunProtected((ProtectedAction) (() => this.Load(s)));
      }
      finally
      {
        this.deferredLoadingStreamProvider = (Func<Stream>) null;
      }
    }

    public void UpdateEventRaise(
      IEnumerable<IViewDefinition> updatedViewDefinitions)
    {
      if (this.ViewDefinitionUpdate == null)
        return;
      this.ViewDefinitionUpdate((object) this, new ViewDefinitionUpdateEventArgs(updatedViewDefinitions));
    }

    public void Store(Stream outputStream) => this.Store((IEnumerable<IViewDefinition>) this.viewDefinitionDict.Values, outputStream);

    public void Store(IEnumerable<IViewDefinition> viewDefinitions, Stream outputStream)
    {
      Microsoft.SpecExplorer.Viewer.Views views = new Microsoft.SpecExplorer.Viewer.Views();
      HashSet<string> stringSet = new HashSet<string>();
      List<ViewDefinition> viewDefinitionList = new List<ViewDefinition>();
      foreach (ViewDefinition viewDefinition in viewDefinitions)
      {
        if (!stringSet.Add(viewDefinition.Name))
        {
          this.host.NotificationDialog(Resources.SpecExplorer, string.Format("Duplicate view name: {0}.", (object) viewDefinition.Name));
          return;
        }
        if (!viewDefinition.IsDefault)
          viewDefinitionList.Add(viewDefinition);
      }
      views.ViewList = viewDefinitionList.ToArray();
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (Microsoft.SpecExplorer.Viewer.Views));
      try
      {
        using (XmlTextWriter xmlTextWriter = new XmlTextWriter(outputStream, Encoding.UTF8))
        {
          xmlTextWriter.Formatting = Formatting.Indented;
          xmlSerializer.Serialize((XmlWriter) xmlTextWriter, (object) views);
          xmlTextWriter.Flush();
          xmlTextWriter.Close();
        }
      }
      catch (InvalidOperationException ex)
      {
        throw new ViewDefinitionManagerException(ex.Message, (Exception) ex);
      }
    }

    public void Load(Stream inputStream)
    {
      List<IViewDefinition> viewDefinitionList = new List<IViewDefinition>();
      viewDefinitionList.Add((IViewDefinition) this.defaultView);
      XmlReader xmlReader = (XmlReader) null;
      XmlReader schemaDocument = (XmlReader) null;
      try
      {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof (Microsoft.SpecExplorer.Viewer.Views));
        XmlReaderSettings settings = new XmlReaderSettings();
        schemaDocument = XmlReader.Create((TextReader) new StringReader(Resources.ViewDefinitionSchema));
        settings.Schemas.Add((string) null, schemaDocument);
        settings.ValidationType = ValidationType.Schema;
        settings.ValidationEventHandler += new ValidationEventHandler(this.ViewDefinitionValidationEventHandler);
        settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
        if (inputStream.Length > 0L)
        {
          xmlReader = XmlReader.Create(inputStream, settings);
          Microsoft.SpecExplorer.Viewer.Views views = (Microsoft.SpecExplorer.Viewer.Views) xmlSerializer.Deserialize(xmlReader);
          viewDefinitionList.AddRange((IEnumerable<IViewDefinition>) views.ViewList);
        }
      }
      catch (InvalidOperationException ex)
      {
        throw new ViewDefinitionManagerException(ex.Message);
      }
      finally
      {
        xmlReader?.Close();
        schemaDocument?.Close();
      }
      this.viewDefinitionDict.Clear();
      this.Add((IEnumerable<IViewDefinition>) viewDefinitionList);
    }

    private void ViewDefinitionValidationEventHandler(object sender, ValidationEventArgs e)
    {
      this.host.DiagMessage(DiagnosisKind.Warning, string.Format("Schema Validation Failed: {0} at line {1} column {2}.", (object) e.Message, (object) e.Exception.LineNumber, (object) e.Exception.LinePosition), (object) null);
      throw new InvalidOperationException(e.Message);
    }
  }
}
