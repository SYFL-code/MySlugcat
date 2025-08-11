using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SlughostMod;

public class MyModdedEnums // 自定义枚举
{

    public class CreatureTemplateType // 生物模板类型
    {
        public static CreatureTemplate.Type CreatureGhost;

        public static void RegisterValues()
        {
            string entryName = "Creaturehost";
            int conflictIndex = 1;
            while (CreatureTemplate.Type.values.entries.Contains(entryName))
            {
                conflictIndex += 1;
                entryName = "Creaturehost" + conflictIndex.ToString();
            }
            CreatureGhost = new CreatureTemplate.Type(entryName, true);

        }

        public static void UnregisterValues()
        {
            if (CreatureGhost != null)
            {
                CreatureGhost.Unregister();
                CreatureGhost = null;
            }
        }

        private void PlayerGraphicsOnInitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig(self, sLeaser, rCam);
            if (self.player is PlayerGhost && !rCam.room.game.DEBUGMODE)
            {
                for (int ghostSprite = 0; ghostSprite < 9; ghostSprite++)
                {
                    sLeaser.sprites[ghostSprite].shader = rCam.game.rainWorld.Shaders["Hologram"];
                    sLeaser.sprites[ghostSprite].alpha = 0.95f;
                }

            }
        }

    }
}
