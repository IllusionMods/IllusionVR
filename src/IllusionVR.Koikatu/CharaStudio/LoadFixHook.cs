using HarmonyLib;
using IllusionVR.Core;
using Studio;
using System;
using VRGIN.Core;

namespace KKCharaStudioVR
{
    public static class LoadFixHook
    {
        public static bool forceSetStandingMode;

        public static void InstallHook()
        {
            new Harmony("KKChacaStudioVR.LoadFixHook").PatchAll(typeof(LoadFixHook));
        }

        [HarmonyPatch(typeof(SceneLoadScene), "OnClickLoad", new Type[] { }, null)]
        [HarmonyPrefix]
        public static bool LoadScenePreHook(Studio.Studio __instance)
        {
            IVRLog.LogDebug("Start Scene Loading.");
            if(VRManager.Instance.Mode is GenericStandingMode)
            {
                (VR.Manager.Interpreter as KKCharaStudioInterpreter).ForceResetVRMode();
            }
            return true;
        }
    }
}
