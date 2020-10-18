using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using VRGIN.Core;

namespace IllusionVR.Koikatu.MainGame
{
    /// <summary>
    /// Class that holds settings for VR. Saved as an XML file.
    /// 
    /// In order to create your own settings file, extend this class and add your own properties. Make sure to call <see cref="TriggerPropertyChanged(string)"/> if you want to use
    /// the events.
    /// IMPORTANT: When extending, add an XmlRoot annotation to the class like so:
    /// <code>[XmlRoot("Settings")]</code>
    /// </summary>
    [XmlRoot("Settings")]
    public class KoikatuSettings : VRSettings
    {
        // XMLSerializerは配列にデフォルト値をつけると、指定値とデフォルト値の両方を含む配列にしてしまうので
        public static KoikatuSettings Load(string path)
        {
            KoikatuSettings settings = Load<KoikatuSettings>(path);
            if(settings.KeySets.Count == 0)
            {
                settings.KeySets = new List<KeySet> { new KeySet() };
            }

            return settings;
        }

        [XmlElement(Type = typeof(List<KeySet>))]
        public List<KeySet> KeySets { get; set; } = null;

        public bool UsingHeadPos { get; set; } = false;

        public float StandingCameraPos { get; set; } = 1.5f;

        public float CrouchingCameraPos { get; set; } = 0.7f;

        public bool CrouchByHMDPos { get; set; } = true;

        public float CrouchThrethould { get; set; } = 0.15f;

        public float StandUpThrethould { get; set; } = -0.55f;

        public float TouchpadThreshold { get; set; } = 0.8f;

        public float RotationAngle { get; set; } = 45f;
    }

    [XmlRoot("KeySet")]
    public class KeySet
    {
        public KeySet()
        {
            Trigger = "WALK";
            Grip = "PL2CAM";
            Up = "F3";
            Down = "F4";
            Right = "RROTATION";
            Left = "LROTATION";
            Center = "RBUTTON";
        }

        public KeySet(string trigger, string grip, string Up, string Down, string Right, string Left, string Center)
        {
            Trigger = trigger;
            Grip = grip;
            this.Up = Up;
            this.Down = Down;
            this.Right = Right;
            this.Left = Left;
            this.Center = Center;
        }

        [XmlElement("Trigger")]
        public String Trigger { get; set; }

        [XmlElement("Grip")]
        public String Grip { get; set; }

        [XmlElement("Up")]
        public String Up { get; set; }

        [XmlElement("Down")]
        public String Down { get; set; }

        [XmlElement("Right")]
        public String Right { get; set; }

        [XmlElement("Left")]
        public String Left { get; set; }

        [XmlElement("Center")]
        public String Center { get; set; }
    }
}
