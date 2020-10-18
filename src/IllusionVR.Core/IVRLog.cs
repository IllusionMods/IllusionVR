using BepInEx.Logging;

namespace IllusionVR.Core
{
    public static class IVRLog
    {
        private static ManualLogSource logger;

        public static void SetLogger(ManualLogSource logger)
        {
            IVRLog.logger = logger;
        }

        public static void Log(LogLevel level, object data)
        {
            logger.Log(level, data);
        }

        public static void LogInfo(object data)
        {
            logger.LogInfo(data);
        }

        public static void LogError(object data)
        {
            logger.LogError(data);
        }

        public static void LogWarning(object data)
        {
            logger.LogWarning(data);
        }

        public static void LogDebug(object data)
        {
            logger.LogDebug(data);
        }

        public static void LogFatal(object data)
        {
            logger.LogFatal(data);
        }

        public static void LogMessage(object data)
        {
            logger.LogMessage(data);
        }
    }
}
