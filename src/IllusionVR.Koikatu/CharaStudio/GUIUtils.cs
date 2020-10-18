using System;
using UnityEngine;

namespace KKCharaStudioVR
{
    public class GUIUtils
    {
        private static bool isVR = false;

        private static Texture2D windowBG = new Texture2D(1, 1, TextureFormat.ARGB32, false);

        static GUIUtils()
        {
            if((Environment.CommandLine.Contains("--vr") || Environment.CommandLine.Contains("--studiovr")) && !Environment.CommandLine.Contains("--novr"))
            {
                isVR = true;
            }
            windowBG.SetPixel(0, 0, Color.black);
            windowBG.Apply();
        }

        public static GUIStyle GetWindowStyle()
        {
            GUIStyle guistyle = new GUIStyle(GUI.skin.window);
            if(isVR)
            {
                GUI.backgroundColor = Color.black;
                guistyle.onNormal.background = windowBG;
                guistyle.normal.background = windowBG;
                guistyle.hover.background = windowBG;
                guistyle.focused.background = windowBG;
                guistyle.active.background = windowBG;
                guistyle.hover.textColor = Color.blue;
                guistyle.onHover.textColor = Color.blue;
            }
            return guistyle;
        }
    }
}
