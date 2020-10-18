using System;
using System.Collections.Generic;
using System.Linq;
using VRGIN.Controls;
using VRGIN.Controls.Tools;
using VRGIN.Core;
using VRGIN.Helpers;
using VRGIN.Modes;

namespace IllusionVR.Koikatu.MainGame
{
    internal class GenericStandingMode : StandingMode
    {
        protected override IEnumerable<IShortcut> CreateShortcuts()
        {
            return base.CreateShortcuts().Concat(new IShortcut[] {
                new MultiKeyboardShortcut(new KeyStroke("Ctrl+C"), new KeyStroke("Ctrl+C"), () => { VR.Manager.SetMode<GenericSeatedMode>(); })
            });
        }

        public override IEnumerable<Type> Tools => base.Tools.Concat(new Type[] { typeof(MenuTool), typeof(WarpTool), typeof(SchoolTool) });
    }
}
