using UnityEngine;
using VRGIN.Core;
using VRGIN.Modes;

namespace IllusionVR.Koikatu.MainGame.Interpreters
{
    internal class HSceneInterpreter : SceneInterpreter
    {
        private bool _NeedsResetCamera;

        public override void OnStart()
        {
            _NeedsResetCamera = true;
            VR.Manager.SetMode<StandingMode>();
        }

        public override void OnDisable()
        {
            // nothing to do.
        }

        public override void OnUpdate()
        {
            if(_NeedsResetCamera)
            {
                ResetCamera();
            }
        }

        private void ResetCamera()
        {
            VRLog.Debug("HScene ResetCamera");

            var cam = Object.FindObjectOfType<CameraControl_Ver2>();

            if(cam != null)
            {
                cam.enabled = false;
                _NeedsResetCamera = false;

                VRLog.Debug("succeeded");
            }
        }
    }
}
