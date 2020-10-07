using UnityEngine;

namespace IllusionVR.Koikatu.Interpreters
{
    internal class MakerInterpreter : SceneInterpreter
    {
        public override void OnDisable()
        {

        }

        public override void OnStart()
        {
            RenderSettings.skybox = new Material(Shader.Find("Skybox/Procedural"));
        }

        public override void OnUpdate()
        {

        }
    }
}
