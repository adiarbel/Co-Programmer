// Guids.cs
// MUST match guids.h
using System;

namespace Company.VSPackage1
{
    static class GuidList
    {
        public const string guidVSPackage1PkgString = "0d8b7db9-221f-4434-9594-2c8caefc7625";
        public const string guidVSPackage1CmdSetString = "43891a24-d81c-4e70-98b1-386c3957f6cd";
        public const string guidButtonGroupCmdSetString = "f69209e9-975a-4543-821d-1f4a2c52d737";
        public const string guidTopLevelMenuCmdSetString = "2feb817a-40d9-4779-bf51-9bf22725b04d";
        public static readonly Guid guidVSPackage1CmdSet = new Guid(guidVSPackage1CmdSetString);
        public static readonly Guid guidButtonGroupCmdSet = new Guid(guidButtonGroupCmdSetString);
        public static readonly Guid guidTopLevelMenuCmdSet = new Guid(guidTopLevelMenuCmdSetString);
    };
}