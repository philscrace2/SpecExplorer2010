// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ModelingGuidance.GuidanceUserControl
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.SpecExplorer.ModelingGuidance
{  
  public partial class ModelingGuidanceUserControl : UserControl
  {
    public ModelingGuidanceUserControl()
    {
      this.InitializeComponent();
      this.Loaded += (RoutedEventHandler) delegate
      {
        PresentationSource presentationSource = PresentationSource.FromVisual((Visual) this);
        if (presentationSource == null || !(presentationSource.CompositionTarget is HwndTarget compositionTarget2))
          return;
        compositionTarget2.RenderMode = RenderMode.SoftwareOnly;
      };
    }

        public GuidanceControlModel ControlModel
        {
            get
            {
                return viewerModel;
            }
            set { }
        }
        private void FlowDocumentLoaded(object sender, RoutedEventArgs evtArgs)
    {
      FlowDocument flowDocument = sender as FlowDocument;
      if (flowDocument.Tag == null)
        return;
      using (Stream stream = (Stream) new MemoryStream(new ASCIIEncoding().GetBytes(flowDocument.Tag.ToString())))
        new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd).Load(stream, DataFormats.Xaml);
    }
  }
}
