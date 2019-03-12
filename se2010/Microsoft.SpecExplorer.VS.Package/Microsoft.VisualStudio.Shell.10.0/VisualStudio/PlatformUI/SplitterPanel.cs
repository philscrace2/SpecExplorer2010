// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.SplitterPanel
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Microsoft.VisualStudio.PlatformUI
{
  public class SplitterPanel : Panel
  {
    public static readonly DependencyProperty SplitterLengthProperty = DependencyProperty.RegisterAttached("SplitterLength", typeof (SplitterLength), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) new SplitterLength(100.0), FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof (Orientation), typeof (Orientation), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty ShowResizePreviewProperty = DependencyProperty.Register(nameof (ShowResizePreview), typeof (bool), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    public static readonly DependencyProperty MinimumLengthProperty = DependencyProperty.RegisterAttached("MinimumLength", typeof (double), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    public static readonly DependencyProperty MaximumLengthProperty = DependencyProperty.RegisterAttached("MaximumLength", typeof (double), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.MaxValue));
    private static readonly DependencyPropertyKey ActualSplitterLengthPropertyKey = DependencyProperty.RegisterAttachedReadOnly("ActualSplitterLength", typeof (double), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) 0.0));
    private static readonly DependencyPropertyKey IndexPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Index", typeof (int), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) -1));
    private static readonly DependencyPropertyKey IsFirstPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsFirst", typeof (bool), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    private static readonly DependencyPropertyKey IsLastPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsLast", typeof (bool), typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    public static readonly DependencyProperty ActualSplitterLengthProperty = SplitterPanel.ActualSplitterLengthPropertyKey.DependencyProperty;
    public static readonly DependencyProperty IndexProperty = SplitterPanel.IndexPropertyKey.DependencyProperty;
    public static readonly DependencyProperty IsFirstProperty = SplitterPanel.IsFirstPropertyKey.DependencyProperty;
    public static readonly DependencyProperty IsLastProperty = SplitterPanel.IsLastPropertyKey.DependencyProperty;
    private SplitterResizePreviewWindow currentPreviewWindow;

    static SplitterPanel()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (SplitterPanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (SplitterPanel)));
    }

    public SplitterPanel()
    {
      this.AddHandler(Thumb.DragStartedEvent, (Delegate) new DragStartedEventHandler(this.OnSplitterDragStarted));
      AutomationProperties.SetAutomationId((DependencyObject) this, nameof (SplitterPanel));
    }

    private bool IsShowingResizePreview
    {
      get
      {
        return this.currentPreviewWindow != null;
      }
    }

    public static double GetActualSplitterLength(UIElement element)
    {
      return (double) element.GetValue(SplitterPanel.ActualSplitterLengthProperty);
    }

    protected static void SetActualSplitterLength(UIElement element, double value)
    {
      element.SetValue(SplitterPanel.ActualSplitterLengthPropertyKey, (object) value);
    }

    public static int GetIndex(UIElement element)
    {
      return (int) element.GetValue(SplitterPanel.IndexProperty);
    }

    public static bool GetIsFirst(UIElement element)
    {
      return (bool) element.GetValue(SplitterPanel.IsFirstProperty);
    }

    protected static void SetIsFirst(UIElement element, bool value)
    {
      element.SetValue(SplitterPanel.IsFirstPropertyKey, (object) value);
    }

    public static bool GetIsLast(UIElement element)
    {
      return (bool) element.GetValue(SplitterPanel.IsLastProperty);
    }

    protected static void SetIsLast(UIElement element, bool value)
    {
      element.SetValue(SplitterPanel.IsLastPropertyKey, (object) value);
    }

    protected static void SetIndex(UIElement element, int value)
    {
      element.SetValue(SplitterPanel.IndexPropertyKey, (object) value);
    }

    public static SplitterLength GetSplitterLength(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      return (SplitterLength) element.GetValue(SplitterPanel.SplitterLengthProperty);
    }

    public static void SetSplitterLength(UIElement element, SplitterLength value)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      element.SetValue(SplitterPanel.SplitterLengthProperty, (object) value);
    }

    public static double GetMinimumLength(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      return (double) element.GetValue(SplitterPanel.MinimumLengthProperty);
    }

    public static void SetMinimumLength(UIElement element, double value)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      element.SetValue(SplitterPanel.MinimumLengthProperty, (object) value);
    }

    public static double GetMaximumLength(UIElement element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      return (double) element.GetValue(SplitterPanel.MaximumLengthProperty);
    }

    public static void SetMaximumLength(UIElement element, double value)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      element.SetValue(SplitterPanel.MaximumLengthProperty, (object) value);
    }

    public Orientation Orientation
    {
      get
      {
        return (Orientation) this.GetValue(SplitterPanel.OrientationProperty);
      }
      set
      {
        this.SetValue(SplitterPanel.OrientationProperty, (object) value);
      }
    }

    public bool ShowResizePreview
    {
      get
      {
        return (bool) this.GetValue(SplitterPanel.ShowResizePreviewProperty);
      }
      set
      {
        this.SetValue(SplitterPanel.ShowResizePreviewProperty, (object) value);
      }
    }

    private void UpdateIndices()
    {
      int count = this.InternalChildren.Count;
      int num = this.InternalChildren.Count - 1;
      for (int index = 0; index < count; ++index)
      {
        SplitterPanel.SetIndex(this.InternalChildren[index], index);
        SplitterPanel.SetIsFirst(this.InternalChildren[index], index == 0);
        SplitterPanel.SetIsLast(this.InternalChildren[index], index == num);
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      this.UpdateIndices();
      return SplitterPanel.Measure(availableSize, this.Orientation, (IEnumerable<SplitterMeasureData>) SplitterMeasureData.FromElements((IList) this.InternalChildren), true);
    }

    private static Size MeasureNonreal(
      Size availableSize,
      Orientation orientation,
      IEnumerable<SplitterMeasureData> measureData,
      bool remeasureElements)
    {
      double num1 = 0.0;
      double num2 = 0.0;
      foreach (SplitterMeasureData splitterMeasureData in measureData)
      {
        if (remeasureElements)
          splitterMeasureData.Element.Measure(availableSize);
        if (orientation == Orientation.Horizontal)
        {
          num1 += splitterMeasureData.Element.DesiredSize.Width;
          num2 = Math.Max(num2, splitterMeasureData.Element.DesiredSize.Height);
        }
        else
        {
          num1 = Math.Max(num1, splitterMeasureData.Element.DesiredSize.Width);
          num2 += splitterMeasureData.Element.DesiredSize.Height;
        }
      }
      Rect rect = new Rect(0.0, 0.0, num1, num2);
      foreach (SplitterMeasureData splitterMeasureData in measureData)
      {
        if (orientation == Orientation.Horizontal)
        {
          rect.Width = splitterMeasureData.Element.DesiredSize.Width;
          splitterMeasureData.MeasuredBounds = rect;
          rect.X += rect.Width;
        }
        else
        {
          rect.Height = splitterMeasureData.Element.DesiredSize.Height;
          splitterMeasureData.MeasuredBounds = rect;
          rect.Y += rect.Height;
        }
      }
      return new Size(num1, num2);
    }

    public static Size Measure(
      Size availableSize,
      Orientation orientation,
      IEnumerable<SplitterMeasureData> measureData,
      bool remeasureElements)
    {
      double num1 = 0.0;
      double num2 = 0.0;
      double num3 = 0.0;
      double num4 = 0.0;
      if (orientation == Orientation.Horizontal && availableSize.Width.IsNonreal() || orientation == Orientation.Vertical && availableSize.Height.IsNonreal())
        return SplitterPanel.MeasureNonreal(availableSize, orientation, measureData, remeasureElements);
      foreach (SplitterMeasureData splitterMeasureData in measureData)
      {
        SplitterLength attachedLength = splitterMeasureData.AttachedLength;
        double minimumLength = SplitterPanel.GetMinimumLength(splitterMeasureData.Element);
        if (attachedLength.IsStretch)
        {
          num1 += attachedLength.Value;
          num4 += minimumLength;
        }
        else
        {
          num2 += attachedLength.Value;
          num3 += minimumLength;
        }
        splitterMeasureData.IsMinimumReached = false;
        splitterMeasureData.IsMaximumReached = false;
      }
      double num5 = num4 + num3;
      double width = availableSize.Width;
      double height = availableSize.Height;
      double num6 = orientation == Orientation.Horizontal ? width : height;
      double num7 = num2 == 0.0 ? 0.0 : Math.Max(0.0, num6 - num1);
      double num8 = num7 == 0.0 ? num6 : num1;
      if (num5 <= num6)
      {
        foreach (SplitterMeasureData splitterMeasureData in measureData)
        {
          SplitterLength attachedLength = splitterMeasureData.AttachedLength;
          double maximumLength = SplitterPanel.GetMaximumLength(splitterMeasureData.Element);
          if (attachedLength.IsStretch && (num1 == 0.0 ? 0.0 : attachedLength.Value / num1 * num8) > maximumLength)
          {
            splitterMeasureData.IsMaximumReached = true;
            if (num1 == attachedLength.Value)
            {
              num1 = maximumLength;
              splitterMeasureData.AttachedLength = new SplitterLength(maximumLength);
            }
            else
            {
              num1 -= attachedLength.Value;
              splitterMeasureData.AttachedLength = new SplitterLength(num1);
              num1 += num1;
            }
            num7 = num2 == 0.0 ? 0.0 : Math.Max(0.0, num6 - num1);
            num8 = num7 == 0.0 ? num6 : num1;
          }
        }
        if (num7 < num3)
        {
          num7 = num3;
          num8 = num6 - num7;
        }
        foreach (SplitterMeasureData splitterMeasureData in measureData)
        {
          SplitterLength attachedLength = splitterMeasureData.AttachedLength;
          double minimumLength = SplitterPanel.GetMinimumLength(splitterMeasureData.Element);
          if (attachedLength.IsFill)
          {
            if ((num2 == 0.0 ? 0.0 : attachedLength.Value / num2 * num7) < minimumLength)
            {
              splitterMeasureData.IsMinimumReached = true;
              num7 -= minimumLength;
              num2 -= attachedLength.Value;
            }
          }
          else if ((num1 == 0.0 ? 0.0 : attachedLength.Value / num1 * num8) < minimumLength)
          {
            splitterMeasureData.IsMinimumReached = true;
            num8 -= minimumLength;
            num1 -= attachedLength.Value;
          }
        }
      }
      Size availableSize1 = new Size(width, height);
      Rect rect = new Rect(0.0, 0.0, width, height);
      foreach (SplitterMeasureData splitterMeasureData in measureData)
      {
        SplitterLength attachedLength = splitterMeasureData.AttachedLength;
        double num9 = splitterMeasureData.IsMinimumReached ? SplitterPanel.GetMinimumLength(splitterMeasureData.Element) : (!attachedLength.IsFill ? (num1 == 0.0 ? 0.0 : attachedLength.Value / num1 * num8) : (num2 == 0.0 ? 0.0 : attachedLength.Value / num2 * num7));
        if (remeasureElements)
          SplitterPanel.SetActualSplitterLength(splitterMeasureData.Element, num9);
        if (orientation == Orientation.Horizontal)
        {
          availableSize1.Width = num9;
          splitterMeasureData.MeasuredBounds = new Rect(rect.Left, rect.Top, num9, rect.Height);
          rect.X += num9;
          if (remeasureElements)
            splitterMeasureData.Element.Measure(availableSize1);
        }
        else
        {
          availableSize1.Height = num9;
          splitterMeasureData.MeasuredBounds = new Rect(rect.Left, rect.Top, rect.Width, num9);
          rect.Y += num9;
          if (remeasureElements)
            splitterMeasureData.Element.Measure(availableSize1);
        }
      }
      return new Size(width, height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      Rect finalRect = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
      foreach (UIElement internalChild in this.InternalChildren)
      {
        double actualSplitterLength = SplitterPanel.GetActualSplitterLength(internalChild);
        if (this.Orientation == Orientation.Horizontal)
        {
          finalRect.Width = actualSplitterLength;
          internalChild.Arrange(finalRect);
          finalRect.X += actualSplitterLength;
        }
        else
        {
          finalRect.Height = actualSplitterLength;
          internalChild.Arrange(finalRect);
          finalRect.Y += actualSplitterLength;
        }
      }
      return finalSize;
    }

    private void OnSplitterDragStarted(object sender, DragStartedEventArgs args)
    {
      SplitterGrip originalSource = args.OriginalSource as SplitterGrip;
      if (originalSource == null)
        return;
      args.Handled = true;
      originalSource.DragDelta += new DragDeltaEventHandler(this.OnSplitterResized);
      originalSource.DragCompleted += new DragCompletedEventHandler(this.OnSplitterDragCompleted);
      if (!this.ShowResizePreview)
        return;
      this.currentPreviewWindow = new SplitterResizePreviewWindow();
      this.currentPreviewWindow.Show((UIElement) originalSource);
    }

    private void OnSplitterDragCompleted(object sender, DragCompletedEventArgs args)
    {
      SplitterGrip grip = sender as SplitterGrip;
      if (grip == null)
        return;
      args.Handled = true;
      if (this.IsShowingResizePreview)
      {
        this.currentPreviewWindow.Hide();
        this.currentPreviewWindow = (SplitterResizePreviewWindow) null;
        if (!args.Canceled)
        {
          Point logicalUnits = new Point(args.HorizontalChange, args.VerticalChange).DeviceToLogicalUnits();
          this.CommitResize(grip, logicalUnits.X, logicalUnits.Y);
        }
      }
      grip.DragDelta -= new DragDeltaEventHandler(this.OnSplitterResized);
      grip.DragCompleted -= new DragCompletedEventHandler(this.OnSplitterDragCompleted);
    }

    private void OnSplitterResized(object sender, DragDeltaEventArgs args)
    {
      SplitterGrip grip = sender as SplitterGrip;
      if (grip == null)
        return;
      args.Handled = true;
      if (this.IsShowingResizePreview)
        this.TrackResizePreview(grip, args.HorizontalChange, args.VerticalChange);
      else
        this.CommitResize(grip, args.HorizontalChange, args.VerticalChange);
    }

    private void CommitResize(SplitterGrip grip, double horizontalChange, double verticalChange)
    {
      int gripIndex;
      int resizeIndex1;
      int resizeIndex2;
      if (!this.GetResizeIndices(grip, out gripIndex, out resizeIndex1, out resizeIndex2))
        return;
      double pixelAmount = this.Orientation == Orientation.Horizontal ? horizontalChange : verticalChange;
      this.ResizeChildren(resizeIndex1, resizeIndex2, pixelAmount);
    }

    private void TrackResizePreview(
      SplitterGrip grip,
      double horizontalChange,
      double verticalChange)
    {
      int gripIndex;
      int resizeIndex1;
      int resizeIndex2;
      if (!this.GetResizeIndices(grip, out gripIndex, out resizeIndex1, out resizeIndex2))
        return;
      double pixelAmount = this.Orientation == Orientation.Horizontal ? horizontalChange : verticalChange;
      IList<SplitterMeasureData> splitterMeasureDataList = SplitterMeasureData.FromElements((IList) this.InternalChildren);
      this.ResizeChildrenCore(splitterMeasureDataList[resizeIndex1], splitterMeasureDataList[resizeIndex2], pixelAmount);
      SplitterPanel.Measure(this.RenderSize, this.Orientation, (IEnumerable<SplitterMeasureData>) splitterMeasureDataList, false);
      Point point = grip.TransformToAncestor((Visual) this).Transform(new Point(0.0, 0.0));
      if (this.Orientation == Orientation.Horizontal)
        point.X += splitterMeasureDataList[gripIndex].MeasuredBounds.Width - this.InternalChildren[gripIndex].RenderSize.Width;
      else
        point.Y += splitterMeasureDataList[gripIndex].MeasuredBounds.Height - this.InternalChildren[gripIndex].RenderSize.Height;
      Point screen = this.PointToScreen(point);
      this.currentPreviewWindow.Move((double) (int) screen.X, (double) (int) screen.Y);
    }

    private bool GetResizeIndices(
      SplitterGrip grip,
      out int gripIndex,
      out int resizeIndex1,
      out int resizeIndex2)
    {
      for (int index = 0; index < this.InternalChildren.Count; ++index)
      {
        if (this.InternalChildren[index].IsAncestorOf((DependencyObject) grip))
        {
          gripIndex = index;
          switch (grip.ResizeBehavior)
          {
            case GridResizeBehavior.CurrentAndNext:
              resizeIndex1 = index;
              resizeIndex2 = index + 1;
              break;
            case GridResizeBehavior.PreviousAndCurrent:
              resizeIndex1 = index - 1;
              resizeIndex2 = index;
              break;
            case GridResizeBehavior.PreviousAndNext:
              resizeIndex1 = index - 1;
              resizeIndex2 = index + 1;
              break;
            default:
              throw new InvalidOperationException("BasedOnAlignment is not a valid resize behavior");
          }
          if (resizeIndex1 >= 0 && resizeIndex2 >= 0 && resizeIndex1 < this.InternalChildren.Count)
            return resizeIndex2 < this.InternalChildren.Count;
          return false;
        }
      }
      gripIndex = -1;
      resizeIndex1 = -1;
      resizeIndex2 = -1;
      return false;
    }

    internal void ResizeChildren(int index1, int index2, double pixelAmount)
    {
      SplitterMeasureData child1 = new SplitterMeasureData(this.InternalChildren[index1]);
      SplitterMeasureData child2 = new SplitterMeasureData(this.InternalChildren[index2]);
      if (!this.ResizeChildrenCore(child1, child2, pixelAmount))
        return;
      SplitterPanel.SetSplitterLength(child1.Element, child1.AttachedLength);
      SplitterPanel.SetSplitterLength(child2.Element, child2.AttachedLength);
      this.InvalidateMeasure();
    }

    private bool ResizeChildrenCore(
      SplitterMeasureData child1,
      SplitterMeasureData child2,
      double pixelAmount)
    {
      UIElement element1 = child1.Element;
      UIElement element2 = child2.Element;
      SplitterLength attachedLength1 = child1.AttachedLength;
      SplitterLength attachedLength2 = child2.AttachedLength;
      double actualSplitterLength1 = SplitterPanel.GetActualSplitterLength(element1);
      double actualSplitterLength2 = SplitterPanel.GetActualSplitterLength(element2);
      double num1 = Math.Max(0.0, Math.Min(actualSplitterLength1 + actualSplitterLength2, actualSplitterLength1 + pixelAmount));
      double num2 = Math.Max(0.0, Math.Min(actualSplitterLength1 + actualSplitterLength2, actualSplitterLength2 - pixelAmount));
      double minimumLength1 = SplitterPanel.GetMinimumLength(element1);
      double minimumLength2 = SplitterPanel.GetMinimumLength(element2);
      if (minimumLength1 + minimumLength2 > num1 + num2)
        return false;
      if (num1 < minimumLength1)
      {
        num2 -= minimumLength1 - num1;
        num1 = minimumLength1;
      }
      if (num2 < minimumLength2)
      {
        num1 -= minimumLength2 - num2;
        num2 = minimumLength2;
      }
      if (attachedLength1.IsFill && attachedLength2.IsFill || attachedLength1.IsStretch && attachedLength2.IsStretch)
      {
        child1.AttachedLength = new SplitterLength(num1 / (num1 + num2) * (attachedLength1.Value + attachedLength2.Value), attachedLength1.SplitterUnitType);
        child2.AttachedLength = new SplitterLength(num2 / (num1 + num2) * (attachedLength1.Value + attachedLength2.Value), attachedLength1.SplitterUnitType);
      }
      else if (attachedLength1.IsFill)
        child2.AttachedLength = new SplitterLength(num2, SplitterUnitType.Stretch);
      else
        child1.AttachedLength = new SplitterLength(num1, SplitterUnitType.Stretch);
      return true;
    }
  }
}
