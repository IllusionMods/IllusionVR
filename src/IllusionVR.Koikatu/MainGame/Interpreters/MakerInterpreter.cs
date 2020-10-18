using UnityEngine;

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
                    var ass = AssetBundle.LoadFromMemory(KoikatuVR.Resources.illusionvr);
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
        }

        public override void OnUpdate()
        {

        }
    }
}
