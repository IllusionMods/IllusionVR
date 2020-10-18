using HarmonyLib;
using Studio;
using System;
using UnityEngine;
using UnityEngine.UI;
using VRGIN.Core;
using VRUtil;

namespace KKCharaStudioVR
{
    public class VRCameraMoveHelper : MonoBehaviour
    {
        public bool showGUI = true;

        public RectTransform menuRect;

        private static VRCameraMoveHelper _instance;

        public bool keepY = true;

        public bool moveAlong;

        public Vector3 moveAlongBasePos;

        public Quaternion moveAlongBaseRot;

        private Studio.Studio studio;

        private GameObject moveDummy;

        private int windowID = 8752;

        private const int panelWidth = 400;

        private const int panelHeight = 100;

        private Rect windowRect = new Rect(-1f, -1f, 0f, 0f);

        private string windowTitle = "";

        private float DEFAULT_DISTANCE = 3f;

        private float DISTANCE_RATIO = 1f;

        public static VRCameraMoveHelper Instance => VRCameraMoveHelper._instance;

        public static void Install(GameObject container)
        {
            if(VRCameraMoveHelper._instance == null)
            {
                VRCameraMoveHelper._instance = container.AddComponent<VRCameraMoveHelper>();
            }
        }

        private void Start()
        {
        }

        private void OnLevelWasLoaded(int level)
        {
            studio = Singleton<Studio.Studio>.Instance;
            if(studio == null)
            {
                return;
            }
            Transform cameraMenuRootT = studio.transform.Find("Canvas System Menu/02_Camera");
            VRCameraMoveHelper._instance.Init(cameraMenuRootT);
        }

        private void OnGUI()
        {
            if(showGUI && menuRect != null && menuRect.gameObject.activeInHierarchy)
            {
                GUISkin skin = GUI.skin;
                try
                {
                    GUI.skin = VRIMGUIUtil.VRGUISkin;
                    if(windowRect.x == -1f && windowRect.y == -1f)
                    {
                        windowRect = new Rect(Screen.width / 2, 60f * menuRect.lossyScale.y, 400f, 100f);
                    }
                    windowRect = GUI.Window(windowID, windowRect, new GUI.WindowFunction(FuncWindowGUI), windowTitle);
                }
                finally
                {
                    GUI.skin = skin;
                }
            }
        }

        private void FuncWindowGUI(int winID)
        {
            try
            {
                GUI.enabled = true;
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] array = new GUILayoutOption[]
                {
                    GUILayout.Width(80f),
                    GUILayout.Height(35f)
                };
                if(GUILayout.Button("Back(1m)", array))
                {
                    MoveForwardBackward(-1f);
                }
                if(GUILayout.Button("Back(2m)", array))
                {
                    MoveForwardBackward(-2f);
                }
                if(GUILayout.Button("Jump", array))
                {
                    MoveToSelectedObject(true);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if(GUILayout.Button("Fwd(1m)", array))
                {
                    MoveForwardBackward(1f);
                }
                if(GUILayout.Button("Fwd(2m)", array))
                {
                    MoveForwardBackward(2f);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUI.DragWindow();
            }
            catch(Exception value)
            {
                Console.WriteLine(value);
            }
        }

        public void SaveCamera(int slot)
        {
            if(VR.Camera.Head == null)
            {
                return;
            }
            CurrentToCameraCtrl();
            studio.sceneInfo.cameraData[slot] = studio.cameraCtrl.Export();
        }

        public void CurrentToCameraCtrl()
        {
            GetCurrentLookDirAndRot(out Vector3 vector, out Vector3 vector2, out Vector3 vector3);
            //var cameraData = new CameraControl.CameraData();
            var cameraData = Activator.CreateInstance(typeof(BaseCameraControl).Assembly.GetType("BaseCameraControl+CameraData"));
            VR.Camera.Head.TransformPoint(vector2.normalized * DEFAULT_DISTANCE * DISTANCE_RATIO);
            Vector3 vector4 = new Vector3(0f, 0f, -1f * DEFAULT_DISTANCE * DISTANCE_RATIO);
            //cameraData.Set(vector, vector3, vector4, studio.cameraCtrl.fieldOfView);
            Traverse.Create(cameraData).Method("Set").GetValue(vector, vector3, vector4, studio.cameraCtrl.fieldOfView);
            //studio.cameraCtrl.Import(cameraData);
            Traverse.Create(studio.cameraCtrl).Method("Import").GetValue(cameraData);
        }

        private void GetCurrentLookDirAndRot(out Vector3 lookPoint, out Vector3 dir, out Vector3 rot)
        {
            lookPoint = VR.Camera.Head.TransformPoint(Vector3.forward * DEFAULT_DISTANCE * DISTANCE_RATIO);
            Vector3 vector = lookPoint;
            vector.y = VR.Camera.Head.position.y;
            dir = vector - VR.Camera.Head.position;
            if(dir == Vector3.zero)
            {
                dir = Vector3.forward;
            }
            rot = Quaternion.LookRotation(dir).eulerAngles;
        }

        public void MoveToCamera(int slot)
        {
            var cameraData = studio.sceneInfo.cameraData[slot];
            studio.cameraCtrl.Import(cameraData);
            MoveToCurrent();
        }

        public void MoveToCurrent()
        {
            var cameraData = studio.cameraCtrl.Export();
            Vector3 tobeHeadPos = cameraData.pos + Quaternion.Euler(cameraData.rotate) * cameraData.distance;
            Quaternion tobeHeadRot = Quaternion.Euler(cameraData.rotate);
            MoveTo(tobeHeadPos, tobeHeadRot);
        }

        public void MoveTo(Vector3 tobeHeadPos, Quaternion tobeHeadRot)
        {
            GameObject vrorigin = GetVROrigin();
            if(vrorigin == null)
            {
                return;
            }
            Transform parent = vrorigin.transform.parent;
            moveDummy.transform.position = VR.Camera.Head.position;
            moveDummy.transform.rotation = GripMoveKKCharaStudioTool.RemoveXZRot(VR.Camera.Head.rotation);
            vrorigin.transform.parent = moveDummy.transform;
            moveDummy.transform.position = tobeHeadPos;
            moveDummy.transform.rotation = tobeHeadRot;
            vrorigin.transform.parent = parent;
            vrorigin.transform.rotation = GripMoveKKCharaStudioTool.RemoveXZRot(vrorigin.transform.rotation);
        }

        private GameObject GetVROrigin()
        {
            if(VR.Camera && VR.Camera.SteamCam && VR.Camera.SteamCam.origin)
            {
                return VR.Camera.SteamCam.origin.gameObject;
            }
            return null;
        }

        public void MoveToSelectedObject(bool lockY)
        {
            ObjectCtrlInfo[] selectObjectCtrl = Singleton<Studio.Studio>.Instance.treeNodeCtrl.selectObjectCtrl;
            if(selectObjectCtrl != null && selectObjectCtrl.Length != 0)
            {
                ObjectCtrlInfo objectCtrlInfo = selectObjectCtrl[0];
                Vector3 position = objectCtrlInfo.guideObject.transformTarget.position;
                if(objectCtrlInfo is OCIChar)
                {
                    position = (objectCtrlInfo as OCIChar).charInfo.objHead.transform.position;
                }
                MoveToPoint(position, lockY);
            }
        }

        public void MoveToPoint(Vector3 targetPos, bool lockY)
        {
            GetCurrentLookDirAndRot(out Vector3 vector, out Vector3 vector2, out Vector3 vector3);
            Vector3 vector4 = targetPos - vector2.normalized * 0.5f;
            if(lockY)
            {
                vector4.y = VR.Camera.Head.position.y;
            }
            else
            {
                vector4.y += VR.Camera.Head.position.y - vector.y;
            }
            MoveTo(vector4, Quaternion.Euler(vector3));
        }

        public void MoveForwardBackward(float distance)
        {
            GetCurrentLookDirAndRot(out _, out Vector3 vector2, out Vector3 vector3);
            Vector3 tobeHeadPos = VR.Camera.Head.position + vector2 * distance;
            tobeHeadPos.y = VR.Camera.Head.position.y;
            MoveTo(tobeHeadPos, Quaternion.Euler(vector3));
        }

        private void Init(Transform cameraMenuRootT)
        {
            VRLog.Info("Initializing VRCameraMoveHelper", new object[0]);
            try
            {
                menuRect = cameraMenuRootT.GetComponent<RectTransform>();
                if(moveDummy == null)
                {
                    moveDummy = new GameObject("MoveDummy");
                    DontDestroyOnLoad(moveDummy);
                    moveDummy.transform.parent = base.gameObject.transform;
                }
                for(int i = 0; i < menuRect.childCount; i++)
                {
                    Transform child = menuRect.GetChild(i);
                    int idx = -1;
                    if(int.TryParse(child.name, out idx))
                    {
                        child.Find("Button Save").gameObject.GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            OnSaveButtonClick(idx);
                        });
                        child.Find("Button Load").gameObject.GetComponent<Button>().onClick.AddListener(delegate ()
                        {
                            OnLoadButtonClick(idx);
                        });
                    }
                    else
                    {
                        VRLog.Info("Not Found. {0}", child.name);
                    }
                }
            }
            catch(Exception obj)
            {
                VRLog.Error(obj);
            }
            VRLog.Info("VR Camera Helper installed.");
        }

        private void OnSaveButtonClick(int idx)
        {
            SaveCamera(idx);
        }

        private void OnLoadButtonClick(int idx)
        {
            MoveToCamera(idx);
        }
    }
}
