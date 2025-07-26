using On;
using IL;
using System;
using Mono.Cecil;
using MoreSlugcats;
using Noise;
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
using static MonoMod.InlineRT.MonoModRule;


namespace MySlugcat
{
    //视觉异常
    public class VisionSystem
    {
        // 核心组件
        private FContainer? visionContainer;
        private FSprite background;
        private List<VisionSpot> visionSpots = new List<VisionSpot>();

        // 配置参数
        public float BaseVisionRadius { get; set; } = 100f;
        public Color BackgroundColor { get; set; } = Color.black;
        public int RenderOrder { get; set; } = 1000;

        /// <summary>
        /// 获取当前可视点数量
        /// </summary>
        public int VisionSpotsCount => visionSpots.Count;

        /// <summary>
        /// 通过索引获取可视点
        /// </summary>
        public VisionSpot? GetVisionSpot(int index)
        {
            if (index >= 0 && index < visionSpots.Count)
            {
                return visionSpots[index];
            }
            return null;
        }

        /// <summary>
        /// 获取所有可视点
        /// </summary>
        public IEnumerable<VisionSpot> GetAllVisionSpots()
        {
            foreach (var spot in visionSpots)
            {
                yield return spot;
            }
        }


        // 初始化视觉系统
        public VisionSystem()
        {
            visionContainer = new FContainer();
            Futile.stage.AddChild(visionContainer);
            visionContainer.sortZ = RenderOrder;

            // 创建全屏背景
            background = new FSprite("pixel")
            {
                width = 1800,
                height = 1800,
                color = BackgroundColor,
                x = 1800 / 2f,
                y = 1800 / 2f
            };
            visionContainer.AddChild(background);

            // 确保初始状态完全透明且不可见
            visionContainer.alpha = 0f;
            visionContainer.isVisible = false;
            background.alpha = 0f;
            //visionSpots.alpha = 0f;
        }

        public void Hide()
        {
            if (visionContainer != null)
            {
                visionContainer.alpha = 0f;
                visionContainer.isVisible = false;
                background.alpha = 0f;
            }
        }

        public void Show()
        {
            if (visionContainer != null)
            {
                visionContainer.alpha = 1f;
                visionContainer.isVisible = true;
                background.alpha = 1f;
            }
        }

        // 添加新的可视点
        public void AddVisionSpot(Vector2 position, float? radius = null)
        {
            if (visionContainer != null)
            {
                float spotRadius = radius ?? BaseVisionRadius;
                VisionSpot newSpot = new VisionSpot(position, spotRadius);
                visionContainer.AddChild(newSpot.Circle);
                visionSpots.Add(newSpot);
            }
        }

        // 移除可视点
        public void RemoveVisionSpot(int index)
        {
            if (index >= 0 && index < visionSpots.Count && visionContainer != null)
            {
                visionContainer.RemoveChild(visionSpots[index].Circle);
                visionSpots.RemoveAt(index);
            }
        }

        // 更新系统
        public void Update()
        {
            foreach (var spot in visionSpots)
            {
                spot.Update();
            }
        }

        // 清理资源
        public void Destroy()
        {
            if (visionContainer != null)
            {
                Futile.stage.RemoveChild(visionContainer);
                visionContainer.RemoveAllChildren();
                visionContainer = null;
            }
            visionSpots.Clear();
        }

        // 可视点内部类
        public class VisionSpot
        {
            public FSprite Circle { get; private set; }
            public Vector2 TargetPosition { get; set; }
            public float TargetRadius { get; set; }

            // 平滑移动参数
            public Vector2 currentPosition;
            public float currentRadius;
            public float PositionLerpFactor { get; set; } = 0.2f;
            public float RadiusLerpFactor { get; set; } = 0.1f;

            internal bool ShouldRemove { get; private set; }

            public VisionSpot(Vector2 position, float radius)
            {
                TargetPosition = position;
                TargetRadius = radius;
                currentPosition = position;
                currentRadius = radius;

                Circle = new FSprite("Circle20")
                {
                    x = position.x,
                    y = position.y,
                    scale = radius / 20f,
                    color = new Color(1f, 1f, 1f, 0f) // 透明，用于遮罩
                };
            }

            public void Update()
            {
                // 平滑移动
                currentPosition = Vector2.Lerp(currentPosition, TargetPosition, PositionLerpFactor);
                currentRadius = Mathf.Lerp(currentRadius, TargetRadius, RadiusLerpFactor);

                // 更新精灵
                Circle.x = currentPosition.x;
                Circle.y = currentPosition.y;
                Circle.scale = currentRadius / 20f;
            }

            /// <summary>
            /// 立即移除这个可视点
            /// </summary>
            public void Remove()
            {
                ShouldRemove = true;
            }

        }
    }
}
