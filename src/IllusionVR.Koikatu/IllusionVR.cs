using BepInEx;
using BepInEx.Logging;
using System;
using UnityEngine;
using VRGIN.Helpers;

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
            DefaultSkybox = RenderSettings.skybox;

            bool vrDeactivated = Environment.CommandLine.Contains("--novr");
            bool vrActivated = Environment.CommandLine.Contains("--vr");

            if(vrActivated || (!vrDeactivated && SteamVRDetector.IsRunning))
                VRLoader.Create(true);
            else
                VRLoader.Create(false);
        }
    }
}
