using UnityEngine;
using VRGIN.Core;

namespace KoikatuVR.Interpreters
{
    class KoikatuInterpreter : GameInterpreter
    {
        public const int NoScene = -1;
        public const int OtherScene = 0;
        public const int ActionScene = 1;
        public const int TalkScene = 2;
        public const int HScene = 3;
        public const int NightMenuScene = 4;

        private int _SceneType;
        public SceneInterpreter SceneInterpreter;

        protected override void OnAwake()
        {
            base.OnAwake();

            _SceneType = NoScene;
            SceneInterpreter = new OtherSceneInterpreter();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            DetectScene();
            SceneInterpreter.OnUpdate();
        }

        // 前回とSceneが変わっていれば切り替え処理をする
        private void DetectScene()
        {
            int nextSceneType = NoScene;
            SceneInterpreter nextInterpreter = new OtherSceneInterpreter();

            if (GameObject.Find("TalkScene") != null)
            {
                if (_SceneType != TalkScene)
                {
                    nextSceneType = TalkScene;
                    //nextInterpreter = new TalkSceneInterpreter(); 特有の処理がないため不要
                    VRLog.Info("Start TalkScene");
                }
            }

            else if (GameObject.Find("HScene") != null)
            {
                if (_SceneType != HScene)
                {
                    nextSceneType = HScene;
                    nextInterpreter = new HSceneInterpreter();
                    VRLog.Info("Start HScene");
                }
            }

            else if (GameObject.Find("NightMenuScene") != null)
            {
                if (_SceneType != NightMenuScene)
                {
                    nextSceneType = NightMenuScene;
                    nextInterpreter = new NightMenuSceneInterpreter();
                    VRLog.Info("Start NightMenuScene");
                }
            }

            else if (GameObject.Find("ActionScene") != null)
            {
                if (_SceneType != ActionScene)
                {
                    nextSceneType = ActionScene;
                    nextInterpreter = new ActionSceneInterpreter();
                    VRLog.Info("Start ActionScene");
                }
            }

            else
            {
                if (_SceneType != OtherScene)
                {
                    nextSceneType = OtherScene;
                    //nextInterpreter = new OtherSceneInterpreter();
                    VRLog.Info("Start OtherScene");
                }
            }

            if (nextSceneType != NoScene)
            {
                SceneInterpreter.OnDisable();

                _SceneType = nextSceneType;
                SceneInterpreter = nextInterpreter;
                SceneInterpreter.OnStart();
            }
        }
    }
}
