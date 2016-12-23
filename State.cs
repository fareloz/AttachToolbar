using System.Collections.Generic;

namespace AttachToolbar
{
    public static class State
    {
        public static string ProcessName = "";
        public static List<string> ProcessList = new List<string>();
        public static AttachEngineType EngineType = AttachEngineType.Native;
        public static bool IsAttached = false;
    }
}
