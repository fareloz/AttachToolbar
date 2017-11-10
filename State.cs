using System.Collections.Generic;

namespace AttachToolbar
{
    internal static class State
    {
        public static void Clear()
        {
            ProcessList.Clear();
            EngineType = EngineType.Native;
            ProcessIndex = -1;
        }
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


        public static int ProcessIndex = -1;
        public static List<string> ProcessList = new List<string>();
        public static EngineType EngineType = EngineType.Native;
        public static SettingsManager Settings;
    }
}
