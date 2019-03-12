// Decompiled with JetBrains decompiler
// Type: Microsoft.Internal.VisualStudio.PlatformUI.ShapeIdentifierMap
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.Internal.Performance;
using System.ComponentModel;

namespace Microsoft.Internal.VisualStudio.PlatformUI
{
  internal class ShapeIdentifierMap : MruCache<ShapeIdentifier, PropertyDescriptorCollection>
  {
    public ShapeIdentifierMap(int capacity)
      : base(capacity)
    {
    }

    public ShapeIdentifierMap()
      : this(256)
    {
    }

    public PropertyDescriptorCollection GetPropertyDescriptors(
      IUIDataSource datasource)
    {
      ShapeIdentifier shapeIdentifier = datasource.ShapeIdentifier;
      PropertyDescriptorCollection descriptorCollection;
      if (this.TryGetValue(shapeIdentifier, out descriptorCollection))
        return descriptorCollection;
      PropertyDescriptorCollection descriptors = ShapeIdentifierMap.GetDescriptors(datasource);
      this.Add(shapeIdentifier, descriptors);
      return descriptors;
    }

    private static PropertyDescriptorCollection GetDescriptors(
      IUIDataSource datasource)
    {
      PropertyDescriptorCollection descriptorCollection = new PropertyDescriptorCollection((PropertyDescriptor[]) null);
      foreach (IPropertyDescription property in datasource.Properties)
        descriptorCollection.Add((PropertyDescriptor) new DataSource.DataSourcePropertyDescriptor(property));
      return descriptorCollection;
    }

    protected override void OnCacheMiss(ShapeIdentifier identifier)
    {
      CodeMarkers.Instance.CodeMarker(CodeMarkerEvent.perfGel_ShapeIdentifierCacheMiss);
      base.OnCacheMiss(identifier);
    }

    protected override void OnDiscard(ShapeIdentifier identifier)
    {
      CodeMarkers.Instance.CodeMarker(CodeMarkerEvent.perfGel_ShapeIdentifierCacheDiscard);
      base.OnDiscard(identifier);
    }
  }
}
