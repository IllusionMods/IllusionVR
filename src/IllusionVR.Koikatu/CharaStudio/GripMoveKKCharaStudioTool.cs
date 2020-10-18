using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Studio;
using UnityEngine;
using Valve.VR;
using VRGIN.Controls;
using VRGIN.Controls.Handlers;
using VRGIN.Controls.Tools;
using VRGIN.Core;
using VRGIN.Helpers;
using VRGIN.Visuals;

namespace KKCharaStudioVR
{
	internal class GripMoveKKCharaStudioTool : Tool
	{
		private GUIQuad internalGui;

		private float pressDownTime;

		private Vector2 touchDownPosition;

		private float menuDownTime;

		private float touchpadDownTime;

		private double _DeltaX;

		private double _DeltaY;

		private EVRButtonId moveSelfButton = EVRButtonId.k_EButton_Grip;

		private EVRButtonId grabScreenButton = EVRButtonId.k_EButton_Axis1;

		private string moveSelfButtonName = "rgrip";

		private KKCharaStudioVRSettings _settings;

		private float triggerDownTime;

		private float gripDownTime;

		private GameObject mirror1;

		private GameObject grabHandle;

		private GameObject pointer;

		private bool screenGrabbed;

		private GameObject lastGrabbedObject;

		private GameObject grabbingObject;

		private MenuHandler menuHandlder;

		private GripMenuHandler gripMenuHandler;

		private IKTool ikTool;

		private float nearestGrabable = float.MaxValue;

		private string[] FINGER_KEYS = new string[]
		{
			"j_thumb",
			"j_index",
			"j_middle",
			"j_ring",
			"j_little"
		};

		private static FieldInfo f_dicGuideObject = typeof(GuideObjectManager).GetField("dicGuideObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		private GameObject marker;

		public GameObject target;

		private bool lockRotXZ = true;

		public override Texture2D Image
		{
			get
			{
				return UnityHelper.LoadImage("icon_gripmove.png");
			}
		}

		public GUIQuad Gui { get; private set; }

		private void resetGUIPosition()
		{
			Transform head = VR.Camera.Head;
			internalGui.transform.parent = base.transform;
			internalGui.transform.localScale = Vector3.one * 0.4f;
			if (head != null)
			{
				internalGui.transform.position = head.TransformPoint(new Vector3(0f, 0f, 0.3f));
				internalGui.transform.rotation = Quaternion.LookRotation(head.TransformVector(new Vector3(0f, 0f, 1f)));
			}
			else
			{
				internalGui.transform.localPosition = new Vector3(0f, 0.05f, -0.06f);
				internalGui.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
			}
			internalGui.transform.parent = base.transform.parent;
			internalGui.UpdateAspect();
		}

		private void CreatePointer()
		{
			if (pointer == null)
			{
				pointer = GameObject.CreatePrimitive(0);
				pointer.name = "pointer";
				pointer.GetComponent<SphereCollider>();
				pointer.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
				pointer.transform.parent = base.transform;
				pointer.transform.localPosition = new Vector3(0f, -0.03f, 0.03f);
				Renderer component = pointer.GetComponent<Renderer>();
				component.enabled = true;
				Material material = new Material(MaterialHelper.GetColorZOrderShader());
				component.material = material;
			}
		}

		protected override void OnDestroy()
		{
			if (marker != null)
			{
				Destroy(marker);
			}
			if (mirror1 != null)
			{
				Destroy(mirror1);
			}
			if (grabHandle != null)
			{
				Destroy(grabHandle);
			}
			if (internalGui != null)
			{
				DestroyImmediate(internalGui.gameObject);
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			try
			{
				VRLog.Info("Loading GripMoveTool", new object[0]);
				_settings = (VR.Manager.Context.Settings as KKCharaStudioVRSettings);
				internalGui = GUIQuad.Create(null);
				resetGUIPosition();
				internalGui.gameObject.AddComponent<MoveableGUIObject>();
				internalGui.gameObject.AddComponent<BoxCollider>();
				internalGui.IsOwned = true;
				DontDestroyOnLoad(internalGui.gameObject);
				CreatePointer();
				gripMenuHandler = base.gameObject.AddComponent<GripMenuHandler>();
				gripMenuHandler.enabled = false;
			}
			catch (Exception obj)
			{
				VRLog.Info(obj);
			}
			if (marker == null)
			{
				marker = new GameObject("__GripMoveMarker__");
				marker.transform.parent = base.transform.parent;
				marker.transform.position = base.transform.position;
				marker.transform.rotation = base.transform.rotation;
			}
			if (_settings != null)
			{
				moveSelfButton = EVRButtonId.k_EButton_Grip;
				moveSelfButtonName = "rgrip";
				grabScreenButton = EVRButtonId.k_EButton_Axis1;
			}
			menuHandlder = base.GetComponent<MenuHandler>();
			ikTool = IKTool.instance;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (gripMenuHandler != null)
			{
				gripMenuHandler.enabled = false;
			}
			if (menuHandlder != null)
			{
				menuHandlder.enabled = true;
			}
			if (internalGui)
			{
				internalGui.gameObject.SetActive(false);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (gripMenuHandler != null)
			{
				gripMenuHandler.enabled = true;
			}
			if (menuHandlder != null)
			{
				menuHandlder.enabled = false;
			}
			if (internalGui)
			{
				internalGui.gameObject.SetActive(true);
			}
		}

		protected override void OnLevel(int level)
		{
			base.OnLevel(level);
			base.StopAllCoroutines();
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			if (controller != null)
			{
				if (controller.GetPressDown(EVRButtonId.k_EButton_Axis1))
				{
					triggerDownTime = Time.time;
				}
				if (controller.GetPressDown(EVRButtonId.k_EButton_Grip))
				{
					gripDownTime = Time.time;
				}
				if (controller.GetPressDown(EVRButtonId.k_EButton_ApplicationMenu))
				{
					menuDownTime = Time.time;
				}
				if (controller.GetPressDown(EVRButtonId.k_EButton_Axis0) || controller.GetPressDown(EVRButtonId.k_EButton_A))
				{
					touchpadDownTime = Time.time;
				}
				if (controller.GetPress(EVRButtonId.k_EButton_Axis1) && controller.GetPress(EVRButtonId.k_EButton_Grip) && controller.GetPress(EVRButtonId.k_EButton_ApplicationMenu) && Time.time - menuDownTime > 0.5f)
				{
					lockRotXZ = !lockRotXZ;
					if (lockRotXZ)
					{
						ResetRotation();
					}
				}
				if (controller.GetPress(EVRButtonId.k_EButton_ApplicationMenu) && Time.time - menuDownTime > 1.5f)
				{
					resetGUIPosition();
					menuDownTime = Time.time;
				}
				if (controller.GetPressDown(EVRButtonId.k_EButton_Axis0) || controller.GetPressDown(EVRButtonId.k_EButton_A))
				{
					controller.GetPress(EVRButtonId.k_EButton_Grip);
				}
				bool pressDown = controller.GetPressDown(grabScreenButton);
				bool press = controller.GetPress(grabScreenButton);
				bool pressUp = controller.GetPressUp(grabScreenButton);
				if (grabHandle == null)
				{
					grabHandle = new GameObject("__GripMoveGrabHandle__");
					grabHandle.transform.parent = base.transform;
					grabHandle.transform.position = base.transform.position;
					grabHandle.transform.rotation = base.transform.rotation;
				}
				if (pressDown && screenGrabbed && lastGrabbedObject != null && grabHandle != null)
				{
					grabbingObject = lastGrabbedObject;
					grabHandle.transform.position = lastGrabbedObject.transform.position;
					grabHandle.transform.rotation = lastGrabbedObject.transform.rotation;
					if (lastGrabbedObject.GetComponent<MoveableGUIObject>() != null)
					{
						Transform parent = lastGrabbedObject.transform.parent;
						MoveableGUIObject component = lastGrabbedObject.GetComponent<MoveableGUIObject>();
						if (component.guideObject != null)
						{
							ApplyFingerFKIfNeeded(component.guideObject);
							grabHandle.transform.rotation = component.guideObject.transformTarget.rotation;
							grabbingObject.transform.rotation = component.guideObject.transformTarget.rotation;
							component.OnMoveStart();
						}
					}
				}
				bool flag = false;
				if ((controller.GetPressDown(EVRButtonId.k_EButton_Axis0) || controller.GetPressDown(EVRButtonId.k_EButton_A)) && lastGrabbedObject != null && lastGrabbedObject.GetComponent<MoveableGUIObject>() != null)
				{
					GuideObject guideObject = lastGrabbedObject.GetComponent<MoveableGUIObject>().guideObject;
					if (guideObject != null)
					{
						if (guideObject.guideSelect != null && guideObject.guideSelect.treeNodeObject != null)
						{
							guideObject.guideSelect.treeNodeObject.OnClickSelect();
						}
						else
						{
							Singleton<GuideObjectManager>.Instance.selectObject = guideObject;
						}
						flag = true;
					}
				}
				if (controller.GetPressDown(EVRButtonId.k_EButton_Axis0) || (controller.GetPressDown(EVRButtonId.k_EButton_A) && !flag))
				{
					VRLog.Info("Called on Select VRToggle", new object[0]);
					if (gripMenuHandler && gripMenuHandler.LaserVisible)
					{
						VRItemObjMoveHelper.Instance.VRToggleObjectSelectOnCursor();
					}
				}
				if (press && grabbingObject != null)
				{
					grabbingObject.transform.position = grabHandle.transform.position;
					grabbingObject.transform.rotation = grabHandle.transform.rotation;
					if (grabbingObject.GetComponent<MoveableGUIObject>() != null)
					{
						grabbingObject.GetComponent<MoveableGUIObject>().OnMoved();
					}
				}
				if (screenGrabbed && grabbingObject != null && pressUp)
				{
					if (grabbingObject.GetComponent<MoveableGUIObject>() != null)
					{
						grabbingObject.GetComponent<MoveableGUIObject>().OnReleased();
					}
					grabbingObject = null;
				}
				if (controller.GetPress(moveSelfButton) && grabbingObject == null)
				{
					target = VR.Camera.SteamCam.origin.gameObject;
					if (target != null)
					{
						if (mirror1 == null)
						{
							mirror1 = new GameObject("__GripMoveMirror1__");
							mirror1.transform.position = base.transform.position;
							mirror1.transform.rotation = base.transform.rotation;
						}
						Vector3 vector = marker.transform.position - base.transform.position;
						Quaternion q = marker.transform.rotation * Quaternion.Inverse(base.transform.rotation);
						Quaternion quaternion = RemoveLockedAxisRot(q);
						Transform parent2 = target.transform.parent;
						mirror1.transform.position = base.transform.position;
						mirror1.transform.rotation = base.transform.rotation;
						target.transform.parent = mirror1.transform;
						mirror1.transform.rotation = quaternion * mirror1.transform.rotation;
						mirror1.transform.position = mirror1.transform.position + vector;
						target.transform.parent = parent2;
					}
				}
				lastGrabbedObject = null;
				nearestGrabable = float.MaxValue;
				marker.transform.position = base.transform.position;
				marker.transform.rotation = base.transform.rotation;
			}
		}

		private void ApplyFingerFKIfNeeded(GuideObject guideObject)
		{
			new List<Transform>();
			List<GuideObject> list = new List<GuideObject>();
			if (IsFinger(guideObject.transformTarget))
			{
				list.Add(guideObject);
			}
			foreach (GuideObject guideObject2 in list)
			{
				guideObject2.transformTarget.localEulerAngles = guideObject2.changeAmount.rot;
			}
		}

		private bool IsFinger(Transform t)
		{
			foreach (string value in FINGER_KEYS)
			{
				if (t.name.Contains(value))
				{
					return true;
				}
			}
			return false;
		}

		public override List<HelpText> GetHelpTexts()
		{
			return new List<HelpText>(new HelpText[]
			{
				HelpText.Create("Swipe as wheel.", base.FindAttachPosition(new string[]
				{
					"touchpad"
				}), new Vector3(0.06f, 0.04f, 0f), null),
				HelpText.Create("Grip and move controller to move yourself", base.FindAttachPosition(new string[]
				{
					"rgrip"
				}), new Vector3(0.06f, 0.04f, 0f), null),
				HelpText.Create("Trigger to grab objects / IK markers and move them along with controller.", base.FindAttachPosition(new string[]
				{
					"trigger"
				}), new Vector3(-0.06f, -0.04f, 0f), null)
			});
		}

		private SteamVR_Controller.Device controller
		{
			get
			{
				SteamVR_TrackedObject component = base.gameObject.GetComponent<SteamVR_TrackedObject>();
				if (component != null)
				{
					return SteamVR_Controller.Input((int)component.index);
				}
				return null;
			}
		}

		private void ResetRotation()
		{
			if (target != null)
			{
				Vector3 eulerAngles = target.transform.rotation.eulerAngles;
				eulerAngles.x = 0f;
				eulerAngles.z = 0f;
				target.transform.rotation = Quaternion.Euler(eulerAngles);
			}
		}

		private IEnumerator UpdateMarkerPos()
		{
			yield return new WaitForEndOfFrame();
			marker.transform.position = transform.position;
			marker.transform.rotation = transform.rotation;
			yield break;
		}

		private Quaternion RemoveLockedAxisRot(Quaternion q)
		{
			if (lockRotXZ)
			{
				return GripMoveKKCharaStudioTool.RemoveXZRot(q);
			}
			return q;
		}

		public static Quaternion RemoveXZRot(Quaternion q)
		{
			Vector3 eulerAngles = q.eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			return Quaternion.Euler(eulerAngles);
		}

		private void OnTriggerStay(Collider collider)
		{
			if (collider.GetComponent<GUIQuad>() != null)
			{
				screenGrabbed = true;
				lastGrabbedObject = collider.gameObject;
			}
			else if (collider.GetComponent<MoveableGUIObject>() != null)
			{
				screenGrabbed = true;
				if (lastGrabbedObject != null)
				{
					float sqrMagnitude = (collider.gameObject.transform.position - pointer.transform.position).sqrMagnitude;
					if (sqrMagnitude < nearestGrabable)
					{
						lastGrabbedObject = collider.gameObject;
						nearestGrabable = sqrMagnitude;
					}
				}
				else
				{
					lastGrabbedObject = collider.gameObject;
				}
			}
			if (screenGrabbed && lastGrabbedObject != null && pointer != null)
			{
				pointer.GetComponent<MeshRenderer>().material.color = Color.red;
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
		}

		private void OnTriggerExit(Collider collider)
		{
			GameObject gameObject = collider.gameObject;
			if (screenGrabbed && collider.GetComponent<MoveableGUIObject>() != null && gameObject == lastGrabbedObject)
			{
				pointer.GetComponent<MeshRenderer>().material.color = Color.white;
				screenGrabbed = false;
				lastGrabbedObject = null;
			}
		}
	}
}
