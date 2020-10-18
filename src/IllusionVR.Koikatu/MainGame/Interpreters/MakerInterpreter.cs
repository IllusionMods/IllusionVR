using UnityEngine;
using VRGIN.Core;
using VRGIN.Modes;

namespace IllusionVR.Koikatu.MainGame.Interpreters
{
    internal class MakerInterpreter : SceneInterpreter
    {
        private Material skyboxMat;
        private Material SkyboxMat
        {
            get
            {
                if(!skyboxMat)
                {
                    var ass = AssetBundle.LoadFromMemory(Properties.Resources.illusionvr);
                    skyboxMat = ass.LoadAsset<Material>("VRSkybox");
                    ass.Unload(false);
                }

                return skyboxMat;
            }
        }

        public override void OnDisable()
        {

        }

        public override void OnStart()
        {
            RenderSettings.skybox = SkyboxMat;
            VR.Manager.SetMode<StandingMode>();
        }

        public override void OnUpdate()
        {

        }
    }
}
