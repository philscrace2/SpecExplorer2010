// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ModelingGuidance.GuidanceLoaderImpl
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.Properties;
using Microsoft.Xrt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
  internal class GuidanceLoaderImpl : ComponentBase, IGuidanceLoader
  {
    private string perLoadingErrorMsg;
    private HashSet<IGuidance> loadedGuidanceList = new HashSet<IGuidance>((IEqualityComparer<IGuidance>) new GuidanceEqualityComparer());

    public IGuidance LoadGuidance(Stream guidanceDataStream)
    {
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (GuidanceImpl));
      IGuidance guidance = (IGuidance) null;
      this.perLoadingErrorMsg = string.Empty;
      using (Stream input = (Stream) new MemoryStream(new ASCIIEncoding().GetBytes(Resources.GuidanceDefinitionSchema)))
      {
        using (XmlReader schemaDocument = XmlReader.Create(input))
        {
          XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
          xmlSchemaSet.Add((string) null, schemaDocument);
          XmlReaderSettings settings = new XmlReaderSettings()
          {
            ValidationType = ValidationType.Schema,
            Schemas = xmlSchemaSet
          };
          settings.ValidationEventHandler += new ValidationEventHandler(this.ValidationEventCallBack);
          using (XmlReader xmlReader = XmlReader.Create(guidanceDataStream, settings))
          {
            try
            {
              guidance = (IGuidance) xmlSerializer.Deserialize(xmlReader);
              if (this.loadedGuidanceList.Contains(guidance))
                this.AddErrorMessage("Already loaded Guidance with Id : " + guidance.Id);
            }
            catch (InvalidOperationException ex)
            {
              this.AddErrorMessage(ex.Message);
            }
          }
        }
      }
      if (!string.IsNullOrEmpty(this.perLoadingErrorMsg))
        throw new GuidanceException(this.perLoadingErrorMsg);
      this.loadedGuidanceList.Add(guidance);
      return guidance;
    }

    public void LoadGuidanceUsage(string combineUsageString)
    {
      IEnumerable<GuidanceUsageInfo> multipleGuidance = GuidanceUsageInfo.ParseForMultipleGuidance(combineUsageString);
      if (multipleGuidance == null)
        return;
      using (IEnumerator<GuidanceUsageInfo> enumerator = multipleGuidance.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          GuidanceUsageInfo usageInfo = enumerator.Current;
          IGuidance guidance1 = this.loadedGuidanceList.FirstOrDefault<IGuidance>((Func<IGuidance, bool>) (guidance => guidance.Id == usageInfo.GuidanceId));
          if (guidance1 != null)
          {
            foreach (IActivityReference activityReference in guidance1.Structure)
            {
              if (usageInfo.CompletedActivityIds.Contains<string>(activityReference.Activity.Id))
                activityReference.IsCompleted = true;
            }
          }
        }
      }
    }

    public IEnumerable<IGuidance> LoadedGuidanceList => (IEnumerable<IGuidance>) this.loadedGuidanceList;

    public void UnloadGuidanceList() => this.loadedGuidanceList.Clear();

    private void ValidationEventCallBack(object sender, ValidationEventArgs evtArgs)
    {
      if (evtArgs.Severity != XmlSeverityType.Error)
        return;
      this.AddErrorMessage(evtArgs.Message);
    }

    private void AddErrorMessage(string errorMessage)
    {
      GuidanceLoaderImpl guidanceLoaderImpl = this;
      guidanceLoaderImpl.perLoadingErrorMsg = guidanceLoaderImpl.perLoadingErrorMsg + errorMessage + ". ";
    }
  }
}
