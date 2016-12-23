using System.Collections.Generic;

namespace AttachToolbar
{
    internal static class State
    {
        public static int ProcessIndex = -1;
        public static List<string> ProcessList = new List<string>();
        public static EngineType EngineType = EngineType.Native;
        public static bool IsAttached = false;

        public static string ProcessName
        {
            get
            {
                string processName = ProcessIndex >= 0
                        ? ProcessList[ProcessIndex]
                        : "";
                return processName;
            }

            set
            {
                ProcessIndex = ProcessList.IndexOf(value);
            }
        }

        public static void Clear()
        {
            ProcessList.Clear();
            EngineType = EngineType.Native;
            IsAttached = false;
            ProcessIndex = -1;
        }
    }
}
