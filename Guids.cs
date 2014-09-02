using System;

namespace AttachManager
{
    static class GuidList
    {
        public const string guidAttachManagerPkgString = "00C35747-E7C6-411A-BB9C-D68ABCF83145";
        public const string guidAttachManagerCmdSetString = "886E472F-D4E6-47F2-AF41-8A9D38067890";

        public static readonly Guid guidAttachManagerCmdSet = new Guid(guidAttachManagerCmdSetString);
    };
}