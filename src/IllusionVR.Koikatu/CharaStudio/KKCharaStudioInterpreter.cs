using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IllusionVR.Core;
using Manager;
using UnityEngine;
using VRGIN.Core;

namespace KKCharaStudioVR
{
	internal class KKCharaStudioInterpreter : GameInterpreter
	{
		private List<KKCharaStudioActor> _Actors = new List<KKCharaStudioActor>();

		private Camera _SubCamera;

		private StudioScene studioScene;

		private int additionalCullingMask;

        protected override void OnAwake()
        {
			SaveLoadSceneHook.InstallHook();
			LoadFixHook.InstallHook();

			VR.Manager.SetMode<GenericStandingMode>();
			var gameObject = new GameObject("KKCharaStudioVR");
			DontDestroyOnLoad(gameObject);
			IKTool.Create(gameObject);
			VRControllerMgr.Install(gameObject);
			VRCameraMoveHelper.Install(gameObject);
			VRItemObjMoveHelper.Install(gameObject);
			gameObject.AddComponent<KKCharaStudioVRGUI>();
			DontDestroyOnLoad(VRCamera.Instance.gameObject);
		}

        protected override void OnStart()
		{
			studioScene = FindObjectOfType<StudioScene>();
			additionalCullingMask = LayerMask.GetMask("Studio/Select");
		}

		protected override void OnLevel(int level)
		{
			base.OnLevel(level);
		}

		public override Camera FindCamera()
		{
			return null;
		}

		public override IActor FindNextActorToImpersonate()
		{
			List<IActor> list = Actors.ToList<IActor>();
			IActor actor = FindImpersonatedActor();
			if (actor == null)
			{
				return list.FirstOrDefault<IActor>();
			}
			return list[(list.IndexOf(actor) + 1) % list.Count];
		}

		protected override void OnUpdate()
		{
			try
			{
				if (VR.Manager)
				{
					RefreshActors();
					UpdateMainCameraCullingMask();
				}
			}
			catch (Exception)
			{
			}
		}

		private void UpdateMainCameraCullingMask()
		{
			Camera component = VR.Camera.SteamCam.GetComponent<Camera>();
			if (Singleton<Studio.Studio>.Instance.workInfo.visibleAxis)
			{
				component.cullingMask |= additionalCullingMask;
				return;
			}
			component.cullingMask &= ~additionalCullingMask;
		}

		private void RefreshActors()
		{
			_Actors.Clear();
			foreach (ChaControl chaControl in Singleton<Character>.Instance.dictEntryChara.Values)
			{
				if (chaControl.objBodyBone)
				{
					AddActor(DefaultActorBehaviour<ChaControl>.Create<KKCharaStudioActor>(chaControl));
				}
			}
		}

		private void AddActor(KKCharaStudioActor actor)
		{
			if (!actor.Eyes)
			{
				actor.Head.Reinitialize();
				return;
			}
			_Actors.Add(actor);
		}

		public override IEnumerable<IActor> Actors
		{
			get
			{
				return _Actors.Cast<IActor>();
			}
		}

		public void ForceResetVRMode()
		{
			base.StartCoroutine(ForceResetVRModeCo());
		}

		private IEnumerator ForceResetVRModeCo()
		{
			IVRLog.LogDebug("Check and reset to StandingMode if not.");
			yield return null;
			yield return null;
			yield return null;
			yield return null;
			yield return null;
			if (!VRManager.Instance.Mode || !(VRManager.Instance.Mode is GenericStandingMode))
			{
				IVRLog.LogDebug("Mode is not StandingMode. Force reset as Standing Mode.");
				KKCharaStudioInterpreter.ForceResetAsStandingMode();
			}
			else
			{
				IVRLog.LogDebug("Is Standing Mode. Skip to setting force.");
			}
			yield break;
		}

		public static void ForceResetAsStandingMode()
		{
			try
			{
				VR.Manager.SetMode<GenericStandingMode>();
				if (VR.Camera)
				{
					Camera blueprint = VR.Camera.Blueprint;
					Camera mainCmaera = Singleton<Studio.Studio>.Instance.cameraCtrl.mainCmaera;
					IVRLog.LogDebug(string.Format("Force replace blueprint camera with {0}", mainCmaera));
					Camera camera = VR.Camera.SteamCam.camera;
					Camera camera2 = mainCmaera;
					camera.nearClipPlane = VR.Context.NearClipPlane;
					camera.farClipPlane = Mathf.Max(camera2.farClipPlane, 10f);
					camera.clearFlags = ((camera2.clearFlags == CameraClearFlags.Skybox) ? CameraClearFlags.Skybox : CameraClearFlags.Color);
					camera.renderingPath = camera2.renderingPath;
					camera.clearStencilAfterLightingPass = camera2.clearStencilAfterLightingPass;
					camera.depthTextureMode = camera2.depthTextureMode;
					camera.layerCullDistances = camera2.layerCullDistances;
					camera.layerCullSpherical = camera2.layerCullSpherical;
					camera.useOcclusionCulling = camera2.useOcclusionCulling;
					camera.allowHDR = camera2.allowHDR;
					camera.backgroundColor = camera2.backgroundColor;
					Skybox component = camera2.GetComponent<Skybox>();
					if (component != null)
					{
						Skybox skybox = camera.gameObject.GetComponent<Skybox>();
						if (skybox == null)
						{
							skybox = skybox.gameObject.AddComponent<Skybox>();
						}
						skybox.material = component.material;
					}
					VR.Camera.CopyFX(camera2);
				}
				else
				{
					IVRLog.LogDebug("VR.Camera is null");
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}
	}
}
