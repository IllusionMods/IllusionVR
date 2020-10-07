using BepInEx;
using System;
using UnityEngine;
using VRGIN.Helpers;

namespace IllusionVR.Koikatu
{
    [BepInPlugin("keelhauled.illusionvr.koikatu", "IllusionVR", "1.0.0")]
    public class IllusionVR : BaseUnityPlugin
    {
        public static Material skybox;

        private void Awake()
        {
            skybox = RenderSettings.skybox;
            Logger.LogInfo(skybox.name);

            bool vrDeactivated = Environment.CommandLine.Contains("--novr");
            bool vrActivated = Environment.CommandLine.Contains("--vr");

            if(vrActivated || (!vrDeactivated && SteamVRDetector.IsRunning))
                VRLoader.Create(true);
            else
                VRLoader.Create(false);
        }
    }
}
