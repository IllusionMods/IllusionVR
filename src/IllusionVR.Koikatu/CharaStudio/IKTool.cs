using Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KKCharaStudioVR
{
    public class IKTool : MonoBehaviour
    {
        private bool markerShowOverlay = true;

        private float markerOverlayAlpha = 0.4f;

        private float markerSize = 0.06f;

        private float markerSizeBend = 0.05f;

        private Material markerSharedMaterial;

        private Material markerSharedMaterial_IKTarget;

        private Material markerSharedMaterial_IKBendTarget;

        private GameObject handle;

        public static IKTool instance;

        private const float DEFAUTL_SCALE_POS = 0.25f;

        private float DEFAULT_SCALE_POS_XYZ_DIST = Mathf.Sqrt(0.1875f);

        public static IKTool Create(GameObject container)
        {
            if(instance != null)
            {
                return instance;
            }
            instance = container.AddComponent<IKTool>();
            return instance;
        }

        private void Awake()
        {
        }

        private void Start()
        {
            StartWatch();
        }

        private IEnumerator InstallMoveableObjectCo()
        {
            var studio = Singleton<Studio.Studio>.Instance;
            GuideObjectManager guideObjectManager = Singleton<GuideObjectManager>.Instance;
            for(; ; )
            {
                yield return new WaitForSeconds(1f);
                try
                {
                    foreach(KeyValuePair<int, ObjectCtrlInfo> keyValuePair in studio.dicObjectCtrl)
                    {
                        if(keyValuePair.Value.guideObject.gameObject != null)
                        {
                            ObjectCtrlInfo value = keyValuePair.Value;
                            MakeObjectMoveable(value.guideObject, true, true);
                            if(value is OCIChar)
                            {
                                OCIChar ocichar = value as OCIChar;
                                foreach(OCIChar.IKInfo ikinfo in ocichar.listIKTarget)
                                {
                                    MakeObjectMoveable(ikinfo.guideObject, true, false);
                                }
                                foreach(OCIChar.BoneInfo boneInfo in ocichar.listBones)
                                {
                                    MakeObjectMoveable(boneInfo.guideObject, true, false);
                                }
                            }
                        }
                    }
                    continue;
                }
                catch(Exception value2)
                {
                    Console.WriteLine(value2);
                    continue;
                }
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            StartWatch();
        }

        private void StartWatch()
        {
            StopAllCoroutines();
            StartCoroutine(InstallMoveableObjectCo());
            if(handle == null)
            {
                handle = new GameObject("handle");
                handle.transform.parent = gameObject.transform;
            }
        }

        private void MakeObjectMoveable(GuideObject guideObject, bool replaceMaterial = false, bool installToCenter = false)
        {
            if(guideObject.transformTarget == null)
            {
                return;
            }
            if(installToCenter)
            {
                InstallGripMoveMarker(guideObject.gameObject, new Action<MonoBehaviour>(OnObjectMove), guideObject, replaceMaterial, installToCenter);
            }
            else
            {
                GameObject gameObject = guideObject.gameObject.transform.Find("Sphere").gameObject;
                InstallGripMoveMarker(gameObject, new Action<MonoBehaviour>(OnObjectMove), guideObject, replaceMaterial, installToCenter);
            }
            if(guideObject.enableScale)
            {
                InstallScaleMoveMarker(guideObject);
            }
        }

        private bool InstallGripMoveMarker(GameObject target, Action<MonoBehaviour> moveHandler, GuideObject guideObject, bool replaceMaterial, bool installToCenter)
        {
            if(target.transform.Find("_gripmovemarker") == null)
            {
                Renderer visibleReference = null;
                GameObject gameObject;
                if(installToCenter)
                {
                    gameObject = GameObject.CreatePrimitive(0);
                    gameObject.name = "_gripmovemarker";
                    gameObject.layer = LayerMask.NameToLayer("Studio/Select");
                    Renderer component = gameObject.GetComponent<Renderer>();
                    Material material = new Material(MaterialHelper.GetColorZOrderShader());
                    material.color = new Color(0f, 1f, 0f, 0.3f);
                    material.SetFloat("_AlphaRatio", 1.5f);
                    material.renderQueue = 3800;
                    component.material = material;
                    Transform transform = target.transform.Find("move/XYZ");
                    if(transform != null)
                    {
                        visibleReference = transform.gameObject.GetComponent<Renderer>();
                    }
                }
                else
                {
                    gameObject = new GameObject("_gripmovemarker");
                    if(replaceMaterial)
                    {
                        Renderer component2 = target.GetComponent<Renderer>();
                        if(component2 != null)
                        {
                            Material material2 = new Material(MaterialHelper.GetColorZOrderShader());
                            material2.CopyPropertiesFromMaterial(component2.material);
                            material2.SetFloat("_AlphaRatio", 1.5f);
                            material2.renderQueue = 3800;
                            component2.material = material2;
                            visibleReference = component2;
                        }
                    }
                }
                SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
                Transform transform2 = gameObject.transform;
                transform2.transform.parent = target.transform;
                transform2.transform.localPosition = Vector3.zero;
                transform2.transform.rotation = guideObject.transformTarget.rotation;
                transform2.transform.localScale = Vector3.one;
                sphereCollider.isTrigger = true;
                MoveableGUIObject moveableGUIObject = gameObject.AddComponent<MoveableGUIObject>();
                moveableGUIObject.guideObject = guideObject;
                moveableGUIObject.onMoveLister.Add(moveHandler);
                moveableGUIObject.visibleReference = visibleReference;
                if(installToCenter)
                {
                    moveableGUIObject.isMoveObj = true;
                }
                return true;
            }
            return false;
        }

        private bool InstallScaleMoveMarker(GuideObject guideObject)
        {
            Transform transform = guideObject.gameObject.transform.Find("scale");
            if(transform.transform.Find("X/_gripmovemarker_scale") == null)
            {
                foreach(string text in new string[]
                {
                    "XYZ",
                    "X",
                    "Y",
                    "Z"
                })
                {
                    Transform transform2 = transform.Find(text);
                    GuideScale component = transform2.gameObject.GetComponent<GuideScale>();
                    if(component != null)
                    {
                        GameObject gameObject = GameObject.CreatePrimitive(0);
                        gameObject.name = "_gripmovemarker_scale";
                        gameObject.layer = LayerMask.NameToLayer("Studio/Select");
                        Renderer component2 = gameObject.GetComponent<Renderer>();
                        Material material = new Material(MaterialHelper.GetColorZOrderShader());
                        material.color = new Color(0f, 1f, 1f, 0.3f);
                        material.SetFloat("_AlphaRatio", 1.5f);
                        material.renderQueue = 3800;
                        component2.material = material;
                        Renderer component3 = transform2.gameObject.GetComponent<Renderer>();
                        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
                        Transform transform3 = gameObject.transform;
                        transform3.transform.parent = transform2;
                        transform3.transform.localPosition = CalcScaleHandleDefaultPos(component);
                        transform3.transform.rotation = guideObject.transformTarget.rotation;
                        transform3.transform.localScale = Vector3.one;
                        sphereCollider.isTrigger = true;
                        MoveableGUIObject moveableGUIObject = gameObject.AddComponent<MoveableGUIObject>();
                        moveableGUIObject.guideObject = guideObject;
                        moveableGUIObject.guideScale = component;
                        moveableGUIObject.onMoveLister.Add(new Action<MonoBehaviour>(OnScaleMove));
                        moveableGUIObject.onReleasedLister.Add(new Action<MonoBehaviour>(OnScaleReleased));
                        moveableGUIObject.visibleReference = component3;
                    }
                }
                return true;
            }
            return false;
        }

        private void OnObjectMove(MonoBehaviour marker)
        {
            DoOnMove(marker, marker.transform.parent, true, true);
        }

        private void OnObjectCubeMoveNoRotation(MonoBehaviour marker)
        {
            DoOnMove(marker, marker.transform.parent.parent, false, true);
        }

        private void OnObjectRotationNoMove(MonoBehaviour marker)
        {
            DoOnMove(marker, marker.transform.parent, true, false);
        }

        private void OnRawObjectMove(MonoBehaviour marker)
        {
            DoOnMove(marker, marker.transform.parent, true, true);
        }

        private void DoOnMove(MonoBehaviour marker, Transform target, bool rotation = true, bool pos = true)
        {
            MoveableGUIObject component = marker.GetComponent<MoveableGUIObject>();
            Transform parent = marker.transform.parent;
            GuideObject guideObject = component.guideObject;
            pos &= guideObject.enablePos;
            rotation &= guideObject.enableRot;
            if(pos)
            {
                target.position += marker.transform.position - parent.transform.position;
                guideObject.transformTarget.transform.position = target.position;
                guideObject.changeAmount.pos = guideObject.transformTarget.localPosition;
            }
            marker.transform.localPosition = Vector3.zero;
            if(rotation)
            {
                guideObject.transformTarget.rotation = marker.transform.rotation;
                guideObject.changeAmount.rot = guideObject.transformTarget.localEulerAngles;
            }
        }

        private void OnScaleMove(MonoBehaviour marker)
        {
            MoveableGUIObject component = marker.GetComponent<MoveableGUIObject>();
            Transform parent = marker.transform.parent;
            GuideObject guideObject = component.guideObject;
            GuideScale guideScale = component.guideScale;
            if(guideObject.enableScale && component.guideScale)
            {
                float magnitude = marker.transform.localPosition.magnitude;
                if(magnitude > 0f)
                {
                    float num = magnitude / 0.25f;
                    Vector3 vector = component.oldScale;
                    switch(guideScale.axis)
                    {
                        case GuideScale.ScaleAxis.X:
                            vector.x *= num;
                            break;
                        case GuideScale.ScaleAxis.Y:
                            vector.y *= num;
                            break;
                        case GuideScale.ScaleAxis.Z:
                            vector.z *= num;
                            break;
                        case GuideScale.ScaleAxis.XYZ:
                            vector *= num;
                            break;
                    }
                    vector.x = Mathf.Max(vector.x, 0.01f);
                    vector.y = Mathf.Max(vector.y, 0.01f);
                    vector.z = Mathf.Max(vector.z, 0.01f);
                    guideObject.changeAmount.scale = vector;
                }
            }
        }

        private void OnScaleReleased(MonoBehaviour marker)
        {
            MoveableGUIObject component = marker.GetComponent<MoveableGUIObject>();
            GuideObject guideObject = component.guideObject;
            GuideScale guideScale = component.guideScale;
            if(guideObject.enableScale && guideScale)
            {
                marker.transform.localPosition = CalcScaleHandleDefaultPos(guideScale);
            }
        }

        private Vector3 CalcScaleHandleDefaultPos(GuideScale guideScale)
        {
            switch(guideScale.axis)
            {
                case GuideScale.ScaleAxis.X:
                    return new Vector3(0.25f, 0f, 0f);
                case GuideScale.ScaleAxis.Y:
                    return new Vector3(0f, 0.25f, 0f);
                case GuideScale.ScaleAxis.Z:
                    return new Vector3(0f, 0f, 0.25f);
                case GuideScale.ScaleAxis.XYZ:
                    return new Vector3(0.25f, 0.25f, 0.25f) * 0.25f / DEFAULT_SCALE_POS_XYZ_DIST;
                default:
                    return Vector3.zero;
            }
        }

        private Material CreateForceDrawMaterial()
        {
            Material result;
            try
            {
                Material material = new Material(MaterialHelper.GetColorZOrderShader());
                Color red = Color.red;
                red.a = markerOverlayAlpha;
                material.SetColor("_Color", red);
                result = material;
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
