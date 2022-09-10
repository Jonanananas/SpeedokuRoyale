using System.Diagnostics;

using UnityEngine;
public static class Trace {
    [Range(1, 3)]
    public static int LoggingLevel = 3;
    [Conditional("ENABLE_LOGS")]
    public static void LogError(string logText) {
        UnityEngine.Debug.LogError(logText);
    }
    [Conditional("ENABLE_LOGS")]
    public static void LogWarning(string logText) {
        UnityEngine.Debug.LogWarning(logText);
    }
    [Conditional("ENABLE_LOGS")]
    public static void Log(string logText) {
        UnityEngine.Debug.Log(logText);
    }
}
