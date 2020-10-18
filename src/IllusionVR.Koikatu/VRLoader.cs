using IllusionVR.Koikatu.Interpreters;
using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using VRGIN.Core;
using BepInEx;
using KKCharaStudioVR;
using IllusionVR.Core;

namespace IllusionVR.Koikatu
{
    internal class VRLoader : ProtectedBehaviour
    {
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

                if(Paths.ProcessName == "CharaStudio")
                {
                    SaveLoadSceneHook.InstallHook();
                    LoadFixHook.InstallHook();

                    VRManager.Create<KKCharaStudioInterpreter>(CreateContext(Path.Combine(Paths.ConfigPath, "KKCSVRContext.xml")));
                    VR.Manager.SetMode<GenericStandingMode>();
                    GameObject gameObject = new GameObject("KKCharaStudioVR");
                    DontDestroyOnLoad(gameObject);
                    IKTool.Create(gameObject);
                    VRControllerMgr.Install(gameObject);
                    VRCameraMoveHelper.Install(gameObject);
                    VRItemObjMoveHelper.Install(gameObject);
                    gameObject.AddComponent<KKCharaStudioVRGUI>();
                    DontDestroyOnLoad(VRCamera.Instance.gameObject);
                }
                else
                {
                    VRManager.Create<KoikatuInterpreter>(CreateContext(Path.Combine(Paths.ConfigPath, "VRContext.xml")));
                    VR.Manager.SetMode<GenericStandingMode>();
                }
            }
        }

        private IVRManagerContext CreateContext(string path)
        {
            var serializer = new XmlSerializer(typeof(ConfigurableContext));

            if(File.Exists(path))
            {
                // Attempt to load XML
                using(var file = File.OpenRead(path))
                {
                    try
                    {
                        return serializer.Deserialize(file) as ConfigurableContext;
                    }
                    catch(Exception ex)
                    {
                        IVRLog.LogError($"Failed to deserialize {path} -- using default\n{ex}");
                    }
                }
            }

            // Create and save file
            var context = new ConfigurableContext();
            try
            {
                using(var file = new StreamWriter(path))
                {
                    file.BaseStream.SetLength(0);
                    serializer.Serialize(file, context);
                }
            }
            catch(Exception ex)
            {
                IVRLog.LogError($"Failed to write {path}\n{ex}");
            }

            return context;
        }
    }
}
