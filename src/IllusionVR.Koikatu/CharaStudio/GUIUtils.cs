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
			if ((Environment.CommandLine.Contains("--vr") || Environment.CommandLine.Contains("--studiovr")) && !Environment.CommandLine.Contains("--novr"))
			{
				GUIUtils.isVR = true;
			}
			GUIUtils.windowBG.SetPixel(0, 0, Color.black);
			GUIUtils.windowBG.Apply();
		}

		public static GUIStyle GetWindowStyle()
		{
			GUIStyle guistyle = new GUIStyle(GUI.skin.window);
			if (GUIUtils.isVR)
			{
				GUI.backgroundColor = Color.black;
				guistyle.onNormal.background = GUIUtils.windowBG;
				guistyle.normal.background = GUIUtils.windowBG;
				guistyle.hover.background = GUIUtils.windowBG;
				guistyle.focused.background = GUIUtils.windowBG;
				guistyle.active.background = GUIUtils.windowBG;
				guistyle.hover.textColor = Color.blue;
				guistyle.onHover.textColor = Color.blue;
			}
			return guistyle;
		}
	}
}
