using UnityEngine;


namespace MySlugcat;
class DissolveBubble : CosmeticSprite
{
	public float life;
	public float maxScale;
	public int lifeTime;
	public Vector2 originPoint;
	public float angle;
	public float dist;
	private Color color;

	public DissolveBubble(Vector2 originPt, float intensity, Color color)
	{
		originPoint = originPt;

		// 最大尺寸：0.1 ~ (0.1+0.15) * intensity
		maxScale = Mathf.Max(0.1f, Random.value * 0.15f + intensity);

		// 初始角度与偏移距离
		angle = Random.Range(0f, 2f * Mathf.PI);
		dist = Random.Range(0f, 4f);

		pos.x = originPoint.x + Mathf.Cos(angle) * dist;
		pos.y = originPoint.y + Mathf.Sin(angle) * dist;
		lastPos = pos;

		life = 1f;          // 生命值从 1 递减到 0
		lifeTime = 60;      // 60 帧后消失
		this.color = color; // 传入颜色
	}

	public override void Update(bool eu)
	{
		lastPos = pos;

		// 每帧减少生命值
		life -= 1f / lifeTime;
		if (life <= 0f)
			Destroy();

		// 角度持续旋转，使气泡绕原点做圆周运动
		angle += 0.1f;

		pos.x = originPoint.x + Mathf.Cos(angle) * dist;
		pos.y = originPoint.y + Mathf.Sin(angle) * dist;

		base.Update(eu);
	}

	public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
	{
		// 创建圆形矢量图形的精灵
		sLeaser.sprites = new FSprite[1];
		sLeaser.sprites[0] = new FSprite("Futile_White", true)
		{
			scaleX = maxScale,
			scaleY = maxScale,
			shader = rCam.game.rainWorld.Shaders["VectorCircle"]
		};

		AddToContainer(sLeaser, rCam, null);
	}

	public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
	{
		float scale = maxScale * life;

		var sprite = sLeaser.sprites[0];
		sprite.x = Mathf.Lerp(lastPos.x, pos.x, timeStacker) - camPos.x;
		sprite.y = Mathf.Lerp(lastPos.y, pos.y, timeStacker) - camPos.y;
		sprite.color = color;
		sprite.scaleX = 1f * scale;
		sprite.scaleY = 1f * scale;

		base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
	}

	public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
	{
		base.ApplyPalette(sLeaser, rCam, palette);
	}

	public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
	{
		newContatiner ??= rCam.ReturnFContainer("Midground");

		foreach (FSprite fsprite in sLeaser.sprites)
		{
			fsprite.RemoveFromContainer();
			newContatiner.AddChild(fsprite);
		}
	}
}
