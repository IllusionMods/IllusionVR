using UnityEngine;
using VRGIN.Core;
using WindowsInput.Native;

namespace KoikatuVR.Interpreters
{
    class ActionSceneInterpreter : SceneInterpreter
    {
        private KoikatuSettings _Settings;

        private GameObject _Map;
        private GameObject _CameraSystem;
        private bool _NeedsResetCamera;
        private bool _NeedsMoveCamera;
        private bool _IsStanding = true;
        private bool _Walking = false;
        private bool _Dashing = false; // ダッシュ時は_Walkingと両方trueになる
        private int _MoveCameraWaitTime = 0;

        public override void OnStart()
        {
            VRLog.Info("ActionScene OnStart");

            _Settings = (VR.Context.Settings as KoikatuSettings);

            ResetState();
            HoldCamera();
        }

        public override void OnDisable()
        {
            VRLog.Info("ActionScene OnDisable");

            ResetState();
            ReleaseCamera();
        }

        private void ResetState()
        {
            VRLog.Info("ActionScene ResetState");

            StandUp();
            StopWalking();
            _NeedsResetCamera = true;
            _NeedsMoveCamera = false;
            _MoveCameraWaitTime = 0;
        }

        private void ResetCamera()
        {
            var pl = GameObject.Find("ActionScene/Player/chaM_001/BodyTop");
            //_CameraSystem = GameObject.Find("ActionScene/CameraSystem");

            if (pl != null && pl.activeSelf)
            {
                GameObject scene = GameObject.Find("ActionScene");
                _CameraSystem = scene.transform.Find("CameraSystem").gameObject;

                // トイレなどでFPS視点になっている場合にTPS視点に戻す
                _CameraSystem.GetComponent<ActionGame.CameraStateDefinitionChange>().ModeChangeForce((ActionGame.CameraMode?) ActionGame.CameraMode.TPS);
                //scene.GetComponent<ActionScene>().isCursorLock = false;

                // プレイヤーキャラの頭を非表示にする
                if (pl.transform.Find("p_cf_body_bone/cf_j_root") is Transform t1) t1.gameObject.SetActive(false);
                else if (pl.transform.Find("p_cf_body_bone_low/cf_j_root") is Transform t2) t2.gameObject.SetActive(false);

                // カメラをプレイヤーの位置に移動
                MoveCameraToPlayer();

                _NeedsResetCamera = false;
                VRLog.Info("ResetCamera succeeded");
            }
        }

        private void HoldCamera()
        {
            VRLog.Info("ActionScene HoldCamera");

            _CameraSystem = GameObject.Find("ActionScene").transform.Find("CameraSystem").gameObject;

            if (_CameraSystem != null)
            {
                _CameraSystem.SetActive(false);

                VRLog.Info("succeeded");
            }
        }

        private void ReleaseCamera()
        {
            VRLog.Info("ActionScene ReleaseCamera");

            if (_CameraSystem != null)
            {
                _CameraSystem.SetActive(true);

                VRLog.Info("succeeded");
            }
        }

        public override void OnUpdate()
        {
            var player = GameObject.Find("ActionScene/Player");
            var camera = StrayTech.MonoBehaviourSingleton<StrayTech.CameraSystem>.Instance.CurrentCamera;
            if (player != null && camera != null)
            {
                camera.transform.rotation = player.transform.rotation;
                camera.transform.position = player.transform.position;
            }

            GameObject map = GameObject.Find("Map");

            if (map != _Map)
            {

                VRLog.Info("! map changed.");

                ResetState();
                _Map = map;
                _NeedsResetCamera = true;
            }

            UpdateCrouch();

            if (_MoveCameraWaitTime > 0)
            {
                _MoveCameraWaitTime--;

                if (_MoveCameraWaitTime == 0)
                {
                    _NeedsMoveCamera = true;
                }
            }

            if (_NeedsMoveCamera || _Walking)
            {
                MoveCameraToPlayer(_Walking);
                _NeedsMoveCamera = false;
                _MoveCameraWaitTime = 0;
            }

            if (_NeedsResetCamera)
            {
                ResetCamera();
            }
        }

        private void UpdateCrouch()
        {
            if (_Settings.CrouchByHMDPos)// && _CameraSystem != null)
            {
                var cam = GameObject.Find("VRGIN_Camera (origin)").transform;
                var headCam = GameObject.Find("VRGIN_Camera (origin)/VRGIN_Camera (eye)/VRGIN_Camera (head)").transform;
                var delta_y = cam.position.y - headCam.position.y;

                if (_IsStanding && delta_y > _Settings.CrouchThrethould)
                {
                    Crouch();
                }
                else if (!_IsStanding && delta_y < _Settings.StandUpThrethould)
                {
                    StandUp();
                }
            }
        }

        public void MoveCameraToPlayer(bool onlyPosition = false)
        {
            var player = GameObject.Find("ActionScene/Player").transform;

            var playerHead = player.transform.Find("chaM_001/BodyTop/p_cf_body_bone/cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_j_neck/cf_j_head/cf_s_head");
            playerHead = playerHead ?? player.transform.Find("chaM_001/BodyTop/p_cf_body_bone_low/cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_j_neck/cf_j_head/cf_s_head");
            //var playerHead = GameObject.Find("ActionScene/Player/chaM_001/BodyTop/p_cf_body_bone_low/cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_j_neck/cf_j_head/cf_s_head").transform;
            var cam = GameObject.Find("VRGIN_Camera (origin)").transform;
            var headCam = GameObject.Find("VRGIN_Camera (origin)/VRGIN_Camera (eye)/VRGIN_Camera (head)").transform;

            // 歩いているときに回転をコピーするとおかしくなるバグの暫定対策
            // 歩く方向がHMDの方向基準なので歩いている時はコピーしなくても回転は一致する
            if (!onlyPosition)
            {
                cam.rotation = player.rotation;
                var delta_y = cam.rotation.eulerAngles.y - headCam.rotation.eulerAngles.y;
                cam.Rotate(Vector3.up * delta_y);
            }

            Vector3 cf = Vector3.Scale(player.forward, new Vector3(1, 0, 1)).normalized;

            Vector3 pos;
            if (_Settings.UsingHeadPos)
            {
                pos = playerHead.position;
            }
            else
            {
                pos = player.position;
                pos.y += _IsStanding ? _Settings.StandingCameraPos : _Settings.CrouchingCameraPos;
            }

            // 首が見えるとうざいのでほんの少し前目にする
            cam.position = pos - (headCam.position - cam.position) + cf * 0.23f;
        }

        public void MovePlayerToCamera(bool onlyRotation = false)
        {
            var player = GameObject.Find("ActionScene/Player").transform;
            var playerHead = player.transform.Find("chaM_001/BodyTop/p_cf_body_bone/cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_j_neck/cf_j_head/cf_s_head");
            playerHead = playerHead ?? player.transform.Find("chaM_001/BodyTop/p_cf_body_bone_low/cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_j_neck/cf_j_head/cf_s_head");
            //var playerHead = GameObject.Find("ActionScene/Player/chaM_001/BodyTop/p_cf_body_bone_low/cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_j_neck/cf_j_head/cf_s_head").transform;
            //var cam = GameObject.Find("VRGIN_Camera (origin)").transform;
            var headCam = GameObject.Find("VRGIN_Camera (origin)/VRGIN_Camera (eye)/VRGIN_Camera (head)").transform;

            var pos = headCam.position;
            pos.y += player.position.y - playerHead.position.y;

            var delta_y = headCam.rotation.eulerAngles.y - player.rotation.eulerAngles.y;
            player.Rotate(Vector3.up * delta_y);
            Vector3 cf = Vector3.Scale(player.forward, new Vector3(1, 0, 1)).normalized;

            if (!onlyRotation)
            {
                player.position = pos - cf * 0.1f;
            }
        }

        public void RotatePlayer(float angle)
        {
            var player = GameObject.Find("ActionScene/Player").transform;
            player.Rotate(Vector3.up * angle);
            _NeedsMoveCamera = true;
        }

        public void Crouch()
        {
            if (_IsStanding)
            {
                _IsStanding = false;
                VR.Input.Keyboard.KeyDown(VirtualKeyCode.VK_Z);

                // 数F待ってから視点移動する
                //_NeedsMoveCamera = true;
                _MoveCameraWaitTime = 30;
            }
        }

        public void StandUp()
        {
            if (!_IsStanding)
            {
                _IsStanding = true;
                VR.Input.Keyboard.KeyUp(VirtualKeyCode.VK_Z);

                // 数F待ってから視点移動する
                //_NeedsMoveCamera = true;
                _MoveCameraWaitTime = 30;
            }
        }

        public void StartWalking(bool dash = false)
        {
            MovePlayerToCamera(true);

            if (!dash)
            {
                VR.Input.Keyboard.KeyDown(VirtualKeyCode.SHIFT);
                _Dashing = true;
            }

            VR.Input.Mouse.LeftButtonDown();
            _Walking = true;
        }

        public void StopWalking()
        {
            VR.Input.Mouse.LeftButtonUp();

            if (_Dashing)
            {
                VR.Input.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
                _Dashing = false;
            }

            _Walking = false;
        }
    }
}
