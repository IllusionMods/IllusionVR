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

        private static bool isVREnable = false;

        private static VRLoader instance;
        public static VRLoader Instance
        {
            get
            {
                if(instance == null)
                    throw new InvalidOperationException("VR Loader has not been created yet!");

                return instance;
            }
        }

        public static VRLoader Create(bool isEnable)
        {
            isVREnable = isEnable;
            instance = new GameObject("VRLoader").AddComponent<VRLoader>();

            return instance;
        }

        protected override void OnAwake()
        {
            StartCoroutine(isVREnable ? LoadDevice(DeviceOpenVR) : LoadDevice(DeviceNone));
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
                
                VRLog.Debug("VR succesfully initialized");
                OnVRSuccess?.Invoke();
            }
        }
    }
}
