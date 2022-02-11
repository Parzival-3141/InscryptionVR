using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Logging;

namespace InscryptionVR.Modules
{
    public static class Utils
    {
        public static void LogArray<T>(this ManualLogSource logger, string logTitle, IEnumerable<T> array, bool logIndex = false, LogLevel level = LogLevel.Info, Func<T, string> itemFormat = null)
        {
            logger.Log(level, $"--- {logTitle} ---");

            int num = 0;
            foreach (T item in array)
            {
                string n = logIndex ? $"[{num++}] " : "";

                if (itemFormat is null)
                    logger.Log(level, n + item.ToString());
                else
                    logger.Log(level, n + itemFormat.Invoke(item));
            }

            logger.Log(level, "--- end ---");
        }
    }
}
