using BepInEx;
using BepInEx.Logging;
using IllusionVR.Core;
using System;
using VRGIN.Core;
using VRGIN.Helpers;

namespace IllusionVR.Koikatu
{
    [BepInPlugin("keelhauled.illusionvr.koikatu", "IllusionVR", "1.0.0")]
    public class KoikatuVR : BaseUnityPlugin
    {
        private void Awake()
        {
            IVRLog.SetLogger(Logger);
            VRLog.logCall += (x, y) => IVRLog.Log(ConvertLogLevel(y), x);

            bool vrDeactivated = Environment.CommandLine.Contains("--novr");
            bool vrActivated = Environment.CommandLine.Contains("--vr");

            if(vrActivated || (!vrDeactivated && SteamVRDetector.IsRunning))
                VRLoader.Create(true);
            else
                VRLoader.Create(false);
        }

        private LogLevel ConvertLogLevel(VRLog.LogMode logMode)
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
