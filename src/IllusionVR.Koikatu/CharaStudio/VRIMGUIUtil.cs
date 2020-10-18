using System;
using UnityEngine;

namespace VRUtil
{
	public class VRIMGUIUtil
	{
		private static GUISkin _guiSkin = null;

		private static Color windowTextColor = new Color(0.1f, 0.5f, 0.1f, 1f);

		private static Color buttonTextColor = new Color(0.3f, 0.9f, 0.3f, 1f);

		public static GUISkin VRGUISkin
		{
			get
			{
				if (VRIMGUIUtil._guiSkin == null)
				{
					VRIMGUIUtil._guiSkin = VRIMGUIUtil.CreateVRGUISkin(GUI.skin);
				}
				return VRIMGUIUtil._guiSkin;
			}
			set
			{
				if (value != null)
				{
					VRIMGUIUtil._guiSkin = value;
				}
			}
		}

		public static GUISkin CreateVRGUISkin(GUISkin cloneFrom)
		{
			GUISkin guiskin = UnityEngine.Object.Instantiate<GUISkin>(cloneFrom);
			GUIStyle guistyle = new GUIStyle(guiskin.window);
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.SetPixel(0, 0, new Color(0.9f, 0.9f, 0.9f, 1f));
			texture2D.Apply();
			guistyle.normal.textColor = VRIMGUIUtil.windowTextColor;
			guistyle.normal.background = texture2D;
			guistyle.onNormal.textColor = VRIMGUIUtil.windowTextColor;
			guistyle.onNormal.background = texture2D;
			guiskin.window = guistyle;
			guiskin.button.normal.textColor = VRIMGUIUtil.buttonTextColor;
			guiskin.button.onNormal.textColor = VRIMGUIUtil.buttonTextColor;
			guiskin.button.normal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.button.normal.background, false);
			guiskin.button.onNormal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.button.onNormal.background, false);
			guiskin.label.normal.textColor = VRIMGUIUtil.windowTextColor;
			guiskin.label.onNormal.textColor = VRIMGUIUtil.windowTextColor;
			guiskin.toggle.normal.textColor = VRIMGUIUtil.windowTextColor;
			guiskin.toggle.onNormal.textColor = VRIMGUIUtil.windowTextColor;
			guiskin.toggle.normal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.toggle.normal.background, false);
			guiskin.toggle.onNormal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.toggle.normal.background, false);
			guiskin.settings.selectionColor = VRIMGUIUtil.windowTextColor;
			guiskin.textField.normal.textColor = VRIMGUIUtil.buttonTextColor;
			guiskin.textField.onNormal.textColor = VRIMGUIUtil.buttonTextColor;
			guiskin.textField.focused.textColor = VRIMGUIUtil.buttonTextColor;
			guiskin.textField.onFocused.textColor = VRIMGUIUtil.buttonTextColor;
			guiskin.textField.normal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.textField.normal.background, false);
			guiskin.textField.onNormal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.textField.onNormal.background, false);
			guiskin.textField.focused.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.textField.focused.background, false);
			guiskin.textField.onFocused.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.textField.onFocused.background, false);
			guiskin.textArea.normal.textColor = VRIMGUIUtil.buttonTextColor;
			guiskin.textArea.onNormal.textColor = VRIMGUIUtil.buttonTextColor;
			guiskin.textArea.focused.textColor = VRIMGUIUtil.buttonTextColor;
			guiskin.textArea.onFocused.textColor = VRIMGUIUtil.buttonTextColor;
			guiskin.textArea.normal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.textArea.normal.background, false);
			guiskin.textArea.onNormal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.textArea.onNormal.background, false);
			guiskin.textArea.focused.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.textArea.focused.background, false);
			guiskin.textArea.onFocused.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.textArea.onFocused.background, false);
			guiskin.box.normal.background = texture2D;
			guiskin.box.onNormal.background = texture2D;
			guiskin.box.normal.textColor = VRIMGUIUtil.windowTextColor;
			guiskin.box.onNormal.textColor = VRIMGUIUtil.windowTextColor;
			guiskin.horizontalSlider.normal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.horizontalSlider.normal.background, false);
			guiskin.horizontalSlider.onNormal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.horizontalSlider.onNormal.background, false);
			guiskin.verticalSlider.normal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.verticalSlider.normal.background, false);
			guiskin.verticalSlider.onNormal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.verticalSlider.onNormal.background, false);
			guiskin.horizontalSliderThumb.normal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.horizontalSliderThumb.normal.background, false);
			guiskin.horizontalSliderThumb.onNormal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.horizontalSliderThumb.onNormal.background, false);
			guiskin.verticalSliderThumb.normal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.verticalSliderThumb.normal.background, false);
			guiskin.verticalSliderThumb.onNormal.background = VRIMGUIUtil.CreateColorInvertedTexture(guiskin.verticalSliderThumb.onNormal.background, false);
			return guiskin;
		}

		public static Texture2D CreateColorInvertedTexture(Texture tex, bool isLiner = false)
		{
			float num = 0.5f;
			float num2 = 0.5f;
			if (tex == null)
			{
				return null;
			}
			RenderTextureReadWrite renderTextureReadWrite = 1;
			if (!isLiner)
			{
				renderTextureReadWrite = 2;
			}
			RenderTexture renderTexture = new RenderTexture(tex.width, tex.height, 0, 0, renderTextureReadWrite);
			bool sRGBWrite = GL.sRGBWrite;
			GL.sRGBWrite = !isLiner;
			Graphics.SetRenderTarget(renderTexture);
			GL.Clear(false, true, new Color(0f, 0f, 0f, 0f));
			Graphics.SetRenderTarget(null);
			Graphics.Blit(tex, renderTexture);
			GL.sRGBWrite = sRGBWrite;
			Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, 5, isLiner);
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = null;
			renderTexture.Release();
			Color[] pixels = texture2D.GetPixels();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i].r = Mathf.Clamp01(pixels[i].r + num);
				pixels[i].g = Mathf.Clamp01(pixels[i].g + num);
				pixels[i].b = Mathf.Clamp01(pixels[i].b + num);
				if (pixels[i].a != 0f)
				{
					pixels[i].a = Mathf.Clamp01(pixels[i].a + num2);
				}
			}
			texture2D.SetPixels(pixels);
			texture2D.Apply();
			texture2D.wrapMode = tex.wrapMode;
			return texture2D;
		}
	}
}
