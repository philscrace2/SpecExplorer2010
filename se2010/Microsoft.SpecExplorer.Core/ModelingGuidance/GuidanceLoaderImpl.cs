using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.SpecExplorer.Properties;
using Microsoft.Xrt;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
	internal class GuidanceLoaderImpl : ComponentBase, IGuidanceLoader
	{
		private string perLoadingErrorMsg;

		private HashSet<IGuidance> loadedGuidanceList = new HashSet<IGuidance>(new GuidanceEqualityComparer());

		public IEnumerable<IGuidance> LoadedGuidanceList
		{
			get
			{
				return loadedGuidanceList;
			}
		}

		public IGuidance LoadGuidance(Stream guidanceDataStream)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(GuidanceImpl));
			IGuidance guidance = null;
			perLoadingErrorMsg = string.Empty;
			using (Stream input = new MemoryStream(new ASCIIEncoding().GetBytes(Resources.GuidanceDefinitionSchema)))
			{
				using (XmlReader schemaDocument = XmlReader.Create(input))
				{
					XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
					xmlSchemaSet.Add(null, schemaDocument);
					XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
					xmlReaderSettings.ValidationType = ValidationType.Schema;
					xmlReaderSettings.Schemas = xmlSchemaSet;
					XmlReaderSettings xmlReaderSettings2 = xmlReaderSettings;
					xmlReaderSettings2.ValidationEventHandler += ValidationEventCallBack;
					using (XmlReader xmlReader = XmlReader.Create(guidanceDataStream, xmlReaderSettings2))
					{
						try
						{
							guidance = (GuidanceImpl)xmlSerializer.Deserialize(xmlReader);
							if (loadedGuidanceList.Contains(guidance))
							{
								AddErrorMessage("Already loaded Guidance with Id : " + guidance.Id);
							}
						}
						catch (InvalidOperationException ex)
						{
							AddErrorMessage(ex.Message);
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(perLoadingErrorMsg))
			{
				throw new GuidanceException(perLoadingErrorMsg);
			}
			loadedGuidanceList.Add(guidance);
			return guidance;
		}

		public void LoadGuidanceUsage(string combineUsageString)
		{
			IEnumerable<GuidanceUsageInfo> enumerable = GuidanceUsageInfo.ParseForMultipleGuidance(combineUsageString);
			if (enumerable == null)
			{
				return;
			}
			GuidanceUsageInfo usageInfo;
			foreach (GuidanceUsageInfo item in enumerable)
			{
				usageInfo = item;
				IGuidance guidance2 = loadedGuidanceList.FirstOrDefault((IGuidance guidance) => guidance.Id == usageInfo.GuidanceId);
				if (guidance2 == null)
				{
					continue;
				}
				IActivityReference[] structure = guidance2.Structure;
				foreach (IActivityReference activityReference in structure)
				{
					if (usageInfo.CompletedActivityIds.Contains(activityReference.Activity.Id))
					{
						activityReference.IsCompleted = true;
					}
				}
			}
		}

		public void UnloadGuidanceList()
		{
			loadedGuidanceList.Clear();
		}

		private void ValidationEventCallBack(object sender, ValidationEventArgs evtArgs)
		{
			if (evtArgs.Severity == XmlSeverityType.Error)
			{
				AddErrorMessage(evtArgs.Message);
			}
		}

		private void AddErrorMessage(string errorMessage)
		{
			perLoadingErrorMsg = perLoadingErrorMsg + errorMessage + ". ";
		}
	}
}
