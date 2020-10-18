using System;
using System.Collections.Generic;
using System.Linq;
using VRGIN.Controls;
using VRGIN.Controls.Tools;
using VRGIN.Core;
using VRGIN.Helpers;
using VRGIN.Modes;

namespace KKCharaStudioVR
{
	internal class GenericStandingMode : StandingMode
	{
		protected override IEnumerable<IShortcut> CreateShortcuts()
		{
			IEnumerable<IShortcut> first = base.CreateShortcuts();
			IShortcut[] array = new IShortcut[1];
			array[0] = new MultiKeyboardShortcut(new KeyStroke("Ctrl+C"), new KeyStroke("Ctrl+C"), delegate()
			{
				VR.Manager.SetMode<GenericSeatedMode>();
			}, KeyMode.PressUp);
			return first.Concat(array);
		}

		public override IEnumerable<Type> Tools
		{
			get
			{
				return base.Tools.Concat(new Type[]
				{
					typeof(MenuTool),
					typeof(WarpTool),
					typeof(GripMoveKKCharaStudioTool)
				});
			}
		}
	}
}
