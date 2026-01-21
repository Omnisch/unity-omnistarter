// author: Omnistudio
// version: 2026.01.21

using System;
using UnityEngine;

namespace Omnis.Utils
{
    public static class LogHelper
    {
        public static void LogInfo(string message, Action<string, LogLevel> upstream = null)
            => Log(message, LogLevel.Info, upstream);

        public static void LogWarning(string message, Action<string, LogLevel> upstream = null)
            => Log(message, LogLevel.Warning, upstream);

        public static string LogError(string message, Action<string, LogLevel> upstream = null) {
            Log(message, LogLevel.Error, upstream);
            return message;
        }

        public static void Log(string message, LogLevel logLevel = LogLevel.Info, Action<string, LogLevel> upstream = null) {
            switch (logLevel) {
                case LogLevel.Info:
                    Debug.Log(message);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogLevel.Error:
                    Debug.LogError(message);
                    break;
            }
            upstream?.Invoke(message, logLevel);
        }
    }

    public enum LogLevel { Info, Warning, Error }
}
