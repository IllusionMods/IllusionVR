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
                if(_guiSkin == null)
                {
                    _guiSkin = CreateVRGUISkin(GUI.skin);
                }
                return _guiSkin;
            }
            set
            {
                if(value != null)
                {
                    _guiSkin = value;
                }
            }
        }

        public static GUISkin CreateVRGUISkin(GUISkin cloneFrom)
        {
            GUISkin guiskin = Object.Instantiate<GUISkin>(cloneFrom);
            GUIStyle guistyle = new GUIStyle(guiskin.window);
            Texture2D texture2D = new Texture2D(1, 1);
            texture2D.SetPixel(0, 0, new Color(0.9f, 0.9f, 0.9f, 1f));
            texture2D.Apply();
            guistyle.normal.textColor = windowTextColor;
            guistyle.normal.background = texture2D;
            guistyle.onNormal.textColor = windowTextColor;
            guistyle.onNormal.background = texture2D;
            guiskin.window = guistyle;
            guiskin.button.normal.textColor = buttonTextColor;
            guiskin.button.onNormal.textColor = buttonTextColor;
            guiskin.button.normal.background = CreateColorInvertedTexture(guiskin.button.normal.background, false);
            guiskin.button.onNormal.background = CreateColorInvertedTexture(guiskin.button.onNormal.background, false);
            guiskin.label.normal.textColor = windowTextColor;
            guiskin.label.onNormal.textColor = windowTextColor;
            guiskin.toggle.normal.textColor = windowTextColor;
            guiskin.toggle.onNormal.textColor = windowTextColor;
            guiskin.toggle.normal.background = CreateColorInvertedTexture(guiskin.toggle.normal.background, false);
            guiskin.toggle.onNormal.background = CreateColorInvertedTexture(guiskin.toggle.normal.background, false);
            guiskin.settings.selectionColor = windowTextColor;
            guiskin.textField.normal.textColor = buttonTextColor;
            guiskin.textField.onNormal.textColor = buttonTextColor;
            guiskin.textField.focused.textColor = buttonTextColor;
            guiskin.textField.onFocused.textColor = buttonTextColor;
            guiskin.textField.normal.background = CreateColorInvertedTexture(guiskin.textField.normal.background, false);
            guiskin.textField.onNormal.background = CreateColorInvertedTexture(guiskin.textField.onNormal.background, false);
            guiskin.textField.focused.background = CreateColorInvertedTexture(guiskin.textField.focused.background, false);
            guiskin.textField.onFocused.background = CreateColorInvertedTexture(guiskin.textField.onFocused.background, false);
            guiskin.textArea.normal.textColor = buttonTextColor;
            guiskin.textArea.onNormal.textColor = buttonTextColor;
            guiskin.textArea.focused.textColor = buttonTextColor;
            guiskin.textArea.onFocused.textColor = buttonTextColor;
            guiskin.textArea.normal.background = CreateColorInvertedTexture(guiskin.textArea.normal.background, false);
            guiskin.textArea.onNormal.background = CreateColorInvertedTexture(guiskin.textArea.onNormal.background, false);
            guiskin.textArea.focused.background = CreateColorInvertedTexture(guiskin.textArea.focused.background, false);
            guiskin.textArea.onFocused.background = CreateColorInvertedTexture(guiskin.textArea.onFocused.background, false);
            guiskin.box.normal.background = texture2D;
            guiskin.box.onNormal.background = texture2D;
            guiskin.box.normal.textColor = windowTextColor;
            guiskin.box.onNormal.textColor = windowTextColor;
            guiskin.horizontalSlider.normal.background = CreateColorInvertedTexture(guiskin.horizontalSlider.normal.background, false);
            guiskin.horizontalSlider.onNormal.background = CreateColorInvertedTexture(guiskin.horizontalSlider.onNormal.background, false);
            guiskin.verticalSlider.normal.background = CreateColorInvertedTexture(guiskin.verticalSlider.normal.background, false);
            guiskin.verticalSlider.onNormal.background = CreateColorInvertedTexture(guiskin.verticalSlider.onNormal.background, false);
            guiskin.horizontalSliderThumb.normal.background = CreateColorInvertedTexture(guiskin.horizontalSliderThumb.normal.background, false);
            guiskin.horizontalSliderThumb.onNormal.background = CreateColorInvertedTexture(guiskin.horizontalSliderThumb.onNormal.background, false);
            guiskin.verticalSliderThumb.normal.background = CreateColorInvertedTexture(guiskin.verticalSliderThumb.normal.background, false);
            guiskin.verticalSliderThumb.onNormal.background = CreateColorInvertedTexture(guiskin.verticalSliderThumb.onNormal.background, false);
            return guiskin;
        }

        public static Texture2D CreateColorInvertedTexture(Texture tex, bool isLiner = false)
        {
            float num = 0.5f;
            float num2 = 0.5f;
            if(tex == null)
            {
                return null;
            }
            RenderTextureReadWrite renderTextureReadWrite = RenderTextureReadWrite.Linear;
            if(!isLiner)
            {
                renderTextureReadWrite = RenderTextureReadWrite.sRGB;
            }
            RenderTexture renderTexture = new RenderTexture(tex.width, tex.height, 0, 0, renderTextureReadWrite);
            bool sRGBWrite = GL.sRGBWrite;
            GL.sRGBWrite = !isLiner;
            Graphics.SetRenderTarget(renderTexture);
            GL.Clear(false, true, new Color(0f, 0f, 0f, 0f));
            Graphics.SetRenderTarget(null);
            Graphics.Blit(tex, renderTexture);
            GL.sRGBWrite = sRGBWrite;
            Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, isLiner);
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = null;
            renderTexture.Release();
            Color[] pixels = texture2D.GetPixels();
            for(int i = 0; i < pixels.Length; i++)
            {
                pixels[i].r = Mathf.Clamp01(pixels[i].r + num);
                pixels[i].g = Mathf.Clamp01(pixels[i].g + num);
                pixels[i].b = Mathf.Clamp01(pixels[i].b + num);
                if(pixels[i].a != 0f)
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
