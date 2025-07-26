using UnityEngine;
using RWCustom;
using HUD;
using MoreSlugcats;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MySlugcat
{
    // PerceptionSkill 感知能力
    public class VisionSystem : HudPart
    {


        public static void Hook()
        {
            //Log.Logger(7, "IntelHUD", "MySlugcat:IntelHUD​​:Hook", $"sst");
            On.HUD.HUD.InitSleepHud += HUD_InitSleepHud;
            On.HUD.HUD.InitSinglePlayerHud += HUD_InitSinglePlayerHud;
        }

        private static void HUD_InitSleepHud(On.HUD.HUD.orig_InitSleepHud orig, HUD.HUD self, Menu.SleepAndDeathScreen sleepAndDeathScreen, HUD.Map.MapData mapData, SlugcatStats charStats)
        {
            ///Log.Logger(7, "IntelHUD", "MySlugcat:IntelHUD​​:HUD_InitSleepHud", $"st");
            orig.Invoke(self, sleepAndDeathScreen, mapData, charStats);
            self.AddPart(new VisionSystem(self));
        }

        private static void HUD_InitSinglePlayerHud(On.HUD.HUD.orig_InitSinglePlayerHud orig, HUD.HUD self, RoomCamera cam)
        {
            //Log.Logger(7, "IntelHUD", "MySlugcat:IntelHUD​​:HUD_InitSinglePlayerHud", $"st");
            orig.Invoke(self, cam);
            self.AddPart(new VisionSystem(self));
        }

        public VisionSystem(HUD.HUD hud) : base(hud)
        {

            // 1. 创建一个 FContainer 作为父容器
            FContainer container = new FContainer();
            hud.fContainers[1].AddChild(container); // 添加到HUD容器

            // 2. 添加一个长方形（背景）
            FSprite rect = new FSprite("pixel"); // 假设 "square" 是白色矩形
            rect.width = 400; // 设置宽度
            rect.height = 200; // 设置高度
            rect.color = Color.blue; // 设置颜色
            container.AddChild(rect);

            // 3. 添加一个圆形（作为“透明”部分）
            FSprite circle = new FSprite("circle"); // 假设 "circle" 是白色圆形
            circle.width = 100; // 设置直径
            circle.height = 100;
            circle.x = 0; // 居中
            circle.y = 0;
            circle.color = new Color(0, 0, 0, 0); // 完全透明（如果引擎支持）

            // 如果引擎不支持直接透明，可以使用混合模式（如 Futile 的 `BlendMode.Subtractive`）
            // circle.blendMode = BlendMode.Subtractive; // 可能需要调整

            container.AddChild(circle);

        }



    }
}
