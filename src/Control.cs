using On;
using IL;
using System;
using System.Threading.Tasks;
using Mono.Cecil;
using MoreSlugcats;
using RWCustom;
using Smoke;
using static PhysicalObject;
using UnityEngine;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections;
using SlugBase.Features;
using System.Diagnostics;
using ImprovedInput;
using System.Linq;
using RewiredConsts;
using Menu.Remix;
using MonoMod.RuntimeDetour;
using Watcher;
using static MonoMod.InlineRT.MonoModRule;
using System.Reflection;
using System.Threading;


namespace MySlugcat
{
    // FSpriteControl 精灵控制中心

    public class Control
    {
        public static RainWorldGame? RWG;
        public static Vector2 camPos = new Vector2(300f, 300f);


        public static PointerFSprite[] pointer = new PointerFSprite[20];

        public static void Hook()
        {
#if MYDEBUG
            try
            {
#endif

            On.Player.ctor += Player_ctor;
            On.Player.Update += Player_Update;
            On.RoomCamera.SpriteLeaser.Update += SLeaser_Update;
            On.RainWorldGame.Update += RainWorldGame_Update;

#if MYDEBUG
            }
            catch (Exception e)
            {
                StackTrace st = new StackTrace(new StackFrame(true));
                StackFrame sf = st.GetFrame(0);
                var sr = sf.GetFileName().Split('\\');
                MyDebug.outStr = sr[sr.Length - 1] + "\n";
                MyDebug.outStr += sf.GetMethod() + "\n";
                MyDebug.outStr += e;
                UnityEngine.Debug.Log(e);
            }
#endif
        }

        private static readonly object _singletonLock = new object();
        private static bool _isMainUpdateRunning = false;
        private static bool Running = false;

        public static void MainUpdate()
        {
            lock (_singletonLock)
            {
                // 如果已经在运行，直接返回
                if (_isMainUpdateRunning)
                {
                    Console.WriteLine("MainUpdate 已经在运行！");
                    return;
                }

                _isMainUpdateRunning = true; // 标记为已运行
            }

            try
            {
                Console.WriteLine("MainUpdate 已经在运行！1");
                // 真正的游戏循环
                //var stopwatch = Stopwatch.StartNew();
                //double targetFrameTime = 1000.0 / 40.0; // 40 FPS（每帧 = 25ms）
                //double previousTime = 0;

                while (_isMainUpdateRunning)// 用标志位控制退出
                {
                    //Log.Logger(7, "PerceptionSkill", "MySlugcat:CreaturePointer:MainUpdate_st1",
                    //    $"({1})");

                    for (int i = 0; i < 20; i++)
                    {
                        if (pointer != null && pointer[i] != null && !pointer[i].slatedForDestroy)
                        {
                            //Log.Logger(7, "PerceptionSkill", "MySlugcat:CreaturePointer:MainUpdate_zh2",
                            //    $"Null ({pointer[i].owner == null})");
                            if (pointer[i].owner == null)
                            {
                                pointer[i].Destroy();
                                pointer[i].slatedForDestroy = true;
                            }
                            else
                            {
                                //pointer[i].Update_(camPos);
                                pointer[i].Update(i, false);
                            }
                        }
                    }

                    //double currentTime = stopwatch.Elapsed.TotalMilliseconds;
                    //double deltaTime = currentTime - previousTime;

                    //if (deltaTime >= targetFrameTime)
                    //{

                    //Update(deltaTime / 1000.0); // 传入 deltaTime（秒）
                    //Render();
                    //ProcessInput();

                    //previousTime = currentTime;
                    //}
                    //else
                    //{
                    //    // 如果还没到下一帧，让出 CPU 时间
                    //    Thread.Sleep(0);
                    //}
                }
            }
            finally
            {
                // 确保退出时释放标志位
                _isMainUpdateRunning = false;
            }

        }

        private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig.Invoke(self, abstractCreature, world);

            if (_isMainUpdateRunning == false)
            {
                Task.Run(() => MainUpdate()); // 异步启动（避免阻塞）
            }

            if (self.slugcatStats.name == Plugin.YourSlugID && SC.PerceptionSkill)
            {
                int N = self.playerState.playerNumber;
                if (N == 0)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        if (pointer[i] != null && !pointer[i].slatedForDestroy)
                        {
                            pointer[i].Destroy();
                            pointer[i].slatedForDestroy = true;
                        }
                    }
                }
                pointer[N] = new PointerFSprite(self);
                pointer[N].slatedForDestroy = false;
            }
        }

        public static void RainWorldGame_Update(On.RainWorldGame.orig_Update orig, RainWorldGame rainWorldGame)
        {
            orig(rainWorldGame);

            RWG = rainWorldGame;
        }

        public static void SLeaser_Update(On.RoomCamera.SpriteLeaser.orig_Update orig, RoomCamera.SpriteLeaser self, float timeStacker, RoomCamera rCam, Vector2 cameraPos)
        {
            orig.Invoke(self, timeStacker, rCam, cameraPos);

            camPos = cameraPos;

            if (_isMainUpdateRunning == false && Running == false)
            {
                Running = true;
                Task.Run(() => MainUpdate()); // 异步启动（避免阻塞）
            }

            //Log.Logger(7, "PerceptionSkill", "MySlugcat:CreaturePointer:SLeaser_Update_1",
            //    $"Null({update0 == null})");//



            /*            for (int i = 0; i < 20; i++)
                        {
                            if (pointer[i] != null && !pointer[i].slatedForDestroy)
                            {
                                if (pointer[i].owner == null)
                                {
                                    pointer[i].Destroy();
                                    pointer[i].slatedForDestroy = true;
                                }
                                else
                                {
                                    if (!pointer[i].slatedForDestroy)
                                    {
                                        pointer[i].Update_(camPos);
                                        //pointer[i].Update(i, false);
                                    }

                                }
                            }
                        }*/

        }

        private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);

            /*        if (self.slugcatStats.name == Plugin.YourSlugID && 1 == UnityEngine.Random.Range(0, 3000000) && update0 != null)
                    {
                        update0.N = 0;
                        update0.i = UnityEngine.Random.Range(-46666, 30000);
                    }*/

            int N = self.playerState.playerNumber;
            if (pointer != null && pointer[N] != null && !pointer[N].slatedForDestroy && self.slugcatStats.name == Plugin.YourSlugID)
            {
                pointer[N].start = true;
                pointer[N].i = pointer[N].lasti;
                pointer[N].Update(N, false);
            }
        }


    }
}
