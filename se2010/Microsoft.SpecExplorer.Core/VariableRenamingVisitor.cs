// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VariableRenamingVisitor
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.ObjectModel;
using System.Collections.Generic;

namespace Microsoft.SpecExplorer
{
  public class VariableRenamingVisitor : SerializableExpressionVisitor
  {
    private Dictionary<string, string> variables = new Dictionary<string, string>();
    private int count;

    public virtual SerializableExpression VisitParameterExpression(
      SerializableParameterExpression parameter)
    {
      SerializableParameterExpression parameterExpression = new SerializableParameterExpression();
      ((SerializableExpression) parameterExpression).NodeType = ((SerializableExpression) parameter).NodeType;
      parameterExpression.ParameterType = parameter.ParameterType;
      ((SerializableExpression) parameterExpression).Key = ((SerializableExpression) parameter).Key;
      parameterExpression.Name = this.BuildVariableName(parameter.Name);
      return (SerializableExpression) parameterExpression;
    }

    private string BuildVariableName(string name)
    {
      string str;
      if (!this.variables.TryGetValue(name, out str))
      {
        str = "v" + this.count.ToString();
        this.variables[name] = str;
        ++this.count;
      }
      return str;
    }
  }
}
