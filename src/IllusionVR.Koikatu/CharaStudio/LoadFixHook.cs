using HarmonyLib;
using Studio;
using System;
using VRGIN.Core;

namespace IllusionVR.Koikatu.CharaStudio
{
    public static class LoadFixHook
    {
        public static void InstallHook()
        {
            Harmony.CreateAndPatchAll(typeof(LoadFixHook));
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SceneLoadScene), "OnClickLoad", new Type[] { })]
        public static void LoadScenePreHook(Studio.Studio __instance)
        {
            if(VRManager.Instance.Mode is StudioStandingMode)
                (VR.Manager.Interpreter as KKCharaStudioInterpreter).ForceResetVRMode();
        }
    }
}
