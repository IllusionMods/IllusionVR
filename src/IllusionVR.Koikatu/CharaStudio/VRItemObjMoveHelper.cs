using Studio;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRGIN.Core;
using VRGIN.Helpers;

namespace IllusionVR.Koikatu.CharaStudio
{
    public class VRItemObjMoveHelper : MonoBehaviour
    {
        public bool showGUI = true;
        public RectTransform menuRect;
        private Canvas workspaceCanvas;
        private static VRItemObjMoveHelper _instance;
        public bool keepY = true;
        public bool moveAlong;
        public Vector3 moveAlongBasePos;
        public Quaternion moveAlongBaseRot;
        private ObjMoveHelper helper = new ObjMoveHelper();
        private GameObject steamVRHeadOrigin;
        private Studio.Studio studio;
        private GameObject moveDummy;
        private int windowID = 8751;
        private const int panelWidth = 300;
        private const int panelHeight = 150;
        private Rect windowRect = new Rect(0f, 0f, 300f, 150f);
        private string windowTitle = "";
        private Texture2D bgTex;
        private Button callButton;
        private Button callXZButton;
        private static FieldInfo f_m_TreeNodeObject = typeof(TreeNodeCtrl).GetField("m_TreeNodeObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);
        private static MethodInfo m_AddSelectNode = typeof(TreeNodeCtrl).GetMethod("AddSelectNode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
        public static VRItemObjMoveHelper Instance => _instance;

        public static void Install(GameObject container)
        {
            if(_instance == null)
            {
                _instance = container.AddComponent<VRItemObjMoveHelper>();
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            studio = Singleton<Studio.Studio>.Instance;
            if(studio == null)
            {
                return;
            }
            Transform objectListCanvas = studio.gameObject.transform.Find("Canvas Object List");
            _instance.Init(objectListCanvas);
            bgTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            bgTex.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f, 1f));
            bgTex.Apply();
        }

        private Texture2D LoadImage(string path)
        {
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            byte[] array = File.ReadAllBytes(path);
            texture2D.LoadImage(array);
            return texture2D;
        }

        public static Rect RectTransformToScreenSpace(RectTransform transform)
        {
            Vector2 vector = Vector2.Scale(transform.rect.size, transform.lossyScale);
            Rect result = new Rect(transform.position.x, Screen.height - transform.position.y, vector.x, vector.y);
            result.x -= transform.pivot.x * vector.x;
            result.y -= (1f - transform.pivot.y) * vector.y;
            return result;
        }

        private void Init(Transform objectListCanvas)
        {
            workspaceCanvas = objectListCanvas.gameObject.GetComponent<Canvas>();
            menuRect = objectListCanvas.Find("Image Bar/Scroll View").gameObject.GetComponent<RectTransform>();
            steamVRHeadOrigin = VR.Camera.SteamCam.origin.gameObject;
            if(moveDummy == null)
            {
                moveDummy = new GameObject("MoveDummy");
                DontDestroyOnLoad(moveDummy);
                moveDummy.transform.parent = gameObject.transform;
            }
            Transform transform = objectListCanvas.Find("Image Bar/Button Duplicate");
            Transform transform2 = objectListCanvas.Find("Image Bar/Button Route");
            Transform transform3 = objectListCanvas.Find("Image Bar/Button Camera");
            if(transform != null)
            {
                float num = transform3.localPosition.x - transform2.localPosition.x;
                Sprite sprite = Sprite.Create(UnityHelper.LoadImage("icon_call.png"), new Rect(0f, 0f, 32f, 32f), Vector2.zero);
                Sprite sprite2 = Sprite.Create(UnityHelper.LoadImage("icon_call_xz.png"), new Rect(0f, 0f, 32f, 32f), Vector2.zero);
                if(callButton == null)
                {
                    GameObject gameObject = Instantiate<GameObject>(transform.gameObject);
                    gameObject.name = "Button Call";
                    gameObject.transform.SetParent(transform.transform.parent);
                    gameObject.transform.localPosition = new Vector3(transform2.localPosition.x - num * 2f, transform2.localPosition.y, transform2.localPosition.z);
                    gameObject.transform.localScale = Vector3.one;
                    Button component = gameObject.GetComponent<Button>();
                    SpriteState spriteState = default;
                    spriteState.disabledSprite = sprite;
                    spriteState.highlightedSprite = sprite;
                    spriteState.pressedSprite = sprite;
                    component.spriteState = spriteState;
                    component.onClick = new Button.ButtonClickedEvent();
                    component.onClick.AddListener(new UnityAction(OnCallClick));
                    component.interactable = true;
                    callButton = component;
                    DestroyImmediate(gameObject.GetComponent<Image>());
                    Image image = gameObject.AddComponent<Image>();
                    image.sprite = sprite;
                    image.type = 0;
                    image.SetAllDirty();
                }
                if(callXZButton == null)
                {
                    GameObject gameObject2 = Instantiate<GameObject>(transform.gameObject);
                    gameObject2.name = "Button Call YLock";
                    gameObject2.transform.SetParent(transform.transform.parent);
                    gameObject2.transform.localPosition = new Vector3(transform2.localPosition.x - num, transform2.localPosition.y, transform2.localPosition.z);
                    gameObject2.transform.localScale = Vector3.one;
                    Button component2 = gameObject2.GetComponent<Button>();
                    SpriteState spriteState2 = default;
                    spriteState2.disabledSprite = sprite2;
                    spriteState2.highlightedSprite = sprite2;
                    spriteState2.pressedSprite = sprite2;
                    component2.spriteState = spriteState2;
                    component2.onClick = new Button.ButtonClickedEvent();
                    component2.onClick.AddListener(new UnityAction(OnCallClickYLock));
                    component2.interactable = true;
                    callXZButton = component2;
                    DestroyImmediate(gameObject2.GetComponent<Image>());
                    Image image2 = gameObject2.AddComponent<Image>();
                    image2.sprite = sprite2;
                    image2.type = 0;
                    image2.SetAllDirty();
                }
            }
            VRLog.Debug("VR ItemObjMoveHelper installed", new object[0]);
        }

        private void OnCallClick()
        {
            MoveAllCharaAndItemsHere(false);
        }

        private void OnCallClickYLock()
        {
            MoveAllCharaAndItemsHere(true);
        }

        public void CallCurrentObject(bool keepY = false)
        {
            if(studio.treeNodeCtrl.selectObjectCtrl != null && studio.treeNodeCtrl.selectObjectCtrl.Length != 0)
            {
                ObjectCtrlInfo objectCtrlInfo = studio.treeNodeCtrl.selectObjectCtrl[0];
                if(objectCtrlInfo != null)
                {
                    MoveObjectHere(objectCtrlInfo);
                }
            }
        }

        public void MoveAllCharaAndItemsHere(bool keepY = false)
        {
            Vector3 newPos = VR.Camera.Head.TransformPoint(0f, 0f, 0.2f);
            ObjectCtrlInfo firstObject = helper.GetFirstObject();
            if(firstObject != null)
            {
                helper.moveAlongBasePos = firstObject.guideObject.transformTarget.position;
                helper.MoveAllCharaAndItemsHere(newPos, keepY);
                moveAlongBasePos = newPos;
            }
        }

        public void MoveObjectHere(ObjectCtrlInfo oci)
        {
            Vector3 newPos = VR.Camera.Head.TransformPoint(0f, 0f, 0.2f);
            helper.MoveObject(oci, newPos, keepY);
        }

        public void VRToggleObjectSelectOnCursor()
        {
            var instance = Singleton<Studio.Studio>.Instance;
            if(instance == null)
            {
                return;
            }
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> list = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, list);
            foreach(RaycastResult raycastResult in list)
            {
                if(raycastResult.gameObject != null && raycastResult.gameObject.transform.parent != null)
                {
                    TreeNodeObject component = raycastResult.gameObject.transform.parent.gameObject.GetComponent<TreeNodeObject>();
                    if(component != null && instance.dicInfo.ContainsKey(component))
                    {
                        m_AddSelectNode.Invoke(instance.treeNodeCtrl, new object[]
                        {
                            component,
                            true
                        });
                        break;
                    }
                }
            }
        }
    }
}
