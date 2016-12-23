using System.Collections.Generic;

namespace AttachToolbar
{
    internal static class State
    {
        public static string ProcessName = "";
        public static List<string> ProcessList = new List<string>();
        public static EngineType EngineType = EngineType.Native;
        public static bool IsAttached = false;

        public static void Clear()
        {
            ProcessList.Clear();
            EngineType = EngineType.Native;
            IsAttached = false;
            ProcessName = "";
        }
    }
}
