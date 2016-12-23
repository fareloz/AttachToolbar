using System.Collections.Generic;

namespace AttachToolbar
{
    public static class State
    {
        public static string ProcessName = "";
        public static List<string> ProcessList = new List<string>();
        public static EngineType EngineType = EngineType.Native;
        public static bool IsAttached = false;
    }
}
