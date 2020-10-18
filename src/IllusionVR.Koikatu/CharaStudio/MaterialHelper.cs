using System;
using UnityEngine;

namespace IllusionVR.Koikatu.CharaStudio
{
    internal class MaterialHelper
    {
        private static AssetBundle _GripMovePluginResources;

        private static Shader _ColorZOrderShader;

        public static Shader GetColorZOrderShader()
        {
            if(_ColorZOrderShader != null)
            {
                return _ColorZOrderShader;
            }
            Shader result;
            try
            {
                if(_GripMovePluginResources == null)
                {
                    _GripMovePluginResources = AssetBundle.LoadFromMemory(KoikatuVR.Resources.kkcharastudiovrshader);
                }
                _ColorZOrderShader = _GripMovePluginResources.LoadAsset<Shader>("ColorZOrder");
                result = _ColorZOrderShader;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = null;
            }
            return result;
        }
    }
}
