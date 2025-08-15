using System.Collections.Generic;
using RWCustom;
using UnityEngine;

namespace MySlugcat;

// DespawnAnimation 类继承自 UpdatableAndDeletable，负责处理生物消失的动画效果
class DespawnAnimation : UpdatableAndDeletable
{

	public readonly Creature creature;
	private int time = -200; // it's negative so that it will wait a bit before actually displaying particle effects
							 //它是负的，所以在实际显示粒子效果之前会等待一段时间
	private int phase = 0;

	private readonly List<Vector2> curvePositions = new();
	private Color dissolveColor = Color.red;

	// 构造函数，初始化 DespawnAnimation 对象，接受一个 Creature 对象作为参数
	public DespawnAnimation(Creature creature) : base()
	{
		this.creature = creature;
	}

	// Update 方法，每帧更新动画状态，接受一个布尔值 eu 作为参数
	public override void Update(bool eu)
	{
		if (phase == 3)
		{
			Destroy();
		}

		if (phase == 0 && creature.room is null)
		{
			creature.abstractCreature.Room?.RemoveEntity(creature.abstractCreature);
			AbCreatureModuleManager.UnregisterAbCreature(creature.abstractCreature);
			creature.Destroy();
			Destroy();
		}

		if (creature.room is not null)
		{
			UpdateParticleCurve();

			// get creature color
			// defaults to black
			//获取生物颜色
			//默认为黑色
			dissolveColor = room.game.cameras[0].currentPalette.blackColor;

			/*if (creature is Lizard lizard && lizard.Template.type == CreatureTemplate.Type.WhiteLizard)
			{
				dissolveColor = (lizard.graphicsModule as LizardGraphics).whiteCamoColor;
			}*/
		}

		if (phase == 0)
		{
			time++;

			if (time >= 0)
			{
				var animationProgress = time / 400f;
				SpawnBubble(animationProgress * 2.5f, dissolveColor);

				if (creature.graphicsModule is LizardGraphics lizardGfx && lizardGfx.lightSource is not null)
				{
					lizardGfx.lightSource.setAlpha *= 1f - animationProgress;
				}

				//SpriteTinter.SetColorData(creature.graphicsModule, dissolveColor, animationProgress);

				if (animationProgress >= 1f)
				{
					creature.AllGraspsLetGoOfThisObject(true);
					creature.LoseAllGrasps();
					creature.Destroy();
					phase++;
				}
			}
		}
		else if (phase == 1)
		{
			time -= 4;

			if (time <= 0)
			{
				phase++;
				Destroy();
			}

			var animationProgress = time / 400f;
			SpawnBubble(animationProgress * 2.5f, dissolveColor);
		}
	}

	// SpawnBubble 方法，在粒子曲线的随机点上生成溶解气泡，接受气泡的缩放比例和颜色作为参数
	private void SpawnBubble(float scale, Color color)
	{
		//在粒子曲线的随机点上产生溶解气泡
		Vector2 originPoint;
		if (curvePositions.Count <= 1)
		{
			originPoint = curvePositions[0];
		}
		else
		{
			int index = Random.Range(0, curvePositions.Count - 1);
			originPoint = Vector2.Lerp(curvePositions[index], curvePositions[index + 1], Random.value);
		}

		room.AddObject(new DissolveBubble(originPoint, scale, color));
	}

	// UpdateParticleCurve 方法，更新粒子曲线的位置，根据生物的身体部位位置生成曲线点
	private void UpdateParticleCurve()
	{
		var graphics = creature.graphicsModule;
		curvePositions.Clear();

		for (int i = 0; i < graphics.bodyParts.Length; i++)
		{
			curvePositions.Add(graphics.bodyParts[i].pos);
		}
	}
}
