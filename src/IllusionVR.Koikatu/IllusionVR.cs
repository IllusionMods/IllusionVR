using BepInEx;
using BepInEx.Logging;
using System;
using UnityEngine;
using VRGIN.Helpers;
using VRGIN.Core;

namespace IllusionVR.Koikatu
{
    [BepInPlugin("keelhauled.illusionvr.koikatu", "IllusionVR", "1.0.0")]
    public class IllusionVR : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        public static Material DefaultSkybox;

        private void Awake()
        {
            Logger = base.Logger;
            VRLog.logCall += (x, y) => Logger.Log(ConvertLogLevel(y), x);

            DefaultSkybox = RenderSettings.skybox;

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
