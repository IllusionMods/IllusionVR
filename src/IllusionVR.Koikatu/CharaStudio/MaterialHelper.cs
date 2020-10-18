using System;
using UnityEngine;

namespace KKCharaStudioVR
{
	internal class MaterialHelper
	{
		private static AssetBundle _GripMovePluginResources;

		private static Shader _ColorZOrderShader;

		public static Shader GetColorZOrderShader()
		{
			if (MaterialHelper._ColorZOrderShader != null)
			{
				return MaterialHelper._ColorZOrderShader;
			}
			Shader result;
			try
			{
				if (MaterialHelper._GripMovePluginResources == null)
				{
					MaterialHelper._GripMovePluginResources = AssetBundle.LoadFromMemory(Resource.kkcharastudiovrshader);
				}
				MaterialHelper._ColorZOrderShader = MaterialHelper._GripMovePluginResources.LoadAsset<Shader>("ColorZOrder");
				result = MaterialHelper._ColorZOrderShader;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				result = null;
			}
			return result;
		}
	}
}
