using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Menu;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace MySlugcat
{
    public class Options : OptionInterface
    {
        public int curTab;

        public static Configurable<bool>? logDebug;

        public static Configurable<bool>? copyID;

        public static Configurable<float>? loglevel;

        OpTextBox? loglevelTextBox;
        OpLabel? loglevelLabel;

        public Options()
        {
            Options.logDebug = this.config.Bind<bool>("logDebug", false, new ConfigurableInfo("Useful for debugging if you share your log files.", null, "", new object[]
            {
                "Log debug"
            }));
            Options.copyID = this.config.Bind<bool>("copyID", true, new ConfigurableInfo("Creates an exact copy of the previous object when duplicating.", null, "", new object[]
            {
                "Copy ID duplicate"
            }));
            Options.loglevel = this.config.Bind<float>("loglevel", 9f, new ConfigurableInfo("The maximum value is 10, and the minimum value is 0.", null, "", new object[]
			{
                "Log Level"
            }));
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Tabs = new OpTab[]
{
                new OpTab(this, "General 1"),
                new OpTab(this, "General 2"),
                new OpTab(this, "Tools 1"),
                new OpTab(this, "Tools 2"),
                new OpTab(this, "Tool Settings")
};
            this.curTab = 0;
            this.AddTitle();
            float num = 90f;
            float num2 = 460f;
            float num3 = 40f;
            if (logDebug != null && copyID != null)
            {
                this.AddCheckBox(Options.logDebug, new Vector2(num, num2 -= num3), null);
                this.AddCheckBox(Options.copyID, new Vector2(num, num2 -= num3), null);
            }

            //this.AddTextBox<float>(Options.loglevel, new Vector2(num, num2 -= num3), 50f);

            if (loglevel != null)
            {
                Vector2 pos = new Vector2(num, num2 -= num3);
                float width = 50f;
                OpTextBox loglevelTextBox = new OpTextBox(Options.loglevel, pos, width)
                {
                    allowSpace = true,
                    description = Options.loglevel.info.description
                };
                OpLabel loglevelLabel = new OpLabel(pos.x + width + 18f, pos.y + 2f, Options.loglevel.info.Tags[0] as string, false)
                {
                    description = Options.loglevel.info.description
                };
                this.Tabs[this.curTab].AddItems(new UIelement[]
                {
                loglevelTextBox,
                loglevelLabel
                });
            }
        }

        public override void Update()
        {
            if (loglevelTextBox != null && loglevelLabel != null)
            {
                if (logDebug == null || logDebug.Value)
                {
                    loglevelTextBox.Show();
                    loglevelLabel.Show();
                }
                else
                {
                    loglevelTextBox.Hide();
                    loglevelLabel.Hide();
                }
            }

        }

        private void AddTitle()
        {
            OpLabel opLabel = new OpLabel(new Vector2(150f, 560f), new Vector2(300f, 30f), "Mouse Drag", FLabelAlignment.Center, true, null);
            OpLabel opLabel2 = new OpLabel(new Vector2(150f, 540f), new Vector2(300f, 30f), "Version 1.1.0", FLabelAlignment.Center, false, null);
            this.Tabs[this.curTab].AddItems(new UIelement[]
            {
                opLabel,
                opLabel2
            });
        }

        private void AddCheckBox(Configurable<bool> option, Vector2 pos, Color? c = null)
        {
            if (c == null)
            {
                c = new Color?(MenuColorEffect.rgbMediumGrey);
            }
            OpCheckBox opCheckBox = new OpCheckBox(option, pos)
            {
                description = option.info.description,
                colorEdge = c.Value
            };
            OpLabel opLabel = new OpLabel(pos.x + 40f, pos.y + 2f, option.info.Tags[0] as string, false)
            {
                description = option.info.description,
                color = c.Value
            };
            this.Tabs[this.curTab].AddItems(new UIelement[]
            {
                opCheckBox,
                opLabel
            });
        }

        private void AddTextBox<T>(Configurable<T> option, Vector2 pos, float width = 150f)
        {
            OpTextBox opTextBox = new OpTextBox(option, pos, width)
            {
                allowSpace = true,
                description = option.info.description
            };
            OpLabel opLabel = new OpLabel(pos.x + width + 18f, pos.y + 2f, option.info.Tags[0] as string, false)
            {
                description = option.info.description
            };
            this.Tabs[this.curTab].AddItems(new UIelement[]
            {
                opTextBox,
                opLabel
            });
        }

        public void PostTranslate()
        {
            IEnumerable<FieldInfo> enumerable = from f in base.GetType().GetFields(BindingFlags.Static | BindingFlags.Public)
                                                where f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(Configurable<>)
                                                select f;
            RainWorld rainWorld = Custom.rainWorld;
            if (((rainWorld != null) ? rainWorld.inGameTranslator : null) == null || enumerable == null)
            {
                return;
            }
            foreach (FieldInfo fieldInfo in enumerable)
            {
                ConfigurableBase? configurableBase = (ConfigurableBase?)((fieldInfo != null) ? fieldInfo.GetValue(null) : null);
                string? value;
                if (configurableBase == null)
                {
                    value = null;
                }
                else
                {
                    ConfigurableInfo info = configurableBase.info;
                    value = ((info != null) ? info.description : null);
                }
                if (!string.IsNullOrEmpty(value) && configurableBase != null)
                {
                    configurableBase.info.description = Custom.rainWorld.inGameTranslator.Translate(configurableBase.info.description.Replace("\n", "<LINE>")).Replace("<LINE>", "\n");
                }
                int num = 0;
                for (; ; )
                {
                    int num2 = num;
                    int? num3;
                    if (configurableBase == null)
                    {
                        num3 = null;
                    }
                    else
                    {
                        ConfigurableInfo info2 = configurableBase.info;
                        if (info2 == null)
                        {
                            num3 = null;
                        }
                        else
                        {
                            object[] tags = info2.Tags;
                            num3 = ((tags != null) ? new int?(tags.Count<object>()) : null);
                        }
                    }
                    int? num4 = num3;
                    if (!(num2 < num4.GetValueOrDefault() & num4 != null))
                    {
                        break;
                    }
                    if (configurableBase != null && !string.IsNullOrEmpty(configurableBase.info.Tags[num] as string))
                    {
                        configurableBase.info.Tags[num] = Custom.rainWorld.inGameTranslator.Translate(((string)configurableBase.info.Tags[num]).Replace("\n", "<LINE>")).Replace("<LINE>", "\n");
                    }
                    num++;
                }
            }
            int num5 = 0;
            for (; ; )
            {
                int num6 = num5;
                OpTab[] tabs = this.Tabs;
                int? num4 = (tabs != null) ? new int?(tabs.Length) : null;
                if (!(num6 < num4.GetValueOrDefault() & num4 != null))
                {
                    break;
                }
                if (this.Tabs[num5] != null && !string.IsNullOrEmpty(this.Tabs[num5].name))
                {
                    this.Tabs[num5].name = Custom.rainWorld.inGameTranslator.Translate(this.Tabs[num5].name.Replace("\n", "<LINE>")).Replace("<LINE>", "\n");
                }
                num5++;
            }
            Configurable<bool>? configurable = Options.logDebug;
            if (configurable == null || configurable.Value)
            {
                Log.Logger(2, "Options.PostTranslate, completed translation of options");
            }
        }


    }
}
