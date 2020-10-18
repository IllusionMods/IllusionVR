using System;
using System.Diagnostics;

namespace VRGIN.Core
{
    public class VRLog
    {
        public static Action<string, LogMode> logCall;

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
            Log("{0}", new object[] { obj }, LogMode.Debug);
        }

        public static void Info(object obj)
        {
            Log("{0}", new object[] { obj }, LogMode.Info);
        }

        public static void Warn(object obj)
        {
            Log("{0}", new object[] { obj }, LogMode.Warning);
        }

        public static void Error(object obj)
        {
            Log("{0}", new object[] { obj }, LogMode.Error);
        }

        public static void Log(string text, object[] args, LogMode severity)
        {
            logCall?.Invoke(string.Format(Format(text, severity), args), severity);
        }

        private static String Format(string text, LogMode mode)
        {
            var trace = new StackTrace(3);
            var caller = trace.GetFrame(0);
            return String.Format("[{0}][{1}][{3}#{4}] {2}", DateTime.Now.ToString("HH':'mm':'ss"), mode.ToString().ToUpper(), text, caller.GetMethod().DeclaringType.Name, caller.GetMethod().Name, caller.GetFileLineNumber());
        }
    }
}
