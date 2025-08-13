/*using RWCustom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MySlugcat
{
    public class MyDebug
    {
        FContainer container;
        private FLabel Label_Text;
        Player player;
        public static string outStr = "null";

        public MyDebug(Player player)
        {
            Label_Text = new FLabel(Custom.GetDisplayFont(), "");
            container = new FContainer();
            this.player = player;
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser leaser, RoomCamera rCam)
        {
            //取玩家变量
            container.RemoveAllChildren();
            container.AddChild(Label_Text);
            var hud = rCam.ReturnFContainer("HUD");
            hud.AddChild(container);
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            //取玩家变量
            GlobalVar.playerVar.TryGetValue(player, out PlayerVar pv);

            var pos = Vector2.Lerp(player.bodyChunks[1].lastPos, player.bodyChunks[1].pos, timeStacker);
            pos.x -= rCam.pos.x;
            pos.y -= rCam.pos.y;
            pos.y += 80;
            Label_Text.SetPosition(pos);
            Label_Text.text = outStr;//string.Format("PressedIceShield={0}", GlobalVar.IsPressedIceShield(player));
        }
    }
}*/
