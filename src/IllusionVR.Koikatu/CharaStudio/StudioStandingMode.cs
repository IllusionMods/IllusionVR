using System;
using System.Collections.Generic;
using System.Linq;
using VRGIN.Controls.Tools;
using VRGIN.Modes;

namespace IllusionVR.Koikatu
{
    internal class StudioStandingMode : StandingMode
    {
        public override IEnumerable<Type> Tools => base.Tools.Concat(new Type[]{ typeof(MenuTool), typeof(WarpTool)});
    }
}
