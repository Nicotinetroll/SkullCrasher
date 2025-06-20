using UnityEngine;

namespace PalbaGames
{
    /// <summary>
    /// Global debug logger for PalbaGames. Use to toggle debug logs easily.
    /// </summary>
    public static class DebugLogger
    {
        public static bool EnableLogs = true;

        public static void Log(object message)
        {
            if (EnableLogs)
                Debug.Log($"[PalbaGames] {message}");
        }

        public static void LogWarning(object message)
        {
            if (EnableLogs)
                Debug.LogWarning($"[PalbaGames] {message}");
        }

        public static void LogError(object message)
        {
            if (EnableLogs)
                Debug.LogError($"[PalbaGames] {message}");
        }
    }
}