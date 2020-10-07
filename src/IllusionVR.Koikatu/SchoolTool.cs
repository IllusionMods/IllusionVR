using System;
using System.Collections.Generic;
using UnityEngine;
using VRGIN.Controls;
using VRGIN.Controls.Tools;
using VRGIN.Core;
using VRGIN.Helpers;
using static SteamVR_Controller;
using WindowsInput.Native;
using KoikatuVR.Interpreters;

namespace KoikatuVR
{
    public class SchoolTool : Tool
    {
        private ActionSceneInterpreter _Interpreter;
        private KoikatuSettings _Settings;
        private KeySet _KeySet;
        private int _KeySetIndex = 0;

        // 手抜きのためNumpad方式で方向を保存
        private int _PrevTouchDirection = -1;
        private bool _Pl2Cam = false;

        private void ChangeKeySet()
        {
            List<KeySet> keySets = _Settings.KeySets;

            _KeySetIndex = (_KeySetIndex + 1) % keySets.Count;
            _KeySet = keySets[_KeySetIndex];
        }

        public override Texture2D Image
        {
            get
            {
                return UnityHelper.LoadImage("icon_school.png");
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            _Settings = (VR.Context.Settings as KoikatuSettings);
            _KeySet = _Settings.KeySets[0];
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnDestroy()
        {
            // nothing to do.
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _Interpreter = (VR.Interpreter as KoikatuInterpreter).SceneInterpreter as ActionSceneInterpreter;
        }

        protected override void OnLevel(int level)
        {
            base.OnLevel(level);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            var device = this.Controller;

            if (device.GetPressDown(ButtonMask.Trigger))
            {
                InputKey(_KeySet.Trigger, KeyMode.PressDown);
            }

            if (device.GetPressUp(ButtonMask.Trigger))
            {
                InputKey(_KeySet.Trigger, KeyMode.PressUp);
            }

            if (device.GetPressDown(ButtonMask.Grip))
            {
                InputKey(_KeySet.Grip, KeyMode.PressDown);
            }

            if (device.GetPressUp(ButtonMask.Grip))
            {
                InputKey(_KeySet.Grip, KeyMode.PressUp);
            }

            if (device.GetPressDown(ButtonMask.Touchpad))
            {
                Vector2 touchPosition = device.GetAxis();
                {
                    float threshold = _Settings.TouchpadThreshold;

                    if (touchPosition.y > threshold) // up
                    {
                        InputKey(_KeySet.Up, KeyMode.PressDown);
                        _PrevTouchDirection = 8;
                    }
                    else if (touchPosition.y < -threshold) // down
                    {
                        InputKey(_KeySet.Down, KeyMode.PressDown);
                        _PrevTouchDirection = 2;
                    }
                    else if (touchPosition.x > threshold) // right
                    {
                        InputKey(_KeySet.Right, KeyMode.PressDown);
                        _PrevTouchDirection = 6;
                    }
                    else if (touchPosition.x < -threshold)// left
                    {
                        InputKey(_KeySet.Left, KeyMode.PressDown);
                        _PrevTouchDirection = 4;
                    }
                    else
                    {
                        InputKey(_KeySet.Center, KeyMode.PressDown);
                        _PrevTouchDirection = 5;
                    }
                }
             }

            // 上げたときの位置によらず、押したボタンを離す
            if (device.GetPressUp(ButtonMask.Touchpad))
            {
                Vector2 touchPosition = device.GetAxis();
                {
                    if (_PrevTouchDirection == 8) // up
                    {
                        InputKey(_KeySet.Up, KeyMode.PressUp);
                    }
                    else if (_PrevTouchDirection == 2) // down
                    {
                        InputKey(_KeySet.Down, KeyMode.PressUp);
                    }
                    else if (_PrevTouchDirection == 6) // right
                    {
                        InputKey(_KeySet.Right, KeyMode.PressUp);
                    }
                    else if (_PrevTouchDirection == 4)// left
                    {
                        InputKey(_KeySet.Left, KeyMode.PressUp);
                    }
                    else if (_PrevTouchDirection == 5)
                    {
                        InputKey(_KeySet.Center, KeyMode.PressUp);
                    }
                }
            }

            if (_Pl2Cam)
            {
                _Interpreter.MovePlayerToCamera();
            }
        }

        private void InputKey(string keyName, KeyMode mode)
        {
            if (mode == KeyMode.PressDown)
            {
                switch (keyName)
                {
                    case "WALK":
                        _Interpreter.StartWalking();
                        break;
                    case "DASH":
                        _Interpreter.StartWalking(true);
                        break;
                    case "PL2CAM":
                        _Pl2Cam = true;
                        break;
                    case "LBUTTON":
                        VR.Input.Mouse.LeftButtonDown();
                        break;
                    case "RBUTTON":
                        VR.Input.Mouse.RightButtonDown();
                        break;
                    case "MBUTTON":
                        VR.Input.Mouse.MiddleButtonDown();
                        break;
                    case "LROTATION":
                    case "RROTATION":
                    case "NEXT":
                        // ここでは何もせず、上げたときだけ処理する
                        break;
                    case "CROUCH":
                        _Interpreter.Crouch();
                        break;
                    default:
                        VR.Input.Keyboard.KeyDown((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), keyName));
                        break;
                }
            }
            else
            {
                switch (keyName)
                {
                    case "WALK":
                        _Interpreter.StopWalking();
                        break;
                    case "DASH":
                        _Interpreter.StopWalking();
                        break;
                    case "PL2CAM":
                        _Pl2Cam = false;
                        break;
                    case "LBUTTON":
                        VR.Input.Mouse.LeftButtonUp();
                        break;
                    case "RBUTTON":
                        VR.Input.Mouse.RightButtonUp();
                        break;
                    case "MBUTTON":
                        VR.Input.Mouse.MiddleButtonUp();
                        break;
                    case "LROTATION":
                        _Interpreter.RotatePlayer(-_Settings.RotationAngle);
                        break;
                    case "RROTATION":
                        _Interpreter.RotatePlayer(_Settings.RotationAngle);
                        break;
                    case "NEXT":
                        ChangeKeySet();
                        break;
                    case "CROUCH":
                        _Interpreter.StandUp();
                        break;
                    default:
                        VR.Input.Keyboard.KeyUp((VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), keyName));
                        break;
                }
            }
        }

        public override List<HelpText> GetHelpTexts()
        {
            return new List<HelpText>();
        }
    }
}
