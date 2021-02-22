using BepInEx;
using BepInEx.Logging;
using System;
using VRGIN.Core;
using VRGIN.Helpers;

namespace IllusionVR.Core
{
    public class IllusionVRCore : BaseUnityPlugin
    {
        protected virtual void Awake()
        {
            IVRLog.SetLogger(Logger);
            VRLog.LogCall += (x, y) => IVRLog.Log(ConvertLogLevel(y), x);

            bool vrDeactivated = Environment.CommandLine.Contains("--novr");
            bool vrActivated = Environment.CommandLine.Contains("--vr");

            if(vrActivated || (!vrDeactivated && SteamVRDetector.IsRunning))
                VRLoader.Create(true);
            else
                VRLoader.Create(false);
        }

        private static LogLevel ConvertLogLevel(VRLog.LogMode logMode)
        {
            switch(logMode)
            {
                case VRLog.LogMode.Debug: return LogLevel.Debug;
                case VRLog.LogMode.Error: return LogLevel.Error;
                case VRLog.LogMode.Info: return LogLevel.Info;
                case VRLog.LogMode.Warning: return LogLevel.Warning;
                default: return LogLevel.Debug;
            }
        }
    }
}
