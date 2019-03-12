// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.VsBrushes
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell
{
  public static class VsBrushes
  {
    private const string BrushPrefix = "VsBrush.";

    public static object GetBrushKey(int vsSysColor)
    {
      if (vsSysColor > -5 || vsSysColor < -505)
        throw new ArgumentOutOfRangeException(nameof (vsSysColor));
      return (object) ("VsBrush." + VsColors.GetColorBaseKey(vsSysColor));
    }

    public static int GetColorID(object vsBrushKey)
    {
      string str = vsBrushKey as string;
      if (str == null || !str.StartsWith("VsBrush."))
        throw new ArgumentOutOfRangeException(nameof (vsBrushKey));
      int num;
      if (!VsColors.ColorBaseNameToID.TryGetValue(str.Substring("VsBrush.".Length), out num))
        throw new ArgumentOutOfRangeException(nameof (vsBrushKey));
      return num;
    }

    public static object AccentBorderKey
    {
      get
      {
        return (object) "VsBrush.AccentBorder";
      }
    }

    public static object AccentDarkKey
    {
      get
      {
        return (object) "VsBrush.AccentDark";
      }
    }

    public static object AccentLightKey
    {
      get
      {
        return (object) "VsBrush.AccentLight";
      }
    }

    public static object AccentMediumKey
    {
      get
      {
        return (object) "VsBrush.AccentMedium";
      }
    }

    public static object AccentPaleKey
    {
      get
      {
        return (object) "VsBrush.AccentPale";
      }
    }

    public static object ActiveBorderKey
    {
      get
      {
        return (object) "VsBrush.ActiveBorder";
      }
    }

    public static object ActiveCaptionKey
    {
      get
      {
        return (object) "VsBrush.ActiveCaption";
      }
    }

    public static object AppWorkspaceKey
    {
      get
      {
        return (object) "VsBrush.AppWorkspace";
      }
    }

    public static object AutoHideResizeGripKey
    {
      get
      {
        return (object) "VsBrush.AutoHideResizeGrip";
      }
    }

    public static object AutoHideTabBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.AutoHideTabBackgroundBegin";
      }
    }

    public static object AutoHideTabBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.AutoHideTabBackgroundEnd";
      }
    }

    public static object AutoHideTabBorderKey
    {
      get
      {
        return (object) "VsBrush.AutoHideTabBorder";
      }
    }

    public static object AutoHideTabMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.AutoHideTabMouseOverBackgroundBegin";
      }
    }

    public static object AutoHideTabMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.AutoHideTabMouseOverBackgroundEnd";
      }
    }

    public static object AutoHideTabMouseOverBorderKey
    {
      get
      {
        return (object) "VsBrush.AutoHideTabMouseOverBorder";
      }
    }

    public static object AutoHideTabMouseOverTextKey
    {
      get
      {
        return (object) "VsBrush.AutoHideTabMouseOverText";
      }
    }

    public static object AutoHideTabTextKey
    {
      get
      {
        return (object) "VsBrush.AutoHideTabText";
      }
    }

    public static object BackgroundKey
    {
      get
      {
        return (object) "VsBrush.Background";
      }
    }

    public static object BrandedUIBackgroundKey
    {
      get
      {
        return (object) "VsBrush.BrandedUIBackground";
      }
    }

    public static object BrandedUIBorderKey
    {
      get
      {
        return (object) "VsBrush.BrandedUIBorder";
      }
    }

    public static object BrandedUIFillKey
    {
      get
      {
        return (object) "VsBrush.BrandedUIFill";
      }
    }

    public static object BrandedUITextKey
    {
      get
      {
        return (object) "VsBrush.BrandedUIText";
      }
    }

    public static object BrandedUITitleKey
    {
      get
      {
        return (object) "VsBrush.BrandedUITitle";
      }
    }

    public static object ButtonFaceKey
    {
      get
      {
        return (object) "VsBrush.ButtonFace";
      }
    }

    public static object ButtonHighlightKey
    {
      get
      {
        return (object) "VsBrush.ButtonHighlight";
      }
    }

    public static object ButtonShadowKey
    {
      get
      {
        return (object) "VsBrush.ButtonShadow";
      }
    }

    public static object ButtonTextKey
    {
      get
      {
        return (object) "VsBrush.ButtonText";
      }
    }

    public static object CaptionTextKey
    {
      get
      {
        return (object) "VsBrush.CaptionText";
      }
    }

    public static object ClassDesignerClassCompartmentKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerClassCompartment";
      }
    }

    public static object ClassDesignerClassHeaderBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerClassHeaderBackground";
      }
    }

    public static object ClassDesignerCommentBorderKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerCommentBorder";
      }
    }

    public static object ClassDesignerCommentShapeBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerCommentShapeBackground";
      }
    }

    public static object ClassDesignerCommentTextKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerCommentText";
      }
    }

    public static object ClassDesignerCompartmentSeparatorKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerCompartmentSeparator";
      }
    }

    public static object ClassDesignerConnectionRouteBorderKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerConnectionRouteBorder";
      }
    }

    public static object ClassDesignerDefaultConnectionKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerDefaultConnection";
      }
    }

    public static object ClassDesignerDefaultShapeBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerDefaultShapeBackground";
      }
    }

    public static object ClassDesignerDefaultShapeBorderKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerDefaultShapeBorder";
      }
    }

    public static object ClassDesignerDefaultShapeSubtitleKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerDefaultShapeSubtitle";
      }
    }

    public static object ClassDesignerDefaultShapeTextKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerDefaultShapeText";
      }
    }

    public static object ClassDesignerDefaultShapeTitleKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerDefaultShapeTitle";
      }
    }

    public static object ClassDesignerDefaultShapeTitleBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerDefaultShapeTitleBackground";
      }
    }

    public static object ClassDesignerDelegateCompartmentKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerDelegateCompartment";
      }
    }

    public static object ClassDesignerDelegateHeaderKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerDelegateHeader";
      }
    }

    public static object ClassDesignerDiagramBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerDiagramBackground";
      }
    }

    public static object ClassDesignerEmphasisBorderKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerEmphasisBorder";
      }
    }

    public static object ClassDesignerEnumHeaderKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerEnumHeader";
      }
    }

    public static object ClassDesignerFieldAssociationKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerFieldAssociation";
      }
    }

    public static object ClassDesignerGradientEndKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerGradientEnd";
      }
    }

    public static object ClassDesignerInheritanceKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerInheritance";
      }
    }

    public static object ClassDesignerInterfaceCompartmentKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerInterfaceCompartment";
      }
    }

    public static object ClassDesignerInterfaceHeaderKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerInterfaceHeader";
      }
    }

    public static object ClassDesignerLassoKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerLasso";
      }
    }

    public static object ClassDesignerLollipopKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerLollipop";
      }
    }

    public static object ClassDesignerPropertyAssociationKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerPropertyAssociation";
      }
    }

    public static object ClassDesignerReferencedAssemblyBorderKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerReferencedAssemblyBorder";
      }
    }

    public static object ClassDesignerResizingShapeBorderKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerResizingShapeBorder";
      }
    }

    public static object ClassDesignerShapeBorderKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerShapeBorder";
      }
    }

    public static object ClassDesignerShapeShadowKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerShapeShadow";
      }
    }

    public static object ClassDesignerTempConnectionKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerTempConnection";
      }
    }

    public static object ClassDesignerTypedefKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerTypedef";
      }
    }

    public static object ClassDesignerTypedefHeaderKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerTypedefHeader";
      }
    }

    public static object ClassDesignerUnresolvedTextKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerUnresolvedText";
      }
    }

    public static object ClassDesignerVBModuleCompartmentKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerVBModuleCompartment";
      }
    }

    public static object ClassDesignerVBModuleHeaderKey
    {
      get
      {
        return (object) "VsBrush.ClassDesignerVBModuleHeader";
      }
    }

    public static object ComboBoxBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxBackground";
      }
    }

    public static object ComboBoxBorderKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxBorder";
      }
    }

    public static object ComboBoxDisabledBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxDisabledBackground";
      }
    }

    public static object ComboBoxDisabledBorderKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxDisabledBorder";
      }
    }

    public static object ComboBoxDisabledGlyphKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxDisabledGlyph";
      }
    }

    public static object ComboBoxGlyphKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxGlyph";
      }
    }

    public static object ComboBoxMouseDownBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxMouseDownBackground";
      }
    }

    public static object ComboBoxMouseDownBorderKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxMouseDownBorder";
      }
    }

    public static object ComboBoxMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxMouseOverBackgroundBegin";
      }
    }

    public static object ComboBoxMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxMouseOverBackgroundEnd";
      }
    }

    public static object ComboBoxMouseOverBackgroundMiddle1Key
    {
      get
      {
        return (object) "VsBrush.ComboBoxMouseOverBackgroundMiddle1";
      }
    }

    public static object ComboBoxMouseOverBackgroundMiddle2Key
    {
      get
      {
        return (object) "VsBrush.ComboBoxMouseOverBackgroundMiddle2";
      }
    }

    public static object ComboBoxMouseOverBorderKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxMouseOverBorder";
      }
    }

    public static object ComboBoxMouseOverGlyphKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxMouseOverGlyph";
      }
    }

    public static object ComboBoxPopupBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxPopupBackgroundBegin";
      }
    }

    public static object ComboBoxPopupBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxPopupBackgroundEnd";
      }
    }

    public static object ComboBoxPopupBorderKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxPopupBorder";
      }
    }

    public static object CommandBarBorderKey
    {
      get
      {
        return (object) "VsBrush.CommandBarBorder";
      }
    }

    public static object CommandBarCheckBoxKey
    {
      get
      {
        return (object) "VsBrush.CommandBarCheckBox";
      }
    }

    public static object CommandBarDragHandleKey
    {
      get
      {
        return (object) "VsBrush.CommandBarDragHandle";
      }
    }

    public static object CommandBarDragHandleShadowKey
    {
      get
      {
        return (object) "VsBrush.CommandBarDragHandleShadow";
      }
    }

    public static object CommandBarGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.CommandBarGradientBegin";
      }
    }

    public static object CommandBarGradientEndKey
    {
      get
      {
        return (object) "VsBrush.CommandBarGradientEnd";
      }
    }

    public static object CommandBarGradientMiddleKey
    {
      get
      {
        return (object) "VsBrush.CommandBarGradientMiddle";
      }
    }

    public static object CommandBarHoverKey
    {
      get
      {
        return (object) "VsBrush.CommandBarHover";
      }
    }

    public static object CommandBarHoverOverSelectedKey
    {
      get
      {
        return (object) "VsBrush.CommandBarHoverOverSelected";
      }
    }

    public static object CommandBarHoverOverSelectedIconKey
    {
      get
      {
        return (object) "VsBrush.CommandBarHoverOverSelectedIcon";
      }
    }

    public static object CommandBarHoverOverSelectedIconBorderKey
    {
      get
      {
        return (object) "VsBrush.CommandBarHoverOverSelectedIconBorder";
      }
    }

    public static object CommandBarMenuBackgroundGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMenuBackgroundGradientBegin";
      }
    }

    public static object CommandBarMenuBackgroundGradientEndKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMenuBackgroundGradientEnd";
      }
    }

    public static object CommandBarMenuBorderKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMenuBorder";
      }
    }

    public static object CommandBarMenuIconBackgroundKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMenuIconBackground";
      }
    }

    public static object CommandBarMenuMouseOverSubmenuGlyphKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMenuMouseOverSubmenuGlyph";
      }
    }

    public static object CommandBarMenuSeparatorKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMenuSeparator";
      }
    }

    public static object CommandBarMenuSubmenuGlyphKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMenuSubmenuGlyph";
      }
    }

    public static object CommandBarMouseDownBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMouseDownBackgroundBegin";
      }
    }

    public static object CommandBarMouseDownBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMouseDownBackgroundEnd";
      }
    }

    public static object CommandBarMouseDownBackgroundMiddleKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMouseDownBackgroundMiddle";
      }
    }

    public static object CommandBarMouseDownBorderKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMouseDownBorder";
      }
    }

    public static object CommandBarMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMouseOverBackgroundBegin";
      }
    }

    public static object CommandBarMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMouseOverBackgroundEnd";
      }
    }

    public static object CommandBarMouseOverBackgroundMiddle1Key
    {
      get
      {
        return (object) "VsBrush.CommandBarMouseOverBackgroundMiddle1";
      }
    }

    public static object CommandBarMouseOverBackgroundMiddle2Key
    {
      get
      {
        return (object) "VsBrush.CommandBarMouseOverBackgroundMiddle2";
      }
    }

    public static object CommandBarOptionsBackgroundKey
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsBackground";
      }
    }

    public static object CommandBarOptionsGlyphKey
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsGlyph";
      }
    }

    public static object CommandBarOptionsMouseDownBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsMouseDownBackgroundBegin";
      }
    }

    public static object CommandBarOptionsMouseDownBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsMouseDownBackgroundEnd";
      }
    }

    public static object CommandBarOptionsMouseDownBackgroundMiddleKey
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsMouseDownBackgroundMiddle";
      }
    }

    public static object CommandBarOptionsMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsMouseOverBackgroundBegin";
      }
    }

    public static object CommandBarOptionsMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsMouseOverBackgroundEnd";
      }
    }

    public static object CommandBarOptionsMouseOverBackgroundMiddle1Key
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsMouseOverBackgroundMiddle1";
      }
    }

    public static object CommandBarOptionsMouseOverBackgroundMiddle2Key
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsMouseOverBackgroundMiddle2";
      }
    }

    public static object CommandBarOptionsMouseOverGlyphKey
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsMouseOverGlyph";
      }
    }

    public static object CommandBarSelectedKey
    {
      get
      {
        return (object) "VsBrush.CommandBarSelected";
      }
    }

    public static object CommandBarSelectedBorderKey
    {
      get
      {
        return (object) "VsBrush.CommandBarSelectedBorder";
      }
    }

    public static object CommandBarShadowKey
    {
      get
      {
        return (object) "VsBrush.CommandBarShadow";
      }
    }

    public static object CommandBarTextActiveKey
    {
      get
      {
        return (object) "VsBrush.CommandBarTextActive";
      }
    }

    public static object CommandBarTextHoverKey
    {
      get
      {
        return (object) "VsBrush.CommandBarTextHover";
      }
    }

    public static object CommandBarTextInactiveKey
    {
      get
      {
        return (object) "VsBrush.CommandBarTextInactive";
      }
    }

    public static object CommandBarTextSelectedKey
    {
      get
      {
        return (object) "VsBrush.CommandBarTextSelected";
      }
    }

    public static object CommandBarToolBarBorderKey
    {
      get
      {
        return (object) "VsBrush.CommandBarToolBarBorder";
      }
    }

    public static object CommandBarToolBarSeparatorKey
    {
      get
      {
        return (object) "VsBrush.CommandBarToolBarSeparator";
      }
    }

    public static object CommandShelfBackgroundGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.CommandShelfBackgroundGradientBegin";
      }
    }

    public static object CommandShelfBackgroundGradientEndKey
    {
      get
      {
        return (object) "VsBrush.CommandShelfBackgroundGradientEnd";
      }
    }

    public static object CommandShelfBackgroundGradientMiddleKey
    {
      get
      {
        return (object) "VsBrush.CommandShelfBackgroundGradientMiddle";
      }
    }

    public static object CommandShelfHighlightGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.CommandShelfHighlightGradientBegin";
      }
    }

    public static object CommandShelfHighlightGradientEndKey
    {
      get
      {
        return (object) "VsBrush.CommandShelfHighlightGradientEnd";
      }
    }

    public static object CommandShelfHighlightGradientMiddleKey
    {
      get
      {
        return (object) "VsBrush.CommandShelfHighlightGradientMiddle";
      }
    }

    public static object ControlEditHintTextKey
    {
      get
      {
        return (object) "VsBrush.ControlEditHintText";
      }
    }

    public static object ControlEditRequiredBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ControlEditRequiredBackground";
      }
    }

    public static object ControlEditRequiredHintTextKey
    {
      get
      {
        return (object) "VsBrush.ControlEditRequiredHintText";
      }
    }

    public static object ControlLinkTextKey
    {
      get
      {
        return (object) "VsBrush.ControlLinkText";
      }
    }

    public static object ControlLinkTextHoverKey
    {
      get
      {
        return (object) "VsBrush.ControlLinkTextHover";
      }
    }

    public static object ControlLinkTextPressedKey
    {
      get
      {
        return (object) "VsBrush.ControlLinkTextPressed";
      }
    }

    public static object ControlOutlineKey
    {
      get
      {
        return (object) "VsBrush.ControlOutline";
      }
    }

    public static object DebuggerDataTipActiveBackgroundKey
    {
      get
      {
        return (object) "VsBrush.DebuggerDataTipActiveBackground";
      }
    }

    public static object DebuggerDataTipActiveBorderKey
    {
      get
      {
        return (object) "VsBrush.DebuggerDataTipActiveBorder";
      }
    }

    public static object DebuggerDataTipActiveHighlightKey
    {
      get
      {
        return (object) "VsBrush.DebuggerDataTipActiveHighlight";
      }
    }

    public static object DebuggerDataTipActiveHighlightTextKey
    {
      get
      {
        return (object) "VsBrush.DebuggerDataTipActiveHighlightText";
      }
    }

    public static object DebuggerDataTipActiveSeparatorKey
    {
      get
      {
        return (object) "VsBrush.DebuggerDataTipActiveSeparator";
      }
    }

    public static object DebuggerDataTipActiveTextKey
    {
      get
      {
        return (object) "VsBrush.DebuggerDataTipActiveText";
      }
    }

    public static object DebuggerDataTipInactiveBackgroundKey
    {
      get
      {
        return (object) "VsBrush.DebuggerDataTipInactiveBackground";
      }
    }

    public static object DebuggerDataTipInactiveBorderKey
    {
      get
      {
        return (object) "VsBrush.DebuggerDataTipInactiveBorder";
      }
    }

    public static object DebuggerDataTipInactiveHighlightKey
    {
      get
      {
        return (object) "VsBrush.DebuggerDataTipInactiveHighlight";
      }
    }

    public static object DebuggerDataTipInactiveHighlightTextKey
    {
      get
      {
        return (object) "VsBrush.DebuggerDataTipInactiveHighlightText";
      }
    }

    public static object DebuggerDataTipInactiveSeparatorKey
    {
      get
      {
        return (object) "VsBrush.DebuggerDataTipInactiveSeparator";
      }
    }

    public static object DebuggerDataTipInactiveTextKey
    {
      get
      {
        return (object) "VsBrush.DebuggerDataTipInactiveText";
      }
    }

    public static object DesignerBackgroundKey
    {
      get
      {
        return (object) "VsBrush.DesignerBackground";
      }
    }

    public static object DesignerSelectionDotsKey
    {
      get
      {
        return (object) "VsBrush.DesignerSelectionDots";
      }
    }

    public static object DesignerTrayKey
    {
      get
      {
        return (object) "VsBrush.DesignerTray";
      }
    }

    public static object DesignerWatermarkKey
    {
      get
      {
        return (object) "VsBrush.DesignerWatermark";
      }
    }

    public static object DiagReportBackgroundKey
    {
      get
      {
        return (object) "VsBrush.DiagReportBackground";
      }
    }

    public static object DiagReportSecondaryPageHeaderKey
    {
      get
      {
        return (object) "VsBrush.DiagReportSecondaryPageHeader";
      }
    }

    public static object DiagReportSecondaryPageSubtitleKey
    {
      get
      {
        return (object) "VsBrush.DiagReportSecondaryPageSubtitle";
      }
    }

    public static object DiagReportSecondaryPageTitleKey
    {
      get
      {
        return (object) "VsBrush.DiagReportSecondaryPageTitle";
      }
    }

    public static object DiagReportSummaryPageHeaderKey
    {
      get
      {
        return (object) "VsBrush.DiagReportSummaryPageHeader";
      }
    }

    public static object DiagReportSummaryPageSubtitleKey
    {
      get
      {
        return (object) "VsBrush.DiagReportSummaryPageSubtitle";
      }
    }

    public static object DiagReportSummaryPageTitleKey
    {
      get
      {
        return (object) "VsBrush.DiagReportSummaryPageTitle";
      }
    }

    public static object DiagReportTextKey
    {
      get
      {
        return (object) "VsBrush.DiagReportText";
      }
    }

    public static object DockTargetBackgroundKey
    {
      get
      {
        return (object) "VsBrush.DockTargetBackground";
      }
    }

    public static object DockTargetBorderKey
    {
      get
      {
        return (object) "VsBrush.DockTargetBorder";
      }
    }

    public static object DockTargetButtonBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.DockTargetButtonBackgroundBegin";
      }
    }

    public static object DockTargetButtonBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.DockTargetButtonBackgroundEnd";
      }
    }

    public static object DockTargetButtonBorderKey
    {
      get
      {
        return (object) "VsBrush.DockTargetButtonBorder";
      }
    }

    public static object DockTargetGlyphArrowKey
    {
      get
      {
        return (object) "VsBrush.DockTargetGlyphArrow";
      }
    }

    public static object DockTargetGlyphBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.DockTargetGlyphBackgroundBegin";
      }
    }

    public static object DockTargetGlyphBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.DockTargetGlyphBackgroundEnd";
      }
    }

    public static object DockTargetGlyphBorderKey
    {
      get
      {
        return (object) "VsBrush.DockTargetGlyphBorder";
      }
    }

    public static object DropDownBackgroundKey
    {
      get
      {
        return (object) "VsBrush.DropDownBackground";
      }
    }

    public static object DropDownBorderKey
    {
      get
      {
        return (object) "VsBrush.DropDownBorder";
      }
    }

    public static object DropDownDisabledBackgroundKey
    {
      get
      {
        return (object) "VsBrush.DropDownDisabledBackground";
      }
    }

    public static object DropDownDisabledBorderKey
    {
      get
      {
        return (object) "VsBrush.DropDownDisabledBorder";
      }
    }

    public static object DropDownDisabledGlyphKey
    {
      get
      {
        return (object) "VsBrush.DropDownDisabledGlyph";
      }
    }

    public static object DropDownGlyphKey
    {
      get
      {
        return (object) "VsBrush.DropDownGlyph";
      }
    }

    public static object DropDownMouseDownBackgroundKey
    {
      get
      {
        return (object) "VsBrush.DropDownMouseDownBackground";
      }
    }

    public static object DropDownMouseDownBorderKey
    {
      get
      {
        return (object) "VsBrush.DropDownMouseDownBorder";
      }
    }

    public static object DropDownMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.DropDownMouseOverBackgroundBegin";
      }
    }

    public static object DropDownMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.DropDownMouseOverBackgroundEnd";
      }
    }

    public static object DropDownMouseOverBackgroundMiddle1Key
    {
      get
      {
        return (object) "VsBrush.DropDownMouseOverBackgroundMiddle1";
      }
    }

    public static object DropDownMouseOverBackgroundMiddle2Key
    {
      get
      {
        return (object) "VsBrush.DropDownMouseOverBackgroundMiddle2";
      }
    }

    public static object DropDownMouseOverBorderKey
    {
      get
      {
        return (object) "VsBrush.DropDownMouseOverBorder";
      }
    }

    public static object DropDownMouseOverGlyphKey
    {
      get
      {
        return (object) "VsBrush.DropDownMouseOverGlyph";
      }
    }

    public static object DropDownPopupBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.DropDownPopupBackgroundBegin";
      }
    }

    public static object DropDownPopupBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.DropDownPopupBackgroundEnd";
      }
    }

    public static object DropDownPopupBorderKey
    {
      get
      {
        return (object) "VsBrush.DropDownPopupBorder";
      }
    }

    public static object DropShadowBackgroundKey
    {
      get
      {
        return (object) "VsBrush.DropShadowBackground";
      }
    }

    public static object EditorExpansionBorderKey
    {
      get
      {
        return (object) "VsBrush.EditorExpansionBorder";
      }
    }

    public static object EditorExpansionFillKey
    {
      get
      {
        return (object) "VsBrush.EditorExpansionFill";
      }
    }

    public static object EditorExpansionLinkKey
    {
      get
      {
        return (object) "VsBrush.EditorExpansionLink";
      }
    }

    public static object EditorExpansionTextKey
    {
      get
      {
        return (object) "VsBrush.EditorExpansionText";
      }
    }

    public static object EnvironmentBackgroundKey
    {
      get
      {
        return (object) "VsBrush.EnvironmentBackground";
      }
    }

    public static object EnvironmentBackgroundGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.EnvironmentBackgroundGradientBegin";
      }
    }

    public static object EnvironmentBackgroundGradientEndKey
    {
      get
      {
        return (object) "VsBrush.EnvironmentBackgroundGradientEnd";
      }
    }

    public static object EnvironmentBackgroundGradientMiddle1Key
    {
      get
      {
        return (object) "VsBrush.EnvironmentBackgroundGradientMiddle1";
      }
    }

    public static object EnvironmentBackgroundGradientMiddle2Key
    {
      get
      {
        return (object) "VsBrush.EnvironmentBackgroundGradientMiddle2";
      }
    }

    public static object EnvironmentBackgroundTexture1Key
    {
      get
      {
        return (object) "VsBrush.EnvironmentBackgroundTexture1";
      }
    }

    public static object EnvironmentBackgroundTexture2Key
    {
      get
      {
        return (object) "VsBrush.EnvironmentBackgroundTexture2";
      }
    }

    public static object ExtensionManagerStarHighlight1Key
    {
      get
      {
        return (object) "VsBrush.ExtensionManagerStarHighlight1";
      }
    }

    public static object ExtensionManagerStarHighlight2Key
    {
      get
      {
        return (object) "VsBrush.ExtensionManagerStarHighlight2";
      }
    }

    public static object ExtensionManagerStarInactive1Key
    {
      get
      {
        return (object) "VsBrush.ExtensionManagerStarInactive1";
      }
    }

    public static object ExtensionManagerStarInactive2Key
    {
      get
      {
        return (object) "VsBrush.ExtensionManagerStarInactive2";
      }
    }

    public static object FileTabBorderKey
    {
      get
      {
        return (object) "VsBrush.FileTabBorder";
      }
    }

    public static object FileTabChannelBackgroundKey
    {
      get
      {
        return (object) "VsBrush.FileTabChannelBackground";
      }
    }

    public static object FileTabDocumentBorderBackgroundKey
    {
      get
      {
        return (object) "VsBrush.FileTabDocumentBorderBackground";
      }
    }

    public static object FileTabDocumentBorderHighlightKey
    {
      get
      {
        return (object) "VsBrush.FileTabDocumentBorderHighlight";
      }
    }

    public static object FileTabDocumentBorderShadowKey
    {
      get
      {
        return (object) "VsBrush.FileTabDocumentBorderShadow";
      }
    }

    public static object FileTabGradientDarkKey
    {
      get
      {
        return (object) "VsBrush.FileTabGradientDark";
      }
    }

    public static object FileTabGradientLightKey
    {
      get
      {
        return (object) "VsBrush.FileTabGradientLight";
      }
    }

    public static object FileTabHotBorderKey
    {
      get
      {
        return (object) "VsBrush.FileTabHotBorder";
      }
    }

    public static object FileTabHotGlyphKey
    {
      get
      {
        return (object) "VsBrush.FileTabHotGlyph";
      }
    }

    public static object FileTabHotGradientBottomKey
    {
      get
      {
        return (object) "VsBrush.FileTabHotGradientBottom";
      }
    }

    public static object FileTabHotGradientTopKey
    {
      get
      {
        return (object) "VsBrush.FileTabHotGradientTop";
      }
    }

    public static object FileTabHotTextKey
    {
      get
      {
        return (object) "VsBrush.FileTabHotText";
      }
    }

    public static object FileTabInactiveDocumentBorderBackgroundKey
    {
      get
      {
        return (object) "VsBrush.FileTabInactiveDocumentBorderBackground";
      }
    }

    public static object FileTabInactiveDocumentBorderEdgeKey
    {
      get
      {
        return (object) "VsBrush.FileTabInactiveDocumentBorderEdge";
      }
    }

    public static object FileTabInactiveGradientBottomKey
    {
      get
      {
        return (object) "VsBrush.FileTabInactiveGradientBottom";
      }
    }

    public static object FileTabInactiveGradientTopKey
    {
      get
      {
        return (object) "VsBrush.FileTabInactiveGradientTop";
      }
    }

    public static object FileTabInactiveTextKey
    {
      get
      {
        return (object) "VsBrush.FileTabInactiveText";
      }
    }

    public static object FileTabLastActiveDocumentBorderBackgroundKey
    {
      get
      {
        return (object) "VsBrush.FileTabLastActiveDocumentBorderBackground";
      }
    }

    public static object FileTabLastActiveDocumentBorderEdgeKey
    {
      get
      {
        return (object) "VsBrush.FileTabLastActiveDocumentBorderEdge";
      }
    }

    public static object FileTabLastActiveGlyphKey
    {
      get
      {
        return (object) "VsBrush.FileTabLastActiveGlyph";
      }
    }

    public static object FileTabLastActiveGradientBottomKey
    {
      get
      {
        return (object) "VsBrush.FileTabLastActiveGradientBottom";
      }
    }

    public static object FileTabLastActiveGradientMiddle1Key
    {
      get
      {
        return (object) "VsBrush.FileTabLastActiveGradientMiddle1";
      }
    }

    public static object FileTabLastActiveGradientMiddle2Key
    {
      get
      {
        return (object) "VsBrush.FileTabLastActiveGradientMiddle2";
      }
    }

    public static object FileTabLastActiveGradientTopKey
    {
      get
      {
        return (object) "VsBrush.FileTabLastActiveGradientTop";
      }
    }

    public static object FileTabLastActiveTextKey
    {
      get
      {
        return (object) "VsBrush.FileTabLastActiveText";
      }
    }

    public static object FileTabSelectedBackgroundKey
    {
      get
      {
        return (object) "VsBrush.FileTabSelectedBackground";
      }
    }

    public static object FileTabSelectedBorderKey
    {
      get
      {
        return (object) "VsBrush.FileTabSelectedBorder";
      }
    }

    public static object FileTabSelectedGradientBottomKey
    {
      get
      {
        return (object) "VsBrush.FileTabSelectedGradientBottom";
      }
    }

    public static object FileTabSelectedGradientMiddle1Key
    {
      get
      {
        return (object) "VsBrush.FileTabSelectedGradientMiddle1";
      }
    }

    public static object FileTabSelectedGradientMiddle2Key
    {
      get
      {
        return (object) "VsBrush.FileTabSelectedGradientMiddle2";
      }
    }

    public static object FileTabSelectedGradientTopKey
    {
      get
      {
        return (object) "VsBrush.FileTabSelectedGradientTop";
      }
    }

    public static object FileTabSelectedTextKey
    {
      get
      {
        return (object) "VsBrush.FileTabSelectedText";
      }
    }

    public static object FileTabTextKey
    {
      get
      {
        return (object) "VsBrush.FileTabText";
      }
    }

    public static object FormSmartTagActionTagBorderKey
    {
      get
      {
        return (object) "VsBrush.FormSmartTagActionTagBorder";
      }
    }

    public static object FormSmartTagActionTagFillKey
    {
      get
      {
        return (object) "VsBrush.FormSmartTagActionTagFill";
      }
    }

    public static object FormSmartTagObjectTagBorderKey
    {
      get
      {
        return (object) "VsBrush.FormSmartTagObjectTagBorder";
      }
    }

    public static object FormSmartTagObjectTagFillKey
    {
      get
      {
        return (object) "VsBrush.FormSmartTagObjectTagFill";
      }
    }

    public static object GrayTextKey
    {
      get
      {
        return (object) "VsBrush.GrayText";
      }
    }

    public static object GridHeadingBackgroundKey
    {
      get
      {
        return (object) "VsBrush.GridHeadingBackground";
      }
    }

    public static object GridHeadingTextKey
    {
      get
      {
        return (object) "VsBrush.GridHeadingText";
      }
    }

    public static object GridLineKey
    {
      get
      {
        return (object) "VsBrush.GridLine";
      }
    }

    public static object HelpHowDoIPaneBackgroundKey
    {
      get
      {
        return (object) "VsBrush.HelpHowDoIPaneBackground";
      }
    }

    public static object HelpHowDoIPaneLinkKey
    {
      get
      {
        return (object) "VsBrush.HelpHowDoIPaneLink";
      }
    }

    public static object HelpHowDoIPaneTextKey
    {
      get
      {
        return (object) "VsBrush.HelpHowDoIPaneText";
      }
    }

    public static object HelpHowDoITaskBackgroundKey
    {
      get
      {
        return (object) "VsBrush.HelpHowDoITaskBackground";
      }
    }

    public static object HelpHowDoITaskLinkKey
    {
      get
      {
        return (object) "VsBrush.HelpHowDoITaskLink";
      }
    }

    public static object HelpHowDoITaskTextKey
    {
      get
      {
        return (object) "VsBrush.HelpHowDoITaskText";
      }
    }

    public static object HelpSearchBackgroundKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchBackground";
      }
    }

    public static object HelpSearchBorderKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchBorder";
      }
    }

    public static object HelpSearchFilterBackgroundKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchFilterBackground";
      }
    }

    public static object HelpSearchFilterBorderKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchFilterBorder";
      }
    }

    public static object HelpSearchFilterTextKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchFilterText";
      }
    }

    public static object HelpSearchFrameBackgroundKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchFrameBackground";
      }
    }

    public static object HelpSearchFrameTextKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchFrameText";
      }
    }

    public static object HelpSearchPanelRulesKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchPanelRules";
      }
    }

    public static object HelpSearchProviderIconKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchProviderIcon";
      }
    }

    public static object HelpSearchProviderSelectedBackgroundKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchProviderSelectedBackground";
      }
    }

    public static object HelpSearchProviderSelectedTextKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchProviderSelectedText";
      }
    }

    public static object HelpSearchProviderUnselectedBackgroundKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchProviderUnselectedBackground";
      }
    }

    public static object HelpSearchProviderUnselectedTextKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchProviderUnselectedText";
      }
    }

    public static object HelpSearchResultLinkSelectedKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchResultLinkSelected";
      }
    }

    public static object HelpSearchResultLinkUnselectedKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchResultLinkUnselected";
      }
    }

    public static object HelpSearchResultSelectedBackgroundKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchResultSelectedBackground";
      }
    }

    public static object HelpSearchResultSelectedTextKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchResultSelectedText";
      }
    }

    public static object HelpSearchTextKey
    {
      get
      {
        return (object) "VsBrush.HelpSearchText";
      }
    }

    public static object HighlightKey
    {
      get
      {
        return (object) "VsBrush.Highlight";
      }
    }

    public static object HighlightTextKey
    {
      get
      {
        return (object) "VsBrush.HighlightText";
      }
    }

    public static object InactiveBorderKey
    {
      get
      {
        return (object) "VsBrush.InactiveBorder";
      }
    }

    public static object InactiveCaptionKey
    {
      get
      {
        return (object) "VsBrush.InactiveCaption";
      }
    }

    public static object InactiveCaptionTextKey
    {
      get
      {
        return (object) "VsBrush.InactiveCaptionText";
      }
    }

    public static object InfoBackgroundKey
    {
      get
      {
        return (object) "VsBrush.InfoBackground";
      }
    }

    public static object InfoTextKey
    {
      get
      {
        return (object) "VsBrush.InfoText";
      }
    }

    public static object MdiClientBorderKey
    {
      get
      {
        return (object) "VsBrush.MdiClientBorder";
      }
    }

    public static object MenuKey
    {
      get
      {
        return (object) "VsBrush.Menu";
      }
    }

    public static object MenuTextKey
    {
      get
      {
        return (object) "VsBrush.MenuText";
      }
    }

    public static object NewProjectBackgroundKey
    {
      get
      {
        return (object) "VsBrush.NewProjectBackground";
      }
    }

    public static object NewProjectItemInactiveBeginKey
    {
      get
      {
        return (object) "VsBrush.NewProjectItemInactiveBegin";
      }
    }

    public static object NewProjectItemInactiveBorderKey
    {
      get
      {
        return (object) "VsBrush.NewProjectItemInactiveBorder";
      }
    }

    public static object NewProjectItemInactiveEndKey
    {
      get
      {
        return (object) "VsBrush.NewProjectItemInactiveEnd";
      }
    }

    public static object NewProjectItemSelectedKey
    {
      get
      {
        return (object) "VsBrush.NewProjectItemSelected";
      }
    }

    public static object NewProjectItemSelectedBorderKey
    {
      get
      {
        return (object) "VsBrush.NewProjectItemSelectedBorder";
      }
    }

    public static object NewProjectProviderHoverBeginKey
    {
      get
      {
        return (object) "VsBrush.NewProjectProviderHoverBegin";
      }
    }

    public static object NewProjectProviderHoverEndKey
    {
      get
      {
        return (object) "VsBrush.NewProjectProviderHoverEnd";
      }
    }

    public static object NewProjectProviderHoverForegroundKey
    {
      get
      {
        return (object) "VsBrush.NewProjectProviderHoverForeground";
      }
    }

    public static object NewProjectProviderHoverMiddle1Key
    {
      get
      {
        return (object) "VsBrush.NewProjectProviderHoverMiddle1";
      }
    }

    public static object NewProjectProviderHoverMiddle2Key
    {
      get
      {
        return (object) "VsBrush.NewProjectProviderHoverMiddle2";
      }
    }

    public static object NewProjectProviderInactiveBeginKey
    {
      get
      {
        return (object) "VsBrush.NewProjectProviderInactiveBegin";
      }
    }

    public static object NewProjectProviderInactiveEndKey
    {
      get
      {
        return (object) "VsBrush.NewProjectProviderInactiveEnd";
      }
    }

    public static object NewProjectProviderInactiveForegroundKey
    {
      get
      {
        return (object) "VsBrush.NewProjectProviderInactiveForeground";
      }
    }

    public static object PageContentExpanderChevronKey
    {
      get
      {
        return (object) "VsBrush.PageContentExpanderChevron";
      }
    }

    public static object PageContentExpanderSeparatorKey
    {
      get
      {
        return (object) "VsBrush.PageContentExpanderSeparator";
      }
    }

    public static object PageSideBarExpanderBodyKey
    {
      get
      {
        return (object) "VsBrush.PageSideBarExpanderBody";
      }
    }

    public static object PageSideBarExpanderChevronKey
    {
      get
      {
        return (object) "VsBrush.PageSideBarExpanderChevron";
      }
    }

    public static object PageSideBarExpanderHeaderKey
    {
      get
      {
        return (object) "VsBrush.PageSideBarExpanderHeader";
      }
    }

    public static object PageSideBarExpanderHeaderHoverKey
    {
      get
      {
        return (object) "VsBrush.PageSideBarExpanderHeaderHover";
      }
    }

    public static object PageSideBarExpanderHeaderPressedKey
    {
      get
      {
        return (object) "VsBrush.PageSideBarExpanderHeaderPressed";
      }
    }

    public static object PageSideBarExpanderSeparatorKey
    {
      get
      {
        return (object) "VsBrush.PageSideBarExpanderSeparator";
      }
    }

    public static object PageSideBarExpanderTextKey
    {
      get
      {
        return (object) "VsBrush.PageSideBarExpanderText";
      }
    }

    public static object PanelBorderKey
    {
      get
      {
        return (object) "VsBrush.PanelBorder";
      }
    }

    public static object PanelGradientDarkKey
    {
      get
      {
        return (object) "VsBrush.PanelGradientDark";
      }
    }

    public static object PanelGradientLightKey
    {
      get
      {
        return (object) "VsBrush.PanelGradientLight";
      }
    }

    public static object PanelHoverOverCloseBorderKey
    {
      get
      {
        return (object) "VsBrush.PanelHoverOverCloseBorder";
      }
    }

    public static object PanelHoverOverCloseFillKey
    {
      get
      {
        return (object) "VsBrush.PanelHoverOverCloseFill";
      }
    }

    public static object PanelHyperlinkKey
    {
      get
      {
        return (object) "VsBrush.PanelHyperlink";
      }
    }

    public static object PanelHyperlinkHoverKey
    {
      get
      {
        return (object) "VsBrush.PanelHyperlinkHover";
      }
    }

    public static object PanelHyperlinkPressedKey
    {
      get
      {
        return (object) "VsBrush.PanelHyperlinkPressed";
      }
    }

    public static object PanelSeparatorKey
    {
      get
      {
        return (object) "VsBrush.PanelSeparator";
      }
    }

    public static object PanelSubGroupSeparatorKey
    {
      get
      {
        return (object) "VsBrush.PanelSubGroupSeparator";
      }
    }

    public static object PanelTextKey
    {
      get
      {
        return (object) "VsBrush.PanelText";
      }
    }

    public static object PanelTitleBarKey
    {
      get
      {
        return (object) "VsBrush.PanelTitleBar";
      }
    }

    public static object PanelTitleBarTextKey
    {
      get
      {
        return (object) "VsBrush.PanelTitleBarText";
      }
    }

    public static object PanelTitleBarUnselectedKey
    {
      get
      {
        return (object) "VsBrush.PanelTitleBarUnselected";
      }
    }

    public static object ProjectDesignerBackgroundGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerBackgroundGradientBegin";
      }
    }

    public static object ProjectDesignerBackgroundGradientEndKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerBackgroundGradientEnd";
      }
    }

    public static object ProjectDesignerBorderInsideKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerBorderInside";
      }
    }

    public static object ProjectDesignerBorderOutsideKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerBorderOutside";
      }
    }

    public static object ProjectDesignerContentsBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerContentsBackground";
      }
    }

    public static object ProjectDesignerTabBackgroundGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabBackgroundGradientBegin";
      }
    }

    public static object ProjectDesignerTabBackgroundGradientEndKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabBackgroundGradientEnd";
      }
    }

    public static object ProjectDesignerTabSelectedBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabSelectedBackground";
      }
    }

    public static object ProjectDesignerTabSelectedBorderKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabSelectedBorder";
      }
    }

    public static object ProjectDesignerTabSelectedHighlight1Key
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabSelectedHighlight1";
      }
    }

    public static object ProjectDesignerTabSelectedHighlight2Key
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabSelectedHighlight2";
      }
    }

    public static object ProjectDesignerTabSelectedInsideBorderKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabSelectedInsideBorder";
      }
    }

    public static object ProjectDesignerTabSepBottomGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabSepBottomGradientBegin";
      }
    }

    public static object ProjectDesignerTabSepBottomGradientEndKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabSepBottomGradientEnd";
      }
    }

    public static object ProjectDesignerTabSepTopGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabSepTopGradientBegin";
      }
    }

    public static object ProjectDesignerTabSepTopGradientEndKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabSepTopGradientEnd";
      }
    }

    public static object ScreenTipBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ScreenTipBackground";
      }
    }

    public static object ScreenTipBorderKey
    {
      get
      {
        return (object) "VsBrush.ScreenTipBorder";
      }
    }

    public static object ScreenTipTextKey
    {
      get
      {
        return (object) "VsBrush.ScreenTipText";
      }
    }

    public static object ScrollBarKey
    {
      get
      {
        return (object) "VsBrush.ScrollBar";
      }
    }

    public static object ScrollBarArrowBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ScrollBarArrowBackground";
      }
    }

    public static object ScrollBarArrowDisabledBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ScrollBarArrowDisabledBackground";
      }
    }

    public static object ScrollBarArrowMouseOverBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ScrollBarArrowMouseOverBackground";
      }
    }

    public static object ScrollBarArrowPressedBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ScrollBarArrowPressedBackground";
      }
    }

    public static object ScrollBarBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ScrollBarBackground";
      }
    }

    public static object ScrollBarDisabledBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ScrollBarDisabledBackground";
      }
    }

    public static object ScrollBarThumbBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ScrollBarThumbBackground";
      }
    }

    public static object ScrollBarThumbBorderKey
    {
      get
      {
        return (object) "VsBrush.ScrollBarThumbBorder";
      }
    }

    public static object ScrollBarThumbGlyphKey
    {
      get
      {
        return (object) "VsBrush.ScrollBarThumbGlyph";
      }
    }

    public static object ScrollBarThumbMouseOverBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ScrollBarThumbMouseOverBackground";
      }
    }

    public static object ScrollBarThumbPressedBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ScrollBarThumbPressedBackground";
      }
    }

    public static object SearchBoxBackgroundKey
    {
      get
      {
        return (object) "VsBrush.SearchBoxBackground";
      }
    }

    public static object SearchBoxBorderKey
    {
      get
      {
        return (object) "VsBrush.SearchBoxBorder";
      }
    }

    public static object SearchBoxMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.SearchBoxMouseOverBackgroundBegin";
      }
    }

    public static object SearchBoxMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.SearchBoxMouseOverBackgroundEnd";
      }
    }

    public static object SearchBoxMouseOverBackgroundMiddle1Key
    {
      get
      {
        return (object) "VsBrush.SearchBoxMouseOverBackgroundMiddle1";
      }
    }

    public static object SearchBoxMouseOverBackgroundMiddle2Key
    {
      get
      {
        return (object) "VsBrush.SearchBoxMouseOverBackgroundMiddle2";
      }
    }

    public static object SearchBoxMouseOverBorderKey
    {
      get
      {
        return (object) "VsBrush.SearchBoxMouseOverBorder";
      }
    }

    public static object SearchBoxPressedBackgroundKey
    {
      get
      {
        return (object) "VsBrush.SearchBoxPressedBackground";
      }
    }

    public static object SearchBoxPressedBorderKey
    {
      get
      {
        return (object) "VsBrush.SearchBoxPressedBorder";
      }
    }

    public static object SideBarBackgroundKey
    {
      get
      {
        return (object) "VsBrush.SideBarBackground";
      }
    }

    public static object SideBarGradientDarkKey
    {
      get
      {
        return (object) "VsBrush.SideBarGradientDark";
      }
    }

    public static object SideBarGradientLightKey
    {
      get
      {
        return (object) "VsBrush.SideBarGradientLight";
      }
    }

    public static object SideBarTextKey
    {
      get
      {
        return (object) "VsBrush.SideBarText";
      }
    }

    public static object SmartTagBorderKey
    {
      get
      {
        return (object) "VsBrush.SmartTagBorder";
      }
    }

    public static object SmartTagFillKey
    {
      get
      {
        return (object) "VsBrush.SmartTagFill";
      }
    }

    public static object SmartTagHoverBorderKey
    {
      get
      {
        return (object) "VsBrush.SmartTagHoverBorder";
      }
    }

    public static object SmartTagHoverFillKey
    {
      get
      {
        return (object) "VsBrush.SmartTagHoverFill";
      }
    }

    public static object SmartTagHoverTextKey
    {
      get
      {
        return (object) "VsBrush.SmartTagHoverText";
      }
    }

    public static object SmartTagTextKey
    {
      get
      {
        return (object) "VsBrush.SmartTagText";
      }
    }

    public static object SnaplinesKey
    {
      get
      {
        return (object) "VsBrush.Snaplines";
      }
    }

    public static object SnaplinesPaddingKey
    {
      get
      {
        return (object) "VsBrush.SnaplinesPadding";
      }
    }

    public static object SnaplinesTextBaselineKey
    {
      get
      {
        return (object) "VsBrush.SnaplinesTextBaseline";
      }
    }

    public static object SortBackgroundKey
    {
      get
      {
        return (object) "VsBrush.SortBackground";
      }
    }

    public static object SortTextKey
    {
      get
      {
        return (object) "VsBrush.SortText";
      }
    }

    public static object SplashScreenBorderKey
    {
      get
      {
        return (object) "VsBrush.SplashScreenBorder";
      }
    }

    public static object StartPageBackgroundGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.StartPageBackgroundGradientBegin";
      }
    }

    public static object StartPageBackgroundGradientEndKey
    {
      get
      {
        return (object) "VsBrush.StartPageBackgroundGradientEnd";
      }
    }

    public static object StartPageButtonBorderKey
    {
      get
      {
        return (object) "VsBrush.StartPageButtonBorder";
      }
    }

    public static object StartPageButtonMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.StartPageButtonMouseOverBackgroundBegin";
      }
    }

    public static object StartPageButtonMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.StartPageButtonMouseOverBackgroundEnd";
      }
    }

    public static object StartPageButtonMouseOverBackgroundMiddle1Key
    {
      get
      {
        return (object) "VsBrush.StartPageButtonMouseOverBackgroundMiddle1";
      }
    }

    public static object StartPageButtonMouseOverBackgroundMiddle2Key
    {
      get
      {
        return (object) "VsBrush.StartPageButtonMouseOverBackgroundMiddle2";
      }
    }

    public static object StartPageButtonPinDownKey
    {
      get
      {
        return (object) "VsBrush.StartPageButtonPinDown";
      }
    }

    public static object StartPageButtonPinHoverKey
    {
      get
      {
        return (object) "VsBrush.StartPageButtonPinHover";
      }
    }

    public static object StartPageButtonPinnedKey
    {
      get
      {
        return (object) "VsBrush.StartPageButtonPinned";
      }
    }

    public static object StartPageButtonTextKey
    {
      get
      {
        return (object) "VsBrush.StartPageButtonText";
      }
    }

    public static object StartPageButtonTextHoverKey
    {
      get
      {
        return (object) "VsBrush.StartPageButtonTextHover";
      }
    }

    public static object StartPageButtonUnpinnedKey
    {
      get
      {
        return (object) "VsBrush.StartPageButtonUnpinned";
      }
    }

    public static object StartPageSelectedItemBackgroundKey
    {
      get
      {
        return (object) "VsBrush.StartPageSelectedItemBackground";
      }
    }

    public static object StartPageSelectedItemStrokeKey
    {
      get
      {
        return (object) "VsBrush.StartPageSelectedItemStroke";
      }
    }

    public static object StartPageSeparatorKey
    {
      get
      {
        return (object) "VsBrush.StartPageSeparator";
      }
    }

    public static object StartPageTabBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.StartPageTabBackgroundBegin";
      }
    }

    public static object StartPageTabBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.StartPageTabBackgroundEnd";
      }
    }

    public static object StartPageTabMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.StartPageTabMouseOverBackgroundBegin";
      }
    }

    public static object StartPageTabMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.StartPageTabMouseOverBackgroundEnd";
      }
    }

    public static object StartPageTextBodyKey
    {
      get
      {
        return (object) "VsBrush.StartPageTextBody";
      }
    }

    public static object StartPageTextBodySelectedKey
    {
      get
      {
        return (object) "VsBrush.StartPageTextBodySelected";
      }
    }

    public static object StartPageTextBodyUnselectedKey
    {
      get
      {
        return (object) "VsBrush.StartPageTextBodyUnselected";
      }
    }

    public static object StartPageTextControlLinkSelectedKey
    {
      get
      {
        return (object) "VsBrush.StartPageTextControlLinkSelected";
      }
    }

    public static object StartPageTextControlLinkSelectedHoverKey
    {
      get
      {
        return (object) "VsBrush.StartPageTextControlLinkSelectedHover";
      }
    }

    public static object StartPageTextDateKey
    {
      get
      {
        return (object) "VsBrush.StartPageTextDate";
      }
    }

    public static object StartPageTextHeadingKey
    {
      get
      {
        return (object) "VsBrush.StartPageTextHeading";
      }
    }

    public static object StartPageTextHeadingMouseOverKey
    {
      get
      {
        return (object) "VsBrush.StartPageTextHeadingMouseOver";
      }
    }

    public static object StartPageTextHeadingSelectedKey
    {
      get
      {
        return (object) "VsBrush.StartPageTextHeadingSelected";
      }
    }

    public static object StartPageTextSubHeadingKey
    {
      get
      {
        return (object) "VsBrush.StartPageTextSubHeading";
      }
    }

    public static object StartPageTextSubHeadingMouseOverKey
    {
      get
      {
        return (object) "VsBrush.StartPageTextSubHeadingMouseOver";
      }
    }

    public static object StartPageTextSubHeadingSelectedKey
    {
      get
      {
        return (object) "VsBrush.StartPageTextSubHeadingSelected";
      }
    }

    public static object StartPageUnselectedItemBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.StartPageUnselectedItemBackgroundBegin";
      }
    }

    public static object StartPageUnselectedItemBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.StartPageUnselectedItemBackgroundEnd";
      }
    }

    public static object StartPageUnselectedItemStrokeKey
    {
      get
      {
        return (object) "VsBrush.StartPageUnselectedItemStroke";
      }
    }

    public static object StatusBarTextKey
    {
      get
      {
        return (object) "VsBrush.StatusBarText";
      }
    }

    public static object TaskListGridLinesKey
    {
      get
      {
        return (object) "VsBrush.TaskListGridLines";
      }
    }

    public static object ThreeDDarkShadowKey
    {
      get
      {
        return (object) "VsBrush.ThreeDDarkShadow";
      }
    }

    public static object ThreeDFaceKey
    {
      get
      {
        return (object) "VsBrush.ThreeDFace";
      }
    }

    public static object ThreeDHighlightKey
    {
      get
      {
        return (object) "VsBrush.ThreeDHighlight";
      }
    }

    public static object ThreeDLightShadowKey
    {
      get
      {
        return (object) "VsBrush.ThreeDLightShadow";
      }
    }

    public static object ThreeDShadowKey
    {
      get
      {
        return (object) "VsBrush.ThreeDShadow";
      }
    }

    public static object TitleBarActiveKey
    {
      get
      {
        return (object) "VsBrush.TitleBarActive";
      }
    }

    public static object TitleBarActiveGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.TitleBarActiveGradientBegin";
      }
    }

    public static object TitleBarActiveGradientEndKey
    {
      get
      {
        return (object) "VsBrush.TitleBarActiveGradientEnd";
      }
    }

    public static object TitleBarActiveGradientMiddle1Key
    {
      get
      {
        return (object) "VsBrush.TitleBarActiveGradientMiddle1";
      }
    }

    public static object TitleBarActiveGradientMiddle2Key
    {
      get
      {
        return (object) "VsBrush.TitleBarActiveGradientMiddle2";
      }
    }

    public static object TitleBarActiveTextKey
    {
      get
      {
        return (object) "VsBrush.TitleBarActiveText";
      }
    }

    public static object TitleBarInactiveKey
    {
      get
      {
        return (object) "VsBrush.TitleBarInactive";
      }
    }

    public static object TitleBarInactiveGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.TitleBarInactiveGradientBegin";
      }
    }

    public static object TitleBarInactiveGradientEndKey
    {
      get
      {
        return (object) "VsBrush.TitleBarInactiveGradientEnd";
      }
    }

    public static object TitleBarInactiveTextKey
    {
      get
      {
        return (object) "VsBrush.TitleBarInactiveText";
      }
    }

    public static object ToolboxBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ToolboxBackground";
      }
    }

    public static object ToolboxDividerKey
    {
      get
      {
        return (object) "VsBrush.ToolboxDivider";
      }
    }

    public static object ToolboxGradientDarkKey
    {
      get
      {
        return (object) "VsBrush.ToolboxGradientDark";
      }
    }

    public static object ToolboxGradientLightKey
    {
      get
      {
        return (object) "VsBrush.ToolboxGradientLight";
      }
    }

    public static object ToolboxHeadingAccentKey
    {
      get
      {
        return (object) "VsBrush.ToolboxHeadingAccent";
      }
    }

    public static object ToolboxHeadingBeginKey
    {
      get
      {
        return (object) "VsBrush.ToolboxHeadingBegin";
      }
    }

    public static object ToolboxHeadingEndKey
    {
      get
      {
        return (object) "VsBrush.ToolboxHeadingEnd";
      }
    }

    public static object ToolboxIconHighlightKey
    {
      get
      {
        return (object) "VsBrush.ToolboxIconHighlight";
      }
    }

    public static object ToolboxIconShadowKey
    {
      get
      {
        return (object) "VsBrush.ToolboxIconShadow";
      }
    }

    public static object ToolboxSelectedHeadingBeginKey
    {
      get
      {
        return (object) "VsBrush.ToolboxSelectedHeadingBegin";
      }
    }

    public static object ToolboxSelectedHeadingEndKey
    {
      get
      {
        return (object) "VsBrush.ToolboxSelectedHeadingEnd";
      }
    }

    public static object ToolboxSelectedHeadingMiddle1Key
    {
      get
      {
        return (object) "VsBrush.ToolboxSelectedHeadingMiddle1";
      }
    }

    public static object ToolboxSelectedHeadingMiddle2Key
    {
      get
      {
        return (object) "VsBrush.ToolboxSelectedHeadingMiddle2";
      }
    }

    public static object ToolWindowBackgroundKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowBackground";
      }
    }

    public static object ToolWindowBorderKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowBorder";
      }
    }

    public static object ToolWindowButtonActiveGlyphKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonActiveGlyph";
      }
    }

    public static object ToolWindowButtonDownKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonDown";
      }
    }

    public static object ToolWindowButtonDownActiveGlyphKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonDownActiveGlyph";
      }
    }

    public static object ToolWindowButtonDownBorderKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonDownBorder";
      }
    }

    public static object ToolWindowButtonDownInactiveGlyphKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonDownInactiveGlyph";
      }
    }

    public static object ToolWindowButtonHoverActiveKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonHoverActive";
      }
    }

    public static object ToolWindowButtonHoverActiveBorderKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonHoverActiveBorder";
      }
    }

    public static object ToolWindowButtonHoverActiveGlyphKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonHoverActiveGlyph";
      }
    }

    public static object ToolWindowButtonHoverInactiveKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonHoverInactive";
      }
    }

    public static object ToolWindowButtonHoverInactiveBorderKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonHoverInactiveBorder";
      }
    }

    public static object ToolWindowButtonHoverInactiveGlyphKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonHoverInactiveGlyph";
      }
    }

    public static object ToolWindowButtonInactiveKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonInactive";
      }
    }

    public static object ToolWindowButtonInactiveBorderKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonInactiveBorder";
      }
    }

    public static object ToolWindowButtonInactiveGlyphKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowButtonInactiveGlyph";
      }
    }

    public static object ToolWindowContentTabGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowContentTabGradientBegin";
      }
    }

    public static object ToolWindowContentTabGradientEndKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowContentTabGradientEnd";
      }
    }

    public static object ToolWindowFloatingFrameKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowFloatingFrame";
      }
    }

    public static object ToolWindowTabBorderKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowTabBorder";
      }
    }

    public static object ToolWindowTabGradientBeginKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowTabGradientBegin";
      }
    }

    public static object ToolWindowTabGradientEndKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowTabGradientEnd";
      }
    }

    public static object ToolWindowTabMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowTabMouseOverBackgroundBegin";
      }
    }

    public static object ToolWindowTabMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowTabMouseOverBackgroundEnd";
      }
    }

    public static object ToolWindowTabMouseOverBorderKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowTabMouseOverBorder";
      }
    }

    public static object ToolWindowTabMouseOverTextKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowTabMouseOverText";
      }
    }

    public static object ToolWindowTabSelectedTabKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowTabSelectedTab";
      }
    }

    public static object ToolWindowTabSelectedTextKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowTabSelectedText";
      }
    }

    public static object ToolWindowTabTextKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowTabText";
      }
    }

    public static object ToolWindowTextKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowText";
      }
    }

    public static object VizSurfaceBrownDarkKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceBrownDark";
      }
    }

    public static object VizSurfaceBrownLightKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceBrownLight";
      }
    }

    public static object VizSurfaceBrownMediumKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceBrownMedium";
      }
    }

    public static object VizSurfaceDarkGoldDarkKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceDarkGoldDark";
      }
    }

    public static object VizSurfaceDarkGoldLightKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceDarkGoldLight";
      }
    }

    public static object VizSurfaceDarkGoldMediumKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceDarkGoldMedium";
      }
    }

    public static object VizSurfaceGoldDarkKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceGoldDark";
      }
    }

    public static object VizSurfaceGoldLightKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceGoldLight";
      }
    }

    public static object VizSurfaceGoldMediumKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceGoldMedium";
      }
    }

    public static object VizSurfaceGreenDarkKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceGreenDark";
      }
    }

    public static object VizSurfaceGreenLightKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceGreenLight";
      }
    }

    public static object VizSurfaceGreenMediumKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceGreenMedium";
      }
    }

    public static object VizSurfacePlumDarkKey
    {
      get
      {
        return (object) "VsBrush.VizSurfacePlumDark";
      }
    }

    public static object VizSurfacePlumLightKey
    {
      get
      {
        return (object) "VsBrush.VizSurfacePlumLight";
      }
    }

    public static object VizSurfacePlumMediumKey
    {
      get
      {
        return (object) "VsBrush.VizSurfacePlumMedium";
      }
    }

    public static object VizSurfaceRedDarkKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceRedDark";
      }
    }

    public static object VizSurfaceRedLightKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceRedLight";
      }
    }

    public static object VizSurfaceRedMediumKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceRedMedium";
      }
    }

    public static object VizSurfaceSoftBlueDarkKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceSoftBlueDark";
      }
    }

    public static object VizSurfaceSoftBlueLightKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceSoftBlueLight";
      }
    }

    public static object VizSurfaceSoftBlueMediumKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceSoftBlueMedium";
      }
    }

    public static object VizSurfaceSteelBlueDarkKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceSteelBlueDark";
      }
    }

    public static object VizSurfaceSteelBlueLightKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceSteelBlueLight";
      }
    }

    public static object VizSurfaceSteelBlueMediumKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceSteelBlueMedium";
      }
    }

    public static object VizSurfaceStrongBlueDarkKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceStrongBlueDark";
      }
    }

    public static object VizSurfaceStrongBlueLightKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceStrongBlueLight";
      }
    }

    public static object VizSurfaceStrongBlueMediumKey
    {
      get
      {
        return (object) "VsBrush.VizSurfaceStrongBlueMedium";
      }
    }

    public static object WindowKey
    {
      get
      {
        return (object) "VsBrush.Window";
      }
    }

    public static object WindowFrameKey
    {
      get
      {
        return (object) "VsBrush.WindowFrame";
      }
    }

    public static object WindowTextKey
    {
      get
      {
        return (object) "VsBrush.WindowText";
      }
    }

    public static object WizardOrientationPanelBackgroundKey
    {
      get
      {
        return (object) "VsBrush.WizardOrientationPanelBackground";
      }
    }

    public static object WizardOrientationPanelTextKey
    {
      get
      {
        return (object) "VsBrush.WizardOrientationPanelText";
      }
    }

    public static object EnvironmentBackgroundTextureKey
    {
      get
      {
        return (object) "VsBrush.EnvironmentBackgroundTexture";
      }
    }

    public static object FileTabGradientKey
    {
      get
      {
        return (object) "VsBrush.FileTabGradient";
      }
    }

    public static object PanelGradientKey
    {
      get
      {
        return (object) "VsBrush.PanelGradient";
      }
    }

    public static object ProjectDesignerBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerBackgroundGradient";
      }
    }

    public static object ProjectDesignerTabBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabBackgroundGradient";
      }
    }

    public static object ProjectDesignerTabSepBottomGradientKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabSepBottomGradient";
      }
    }

    public static object ProjectDesignerTabSepTopGradientKey
    {
      get
      {
        return (object) "VsBrush.ProjectDesignerTabSepTopGradient";
      }
    }

    public static object SideBarGradientKey
    {
      get
      {
        return (object) "VsBrush.SideBarGradient";
      }
    }

    public static object ToolboxGradientKey
    {
      get
      {
        return (object) "VsBrush.ToolboxGradient";
      }
    }

    public static object ToolboxHeadingGradientKey
    {
      get
      {
        return (object) "VsBrush.ToolboxHeadingGradient";
      }
    }

    public static object ToolWindowTabGradientKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowTabGradient";
      }
    }

    public static object FileTabHotGradientKey
    {
      get
      {
        return (object) "VsBrush.FileTabHotGradient";
      }
    }

    public static object ToolWindowTabMouseOverBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.ToolWindowTabMouseOverBackgroundGradient";
      }
    }

    public static object AutoHideTabBackgroundVerticalGradientKey
    {
      get
      {
        return (object) "VsBrush.AutoHideTabBackgroundVerticalGradient";
      }
    }

    public static object AutoHideTabBackgroundHorizontalGradientKey
    {
      get
      {
        return (object) "VsBrush.AutoHideTabBackgroundHorizontalGradient";
      }
    }

    public static object AutoHideTabMouseOverBackgroundVerticalGradientKey
    {
      get
      {
        return (object) "VsBrush.AutoHideTabMouseOverBackgroundVerticalGradient";
      }
    }

    public static object AutoHideTabMouseOverBackgroundHorizontalGradientKey
    {
      get
      {
        return (object) "VsBrush.AutoHideTabMouseOverBackgroundHorizontalGradient";
      }
    }

    public static object EnvironmentBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.EnvironmentBackgroundGradient";
      }
    }

    public static object ComboBoxMouseOverBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxMouseOverBackgroundGradient";
      }
    }

    public static object ComboBoxPopupBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.ComboBoxPopupBackgroundGradient";
      }
    }

    public static object CommandShelfHighlightGradientKey
    {
      get
      {
        return (object) "VsBrush.CommandShelfHighlightGradient";
      }
    }

    public static object CommandShelfBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.CommandShelfBackgroundGradient";
      }
    }

    public static object CommandBarGradientKey
    {
      get
      {
        return (object) "VsBrush.CommandBarGradient";
      }
    }

    public static object CommandBarHorizontalGradientKey
    {
      get
      {
        return (object) "VsBrush.CommandBarHorizontalGradient";
      }
    }

    public static object CommandBarMouseOverBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMouseOverBackgroundGradient";
      }
    }

    public static object CommandBarMouseDownBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMouseDownBackgroundGradient";
      }
    }

    public static object CommandBarOptionsMouseOverBackgroundHorizontalGradientKey
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsMouseOverBackgroundHorizontalGradient";
      }
    }

    public static object CommandBarOptionsMouseDownBackgroundHorizontalGradientKey
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsMouseDownBackgroundHorizontalGradient";
      }
    }

    public static object CommandBarOptionsMouseOverBackgroundVerticalGradientKey
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsMouseOverBackgroundVerticalGradient";
      }
    }

    public static object CommandBarOptionsMouseDownBackgroundVerticalGradientKey
    {
      get
      {
        return (object) "VsBrush.CommandBarOptionsMouseDownBackgroundVerticalGradient";
      }
    }

    public static object CommandBarMenuBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.CommandBarMenuBackgroundGradient";
      }
    }

    public static object DockTargetButtonBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.DockTargetButtonBackgroundGradient";
      }
    }

    public static object DockTargetGlyphBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.DockTargetGlyphBackgroundGradient";
      }
    }

    public static object DropDownMouseOverBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.DropDownMouseOverBackgroundGradient";
      }
    }

    public static object DropDownPopupBackgroundGradientKey
    {
      get
      {
        return (object) "VsBrush.DropDownPopupBackgroundGradient";
      }
    }

    public static object FileTabSelectedGradientKey
    {
      get
      {
        return (object) "VsBrush.FileTabSelectedGradient";
      }
    }

    public static object FileTabInactiveGradientKey
    {
      get
      {
        return (object) "VsBrush.FileTabInactiveGradient";
      }
    }

    public static object TitleBarActiveGradientKey
    {
      get
      {
        return (object) "VsBrush.TitleBarActiveGradient";
      }
    }

    public static object FileTabLastActiveGradientKey
    {
      get
      {
        return (object) "VsBrush.FileTabLastActiveGradient";
      }
    }

    public static object NewProjectProviderHoverGradientKey
    {
      get
      {
        return (object) "VsBrush.NewProjectProviderHoverGradient";
      }
    }

    public static object NewProjectProviderInactiveGradientKey
    {
      get
      {
        return (object) "VsBrush.NewProjectProviderInactiveGradient";
      }
    }

    public static object NewProjectItemInactiveGradientKey
    {
      get
      {
        return (object) "VsBrush.NewProjectItemInactiveGradient";
      }
    }

    public static object ToolboxSelectedHeadingGradientKey
    {
      get
      {
        return (object) "VsBrush.ToolboxSelectedHeadingGradient";
      }
    }

    public static object TitleBarInactiveGradientKey
    {
      get
      {
        return (object) "VsBrush.TitleBarInactiveGradient";
      }
    }

    public static object StartPageBackgroundKey
    {
      get
      {
        return (object) "VsBrush.StartPageBackground";
      }
    }

    public static object StartPageButtonMouseOverBackgroundKey
    {
      get
      {
        return (object) "VsBrush.StartPageButtonMouseOverBackground";
      }
    }

    public static object StartPageTabBackgroundKey
    {
      get
      {
        return (object) "VsBrush.StartPageTabBackground";
      }
    }

    public static object StartPageTabMouseOverBackgroundKey
    {
      get
      {
        return (object) "VsBrush.StartPageTabMouseOverBackground";
      }
    }

    public static object StartPageUnselectedItemBackgroundKey
    {
      get
      {
        return (object) "VsBrush.StartPageUnselectedItemBackground";
      }
    }
  }
}
