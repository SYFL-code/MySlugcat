using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using Menu;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace MySlugcat
{
    public class Options : OptionInterface
    {

        public readonly Configurable<float> PixelSize;
		public readonly Configurable<float> Alpha;

		public readonly Configurable<bool> AllPlayerSkill;

		public readonly Configurable<bool> FrameSkill;
        public readonly Configurable<bool> DeflagrationSkill;
        public readonly Configurable<bool> KnitmeshSkill;

        public readonly Configurable<bool> LogDebug;
        public readonly Configurable<float> Loglevel;

		public static readonly Options Instance = new Options();

		OpTextBox? loglevelTextBox;
        OpLabel? loglevelLabel;

		Options()
		{
			//设置默认值
			PixelSize = config.Bind<float>("PixelSize_conf", 10f);
			Alpha = config.Bind<float>("Alpha_conf", 0.9f);

			AllPlayerSkill = config.Bind<bool>("AllPlayerSkill_conf", false);

			FrameSkill = config.Bind<bool>("FrameSkill_conf", false);
			DeflagrationSkill = config.Bind<bool>("DeflagrationSkill_conf", false);
			KnitmeshSkill = config.Bind<bool>("KnitmeshSkill_conf", false);

			LogDebug = config.Bind<bool>("logDebug_conf", false);
			Loglevel = config.Bind<float>("Loglevel", 10f);
		}

		public override void Initialize()
        {
			OpTab opTab = new OpTab(this, "Options");
			InGameTranslator inGameTranslator = Custom.rainWorld.inGameTranslator;
			this.Tabs = new OpTab[]
			{
				opTab
			};
			//标题
			opTab.AddItems(new UIelement[]
			{
				new OpLabel(10f, 540f, inGameTranslator.Translate("The Accommodator"), true)
				{
					alignment = FLabelAlignment.Left
				}
			});
			//选项
			opTab.AddItems(new UIelement[]
			{
				new OpTextBox(PixelSize, new Vector2(10, 450), 40f),
				new OpLabel(new Vector2(75f, 450f), new Vector2(200f, 24f), inGameTranslator.Translate("PixelSize"), FLabelAlignment.Left, false, null),
				new OpTextBox(Alpha, new Vector2(10, 420), 40f),
				new OpLabel(new Vector2(75f, 420f), new Vector2(200f, 24f), inGameTranslator.Translate("Alpha"), FLabelAlignment.Left, false, null),

				new OpCheckBox(AllPlayerSkill, new Vector2(10, 360)),
				new OpLabel(new Vector2(75f, 360f), new Vector2(200f, 24f), inGameTranslator.Translate("AllPlayerSkill"), FLabelAlignment.Left, false, null),

				new OpCheckBox(LogDebug, new Vector2(10, 50)),
				new OpLabel(new Vector2(75f, 50f), new Vector2(200f, 24f), inGameTranslator.Translate("LogDebug"), FLabelAlignment.Left, false, null),
				new OpTextBox(Loglevel, new Vector2(10, 20), 50f),
				new OpLabel(new Vector2(75f, 20f), new Vector2(200f, 24f), inGameTranslator.Translate("Loglevel"), FLabelAlignment.Left, false, null),

				/*new OpCheckBox(OpCheckBoxSaveIceData_conf, new Vector2(10, 390)),
				new OpLabel(new Vector2(50f, 390f), new Vector2(200f, 24f), inGameTranslator.Translate("Save Ice data to the next cycle(Save bug not fixed yet)"), FLabelAlignment.Left, false, null),
				new OpCheckBox(OpCheckBoxUnlockIceShieldNum_conf, new Vector2(10, 360)),
				new OpLabel(new Vector2(50f, 360f), new Vector2(200f, 24f), inGameTranslator.Translate("Unlock the maximum number of ice shields"), FLabelAlignment.Left, false, null),*/

				//new OpLabel(new Vector2(50f, 420f), new Vector2(200f, 24f), inGameTranslator.Translate("If scavenger dies, the players continue playing"), FLabelAlignment.Left, false, null),
				/*radioButtonGroup,
				radioButton1,
				radioButton2*/
			});
		}



    }
}
