using System.Collections.Generic;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer
{
	public class VariableRenamingVisitor : SerializableExpressionVisitor
	{
		private Dictionary<string, string> variables = new Dictionary<string, string>();

		private int count;

		public override SerializableExpression VisitParameterExpression(SerializableParameterExpression parameter)
		{
			SerializableParameterExpression serializableParameterExpression = new SerializableParameterExpression();
			serializableParameterExpression.NodeType = parameter.NodeType;
			serializableParameterExpression.ParameterType = parameter.ParameterType;
			serializableParameterExpression.Key = parameter.Key;
			serializableParameterExpression.Name = BuildVariableName(parameter.Name);
			return serializableParameterExpression;
		}

		private string BuildVariableName(string name)
		{
			string value;
			if (!variables.TryGetValue(name, out value))
			{
				value = "v" + count;
				variables[name] = value;
				count++;
			}
			return value;
		}
	}
}
