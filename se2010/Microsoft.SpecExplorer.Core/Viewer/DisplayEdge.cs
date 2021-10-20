using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ActionMachines;
using Microsoft.GraphTraversal;
using Microsoft.SpecExplorer.ObjectModel;

namespace Microsoft.SpecExplorer.Viewer
{
	internal class DisplayEdge : Edge<State, Transition>
	{
		private StringBuilder textBuilder = new StringBuilder();

		private List<string> capturedRequirements = new List<string>();

		private List<string> assumeCapturedRequirements = new List<string>();

		private DisplayEdge parentEdge { get; set; }

		internal List<DisplayEdge> subEdges { get; set; }

		internal DisplayEdgeKind displayEdgeKind { get; set; }

		internal string Id { get; set; }

		internal ActionSymbolKind Kind { get; set; }

		internal IList<string> CapturedRequirements
		{
			get
			{
				return capturedRequirements;
			}
		}

		internal IList<string> AssumeCapturedRequirements
		{
			get
			{
				return assumeCapturedRequirements;
			}
		}

		internal string Text
		{
			get
			{
				return textBuilder.ToString();
			}
			set
			{
				textBuilder = new StringBuilder();
				textBuilder.Append(value);
			}
		}

		internal string ActionText { get; private set; }

		internal DisplayEdge(DisplayNode source, DisplayNode target, DisplayEdgeKind kind)
			: base((Node<State>)source, (Node<State>)target, new Transition(), false, (IEnumerable<string>)null)
		{
			subEdges = new List<DisplayEdge>();
			displayEdgeKind = kind;
		}

		internal DisplayEdge(DisplayNode source, DisplayNode target, Transition trans, bool isDisplayRequirements)
			: base((Node<State>)source, (Node<State>)target, trans, trans.Action.IsObservable(), (IEnumerable<string>)null)
		{
			displayEdgeKind = DisplayEdgeKind.Normal;
			if (trans.Action == null || trans.Action.Symbol == null)
			{
				Kind = ActionSymbolKind.Invocation;
			}
			else
			{
				Kind = trans.Action.Symbol.Kind;
				Text = GetEdgeDisplayText(trans);
				ActionText = trans.Action.Symbol.ToDisplayText();
			}
			capturedRequirements.AddRange(trans.CapturedRequirements);
			assumeCapturedRequirements.AddRange(trans.AssumeCapturedRequirements);
			if (isDisplayRequirements)
			{
				AppendRequirements(trans.CapturedRequirements, trans.AssumeCapturedRequirements, textBuilder.Length);
			}
		}

		internal DisplayEdge(DisplayEdge inEdge, DisplayEdge outEdge, bool isDisplayRequirements)
			: base(inEdge.Source, outEdge.Target, new Transition(), false, (IEnumerable<string>)null)
		{
			displayEdgeKind = DisplayEdgeKind.Collapsed;
			Kind = ActionSymbolKind.Call;
			subEdges = new List<DisplayEdge>();
			subEdges.Add(inEdge);
			subEdges.Add(outEdge);
			BuildCollapseEdgeText(inEdge.Text, outEdge.Text);
			ActionText = inEdge.ActionText;
			capturedRequirements.AddRange(inEdge.Label.CapturedRequirements);
			assumeCapturedRequirements.AddRange(inEdge.Label.AssumeCapturedRequirements);
			capturedRequirements.AddRange(outEdge.Label.CapturedRequirements);
			assumeCapturedRequirements.AddRange(outEdge.Label.AssumeCapturedRequirements);
			if (isDisplayRequirements)
			{
				AppendRequirements(capturedRequirements, assumeCapturedRequirements, textBuilder.Length);
			}
		}

		internal DisplayEdge(DisplayNode source, DisplayNode target, DisplayEdge displayEdge)
			: base((Node<State>)source, (Node<State>)target, displayEdge.Label, displayEdge.IsObservable, displayEdge.Requirements)
		{
			subEdges = displayEdge.subEdges;
			parentEdge = displayEdge.parentEdge;
			textBuilder = displayEdge.textBuilder;
			ActionText = displayEdge.ActionText;
			displayEdgeKind = displayEdge.displayEdgeKind;
			Kind = displayEdge.Kind;
		}

		internal void AddSubEdge(DisplayEdge edge)
		{
			if (displayEdgeKind != DisplayEdgeKind.Hyper)
			{
				throw new InvalidOperationException("Can not add sub edge to non hyper edge");
			}
			subEdges.Add(edge);
			if (subEdges.Count == 1)
			{
				Kind = edge.Kind;
			}
			else
			{
				Kind |= edge.Kind;
			}
			edge.parentEdge = this;
			if (edge.Label.CapturedRequirements != null)
			{
				capturedRequirements.AddRange(edge.Label.CapturedRequirements);
			}
			if (edge.Label.AssumeCapturedRequirements != null)
			{
				assumeCapturedRequirements.AddRange(edge.Label.AssumeCapturedRequirements);
			}
			if (textBuilder.Length > 0)
			{
				textBuilder.Append("\r\n");
			}
			textBuilder.Append(edge.Text);
		}

		private void BuildCollapseEdgeText(string inText, string outText)
		{
			int num = inText.IndexOf('\r');
			if (num > 0)
			{
				inText = inText.Substring(0, num);
			}
			num = outText.IndexOf('\r');
			if (num > 0)
			{
				outText = outText.Substring(0, num);
			}
			textBuilder.Append(inText.Substring(5));
			int num2 = outText.IndexOf('/');
			if (num2 > 0)
			{
				textBuilder.Append(outText.Substring(num2));
			}
		}

		private void AppendRequirements(IEnumerable<string> capturedRequirements, IEnumerable<string> assumeCapturedRequirements, int maxWidth)
		{
			RequirementSequence requirementSequence = new RequirementSequence(capturedRequirements, assumeCapturedRequirements);
			string text = requirementSequence.ToString();
			if (!string.IsNullOrEmpty(text))
			{
				textBuilder.AppendLine();
				textBuilder.Append(AnnotationFormatter.Format(text, maxWidth));
			}
		}

		private string GetEdgeDisplayText(Transition trans)
		{
			if (trans.Action.Symbol.Kind == ActionSymbolKind.PreConstraintCheck)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("check:");
				if (trans.PreConstraints.Length > 0)
				{
					Constraint[] preConstraints = trans.PreConstraints;
					foreach (Constraint constraint in preConstraints)
					{
						stringBuilder.AppendLine(constraint.Text);
					}
				}
				return stringBuilder.ToString();
			}
			return trans.Action.Text;
		}
	}
}
