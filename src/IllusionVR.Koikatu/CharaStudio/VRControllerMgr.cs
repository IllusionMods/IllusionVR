using IllusionUtility.GetUtility;
using System.Collections;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Modes;

namespace IllusionVR.Koikatu.CharaStudio
{
    public class VRControllerMgr : MonoBehaviour
    {
        private static VRControllerMgr _instance;

        private bool isOculusTouchMode;

        private bool touchModeCheckCompleted;

        public static VRControllerMgr Install(GameObject container)
        {
            if(_instance == null)
            {
                _instance = container.AddComponent<VRControllerMgr>();
                _instance.OnLevelWasLoaded(Application.loadedLevel);
            }
            return _instance;
        }

        public static bool IsOculusTouchMode => _instance.isOculusTouchMode;

        private void OnLevelWasLoaded(int level)
        {
            StopAllCoroutines();
            touchModeCheckCompleted = false;
            StartCoroutine(CheckTouchMode());
        }

        private IEnumerator CheckTouchMode()
        {
            while(!touchModeCheckCompleted)
            {
                CheckControllerType();
                yield return new WaitForSeconds(0.5f);
            }
            yield break;
        }

        private void CheckControllerType()
        {
            if(isOculusTouchMode)
            {
                touchModeCheckCompleted = true;
                return;
            }
            if(VR.Mode is StandingMode)
            {
                if(VR.Mode.Left != null && VR.Mode.Left.IsTracking)
                {
                    if(TransformFindEx.FindLoop(VR.Mode.Left.transform, "touchpad") != null)
                    {
                        isOculusTouchMode = false;
                        touchModeCheckCompleted = true;
                        return;
                    }
                    if(TransformFindEx.FindLoop(VR.Mode.Left.transform, "thumbstick") != null)
                    {
                        isOculusTouchMode = true;
                        touchModeCheckCompleted = true;
                    }
                }
                if(VR.Mode.Right != null && VR.Mode.Right.IsTracking)
                {
                    if(TransformFindEx.FindLoop(VR.Mode.Right.transform, "touchpad") != null)
                    {
                        isOculusTouchMode = false;
                        touchModeCheckCompleted = true;
                        return;
                    }
                    if(TransformFindEx.FindLoop(VR.Mode.Right.transform, "thumbstick") != null)
                    {
                        isOculusTouchMode = true;
                        touchModeCheckCompleted = true;
                    }
                }
            }
        }
    }
}
