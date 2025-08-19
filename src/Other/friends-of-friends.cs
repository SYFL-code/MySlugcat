using BepInEx;
using System.CodeDom.Compiler;
using System.Security.Permissions;
using static CreatureTemplate.Relationship.Type;


namespace MySlugcat;

sealed class friends_of_friends
{
	/// <summary>
	/// 查找两个生物之间的共同朋友
	/// </summary>
	public static Tracker.CreatureRepresentation MutualFriend(AbstractCreature one, AbstractCreature two)
	{
		if (two.state.dead || one.abstractAI?.RealAI?.tracker is not Tracker tracker || two?.abstractAI?.RealAI?.tracker is not Tracker)
		{
			return null;
		}

		foreach (Tracker.CreatureRepresentation friendRep in tracker.creatures)
		{
			AbstractCreature friend = friendRep.representedCreature;

			// 如果`self`是`friend`的朋友，且`other`也是`friend`的朋友，则他们是共同朋友
			if (friend != two && friend.state.alive && Friends(one, friend) && Friends(two, friend))
			{
				return friendRep;
			}
		}

		return null;
	}

	/// <summary>
	/// 判断两个生物是否是朋友
	/// </summary>
	public static bool Friends(AbstractCreature self, AbstractCreature other)
	{
		if (self?.state == null || other?.state == null || self.state.dead || other.state.dead)
		{
			return false;
		}
		// 所有的slugcats都是朋友。和平与爱
		if (self.realizedObject is Player && other.realizedObject is Player)
		{
			return true;
		}
		// 友谊追踪器的目标显然是朋友
		if (self.abstractAI?.RealAI?.friendTracker is FriendTracker f && f.friendRel?.subjectID == other.ID)
		{
			return true;
		}
		// 同一个小队的成员是朋友
		var otherRep = self.abstractAI?.RealAI?.tracker?.RepresentationForCreature(other, false);
		if (otherRep?.dynamicRelationship?.currentRelationship.type == Pack)
		{
			return true;
		}

		float rep = RepOfCreature(self, other);

		// 如果我们喜欢玩家，就和平相处
		if (rep > 0.5f)
		{
			return true;
		}

		// 如果我们讨厌玩家，就不和平相处
		if (rep < -0.5f)
		{
			return false;
		}

		// 如果我们对玩家的态度是中立，但其他玩家喜欢他们，就和平相处
		if (self.creatureTemplate?.communityID != null && self.world.game.session is StoryGameSession sess && sess.creatureCommunities is CreatureCommunities communities
			&& other.realizedCreature is Player p
			&& communities.LikeOfPlayer(self.creatureTemplate.communityID, self.world.region?.regionNumber ?? -1, p.playerState.playerNumber) > 0.8f)
		{
			return rep > -0.1f;
		}

		return false;
	}

	/// <summary>
	/// 计算一个生物对另一个生物的好感度
	/// </summary>
	public static float RepOfCreature(AbstractCreature self, AbstractCreature other)
	{
		var otherRep = self.abstractAI?.RealAI?.tracker?.RepresentationForCreature(other, false);

		float rep = 0;
		if (otherRep != null && other.realizedCreature is Player)
		{
			if (self.realizedObject is Scavenger scav) rep = scav.AI.LikeOfPlayer(otherRep.dynamicRelationship);
			if (self.realizedObject is Lizard liz) rep = liz.AI.LikeOfPlayer(otherRep);
			if (self.realizedObject is Cicada cicada) rep = cicada.AI.LikeOfPlayer(otherRep);
		}
		else if (self.state.socialMemory?.GetRelationship(other.ID)?.like is float like)
		{
			rep = like;
		}

		return rep;
	}

	/// <summary>
	/// 查找两个生物之间的朋友的朋友关系
	/// </summary>
	public static CreatureTemplate.Relationship? FriendOfFriendRelationship(AbstractCreature one, AbstractCreature two)
	{
		if (MutualFriend(one, two) is Tracker.CreatureRepresentation mutualFriend)
		{
			// 共同朋友的小队成员也是我们的好友
			if (mutualFriend.dynamicRelationship.currentRelationship.type == Pack)
			{
				return new CreatureTemplate.Relationship(Pack, mutualFriend.dynamicRelationship.currentRelationship.intensity * 0.5f);
			}

			// 否则礼貌地忽略他们
			return new CreatureTemplate.Relationship(Ignores, 0.5f);
		}

		// 检查是否都有一个共同的好友
		if (one.abstractAI?.RealAI?.friendTracker?.friend != null && two.abstractAI?.RealAI?.friendTracker?.friend != null &&
			one.abstractAI.RealAI.friendTracker.friend.abstractCreature.ID == two.abstractAI.RealAI.friendTracker.friend.abstractCreature.ID)
		{
			return new(one.abstractAI.RealAI.friendTracker.friendRel.like > 0.8f ? Pack : Ignores, 0.5f);
		}

		// 检查社交记忆
		if (one.state.socialMemory != null && two.state.socialMemory != null)
		{
			foreach (var rel in one.state.socialMemory.relationShips)
			{
				if (rel.like > 0.8f && two.state.socialMemory.GetLike(rel.subjectID) > 0.8f)
				{
					return new(Ignores, 0.5f);
				}
			}
		}

		// 检查生物社区
		if (one.world.game.GetStorySession?.creatureCommunities is CreatureCommunities communities && one.world.game.Players.Count > 0
			&& communities.LikeOfPlayer(one.creatureTemplate.communityID, one.world.region?.regionNumber ?? -1, 0) > 0.8f
			&& communities.LikeOfPlayer(two.creatureTemplate.communityID, two.world.region?.regionNumber ?? -1, 0) > 0.8f
			&& RepOfCreature(one, two) > -0.1f
			&& RepOfCreature(one, one.world.game.Players[0]) > -0.1f
			&& RepOfCreature(two, one.world.game.Players[0]) > -0.1f
			)
		{
			return new(Ignores, 0.5f);
		}

		return null;
	}

	/// <summary>
	/// 判断两个生物是否有共同好友
	/// </summary>
	public static bool FriendOfFriend(AbstractCreature one, AbstractCreature two)
	{
		return FriendOfFriendRelationship(one, two) != null;
	}

	// 插件启用时的初始化方法
	/*public static void Hook()
	{
		On.RelationshipTracker.DynamicRelationship.Update += DynamicRelationship_Update;
		On.LizardAI.DoIWantToBiteThisCreature += LizardAI_DoIWantToBiteThisCreature;
	}*/

	/// <summary>
	/// 动态关系更新方法
	/// </summary>
	public static void DynamicRelationship_Update(ref bool Execute, ref On.RelationshipTracker.DynamicRelationship.orig_Update orig, ref RelationshipTracker.DynamicRelationship self)
	{
		if (FriendOfFriendRelationship(self.rt.AI.creature, self.trackerRep.representedCreature) is CreatureTemplate.Relationship fof)
		{
			(self.rt.AI as IUseARelationshipTracker).UpdateDynamicRelationship(self);
			if (fof.type != self.currentRelationship.type)
			{
				self.rt.SortCreatureIntoModule(self, fof);
			}
			self.trackerRep.priority = fof.intensity * self.trackedByModuleWeigth;
			self.currentRelationship = fof;
			Execute = false;
		}
		else
		{
			orig(self);
		}
	}

	/// <summary>
	/// 判断蜥蜴AI是否想要咬一个生物
	/// </summary>
	public static bool LizardAI_DoIWantToBiteThisCreature(ref bool Execute, ref bool return_, ref On.LizardAI.orig_DoIWantToBiteThisCreature orig, ref LizardAI self, ref  Tracker.CreatureRepresentation otherCrit)
	{
		Execute = false;
		if (!orig(self, otherCrit))
		{
			return false;
		}
		// 如果我们有共同好友，不要战斗
		if (FriendOfFriend(self.creature, otherCrit.representedCreature))
		{
			return false;
		}
		return true;
	}
}

