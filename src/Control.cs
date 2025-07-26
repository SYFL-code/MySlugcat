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
//using static MySlugcat.VisionSystem;


namespace MySlugcat
{
    // FSpriteControl 精灵控制中心

    public class Control
    {
        public static Player[] players = new Player[20];
        public static int PlayersQuantity = 0;
        public static int FailurePlayersQuantity = 0;

        private static bool[] isStart = Enumerable.Repeat(true, 20).ToArray();
        public static bool[] isRunning = Enumerable.Repeat(true, 20).ToArray();
        private static float[] lastTime = Enumerable.Repeat(0f, 20).ToArray();

        public static RainWorldGame? RWG;
        public static Vector2 camPos = new Vector2(300f, 300f);


        public static PointerFSprite[] pointer = new PointerFSprite[20];

        // 初始化视觉系统
        public static VisionSystem visionSystem = new VisionSystem()
        {
            BaseVisionRadius = 120f,
            BackgroundColor = new Color(0f, 0f, 0f, 0f)
        };


        public static void Hook()
        {
            On.Player.ctor += Player_ctor;
            On.Player.Update += Player_Update;
            On.RoomCamera.SpriteLeaser.Update += SLeaser_Update;
            On.RainWorldGame.Update += RainWorldGame_Update;
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

                    visionSystem.Update();

                    int Failure = 0;
                    for (int i = 0; i < 20; i++)
                    {
                        // 检查B是否停止 - 比如超过一定时间没有运行
                        if (Time.time - lastTime[i] > 0.5f && !isStart[i])
                        { // 0.5秒阈值
                            isRunning[i] = false;
                        }
                        else
                        {
                            isRunning[i] = true;
                        }

                        if (isRunning[i] == false)
                        {
                            Failure += 1;
                            if (pointer != null && pointer[i] != null && !pointer[i].slatedForDestroy)
                            {
                                pointer[i].Destroy();
                            }
                        }


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
                    FailurePlayersQuantity = Failure;
                    if (PlayersQuantity == FailurePlayersQuantity)
                    {
                        PlayersQuantity = 0;
                        FailurePlayersQuantity = 0;

                        isStart = Enumerable.Repeat(true, 20).ToArray();
                        isRunning = Enumerable.Repeat(true, 20).ToArray();
                        lastTime = Enumerable.Repeat(0f, 20).ToArray();

                        for (int i = 0; i < visionSystem.VisionSpotsCount; i++)
                        {
                            visionSystem.RemoveVisionSpot(0);
                        }
                        visionSystem.Hide();
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

/*            if (_isMainUpdateRunning == false)
            {
                Task.Run(() => MainUpdate()); // 异步启动（避免阻塞）
            }*/
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
            Vector2 pos = self.firstChunk.pos;

            if (_isMainUpdateRunning == false && Running == false)
            {
                Running = true;
                Task.Run(() => MainUpdate()); // 异步启动（避免阻塞）
                //visionSystem.Show();
            }

            if (self.slugcatStats.name == Plugin.YourSlugID && SC.PerceptionSkill && isStart[N])
            {
                /*if (N == 0)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        if (pointer[i] != null && !pointer[i].slatedForDestroy)
                        {
                            pointer[i].Destroy();
                            pointer[i].slatedForDestroy = true;
                        }
                    }
                }*/
                PlayersQuantity += 1;
                pointer[N] = new PointerFSprite(self);
                pointer[N].slatedForDestroy = false;
                // 添加常规可视点
                visionSystem.AddVisionSpot(pos);
                visionSystem.Show();
            }

            // 动态移动第一个点
            var secondSpot = visionSystem.GetVisionSpot(N);
            if (secondSpot != null)
            {
                secondSpot.TargetPosition = pos; // 向右移动
            }

            players[N] = self;
            lastTime[N] = Time.time;
            isStart[N] = false;

            if (pointer != null && pointer[N] != null && !pointer[N].slatedForDestroy && self.slugcatStats.name == Plugin.YourSlugID)
            {
                //pointer[N].start = true;
                //pointer[N].i = pointer[N].lasti;
                //pointer[N].Update(N, false);
            }
        }


    }
}
