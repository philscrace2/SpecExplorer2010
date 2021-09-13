// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.IViewDefinitionManager
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

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

    void Load(Stream inputStream);

    void SetDeferredLoading(Func<Stream> streamProvider);

    void Store(IEnumerable<IViewDefinition> viewDefinitions, Stream outputStream);

    void Store(Stream outputStream);

    void Add(IViewDefinition viewDefinition);

    void Add(IEnumerable<IViewDefinition> viewDefinitions);

    void Remove(IViewDefinition viewDefinition);

    void Reset();

    bool TryGetViewDefinition(string name, out IViewDefinition viewDefinition);

    event EventHandler<ViewDefinitionUpdateEventArgs> ViewDefinitionUpdate;

    void UpdateEventRaise(
      IEnumerable<IViewDefinition> updatedViewDefinitions);
  }
}
