using BepInEx;
using IllusionVR.Core;
using IllusionVR.Koikatu.CharaStudio;
using IllusionVR.Koikatu.MainGame;
using IllusionVR.Koikatu.MainGame.Interpreters;
using VRGIN.Core;

namespace IllusionVR.Koikatu
{
    [BepInPlugin("keelhauled.illusionvr.koikatu", "IllusionVR", "1.0.0")]
    public class KoikatuVR : IllusionVRCore
    {
        protected override void Awake()
        {
            base.Awake();

            VRLoader.OnVRSuccess += () =>
            {
                if(Paths.ProcessName == "CharaStudio")
                    VRManager.Create<KKCharaStudioInterpreter>(StudioContext.CreateContext("KKCSVRContext.xml"));
                else
                    VRManager.Create<KoikatuInterpreter>(MainGameContext.CreateContext("VRContext.xml"));
            };
        }
    }
}
