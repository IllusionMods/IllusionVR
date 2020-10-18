using UnityEngine;
using VRGIN.Core;

namespace IllusionVR.Koikatu.Interpreters
{
    internal class KoikatuInterpreter : GameInterpreter
    {
        private SceneType CurrentSceneType;
        public SceneInterpreter SceneInterpreter;

        protected override void OnAwake()
        {
            base.OnAwake();

            CurrentSceneType = SceneType.NoScene;
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
            var nextSceneType = SceneType.NoScene;
            SceneInterpreter nextInterpreter = new OtherSceneInterpreter();

            if(GameObject.Find("TalkScene") != null)
            {
                if(CurrentSceneType != SceneType.Talk)
                {
                    nextSceneType = SceneType.Talk;
                    //nextInterpreter = new TalkSceneInterpreter(); 特有の処理がないため不要
                    IllusionVR.Logger.LogDebug("Start TalkScene");
                }
            }

            else if(GameObject.Find("HScene") != null)
            {
                if(CurrentSceneType != SceneType.HScene)
                {
                    nextSceneType = SceneType.HScene;
                    nextInterpreter = new HSceneInterpreter();
                    IllusionVR.Logger.LogDebug("Start HScene");
                }
            }

            else if(GameObject.Find("NightMenuScene") != null)
            {
                if(CurrentSceneType != SceneType.NightMenu)
                {
                    nextSceneType = SceneType.NightMenu;
                    nextInterpreter = new NightMenuSceneInterpreter();
                    IllusionVR.Logger.LogDebug("Start NightMenuScene");
                }
            }

            else if(GameObject.Find("ActionScene") != null)
            {
                if(CurrentSceneType != SceneType.Action)
                {
                    nextSceneType = SceneType.Action;
                    nextInterpreter = new ActionSceneInterpreter();
                    IllusionVR.Logger.LogDebug("Start ActionScene");
                }
            }

            else if(GameObject.Find("CustomScene") != null)
            {
                if(CurrentSceneType != SceneType.Maker)
                {
                    nextSceneType = SceneType.Maker;
                    nextInterpreter = new MakerInterpreter();
                    IllusionVR.Logger.LogDebug("Start MakerScene");
                }
            }

            else
            {
                if(CurrentSceneType != SceneType.Other)
                {
                    nextSceneType = SceneType.Other;
                    //nextInterpreter = new OtherSceneInterpreter();
                    IllusionVR.Logger.LogDebug("Start OtherScene");
                }
            }

            if(nextSceneType != SceneType.NoScene)
            {
                SceneInterpreter.OnDisable();

                CurrentSceneType = nextSceneType;
                SceneInterpreter = nextInterpreter;
                SceneInterpreter.OnStart();
            }
        }
    }

    public enum SceneType
    {
        NoScene,
        Other,
        Action,
        Talk,
        HScene,
        NightMenu,
        Maker
    }
}
