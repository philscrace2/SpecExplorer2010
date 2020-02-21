// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Resources
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.SpecExplorer
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) Microsoft.SpecExplorer.Resources.resourceMan, (object) null))
          Microsoft.SpecExplorer.Resources.resourceMan = new ResourceManager("Microsoft.SpecExplorer.Resources", typeof (Microsoft.SpecExplorer.Resources).Assembly);
        return Microsoft.SpecExplorer.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.resourceCulture;
      }
      set
      {
        Microsoft.SpecExplorer.Resources.resourceCulture = value;
      }
    }

    internal static string AbortingExploration
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(AbortingExploration, Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ApplicationName
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(ApplicationName, Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string BuildingProjectFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(BuildingProjectFormat, Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string BuildProjectFailedFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(BuildProjectFailedFormat, Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string BuildProjectSucceededFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(BuildProjectSucceededFormat, Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string CanNotCreateWindow
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(CanNotCreateWindow, Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string CodeGenerationTimeout
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(CodeGenerationTimeout, Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ExecutionAborted
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ExecutionAborted), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ExecutionFinished
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ExecutionFinished), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ExplorationAborted
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ExplorationAborted), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ExplorationFinished
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ExplorationFinished), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ExplorationInProgress
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ExplorationInProgress), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ExplorationMachineFailedFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ExplorationMachineFailedFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ExplorationMachineSucceededFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ExplorationMachineSucceededFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ExplorationManagerToolWindowTitle
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ExplorationManagerToolWindowTitle), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ExploringMachineFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ExploringMachineFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ExtensionDirectoryName
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ExtensionDirectoryName), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string FailedToBuildProject
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (FailedToBuildProject), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string InvalidMachineExplorationResultFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (InvalidMachineExplorationResultFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string MachineResultUpToDateFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (MachineResultUpToDateFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string OnTheFlyReplayAborted
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (OnTheFlyReplayAborted), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string OnTheFlyReplayFinished
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (OnTheFlyReplayFinished), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string OnTheFlyReplayInProgress
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (OnTheFlyReplayInProgress), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string OnTheFlyTestAborted
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (OnTheFlyTestAborted), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string OnTheFlyTestFinished
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (OnTheFlyTestFinished), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string OnTheFlyTestInProgress
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (OnTheFlyTestInProgress), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string PostProcessorAborted
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (PostProcessorAborted), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string SaveOfReadOnlyFileFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (SaveOfReadOnlyFileFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string SEMainAssemblyName
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (SEMainAssemblyName), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string SkippingFailedProjectMachineFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (SkippingFailedProjectMachineFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string SkippingNoProjectMachineFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (SkippingNoProjectMachineFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string SpecExplorer
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (SpecExplorer), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string StateComparisonWindowTitle
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (StateComparisonWindowTitle), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string StatesBrowserToolWindowTitle
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (StatesBrowserToolWindowTitle), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string StepBrowserToolWindow
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (StepBrowserToolWindow), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string SummaryTemplate
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (SummaryTemplate), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string TestCodeGenerationAborted
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (TestCodeGenerationAborted), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string TestCodeGenerationFailedFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (TestCodeGenerationFailedFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string TestCodeGenerationInProgressFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (TestCodeGenerationInProgressFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string TestCodeGenerationSucceededFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (TestCodeGenerationSucceededFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ToolWindowTitle
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ToolWindowTitle), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ValidatingMachineFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ValidatingMachineFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ValidationFailed
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ValidationFailed), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ValidationInProgress
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ValidationInProgress), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ValidationMachineFailedFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ValidationMachineFailedFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ValidationMachineSucceededFormat
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ValidationMachineSucceededFormat), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string ValidationSucceeded
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (ValidationSucceeded), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string WorkflowFileExtension
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (WorkflowFileExtension), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string WorkflowLoadingFailure
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (WorkflowLoadingFailure), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string WorkflowsDirectory
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (WorkflowsDirectory), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }

    internal static string WorkflowToolWindowTitle
    {
      get
      {
        return Microsoft.SpecExplorer.Resources.ResourceManager.GetString(nameof (WorkflowToolWindowTitle), Microsoft.SpecExplorer.Resources.resourceCulture);
      }
    }
  }
}
