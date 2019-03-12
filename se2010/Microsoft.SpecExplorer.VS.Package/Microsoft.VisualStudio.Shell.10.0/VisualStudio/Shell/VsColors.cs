// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.VsColors
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Shell
{
  public static class VsColors
  {
    private static readonly string[] ColorBaseName = new string[501];
    internal const int FirstColor = -5;
    internal const int LastColor = -505;
    private const string ColorPrefix = "VsColor.";
    private static Dictionary<string, int> colorBaseNameToID;

    internal static Dictionary<string, int> ColorBaseNameToID
    {
      get
      {
        VsColors.EnsureReverseColorNameMap();
        return VsColors.colorBaseNameToID;
      }
    }

    static VsColors()
    {
      VsColors.BuildColorNameTable();
    }

    public static object GetColorKey(int vsSysColor)
    {
      if (vsSysColor > -5 || vsSysColor < -505)
        throw new ArgumentOutOfRangeException(nameof (vsSysColor));
      return (object) ("VsColor." + VsColors.GetColorBaseKey(vsSysColor));
    }

    public static int GetColorID(object vsColorKey)
    {
      string str = vsColorKey as string;
      if (str == null || !str.StartsWith("VsColor."))
        throw new ArgumentOutOfRangeException(nameof (vsColorKey));
      int num;
      if (!VsColors.ColorBaseNameToID.TryGetValue(str.Substring("VsColor.".Length), out num))
        throw new ArgumentOutOfRangeException(nameof (vsColorKey));
      return num;
    }

    internal static string GetColorBaseKey(int colorID)
    {
      return VsColors.ColorBaseName[-5 - colorID];
    }

    private static void BuildColorNameTable()
    {
      VsColors.ColorBaseName[0] = "AccentBorder";
      VsColors.ColorBaseName[1] = "AccentDark";
      VsColors.ColorBaseName[2] = "AccentLight";
      VsColors.ColorBaseName[3] = "AccentMedium";
      VsColors.ColorBaseName[4] = "AccentPale";
      VsColors.ColorBaseName[5] = "CommandBarBorder";
      VsColors.ColorBaseName[6] = "CommandBarDragHandle";
      VsColors.ColorBaseName[7] = "CommandBarDragHandleShadow";
      VsColors.ColorBaseName[8] = "CommandBarGradientBegin";
      VsColors.ColorBaseName[9] = "CommandBarGradientEnd";
      VsColors.ColorBaseName[10] = "CommandBarGradientMiddle";
      VsColors.ColorBaseName[11] = "CommandBarHover";
      VsColors.ColorBaseName[12] = "CommandBarHoverOverSelected";
      VsColors.ColorBaseName[13] = "CommandBarHoverOverSelectedIcon";
      VsColors.ColorBaseName[14] = "CommandBarHoverOverSelectedIconBorder";
      VsColors.ColorBaseName[15] = "CommandBarSelected";
      VsColors.ColorBaseName[16] = "CommandBarShadow";
      VsColors.ColorBaseName[17] = "CommandBarTextActive";
      VsColors.ColorBaseName[18] = "CommandBarTextHover";
      VsColors.ColorBaseName[19] = "CommandBarTextInactive";
      VsColors.ColorBaseName[20] = "CommandBarTextSelected";
      VsColors.ColorBaseName[21] = "ControlEditHintText";
      VsColors.ColorBaseName[22] = "ControlEditRequiredBackground";
      VsColors.ColorBaseName[23] = "ControlEditRequiredHintText";
      VsColors.ColorBaseName[24] = "ControlLinkText";
      VsColors.ColorBaseName[25] = "ControlLinkTextHover";
      VsColors.ColorBaseName[26] = "ControlLinkTextPressed";
      VsColors.ColorBaseName[27] = "ControlOutline";
      VsColors.ColorBaseName[28] = "DebuggerDataTipActiveBackground";
      VsColors.ColorBaseName[29] = "DebuggerDataTipActiveBorder";
      VsColors.ColorBaseName[30] = "DebuggerDataTipActiveHighlight";
      VsColors.ColorBaseName[31] = "DebuggerDataTipActiveHighlightText";
      VsColors.ColorBaseName[32] = "DebuggerDataTipActiveSeparator";
      VsColors.ColorBaseName[33] = "DebuggerDataTipActiveText";
      VsColors.ColorBaseName[34] = "DebuggerDataTipInactiveBackground";
      VsColors.ColorBaseName[35] = "DebuggerDataTipInactiveBorder";
      VsColors.ColorBaseName[36] = "DebuggerDataTipInactiveHighlight";
      VsColors.ColorBaseName[37] = "DebuggerDataTipInactiveHighlightText";
      VsColors.ColorBaseName[38] = "DebuggerDataTipInactiveSeparator";
      VsColors.ColorBaseName[39] = "DebuggerDataTipInactiveText";
      VsColors.ColorBaseName[40] = "DesignerBackground";
      VsColors.ColorBaseName[41] = "DesignerSelectionDots";
      VsColors.ColorBaseName[42] = "DesignerTray";
      VsColors.ColorBaseName[43] = "DesignerWatermark";
      VsColors.ColorBaseName[44] = "EditorExpansionBorder";
      VsColors.ColorBaseName[45] = "EditorExpansionFill";
      VsColors.ColorBaseName[46] = "EditorExpansionLink";
      VsColors.ColorBaseName[47] = "EditorExpansionText";
      VsColors.ColorBaseName[48] = "EnvironmentBackground";
      VsColors.ColorBaseName[49] = "EnvironmentBackgroundGradientBegin";
      VsColors.ColorBaseName[50] = "EnvironmentBackgroundGradientEnd";
      VsColors.ColorBaseName[51] = "FileTabBorder";
      VsColors.ColorBaseName[52] = "FileTabChannelBackground";
      VsColors.ColorBaseName[53] = "FileTabGradientDark";
      VsColors.ColorBaseName[54] = "FileTabGradientLight";
      VsColors.ColorBaseName[55] = "FileTabSelectedBackground";
      VsColors.ColorBaseName[56] = "FileTabSelectedBorder";
      VsColors.ColorBaseName[57] = "FileTabSelectedText";
      VsColors.ColorBaseName[58] = "FileTabText";
      VsColors.ColorBaseName[59] = "FormSmartTagActionTagBorder";
      VsColors.ColorBaseName[60] = "FormSmartTagActionTagFill";
      VsColors.ColorBaseName[61] = "FormSmartTagObjectTagBorder";
      VsColors.ColorBaseName[62] = "FormSmartTagObjectTagFill";
      VsColors.ColorBaseName[63] = "GridHeadingBackground";
      VsColors.ColorBaseName[64] = "GridHeadingText";
      VsColors.ColorBaseName[65] = "GridLine";
      VsColors.ColorBaseName[66] = "HelpHowDoIPaneBackground";
      VsColors.ColorBaseName[67] = "HelpHowDoIPaneLink";
      VsColors.ColorBaseName[68] = "HelpHowDoIPaneText";
      VsColors.ColorBaseName[69] = "HelpHowDoITaskBackground";
      VsColors.ColorBaseName[70] = "HelpHowDoITaskLink";
      VsColors.ColorBaseName[71] = "HelpHowDoITaskText";
      VsColors.ColorBaseName[87] = "HelpSearchBackground";
      VsColors.ColorBaseName[74] = "HelpSearchBorder";
      VsColors.ColorBaseName[76] = "HelpSearchFilterBackground";
      VsColors.ColorBaseName[77] = "HelpSearchFilterBorder";
      VsColors.ColorBaseName[75] = "HelpSearchFilterText";
      VsColors.ColorBaseName[72] = "HelpSearchFrameBackground";
      VsColors.ColorBaseName[73] = "HelpSearchFrameText";
      VsColors.ColorBaseName[89] = "HelpSearchPanelRules";
      VsColors.ColorBaseName[82] = "HelpSearchProviderIcon";
      VsColors.ColorBaseName[80] = "HelpSearchProviderSelectedBackground";
      VsColors.ColorBaseName[81] = "HelpSearchProviderSelectedText";
      VsColors.ColorBaseName[78] = "HelpSearchProviderUnselectedBackground";
      VsColors.ColorBaseName[79] = "HelpSearchProviderUnselectedText";
      VsColors.ColorBaseName[83] = "HelpSearchResultLinkSelected";
      VsColors.ColorBaseName[84] = "HelpSearchResultLinkUnselected";
      VsColors.ColorBaseName[85] = "HelpSearchResultSelectedBackground";
      VsColors.ColorBaseName[86] = "HelpSearchResultSelectedText";
      VsColors.ColorBaseName[88] = "HelpSearchText";
      VsColors.ColorBaseName[90] = "MdiClientBorder";
      VsColors.ColorBaseName[91] = "PanelBorder";
      VsColors.ColorBaseName[92] = "PanelGradientDark";
      VsColors.ColorBaseName[93] = "PanelGradientLight";
      VsColors.ColorBaseName[94] = "PanelHoverOverCloseBorder";
      VsColors.ColorBaseName[95] = "PanelHoverOverCloseFill";
      VsColors.ColorBaseName[96] = "PanelHyperlink";
      VsColors.ColorBaseName[97] = "PanelHyperlinkHover";
      VsColors.ColorBaseName[98] = "PanelHyperlinkPressed";
      VsColors.ColorBaseName[99] = "PanelSeparator";
      VsColors.ColorBaseName[100] = "PanelSubGroupSeparator";
      VsColors.ColorBaseName[101] = "PanelText";
      VsColors.ColorBaseName[102] = "PanelTitleBar";
      VsColors.ColorBaseName[103] = "PanelTitleBarText";
      VsColors.ColorBaseName[104] = "PanelTitleBarUnselected";
      VsColors.ColorBaseName[105] = "ProjectDesignerBackgroundGradientBegin";
      VsColors.ColorBaseName[106] = "ProjectDesignerBackgroundGradientEnd";
      VsColors.ColorBaseName[108] = "ProjectDesignerBorderInside";
      VsColors.ColorBaseName[107] = "ProjectDesignerBorderOutside";
      VsColors.ColorBaseName[109] = "ProjectDesignerContentsBackground";
      VsColors.ColorBaseName[110] = "ProjectDesignerTabBackgroundGradientBegin";
      VsColors.ColorBaseName[111] = "ProjectDesignerTabBackgroundGradientEnd";
      VsColors.ColorBaseName[116] = "ProjectDesignerTabSelectedBackground";
      VsColors.ColorBaseName[113] = "ProjectDesignerTabSelectedBorder";
      VsColors.ColorBaseName[114] = "ProjectDesignerTabSelectedHighlight1";
      VsColors.ColorBaseName[115] = "ProjectDesignerTabSelectedHighlight2";
      VsColors.ColorBaseName[112] = "ProjectDesignerTabSelectedInsideBorder";
      VsColors.ColorBaseName[117] = "ProjectDesignerTabSepBottomGradientBegin";
      VsColors.ColorBaseName[118] = "ProjectDesignerTabSepBottomGradientEnd";
      VsColors.ColorBaseName[119] = "ProjectDesignerTabSepTopGradientBegin";
      VsColors.ColorBaseName[120] = "ProjectDesignerTabSepTopGradientEnd";
      VsColors.ColorBaseName[122] = "ScreenTipBackground";
      VsColors.ColorBaseName[121] = "ScreenTipBorder";
      VsColors.ColorBaseName[123] = "ScreenTipText";
      VsColors.ColorBaseName[124] = "SideBarBackground";
      VsColors.ColorBaseName[125] = "SideBarGradientDark";
      VsColors.ColorBaseName[126] = "SideBarGradientLight";
      VsColors.ColorBaseName[(int) sbyte.MaxValue] = "SideBarText";
      VsColors.ColorBaseName[128] = "SmartTagBorder";
      VsColors.ColorBaseName[129] = "SmartTagFill";
      VsColors.ColorBaseName[130] = "SmartTagHoverBorder";
      VsColors.ColorBaseName[131] = "SmartTagHoverFill";
      VsColors.ColorBaseName[132] = "SmartTagHoverText";
      VsColors.ColorBaseName[133] = "SmartTagText";
      VsColors.ColorBaseName[134] = "Snaplines";
      VsColors.ColorBaseName[135] = "SnaplinesPadding";
      VsColors.ColorBaseName[136] = "SnaplinesTextBaseline";
      VsColors.ColorBaseName[137] = "SortBackground";
      VsColors.ColorBaseName[138] = "SortText";
      VsColors.ColorBaseName[139] = "TaskListGridLines";
      VsColors.ColorBaseName[140] = "TitleBarActive";
      VsColors.ColorBaseName[141] = "TitleBarActiveGradientBegin";
      VsColors.ColorBaseName[142] = "TitleBarActiveGradientEnd";
      VsColors.ColorBaseName[143] = "TitleBarActiveText";
      VsColors.ColorBaseName[144] = "TitleBarInactive";
      VsColors.ColorBaseName[145] = "TitleBarInactiveGradientBegin";
      VsColors.ColorBaseName[146] = "TitleBarInactiveGradientEnd";
      VsColors.ColorBaseName[147] = "TitleBarInactiveText";
      VsColors.ColorBaseName[148] = "ToolboxBackground";
      VsColors.ColorBaseName[149] = "ToolboxDivider";
      VsColors.ColorBaseName[150] = "ToolboxGradientDark";
      VsColors.ColorBaseName[151] = "ToolboxGradientLight";
      VsColors.ColorBaseName[152] = "ToolboxHeadingAccent";
      VsColors.ColorBaseName[153] = "ToolboxHeadingBegin";
      VsColors.ColorBaseName[154] = "ToolboxHeadingEnd";
      VsColors.ColorBaseName[155] = "ToolboxIconHighlight";
      VsColors.ColorBaseName[156] = "ToolboxIconShadow";
      VsColors.ColorBaseName[157] = "ToolWindowBackground";
      VsColors.ColorBaseName[158] = "ToolWindowBorder";
      VsColors.ColorBaseName[159] = "ToolWindowButtonDown";
      VsColors.ColorBaseName[160] = "ToolWindowButtonDownBorder";
      VsColors.ColorBaseName[161] = "ToolWindowButtonHoverActive";
      VsColors.ColorBaseName[162] = "ToolWindowButtonHoverActiveBorder";
      VsColors.ColorBaseName[163] = "ToolWindowButtonHoverInactive";
      VsColors.ColorBaseName[164] = "ToolWindowButtonHoverInactiveBorder";
      VsColors.ColorBaseName[167] = "ToolWindowTabBorder";
      VsColors.ColorBaseName[168] = "ToolWindowTabGradientBegin";
      VsColors.ColorBaseName[169] = "ToolWindowTabGradientEnd";
      VsColors.ColorBaseName[166] = "ToolWindowTabSelectedTab";
      VsColors.ColorBaseName[171] = "ToolWindowTabSelectedText";
      VsColors.ColorBaseName[170] = "ToolWindowTabText";
      VsColors.ColorBaseName[165] = "ToolWindowText";
      VsColors.ColorBaseName[172] = "WizardOrientationPanelBackground";
      VsColors.ColorBaseName[173] = "WizardOrientationPanelText";
      VsColors.ColorBaseName[185] = "BrandedUIBackground";
      VsColors.ColorBaseName[183] = "BrandedUIBorder";
      VsColors.ColorBaseName[186] = "BrandedUIFill";
      VsColors.ColorBaseName[184] = "BrandedUIText";
      VsColors.ColorBaseName[182] = "BrandedUITitle";
      VsColors.ColorBaseName[180] = "FileTabDocumentBorderBackground";
      VsColors.ColorBaseName[181] = "FileTabDocumentBorderHighlight";
      VsColors.ColorBaseName[179] = "FileTabDocumentBorderShadow";
      VsColors.ColorBaseName[178] = "FileTabHotGradientBottom";
      VsColors.ColorBaseName[177] = "FileTabHotGradientTop";
      VsColors.ColorBaseName[176] = "FileTabSelectedGradientBottom";
      VsColors.ColorBaseName[175] = "FileTabSelectedGradientTop";
      VsColors.ColorBaseName[174] = "SplashScreenBorder";
      VsColors.ColorBaseName[187] = "ActiveBorder";
      VsColors.ColorBaseName[188] = "ActiveCaption";
      VsColors.ColorBaseName[189] = "AppWorkspace";
      VsColors.ColorBaseName[190] = "Background";
      VsColors.ColorBaseName[191] = "ButtonFace";
      VsColors.ColorBaseName[192] = "ButtonHighlight";
      VsColors.ColorBaseName[193] = "ButtonShadow";
      VsColors.ColorBaseName[194] = "ButtonText";
      VsColors.ColorBaseName[195] = "CaptionText";
      VsColors.ColorBaseName[196] = "GrayText";
      VsColors.ColorBaseName[197] = "Highlight";
      VsColors.ColorBaseName[198] = "HighlightText";
      VsColors.ColorBaseName[199] = "InactiveBorder";
      VsColors.ColorBaseName[200] = "InactiveCaption";
      VsColors.ColorBaseName[201] = "InactiveCaptionText";
      VsColors.ColorBaseName[202] = "InfoBackground";
      VsColors.ColorBaseName[203] = "InfoText";
      VsColors.ColorBaseName[204] = "Menu";
      VsColors.ColorBaseName[205] = "MenuText";
      VsColors.ColorBaseName[206] = "ScrollBar";
      VsColors.ColorBaseName[207] = "ThreeDDarkShadow";
      VsColors.ColorBaseName[208] = "ThreeDFace";
      VsColors.ColorBaseName[209] = "ThreeDHighlight";
      VsColors.ColorBaseName[210] = "ThreeDLightShadow";
      VsColors.ColorBaseName[211] = "ThreeDShadow";
      VsColors.ColorBaseName[212] = "Window";
      VsColors.ColorBaseName[213] = "WindowFrame";
      VsColors.ColorBaseName[214] = "WindowText";
      VsColors.ColorBaseName[215] = "AutoHideTabText";
      VsColors.ColorBaseName[216] = "AutoHideTabBackgroundBegin";
      VsColors.ColorBaseName[217] = "AutoHideTabBackgroundEnd";
      VsColors.ColorBaseName[218] = "AutoHideTabBorder";
      VsColors.ColorBaseName[219] = "AutoHideTabMouseOverText";
      VsColors.ColorBaseName[220] = "AutoHideTabMouseOverBackgroundBegin";
      VsColors.ColorBaseName[221] = "AutoHideTabMouseOverBackgroundEnd";
      VsColors.ColorBaseName[222] = "AutoHideTabMouseOverBorder";
      VsColors.ColorBaseName[223] = "AutoHideResizeGrip";
      VsColors.ColorBaseName[224] = "ClassDesignerClassCompartment";
      VsColors.ColorBaseName[225] = "ClassDesignerClassHeaderBackground";
      VsColors.ColorBaseName[226] = "ClassDesignerCommentBorder";
      VsColors.ColorBaseName[227] = "ClassDesignerCommentShapeBackground";
      VsColors.ColorBaseName[228] = "ClassDesignerCommentText";
      VsColors.ColorBaseName[229] = "ClassDesignerCompartmentSeparator";
      VsColors.ColorBaseName[230] = "ClassDesignerConnectionRouteBorder";
      VsColors.ColorBaseName[231] = "ClassDesignerDefaultConnection";
      VsColors.ColorBaseName[232] = "ClassDesignerDefaultShapeTitleBackground";
      VsColors.ColorBaseName[233] = "ClassDesignerDefaultShapeBackground";
      VsColors.ColorBaseName[234] = "ClassDesignerDefaultShapeBorder";
      VsColors.ColorBaseName[235] = "ClassDesignerDefaultShapeSubtitle";
      VsColors.ColorBaseName[236] = "ClassDesignerDefaultShapeText";
      VsColors.ColorBaseName[237] = "ClassDesignerDefaultShapeTitle";
      VsColors.ColorBaseName[238] = "ClassDesignerDelegateCompartment";
      VsColors.ColorBaseName[239] = "ClassDesignerDelegateHeader";
      VsColors.ColorBaseName[240] = "ClassDesignerDiagramBackground";
      VsColors.ColorBaseName[241] = "ClassDesignerEmphasisBorder";
      VsColors.ColorBaseName[242] = "ClassDesignerEnumHeader";
      VsColors.ColorBaseName[243] = "ClassDesignerFieldAssociation";
      VsColors.ColorBaseName[244] = "ClassDesignerGradientEnd";
      VsColors.ColorBaseName[245] = "ClassDesignerInheritance";
      VsColors.ColorBaseName[246] = "ClassDesignerInterfaceHeader";
      VsColors.ColorBaseName[247] = "ClassDesignerInterfaceCompartment";
      VsColors.ColorBaseName[248] = "ClassDesignerLasso";
      VsColors.ColorBaseName[249] = "ClassDesignerLollipop";
      VsColors.ColorBaseName[250] = "ClassDesignerPropertyAssociation";
      VsColors.ColorBaseName[251] = "ClassDesignerReferencedAssemblyBorder";
      VsColors.ColorBaseName[252] = "ClassDesignerResizingShapeBorder";
      VsColors.ColorBaseName[253] = "ClassDesignerShapeBorder";
      VsColors.ColorBaseName[254] = "ClassDesignerShapeShadow";
      VsColors.ColorBaseName[(int) byte.MaxValue] = "ClassDesignerTempConnection";
      VsColors.ColorBaseName[256] = "ClassDesignerTypedef";
      VsColors.ColorBaseName[257] = "ClassDesignerTypedefHeader";
      VsColors.ColorBaseName[258] = "ClassDesignerUnresolvedText";
      VsColors.ColorBaseName[259] = "ClassDesignerVBModuleCompartment";
      VsColors.ColorBaseName[260] = "ClassDesignerVBModuleHeader";
      VsColors.ColorBaseName[261] = "ComboBoxBackground";
      VsColors.ColorBaseName[262] = "ComboBoxBorder";
      VsColors.ColorBaseName[263] = "ComboBoxDisabledBackground";
      VsColors.ColorBaseName[264] = "ComboBoxDisabledBorder";
      VsColors.ColorBaseName[265] = "ComboBoxDisabledGlyph";
      VsColors.ColorBaseName[266] = "ComboBoxGlyph";
      VsColors.ColorBaseName[267] = "ComboBoxMouseDownBackground";
      VsColors.ColorBaseName[268] = "ComboBoxMouseDownBorder";
      VsColors.ColorBaseName[269] = "ComboBoxMouseOverBackgroundBegin";
      VsColors.ColorBaseName[270] = "ComboBoxMouseOverBackgroundMiddle1";
      VsColors.ColorBaseName[271] = "ComboBoxMouseOverBackgroundMiddle2";
      VsColors.ColorBaseName[272] = "ComboBoxMouseOverBackgroundEnd";
      VsColors.ColorBaseName[273] = "ComboBoxMouseOverBorder";
      VsColors.ColorBaseName[274] = "ComboBoxMouseOverGlyph";
      VsColors.ColorBaseName[275] = "ComboBoxPopupBackgroundBegin";
      VsColors.ColorBaseName[276] = "ComboBoxPopupBackgroundEnd";
      VsColors.ColorBaseName[277] = "ComboBoxPopupBorder";
      VsColors.ColorBaseName[278] = "CommandBarCheckBox";
      VsColors.ColorBaseName[279] = "CommandBarMenuBackgroundGradientBegin";
      VsColors.ColorBaseName[280] = "CommandBarMenuBackgroundGradientEnd";
      VsColors.ColorBaseName[281] = "CommandBarMenuBorder";
      VsColors.ColorBaseName[282] = "CommandBarMenuIconBackground";
      VsColors.ColorBaseName[283] = "CommandBarMenuMouseOverSubmenuGlyph";
      VsColors.ColorBaseName[284] = "CommandBarMenuSeparator";
      VsColors.ColorBaseName[285] = "CommandBarMenuSubmenuGlyph";
      VsColors.ColorBaseName[286] = "CommandBarMouseDownBackgroundBegin";
      VsColors.ColorBaseName[287] = "CommandBarMouseDownBackgroundMiddle";
      VsColors.ColorBaseName[288] = "CommandBarMouseDownBackgroundEnd";
      VsColors.ColorBaseName[289] = "CommandBarMouseDownBorder";
      VsColors.ColorBaseName[290] = "CommandBarMouseOverBackgroundBegin";
      VsColors.ColorBaseName[291] = "CommandBarMouseOverBackgroundMiddle1";
      VsColors.ColorBaseName[292] = "CommandBarMouseOverBackgroundMiddle2";
      VsColors.ColorBaseName[293] = "CommandBarMouseOverBackgroundEnd";
      VsColors.ColorBaseName[294] = "CommandBarOptionsBackground";
      VsColors.ColorBaseName[295] = "CommandBarOptionsGlyph";
      VsColors.ColorBaseName[296] = "CommandBarOptionsMouseDownBackgroundBegin";
      VsColors.ColorBaseName[297] = "CommandBarOptionsMouseDownBackgroundMiddle";
      VsColors.ColorBaseName[298] = "CommandBarOptionsMouseDownBackgroundEnd";
      VsColors.ColorBaseName[299] = "CommandBarOptionsMouseOverBackgroundBegin";
      VsColors.ColorBaseName[300] = "CommandBarOptionsMouseOverBackgroundMiddle1";
      VsColors.ColorBaseName[301] = "CommandBarOptionsMouseOverBackgroundMiddle2";
      VsColors.ColorBaseName[302] = "CommandBarOptionsMouseOverBackgroundEnd";
      VsColors.ColorBaseName[303] = "CommandBarOptionsMouseOverGlyph";
      VsColors.ColorBaseName[304] = "CommandBarSelectedBorder";
      VsColors.ColorBaseName[305] = "CommandBarToolBarBorder";
      VsColors.ColorBaseName[306] = "CommandBarToolBarSeparator";
      VsColors.ColorBaseName[307] = "CommandShelfBackgroundGradientBegin";
      VsColors.ColorBaseName[308] = "CommandShelfBackgroundGradientMiddle";
      VsColors.ColorBaseName[309] = "CommandShelfBackgroundGradientEnd";
      VsColors.ColorBaseName[310] = "CommandShelfHighlightGradientBegin";
      VsColors.ColorBaseName[311] = "CommandShelfHighlightGradientMiddle";
      VsColors.ColorBaseName[312] = "CommandShelfHighlightGradientEnd";
      VsColors.ColorBaseName[321] = "DockTargetBackground";
      VsColors.ColorBaseName[322] = "DockTargetBorder";
      VsColors.ColorBaseName[323] = "DockTargetButtonBackgroundBegin";
      VsColors.ColorBaseName[324] = "DockTargetButtonBackgroundEnd";
      VsColors.ColorBaseName[325] = "DockTargetButtonBorder";
      VsColors.ColorBaseName[326] = "DockTargetGlyphBackgroundBegin";
      VsColors.ColorBaseName[327] = "DockTargetGlyphBackgroundEnd";
      VsColors.ColorBaseName[328] = "DockTargetGlyphArrow";
      VsColors.ColorBaseName[329] = "DockTargetGlyphBorder";
      VsColors.ColorBaseName[313] = "DiagReportBackground";
      VsColors.ColorBaseName[314] = "DiagReportSecondaryPageHeader";
      VsColors.ColorBaseName[315] = "DiagReportSecondaryPageSubtitle";
      VsColors.ColorBaseName[316] = "DiagReportSecondaryPageTitle";
      VsColors.ColorBaseName[317] = "DiagReportSummaryPageHeader";
      VsColors.ColorBaseName[318] = "DiagReportSummaryPageSubtitle";
      VsColors.ColorBaseName[319] = "DiagReportSummaryPageTitle";
      VsColors.ColorBaseName[320] = "DiagReportText";
      VsColors.ColorBaseName[330] = "DropDownBackground";
      VsColors.ColorBaseName[331] = "DropDownBorder";
      VsColors.ColorBaseName[332] = "DropDownDisabledBackground";
      VsColors.ColorBaseName[333] = "DropDownDisabledBorder";
      VsColors.ColorBaseName[334] = "DropDownDisabledGlyph";
      VsColors.ColorBaseName[335] = "DropDownGlyph";
      VsColors.ColorBaseName[336] = "DropDownMouseDownBackground";
      VsColors.ColorBaseName[337] = "DropDownMouseDownBorder";
      VsColors.ColorBaseName[338] = "DropDownMouseOverBackgroundBegin";
      VsColors.ColorBaseName[339] = "DropDownMouseOverBackgroundMiddle1";
      VsColors.ColorBaseName[340] = "DropDownMouseOverBackgroundMiddle2";
      VsColors.ColorBaseName[341] = "DropDownMouseOverBackgroundEnd";
      VsColors.ColorBaseName[342] = "DropDownMouseOverBorder";
      VsColors.ColorBaseName[343] = "DropDownMouseOverGlyph";
      VsColors.ColorBaseName[344] = "DropDownPopupBackgroundBegin";
      VsColors.ColorBaseName[345] = "DropDownPopupBackgroundEnd";
      VsColors.ColorBaseName[346] = "DropDownPopupBorder";
      VsColors.ColorBaseName[347] = "DropShadowBackground";
      VsColors.ColorBaseName[348] = "EnvironmentBackgroundGradientMiddle1";
      VsColors.ColorBaseName[349] = "EnvironmentBackgroundGradientMiddle2";
      VsColors.ColorBaseName[350] = "EnvironmentBackgroundTexture1";
      VsColors.ColorBaseName[351] = "EnvironmentBackgroundTexture2";
      VsColors.ColorBaseName[352] = "ExtensionManagerStarHighlight1";
      VsColors.ColorBaseName[353] = "ExtensionManagerStarHighlight2";
      VsColors.ColorBaseName[354] = "ExtensionManagerStarInactive1";
      VsColors.ColorBaseName[355] = "ExtensionManagerStarInactive2";
      VsColors.ColorBaseName[356] = "FileTabHotBorder";
      VsColors.ColorBaseName[357] = "FileTabHotText";
      VsColors.ColorBaseName[358] = "FileTabHotGlyph";
      VsColors.ColorBaseName[359] = "FileTabInactiveGradientTop";
      VsColors.ColorBaseName[360] = "FileTabInactiveGradientBottom";
      VsColors.ColorBaseName[361] = "FileTabInactiveDocumentBorderBackground";
      VsColors.ColorBaseName[362] = "FileTabInactiveDocumentBorderEdge";
      VsColors.ColorBaseName[363] = "FileTabInactiveText";
      VsColors.ColorBaseName[364] = "FileTabLastActiveGradientTop";
      VsColors.ColorBaseName[365] = "FileTabLastActiveGradientMiddle1";
      VsColors.ColorBaseName[366] = "FileTabLastActiveGradientMiddle2";
      VsColors.ColorBaseName[367] = "FileTabLastActiveGradientBottom";
      VsColors.ColorBaseName[368] = "FileTabLastActiveDocumentBorderBackground";
      VsColors.ColorBaseName[369] = "FileTabLastActiveDocumentBorderEdge";
      VsColors.ColorBaseName[370] = "FileTabLastActiveText";
      VsColors.ColorBaseName[371] = "FileTabLastActiveGlyph";
      VsColors.ColorBaseName[372] = "FileTabSelectedGradientMiddle1";
      VsColors.ColorBaseName[373] = "FileTabSelectedGradientMiddle2";
      VsColors.ColorBaseName[374] = "NewProjectBackground";
      VsColors.ColorBaseName[375] = "NewProjectProviderHoverForeground";
      VsColors.ColorBaseName[376] = "NewProjectProviderHoverBegin";
      VsColors.ColorBaseName[377] = "NewProjectProviderHoverMiddle1";
      VsColors.ColorBaseName[378] = "NewProjectProviderHoverMiddle2";
      VsColors.ColorBaseName[379] = "NewProjectProviderHoverEnd";
      VsColors.ColorBaseName[380] = "NewProjectProviderInactiveForeground";
      VsColors.ColorBaseName[381] = "NewProjectProviderInactiveBegin";
      VsColors.ColorBaseName[382] = "NewProjectProviderInactiveEnd";
      VsColors.ColorBaseName[383] = "NewProjectItemSelectedBorder";
      VsColors.ColorBaseName[384] = "NewProjectItemSelected";
      VsColors.ColorBaseName[385] = "NewProjectItemInactiveBegin";
      VsColors.ColorBaseName[386] = "NewProjectItemInactiveEnd";
      VsColors.ColorBaseName[387] = "NewProjectItemInactiveBorder";
      VsColors.ColorBaseName[388] = "PageContentExpanderChevron";
      VsColors.ColorBaseName[389] = "PageContentExpanderSeparator";
      VsColors.ColorBaseName[390] = "PageSideBarExpanderBody";
      VsColors.ColorBaseName[391] = "PageSideBarExpanderChevron";
      VsColors.ColorBaseName[392] = "PageSideBarExpanderHeader";
      VsColors.ColorBaseName[393] = "PageSideBarExpanderHeaderHover";
      VsColors.ColorBaseName[394] = "PageSideBarExpanderHeaderPressed";
      VsColors.ColorBaseName[395] = "PageSideBarExpanderSeparator";
      VsColors.ColorBaseName[396] = "PageSideBarExpanderText";
      VsColors.ColorBaseName[397] = "ScrollBarArrowBackground";
      VsColors.ColorBaseName[398] = "ScrollBarArrowDisabledBackground";
      VsColors.ColorBaseName[399] = "ScrollBarArrowMouseOverBackground";
      VsColors.ColorBaseName[400] = "ScrollBarArrowPressedBackground";
      VsColors.ColorBaseName[401] = "ScrollBarBackground";
      VsColors.ColorBaseName[402] = "ScrollBarDisabledBackground";
      VsColors.ColorBaseName[403] = "ScrollBarThumbBackground";
      VsColors.ColorBaseName[404] = "ScrollBarThumbBorder";
      VsColors.ColorBaseName[405] = "ScrollBarThumbGlyph";
      VsColors.ColorBaseName[406] = "ScrollBarThumbMouseOverBackground";
      VsColors.ColorBaseName[407] = "ScrollBarThumbPressedBackground";
      VsColors.ColorBaseName[408] = "SearchBoxBackground";
      VsColors.ColorBaseName[409] = "SearchBoxBorder";
      VsColors.ColorBaseName[410] = "SearchBoxMouseOverBackgroundBegin";
      VsColors.ColorBaseName[411] = "SearchBoxMouseOverBackgroundMiddle1";
      VsColors.ColorBaseName[412] = "SearchBoxMouseOverBackgroundMiddle2";
      VsColors.ColorBaseName[413] = "SearchBoxMouseOverBackgroundEnd";
      VsColors.ColorBaseName[414] = "SearchBoxMouseOverBorder";
      VsColors.ColorBaseName[415] = "SearchBoxPressedBackground";
      VsColors.ColorBaseName[416] = "SearchBoxPressedBorder";
      VsColors.ColorBaseName[417] = "StartPageBackgroundGradientBegin";
      VsColors.ColorBaseName[418] = "StartPageBackgroundGradientEnd";
      VsColors.ColorBaseName[419] = "StartPageButtonBorder";
      VsColors.ColorBaseName[420] = "StartPageButtonMouseOverBackgroundBegin";
      VsColors.ColorBaseName[421] = "StartPageButtonMouseOverBackgroundEnd";
      VsColors.ColorBaseName[422] = "StartPageButtonMouseOverBackgroundMiddle1";
      VsColors.ColorBaseName[423] = "StartPageButtonMouseOverBackgroundMiddle2";
      VsColors.ColorBaseName[424] = "StartPageButtonPinned";
      VsColors.ColorBaseName[425] = "StartPageButtonPinDown";
      VsColors.ColorBaseName[426] = "StartPageButtonPinHover";
      VsColors.ColorBaseName[427] = "StartPageButtonUnpinned";
      VsColors.ColorBaseName[428] = "StartPageButtonText";
      VsColors.ColorBaseName[429] = "StartPageButtonTextHover";
      VsColors.ColorBaseName[430] = "StartPageSelectedItemBackground";
      VsColors.ColorBaseName[431] = "StartPageSelectedItemStroke";
      VsColors.ColorBaseName[432] = "StartPageSeparator";
      VsColors.ColorBaseName[433] = "StartPageTabBackgroundBegin";
      VsColors.ColorBaseName[434] = "StartPageTabBackgroundEnd";
      VsColors.ColorBaseName[435] = "StartPageTabMouseOverBackgroundBegin";
      VsColors.ColorBaseName[436] = "StartPageTabMouseOverBackgroundEnd";
      VsColors.ColorBaseName[437] = "StartPageTextBody";
      VsColors.ColorBaseName[438] = "StartPageTextBodySelected";
      VsColors.ColorBaseName[439] = "StartPageTextBodyUnselected";
      VsColors.ColorBaseName[440] = "StartPageTextControlLinkSelected";
      VsColors.ColorBaseName[441] = "StartPageTextControlLinkSelectedHover";
      VsColors.ColorBaseName[442] = "StartPageTextDate";
      VsColors.ColorBaseName[443] = "StartPageTextHeading";
      VsColors.ColorBaseName[444] = "StartPageTextHeadingMouseOver";
      VsColors.ColorBaseName[445] = "StartPageTextHeadingSelected";
      VsColors.ColorBaseName[446] = "StartPageTextSubHeading";
      VsColors.ColorBaseName[447] = "StartPageTextSubHeadingMouseOver";
      VsColors.ColorBaseName[448] = "StartPageTextSubHeadingSelected";
      VsColors.ColorBaseName[449] = "StartPageUnselectedItemBackgroundBegin";
      VsColors.ColorBaseName[450] = "StartPageUnselectedItemBackgroundEnd";
      VsColors.ColorBaseName[451] = "StartPageUnselectedItemStroke";
      VsColors.ColorBaseName[452] = "StatusBarText";
      VsColors.ColorBaseName[453] = "TitleBarActiveGradientMiddle1";
      VsColors.ColorBaseName[454] = "TitleBarActiveGradientMiddle2";
      VsColors.ColorBaseName[459] = "ToolWindowButtonInactiveGlyph";
      VsColors.ColorBaseName[460] = "ToolWindowButtonInactive";
      VsColors.ColorBaseName[461] = "ToolWindowButtonInactiveBorder";
      VsColors.ColorBaseName[462] = "ToolWindowButtonHoverInactiveGlyph";
      VsColors.ColorBaseName[463] = "ToolWindowButtonDownInactiveGlyph";
      VsColors.ColorBaseName[464] = "ToolWindowButtonActiveGlyph";
      VsColors.ColorBaseName[465] = "ToolWindowButtonHoverActiveGlyph";
      VsColors.ColorBaseName[466] = "ToolWindowButtonDownActiveGlyph";
      VsColors.ColorBaseName[467] = "ToolWindowContentTabGradientBegin";
      VsColors.ColorBaseName[468] = "ToolWindowContentTabGradientEnd";
      VsColors.ColorBaseName[469] = "ToolWindowFloatingFrame";
      VsColors.ColorBaseName[470] = "ToolWindowTabMouseOverBackgroundBegin";
      VsColors.ColorBaseName[471] = "ToolWindowTabMouseOverBackgroundEnd";
      VsColors.ColorBaseName[472] = "ToolWindowTabMouseOverBorder";
      VsColors.ColorBaseName[473] = "ToolWindowTabMouseOverText";
      VsColors.ColorBaseName[474] = "VizSurfaceBrownDark";
      VsColors.ColorBaseName[475] = "VizSurfaceBrownLight";
      VsColors.ColorBaseName[476] = "VizSurfaceBrownMedium";
      VsColors.ColorBaseName[477] = "VizSurfaceDarkGoldDark";
      VsColors.ColorBaseName[478] = "VizSurfaceDarkGoldLight";
      VsColors.ColorBaseName[479] = "VizSurfaceDarkGoldMedium";
      VsColors.ColorBaseName[480] = "VizSurfaceGoldDark";
      VsColors.ColorBaseName[481] = "VizSurfaceGoldLight";
      VsColors.ColorBaseName[482] = "VizSurfaceGoldMedium";
      VsColors.ColorBaseName[483] = "VizSurfaceGreenDark";
      VsColors.ColorBaseName[484] = "VizSurfaceGreenLight";
      VsColors.ColorBaseName[485] = "VizSurfaceGreenMedium";
      VsColors.ColorBaseName[486] = "VizSurfacePlumDark";
      VsColors.ColorBaseName[487] = "VizSurfacePlumLight";
      VsColors.ColorBaseName[488] = "VizSurfacePlumMedium";
      VsColors.ColorBaseName[489] = "VizSurfaceRedDark";
      VsColors.ColorBaseName[490] = "VizSurfaceRedLight";
      VsColors.ColorBaseName[491] = "VizSurfaceRedMedium";
      VsColors.ColorBaseName[492] = "VizSurfaceSoftBlueDark";
      VsColors.ColorBaseName[493] = "VizSurfaceSoftBlueLight";
      VsColors.ColorBaseName[494] = "VizSurfaceSoftBlueMedium";
      VsColors.ColorBaseName[495] = "VizSurfaceSteelBlueDark";
      VsColors.ColorBaseName[496] = "VizSurfaceSteelBlueLight";
      VsColors.ColorBaseName[497] = "VizSurfaceSteelBlueMedium";
      VsColors.ColorBaseName[498] = "VizSurfaceStrongBlueDark";
      VsColors.ColorBaseName[499] = "VizSurfaceStrongBlueLight";
      VsColors.ColorBaseName[500] = "VizSurfaceStrongBlueMedium";
      VsColors.ColorBaseName[455] = "ToolboxSelectedHeadingBegin";
      VsColors.ColorBaseName[456] = "ToolboxSelectedHeadingMiddle1";
      VsColors.ColorBaseName[457] = "ToolboxSelectedHeadingMiddle2";
      VsColors.ColorBaseName[458] = "ToolboxSelectedHeadingEnd";
    }

    private static void EnsureReverseColorNameMap()
    {
      if (VsColors.colorBaseNameToID != null)
        return;
      VsColors.colorBaseNameToID = new Dictionary<string, int>();
      for (int index = 0; index < VsColors.ColorBaseName.Length; ++index)
        VsColors.colorBaseNameToID[VsColors.ColorBaseName[index]] = -5 - index;
    }

    public static object AccentBorderKey
    {
      get
      {
        return (object) "VsColor.AccentBorder";
      }
    }

    public static object AccentDarkKey
    {
      get
      {
        return (object) "VsColor.AccentDark";
      }
    }

    public static object AccentLightKey
    {
      get
      {
        return (object) "VsColor.AccentLight";
      }
    }

    public static object AccentMediumKey
    {
      get
      {
        return (object) "VsColor.AccentMedium";
      }
    }

    public static object AccentPaleKey
    {
      get
      {
        return (object) "VsColor.AccentPale";
      }
    }

    public static object ActiveBorderKey
    {
      get
      {
        return (object) "VsColor.ActiveBorder";
      }
    }

    public static object ActiveCaptionKey
    {
      get
      {
        return (object) "VsColor.ActiveCaption";
      }
    }

    public static object AppWorkspaceKey
    {
      get
      {
        return (object) "VsColor.AppWorkspace";
      }
    }

    public static object AutoHideResizeGripKey
    {
      get
      {
        return (object) "VsColor.AutoHideResizeGrip";
      }
    }

    public static object AutoHideTabBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.AutoHideTabBackgroundBegin";
      }
    }

    public static object AutoHideTabBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.AutoHideTabBackgroundEnd";
      }
    }

    public static object AutoHideTabBorderKey
    {
      get
      {
        return (object) "VsColor.AutoHideTabBorder";
      }
    }

    public static object AutoHideTabMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.AutoHideTabMouseOverBackgroundBegin";
      }
    }

    public static object AutoHideTabMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.AutoHideTabMouseOverBackgroundEnd";
      }
    }

    public static object AutoHideTabMouseOverBorderKey
    {
      get
      {
        return (object) "VsColor.AutoHideTabMouseOverBorder";
      }
    }

    public static object AutoHideTabMouseOverTextKey
    {
      get
      {
        return (object) "VsColor.AutoHideTabMouseOverText";
      }
    }

    public static object AutoHideTabTextKey
    {
      get
      {
        return (object) "VsColor.AutoHideTabText";
      }
    }

    public static object BackgroundKey
    {
      get
      {
        return (object) "VsColor.Background";
      }
    }

    public static object BrandedUIBackgroundKey
    {
      get
      {
        return (object) "VsColor.BrandedUIBackground";
      }
    }

    public static object BrandedUIBorderKey
    {
      get
      {
        return (object) "VsColor.BrandedUIBorder";
      }
    }

    public static object BrandedUIFillKey
    {
      get
      {
        return (object) "VsColor.BrandedUIFill";
      }
    }

    public static object BrandedUITextKey
    {
      get
      {
        return (object) "VsColor.BrandedUIText";
      }
    }

    public static object BrandedUITitleKey
    {
      get
      {
        return (object) "VsColor.BrandedUITitle";
      }
    }

    public static object ButtonFaceKey
    {
      get
      {
        return (object) "VsColor.ButtonFace";
      }
    }

    public static object ButtonHighlightKey
    {
      get
      {
        return (object) "VsColor.ButtonHighlight";
      }
    }

    public static object ButtonShadowKey
    {
      get
      {
        return (object) "VsColor.ButtonShadow";
      }
    }

    public static object ButtonTextKey
    {
      get
      {
        return (object) "VsColor.ButtonText";
      }
    }

    public static object CaptionTextKey
    {
      get
      {
        return (object) "VsColor.CaptionText";
      }
    }

    public static object ClassDesignerClassCompartmentKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerClassCompartment";
      }
    }

    public static object ClassDesignerClassHeaderBackgroundKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerClassHeaderBackground";
      }
    }

    public static object ClassDesignerCommentBorderKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerCommentBorder";
      }
    }

    public static object ClassDesignerCommentShapeBackgroundKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerCommentShapeBackground";
      }
    }

    public static object ClassDesignerCommentTextKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerCommentText";
      }
    }

    public static object ClassDesignerCompartmentSeparatorKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerCompartmentSeparator";
      }
    }

    public static object ClassDesignerConnectionRouteBorderKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerConnectionRouteBorder";
      }
    }

    public static object ClassDesignerDefaultConnectionKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerDefaultConnection";
      }
    }

    public static object ClassDesignerDefaultShapeBackgroundKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerDefaultShapeBackground";
      }
    }

    public static object ClassDesignerDefaultShapeBorderKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerDefaultShapeBorder";
      }
    }

    public static object ClassDesignerDefaultShapeSubtitleKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerDefaultShapeSubtitle";
      }
    }

    public static object ClassDesignerDefaultShapeTextKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerDefaultShapeText";
      }
    }

    public static object ClassDesignerDefaultShapeTitleKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerDefaultShapeTitle";
      }
    }

    public static object ClassDesignerDefaultShapeTitleBackgroundKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerDefaultShapeTitleBackground";
      }
    }

    public static object ClassDesignerDelegateCompartmentKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerDelegateCompartment";
      }
    }

    public static object ClassDesignerDelegateHeaderKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerDelegateHeader";
      }
    }

    public static object ClassDesignerDiagramBackgroundKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerDiagramBackground";
      }
    }

    public static object ClassDesignerEmphasisBorderKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerEmphasisBorder";
      }
    }

    public static object ClassDesignerEnumHeaderKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerEnumHeader";
      }
    }

    public static object ClassDesignerFieldAssociationKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerFieldAssociation";
      }
    }

    public static object ClassDesignerGradientEndKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerGradientEnd";
      }
    }

    public static object ClassDesignerInheritanceKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerInheritance";
      }
    }

    public static object ClassDesignerInterfaceCompartmentKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerInterfaceCompartment";
      }
    }

    public static object ClassDesignerInterfaceHeaderKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerInterfaceHeader";
      }
    }

    public static object ClassDesignerLassoKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerLasso";
      }
    }

    public static object ClassDesignerLollipopKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerLollipop";
      }
    }

    public static object ClassDesignerPropertyAssociationKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerPropertyAssociation";
      }
    }

    public static object ClassDesignerReferencedAssemblyBorderKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerReferencedAssemblyBorder";
      }
    }

    public static object ClassDesignerResizingShapeBorderKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerResizingShapeBorder";
      }
    }

    public static object ClassDesignerShapeBorderKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerShapeBorder";
      }
    }

    public static object ClassDesignerShapeShadowKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerShapeShadow";
      }
    }

    public static object ClassDesignerTempConnectionKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerTempConnection";
      }
    }

    public static object ClassDesignerTypedefKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerTypedef";
      }
    }

    public static object ClassDesignerTypedefHeaderKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerTypedefHeader";
      }
    }

    public static object ClassDesignerUnresolvedTextKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerUnresolvedText";
      }
    }

    public static object ClassDesignerVBModuleCompartmentKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerVBModuleCompartment";
      }
    }

    public static object ClassDesignerVBModuleHeaderKey
    {
      get
      {
        return (object) "VsColor.ClassDesignerVBModuleHeader";
      }
    }

    public static object ComboBoxBackgroundKey
    {
      get
      {
        return (object) "VsColor.ComboBoxBackground";
      }
    }

    public static object ComboBoxBorderKey
    {
      get
      {
        return (object) "VsColor.ComboBoxBorder";
      }
    }

    public static object ComboBoxDisabledBackgroundKey
    {
      get
      {
        return (object) "VsColor.ComboBoxDisabledBackground";
      }
    }

    public static object ComboBoxDisabledBorderKey
    {
      get
      {
        return (object) "VsColor.ComboBoxDisabledBorder";
      }
    }

    public static object ComboBoxDisabledGlyphKey
    {
      get
      {
        return (object) "VsColor.ComboBoxDisabledGlyph";
      }
    }

    public static object ComboBoxGlyphKey
    {
      get
      {
        return (object) "VsColor.ComboBoxGlyph";
      }
    }

    public static object ComboBoxMouseDownBackgroundKey
    {
      get
      {
        return (object) "VsColor.ComboBoxMouseDownBackground";
      }
    }

    public static object ComboBoxMouseDownBorderKey
    {
      get
      {
        return (object) "VsColor.ComboBoxMouseDownBorder";
      }
    }

    public static object ComboBoxMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.ComboBoxMouseOverBackgroundBegin";
      }
    }

    public static object ComboBoxMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.ComboBoxMouseOverBackgroundEnd";
      }
    }

    public static object ComboBoxMouseOverBackgroundMiddle1Key
    {
      get
      {
        return (object) "VsColor.ComboBoxMouseOverBackgroundMiddle1";
      }
    }

    public static object ComboBoxMouseOverBackgroundMiddle2Key
    {
      get
      {
        return (object) "VsColor.ComboBoxMouseOverBackgroundMiddle2";
      }
    }

    public static object ComboBoxMouseOverBorderKey
    {
      get
      {
        return (object) "VsColor.ComboBoxMouseOverBorder";
      }
    }

    public static object ComboBoxMouseOverGlyphKey
    {
      get
      {
        return (object) "VsColor.ComboBoxMouseOverGlyph";
      }
    }

    public static object ComboBoxPopupBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.ComboBoxPopupBackgroundBegin";
      }
    }

    public static object ComboBoxPopupBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.ComboBoxPopupBackgroundEnd";
      }
    }

    public static object ComboBoxPopupBorderKey
    {
      get
      {
        return (object) "VsColor.ComboBoxPopupBorder";
      }
    }

    public static object CommandBarBorderKey
    {
      get
      {
        return (object) "VsColor.CommandBarBorder";
      }
    }

    public static object CommandBarCheckBoxKey
    {
      get
      {
        return (object) "VsColor.CommandBarCheckBox";
      }
    }

    public static object CommandBarDragHandleKey
    {
      get
      {
        return (object) "VsColor.CommandBarDragHandle";
      }
    }

    public static object CommandBarDragHandleShadowKey
    {
      get
      {
        return (object) "VsColor.CommandBarDragHandleShadow";
      }
    }

    public static object CommandBarGradientBeginKey
    {
      get
      {
        return (object) "VsColor.CommandBarGradientBegin";
      }
    }

    public static object CommandBarGradientEndKey
    {
      get
      {
        return (object) "VsColor.CommandBarGradientEnd";
      }
    }

    public static object CommandBarGradientMiddleKey
    {
      get
      {
        return (object) "VsColor.CommandBarGradientMiddle";
      }
    }

    public static object CommandBarHoverKey
    {
      get
      {
        return (object) "VsColor.CommandBarHover";
      }
    }

    public static object CommandBarHoverOverSelectedKey
    {
      get
      {
        return (object) "VsColor.CommandBarHoverOverSelected";
      }
    }

    public static object CommandBarHoverOverSelectedIconKey
    {
      get
      {
        return (object) "VsColor.CommandBarHoverOverSelectedIcon";
      }
    }

    public static object CommandBarHoverOverSelectedIconBorderKey
    {
      get
      {
        return (object) "VsColor.CommandBarHoverOverSelectedIconBorder";
      }
    }

    public static object CommandBarMenuBackgroundGradientBeginKey
    {
      get
      {
        return (object) "VsColor.CommandBarMenuBackgroundGradientBegin";
      }
    }

    public static object CommandBarMenuBackgroundGradientEndKey
    {
      get
      {
        return (object) "VsColor.CommandBarMenuBackgroundGradientEnd";
      }
    }

    public static object CommandBarMenuBorderKey
    {
      get
      {
        return (object) "VsColor.CommandBarMenuBorder";
      }
    }

    public static object CommandBarMenuIconBackgroundKey
    {
      get
      {
        return (object) "VsColor.CommandBarMenuIconBackground";
      }
    }

    public static object CommandBarMenuMouseOverSubmenuGlyphKey
    {
      get
      {
        return (object) "VsColor.CommandBarMenuMouseOverSubmenuGlyph";
      }
    }

    public static object CommandBarMenuSeparatorKey
    {
      get
      {
        return (object) "VsColor.CommandBarMenuSeparator";
      }
    }

    public static object CommandBarMenuSubmenuGlyphKey
    {
      get
      {
        return (object) "VsColor.CommandBarMenuSubmenuGlyph";
      }
    }

    public static object CommandBarMouseDownBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.CommandBarMouseDownBackgroundBegin";
      }
    }

    public static object CommandBarMouseDownBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.CommandBarMouseDownBackgroundEnd";
      }
    }

    public static object CommandBarMouseDownBackgroundMiddleKey
    {
      get
      {
        return (object) "VsColor.CommandBarMouseDownBackgroundMiddle";
      }
    }

    public static object CommandBarMouseDownBorderKey
    {
      get
      {
        return (object) "VsColor.CommandBarMouseDownBorder";
      }
    }

    public static object CommandBarMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.CommandBarMouseOverBackgroundBegin";
      }
    }

    public static object CommandBarMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.CommandBarMouseOverBackgroundEnd";
      }
    }

    public static object CommandBarMouseOverBackgroundMiddle1Key
    {
      get
      {
        return (object) "VsColor.CommandBarMouseOverBackgroundMiddle1";
      }
    }

    public static object CommandBarMouseOverBackgroundMiddle2Key
    {
      get
      {
        return (object) "VsColor.CommandBarMouseOverBackgroundMiddle2";
      }
    }

    public static object CommandBarOptionsBackgroundKey
    {
      get
      {
        return (object) "VsColor.CommandBarOptionsBackground";
      }
    }

    public static object CommandBarOptionsGlyphKey
    {
      get
      {
        return (object) "VsColor.CommandBarOptionsGlyph";
      }
    }

    public static object CommandBarOptionsMouseDownBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.CommandBarOptionsMouseDownBackgroundBegin";
      }
    }

    public static object CommandBarOptionsMouseDownBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.CommandBarOptionsMouseDownBackgroundEnd";
      }
    }

    public static object CommandBarOptionsMouseDownBackgroundMiddleKey
    {
      get
      {
        return (object) "VsColor.CommandBarOptionsMouseDownBackgroundMiddle";
      }
    }

    public static object CommandBarOptionsMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.CommandBarOptionsMouseOverBackgroundBegin";
      }
    }

    public static object CommandBarOptionsMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.CommandBarOptionsMouseOverBackgroundEnd";
      }
    }

    public static object CommandBarOptionsMouseOverBackgroundMiddle1Key
    {
      get
      {
        return (object) "VsColor.CommandBarOptionsMouseOverBackgroundMiddle1";
      }
    }

    public static object CommandBarOptionsMouseOverBackgroundMiddle2Key
    {
      get
      {
        return (object) "VsColor.CommandBarOptionsMouseOverBackgroundMiddle2";
      }
    }

    public static object CommandBarOptionsMouseOverGlyphKey
    {
      get
      {
        return (object) "VsColor.CommandBarOptionsMouseOverGlyph";
      }
    }

    public static object CommandBarSelectedKey
    {
      get
      {
        return (object) "VsColor.CommandBarSelected";
      }
    }

    public static object CommandBarSelectedBorderKey
    {
      get
      {
        return (object) "VsColor.CommandBarSelectedBorder";
      }
    }

    public static object CommandBarShadowKey
    {
      get
      {
        return (object) "VsColor.CommandBarShadow";
      }
    }

    public static object CommandBarTextActiveKey
    {
      get
      {
        return (object) "VsColor.CommandBarTextActive";
      }
    }

    public static object CommandBarTextHoverKey
    {
      get
      {
        return (object) "VsColor.CommandBarTextHover";
      }
    }

    public static object CommandBarTextInactiveKey
    {
      get
      {
        return (object) "VsColor.CommandBarTextInactive";
      }
    }

    public static object CommandBarTextSelectedKey
    {
      get
      {
        return (object) "VsColor.CommandBarTextSelected";
      }
    }

    public static object CommandBarToolBarBorderKey
    {
      get
      {
        return (object) "VsColor.CommandBarToolBarBorder";
      }
    }

    public static object CommandBarToolBarSeparatorKey
    {
      get
      {
        return (object) "VsColor.CommandBarToolBarSeparator";
      }
    }

    public static object CommandShelfBackgroundGradientBeginKey
    {
      get
      {
        return (object) "VsColor.CommandShelfBackgroundGradientBegin";
      }
    }

    public static object CommandShelfBackgroundGradientEndKey
    {
      get
      {
        return (object) "VsColor.CommandShelfBackgroundGradientEnd";
      }
    }

    public static object CommandShelfBackgroundGradientMiddleKey
    {
      get
      {
        return (object) "VsColor.CommandShelfBackgroundGradientMiddle";
      }
    }

    public static object CommandShelfHighlightGradientBeginKey
    {
      get
      {
        return (object) "VsColor.CommandShelfHighlightGradientBegin";
      }
    }

    public static object CommandShelfHighlightGradientEndKey
    {
      get
      {
        return (object) "VsColor.CommandShelfHighlightGradientEnd";
      }
    }

    public static object CommandShelfHighlightGradientMiddleKey
    {
      get
      {
        return (object) "VsColor.CommandShelfHighlightGradientMiddle";
      }
    }

    public static object ControlEditHintTextKey
    {
      get
      {
        return (object) "VsColor.ControlEditHintText";
      }
    }

    public static object ControlEditRequiredBackgroundKey
    {
      get
      {
        return (object) "VsColor.ControlEditRequiredBackground";
      }
    }

    public static object ControlEditRequiredHintTextKey
    {
      get
      {
        return (object) "VsColor.ControlEditRequiredHintText";
      }
    }

    public static object ControlLinkTextKey
    {
      get
      {
        return (object) "VsColor.ControlLinkText";
      }
    }

    public static object ControlLinkTextHoverKey
    {
      get
      {
        return (object) "VsColor.ControlLinkTextHover";
      }
    }

    public static object ControlLinkTextPressedKey
    {
      get
      {
        return (object) "VsColor.ControlLinkTextPressed";
      }
    }

    public static object ControlOutlineKey
    {
      get
      {
        return (object) "VsColor.ControlOutline";
      }
    }

    public static object DebuggerDataTipActiveBackgroundKey
    {
      get
      {
        return (object) "VsColor.DebuggerDataTipActiveBackground";
      }
    }

    public static object DebuggerDataTipActiveBorderKey
    {
      get
      {
        return (object) "VsColor.DebuggerDataTipActiveBorder";
      }
    }

    public static object DebuggerDataTipActiveHighlightKey
    {
      get
      {
        return (object) "VsColor.DebuggerDataTipActiveHighlight";
      }
    }

    public static object DebuggerDataTipActiveHighlightTextKey
    {
      get
      {
        return (object) "VsColor.DebuggerDataTipActiveHighlightText";
      }
    }

    public static object DebuggerDataTipActiveSeparatorKey
    {
      get
      {
        return (object) "VsColor.DebuggerDataTipActiveSeparator";
      }
    }

    public static object DebuggerDataTipActiveTextKey
    {
      get
      {
        return (object) "VsColor.DebuggerDataTipActiveText";
      }
    }

    public static object DebuggerDataTipInactiveBackgroundKey
    {
      get
      {
        return (object) "VsColor.DebuggerDataTipInactiveBackground";
      }
    }

    public static object DebuggerDataTipInactiveBorderKey
    {
      get
      {
        return (object) "VsColor.DebuggerDataTipInactiveBorder";
      }
    }

    public static object DebuggerDataTipInactiveHighlightKey
    {
      get
      {
        return (object) "VsColor.DebuggerDataTipInactiveHighlight";
      }
    }

    public static object DebuggerDataTipInactiveHighlightTextKey
    {
      get
      {
        return (object) "VsColor.DebuggerDataTipInactiveHighlightText";
      }
    }

    public static object DebuggerDataTipInactiveSeparatorKey
    {
      get
      {
        return (object) "VsColor.DebuggerDataTipInactiveSeparator";
      }
    }

    public static object DebuggerDataTipInactiveTextKey
    {
      get
      {
        return (object) "VsColor.DebuggerDataTipInactiveText";
      }
    }

    public static object DesignerBackgroundKey
    {
      get
      {
        return (object) "VsColor.DesignerBackground";
      }
    }

    public static object DesignerSelectionDotsKey
    {
      get
      {
        return (object) "VsColor.DesignerSelectionDots";
      }
    }

    public static object DesignerTrayKey
    {
      get
      {
        return (object) "VsColor.DesignerTray";
      }
    }

    public static object DesignerWatermarkKey
    {
      get
      {
        return (object) "VsColor.DesignerWatermark";
      }
    }

    public static object DiagReportBackgroundKey
    {
      get
      {
        return (object) "VsColor.DiagReportBackground";
      }
    }

    public static object DiagReportSecondaryPageHeaderKey
    {
      get
      {
        return (object) "VsColor.DiagReportSecondaryPageHeader";
      }
    }

    public static object DiagReportSecondaryPageSubtitleKey
    {
      get
      {
        return (object) "VsColor.DiagReportSecondaryPageSubtitle";
      }
    }

    public static object DiagReportSecondaryPageTitleKey
    {
      get
      {
        return (object) "VsColor.DiagReportSecondaryPageTitle";
      }
    }

    public static object DiagReportSummaryPageHeaderKey
    {
      get
      {
        return (object) "VsColor.DiagReportSummaryPageHeader";
      }
    }

    public static object DiagReportSummaryPageSubtitleKey
    {
      get
      {
        return (object) "VsColor.DiagReportSummaryPageSubtitle";
      }
    }

    public static object DiagReportSummaryPageTitleKey
    {
      get
      {
        return (object) "VsColor.DiagReportSummaryPageTitle";
      }
    }

    public static object DiagReportTextKey
    {
      get
      {
        return (object) "VsColor.DiagReportText";
      }
    }

    public static object DockTargetBackgroundKey
    {
      get
      {
        return (object) "VsColor.DockTargetBackground";
      }
    }

    public static object DockTargetBorderKey
    {
      get
      {
        return (object) "VsColor.DockTargetBorder";
      }
    }

    public static object DockTargetButtonBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.DockTargetButtonBackgroundBegin";
      }
    }

    public static object DockTargetButtonBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.DockTargetButtonBackgroundEnd";
      }
    }

    public static object DockTargetButtonBorderKey
    {
      get
      {
        return (object) "VsColor.DockTargetButtonBorder";
      }
    }

    public static object DockTargetGlyphArrowKey
    {
      get
      {
        return (object) "VsColor.DockTargetGlyphArrow";
      }
    }

    public static object DockTargetGlyphBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.DockTargetGlyphBackgroundBegin";
      }
    }

    public static object DockTargetGlyphBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.DockTargetGlyphBackgroundEnd";
      }
    }

    public static object DockTargetGlyphBorderKey
    {
      get
      {
        return (object) "VsColor.DockTargetGlyphBorder";
      }
    }

    public static object DropDownBackgroundKey
    {
      get
      {
        return (object) "VsColor.DropDownBackground";
      }
    }

    public static object DropDownBorderKey
    {
      get
      {
        return (object) "VsColor.DropDownBorder";
      }
    }

    public static object DropDownDisabledBackgroundKey
    {
      get
      {
        return (object) "VsColor.DropDownDisabledBackground";
      }
    }

    public static object DropDownDisabledBorderKey
    {
      get
      {
        return (object) "VsColor.DropDownDisabledBorder";
      }
    }

    public static object DropDownDisabledGlyphKey
    {
      get
      {
        return (object) "VsColor.DropDownDisabledGlyph";
      }
    }

    public static object DropDownGlyphKey
    {
      get
      {
        return (object) "VsColor.DropDownGlyph";
      }
    }

    public static object DropDownMouseDownBackgroundKey
    {
      get
      {
        return (object) "VsColor.DropDownMouseDownBackground";
      }
    }

    public static object DropDownMouseDownBorderKey
    {
      get
      {
        return (object) "VsColor.DropDownMouseDownBorder";
      }
    }

    public static object DropDownMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.DropDownMouseOverBackgroundBegin";
      }
    }

    public static object DropDownMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.DropDownMouseOverBackgroundEnd";
      }
    }

    public static object DropDownMouseOverBackgroundMiddle1Key
    {
      get
      {
        return (object) "VsColor.DropDownMouseOverBackgroundMiddle1";
      }
    }

    public static object DropDownMouseOverBackgroundMiddle2Key
    {
      get
      {
        return (object) "VsColor.DropDownMouseOverBackgroundMiddle2";
      }
    }

    public static object DropDownMouseOverBorderKey
    {
      get
      {
        return (object) "VsColor.DropDownMouseOverBorder";
      }
    }

    public static object DropDownMouseOverGlyphKey
    {
      get
      {
        return (object) "VsColor.DropDownMouseOverGlyph";
      }
    }

    public static object DropDownPopupBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.DropDownPopupBackgroundBegin";
      }
    }

    public static object DropDownPopupBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.DropDownPopupBackgroundEnd";
      }
    }

    public static object DropDownPopupBorderKey
    {
      get
      {
        return (object) "VsColor.DropDownPopupBorder";
      }
    }

    public static object DropShadowBackgroundKey
    {
      get
      {
        return (object) "VsColor.DropShadowBackground";
      }
    }

    public static object EditorExpansionBorderKey
    {
      get
      {
        return (object) "VsColor.EditorExpansionBorder";
      }
    }

    public static object EditorExpansionFillKey
    {
      get
      {
        return (object) "VsColor.EditorExpansionFill";
      }
    }

    public static object EditorExpansionLinkKey
    {
      get
      {
        return (object) "VsColor.EditorExpansionLink";
      }
    }

    public static object EditorExpansionTextKey
    {
      get
      {
        return (object) "VsColor.EditorExpansionText";
      }
    }

    public static object EnvironmentBackgroundKey
    {
      get
      {
        return (object) "VsColor.EnvironmentBackground";
      }
    }

    public static object EnvironmentBackgroundGradientBeginKey
    {
      get
      {
        return (object) "VsColor.EnvironmentBackgroundGradientBegin";
      }
    }

    public static object EnvironmentBackgroundGradientEndKey
    {
      get
      {
        return (object) "VsColor.EnvironmentBackgroundGradientEnd";
      }
    }

    public static object EnvironmentBackgroundGradientMiddle1Key
    {
      get
      {
        return (object) "VsColor.EnvironmentBackgroundGradientMiddle1";
      }
    }

    public static object EnvironmentBackgroundGradientMiddle2Key
    {
      get
      {
        return (object) "VsColor.EnvironmentBackgroundGradientMiddle2";
      }
    }

    public static object EnvironmentBackgroundTexture1Key
    {
      get
      {
        return (object) "VsColor.EnvironmentBackgroundTexture1";
      }
    }

    public static object EnvironmentBackgroundTexture2Key
    {
      get
      {
        return (object) "VsColor.EnvironmentBackgroundTexture2";
      }
    }

    public static object ExtensionManagerStarHighlight1Key
    {
      get
      {
        return (object) "VsColor.ExtensionManagerStarHighlight1";
      }
    }

    public static object ExtensionManagerStarHighlight2Key
    {
      get
      {
        return (object) "VsColor.ExtensionManagerStarHighlight2";
      }
    }

    public static object ExtensionManagerStarInactive1Key
    {
      get
      {
        return (object) "VsColor.ExtensionManagerStarInactive1";
      }
    }

    public static object ExtensionManagerStarInactive2Key
    {
      get
      {
        return (object) "VsColor.ExtensionManagerStarInactive2";
      }
    }

    public static object FileTabBorderKey
    {
      get
      {
        return (object) "VsColor.FileTabBorder";
      }
    }

    public static object FileTabChannelBackgroundKey
    {
      get
      {
        return (object) "VsColor.FileTabChannelBackground";
      }
    }

    public static object FileTabDocumentBorderBackgroundKey
    {
      get
      {
        return (object) "VsColor.FileTabDocumentBorderBackground";
      }
    }

    public static object FileTabDocumentBorderHighlightKey
    {
      get
      {
        return (object) "VsColor.FileTabDocumentBorderHighlight";
      }
    }

    public static object FileTabDocumentBorderShadowKey
    {
      get
      {
        return (object) "VsColor.FileTabDocumentBorderShadow";
      }
    }

    public static object FileTabGradientDarkKey
    {
      get
      {
        return (object) "VsColor.FileTabGradientDark";
      }
    }

    public static object FileTabGradientLightKey
    {
      get
      {
        return (object) "VsColor.FileTabGradientLight";
      }
    }

    public static object FileTabHotBorderKey
    {
      get
      {
        return (object) "VsColor.FileTabHotBorder";
      }
    }

    public static object FileTabHotGlyphKey
    {
      get
      {
        return (object) "VsColor.FileTabHotGlyph";
      }
    }

    public static object FileTabHotGradientBottomKey
    {
      get
      {
        return (object) "VsColor.FileTabHotGradientBottom";
      }
    }

    public static object FileTabHotGradientTopKey
    {
      get
      {
        return (object) "VsColor.FileTabHotGradientTop";
      }
    }

    public static object FileTabHotTextKey
    {
      get
      {
        return (object) "VsColor.FileTabHotText";
      }
    }

    public static object FileTabInactiveDocumentBorderBackgroundKey
    {
      get
      {
        return (object) "VsColor.FileTabInactiveDocumentBorderBackground";
      }
    }

    public static object FileTabInactiveDocumentBorderEdgeKey
    {
      get
      {
        return (object) "VsColor.FileTabInactiveDocumentBorderEdge";
      }
    }

    public static object FileTabInactiveGradientBottomKey
    {
      get
      {
        return (object) "VsColor.FileTabInactiveGradientBottom";
      }
    }

    public static object FileTabInactiveGradientTopKey
    {
      get
      {
        return (object) "VsColor.FileTabInactiveGradientTop";
      }
    }

    public static object FileTabInactiveTextKey
    {
      get
      {
        return (object) "VsColor.FileTabInactiveText";
      }
    }

    public static object FileTabLastActiveDocumentBorderBackgroundKey
    {
      get
      {
        return (object) "VsColor.FileTabLastActiveDocumentBorderBackground";
      }
    }

    public static object FileTabLastActiveDocumentBorderEdgeKey
    {
      get
      {
        return (object) "VsColor.FileTabLastActiveDocumentBorderEdge";
      }
    }

    public static object FileTabLastActiveGlyphKey
    {
      get
      {
        return (object) "VsColor.FileTabLastActiveGlyph";
      }
    }

    public static object FileTabLastActiveGradientBottomKey
    {
      get
      {
        return (object) "VsColor.FileTabLastActiveGradientBottom";
      }
    }

    public static object FileTabLastActiveGradientMiddle1Key
    {
      get
      {
        return (object) "VsColor.FileTabLastActiveGradientMiddle1";
      }
    }

    public static object FileTabLastActiveGradientMiddle2Key
    {
      get
      {
        return (object) "VsColor.FileTabLastActiveGradientMiddle2";
      }
    }

    public static object FileTabLastActiveGradientTopKey
    {
      get
      {
        return (object) "VsColor.FileTabLastActiveGradientTop";
      }
    }

    public static object FileTabLastActiveTextKey
    {
      get
      {
        return (object) "VsColor.FileTabLastActiveText";
      }
    }

    public static object FileTabSelectedBackgroundKey
    {
      get
      {
        return (object) "VsColor.FileTabSelectedBackground";
      }
    }

    public static object FileTabSelectedBorderKey
    {
      get
      {
        return (object) "VsColor.FileTabSelectedBorder";
      }
    }

    public static object FileTabSelectedGradientBottomKey
    {
      get
      {
        return (object) "VsColor.FileTabSelectedGradientBottom";
      }
    }

    public static object FileTabSelectedGradientMiddle1Key
    {
      get
      {
        return (object) "VsColor.FileTabSelectedGradientMiddle1";
      }
    }

    public static object FileTabSelectedGradientMiddle2Key
    {
      get
      {
        return (object) "VsColor.FileTabSelectedGradientMiddle2";
      }
    }

    public static object FileTabSelectedGradientTopKey
    {
      get
      {
        return (object) "VsColor.FileTabSelectedGradientTop";
      }
    }

    public static object FileTabSelectedTextKey
    {
      get
      {
        return (object) "VsColor.FileTabSelectedText";
      }
    }

    public static object FileTabTextKey
    {
      get
      {
        return (object) "VsColor.FileTabText";
      }
    }

    public static object FormSmartTagActionTagBorderKey
    {
      get
      {
        return (object) "VsColor.FormSmartTagActionTagBorder";
      }
    }

    public static object FormSmartTagActionTagFillKey
    {
      get
      {
        return (object) "VsColor.FormSmartTagActionTagFill";
      }
    }

    public static object FormSmartTagObjectTagBorderKey
    {
      get
      {
        return (object) "VsColor.FormSmartTagObjectTagBorder";
      }
    }

    public static object FormSmartTagObjectTagFillKey
    {
      get
      {
        return (object) "VsColor.FormSmartTagObjectTagFill";
      }
    }

    public static object GrayTextKey
    {
      get
      {
        return (object) "VsColor.GrayText";
      }
    }

    public static object GridHeadingBackgroundKey
    {
      get
      {
        return (object) "VsColor.GridHeadingBackground";
      }
    }

    public static object GridHeadingTextKey
    {
      get
      {
        return (object) "VsColor.GridHeadingText";
      }
    }

    public static object GridLineKey
    {
      get
      {
        return (object) "VsColor.GridLine";
      }
    }

    public static object HelpHowDoIPaneBackgroundKey
    {
      get
      {
        return (object) "VsColor.HelpHowDoIPaneBackground";
      }
    }

    public static object HelpHowDoIPaneLinkKey
    {
      get
      {
        return (object) "VsColor.HelpHowDoIPaneLink";
      }
    }

    public static object HelpHowDoIPaneTextKey
    {
      get
      {
        return (object) "VsColor.HelpHowDoIPaneText";
      }
    }

    public static object HelpHowDoITaskBackgroundKey
    {
      get
      {
        return (object) "VsColor.HelpHowDoITaskBackground";
      }
    }

    public static object HelpHowDoITaskLinkKey
    {
      get
      {
        return (object) "VsColor.HelpHowDoITaskLink";
      }
    }

    public static object HelpHowDoITaskTextKey
    {
      get
      {
        return (object) "VsColor.HelpHowDoITaskText";
      }
    }

    public static object HelpSearchBackgroundKey
    {
      get
      {
        return (object) "VsColor.HelpSearchBackground";
      }
    }

    public static object HelpSearchBorderKey
    {
      get
      {
        return (object) "VsColor.HelpSearchBorder";
      }
    }

    public static object HelpSearchFilterBackgroundKey
    {
      get
      {
        return (object) "VsColor.HelpSearchFilterBackground";
      }
    }

    public static object HelpSearchFilterBorderKey
    {
      get
      {
        return (object) "VsColor.HelpSearchFilterBorder";
      }
    }

    public static object HelpSearchFilterTextKey
    {
      get
      {
        return (object) "VsColor.HelpSearchFilterText";
      }
    }

    public static object HelpSearchFrameBackgroundKey
    {
      get
      {
        return (object) "VsColor.HelpSearchFrameBackground";
      }
    }

    public static object HelpSearchFrameTextKey
    {
      get
      {
        return (object) "VsColor.HelpSearchFrameText";
      }
    }

    public static object HelpSearchPanelRulesKey
    {
      get
      {
        return (object) "VsColor.HelpSearchPanelRules";
      }
    }

    public static object HelpSearchProviderIconKey
    {
      get
      {
        return (object) "VsColor.HelpSearchProviderIcon";
      }
    }

    public static object HelpSearchProviderSelectedBackgroundKey
    {
      get
      {
        return (object) "VsColor.HelpSearchProviderSelectedBackground";
      }
    }

    public static object HelpSearchProviderSelectedTextKey
    {
      get
      {
        return (object) "VsColor.HelpSearchProviderSelectedText";
      }
    }

    public static object HelpSearchProviderUnselectedBackgroundKey
    {
      get
      {
        return (object) "VsColor.HelpSearchProviderUnselectedBackground";
      }
    }

    public static object HelpSearchProviderUnselectedTextKey
    {
      get
      {
        return (object) "VsColor.HelpSearchProviderUnselectedText";
      }
    }

    public static object HelpSearchResultLinkSelectedKey
    {
      get
      {
        return (object) "VsColor.HelpSearchResultLinkSelected";
      }
    }

    public static object HelpSearchResultLinkUnselectedKey
    {
      get
      {
        return (object) "VsColor.HelpSearchResultLinkUnselected";
      }
    }

    public static object HelpSearchResultSelectedBackgroundKey
    {
      get
      {
        return (object) "VsColor.HelpSearchResultSelectedBackground";
      }
    }

    public static object HelpSearchResultSelectedTextKey
    {
      get
      {
        return (object) "VsColor.HelpSearchResultSelectedText";
      }
    }

    public static object HelpSearchTextKey
    {
      get
      {
        return (object) "VsColor.HelpSearchText";
      }
    }

    public static object HighlightKey
    {
      get
      {
        return (object) "VsColor.Highlight";
      }
    }

    public static object HighlightTextKey
    {
      get
      {
        return (object) "VsColor.HighlightText";
      }
    }

    public static object InactiveBorderKey
    {
      get
      {
        return (object) "VsColor.InactiveBorder";
      }
    }

    public static object InactiveCaptionKey
    {
      get
      {
        return (object) "VsColor.InactiveCaption";
      }
    }

    public static object InactiveCaptionTextKey
    {
      get
      {
        return (object) "VsColor.InactiveCaptionText";
      }
    }

    public static object InfoBackgroundKey
    {
      get
      {
        return (object) "VsColor.InfoBackground";
      }
    }

    public static object InfoTextKey
    {
      get
      {
        return (object) "VsColor.InfoText";
      }
    }

    public static object MdiClientBorderKey
    {
      get
      {
        return (object) "VsColor.MdiClientBorder";
      }
    }

    public static object MenuKey
    {
      get
      {
        return (object) "VsColor.Menu";
      }
    }

    public static object MenuTextKey
    {
      get
      {
        return (object) "VsColor.MenuText";
      }
    }

    public static object NewProjectBackgroundKey
    {
      get
      {
        return (object) "VsColor.NewProjectBackground";
      }
    }

    public static object NewProjectItemInactiveBeginKey
    {
      get
      {
        return (object) "VsColor.NewProjectItemInactiveBegin";
      }
    }

    public static object NewProjectItemInactiveBorderKey
    {
      get
      {
        return (object) "VsColor.NewProjectItemInactiveBorder";
      }
    }

    public static object NewProjectItemInactiveEndKey
    {
      get
      {
        return (object) "VsColor.NewProjectItemInactiveEnd";
      }
    }

    public static object NewProjectItemSelectedKey
    {
      get
      {
        return (object) "VsColor.NewProjectItemSelected";
      }
    }

    public static object NewProjectItemSelectedBorderKey
    {
      get
      {
        return (object) "VsColor.NewProjectItemSelectedBorder";
      }
    }

    public static object NewProjectProviderHoverBeginKey
    {
      get
      {
        return (object) "VsColor.NewProjectProviderHoverBegin";
      }
    }

    public static object NewProjectProviderHoverEndKey
    {
      get
      {
        return (object) "VsColor.NewProjectProviderHoverEnd";
      }
    }

    public static object NewProjectProviderHoverForegroundKey
    {
      get
      {
        return (object) "VsColor.NewProjectProviderHoverForeground";
      }
    }

    public static object NewProjectProviderHoverMiddle1Key
    {
      get
      {
        return (object) "VsColor.NewProjectProviderHoverMiddle1";
      }
    }

    public static object NewProjectProviderHoverMiddle2Key
    {
      get
      {
        return (object) "VsColor.NewProjectProviderHoverMiddle2";
      }
    }

    public static object NewProjectProviderInactiveBeginKey
    {
      get
      {
        return (object) "VsColor.NewProjectProviderInactiveBegin";
      }
    }

    public static object NewProjectProviderInactiveEndKey
    {
      get
      {
        return (object) "VsColor.NewProjectProviderInactiveEnd";
      }
    }

    public static object NewProjectProviderInactiveForegroundKey
    {
      get
      {
        return (object) "VsColor.NewProjectProviderInactiveForeground";
      }
    }

    public static object PageContentExpanderChevronKey
    {
      get
      {
        return (object) "VsColor.PageContentExpanderChevron";
      }
    }

    public static object PageContentExpanderSeparatorKey
    {
      get
      {
        return (object) "VsColor.PageContentExpanderSeparator";
      }
    }

    public static object PageSideBarExpanderBodyKey
    {
      get
      {
        return (object) "VsColor.PageSideBarExpanderBody";
      }
    }

    public static object PageSideBarExpanderChevronKey
    {
      get
      {
        return (object) "VsColor.PageSideBarExpanderChevron";
      }
    }

    public static object PageSideBarExpanderHeaderKey
    {
      get
      {
        return (object) "VsColor.PageSideBarExpanderHeader";
      }
    }

    public static object PageSideBarExpanderHeaderHoverKey
    {
      get
      {
        return (object) "VsColor.PageSideBarExpanderHeaderHover";
      }
    }

    public static object PageSideBarExpanderHeaderPressedKey
    {
      get
      {
        return (object) "VsColor.PageSideBarExpanderHeaderPressed";
      }
    }

    public static object PageSideBarExpanderSeparatorKey
    {
      get
      {
        return (object) "VsColor.PageSideBarExpanderSeparator";
      }
    }

    public static object PageSideBarExpanderTextKey
    {
      get
      {
        return (object) "VsColor.PageSideBarExpanderText";
      }
    }

    public static object PanelBorderKey
    {
      get
      {
        return (object) "VsColor.PanelBorder";
      }
    }

    public static object PanelGradientDarkKey
    {
      get
      {
        return (object) "VsColor.PanelGradientDark";
      }
    }

    public static object PanelGradientLightKey
    {
      get
      {
        return (object) "VsColor.PanelGradientLight";
      }
    }

    public static object PanelHoverOverCloseBorderKey
    {
      get
      {
        return (object) "VsColor.PanelHoverOverCloseBorder";
      }
    }

    public static object PanelHoverOverCloseFillKey
    {
      get
      {
        return (object) "VsColor.PanelHoverOverCloseFill";
      }
    }

    public static object PanelHyperlinkKey
    {
      get
      {
        return (object) "VsColor.PanelHyperlink";
      }
    }

    public static object PanelHyperlinkHoverKey
    {
      get
      {
        return (object) "VsColor.PanelHyperlinkHover";
      }
    }

    public static object PanelHyperlinkPressedKey
    {
      get
      {
        return (object) "VsColor.PanelHyperlinkPressed";
      }
    }

    public static object PanelSeparatorKey
    {
      get
      {
        return (object) "VsColor.PanelSeparator";
      }
    }

    public static object PanelSubGroupSeparatorKey
    {
      get
      {
        return (object) "VsColor.PanelSubGroupSeparator";
      }
    }

    public static object PanelTextKey
    {
      get
      {
        return (object) "VsColor.PanelText";
      }
    }

    public static object PanelTitleBarKey
    {
      get
      {
        return (object) "VsColor.PanelTitleBar";
      }
    }

    public static object PanelTitleBarTextKey
    {
      get
      {
        return (object) "VsColor.PanelTitleBarText";
      }
    }

    public static object PanelTitleBarUnselectedKey
    {
      get
      {
        return (object) "VsColor.PanelTitleBarUnselected";
      }
    }

    public static object ProjectDesignerBackgroundGradientBeginKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerBackgroundGradientBegin";
      }
    }

    public static object ProjectDesignerBackgroundGradientEndKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerBackgroundGradientEnd";
      }
    }

    public static object ProjectDesignerBorderInsideKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerBorderInside";
      }
    }

    public static object ProjectDesignerBorderOutsideKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerBorderOutside";
      }
    }

    public static object ProjectDesignerContentsBackgroundKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerContentsBackground";
      }
    }

    public static object ProjectDesignerTabBackgroundGradientBeginKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerTabBackgroundGradientBegin";
      }
    }

    public static object ProjectDesignerTabBackgroundGradientEndKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerTabBackgroundGradientEnd";
      }
    }

    public static object ProjectDesignerTabSelectedBackgroundKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerTabSelectedBackground";
      }
    }

    public static object ProjectDesignerTabSelectedBorderKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerTabSelectedBorder";
      }
    }

    public static object ProjectDesignerTabSelectedHighlight1Key
    {
      get
      {
        return (object) "VsColor.ProjectDesignerTabSelectedHighlight1";
      }
    }

    public static object ProjectDesignerTabSelectedHighlight2Key
    {
      get
      {
        return (object) "VsColor.ProjectDesignerTabSelectedHighlight2";
      }
    }

    public static object ProjectDesignerTabSelectedInsideBorderKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerTabSelectedInsideBorder";
      }
    }

    public static object ProjectDesignerTabSepBottomGradientBeginKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerTabSepBottomGradientBegin";
      }
    }

    public static object ProjectDesignerTabSepBottomGradientEndKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerTabSepBottomGradientEnd";
      }
    }

    public static object ProjectDesignerTabSepTopGradientBeginKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerTabSepTopGradientBegin";
      }
    }

    public static object ProjectDesignerTabSepTopGradientEndKey
    {
      get
      {
        return (object) "VsColor.ProjectDesignerTabSepTopGradientEnd";
      }
    }

    public static object ScreenTipBackgroundKey
    {
      get
      {
        return (object) "VsColor.ScreenTipBackground";
      }
    }

    public static object ScreenTipBorderKey
    {
      get
      {
        return (object) "VsColor.ScreenTipBorder";
      }
    }

    public static object ScreenTipTextKey
    {
      get
      {
        return (object) "VsColor.ScreenTipText";
      }
    }

    public static object ScrollBarKey
    {
      get
      {
        return (object) "VsColor.ScrollBar";
      }
    }

    public static object ScrollBarArrowBackgroundKey
    {
      get
      {
        return (object) "VsColor.ScrollBarArrowBackground";
      }
    }

    public static object ScrollBarArrowDisabledBackgroundKey
    {
      get
      {
        return (object) "VsColor.ScrollBarArrowDisabledBackground";
      }
    }

    public static object ScrollBarArrowMouseOverBackgroundKey
    {
      get
      {
        return (object) "VsColor.ScrollBarArrowMouseOverBackground";
      }
    }

    public static object ScrollBarArrowPressedBackgroundKey
    {
      get
      {
        return (object) "VsColor.ScrollBarArrowPressedBackground";
      }
    }

    public static object ScrollBarBackgroundKey
    {
      get
      {
        return (object) "VsColor.ScrollBarBackground";
      }
    }

    public static object ScrollBarDisabledBackgroundKey
    {
      get
      {
        return (object) "VsColor.ScrollBarDisabledBackground";
      }
    }

    public static object ScrollBarThumbBackgroundKey
    {
      get
      {
        return (object) "VsColor.ScrollBarThumbBackground";
      }
    }

    public static object ScrollBarThumbBorderKey
    {
      get
      {
        return (object) "VsColor.ScrollBarThumbBorder";
      }
    }

    public static object ScrollBarThumbGlyphKey
    {
      get
      {
        return (object) "VsColor.ScrollBarThumbGlyph";
      }
    }

    public static object ScrollBarThumbMouseOverBackgroundKey
    {
      get
      {
        return (object) "VsColor.ScrollBarThumbMouseOverBackground";
      }
    }

    public static object ScrollBarThumbPressedBackgroundKey
    {
      get
      {
        return (object) "VsColor.ScrollBarThumbPressedBackground";
      }
    }

    public static object SearchBoxBackgroundKey
    {
      get
      {
        return (object) "VsColor.SearchBoxBackground";
      }
    }

    public static object SearchBoxBorderKey
    {
      get
      {
        return (object) "VsColor.SearchBoxBorder";
      }
    }

    public static object SearchBoxMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.SearchBoxMouseOverBackgroundBegin";
      }
    }

    public static object SearchBoxMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.SearchBoxMouseOverBackgroundEnd";
      }
    }

    public static object SearchBoxMouseOverBackgroundMiddle1Key
    {
      get
      {
        return (object) "VsColor.SearchBoxMouseOverBackgroundMiddle1";
      }
    }

    public static object SearchBoxMouseOverBackgroundMiddle2Key
    {
      get
      {
        return (object) "VsColor.SearchBoxMouseOverBackgroundMiddle2";
      }
    }

    public static object SearchBoxMouseOverBorderKey
    {
      get
      {
        return (object) "VsColor.SearchBoxMouseOverBorder";
      }
    }

    public static object SearchBoxPressedBackgroundKey
    {
      get
      {
        return (object) "VsColor.SearchBoxPressedBackground";
      }
    }

    public static object SearchBoxPressedBorderKey
    {
      get
      {
        return (object) "VsColor.SearchBoxPressedBorder";
      }
    }

    public static object SideBarBackgroundKey
    {
      get
      {
        return (object) "VsColor.SideBarBackground";
      }
    }

    public static object SideBarGradientDarkKey
    {
      get
      {
        return (object) "VsColor.SideBarGradientDark";
      }
    }

    public static object SideBarGradientLightKey
    {
      get
      {
        return (object) "VsColor.SideBarGradientLight";
      }
    }

    public static object SideBarTextKey
    {
      get
      {
        return (object) "VsColor.SideBarText";
      }
    }

    public static object SmartTagBorderKey
    {
      get
      {
        return (object) "VsColor.SmartTagBorder";
      }
    }

    public static object SmartTagFillKey
    {
      get
      {
        return (object) "VsColor.SmartTagFill";
      }
    }

    public static object SmartTagHoverBorderKey
    {
      get
      {
        return (object) "VsColor.SmartTagHoverBorder";
      }
    }

    public static object SmartTagHoverFillKey
    {
      get
      {
        return (object) "VsColor.SmartTagHoverFill";
      }
    }

    public static object SmartTagHoverTextKey
    {
      get
      {
        return (object) "VsColor.SmartTagHoverText";
      }
    }

    public static object SmartTagTextKey
    {
      get
      {
        return (object) "VsColor.SmartTagText";
      }
    }

    public static object SnaplinesKey
    {
      get
      {
        return (object) "VsColor.Snaplines";
      }
    }

    public static object SnaplinesPaddingKey
    {
      get
      {
        return (object) "VsColor.SnaplinesPadding";
      }
    }

    public static object SnaplinesTextBaselineKey
    {
      get
      {
        return (object) "VsColor.SnaplinesTextBaseline";
      }
    }

    public static object SortBackgroundKey
    {
      get
      {
        return (object) "VsColor.SortBackground";
      }
    }

    public static object SortTextKey
    {
      get
      {
        return (object) "VsColor.SortText";
      }
    }

    public static object SplashScreenBorderKey
    {
      get
      {
        return (object) "VsColor.SplashScreenBorder";
      }
    }

    public static object StartPageBackgroundGradientBeginKey
    {
      get
      {
        return (object) "VsColor.StartPageBackgroundGradientBegin";
      }
    }

    public static object StartPageBackgroundGradientEndKey
    {
      get
      {
        return (object) "VsColor.StartPageBackgroundGradientEnd";
      }
    }

    public static object StartPageButtonBorderKey
    {
      get
      {
        return (object) "VsColor.StartPageButtonBorder";
      }
    }

    public static object StartPageButtonMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.StartPageButtonMouseOverBackgroundBegin";
      }
    }

    public static object StartPageButtonMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.StartPageButtonMouseOverBackgroundEnd";
      }
    }

    public static object StartPageButtonMouseOverBackgroundMiddle1Key
    {
      get
      {
        return (object) "VsColor.StartPageButtonMouseOverBackgroundMiddle1";
      }
    }

    public static object StartPageButtonMouseOverBackgroundMiddle2Key
    {
      get
      {
        return (object) "VsColor.StartPageButtonMouseOverBackgroundMiddle2";
      }
    }

    public static object StartPageButtonPinDownKey
    {
      get
      {
        return (object) "VsColor.StartPageButtonPinDown";
      }
    }

    public static object StartPageButtonPinHoverKey
    {
      get
      {
        return (object) "VsColor.StartPageButtonPinHover";
      }
    }

    public static object StartPageButtonPinnedKey
    {
      get
      {
        return (object) "VsColor.StartPageButtonPinned";
      }
    }

    public static object StartPageButtonTextKey
    {
      get
      {
        return (object) "VsColor.StartPageButtonText";
      }
    }

    public static object StartPageButtonTextHoverKey
    {
      get
      {
        return (object) "VsColor.StartPageButtonTextHover";
      }
    }

    public static object StartPageButtonUnpinnedKey
    {
      get
      {
        return (object) "VsColor.StartPageButtonUnpinned";
      }
    }

    public static object StartPageSelectedItemBackgroundKey
    {
      get
      {
        return (object) "VsColor.StartPageSelectedItemBackground";
      }
    }

    public static object StartPageSelectedItemStrokeKey
    {
      get
      {
        return (object) "VsColor.StartPageSelectedItemStroke";
      }
    }

    public static object StartPageSeparatorKey
    {
      get
      {
        return (object) "VsColor.StartPageSeparator";
      }
    }

    public static object StartPageTabBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.StartPageTabBackgroundBegin";
      }
    }

    public static object StartPageTabBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.StartPageTabBackgroundEnd";
      }
    }

    public static object StartPageTabMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.StartPageTabMouseOverBackgroundBegin";
      }
    }

    public static object StartPageTabMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.StartPageTabMouseOverBackgroundEnd";
      }
    }

    public static object StartPageTextBodyKey
    {
      get
      {
        return (object) "VsColor.StartPageTextBody";
      }
    }

    public static object StartPageTextBodySelectedKey
    {
      get
      {
        return (object) "VsColor.StartPageTextBodySelected";
      }
    }

    public static object StartPageTextBodyUnselectedKey
    {
      get
      {
        return (object) "VsColor.StartPageTextBodyUnselected";
      }
    }

    public static object StartPageTextControlLinkSelectedKey
    {
      get
      {
        return (object) "VsColor.StartPageTextControlLinkSelected";
      }
    }

    public static object StartPageTextControlLinkSelectedHoverKey
    {
      get
      {
        return (object) "VsColor.StartPageTextControlLinkSelectedHover";
      }
    }

    public static object StartPageTextDateKey
    {
      get
      {
        return (object) "VsColor.StartPageTextDate";
      }
    }

    public static object StartPageTextHeadingKey
    {
      get
      {
        return (object) "VsColor.StartPageTextHeading";
      }
    }

    public static object StartPageTextHeadingMouseOverKey
    {
      get
      {
        return (object) "VsColor.StartPageTextHeadingMouseOver";
      }
    }

    public static object StartPageTextHeadingSelectedKey
    {
      get
      {
        return (object) "VsColor.StartPageTextHeadingSelected";
      }
    }

    public static object StartPageTextSubHeadingKey
    {
      get
      {
        return (object) "VsColor.StartPageTextSubHeading";
      }
    }

    public static object StartPageTextSubHeadingMouseOverKey
    {
      get
      {
        return (object) "VsColor.StartPageTextSubHeadingMouseOver";
      }
    }

    public static object StartPageTextSubHeadingSelectedKey
    {
      get
      {
        return (object) "VsColor.StartPageTextSubHeadingSelected";
      }
    }

    public static object StartPageUnselectedItemBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.StartPageUnselectedItemBackgroundBegin";
      }
    }

    public static object StartPageUnselectedItemBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.StartPageUnselectedItemBackgroundEnd";
      }
    }

    public static object StartPageUnselectedItemStrokeKey
    {
      get
      {
        return (object) "VsColor.StartPageUnselectedItemStroke";
      }
    }

    public static object StatusBarTextKey
    {
      get
      {
        return (object) "VsColor.StatusBarText";
      }
    }

    public static object TaskListGridLinesKey
    {
      get
      {
        return (object) "VsColor.TaskListGridLines";
      }
    }

    public static object ThreeDDarkShadowKey
    {
      get
      {
        return (object) "VsColor.ThreeDDarkShadow";
      }
    }

    public static object ThreeDFaceKey
    {
      get
      {
        return (object) "VsColor.ThreeDFace";
      }
    }

    public static object ThreeDHighlightKey
    {
      get
      {
        return (object) "VsColor.ThreeDHighlight";
      }
    }

    public static object ThreeDLightShadowKey
    {
      get
      {
        return (object) "VsColor.ThreeDLightShadow";
      }
    }

    public static object ThreeDShadowKey
    {
      get
      {
        return (object) "VsColor.ThreeDShadow";
      }
    }

    public static object TitleBarActiveKey
    {
      get
      {
        return (object) "VsColor.TitleBarActive";
      }
    }

    public static object TitleBarActiveGradientBeginKey
    {
      get
      {
        return (object) "VsColor.TitleBarActiveGradientBegin";
      }
    }

    public static object TitleBarActiveGradientEndKey
    {
      get
      {
        return (object) "VsColor.TitleBarActiveGradientEnd";
      }
    }

    public static object TitleBarActiveGradientMiddle1Key
    {
      get
      {
        return (object) "VsColor.TitleBarActiveGradientMiddle1";
      }
    }

    public static object TitleBarActiveGradientMiddle2Key
    {
      get
      {
        return (object) "VsColor.TitleBarActiveGradientMiddle2";
      }
    }

    public static object TitleBarActiveTextKey
    {
      get
      {
        return (object) "VsColor.TitleBarActiveText";
      }
    }

    public static object TitleBarInactiveKey
    {
      get
      {
        return (object) "VsColor.TitleBarInactive";
      }
    }

    public static object TitleBarInactiveGradientBeginKey
    {
      get
      {
        return (object) "VsColor.TitleBarInactiveGradientBegin";
      }
    }

    public static object TitleBarInactiveGradientEndKey
    {
      get
      {
        return (object) "VsColor.TitleBarInactiveGradientEnd";
      }
    }

    public static object TitleBarInactiveTextKey
    {
      get
      {
        return (object) "VsColor.TitleBarInactiveText";
      }
    }

    public static object ToolboxBackgroundKey
    {
      get
      {
        return (object) "VsColor.ToolboxBackground";
      }
    }

    public static object ToolboxDividerKey
    {
      get
      {
        return (object) "VsColor.ToolboxDivider";
      }
    }

    public static object ToolboxGradientDarkKey
    {
      get
      {
        return (object) "VsColor.ToolboxGradientDark";
      }
    }

    public static object ToolboxGradientLightKey
    {
      get
      {
        return (object) "VsColor.ToolboxGradientLight";
      }
    }

    public static object ToolboxHeadingAccentKey
    {
      get
      {
        return (object) "VsColor.ToolboxHeadingAccent";
      }
    }

    public static object ToolboxHeadingBeginKey
    {
      get
      {
        return (object) "VsColor.ToolboxHeadingBegin";
      }
    }

    public static object ToolboxHeadingEndKey
    {
      get
      {
        return (object) "VsColor.ToolboxHeadingEnd";
      }
    }

    public static object ToolboxIconHighlightKey
    {
      get
      {
        return (object) "VsColor.ToolboxIconHighlight";
      }
    }

    public static object ToolboxIconShadowKey
    {
      get
      {
        return (object) "VsColor.ToolboxIconShadow";
      }
    }

    public static object ToolboxSelectedHeadingBeginKey
    {
      get
      {
        return (object) "VsColor.ToolboxSelectedHeadingBegin";
      }
    }

    public static object ToolboxSelectedHeadingEndKey
    {
      get
      {
        return (object) "VsColor.ToolboxSelectedHeadingEnd";
      }
    }

    public static object ToolboxSelectedHeadingMiddle1Key
    {
      get
      {
        return (object) "VsColor.ToolboxSelectedHeadingMiddle1";
      }
    }

    public static object ToolboxSelectedHeadingMiddle2Key
    {
      get
      {
        return (object) "VsColor.ToolboxSelectedHeadingMiddle2";
      }
    }

    public static object ToolWindowBackgroundKey
    {
      get
      {
        return (object) "VsColor.ToolWindowBackground";
      }
    }

    public static object ToolWindowBorderKey
    {
      get
      {
        return (object) "VsColor.ToolWindowBorder";
      }
    }

    public static object ToolWindowButtonActiveGlyphKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonActiveGlyph";
      }
    }

    public static object ToolWindowButtonDownKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonDown";
      }
    }

    public static object ToolWindowButtonDownActiveGlyphKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonDownActiveGlyph";
      }
    }

    public static object ToolWindowButtonDownBorderKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonDownBorder";
      }
    }

    public static object ToolWindowButtonDownInactiveGlyphKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonDownInactiveGlyph";
      }
    }

    public static object ToolWindowButtonHoverActiveKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonHoverActive";
      }
    }

    public static object ToolWindowButtonHoverActiveBorderKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonHoverActiveBorder";
      }
    }

    public static object ToolWindowButtonHoverActiveGlyphKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonHoverActiveGlyph";
      }
    }

    public static object ToolWindowButtonHoverInactiveKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonHoverInactive";
      }
    }

    public static object ToolWindowButtonHoverInactiveBorderKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonHoverInactiveBorder";
      }
    }

    public static object ToolWindowButtonHoverInactiveGlyphKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonHoverInactiveGlyph";
      }
    }

    public static object ToolWindowButtonInactiveKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonInactive";
      }
    }

    public static object ToolWindowButtonInactiveBorderKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonInactiveBorder";
      }
    }

    public static object ToolWindowButtonInactiveGlyphKey
    {
      get
      {
        return (object) "VsColor.ToolWindowButtonInactiveGlyph";
      }
    }

    public static object ToolWindowContentTabGradientBeginKey
    {
      get
      {
        return (object) "VsColor.ToolWindowContentTabGradientBegin";
      }
    }

    public static object ToolWindowContentTabGradientEndKey
    {
      get
      {
        return (object) "VsColor.ToolWindowContentTabGradientEnd";
      }
    }

    public static object ToolWindowFloatingFrameKey
    {
      get
      {
        return (object) "VsColor.ToolWindowFloatingFrame";
      }
    }

    public static object ToolWindowTabBorderKey
    {
      get
      {
        return (object) "VsColor.ToolWindowTabBorder";
      }
    }

    public static object ToolWindowTabGradientBeginKey
    {
      get
      {
        return (object) "VsColor.ToolWindowTabGradientBegin";
      }
    }

    public static object ToolWindowTabGradientEndKey
    {
      get
      {
        return (object) "VsColor.ToolWindowTabGradientEnd";
      }
    }

    public static object ToolWindowTabMouseOverBackgroundBeginKey
    {
      get
      {
        return (object) "VsColor.ToolWindowTabMouseOverBackgroundBegin";
      }
    }

    public static object ToolWindowTabMouseOverBackgroundEndKey
    {
      get
      {
        return (object) "VsColor.ToolWindowTabMouseOverBackgroundEnd";
      }
    }

    public static object ToolWindowTabMouseOverBorderKey
    {
      get
      {
        return (object) "VsColor.ToolWindowTabMouseOverBorder";
      }
    }

    public static object ToolWindowTabMouseOverTextKey
    {
      get
      {
        return (object) "VsColor.ToolWindowTabMouseOverText";
      }
    }

    public static object ToolWindowTabSelectedTabKey
    {
      get
      {
        return (object) "VsColor.ToolWindowTabSelectedTab";
      }
    }

    public static object ToolWindowTabSelectedTextKey
    {
      get
      {
        return (object) "VsColor.ToolWindowTabSelectedText";
      }
    }

    public static object ToolWindowTabTextKey
    {
      get
      {
        return (object) "VsColor.ToolWindowTabText";
      }
    }

    public static object ToolWindowTextKey
    {
      get
      {
        return (object) "VsColor.ToolWindowText";
      }
    }

    public static object VizSurfaceBrownDarkKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceBrownDark";
      }
    }

    public static object VizSurfaceBrownLightKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceBrownLight";
      }
    }

    public static object VizSurfaceBrownMediumKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceBrownMedium";
      }
    }

    public static object VizSurfaceDarkGoldDarkKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceDarkGoldDark";
      }
    }

    public static object VizSurfaceDarkGoldLightKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceDarkGoldLight";
      }
    }

    public static object VizSurfaceDarkGoldMediumKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceDarkGoldMedium";
      }
    }

    public static object VizSurfaceGoldDarkKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceGoldDark";
      }
    }

    public static object VizSurfaceGoldLightKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceGoldLight";
      }
    }

    public static object VizSurfaceGoldMediumKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceGoldMedium";
      }
    }

    public static object VizSurfaceGreenDarkKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceGreenDark";
      }
    }

    public static object VizSurfaceGreenLightKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceGreenLight";
      }
    }

    public static object VizSurfaceGreenMediumKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceGreenMedium";
      }
    }

    public static object VizSurfacePlumDarkKey
    {
      get
      {
        return (object) "VsColor.VizSurfacePlumDark";
      }
    }

    public static object VizSurfacePlumLightKey
    {
      get
      {
        return (object) "VsColor.VizSurfacePlumLight";
      }
    }

    public static object VizSurfacePlumMediumKey
    {
      get
      {
        return (object) "VsColor.VizSurfacePlumMedium";
      }
    }

    public static object VizSurfaceRedDarkKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceRedDark";
      }
    }

    public static object VizSurfaceRedLightKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceRedLight";
      }
    }

    public static object VizSurfaceRedMediumKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceRedMedium";
      }
    }

    public static object VizSurfaceSoftBlueDarkKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceSoftBlueDark";
      }
    }

    public static object VizSurfaceSoftBlueLightKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceSoftBlueLight";
      }
    }

    public static object VizSurfaceSoftBlueMediumKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceSoftBlueMedium";
      }
    }

    public static object VizSurfaceSteelBlueDarkKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceSteelBlueDark";
      }
    }

    public static object VizSurfaceSteelBlueLightKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceSteelBlueLight";
      }
    }

    public static object VizSurfaceSteelBlueMediumKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceSteelBlueMedium";
      }
    }

    public static object VizSurfaceStrongBlueDarkKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceStrongBlueDark";
      }
    }

    public static object VizSurfaceStrongBlueLightKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceStrongBlueLight";
      }
    }

    public static object VizSurfaceStrongBlueMediumKey
    {
      get
      {
        return (object) "VsColor.VizSurfaceStrongBlueMedium";
      }
    }

    public static object WindowKey
    {
      get
      {
        return (object) "VsColor.Window";
      }
    }

    public static object WindowFrameKey
    {
      get
      {
        return (object) "VsColor.WindowFrame";
      }
    }

    public static object WindowTextKey
    {
      get
      {
        return (object) "VsColor.WindowText";
      }
    }

    public static object WizardOrientationPanelBackgroundKey
    {
      get
      {
        return (object) "VsColor.WizardOrientationPanelBackground";
      }
    }

    public static object WizardOrientationPanelTextKey
    {
      get
      {
        return (object) "VsColor.WizardOrientationPanelText";
      }
    }
  }
}
