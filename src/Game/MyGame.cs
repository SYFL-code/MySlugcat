using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySlugcat
{
    public class MyGame
    {
        public static void Hook()
        {
            //打开外层空间大门
            On.RegionGate.customOEGateRequirements += RegionGate_customOEGateRequirements;
        }

        //解锁归乡门
        private static bool RegionGate_customOEGateRequirements(On.RegionGate.orig_customOEGateRequirements orig, RegionGate self)
        {
            Player firstRealizedPlayer = self.room.game.FirstRealizedPlayer;
            if (firstRealizedPlayer.slugcatStats.name == Plugin.YourSlugID)
                return true;
            else
                return orig.Invoke(self);
        }
    }
}
