using IllusionVR.Core;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using VRGIN.Controls.Speech;
using VRGIN.Core;
using VRGIN.Visuals;

namespace IllusionVR.Koikatu.CharaStudio
{
    [XmlRoot("Context")]
    public class StudioContext : IVRManagerContext
    {
        private static string configPath = Path.Combine(BepInEx.Paths.ConfigPath, "IllusionVR");

        public StudioContext()
        {
            _Materials = new DefaultMaterialPalette();
            Settings = KKCharaStudioVRSettings.Load(Path.Combine(configPath, "KKCSVRSettings.xml"));

            ConfineMouse = true;
            EnforceDefaultGUIMaterials = false;
            GUIAlternativeSortingMode = false;
            GuiLayer = "Default";
            GuiFarClipPlane = 1000f;
            GuiNearClipPlane = -1000f;
            IgnoreMask = 0;
            InvisibleLayer = "Ignore Raycast";
            PrimaryColor = Color.cyan;
            SimulateCursor = true;
            UILayer = "UI";
            UILayerMask = LayerMask.GetMask(UILayer);
            UnitToMeter = 1f;
            NearClipPlane = 0.001f;
            PreferredGUI = GUIType.IMGUI;
            CameraClearFlags = CameraClearFlags.Skybox;
        }

        public static StudioContext CreateContext(string contextName)
        {
            var serializer = new XmlSerializer(typeof(StudioContext));
            var path = Path.Combine(configPath, contextName);

            if(File.Exists(path))
            {
                // Attempt to load XML
                using(var file = File.OpenRead(path))
                {
                    try
                    {
                        return serializer.Deserialize(file) as StudioContext;
                    }
                    catch(Exception ex)
                    {
                        IVRLog.LogError($"Failed to deserialize {path} -- using default\n{ex}");
                    }
                }
            }

            // Create and save file
            var context = new StudioContext();
            try
            {
                using(var file = new StreamWriter(path))
                {
                    file.BaseStream.SetLength(0);
                    serializer.Serialize(file, context);
                }
            }
            catch(Exception ex)
            {
                IVRLog.LogError($"Failed to write {path}\n{ex}");
            }

            return context;
        }

        [XmlIgnore]
        public IMaterialPalette Materials => _Materials;
        private DefaultMaterialPalette _Materials;

        [XmlIgnore]
        public VRSettings Settings { get; }

        [XmlIgnore]
        public Type VoiceCommandType => typeof(VoiceCommand);

        public bool ConfineMouse { get; set; }

        public bool EnforceDefaultGUIMaterials { get; set; }

        public bool GUIAlternativeSortingMode { get; set; }

        public float GuiFarClipPlane { get; set; }

        public string GuiLayer { get; set; }

        public float GuiNearClipPlane { get; set; }

        public int IgnoreMask { get; set; }

        public string InvisibleLayer { get; set; }

        public Color PrimaryColor { get; set; }

        public bool SimulateCursor { get; set; }

        public string UILayer { get; set; }

        public int UILayerMask { get; set; }

        public float UnitToMeter { get; set; }

        public float NearClipPlane { get; set; }

        public GUIType PreferredGUI { get; set; }

        public CameraClearFlags CameraClearFlags { get; set; }
    }
}
