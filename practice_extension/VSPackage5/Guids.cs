// Guids.cs
// MUST match guids.h
using System;

namespace Microsoft.SpecExplorer
{
    static class GuidList
    {
        public const string guidVSPackage5PkgString = "e7da5410-f3e0-4fdb-84e3-694089f5fc60";
        public const string guidVSPackage5CmdSetString = "820e91d0-56bc-443d-8b45-038e51aede16";

        public static readonly Guid guidVSPackage5CmdSet = new Guid(guidVSPackage5CmdSetString);

        public const string guidSpecExplorerPackageCmdSetString = "3a3e93b4-11bb-41f5-99d1-d2083ed60672";

        public static readonly Guid guidSpecExplorerPackageCmdSet = new Guid(guidSpecExplorerPackageCmdSetString);

        public static readonly Guid guidSpecExplorerPkg = new Guid("f9b9b97b-5213-4c39-b0df-9b44a2b97c58");
        public static readonly Guid guidSpecExplorerCmdSet = new Guid("3a28b737-dfc6-4f1e-ad96-9dcdbbdef171");
        public static readonly Guid guidEditorFactory = new Guid("04C7681D-A337-4705-8AD9-2206D31A9F7B");
        public static readonly Guid guidViewDocumentFactory = new Guid("B6259F13-EFC3-45ee-9BC6-3ACF05382B0C");
        public static readonly Guid guidSummaryDocumentFactory = new Guid("3C73969B-5878-4A18-B68B-0F1FD24094D3");
        public const string guidSpecExplorerPkgString = "f9b9b97b-5213-4c39-b0df-9b44a2b97c58";
        public const string guidSpecExplorerCmdSetString = "3a28b737-dfc6-4f1e-ad96-9dcdbbdef171";
        public const string guidStateComparisonViewWindowIdString = "C84398D1-E949-42f9-9BBB-C794D39E6361";
        public const string guidTaskListString = "{4a9b7e51-aa16-11d0-a8c5-00a0c921a4d2}";
        public const string guidEditorFactoryString = "04C7681D-A337-4705-8AD9-2206D31A9F7B";
        public const string guidViewDocumentFactoryString = "B6259F13-EFC3-45ee-9BC6-3ACF05382B0C";
        public const string guidSummaryDocumentFactoryString = "3C73969B-5878-4A18-B68B-0F1FD24094D3";
        public const string guidExplorationManagerToolWindowString = "6DC56C89-A22C-44a8-B43F-58AB60F25121";
        public const string guidBrowserToolWindowString = "1079EAE0-5880-4dc0-88FF-139EDF582BCA";
        public const string guidStepBrowserToolWindowString = "7E4F0150-06DA-4084-8F5C-A3A76A70E7D7";
        public const string guidWorkflowToolWindowString = "5AE58719-7142-4f61-A4C7-588FCF0B3C74";
    };
}