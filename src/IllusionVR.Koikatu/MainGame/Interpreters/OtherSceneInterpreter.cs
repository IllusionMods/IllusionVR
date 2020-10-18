using VRGIN.Core;
using VRGIN.Modes;

namespace IllusionVR.Koikatu.MainGame.Interpreters
{
    internal class OtherSceneInterpreter : SceneInterpreter
    {
        public override void OnStart()
        {
            VR.Manager.SetMode<StandingMode>();
        }

        public override void OnDisable()
        {
            // nothing to do.
        }

        public override void OnUpdate()
        {
            // nothing to do.
        }
    }
}
