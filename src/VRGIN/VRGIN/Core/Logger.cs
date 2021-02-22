using System;
using System.Diagnostics;

namespace VRGIN.Core
{
    public static class VRLog
    {
        public static Action<string, LogMode> LogCall;

        public enum LogMode
        {
            Debug,
            Info,
            Warning,
            Error
        }

        public static void Debug(string text, params object[] args)
        {
            Log(text, args, LogMode.Debug);
        }

        public static void Info(string text, params object[] args)
        {
            Log(text, args, LogMode.Info);
        }

        public static void Warn(string text, params object[] args)
        {
            Log(text, args, LogMode.Warning);
        }

        public static void Error(string text, params object[] args)
        {
            Log(text, args, LogMode.Error);
        }

        public static void Debug(object obj)
        {
            Log("{0}", new[] { obj }, LogMode.Debug);
        }

        public static void Info(object obj)
        {
            Log("{0}", new[] { obj }, LogMode.Info);
        }

        public static void Warn(object obj)
        {
            Log("{0}", new[] { obj }, LogMode.Warning);
        }

        public static void Error(object obj)
        {
            Log("{0}", new[] { obj }, LogMode.Error);
        }

        private static void Log(string text, object[] args, LogMode severity)
        {
            LogCall?.Invoke(string.Format(Format(text, severity), args), severity);
        }

        private static string Format(string text, LogMode mode)
        {
            var trace = new StackTrace(3);
            var caller = trace.GetFrame(0);
            return string.Format("[{1}#{2}] {0}", text, caller.GetMethod().DeclaringType.Name, caller.GetMethod().Name, caller.GetFileLineNumber());
        }
    }
}
