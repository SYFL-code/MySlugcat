using System;
using UnityEngine;
using static MySlugcat.PlayerModuleManager.PlayerModule;


namespace MySlugcat
{
	public class Conversation
	{
		public static void Hook()
		{
			On.SLOracleBehaviorHasMark.InitateConversation += MoonConversation_AddEvents;
			On.SSOracleBehavior.PebblesConversation.AddEvents += PebblesConversation_AddEvents;
		}

		private static void MoonConversation_AddEvents(On.SLOracleBehaviorHasMark.orig_InitateConversation orig, SLOracleBehaviorHasMark sLOracleBehaviorHasMark)
		{
			foreach (Player player in sLOracleBehaviorHasMark.PlayersInRoom)
			{
				if (player.slugcatStats.name == Plugin.YourSlugID)
				{
					//你好小生灵。15
					//Hello, little creature. 15
					//我看得出来你似乎经过了一些改造？但看着不太像是五块卵石。0
					//I can see you appear to have undergone some modifications, but they don’t look like Five Pebbles’ work. 0
					//因为他被自己的实验从内部腐化了。也许现在全被腐化了。所以他现在应该没有这样的能力。10
					//His own experiments have rotted him from the inside. Perhaps he is completely corrupted now. So he should be incapable of this. 10
					//也许你应该去找他。如果他还没有全被腐化，他可能有能力帮助你。10
					//Perhaps you should go to him. If he isn’t entirely lost, he might still be able to help you. 10
					//你也看得出来，我没有任何办法可以帮助你，也没有任何东西可以给你。甚至连我的记忆也不行。10
					//As you can see, I have no way to help you, nothing to give you—not even my memories. 10
					sLOracleBehaviorHasMark.dialogBox.NewMessage(sLOracleBehaviorHasMark.Translate(
						"Hello, little creature."), 15);

					sLOracleBehaviorHasMark.dialogBox.NewMessage(sLOracleBehaviorHasMark.Translate(
						"I can see you appear to have undergone some modifications, but they don’t look like Five Pebbles’ work."), 0);

					sLOracleBehaviorHasMark.dialogBox.NewMessage(sLOracleBehaviorHasMark.Translate(
						"His own experiments have rotted him from the inside. Perhaps he is completely corrupted now. So he should be incapable of this."), 10);

					sLOracleBehaviorHasMark.dialogBox.NewMessage(sLOracleBehaviorHasMark.Translate(
						"Perhaps you should go to him. If he isn’t entirely lost, he might still be able to help you."), 10);

					sLOracleBehaviorHasMark.dialogBox.NewMessage(sLOracleBehaviorHasMark.Translate(
						"As you can see, I have no way to help you, nothing to give you—not even my memories."), 10);

					return;
				}
			}
			orig.Invoke(sLOracleBehaviorHasMark);
		}

		private static void PebblesConversation_AddEvents(On.SSOracleBehavior.PebblesConversation.orig_AddEvents orig, SSOracleBehavior.PebblesConversation pebblesConversation)
		{
			foreach (Player player in pebblesConversation.owner.PlayersInRoom)
			{
				if (player.slugcatStats.name == Plugin.YourSlugID)
				{
					//又来了一只时日不多的小生物。15
					//Yet another little creature whose days are numbered. 15
					//一只经过另一个迭代器特殊改造的小生物，但看样子改造失败了。5
					//A small being specially modified by another iterator, but the modification appears to have failed. 5
					//25
					//从你的内部，我感受到了一些很微弱的、处于沉睡的东西，但我不确定那是什么。 10
					//From within you, I sense something very faint and dormant, though I’m not sure what it is. 10
					//它们似乎可以帮助你，我可以尝试帮你唤醒它们。但我不确定你能从它们中得到什么。但我希望这能帮到你。10
					//They seem able to help you; I could try to awaken them for you. I don’t know what you’ll gain from them, but I hope it helps. 10
					// 唤醒
					// Awaken

					// 判断
					// 1.T
					//这……15
					//…This… 15
					//你能感觉到什么？10
					//Can you feel anything? 10
					//我没法确定发生了什么，但我可以肯定你现在得到了一些能力。10
					//I can’t be certain what happened, but I’m sure you’ve gained some abilities. 10

					// 2.F
					//也许只是我多疑了。10
					//Perhaps I’m just being paranoid. 10
					//什么都没有发生……15
					//Nothing has happened… 15

					//……25
					//…25
					//算了，反正都是被困在了一个周而复始的循环当中。0
					//Forget it—either way, you're stuck in a cycle, a repeating pattern.0
					//要知道这并不会让你变得与众不同，从处理层中的微生物，到像我这样的——请见谅<LINE>——相对来说如神一般的存在而言，每个生物都遭遇着相同的困境。0
					//Know that this does not make you special - every living thing shares that same frustration.<LINE>From the microbes in the processing strata to me, who am, if you excuse me, godlike in comparison.0
					//先告诉你好消息。我和我的同类<LINE>的目标，就是解决你和世间万物心中潜藏的那份幽闭恐惧症。<LINE>真是一次奇怪的施舍——你是个一无所知的受益者，而我是个不情不愿的礼物。至于那些高尚的施舍者呢？<LINE>已经不在了。0
					//The good news first. Me and my kind have as our<LINE>purpose to solve that very oscillating claustrophobia in the chests of you and countless others.<LINE>A strange charity - you the unknowing recipient, I the reluctant gift. The noble benefactors?<LINE>Gone.0
					//坏消息是，我们还没有找到确定的答案。而我们的设备每时每刻都因为风化腐蚀的原因变得更糟。<LINE>我救不了天下众生，也救不了你。我甚至救不了我自己。0
					//The bad news is that no definitive solution has been found. And every moment the equipment erodes to a new state of decay.<LINE>I can't help you collectively, or individually. I can't even help myself.0
					//10
					//不过对你来说，还有另外一个方法。一条老路。向西穿过农场阵列，然后在大地断裂之处深入地下，<LINE>到你能抵达的最深处，古代文明就在那里建造了他们的神殿并跳着他们愚蠢的仪式舞蹈。0
					//For you though, there is another way. The old path. Go to the west past the Farm Arrays, and then down into the earth<LINE>where the land fissures, as deep as you can reach, where the ancients built their temples and danced their silly rituals.0
					//这只能解决你自己的问题，没办法解决其他生灵的。0
					//Not that it solves anyone's problem but yours.0


					pebblesConversation.dialogBox.NewMessage(
						pebblesConversation.Translate("Yet another little creature whose days are numbered."), 15);

					pebblesConversation.dialogBox.NewMessage(
						pebblesConversation.Translate("A small being specially modified by another iterator, but the modification appears to have failed."), 5);

					pebblesConversation.events.Add(
						new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(
							pebblesConversation, pebblesConversation.convBehav, 25));

					pebblesConversation.dialogBox.NewMessage(
						pebblesConversation.Translate("From within you, I sense something very faint and dormant, though I'm not sure what it is."), 10);

					pebblesConversation.dialogBox.NewMessage(
						pebblesConversation.Translate("They seem able to help you; I could try to awaken them for you. I don't know what you'll gain from them, but I hope it helps."), 10);

					pebblesConversation.dialogBox.NewMessage(
						pebblesConversation.Translate("Awaken"), 0);

					pebblesConversation.events.Add(
						new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(
							pebblesConversation, pebblesConversation.convBehav, 60));

					bool hasSurvivorPassage = HasPassage(player, TheSurvivorPassage);

					if (hasSurvivorPassage)
					{
						pebblesConversation.dialogBox.NewMessage(
							pebblesConversation.Translate("…This…"), 15);

						pebblesConversation.dialogBox.NewMessage(
							pebblesConversation.Translate("Can you feel anything?"), 10);

						pebblesConversation.dialogBox.NewMessage(
							pebblesConversation.Translate("I can’t be certain what happened, but I’m sure you’ve gained some abilities."), 10);
					}
					else
					{
						pebblesConversation.dialogBox.NewMessage(
							pebblesConversation.Translate("Perhaps I’m just being paranoid."), 10);

						pebblesConversation.dialogBox.NewMessage(
							pebblesConversation.Translate("Nothing has happened…"), 15);
					}

					pebblesConversation.events.Add(new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(pebblesConversation, pebblesConversation.convBehav, 
						25));

					pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate(
						"…"), 0);

					pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate(
						"Forget it—either way, you're stuck in a cycle, a repeating pattern."), 0);

					pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate(
						"Know that this does not make you special - every living thing shares that same frustration.<LINE>From the microbes in the processing strata to me, who am, if you excuse me, godlike in comparison."), 0);

					pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate(
						"The good news first. Me and my kind have as our<LINE>purpose to solve that very oscillating claustrophobia in the chests of you and countless others.<LINE>A strange charity - you the unknowing recipient, I the reluctant gift. The noble benefactors?<LINE>Gone."), 0);

					pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate(
						"The bad news is that no definitive solution has been found. And every moment the equipment erodes to a new state of decay.<LINE>I can't help you collectively, or individually. I can't even help myself."), 0);

					pebblesConversation.events.Add(new SSOracleBehavior.PebblesConversation.PauseAndWaitForStillEvent(pebblesConversation, pebblesConversation.convBehav, 
						10));

					pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate(
						"For you though, there is another way. The old path. Go to the west past the Farm Arrays, and then down into the earth<LINE>where the land fissures, as deep as you can reach, where the ancients built their temples and danced their silly rituals."), 0);

					pebblesConversation.dialogBox.NewMessage(pebblesConversation.Translate(
						"Not that it solves anyone's problem but yours."), 0);

					return;
				}
			}
			orig.Invoke(pebblesConversation);
		}


	}
}

