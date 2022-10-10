using System.Diagnostics;

using UnityEngine;
public static class Trace {
    [Range(0, 3)]
    public static int LoggingLevel = 3;
    [Conditional("ENABLE_LOGS")]
    public static void LogError(string logText) {
        if (LoggingLevel >= 1)
            UnityEngine.Debug.LogError(logText);
    }
    [Conditional("ENABLE_LOGS")]
    public static void LogWarning(string logText) {
        if (LoggingLevel >= 2)
            UnityEngine.Debug.LogWarning(logText);
    }
    [Conditional("ENABLE_LOGS")]
    public static void Log(string logText) {
        if (LoggingLevel >= 3)
            UnityEngine.Debug.Log(logText);
    }
}
