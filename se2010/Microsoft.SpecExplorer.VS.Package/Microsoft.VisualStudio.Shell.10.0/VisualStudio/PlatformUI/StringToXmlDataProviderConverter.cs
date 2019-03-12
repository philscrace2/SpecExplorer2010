// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.StringToXmlDataProviderConverter
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Globalization;
using System.Windows.Data;
using System.Xml;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class StringToXmlDataProviderConverter : ValueConverter<string, XmlDataProvider>
  {
    protected override XmlDataProvider Convert(
      string xmlBlob,
      object parameter,
      CultureInfo culture)
    {
      XmlDataProvider xmlDataProvider = new XmlDataProvider();
      if (!string.IsNullOrEmpty(xmlBlob))
      {
        try
        {
          XmlDocument xmlDocument = new XmlDocument();
          xmlDocument.LoadXml(xmlBlob);
          xmlDataProvider.Document = xmlDocument;
        }
        catch (XmlException ex)
        {
        }
      }
      return xmlDataProvider;
    }

    protected override string ConvertBack(
      XmlDataProvider xmlDataProvider,
      object parameter,
      CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
