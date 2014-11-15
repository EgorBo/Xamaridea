// Guids.cs
// MUST match guids.h
using System;

namespace EgorBo.Xamaridea_VisualStudioPlugin
{
    static class GuidList
    {
        public const string guidXamaridea_VisualStudioPluginPkgString = "c3535449-011a-480d-b116-f2be70bf4d15";
        public const string guidXamaridea_VisualStudioPluginCmdSetString = "e4c7825f-a380-451a-94d8-9212bd49099d";
        public const string guidToolWindowPersistanceString = "c04033dc-fa25-4ece-85d3-8ed65d600602";

        public static readonly Guid guidXamaridea_VisualStudioPluginCmdSet = new Guid(guidXamaridea_VisualStudioPluginCmdSetString);
    };
}