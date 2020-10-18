using System;
using System.Xml.Serialization;
using VRGIN.Core;

namespace KKCharaStudioVR
{
	[XmlRoot("Settings")]
	public class KKCharaStudioVRSettings : VRSettings
	{
		private bool _LockRotXZ = true;

		public static KKCharaStudioVRSettings Load(string path)
		{
			return VRSettings.Load<KKCharaStudioVRSettings>(path);
		}

		[XmlComment("Lock XZ Axis (pitch / roll) rotation.")]
		public bool LockRotXZ
		{
			get
			{
				return _LockRotXZ;
			}
			set
			{
				_LockRotXZ = value;
				base.TriggerPropertyChanged("LockRotXZ");
			}
		}
	}
}
