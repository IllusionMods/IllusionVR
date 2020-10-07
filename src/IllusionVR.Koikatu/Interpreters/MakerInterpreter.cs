using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace IllusionVR.Koikatu.Interpreters
{
    class MakerInterpreter : SceneInterpreter
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
