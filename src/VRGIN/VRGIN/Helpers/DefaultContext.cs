﻿using System;
using UnityEngine;
using VRGIN.Controls.Speech;
using VRGIN.Core;
using VRGIN.Visuals;

namespace VRGIN.Helpers
{
    /// <summary>
    /// Default IVRManagerContext with sensible defaults that you can extend.
    /// </summary>
    public class DefaultContext : IVRManagerContext
    {
        private IMaterialPalette _Materials;
        private VRSettings _Settings;

        public DefaultContext()
        {
            _Materials = CreateMaterialPalette();
            _Settings = CreateSettings();
        }

        protected virtual IMaterialPalette CreateMaterialPalette()
        {
            return new DefaultMaterialPalette();
        }

        protected virtual VRSettings CreateSettings()
        {
            return VRSettings.Load<VRSettings>("VRSettings.xml");
        }

        public virtual bool ConfineMouse => true;

        public virtual bool EnforceDefaultGUIMaterials => false;

        public virtual bool GUIAlternativeSortingMode => false;

        public virtual float GuiFarClipPlane => 10000;

        public virtual string GuiLayer => "Default";

        public virtual float GuiNearClipPlane => -10000;

        public virtual int IgnoreMask => 0;

        public virtual string InvisibleLayer => "Ignore Raycast";

        public IMaterialPalette Materials => _Materials;

        public virtual float NearClipPlane => 0.1f;

        public virtual GUIType PreferredGUI => GUIType.uGUI;

        public virtual Color PrimaryColor => Color.cyan;

        public virtual VRSettings Settings => _Settings;

        public virtual bool SimulateCursor => true;

        public virtual string UILayer => "UI";

        public virtual int UILayerMask => LayerMask.GetMask(UILayer);

        public virtual float UnitToMeter => 1;

        public virtual Type VoiceCommandType => typeof(VoiceCommand);

        public CameraClearFlags CameraClearFlags => CameraClearFlags.Skybox;
    }
}
