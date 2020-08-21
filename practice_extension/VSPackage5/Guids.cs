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
    };
}