using BepInEx;
using System;
using VRGIN.Helpers;

namespace KoikatuVR
{
    [BepInPlugin("keelhauled.illusionvr.koikatu", "IllusionVR", "1.0.0")]
    public class IllusionVR : BaseUnityPlugin
    {
        private void Awake()
        {
            bool vrDeactivated = Environment.CommandLine.Contains("--novr");
            bool vrActivated = Environment.CommandLine.Contains("--vr");

            if(vrActivated || (!vrDeactivated && SteamVRDetector.IsRunning))
                VRLoader.Create(true);
            else
                VRLoader.Create(false);
        }
    }
}
