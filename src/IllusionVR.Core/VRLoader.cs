using System;
using System.Collections;
using UnityEngine;
using VRGIN.Core;

namespace IllusionVR.Core
{
    internal class VRLoader : ProtectedBehaviour
    {
        public static Action OnVRSuccess;

        private const string DeviceOpenVR = "OpenVR";
        private const string DeviceNone = "None";

        private static bool _isVREnable = false;

        private static VRLoader _Instance;
        public static VRLoader Instance
        {
            get
            {
                if(_Instance == null)
                    throw new InvalidOperationException("VR Loader has not been created yet!");

                return _Instance;
            }
        }

        public static VRLoader Create(bool isEnable)
        {
            _isVREnable = isEnable;
            _Instance = new GameObject("VRLoader").AddComponent<VRLoader>();

            return _Instance;
        }

        protected override void OnAwake()
        {
            if(_isVREnable)
                StartCoroutine(LoadDevice(DeviceOpenVR));
            else
                StartCoroutine(LoadDevice(DeviceNone));
        }

        private IEnumerator LoadDevice(string newDevice)
        {
            bool vrMode = newDevice != DeviceNone;

            UnityEngine.VR.VRSettings.LoadDeviceByName(newDevice);
            yield return null;
            UnityEngine.VR.VRSettings.enabled = vrMode;
            yield return null;

            while(UnityEngine.VR.VRSettings.loadedDeviceName != newDevice || UnityEngine.VR.VRSettings.enabled != vrMode)
            {
                yield return null;
            }

            if(vrMode)
            {
                // Boot VRManager!
                // Note: Use your own implementation of GameInterpreter to gain access to a few useful operatoins
                // (e.g. characters, camera judging, colliders, etc.)
                OnVRSuccess?.Invoke();
            }
        }
    }
}
