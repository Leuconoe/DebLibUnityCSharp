using DebLib;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class Debuger
{
    private static bool isDebugMode = true;

    public static void TimeLog(object message)
    {
        if (isDebugMode)
        {

            Debug.Log(string.Format("##DEBUG##:{0:#0.##} : {1}", Time.realtimeSinceStartup, message.ToString()));
        }
        else
        {
            CollectCrashReport(message);
        }
    }

    public static void Log(object message)
    {
        if (isDebugMode)
        {
            Debug.Log(DebUtil.CombineString("##DEBUG##:", message.ToString()));
        }
        else
        {
            CollectCrashReport(message);
        }
    }

    public static void Log(object message, UnityEngine.Object context)
    {
        if (isDebugMode)
        {
            Debug.Log(DebUtil.CombineString("##DEBUG##:", message.ToString()), context);
        }
        else
        {
            CollectCrashReport(message);
        }
    }

    public static void LogWarning(object message)
    {
        if (isDebugMode)
        {
            Debug.LogWarning(DebUtil.CombineString("##DEBUG##:", message.ToString()));
        }
        else
        {
            CollectCrashReport(message);
        }
    }

    public static void LogWarning(object message, UnityEngine.Object context)
    {
        if (isDebugMode)
        {
            Debug.LogWarning(DebUtil.CombineString("##DEBUG##:", message.ToString()), context);
        }
        else
        {
            CollectCrashReport(message);
        }
    }

    public static void LogError(object message)
    {
        if (isDebugMode)
        {
            Debug.LogError(DebUtil.CombineString("##DEBUG##:", message.ToString()));
        }
        else
        {
            CollectCrashReport(message);
        }
    }

    public static void LogError(object message, UnityEngine.Object context)
    {
        if (isDebugMode)
        {
            Debug.LogError(DebUtil.CombineString("##DEBUG##:", message.ToString()), context);
        }
        else
        {
            CollectCrashReport(message);
        }
    }

    public static void CollectCrashReport(object message)
    {
        /*
#if UNITY_EDITOR
        return;
#endif
        string msg = message.ToString();
        if (SDKManager.Instance != null && SDKManager.Instance.CrashReporter != null)
        {
            SDKManager.Instance.CrashReporter.HandleException(msg, UnityEngine.StackTraceUtility.ExtractStackTrace(), LogType.Log);
        }
        */
    }
}