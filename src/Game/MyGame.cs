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
        /*public static void Hook()
        {
            //打开外层空间大门
            On.RegionGate.customOEGateRequirements += RegionGate_customOEGateRequirements;
        }*/

        //解锁归乡门
        public static bool RegionGate_customOEGateRequirements(ref bool Execute, ref bool return_, ref On.RegionGate.orig_customOEGateRequirements orig, ref RegionGate regionGate)
        {
            if (!ModManager.MSC)
            {
                return false;
            }

            if (regionGate.room.game.StoryCharacter == Plugin.YourSlugID)
            {
                return true;
            }
            else
            {
                Execute = false;
				return orig.Invoke(regionGate);
            }
        }
    }
}
