using UnityEngine;

namespace KoikatuVR.Interpreters
{
    class NightMenuSceneInterpreter : SceneInterpreter
    {
        public override void OnStart()
        {
            // 夜メニューの時点で、登校時のイベントでちょうどいい位置に移動しておく
            // これによってキャラクターレイヤの光源がうまく適用されない場合があるのも回避できてる？
            var player = GameObject.Find("ActionScene/Player").transform;
            var playerHead = GameObject.Find("ActionScene/Player/chaM_001/BodyTop/p_cf_body_bone/cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_j_neck/cf_j_head/cf_s_head")?.transform;
            playerHead = playerHead ?? GameObject.Find("ActionScene/Player/chaM_001/BodyTop/p_cf_body_bone_low/cf_j_root/cf_n_height/cf_j_hips/cf_j_spine01/cf_j_spine02/cf_j_spine03/cf_j_neck/cf_j_head/cf_s_head")?.transform;
            var cam = GameObject.Find("VRGIN_Camera (origin)").transform;
            var headCam = GameObject.Find("VRGIN_Camera (origin)/VRGIN_Camera (eye)/VRGIN_Camera (head)").transform;

            cam.rotation = player.rotation;
            var delta_y = 180 + cam.rotation.eulerAngles.y - headCam.rotation.eulerAngles.y;
            cam.Rotate(Vector3.up * delta_y);

            Vector3 cf = Vector3.Scale(player.forward, new Vector3(1, 0, 1)).normalized;

            Vector3 pos;
            pos = playerHead.position;
            cam.position = pos - (headCam.position - cam.position) + cf;
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
