// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.NativeMethods
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Microsoft.VisualStudio
{
  internal static class NativeMethods
  {
    public static IntPtr InvalidIntPtr = (IntPtr) -1;
    public static readonly Guid IID_IServiceProvider = typeof (Microsoft.VisualStudio.OLE.Interop.IServiceProvider).GUID;
    public static readonly Guid IID_IObjectWithSite = typeof (IObjectWithSite).GUID;
    public static readonly Guid IID_IUnknown = new Guid("{00000000-0000-0000-C000-000000000046}");
    public static readonly Guid GUID_PropertyBrowserToolWindow = new Guid(-285584864, (short) -7528, (short) 4560, new byte[8]
    {
      (byte) 143,
      (byte) 120,
      (byte) 0,
      (byte) 160,
      (byte) 201,
      (byte) 17,
      (byte) 0,
      (byte) 87
    });
    public static readonly Guid GUID_VSStandardCommandSet97 = new Guid("{5efc7975-14bc-11cf-9b2b-00aa00573819}");
    public static readonly Guid CLSID_HtmDocData = new Guid(1657280404, (short) -22036, (short) 4560, new byte[8]
    {
      (byte) 129,
      (byte) 152,
      (byte) 0,
      (byte) 160,
      (byte) 201,
      (byte) 27,
      (byte) 190,
      (byte) 227
    });
    public static readonly Guid CLSID_HtmedPackage = new Guid(457407776, (short) -1794, (short) 4562, new byte[8]
    {
      (byte) 166,
      (byte) 174,
      (byte) 0,
      (byte) 16,
      (byte) 75,
      (byte) 204,
      (byte) 114,
      (byte) 105
    });
    public static readonly Guid CLSID_HtmlLanguageService = new Guid(1491695008, (short) -1794, (short) 4562, new byte[8]
    {
      (byte) 166,
      (byte) 174,
      (byte) 0,
      (byte) 16,
      (byte) 75,
      (byte) 204,
      (byte) 114,
      (byte) 105
    });
    public static readonly Guid GUID_HtmlEditorFactory = new Guid("{C76D83F8-A489-11D0-8195-00A0C91BBEE3}");
    public static readonly Guid GUID_TextEditorFactory = new Guid("{8B382828-6202-11d1-8870-0000F87579D2}");
    public static readonly Guid GUID_HTMEDAllowExistingDocData = new Guid(1463996950, (short) -32655, (short) 18297, new byte[8]
    {
      (byte) 191,
      (byte) 95,
      (byte) 162,
      (byte) 77,
      (byte) 95,
      (byte) 49,
      (byte) 66,
      (byte) 186
    });
    public static readonly Guid CLSID_VsEnvironmentPackage = new Guid("{DA9FB551-C724-11d0-AE1F-00A0C90FFFC3}");
    public static readonly Guid GUID_VsNewProjectPseudoFolder = new Guid("{DCF2A94A-45B0-11d1-ADBF-00C04FB6BE4C}");
    public static readonly Guid CLSID_MiscellaneousFilesProject = new Guid("{A2FE74E1-B743-11d0-AE1A-00A0C90FFFC3}");
    public static readonly Guid CLSID_SolutionItemsProject = new Guid("{D1DCDB85-C5E8-11d2-BFCA-00C04F990235}");
    public static readonly Guid SID_SVsGeneralOutputWindowPane = new Guid("{65482c72-defa-41b7-902c-11c091889c83}");
    public static readonly Guid SID_SUIHostCommandDispatcher = new Guid("{e69cd190-1276-11d1-9f64-00a0c911004f}");
    public static readonly Guid CLSID_VsUIHierarchyWindow = new Guid("{7D960B07-7AF8-11D0-8E5E-00A0C911005A}");
    public static readonly Guid GUID_DefaultEditor = new Guid("{6AC5EF80-12BF-11D1-8E9B-00A0C911005A}");
    public static readonly Guid GUID_ExternalEditor = new Guid("{8137C9E8-35FE-4AF2-87B0-DE3C45F395FD}");
    public static readonly Guid GUID_OutWindowGeneralPane = new Guid("{3c24d581-5591-4884-a571-9fe89915cd64}");
    public static readonly Guid BuildOrder = new Guid("2032b126-7c8d-48ad-8026-0e0348004fc0");
    public static readonly Guid BuildOutput = new Guid("1BD8A850-02D1-11d1-BEE7-00A0C913D1F8");
    public static readonly Guid DebugOutput = new Guid("FC076020-078A-11D1-A7DF-00A0C9110051");
    public static readonly Guid GUID_ItemType_PhysicalFile = new Guid("{6bb5f8ee-4483-11d3-8bcf-00c04f8ec28c}");
    public static readonly Guid GUID_ItemType_PhysicalFolder = new Guid("{6bb5f8ef-4483-11d3-8bcf-00c04f8ec28c}");
    public static readonly Guid GUID_ItemType_VirtualFolder = new Guid("{6bb5f8f0-4483-11d3-8bcf-00c04f8ec28c}");
    public static readonly Guid GUID_ItemType_SubProject = new Guid("{EA6618E8-6E24-4528-94BE-6889FE16485C}");
    public static readonly Guid UICONTEXT_SolutionBuilding = new Guid("{adfc4e60-0397-11d1-9f4e-00a0c911004f}");
    public static readonly Guid UICONTEXT_Debugging = new Guid("{adfc4e61-0397-11d1-9f4e-00a0c911004f}");
    public static readonly Guid UICONTEXT_Dragging = new Guid("{b706f393-2e5b-49e7-9e2e-b1825f639b63}");
    public static readonly Guid UICONTEXT_FullScreenMode = new Guid("{adfc4e62-0397-11d1-9f4e-00a0c911004f}");
    public static readonly Guid UICONTEXT_DesignMode = new Guid("{adfc4e63-0397-11d1-9f4e-00a0c911004f}");
    public static readonly Guid UICONTEXT_NoSolution = new Guid("{adfc4e64-0397-11d1-9f4e-00a0c911004f}");
    public static readonly Guid UICONTEXT_SolutionExists = new Guid("{f1536ef8-92ec-443c-9ed7-fdadf150da82}");
    public static readonly Guid UICONTEXT_EmptySolution = new Guid("{adfc4e65-0397-11d1-9f4e-00a0c911004f}");
    public static readonly Guid UICONTEXT_SolutionHasSingleProject = new Guid("{adfc4e66-0397-11d1-9f4e-00a0c911004f}");
    public static readonly Guid UICONTEXT_SolutionHasMultipleProjects = new Guid("{93694fa0-0397-11d1-9f4e-00a0c911004f}");
    public static readonly Guid UICONTEXT_CodeWindow = new Guid("{8fe2df1d-e0da-4ebe-9d5c-415d40e487b5}");
    public static readonly Guid GUID_VsTaskListViewAll = new Guid("{1880202e-fc20-11d2-8bb1-00c04f8ec28c}");
    public static readonly Guid GUID_VsTaskListViewUserTasks = new Guid("{1880202f-fc20-11d2-8bb1-00c04f8ec28c}");
    public static readonly Guid GUID_VsTaskListViewShortcutTasks = new Guid("{18802030-fc20-11d2-8bb1-00c04f8ec28c}");
    public static readonly Guid GUID_VsTaskListViewHTMLTasks = new Guid("{36ac1c0d-fe86-11d2-8bb1-00c04f8ec28c}");
    public static readonly Guid GUID_VsTaskListViewCompilerTasks = new Guid("{18802033-fc20-11d2-8bb1-00c04f8ec28c}");
    public static readonly Guid GUID_VsTaskListViewCommentTasks = new Guid("{18802034-fc20-11d2-8bb1-00c04f8ec28c}");
    public static readonly Guid GUID_VsTaskListViewCurrentFileTasks = new Guid("{18802035-fc20-11d2-8bb1-00c04f8ec28c}");
    public static readonly Guid GUID_VsTaskListViewCheckedTasks = new Guid("{18802036-fc20-11d2-8bb1-00c04f8ec28c}");
    public static readonly Guid GUID_VsTaskListViewUncheckedTasks = new Guid("{18802037-fc20-11d2-8bb1-00c04f8ec28c}");
    public static readonly Guid CLSID_VsTaskList = new Guid("{BC5955D5-aa0d-11d0-a8c5-00a0c921a4d2}");
    public static readonly Guid CLSID_VsTaskListPackage = new Guid("{4A9B7E50-aa16-11d0-a8c5-00a0c921a4d2}");
    public static readonly Guid SID_SVsToolboxActiveXDataProvider = new Guid("{35222106-bb44-11d0-8c46-00c04fc2aae2}");
    public static readonly Guid CLSID_VsDocOutlinePackage = new Guid("{21af45b0-ffa5-11d0-b63f-00a0c922e851}");
    public static readonly Guid CLSID_VsCfgProviderEventsHelper = new Guid("{99913f1f-1ee3-11d1-8a6e-00c04f682e21}");
    public static readonly Guid GUID_COMPlusPage = new Guid("{9A341D95-5A64-11d3-BFF9-00C04F990235}");
    public static readonly Guid GUID_COMClassicPage = new Guid("{9A341D96-5A64-11d3-BFF9-00C04F990235}");
    public static readonly Guid GUID_SolutionPage = new Guid("{9A341D97-5A64-11d3-BFF9-00C04F990235}");
    public static readonly Guid LOGVIEWID_Any = new Guid(uint.MaxValue, ushort.MaxValue, ushort.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
    public static readonly Guid LOGVIEWID_Primary = Guid.Empty;
    public static readonly Guid LOGVIEWID_Debugging = new Guid("{7651a700-06e5-11d1-8ebd-00a0c90f26ea}");
    public static readonly Guid LOGVIEWID_Code = new Guid("{7651a701-06e5-11d1-8ebd-00a0c90f26ea}");
    public static readonly Guid LOGVIEWID_Designer = new Guid("{7651a702-06e5-11d1-8ebd-00a0c90f26ea}");
    public static readonly Guid LOGVIEWID_TextView = new Guid("{7651a703-06e5-11d1-8ebd-00a0c90f26ea}");
    public static readonly Guid LOGVIEWID_UserChooseView = new Guid("{7651a704-06e5-11d1-8ebd-00a0c90f26ea}");
    public static readonly Guid GUID_VsUIHierarchyWindowCmds = new Guid("{60481700-078b-11d1-aaf8-00a0c9055a90}");
    public const int CLSCTX_INPROC_SERVER = 1;
    public const int S_FALSE = 1;
    public const int S_OK = 0;
    public const int IDOK = 1;
    public const int IDCANCEL = 2;
    public const int IDABORT = 3;
    public const int IDRETRY = 4;
    public const int IDIGNORE = 5;
    public const int IDYES = 6;
    public const int IDNO = 7;
    public const int IDCLOSE = 8;
    public const int IDHELP = 9;
    public const int IDTRYAGAIN = 10;
    public const int IDCONTINUE = 11;
    public const int ILD_NORMAL = 0;
    public const int ILD_TRANSPARENT = 1;
    public const int ILD_MASK = 16;
    public const int ILD_ROP = 64;
    public const int OLECMDERR_E_NOTSUPPORTED = -2147221248;
    public const int OLECMDERR_E_UNKNOWNGROUP = -2147221244;
    public const int UNDO_E_CLIENTABORT = -2147205119;
    public const int E_OUTOFMEMORY = -2147024882;
    public const int E_INVALIDARG = -2147024809;
    public const int E_FAIL = -2147467259;
    public const int E_NOINTERFACE = -2147467262;
    public const int E_POINTER = -2147467261;
    public const int E_NOTIMPL = -2147467263;
    public const int E_UNEXPECTED = -2147418113;
    public const int E_HANDLE = -2147024890;
    public const int E_ABORT = -2147467260;
    public const int E_ACCESSDENIED = -2147024891;
    public const int E_PENDING = -2147483638;
    public const int VS_E_UNSUPPORTEDFORMAT = -2147213333;
    public const int VS_E_INCOMPATIBLEDOCDATA = -2147213334;
    public const int VS_E_PACKAGENOTLOADED = -2147213343;
    public const int VS_E_PROJECTNOTLOADED = -2147213342;
    public const int VS_E_SOLUTIONNOTOPEN = -2147213341;
    public const int VS_E_SOLUTIONALREADYOPEN = -2147213340;
    public const int VS_E_PROJECTMIGRATIONFAILED = -2147213339;
    public const int VS_E_WIZARDBACKBUTTONPRESS = -2147213313;
    public const int VS_S_PROJECTFORWARDED = 270320;
    public const int VS_S_TBXMARKER = 270321;
    public const int E_VS_MAPMISSING = -2147213311;
    public const ushort CF_HDROP = 15;
    public const uint MK_CONTROL = 8;
    public const uint MK_SHIFT = 4;
    public const int MAX_PATH = 260;
    public const int OLECLOSE_SAVEIFDIRTY = 0;
    public const int OLECLOSE_NOSAVE = 1;
    public const int OLECLOSE_PROMPTSAVE = 2;
    public const int OLEIVERB_PRIMARY = 0;
    public const int OLEIVERB_SHOW = -1;
    public const int OLEIVERB_OPEN = -2;
    public const int OLEIVERB_HIDE = -3;
    public const int OLEIVERB_UIACTIVATE = -4;
    public const int OLEIVERB_INPLACEACTIVATE = -5;
    public const int OLEIVERB_DISCARDUNDOSTATE = -6;
    public const int OLEIVERB_PROPERTIES = -7;
    public const int OLE_E_OLEVERB = -2147221504;
    public const int OLE_E_ADVF = -2147221503;
    public const int OLE_E_ENUM_NOMORE = -2147221502;
    public const int OLE_E_ADVISENOTSUPPORTED = -2147221501;
    public const int OLE_E_NOCONNECTION = -2147221500;
    public const int OLE_E_NOTRUNNING = -2147221499;
    public const int OLE_E_NOCACHE = -2147221498;
    public const int OLE_E_BLANK = -2147221497;
    public const int OLE_E_CLASSDIFF = -2147221496;
    public const int OLE_E_CANT_GETMONIKER = -2147221495;
    public const int OLE_E_CANT_BINDTOSOURCE = -2147221494;
    public const int OLE_E_STATIC = -2147221493;
    public const int OLE_E_PROMPTSAVECANCELLED = -2147221492;
    public const int OLE_E_INVALIDRECT = -2147221491;
    public const int OLE_E_WRONGCOMPOBJ = -2147221490;
    public const int OLE_E_INVALIDHWND = -2147221489;
    public const int OLE_E_NOT_INPLACEACTIVE = -2147221488;
    public const int OLE_E_CANTCONVERT = -2147221487;
    public const int OLE_E_NOSTORAGE = -2147221486;
    public const int DISP_E_UNKNOWNINTERFACE = -2147352575;
    public const int DISP_E_MEMBERNOTFOUND = -2147352573;
    public const int DISP_E_PARAMNOTFOUND = -2147352572;
    public const int DISP_E_TYPEMISMATCH = -2147352571;
    public const int DISP_E_UNKNOWNNAME = -2147352570;
    public const int DISP_E_NONAMEDARGS = -2147352569;
    public const int DISP_E_BADVARTYPE = -2147352568;
    public const int DISP_E_EXCEPTION = -2147352567;
    public const int DISP_E_OVERFLOW = -2147352566;
    public const int DISP_E_BADINDEX = -2147352565;
    public const int DISP_E_UNKNOWNLCID = -2147352564;
    public const int DISP_E_ARRAYISLOCKED = -2147352563;
    public const int DISP_E_BADPARAMCOUNT = -2147352562;
    public const int DISP_E_PARAMNOTOPTIONAL = -2147352561;
    public const int DISP_E_BADCALLEE = -2147352560;
    public const int DISP_E_NOTACOLLECTION = -2147352559;
    public const int DISP_E_DIVBYZERO = -2147352558;
    public const int DISP_E_BUFFERTOOSMALL = -2147352557;
    public const int OFN_READONLY = 1;
    public const int OFN_OVERWRITEPROMPT = 2;
    public const int OFN_HIDEREADONLY = 4;
    public const int OFN_NOCHANGEDIR = 8;
    public const int OFN_SHOWHELP = 16;
    public const int OFN_ENABLEHOOK = 32;
    public const int OFN_ENABLETEMPLATE = 64;
    public const int OFN_ENABLETEMPLATEHANDLE = 128;
    public const int OFN_NOVALIDATE = 256;
    public const int OFN_ALLOWMULTISELECT = 512;
    public const int OFN_EXTENSIONDIFFERENT = 1024;
    public const int OFN_PATHMUSTEXIST = 2048;
    public const int OFN_FILEMUSTEXIST = 4096;
    public const int OFN_CREATEPROMPT = 8192;
    public const int OFN_SHAREAWARE = 16384;
    public const int OFN_NOREADONLYRETURN = 32768;
    public const int OFN_NOTESTFILECREATE = 65536;
    public const int OFN_NONETWORKBUTTON = 131072;
    public const int OFN_NOLONGNAMES = 262144;
    public const int OFN_EXPLORER = 524288;
    public const int OFN_NODEREFERENCELINKS = 1048576;
    public const int OFN_LONGNAMES = 2097152;
    public const int OFN_ENABLEINCLUDENOTIFY = 4194304;
    public const int OFN_ENABLESIZING = 8388608;
    public const int OFN_USESHELLITEM = 16777216;
    public const int OFN_DONTADDTORECENT = 33554432;
    public const int OFN_FORCESHOWHIDDEN = 268435456;
    public const uint VSITEMID_NIL = 4294967295;
    public const uint VSITEMID_ROOT = 4294967294;
    public const uint VSITEMID_SELECTION = 4294967293;
    public const uint VSCOOKIE_NIL = 0;
    public const uint ALL = 1;
    public const uint SELECTED = 2;
    public const uint UndoManager = 0;
    public const uint WindowFrame = 1;
    public const uint DocumentFrame = 2;
    public const uint StartupProject = 3;
    public const uint PropertyBrowserSID = 4;
    public const uint UserContext = 5;
    public const int ROSTATUS_NotReadOnly = 0;
    public const int ROSTATUS_ReadOnly = 1;
    public const int ROSTATUS_Unknown = -1;
    public const int IEI_DoNotLoadDocData = 268435456;
    public const int CB_SETDROPPEDWIDTH = 352;
    public const int GWL_WNDPROC = -4;
    public const int GWL_STYLE = -16;
    public const int GWL_EXSTYLE = -20;
    public const int GWLP_WNDPROC = -4;
    public const int GWLP_HINSTANCE = -6;
    public const int GWLP_HWNDPARENT = -8;
    public const int GWLP_USERDATA = -21;
    public const int GWLP_ID = -12;
    public const int DWL_MSGRESULT = 0;
    public const int HTMENU = 5;
    public const int WS_POPUP = -2147483648;
    public const int WS_CHILD = 1073741824;
    public const int WS_MINIMIZE = 536870912;
    public const int WS_VISIBLE = 268435456;
    public const int WS_DISABLED = 134217728;
    public const int WS_CLIPSIBLINGS = 67108864;
    public const int WS_CLIPCHILDREN = 33554432;
    public const int WS_MAXIMIZE = 16777216;
    public const int WS_CAPTION = 12582912;
    public const int WS_BORDER = 8388608;
    public const int WS_DLGFRAME = 4194304;
    public const int WS_VSCROLL = 2097152;
    public const int WS_HSCROLL = 1048576;
    public const int WS_SYSMENU = 524288;
    public const int WS_THICKFRAME = 262144;
    public const int WS_TABSTOP = 65536;
    public const int WS_MINIMIZEBOX = 131072;
    public const int WS_MAXIMIZEBOX = 65536;
    public const int WS_EX_DLGMODALFRAME = 1;
    public const int WS_EX_NOPARENTNOTIFY = 4;
    public const int WS_EX_TOPMOST = 8;
    public const int WS_EX_ACCEPTFILES = 16;
    public const int WS_EX_TRANSPARENT = 32;
    public const int WS_EX_MDICHILD = 64;
    public const int WS_EX_TOOLWINDOW = 128;
    public const int WS_EX_WINDOWEDGE = 256;
    public const int WS_EX_CLIENTEDGE = 512;
    public const int WS_EX_CONTEXTHELP = 1024;
    public const int WS_EX_RIGHT = 4096;
    public const int WS_EX_LEFT = 0;
    public const int WS_EX_RTLREADING = 8192;
    public const int WS_EX_LTRREADING = 0;
    public const int WS_EX_LEFTSCROLLBAR = 16384;
    public const int WS_EX_RIGHTSCROLLBAR = 0;
    public const int WS_EX_CONTROLPARENT = 65536;
    public const int WS_EX_STATICEDGE = 131072;
    public const int WS_EX_APPWINDOW = 262144;
    public const int WS_EX_LAYERED = 524288;
    public const int WS_EX_NOINHERITLAYOUT = 1048576;
    public const int WS_EX_LAYOUTRTL = 4194304;
    public const int WS_EX_COMPOSITED = 33554432;
    public const int WS_EX_NOACTIVATE = 134217728;
    public const int WS_EX_OVERLAPPEDWINDOW = 768;
    public const int WS_EX_PALETTEWINDOW = 392;
    public const int LVM_SETEXTENDEDLISTVIEWSTYLE = 4150;
    public const int LVS_EX_LABELTIP = 16384;
    public const int WH_JOURNALPLAYBACK = 1;
    public const int WH_GETMESSAGE = 3;
    public const int WH_MOUSE = 7;
    public const int WSF_VISIBLE = 1;
    public const int WM_NULL = 0;
    public const int WM_CREATE = 1;
    public const int WM_DELETEITEM = 45;
    public const int WM_DESTROY = 2;
    public const int WM_MOVE = 3;
    public const int WM_SIZE = 5;
    public const int WM_ACTIVATE = 6;
    public const int WA_INACTIVE = 0;
    public const int WA_ACTIVE = 1;
    public const int WA_CLICKACTIVE = 2;
    public const int WM_SETFOCUS = 7;
    public const int WM_KILLFOCUS = 8;
    public const int WM_ENABLE = 10;
    public const int WM_SETREDRAW = 11;
    public const int WM_SETTEXT = 12;
    public const int WM_GETTEXT = 13;
    public const int WM_GETTEXTLENGTH = 14;
    public const int WM_PAINT = 15;
    public const int WM_CLOSE = 16;
    public const int WM_QUERYENDSESSION = 17;
    public const int WM_QUIT = 18;
    public const int WM_QUERYOPEN = 19;
    public const int WM_ERASEBKGND = 20;
    public const int WM_SYSCOLORCHANGE = 21;
    public const int WM_ENDSESSION = 22;
    public const int WM_SHOWWINDOW = 24;
    public const int WM_WININICHANGE = 26;
    public const int WM_SETTINGCHANGE = 26;
    public const int WM_DEVMODECHANGE = 27;
    public const int WM_ACTIVATEAPP = 28;
    public const int WM_FONTCHANGE = 29;
    public const int WM_TIMECHANGE = 30;
    public const int WM_CANCELMODE = 31;
    public const int WM_SETCURSOR = 32;
    public const int WM_MOUSEACTIVATE = 33;
    public const int WM_CHILDACTIVATE = 34;
    public const int WM_QUEUESYNC = 35;
    public const int WM_GETMINMAXINFO = 36;
    public const int WM_PAINTICON = 38;
    public const int WM_ICONERASEBKGND = 39;
    public const int WM_NEXTDLGCTL = 40;
    public const int WM_SPOOLERSTATUS = 42;
    public const int WM_DRAWITEM = 43;
    public const int WM_MEASUREITEM = 44;
    public const int WM_VKEYTOITEM = 46;
    public const int WM_CHARTOITEM = 47;
    public const int WM_SETFONT = 48;
    public const int WM_GETFONT = 49;
    public const int WM_SETHOTKEY = 50;
    public const int WM_GETHOTKEY = 51;
    public const int WM_QUERYDRAGICON = 55;
    public const int WM_COMPAREITEM = 57;
    public const int WM_GETOBJECT = 61;
    public const int WM_COMPACTING = 65;
    public const int WM_COMMNOTIFY = 68;
    public const int WM_WINDOWPOSCHANGING = 70;
    public const int WM_WINDOWPOSCHANGED = 71;
    public const int WM_POWER = 72;
    public const int WM_COPYDATA = 74;
    public const int WM_CANCELJOURNAL = 75;
    public const int WM_NOTIFY = 78;
    public const int WM_INPUTLANGCHANGEREQUEST = 80;
    public const int WM_INPUTLANGCHANGE = 81;
    public const int WM_TCARD = 82;
    public const int WM_HELP = 83;
    public const int WM_USERCHANGED = 84;
    public const int WM_NOTIFYFORMAT = 85;
    public const int WM_CONTEXTMENU = 123;
    public const int WM_STYLECHANGING = 124;
    public const int WM_STYLECHANGED = 125;
    public const int WM_DISPLAYCHANGE = 126;
    public const int WM_GETICON = 127;
    public const int WM_SETICON = 128;
    public const int WM_NCCREATE = 129;
    public const int WM_NCDESTROY = 130;
    public const int WM_NCCALCSIZE = 131;
    public const int WM_NCHITTEST = 132;
    public const int WM_NCPAINT = 133;
    public const int WM_NCACTIVATE = 134;
    public const int WM_GETDLGCODE = 135;
    public const int WM_NCMOUSEMOVE = 160;
    public const int WM_NCLBUTTONDOWN = 161;
    public const int WM_NCLBUTTONUP = 162;
    public const int WM_NCLBUTTONDBLCLK = 163;
    public const int WM_NCRBUTTONDOWN = 164;
    public const int WM_NCRBUTTONUP = 165;
    public const int WM_NCRBUTTONDBLCLK = 166;
    public const int WM_NCMBUTTONDOWN = 167;
    public const int WM_NCMBUTTONUP = 168;
    public const int WM_NCMBUTTONDBLCLK = 169;
    public const int WM_NCXBUTTONDOWN = 171;
    public const int WM_NCXBUTTONUP = 172;
    public const int WM_NCXBUTTONDBLCLK = 173;
    public const int WM_KEYFIRST = 256;
    public const int WM_KEYDOWN = 256;
    public const int WM_KEYUP = 257;
    public const int WM_CHAR = 258;
    public const int WM_DEADCHAR = 259;
    public const int WM_CTLCOLOR = 25;
    public const int WM_SYSKEYDOWN = 260;
    public const int WM_SYSKEYUP = 261;
    public const int WM_SYSCHAR = 262;
    public const int WM_SYSDEADCHAR = 263;
    public const int WM_KEYLAST = 264;
    public const int WM_IME_STARTCOMPOSITION = 269;
    public const int WM_IME_ENDCOMPOSITION = 270;
    public const int WM_IME_COMPOSITION = 271;
    public const int WM_IME_KEYLAST = 271;
    public const int WM_INITDIALOG = 272;
    public const int WM_COMMAND = 273;
    public const int WM_SYSCOMMAND = 274;
    public const int WM_TIMER = 275;
    public const int WM_HSCROLL = 276;
    public const int WM_VSCROLL = 277;
    public const int WM_INITMENU = 278;
    public const int WM_INITMENUPOPUP = 279;
    public const int WM_MENUSELECT = 287;
    public const int WM_MENUCHAR = 288;
    public const int WM_ENTERIDLE = 289;
    public const int WM_CHANGEUISTATE = 295;
    public const int WM_UPDATEUISTATE = 296;
    public const int WM_QUERYUISTATE = 297;
    public const int WM_CTLCOLORMSGBOX = 306;
    public const int WM_CTLCOLOREDIT = 307;
    public const int WM_CTLCOLORLISTBOX = 308;
    public const int WM_CTLCOLORBTN = 309;
    public const int WM_CTLCOLORDLG = 310;
    public const int WM_CTLCOLORSCROLLBAR = 311;
    public const int WM_CTLCOLORSTATIC = 312;
    public const int WM_MOUSEFIRST = 512;
    public const int WM_MOUSEMOVE = 512;
    public const int WM_LBUTTONDOWN = 513;
    public const int WM_LBUTTONUP = 514;
    public const int WM_LBUTTONDBLCLK = 515;
    public const int WM_RBUTTONDOWN = 516;
    public const int WM_RBUTTONUP = 517;
    public const int WM_RBUTTONDBLCLK = 518;
    public const int WM_MBUTTONDOWN = 519;
    public const int WM_MBUTTONUP = 520;
    public const int WM_MBUTTONDBLCLK = 521;
    public const int WM_XBUTTONDOWN = 523;
    public const int WM_XBUTTONUP = 524;
    public const int WM_XBUTTONDBLCLK = 525;
    public const int WM_MOUSEWHEEL = 522;
    public const int WM_MOUSELAST = 522;
    public const int WM_PARENTNOTIFY = 528;
    public const int WM_ENTERMENULOOP = 529;
    public const int WM_EXITMENULOOP = 530;
    public const int WM_NEXTMENU = 531;
    public const int WM_SIZING = 532;
    public const int WM_CAPTURECHANGED = 533;
    public const int WM_MOVING = 534;
    public const int WM_POWERBROADCAST = 536;
    public const int WM_DEVICECHANGE = 537;
    public const int WM_IME_SETCONTEXT = 641;
    public const int WM_IME_NOTIFY = 642;
    public const int WM_IME_CONTROL = 643;
    public const int WM_IME_COMPOSITIONFULL = 644;
    public const int WM_IME_SELECT = 645;
    public const int WM_IME_CHAR = 646;
    public const int WM_IME_KEYDOWN = 656;
    public const int WM_IME_KEYUP = 657;
    public const int WM_MDICREATE = 544;
    public const int WM_MDIDESTROY = 545;
    public const int WM_MDIACTIVATE = 546;
    public const int WM_MDIRESTORE = 547;
    public const int WM_MDINEXT = 548;
    public const int WM_MDIMAXIMIZE = 549;
    public const int WM_MDITILE = 550;
    public const int WM_MDICASCADE = 551;
    public const int WM_MDIICONARRANGE = 552;
    public const int WM_MDIGETACTIVE = 553;
    public const int WM_MDISETMENU = 560;
    public const int WM_ENTERSIZEMOVE = 561;
    public const int WM_EXITSIZEMOVE = 562;
    public const int WM_DROPFILES = 563;
    public const int WM_MDIREFRESHMENU = 564;
    public const int WM_MOUSEHOVER = 673;
    public const int WM_MOUSELEAVE = 675;
    public const int WM_CUT = 768;
    public const int WM_COPY = 769;
    public const int WM_PASTE = 770;
    public const int WM_CLEAR = 771;
    public const int WM_UNDO = 772;
    public const int WM_RENDERFORMAT = 773;
    public const int WM_RENDERALLFORMATS = 774;
    public const int WM_DESTROYCLIPBOARD = 775;
    public const int WM_DRAWCLIPBOARD = 776;
    public const int WM_PAINTCLIPBOARD = 777;
    public const int WM_VSCROLLCLIPBOARD = 778;
    public const int WM_SIZECLIPBOARD = 779;
    public const int WM_ASKCBFORMATNAME = 780;
    public const int WM_CHANGECBCHAIN = 781;
    public const int WM_HSCROLLCLIPBOARD = 782;
    public const int WM_QUERYNEWPALETTE = 783;
    public const int WM_PALETTEISCHANGING = 784;
    public const int WM_PALETTECHANGED = 785;
    public const int WM_THEMECHANGED = 794;
    public const int WM_HOTKEY = 786;
    public const int WM_PRINT = 791;
    public const int WM_PRINTCLIENT = 792;
    public const int WM_HANDHELDFIRST = 856;
    public const int WM_HANDHELDLAST = 863;
    public const int WM_AFXFIRST = 864;
    public const int WM_AFXLAST = 895;
    public const int WM_PENWINFIRST = 896;
    public const int WM_PENWINLAST = 911;
    public const int WM_APP = 32768;
    public const int WM_USER = 1024;
    public const int WM_REFLECT = 8192;
    public const int WS_OVERLAPPED = 0;
    public const int WPF_SETMINPOSITION = 1;
    public const int WM_CHOOSEFONT_GETLOGFONT = 1025;
    public const int WHEEL_DELTA = 120;
    public const int DWLP_MSGRESULT = 0;
    public const int PSNRET_NOERROR = 0;
    public const int PSNRET_INVALID = 1;
    public const int PSNRET_INVALID_NOCHANGEPAGE = 2;
    public const int PSN_APPLY = -202;
    public const int PSN_KILLACTIVE = -201;
    public const int PSN_RESET = -203;
    public const int PSN_SETACTIVE = -200;
    public const int GMEM_MOVEABLE = 2;
    public const int GMEM_ZEROINIT = 64;
    public const int GMEM_DDESHARE = 8192;
    public const int SW_HIDE = 0;
    public const int SW_SHOWNORMAL = 1;
    public const int SW_NORMAL = 1;
    public const int SW_SHOWMINIMIZED = 2;
    public const int SW_SHOWMAXIMIZED = 3;
    public const int SW_MAXIMIZE = 3;
    public const int SW_SHOWNOACTIVATE = 4;
    public const int SW_SHOW = 5;
    public const int SW_MINIMIZE = 6;
    public const int SW_SHOWMINNOACTIVE = 7;
    public const int SW_SHOWNA = 8;
    public const int SW_RESTORE = 9;
    public const int SW_SHOWDEFAULT = 10;
    public const int SW_FORCEMINIMIZE = 11;
    public const int SW_MAX = 11;
    public const int TVM_SETINSERTMARK = 4378;
    public const int TVM_GETEDITCONTROL = 4367;
    public const int FILE_ATTRIBUTE_READONLY = 1;
    public const uint CEF_CLONEFILE = 1;
    public const uint CEF_OPENFILE = 2;
    public const uint CEF_SILENT = 4;
    public const uint CEF_OPENASNEW = 8;
    public const int cmdidToolsOptions = 264;
    public const int LOGPIXELSX = 88;
    public const int LOGPIXELSY = 90;
    public const int VK_LBUTTON = 1;
    public const int VK_RBUTTON = 2;
    public const int VK_MBUTTON = 4;
    public const int VK_XBUTTON1 = 5;
    public const int VK_XBUTTON2 = 6;
    public const int VK_SHIFT = 16;
    public const int VK_CONTROL = 17;
    public const int VK_MENU = 18;
    public const int VK_LSHIFT = 160;
    public const int VK_RSHIFT = 161;
    public const int VK_LCONTROL = 162;
    public const int VK_RCONTROL = 163;
    public const int VK_LMENU = 164;
    public const int VK_RMENU = 165;
    public const int VK_LWIN = 91;
    public const int VK_RWIN = 92;
    public const int VK_F1 = 112;
    public const int SPI_SETHIGHCONTRAST = 67;
    public const int ICON_SMALL = 0;
    public const int ICON_BIG = 1;
    public const int SC_SIZE = 61440;
    public const int SC_MOVE = 61456;
    public const int SC_MINIMIZE = 61472;
    public const int SC_MAXIMIZE = 61488;
    public const int SC_NEXTWINDOW = 61504;
    public const int SC_PREVWINDOW = 61520;
    public const int SC_CLOSE = 61536;
    public const int SC_VSCROLL = 61552;
    public const int SC_HSCROLL = 61568;
    public const int SC_MOUSEMENU = 61584;
    public const int SC_KEYMENU = 61696;
    public const int SC_ARRANGE = 61712;
    public const int SC_RESTORE = 61728;
    public const int SC_TASKLIST = 61744;
    public const int SC_SCREENSAVE = 61760;
    public const int SC_HOTKEY = 61776;
    public const int SC_DEFAULT = 61792;
    public const int SC_MONITORPOWER = 61808;
    public const int SC_CONTEXTHELP = 61824;
    public const int SC_SEPARATOR = 61455;
    public const int SWP_NOSIZE = 1;
    public const int SWP_NOMOVE = 2;
    public const int SWP_NOZORDER = 4;
    public const int SWP_NOREDRAW = 8;
    public const int SWP_NOACTIVATE = 16;
    public const int SWP_FRAMECHANGED = 32;
    public const int SWP_SHOWWINDOW = 64;
    public const int SWP_HIDEWINDOW = 128;
    public const int SWP_NOCOPYBITS = 256;
    public const int SWP_NOOWNERZORDER = 512;
    public const int SWP_NOSENDCHANGING = 1024;
    public const int SWP_DEFERERASE = 8192;
    public const int SWP_ASYNCWINDOWPOS = 16384;
    public const int PSP_DEFAULT = 0;
    public const int PSP_DLGINDIRECT = 1;
    public const int PSP_USEHICON = 2;
    public const int PSP_USEICONID = 4;
    public const int PSP_USETITLE = 8;
    public const int PSP_RTLREADING = 16;
    public const int PSP_HASHELP = 32;
    public const int PSP_USEREFPARENT = 64;
    public const int PSP_USECALLBACK = 128;
    public const int PSP_PREMATURE = 1024;
    public const int PSP_HIDEHEADER = 2048;
    public const int PSP_USEHEADERTITLE = 4096;
    public const int PSP_USEHEADERSUBTITLE = 8192;
    public const int PSH_DEFAULT = 0;
    public const int PSH_PROPTITLE = 1;
    public const int PSH_USEHICON = 2;
    public const int PSH_USEICONID = 4;
    public const int PSH_PROPSHEETPAGE = 8;
    public const int PSH_WIZARDHASFINISH = 16;
    public const int PSH_WIZARD = 32;
    public const int PSH_USEPSTARTPAGE = 64;
    public const int PSH_NOAPPLYNOW = 128;
    public const int PSH_USECALLBACK = 256;
    public const int PSH_HASHELP = 512;
    public const int PSH_MODELESS = 1024;
    public const int PSH_RTLREADING = 2048;
    public const int PSH_WIZARDCONTEXTHELP = 4096;
    public const int PSH_WATERMARK = 32768;
    public const int PSH_USEHBMWATERMARK = 65536;
    public const int PSH_USEHPLWATERMARK = 131072;
    public const int PSH_STRETCHWATERMARK = 262144;
    public const int PSH_HEADER = 524288;
    public const int PSH_USEHBMHEADER = 1048576;
    public const int PSH_USEPAGELANG = 2097152;
    public const int PSH_WIZARD_LITE = 4194304;
    public const int PSH_NOCONTEXTHELP = 33554432;
    public const int PSBTN_BACK = 0;
    public const int PSBTN_NEXT = 1;
    public const int PSBTN_FINISH = 2;
    public const int PSBTN_OK = 3;
    public const int PSBTN_APPLYNOW = 4;
    public const int PSBTN_CANCEL = 5;
    public const int PSBTN_HELP = 6;
    public const int PSBTN_MAX = 6;
    public const int TRANSPARENT = 1;
    public const int OPAQUE = 2;
    public const int FW_BOLD = 700;
    private const byte KeyDown = 128;

    public static bool Succeeded(int hr)
    {
      return hr >= 0;
    }

    public static bool Failed(int hr)
    {
      return hr < 0;
    }

    public static int ThrowOnFailure(int hr)
    {
      return NativeMethods.ThrowOnFailure(hr, (int[]) null);
    }

    public static int ThrowOnFailure(int hr, params int[] expectedHRFailure)
    {
      if (NativeMethods.Failed(hr) && (expectedHRFailure == null || Array.IndexOf<int>(expectedHRFailure, hr) < 0))
        Marshal.ThrowExceptionForHR(hr);
      return hr;
    }

    public static int SignedHIWORD(int n)
    {
      return (int) (short) (n >> 16 & (int) ushort.MaxValue);
    }

    public static int SignedLOWORD(int n)
    {
      return (int) (short) (n & (int) ushort.MaxValue);
    }

    public static string GetAbsolutePath(string fileName)
    {
      Uri uri = new Uri(fileName);
      return uri.LocalPath + uri.Fragment;
    }

    public static string GetLocalPath(string fileName)
    {
      Uri uri = new Uri(fileName);
      return uri.LocalPath + uri.Fragment;
    }

    public static string GetLocalPathUnescaped(string url)
    {
      string str = "file:///";
      if (url.StartsWith(str, StringComparison.OrdinalIgnoreCase))
        return url.Substring(str.Length);
      return NativeMethods.GetLocalPath(url);
    }

    public static bool IsSamePath(string file1, string file2)
    {
      if (file1 == null || file1.Length == 0)
      {
        if (file2 != null)
          return file2.Length == 0;
        return true;
      }
      Uri uri1 = new Uri(file1);
      Uri uri2 = new Uri(file2);
      if (uri1.IsFile && uri2.IsFile)
        return 0 == string.Compare(uri1.LocalPath, uri2.LocalPath, StringComparison.OrdinalIgnoreCase);
      return file1 == file2;
    }

    [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetFileAttributes(string name);

    [DllImport("User32", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

    public static IntPtr GetNativeWndProc(Control control)
    {
      IntPtr handle = control.Handle;
      return NativeMethods.GetWindowLong(new HandleRef((object) control, handle), -4);
    }

    public static IntPtr GetWindowLong(HandleRef hWnd, int nIndex)
    {
      return new IntPtr(NativeMethods.GetWindowLong(hWnd.Handle, nIndex));
    }

    public static int GetWindowLong(IntPtr hWnd, int nIndex)
    {
      return NativeMethods.GetWindowLong32(hWnd, nIndex);
    }

    public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {
      if (IntPtr.Size == 8)
        return NativeMethods.GetWindowLongPtr64(hWnd, nIndex);
      return new IntPtr(NativeMethods.GetWindowLong32(hWnd, nIndex));
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
    public static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int SetWindowLong(IntPtr hWnd, short nIndex, int value);

    [DllImport("user32.dll")]
    public static extern IntPtr GetParent(IntPtr hWnd);

    [DllImport("User32", CharSet = CharSet.Auto)]
    public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

    [DllImport("User32", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(
      IntPtr hWnd,
      IntPtr hWndInsertAfter,
      int x,
      int y,
      int cx,
      int cy,
      int flags);

    [DllImport("User32", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("Kernel32", CharSet = CharSet.Auto)]
    public static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);

    [DllImport("Kernel32", CharSet = CharSet.Auto)]
    public static extern IntPtr GlobalReAlloc(HandleRef handle, int bytes, int flags);

    [DllImport("Kernel32", CharSet = CharSet.Auto)]
    public static extern IntPtr GlobalLock(HandleRef handle);

    [DllImport("Kernel32", CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GlobalUnlock(HandleRef handle);

    [DllImport("Kernel32", CharSet = CharSet.Auto)]
    public static extern IntPtr GlobalFree(HandleRef handle);

    [DllImport("Kernel32", CharSet = CharSet.Auto)]
    public static extern int GlobalSize(HandleRef handle);

    [DllImport("Kernel32", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
    public static extern void CopyMemoryW(IntPtr pdst, string psrc, int cb);

    [DllImport("Kernel32", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
    public static extern void CopyMemoryW(IntPtr pdst, char[] psrc, int cb);

    [DllImport("Kernel32", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
    public static extern void CopyMemoryW(StringBuilder pdst, HandleRef psrc, int cb);

    [DllImport("Kernel32", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Unicode)]
    public static extern void CopyMemoryW(char[] pdst, HandleRef psrc, int cb);

    [DllImport("Kernel32", EntryPoint = "RtlMoveMemory")]
    public static extern void CopyMemory(IntPtr pdst, byte[] psrc, int cb);

    [DllImport("Kernel32", EntryPoint = "RtlMoveMemory")]
    public static extern void CopyMemory(byte[] pdst, HandleRef psrc, int cb);

    [DllImport("Kernel32", EntryPoint = "RtlMoveMemory")]
    public static extern void CopyMemory(IntPtr pdst, HandleRef psrc, int cb);

    [DllImport("Kernel32", EntryPoint = "RtlMoveMemory")]
    public static extern void CopyMemory(IntPtr pdst, string psrc, int cb);

    [DllImport("Kernel32", CharSet = CharSet.Unicode)]
    public static extern int WideCharToMultiByte(
      int codePage,
      int flags,
      [MarshalAs(UnmanagedType.LPWStr)] string wideStr,
      int chars,
      [In, Out] byte[] pOutBytes,
      int bufferBytes,
      IntPtr defaultChar,
      IntPtr pDefaultUsed);

    [DllImport("user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsDialogMessageA(IntPtr hDlg, ref Microsoft.VisualStudio.OLE.Interop.MSG msg);

    [DllImport("user32.dll")]
    public static extern void SetFocus(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern IntPtr GetFocus();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool InvalidateRect(IntPtr hwnd, IntPtr rect, bool erase);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetClientRect(IntPtr hWnd, ref NativeMethods.RECT rect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(IntPtr hwnd, out NativeMethods.RECT lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(
      IntPtr hWnd,
      int Msg,
      IntPtr wParam,
      IntPtr lParam);

    [DllImport("UXTheme.dll", CharSet = CharSet.Unicode)]
    public static extern int SetWindowTheme(IntPtr hWnd, string subAppName, string subIdList);

    [DllImport("oleaut32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SafeArrayCreate(
      VarEnum vt,
      uint cDims,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] SAFEARRAYBOUND[] rgsabound);

    [DllImport("oleaut32.dll", CharSet = CharSet.Auto, PreserveSig = false)]
    public static extern void SafeArrayPutElement(IntPtr psa, [MarshalAs(UnmanagedType.LPArray)] long[] rgIndices, IntPtr pv);

    public static bool IsSameComObject(object obj1, object obj2)
    {
      IntPtr pUnk1 = IntPtr.Zero;
      IntPtr pUnk2 = IntPtr.Zero;
      try
      {
        if (obj1 != null)
          pUnk1 = Marshal.GetIUnknownForObject(obj1);
        if (obj2 != null)
          pUnk2 = Marshal.GetIUnknownForObject(obj2);
        return pUnk1 == pUnk2;
      }
      finally
      {
        if (pUnk1 != IntPtr.Zero)
          Marshal.Release(pUnk1);
        if (pUnk2 != IntPtr.Zero)
          Marshal.Release(pUnk2);
      }
    }

    public static int TARGETFRAMEWORKVERSION_MAJOR(uint version)
    {
      return (int) (version >> 16) & (int) ushort.MaxValue;
    }

    public static int TARGETFRAMEWORKVERSION_MINOR(uint version)
    {
      return (int) version & (int) byte.MaxValue;
    }

    public static int TARGETFRAMEWORKVERSION_REVISION(uint version)
    {
      return (int) (version >> 8) & (int) byte.MaxValue;
    }

    [DllImport("ole32.dll")]
    internal static extern int CoRegisterMessageFilter(HandleRef newFilter, ref IntPtr oldMsgFilter);

    [DllImport("gdi32.dll")]
    internal static extern int GetObject(IntPtr hGdiObj, int cbBuffer, IntPtr lpvObject);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyIcon(IntPtr hIcon);

    [DllImport("comctl32.dll")]
    internal static extern int ImageList_GetImageCount(IntPtr himl);

    [DllImport("comctl32.dll")]
    internal static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, uint flags);

    [DllImport("comctl32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ImageList_GetImageInfo(
      IntPtr himl,
      int i,
      out NativeMethods.IMAGEINFO pImageInfo);

    [DllImport("comctl32.dll")]
    internal static extern int ImageList_GetBkColor(IntPtr himl);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool EnumDisplayMonitors(
      IntPtr hdc,
      IntPtr lprcClip,
      NativeMethods.EnumMonitorsDelegate lpfnEnum,
      IntPtr dwData);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IntersectRect(
      out NativeMethods.RECT lprcDst,
      [In] ref NativeMethods.RECT lprcSrc1,
      [In] ref NativeMethods.RECT lprcSrc2);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetMonitorInfo(
      IntPtr hMonitor,
      ref NativeMethods.MONITORINFO monitorInfo);

    internal static void FindMaximumSingleMonitorRectangle(
      NativeMethods.RECT windowRect,
      out NativeMethods.RECT screenSubRect,
      out NativeMethods.RECT monitorRect)
    {
      List<NativeMethods.RECT> rects = new List<NativeMethods.RECT>();
      NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (NativeMethods.EnumMonitorsDelegate) ((IntPtr hMonitor, IntPtr hdcMonitor, ref NativeMethods.RECT rect, IntPtr lpData) =>
      {
        NativeMethods.MONITORINFO monitorInfo = new NativeMethods.MONITORINFO();
        monitorInfo.cbSize = (uint) Marshal.SizeOf(typeof (NativeMethods.MONITORINFO));
        NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);
        rects.Add(monitorInfo.rcWork);
        return true;
      }), IntPtr.Zero);
      long num1 = 0;
      screenSubRect = new NativeMethods.RECT()
      {
        Left = 0,
        Right = 0,
        Top = 0,
        Bottom = 0
      };
      monitorRect = new NativeMethods.RECT()
      {
        Left = 0,
        Right = 0,
        Top = 0,
        Bottom = 0
      };
      foreach (NativeMethods.RECT rect in rects)
      {
        NativeMethods.RECT lprcSrc1 = rect;
        NativeMethods.RECT lprcDst;
        NativeMethods.IntersectRect(out lprcDst, ref lprcSrc1, ref windowRect);
        long num2 = (long) (lprcDst.Width * lprcDst.Height);
        if (num2 > num1)
        {
          screenSubRect = lprcDst;
          monitorRect = rect;
          num1 = num2;
        }
      }
    }

    internal static void FindMaximumSingleMonitorRectangle(
      Rect windowRect,
      out Rect screenSubRect,
      out Rect monitorRect)
    {
      NativeMethods.RECT screenSubRect1;
      NativeMethods.RECT monitorRect1;
      NativeMethods.FindMaximumSingleMonitorRectangle(new NativeMethods.RECT(windowRect), out screenSubRect1, out monitorRect1);
      screenSubRect = new Rect(screenSubRect1.Position, screenSubRect1.Size);
      monitorRect = new Rect(monitorRect1.Position, monitorRect1.Size);
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowInfo(IntPtr hwnd, ref NativeMethods.WINDOWINFO pwi);

    [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern IntPtr GetDC(IntPtr hWnd);

    internal static IntPtr GetDC(HandleRef hWnd)
    {
      return NativeMethods.GetDC(hWnd.Handle);
    }

    [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    internal static int ReleaseDC(HandleRef hWnd, HandleRef hDC)
    {
      return NativeMethods.ReleaseDC(hWnd.Handle, hDC.Handle);
    }

    [DllImport("Gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteDC(IntPtr hdc);

    internal static bool DeleteDC(HandleRef hdc)
    {
      return NativeMethods.DeleteDC(hdc.Handle);
    }

    [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
    internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetKeyboardState(byte[] lpKeyState);

    [DllImport("user32.dll")]
    internal static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    internal static extern short GetKeyState(int vKey);

    internal static bool IsKeyPressed(int vKey)
    {
      return NativeMethods.GetKeyState(vKey) < (short) 0;
    }

    internal static bool IsLeftButtonPressed()
    {
      return NativeMethods.IsKeyPressed(1);
    }

    internal static bool IsRightButtonPressed()
    {
      return NativeMethods.IsKeyPressed(2);
    }

    internal static ModifierKeys ModifierKeys
    {
      get
      {
        byte[] lpKeyState = new byte[256];
        NativeMethods.GetKeyboardState(lpKeyState);
        ModifierKeys modifierKeys = ModifierKeys.None;
        if (((int) lpKeyState[16] & 128) == 128)
          modifierKeys |= ModifierKeys.Shift;
        if (((int) lpKeyState[17] & 128) == 128)
          modifierKeys |= ModifierKeys.Control;
        if (((int) lpKeyState[18] & 128) == 128)
          modifierKeys |= ModifierKeys.Alt;
        if (((int) lpKeyState[91] & 128) == 128 || ((int) lpKeyState[92] & 128) == 128)
          modifierKeys |= ModifierKeys.Windows;
        return modifierKeys;
      }
    }

    [Guid("5EFC7974-14BC-11CF-9B2B-00AA00573819")]
    [ComImport]
    public class OleComponentUIManager
    {
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      public extern OleComponentUIManager();
    }

    [Guid("8E7B96A8-E33D-11D0-A6D5-00C04FB67F6A")]
    [ComImport]
    public class VsTextBuffer
    {
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      public extern VsTextBuffer();
    }

    public enum VSTASKBITMAP
    {
      BMP_USER = -5,
      BMP_SHORTCUT = -4,
      BMP_COMMENT = -3,
      BMP_SQUIGGLE = -2,
      BMP_COMPILE = -1,
    }

    public enum VsUIHierarchyWindowCmdIds
    {
      UIHWCMDID_RightClick = 1,
      UIHWCMDID_DoubleClick = 2,
      UIHWCMDID_EnterKey = 3,
      UIHWCMDID_StartLabelEdit = 4,
      UIHWCMDID_CommitLabelEdit = 5,
      UIHWCMDID_CancelLabelEdit = 6,
    }

    public enum VSSELELEMID
    {
      SEID_UndoManager,
      SEID_WindowFrame,
      SEID_DocumentFrame,
      SEID_StartupProject,
      SEID_PropertyBrowserSID,
      SEID_UserContext,
      SEID_ResultList,
      SEID_LastWindowFrame,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class LOGFONT
    {
      public int lfHeight;
      public int lfWidth;
      public int lfEscapement;
      public int lfOrientation;
      public int lfWeight;
      public byte lfItalic;
      public byte lfUnderline;
      public byte lfStrikeOut;
      public byte lfCharSet;
      public byte lfOutPrecision;
      public byte lfClipPrecision;
      public byte lfQuality;
      public byte lfPitchAndFamily;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string lfFaceName;
    }

    public struct NMHDR
    {
      public IntPtr hwndFrom;
      public int idFrom;
      public int code;
    }

    public struct RECT
    {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;

      public RECT(int left, int top, int right, int bottom)
      {
        this.Left = left;
        this.Top = top;
        this.Right = right;
        this.Bottom = bottom;
      }

      public RECT(Rectangle r)
      {
        this.Left = r.Left;
        this.Top = r.Top;
        this.Right = r.Right;
        this.Bottom = r.Bottom;
      }

      public RECT(Rect rect)
      {
        this.Left = (int) rect.Left;
        this.Top = (int) rect.Top;
        this.Right = (int) rect.Right;
        this.Bottom = (int) rect.Bottom;
      }

      public void Offset(int dx, int dy)
      {
        this.Left += dx;
        this.Right += dx;
        this.Top += dy;
        this.Bottom += dy;
      }

      public System.Windows.Point Position
      {
        get
        {
          return new System.Windows.Point((double) this.Left, (double) this.Top);
        }
      }

      public System.Windows.Size Size
      {
        get
        {
          return new System.Windows.Size((double) this.Width, (double) this.Height);
        }
      }

      public int Height
      {
        get
        {
          return this.Bottom - this.Top;
        }
        set
        {
          this.Bottom = this.Top + value;
        }
      }

      public int Width
      {
        get
        {
          return this.Right - this.Left;
        }
        set
        {
          this.Right = this.Left + value;
        }
      }
    }

    public struct MONITORINFO
    {
      public uint cbSize;
      public NativeMethods.RECT rcMonitor;
      public NativeMethods.RECT rcWork;
      public uint dwFlags;
    }

    public struct IMAGEINFO
    {
      public IntPtr hbmImage;
      public IntPtr hbmMask;
      public int Unused1;
      public int Unused2;
      public NativeMethods.RECT rcImage;
    }

    public struct WINDOWINFO
    {
      public int cbSize;
      public NativeMethods.RECT rcWindow;
      public NativeMethods.RECT rcClient;
      public int dwStyle;
      public int dwExStyle;
      public uint dwWindowStatus;
      public uint cxWindowBorders;
      public uint cyWindowBorders;
      public ushort atomWindowType;
      public ushort wCreatorVersion;
    }

    public struct BITMAP
    {
      public int bmType;
      public int bmWidth;
      public int bmHeight;
      public int bmWidthBytes;
      public ushort bmPlanes;
      public ushort bmBitsPixel;
      public IntPtr bmBits;
    }

    public struct COLORREF
    {
      public uint dwColor;

      public COLORREF(uint dwColor)
      {
        this.dwColor = dwColor;
      }

      public COLORREF(System.Windows.Media.Color color)
      {
        this.dwColor = (uint) ((int) color.R + ((int) color.G << 8) + ((int) color.B << 16));
      }

      public System.Windows.Media.Color GetMediaColor()
      {
        return System.Windows.Media.Color.FromRgb((byte) ((uint) byte.MaxValue & this.dwColor), (byte) ((65280U & this.dwColor) >> 8), (byte) ((16711680U & this.dwColor) >> 16));
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class POINT
    {
      public int x;
      public int y;

      public POINT()
      {
      }

      public POINT(int x, int y)
      {
        this.x = x;
        this.y = y;
      }
    }

    public sealed class OLECMDTEXT
    {
      private OLECMDTEXT()
      {
      }

      public static NativeMethods.OLECMDTEXT.OLECMDTEXTF GetFlags(IntPtr pCmdTextInt)
      {
        Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT structure = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT) Marshal.PtrToStructure(pCmdTextInt, typeof (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));
        if (((int) structure.cmdtextf & 1) != 0)
          return NativeMethods.OLECMDTEXT.OLECMDTEXTF.OLECMDTEXTF_NAME;
        return ((int) structure.cmdtextf & 2) != 0 ? NativeMethods.OLECMDTEXT.OLECMDTEXTF.OLECMDTEXTF_STATUS : NativeMethods.OLECMDTEXT.OLECMDTEXTF.OLECMDTEXTF_NONE;
      }

      public static string GetText(IntPtr pCmdTextInt)
      {
        Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT structure = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT) Marshal.PtrToStructure(pCmdTextInt, typeof (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));
        IntPtr num = Marshal.OffsetOf(typeof (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "rgwz");
        if (structure.cwActual == 0U)
          return string.Empty;
        char[] destination = new char[(IntPtr) (structure.cwActual - 1U)];
        Marshal.Copy((IntPtr) ((long) pCmdTextInt + (long) num), destination, 0, destination.Length);
        StringBuilder stringBuilder = new StringBuilder(destination.Length);
        stringBuilder.Append(destination);
        return stringBuilder.ToString();
      }

      public static void SetText(IntPtr pCmdTextInt, string text)
      {
        Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT structure = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT) Marshal.PtrToStructure(pCmdTextInt, typeof (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));
        char[] charArray = text.ToCharArray();
        IntPtr num1 = Marshal.OffsetOf(typeof (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "rgwz");
        IntPtr num2 = Marshal.OffsetOf(typeof (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "cwActual");
        int length = Math.Min((int) structure.cwBuf - 1, charArray.Length);
        Marshal.Copy(charArray, 0, (IntPtr) ((long) pCmdTextInt + (long) num1), length);
        Marshal.WriteInt16((IntPtr) ((long) pCmdTextInt + (long) num1 + (long) (length * 2)), (short) 0);
        Marshal.WriteInt32((IntPtr) ((long) pCmdTextInt + (long) num2), length + 1);
      }

      public enum OLECMDTEXTF
      {
        OLECMDTEXTF_NONE,
        OLECMDTEXTF_NAME,
        OLECMDTEXTF_STATUS,
      }
    }

    public enum tagOLECMDF
    {
      OLECMDF_SUPPORTED = 1,
      OLECMDF_ENABLED = 2,
      OLECMDF_LATCHED = 4,
      OLECMDF_NINCHED = 8,
      OLECMDF_INVISIBLE = 16, // 0x00000010
    }

    public sealed class StreamConsts
    {
      public const int LOCK_WRITE = 1;
      public const int LOCK_EXCLUSIVE = 2;
      public const int LOCK_ONLYONCE = 4;
      public const int STATFLAG_DEFAULT = 0;
      public const int STATFLAG_NONAME = 1;
      public const int STATFLAG_NOOPEN = 2;
      public const int STGC_DEFAULT = 0;
      public const int STGC_OVERWRITE = 1;
      public const int STGC_ONLYIFCURRENT = 2;
      public const int STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 4;
      public const int STREAM_SEEK_SET = 0;
      public const int STREAM_SEEK_CUR = 1;
      public const int STREAM_SEEK_END = 2;
    }

    internal sealed class DataStreamFromComStream : Stream, IDisposable
    {
      private Microsoft.VisualStudio.OLE.Interop.IStream comStream;

      public DataStreamFromComStream(Microsoft.VisualStudio.OLE.Interop.IStream comStream)
      {
        this.comStream = comStream;
      }

      public override long Position
      {
        get
        {
          return this.Seek(0L, SeekOrigin.Current);
        }
        set
        {
          this.Seek(value, SeekOrigin.Begin);
        }
      }

      public override bool CanWrite
      {
        get
        {
          return true;
        }
      }

      public override bool CanSeek
      {
        get
        {
          return true;
        }
      }

      public override bool CanRead
      {
        get
        {
          return true;
        }
      }

      public override long Length
      {
        get
        {
          long position = this.Position;
          long num = this.Seek(0L, SeekOrigin.End);
          this.Position = position;
          return num - position;
        }
      }

      private void _NotImpl(string message)
      {
        throw new NotSupportedException(message, (Exception) new ExternalException(string.Empty, -2147467263));
      }

      protected override void Dispose(bool disposing)
      {
        try
        {
          if (disposing && this.comStream != null)
            this.Flush();
          this.comStream = (Microsoft.VisualStudio.OLE.Interop.IStream) null;
        }
        finally
        {
          base.Dispose(disposing);
        }
      }

      public override void Flush()
      {
        if (this.comStream == null)
          return;
        try
        {
          this.comStream.Commit(0U);
        }
        catch
        {
        }
      }

      public override int Read(byte[] buffer, int index, int count)
      {
        byte[] pv = buffer;
        if (index != 0)
        {
          pv = new byte[buffer.Length - index];
          buffer.CopyTo((Array) pv, 0);
        }
        uint pcbRead;
        this.comStream.Read(pv, (uint) count, out pcbRead);
        if (index != 0)
          pv.CopyTo((Array) buffer, index);
        return (int) pcbRead;
      }

      public override void SetLength(long value)
      {
        this.comStream.SetSize(new ULARGE_INTEGER()
        {
          QuadPart = (ulong) value
        });
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        LARGE_INTEGER dlibMove = new LARGE_INTEGER();
        ULARGE_INTEGER[] plibNewPosition = new ULARGE_INTEGER[1]
        {
          new ULARGE_INTEGER()
        };
        dlibMove.QuadPart = offset;
        this.comStream.Seek(dlibMove, (uint) origin, plibNewPosition);
        return (long) plibNewPosition[0].QuadPart;
      }

      public override void Write(byte[] buffer, int index, int count)
      {
        if (count <= 0)
          return;
        byte[] pv = buffer;
        if (index != 0)
        {
          pv = new byte[buffer.Length - index];
          buffer.CopyTo((Array) pv, 0);
        }
        uint pcbWritten;
        this.comStream.Write(pv, (uint) count, out pcbWritten);
        if ((long) pcbWritten != (long) count)
          throw new IOException();
        if (index == 0)
          return;
        pv.CopyTo((Array) buffer, index);
      }

      ~DataStreamFromComStream()
      {
      }
    }

    public sealed class ConnectionPointCookie : IDisposable
    {
      private Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer cpc;
      private Microsoft.VisualStudio.OLE.Interop.IConnectionPoint connectionPoint;
      private uint cookie;

      public ConnectionPointCookie(object source, object sink, System.Type eventInterface)
        : this(source, sink, eventInterface, true)
      {
      }

      ~ConnectionPointCookie()
      {
        this.Dispose(false);
      }

      public void Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }

      private void Dispose(bool disposing)
      {
        if (!disposing)
          return;
        try
        {
          if (this.connectionPoint == null || this.cookie == 0U)
            return;
          this.connectionPoint.Unadvise(this.cookie);
        }
        finally
        {
          this.cookie = 0U;
          this.connectionPoint = (Microsoft.VisualStudio.OLE.Interop.IConnectionPoint) null;
          this.cpc = (Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer) null;
        }
      }

      public ConnectionPointCookie(
        object source,
        object sink,
        System.Type eventInterface,
        bool throwException)
      {
        Exception exception = (Exception) null;
        Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer connectionPointContainer = source as Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer;
        if (connectionPointContainer != null)
        {
          this.cpc = connectionPointContainer;
          try
          {
            Guid guid = eventInterface.GUID;
            this.cpc.FindConnectionPoint(ref guid, out this.connectionPoint);
          }
          catch
          {
            this.connectionPoint = (Microsoft.VisualStudio.OLE.Interop.IConnectionPoint) null;
          }
          if (this.connectionPoint == null)
          {
            exception = (Exception) new ArgumentException();
          }
          else
          {
            if (sink != null)
            {
              if (eventInterface.IsInstanceOfType(sink))
              {
                try
                {
                  this.connectionPoint.Advise(sink, out this.cookie);
                  goto label_12;
                }
                catch
                {
                  this.cookie = 0U;
                  this.connectionPoint = (Microsoft.VisualStudio.OLE.Interop.IConnectionPoint) null;
                  exception = new Exception();
                  goto label_12;
                }
              }
            }
            exception = (Exception) new InvalidCastException();
          }
        }
        else
          exception = (Exception) new InvalidCastException();
label_12:
        if (!throwException || this.connectionPoint != null && this.cookie != 0U)
          return;
        if (exception == null)
          throw new ArgumentException();
        throw exception;
      }
    }

    [Guid("9BDA66AE-CA28-4e22-AA27-8A7218A0E3FA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IEventHandler
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      int AddHandler(string bstrEventName);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int RemoveHandler(string bstrEventName);

      IVsEnumBSTR GetHandledEvents();

      bool HandlesEvent(string bstrEventName);
    }

    [Guid("A55CCBCC-7031-432d-B30A-A68DE7BDAD75")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IParameterKind
    {
      void SetParameterPassingMode(
        NativeMethods.PARAMETER_PASSING_MODE ParamPassingMode);

      void SetParameterArrayDimensions(int uDimensions);

      int GetParameterArrayCount();

      int GetParameterArrayDimensions(int uIndex);

      int GetParameterPassingMode();
    }

    public enum PARAMETER_PASSING_MODE
    {
      cmParameterTypeIn = 1,
      cmParameterTypeOut = 2,
      cmParameterTypeInOut = 3,
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("3E596484-D2E4-461a-A876-254C4F097EBB")]
    [ComVisible(true)]
    [ComImport]
    public interface IMethodXML
    {
      void GetXML(ref string pbstrXML);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int SetXML(string pszXML);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetBodyPoint([MarshalAs(UnmanagedType.Interface)] out object bodyPoint);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("58A26C00-BE6F-4B32-803A-98F5B7806C76")]
    [ComImport]
    internal interface IMethodXML2
    {
      void GetXML([MarshalAs(UnmanagedType.IUnknown)] ref object stringReader);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("EA1A87AD-7BC5-4349-B3BE-CADC301F17A3")]
    [ComImport]
    public interface IVBFileCodeModelEvents
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      int StartEdit();

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int EndEdit();
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("23BBD58A-7C59-449b-A93C-43E59EFC080C")]
    [ComImport]
    public interface ICodeClassBase
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      int GetBaseName(out string pBaseName);
    }

    [return: MarshalAs(UnmanagedType.Bool)]
    internal delegate bool EnumMonitorsDelegate(
      IntPtr hMonitor,
      IntPtr hdcMonitor,
      ref NativeMethods.RECT lprcMonitor,
      IntPtr dwData);
  }
}
