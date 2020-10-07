using System;
using System.Linq;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Helpers;

namespace VRGIN.Controls
{
    public class KeyboardShortcut : IShortcut
    {
        public KeyStroke KeyStroke { get; private set; }
        public Action Action { get; private set; }
        public KeyMode CheckMode { get; private set; }

        public KeyboardShortcut(KeyStroke keyStroke, Action action, KeyMode checkMode = KeyMode.PressUp)
        {
            KeyStroke = keyStroke;
            Action = action;
            CheckMode = checkMode;
        }

        public KeyboardShortcut(XmlKeyStroke keyStroke, Action action)
        {
            KeyStroke = keyStroke.GetKeyStrokes().FirstOrDefault();
            Action = action;
            CheckMode = keyStroke.CheckMode;
        }

        public void Evaluate()
        {
            if(KeyStroke.Check(CheckMode))
            {
                Action();
            }
        }

        public void Dispose()
        {
        }
    }

    public class MultiKeyboardShortcut : IShortcut
    {
        private const float WAIT_TIME = 0.5f;

        public KeyStroke[] KeyStrokes { get; private set; }
        public Action Action { get; private set; }
        public KeyMode CheckMode { get; private set; }

        private int _Index = 0;
        private float _Time = 0f;

        public MultiKeyboardShortcut(KeyStroke[] keyStrokes, Action action, KeyMode checkMode = KeyMode.PressUp)
        {
            KeyStrokes = keyStrokes;
            Action = action;
            CheckMode = checkMode;
        }
        public MultiKeyboardShortcut(KeyStroke keyStroke1, KeyStroke keyStroke2, Action action, KeyMode checkMode = KeyMode.PressUp)
        {
            KeyStrokes = new KeyStroke[] { keyStroke1, keyStroke2 };
            Action = action;
            CheckMode = checkMode;
        }

        public MultiKeyboardShortcut(XmlKeyStroke stroke, Action action)
        {
            KeyStrokes = stroke.GetKeyStrokes();
            Action = action;
            CheckMode = stroke.CheckMode;
        }

        public void Evaluate()
        {
            if(Time.time - _Time > WAIT_TIME)
            {
                _Index = 0;
            }

            bool isLast = _Index == KeyStrokes.Length - 1;
            var mode = isLast
                ? CheckMode
                : KeyMode.PressUp;

            if(KeyStrokes[_Index].Check(mode))
            {
                if(isLast)
                {
                    Action();
                }
                else
                {
                    _Index++;
                    _Time = Time.unscaledTime;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
