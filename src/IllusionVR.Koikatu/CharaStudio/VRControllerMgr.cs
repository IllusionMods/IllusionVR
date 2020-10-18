using System;
using System.Collections;
using IllusionUtility.GetUtility;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Modes;

namespace KKCharaStudioVR
{
	public class VRControllerMgr : MonoBehaviour
	{
		private static VRControllerMgr _instance;

		private bool isOculusTouchMode;

		private bool touchModeCheckCompleted;

		public static VRControllerMgr Install(GameObject container)
		{
			if (VRControllerMgr._instance == null)
			{
				VRControllerMgr._instance = container.AddComponent<VRControllerMgr>();
				VRControllerMgr._instance.OnLevelWasLoaded(Application.loadedLevel);
			}
			return VRControllerMgr._instance;
		}

		public static bool IsOculusTouchMode
		{
			get
			{
				return VRControllerMgr._instance.isOculusTouchMode;
			}
		}

		private void Start()
		{
		}

		private void OnLevelWasLoaded(int level)
		{
			base.StopAllCoroutines();
			touchModeCheckCompleted = false;
			base.StartCoroutine(CheckTouchMode());
		}

		private IEnumerator CheckTouchMode()
		{
			while (!touchModeCheckCompleted)
			{
				CheckControllerType();
				yield return new WaitForSeconds(0.5f);
			}
			yield break;
		}

		private void CheckControllerType()
		{
			if (isOculusTouchMode)
			{
				touchModeCheckCompleted = true;
				return;
			}
			if (VR.Mode is StandingMode)
			{
				if (VR.Mode.Left != null && VR.Mode.Left.IsTracking)
				{
					if (TransformFindEx.FindLoop(VR.Mode.Left.transform, "touchpad") != null)
					{
						isOculusTouchMode = false;
						touchModeCheckCompleted = true;
						return;
					}
					if (TransformFindEx.FindLoop(VR.Mode.Left.transform, "thumbstick") != null)
					{
						isOculusTouchMode = true;
						touchModeCheckCompleted = true;
					}
				}
				if (VR.Mode.Right != null && VR.Mode.Right.IsTracking)
				{
					if (TransformFindEx.FindLoop(VR.Mode.Right.transform, "touchpad") != null)
					{
						isOculusTouchMode = false;
						touchModeCheckCompleted = true;
						return;
					}
					if (TransformFindEx.FindLoop(VR.Mode.Right.transform, "thumbstick") != null)
					{
						isOculusTouchMode = true;
						touchModeCheckCompleted = true;
					}
				}
			}
		}
	}
}
