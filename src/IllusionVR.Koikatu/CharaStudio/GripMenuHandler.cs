using System;
using System.Linq;
using UnityEngine;
using Valve.VR;
using VRGIN.Controls;
using VRGIN.Core;
using VRGIN.Native;
using VRGIN.Visuals;

namespace IllusionVR.Koikatu.CharaStudio
{
    public class GripMenuHandler : ProtectedBehaviour
    {
        private Controller _Controller;
        private const float RANGE = 0.25f;
        private const int MOUSE_STABILIZER_THRESHOLD = 30;
        private LineRenderer Laser;
        private Vector2? mouseDownPosition;
        private GUIQuad _Target;
        private GripMenuHandler.ResizeHandler _ResizeHandler;
        private Vector3 _ScaleVector;

        protected override void OnStart()
        {
            base.OnStart();
            _Controller = GetComponent<Controller>();
            _ScaleVector = new Vector2(VRGUI.Width / (float)Screen.width, VRGUI.Height / (float)Screen.height);
            InitLaser();
        }

        protected SteamVR_Controller.Device Device => SteamVR_Controller.Input((int)_Controller.Tracking.index);

        private void InitLaser()
        {
            Laser = new GameObject().AddComponent<LineRenderer>();
            Laser.transform.SetParent(transform, false);
            Laser.material = Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
            Laser.material.renderQueue += 5000;
            Laser.SetColors(Color.cyan, Color.cyan);
            Laser.transform.localRotation = Quaternion.Euler(60f, 0f, 0f);
            Laser.transform.position += Laser.transform.forward * 0.07f;
            Laser.SetVertexCount(2);
            Laser.useWorldSpace = true;
            Laser.SetWidth(0.002f, 0.002f);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if(!VR.Camera.gameObject.activeInHierarchy)
            {
                return;
            }
            if(LaserVisible)
            {
                if(IsResizing)
                {
                    Laser.SetPosition(0, Laser.transform.position);
                    Laser.SetPosition(1, Laser.transform.position);
                }
                else
                {
                    UpdateLaser();
                }
            }
            else if(_Controller.CanAcquireFocus())
            {
                CheckForNearMenu();
            }
            CheckInput();
        }

        private void OnDisable()
        {
        }

        private void EnsureResizeHandler()
        {
            if(!_ResizeHandler)
            {
                _ResizeHandler = _Target.GetComponent<GripMenuHandler.ResizeHandler>();
                if(!_ResizeHandler)
                {
                    _ResizeHandler = _Target.gameObject.AddComponent<GripMenuHandler.ResizeHandler>();
                }
            }
        }

        private void EnsureNoResizeHandler()
        {
            if(_ResizeHandler)
            {
                DestroyImmediate(_ResizeHandler);
            }
            _ResizeHandler = null;
        }

        protected void CheckInput()
        {
            IsPressing = false;
            if(LaserVisible && _Target && !IsResizing)
            {
                if(Device.GetPressDown(EVRButtonId.k_EButton_Axis1))
                {
                    IsPressing = true;
                    VR.Input.Mouse.LeftButtonDown();
                    mouseDownPosition = new Vector2?(Vector2.Scale(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y), _ScaleVector));
                }
                if(Device.GetPress(EVRButtonId.k_EButton_Axis1))
                {
                    IsPressing = true;
                }
                if(Device.GetPressUp(EVRButtonId.k_EButton_Axis1))
                {
                    IsPressing = true;
                    VR.Input.Mouse.LeftButtonUp();
                    mouseDownPosition = null;
                }
            }
        }

        private bool IsResizing => _ResizeHandler && _ResizeHandler.IsDragging;

        private void CheckForNearMenu()
        {
            _Target = GUIQuadRegistry.Quads.FirstOrDefault(new Func<GUIQuad, bool>(IsLaserable));
            if(_Target)
            {
                LaserVisible = true;
            }
        }

        private bool IsLaserable(GUIQuad quad)
        {
            return IsWithinRange(quad) && Raycast(quad, out _);
        }

        private float GetRange(GUIQuad quad)
        {
            return Mathf.Clamp(quad.transform.localScale.magnitude * 0.25f, 0.25f, 1.25f);
        }

        private bool IsWithinRange(GUIQuad quad)
        {
            if(quad.transform.parent == transform)
                return false;

            Vector3 vector = -quad.transform.forward;
            Vector3 position2 = Laser.transform.position;
            Vector3 forward = Laser.transform.forward;
            float num = -quad.transform.InverseTransformPoint(position2).z * quad.transform.localScale.magnitude;
            return num > 0f && num < GetRange(quad) && Vector3.Dot(vector, forward) < 0f;
        }

        private bool Raycast(GUIQuad quad, out RaycastHit hit)
        {
            Vector3 position = Laser.transform.position;
            Vector3 forward = Laser.transform.forward;
            Collider component = quad.GetComponent<Collider>();
            if(component)
            {
                var ray = new Ray(position, forward);
                return component.Raycast(ray, out hit, GetRange(quad));
            }
            hit = default;
            return false;
        }

        private void UpdateLaser()
        {
            Laser.SetPosition(0, Laser.transform.position);
            Laser.SetPosition(1, Laser.transform.position + Laser.transform.forward);
            if(_Target && _Target.gameObject.activeInHierarchy)
            {
                if(!IsWithinRange(_Target) || !Raycast(_Target, out RaycastHit raycastHit))
                {
                    LaserVisible = false;
                    return;
                }
                Laser.SetPosition(1, raycastHit.point);
                if(!IsOtherWorkingOn(_Target))
                {
                    var vector = new Vector2(raycastHit.textureCoord.x * VRGUI.Width, (1f - raycastHit.textureCoord.y) * VRGUI.Height);
                    if(mouseDownPosition == null || Vector2.Distance(mouseDownPosition.Value, vector) > 30f)
                    {
                        MouseOperations.SetClientCursorPosition((int)vector.x, (int)vector.y);
                        mouseDownPosition = null;
                        return;
                    }
                }
            }
            else
            {
                LaserVisible = false;
            }
        }

        private bool IsOtherWorkingOn(GUIQuad target)
        {
            return false;
        }

        public bool LaserVisible
        {
            get => Laser && Laser.gameObject.activeSelf;
            set
            {
                Laser.gameObject.SetActive(value);
                if(value)
                {
                    Laser.SetPosition(0, Laser.transform.position);
                    Laser.SetPosition(1, Laser.transform.position);
                    return;
                }
                mouseDownPosition = null;
            }
        }

        public bool IsPressing { get; private set; }

        private class ResizeHandler : ProtectedBehaviour
        {
            private GUIQuad _Gui;
            private Vector3? _StartLeft;
            private Vector3? _StartRight;
            private Vector3? _StartScale;
            private Quaternion? _StartRotation;
            private Vector3? _StartPosition;
            private Quaternion _StartRotationController;
            private Vector3? _OffsetFromCenter;

            public bool IsDragging { get; private set; }

            protected override void OnStart()
            {
                base.OnStart();
                _Gui = GetComponent<GUIQuad>();
            }

            protected override void OnFixedUpdate()
            {
                base.OnFixedUpdate();
                IsDragging = (GetDevice(VR.Mode.Left).GetPress(EVRButtonId.k_EButton_Axis1) && GetDevice(VR.Mode.Right).GetPress(EVRButtonId.k_EButton_Axis1));
                if(IsDragging)
                {
                    if(_StartScale == null)
                    {
                        Initialize();
                    }
                    Vector3 position = VR.Mode.Left.transform.position;
                    Vector3 position2 = VR.Mode.Right.transform.position;
                    float num = Vector3.Distance(position, position2);
                    float num2 = Vector3.Distance(_StartLeft.Value, _StartRight.Value);
                    Vector3 vector = position2 - position;
                    Vector3 vector2 = position + vector * 0.5f;
                    Quaternion quaternion = Quaternion.Inverse(VR.Camera.SteamCam.origin.rotation);
                    Quaternion averageRotation = GetAverageRotation();
                    Quaternion quaternion2 = quaternion * averageRotation * Quaternion.Inverse(quaternion * _StartRotationController);
                    _Gui.transform.localScale = num / num2 * _StartScale.Value;
                    _Gui.transform.localRotation = quaternion2 * _StartRotation.Value;
                    _Gui.transform.position = vector2 + averageRotation * Quaternion.Inverse(_StartRotationController) * _OffsetFromCenter.Value;
                    return;
                }
                _StartScale = null;
            }

            private Quaternion GetAverageRotation()
            {
                Vector3 position = VR.Mode.Left.transform.position;
                Vector3 normalized = (VR.Mode.Right.transform.position - position).normalized;
                Vector3 vector = Vector3.Lerp(VR.Mode.Left.transform.forward, VR.Mode.Right.transform.forward, 0.5f);
                return Quaternion.LookRotation(Vector3.Cross(normalized, vector).normalized, vector);
            }

            private void Initialize()
            {
                _StartLeft = new Vector3?(VR.Mode.Left.transform.position);
                _StartRight = new Vector3?(VR.Mode.Right.transform.position);
                _StartScale = new Vector3?(_Gui.transform.localScale);
                _StartRotation = new Quaternion?(_Gui.transform.localRotation);
                _StartPosition = new Vector3?(_Gui.transform.position);
                _StartRotationController = GetAverageRotation();
                Vector3.Distance(_StartLeft.Value, _StartRight.Value);
                Vector3 vector = _StartRight.Value - _StartLeft.Value;
                Vector3 vector2 = _StartLeft.Value + vector * 0.5f;
                _OffsetFromCenter = new Vector3?(transform.position - vector2);
            }

            private SteamVR_Controller.Device GetDevice(Controller controller)
            {
                return SteamVR_Controller.Input((int)controller.Tracking.index);
            }
        }
    }
}
