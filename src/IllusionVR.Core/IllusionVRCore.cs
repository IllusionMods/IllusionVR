using BepInEx;
using BepInEx.Logging;
using System;
using HarmonyLib;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Helpers;

namespace IllusionVR.Core
{
    public class IllusionVRCore : BaseUnityPlugin
    {
        private static Texture2D windowBackground;
        
        protected virtual void Awake()
        {
            windowBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            windowBackground.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.5f, 1));
            windowBackground.Apply();
            
            //Harmony.CreateAndPatchAll(typeof(IllusionVRCore));
            
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

        [HarmonyPrefix, HarmonyPatch(typeof(GUIStyleState), MethodType.Constructor, new Type[]{})]
        private static void ForceOpaqueGUIBackground(GUIStyleState __instance)
        {
            var instance = Traverse.Create(__instance);
            instance.Field("m_Background").SetValue(windowBackground);
            instance.Method("SetBackgroundInternal").GetValue(windowBackground);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(GUIStyleState), nameof(GUIStyleState.background), MethodType.Setter)]
        private static bool PreventGUIBackgroundChange()
        {
            return false;
        }
    }
}
