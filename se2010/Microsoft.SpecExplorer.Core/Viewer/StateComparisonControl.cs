// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.Viewer.StateComparisonControl
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using Microsoft.SpecExplorer.DiffAlgorithm;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.SpecExplorer.Viewer
{    
    public partial class StateComparisonControl : UserControl
    {
        private const string TitlePrefix = "Show ";
        private const string ShowBothTitle = "Show Both";
        private const string DefaultLeftTitle = "Left";
        private const string DefaultRightTitle = "Right";
        public static readonly DependencyProperty LeftTitleProperty = DependencyProperty.Register(nameof(LeftTitle), typeof(string), typeof(StateComparisonControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)"Left"));
        public static readonly DependencyProperty RightTitleProperty = DependencyProperty.Register(nameof(RightTitle), typeof(string), typeof(StateComparisonControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)"Right"));
        public static readonly DependencyProperty IntraLineProperty = DependencyProperty.Register(nameof(IntraLine), typeof(bool), typeof(StateComparisonControl), (PropertyMetadata)new FrameworkPropertyMetadata((object)true, new PropertyChangedCallback(StateComparisonControl.CalculateAndRefreshDiffResults)));
        private ObservableCollection<string> showTextCollection = new ObservableCollection<string>();
        private string leftText = string.Empty;
        private string rightText = string.Empty;
        private bool needsReDiff = true;
        private List<DiffBlockPair> diffResult;
        private bool ignoreSelectionChanged;

        public ObservableCollection<string> ShowTextCollection => this.showTextCollection;

        private static void CalculateAndRefreshDiffResults(
          DependencyObject sender,
          DependencyPropertyChangedEventArgs a)
        {
            if (!(sender is StateComparisonControl comparisonControl))
                return;
            comparisonControl.CalculateDiffResults();
            comparisonControl.RenderDiffResults();
        }

        public bool IntraLine
        {
            get => (bool)this.GetValue(StateComparisonControl.IntraLineProperty);
            set => this.SetValue(StateComparisonControl.IntraLineProperty, (object)value);
        }

        public string LeftTitle
        {
            get => this.GetValue(StateComparisonControl.LeftTitleProperty) as string;
            set => this.SetValue(StateComparisonControl.LeftTitleProperty, (object)value);
        }

        public string RightTitle
        {
            get => this.GetValue(StateComparisonControl.RightTitleProperty) as string;
            set => this.SetValue(StateComparisonControl.RightTitleProperty, (object)value);
        }

        public StateComparisonControl()
        {
            InitializeComponent();
            this.SetTitle("Left", "Right");
            this.Loaded += (RoutedEventHandler)delegate
           {
               PresentationSource presentationSource = PresentationSource.FromVisual((Visual)this);
               if (presentationSource == null || !(presentationSource.CompositionTarget is HwndTarget compositionTarget2))
                   return;
               compositionTarget2.RenderMode = RenderMode.SoftwareOnly;
           };
        }

        public void ShowDiff(string leftTitle, string left, string righTitle, string right)
        {
            if (string.IsNullOrEmpty(leftTitle))
                throw new ArgumentException("Title cannot be null or empty.", nameof(leftTitle));
            if (string.IsNullOrEmpty(righTitle))
                throw new ArgumentException("Title cannot be null or empty.", nameof(righTitle));
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            this.SetText(left, right);
            this.SetTitle(leftTitle, righTitle);
            if (string.Compare(left, right) == 0)
            {
                this.informationBar.Text = string.Format("Data state of {0} and {1} are the same, try to set switch EnableExplorationCleanup to true to merge them. If exploration cleanup has completed, {0} and {1} are different states because they have different incoming and outgoing transitions, it was often caused by machine composition.", (object)leftTitle, (object)righTitle);
                this.informationBar.Visibility = Visibility.Visible;
            }
            else
                this.informationBar.Visibility = Visibility.Collapsed;
            if (this.needsReDiff)
                this.CalculateDiffResults();
            this.RenderDiffResults();
        }

        private void SetText(string left, string right)
        {
            if (this.leftText != left)
            {
                this.needsReDiff = true;
                this.leftText = left;
            }
            if (!(this.rightText != right))
                return;
            this.needsReDiff = true;
            this.rightText = right;
        }

        private void SetTitle(string leftTitle, string righTitle)
        {
            this.ignoreSelectionChanged = true;
            try
            {
                if (leftTitle != this.LeftTitle || righTitle != this.RightTitle)
                {
                    this.LeftTitle = leftTitle;
                    this.RightTitle = righTitle;
                    this.ShowTextCollection.Clear();
                    this.ShowTextCollection.Add("Show Both");
                    this.ShowTextCollection.Add("Show " + leftTitle);
                    this.ShowTextCollection.Add("Show " + righTitle);
                }
                this.showWhichComboBox.SelectedIndex = 0;
            }
            finally
            {
                this.ignoreSelectionChanged = false;
            }
        }

        private void CalculateDiffResults() => this.diffResult = new List<DiffBlockPair>(new StringDiffAlgorithm(this.leftText.TrimEnd(), this.rightText.TrimEnd(), this.IntraLine).Execute());

        private void RenderDiffResults()
        {
            bool showLeft = this.showWhichComboBox.SelectedIndex == 0 || this.showWhichComboBox.SelectedIndex == 1;
            bool showRight = this.showWhichComboBox.SelectedIndex == 0 || this.showWhichComboBox.SelectedIndex == 2;
            this.documentContent.Children.Clear();
            if (this.diffResult == null)
                return;
            int? leftLineNumber = new int?(1);
            int? rightLineNumber = new int?(1);
            foreach (DiffBlockPair blockPair in this.diffResult)
                this.GenerateLine(showLeft, showRight, this.documentContent, blockPair, ref leftLineNumber, ref rightLineNumber);
        }

        private void GenerateLine(
          bool showLeft,
          bool showRight,
          StackPanel main,
          DiffBlockPair blockPair,
          ref int? leftLineNumber,
          ref int? rightLineNumber)
        {
            string left = blockPair.Left;
            string right = blockPair.Right;
            switch (blockPair.Type)
            {
                case DiffType.Identical:
                    TextBlock textBlock1 = new TextBlock();
                    textBlock1.Style = this.Resources[(object)"MatchedLineStyle"] as Style;
                    string empty = string.Empty;
                    string text;
                    if (!showRight)
                        text = leftLineNumber.ToString();
                    else if (!showLeft)
                    {
                        text = rightLineNumber.ToString();
                    }
                    else
                    {
                        int? nullable1 = leftLineNumber;
                        int? nullable2 = rightLineNumber;
                        text = (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : (nullable1.HasValue != nullable2.HasValue ? 1 : 0)) == 0 ? leftLineNumber.ToString() : leftLineNumber.ToString() + "(" + rightLineNumber.ToString() + ")";
                    }
                    TextBlock textBlock2 = new TextBlock((Inline)new Run(text));
                    textBlock2.Style = this.Resources[(object)"MatchedHeadStyle"] as Style;
                    textBlock1.Inlines.Add((UIElement)textBlock2);
                    textBlock1.Inlines.Add((Inline)new Run(left));
                    main.Children.Add((UIElement)textBlock1);
                    ref int? local1 = ref leftLineNumber;
                    int? nullable3 = local1;
                    local1 = nullable3.HasValue ? new int?(nullable3.GetValueOrDefault() + 1) : new int?();
                    ref int? local2 = ref rightLineNumber;
                    int? nullable4 = local2;
                    local2 = nullable4.HasValue ? new int?(nullable4.GetValueOrDefault() + 1) : new int?();
                    break;
                case DiffType.Deleted:
                    if (showLeft)
                    {
                        TextBlock textBlock3 = new TextBlock();
                        textBlock3.Style = this.Resources[(object)"LeftWordStyle"] as Style;
                        TextBlock textBlock4 = new TextBlock((Inline)new Run(leftLineNumber.ToString()));
                        textBlock4.Style = this.Resources[(object)"LeftHeadStyle"] as Style;
                        textBlock3.Inlines.Add((UIElement)textBlock4);
                        textBlock3.Inlines.Add((Inline)new Run(left));
                        main.Children.Add((UIElement)textBlock3);
                    }
                    ref int? local3 = ref leftLineNumber;
                    int? nullable5 = local3;
                    local3 = nullable5.HasValue ? new int?(nullable5.GetValueOrDefault() + 1) : new int?();
                    break;
                case DiffType.Inserted:
                    if (showRight)
                    {
                        TextBlock textBlock3 = new TextBlock();
                        textBlock3.Style = this.Resources[(object)"RightWordStyle"] as Style;
                        TextBlock textBlock4 = new TextBlock((Inline)new Run(rightLineNumber.ToString()));
                        textBlock4.Style = this.Resources[(object)"RightHeadStyle"] as Style;
                        textBlock3.Inlines.Add((UIElement)textBlock4);
                        textBlock3.Inlines.Add((Inline)new Run(right));
                        main.Children.Add((UIElement)textBlock3);
                    }
                    ref int? local4 = ref rightLineNumber;
                    int? nullable6 = local4;
                    local4 = nullable6.HasValue ? new int?(nullable6.GetValueOrDefault() + 1) : new int?();
                    break;
                case DiffType.Changed:
                    StackPanel main1 = new StackPanel();
                    main1.Background = this.Resources[(object)"PairBrush"] as Brush;
                    if (showLeft)
                        this.GenerateLineOfChangedLines(main1, true, blockPair, this.Resources[this.IntraLine ? (object)"LeftLineStyle" : (object)"LeftWordStyle"] as Style, this.Resources[(object)"LeftHeadStyle"] as Style, this.Resources[(object)"LeftWordBrush"] as Brush, leftLineNumber ?? 0);
                    ref int? local5 = ref leftLineNumber;
                    int? nullable7 = local5;
                    local5 = nullable7.HasValue ? new int?(nullable7.GetValueOrDefault() + 1) : new int?();
                    if (showRight)
                        this.GenerateLineOfChangedLines(main1, false, blockPair, this.Resources[this.IntraLine ? (object)"RightLineStyle" : (object)"RightWordStyle"] as Style, this.Resources[(object)"RightHeadStyle"] as Style, this.Resources[(object)"RightWordBrush"] as Brush, rightLineNumber ?? 0);
                    ref int? local6 = ref rightLineNumber;
                    int? nullable8 = local6;
                    local6 = nullable8.HasValue ? new int?(nullable8.GetValueOrDefault() + 1) : new int?();
                    main.Children.Add((UIElement)main1);
                    break;
            }
        }

        private void GenerateLineOfChangedLines(
          StackPanel main,
          bool left,
          DiffBlockPair blockPair,
          Style lineStyle,
          Style headStyle,
          Brush wordBrush,
          int lineNumber)
        {
            TextBlock main1 = new TextBlock();
            main1.Style = lineStyle;
            TextBlock textBlock = new TextBlock((Inline)new Run(lineNumber.ToString()));
            textBlock.Style = headStyle;
            main1.Inlines.Add((UIElement)textBlock);
            if (this.IntraLine)
            {
                StringBuilder tokenRun = new StringBuilder();
                foreach (DiffTokenPair tokenPair in blockPair.TokenPairs)
                    StateComparisonControl.GenerateElementsOfChangedLine(main1, left ? tokenPair.Left : tokenPair.Right, tokenRun, tokenPair, wordBrush);
                if (tokenRun.Length > 0)
                    main1.Inlines.Add((Inline)new Run(tokenRun.ToString()));
            }
            else
                main1.Inlines.Add((Inline)new Run(left ? blockPair.Left : blockPair.Right));
            main.Children.Add((UIElement)main1);
        }

        private static void GenerateElementsOfChangedLine(TextBlock main,string line,StringBuilder tokenRun, DiffTokenPair tokenPair, Brush wordBrush)
        {
            bool flag = true;
            string str = line;
            char[] chArray = new char[1] { '\n' };
            foreach (string text in str.Split(chArray))
            {
                if (flag)
                    flag = false;
                else
                    main.Inlines.Add((Inline)new LineBreak());
                if (text.Length > 0)
                {
                    if (tokenPair.Type == DiffType.Identical)
                    {
                        tokenRun.Append(text);
                    }
                    else
                    {
                        if (tokenRun.Length > 0)
                        {
                            main.Inlines.Add((Inline)new Run(tokenRun.ToString()));
                            tokenRun.Length = 0;
                        }
                        main.Inlines.Add((UIElement)new TextBlock((Inline)new Run(text))
                        {
                            Background = wordBrush
                        });
                    }
                }
            }
        }
    }
}
