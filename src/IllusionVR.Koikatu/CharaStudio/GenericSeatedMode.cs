using System;
using System.Collections.Generic;
using System.Linq;
using VRGIN.Controls;
using VRGIN.Core;
using VRGIN.Helpers;
using VRGIN.Modes;

namespace KKCharaStudioVR
{
	internal class GenericSeatedMode : SeatedMode
	{
		protected override IEnumerable<IShortcut> CreateShortcuts()
		{
			IEnumerable<IShortcut> first = base.CreateShortcuts();
			IShortcut[] array = new IShortcut[1];
			array[0] = new MultiKeyboardShortcut(new KeyStroke("Ctrl+C"), new KeyStroke("Ctrl+C"), delegate()
			{
				VR.Manager.SetMode<GenericStandingMode>();
			}, KeyMode.PressUp);
			return first.Concat(array);
		}

		protected override void CreateControllers()
		{
		}
	}
}
