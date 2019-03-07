// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.RuleDeclarationWizardData
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using Microsoft.ActionMachines.Cord;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer.VS
{
  public class RuleDeclarationWizardData
  {
    public ConfigInfo ConfigInfo { get; set; }

    public Dictionary<ProcedureType, HashSet<MethodDescriptor>> MethodDescriptors { get; set; }

    public HashSet<ProcedureType> AdapterTypes { get; set; }

    public Dictionary<ProcedureType, CodeClass> HostClassMap { get; private set; }

    public Dictionary<ProcedureType, CodeClass> TypeBindingMap { get; private set; }

    public RuleDeclarationWizardData()
    {
      this.HostClassMap = new Dictionary<ProcedureType, CodeClass>();
      this.TypeBindingMap = new Dictionary<ProcedureType, CodeClass>();
    }
  }
}
