using System;
using System.Collections.Generic;
using UnityEngine;

namespace KKCharaStudioVR
{
	public class KKCharaStudioVRGUI : MonoBehaviour
	{
		private int windowID = 8731;

		private Rect windowRect = new Rect((float)(Screen.width - 150), (float)(Screen.height - 100), 150f, 100f);

		private string windowTitle = "KKCharaStudioVR";

		private Texture2D windowBG = new Texture2D(1, 1, TextureFormat.ARGB32, false);

		private Dictionary<string, GUIStyle> styleBackup = new Dictionary<string, GUIStyle>();

		private void OnGUI()
		{
		}

		private void FuncWindowGUI(int winID)
		{
			styleBackup = new Dictionary<string, GUIStyle>();
			BackupGUIStyle("Button");
			BackupGUIStyle("Label");
			BackupGUIStyle("Toggle");
			try
			{
				if (Event.current.type == null)
				{
					GUI.FocusControl("");
					GUI.FocusWindow(winID);
				}
				GUI.enabled = true;
				GUIStyle style = GUI.skin.GetStyle("Button");
				style.normal.textColor = Color.white;
				style.alignment = TextAnchor.MiddleCenter;
				GUIStyle style2 = GUI.skin.GetStyle("Label");
				style2.normal.textColor = Color.white;
				style2.alignment = TextAnchor.MiddleLeft;
				style2.wordWrap = false;
				GUIStyle style3 = GUI.skin.GetStyle("Toggle");
				style3.normal.textColor = Color.white;
				style3.onNormal.textColor = Color.white;
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.EndVertical();
				GUI.DragWindow();
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			finally
			{
				RestoreGUIStyle("Button");
				RestoreGUIStyle("Label");
				RestoreGUIStyle("Toggle");
			}
		}

		private void BackupGUIStyle(string name)
		{
			GUIStyle value = new GUIStyle(GUI.skin.GetStyle(name));
			styleBackup.Add(name, value);
		}

		private void RestoreGUIStyle(string name)
		{
			if (styleBackup.ContainsKey(name))
			{
				GUIStyle guistyle = styleBackup[name];
				GUIStyle style = GUI.skin.GetStyle(name);
				style.normal.textColor = guistyle.normal.textColor;
				style.alignment = guistyle.alignment;
				style.wordWrap = guistyle.wordWrap;
			}
		}
	}
}
