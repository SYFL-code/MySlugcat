using ImprovedInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MySlugcat
{
    public static class GlobalVar
    {
        //调试字符串
        public static string? dbgstr;
        //玩家变量
        public static ConditionalWeakTable<Player, PlayerVar> playerVar = new ConditionalWeakTable<Player, PlayerVar>();
        //冰冻能力
        //public static ConditionalWeakTable<Creature, ColorRecord> freezeCreature = new ConditionalWeakTable<Creature, ColorRecord>();
        //减速能力
        //public static ConditionalWeakTable<Creature, SlowDownAbility> slowdownCreature = new ConditionalWeakTable<Creature, SlowDownAbility>();
        //数据保存字段
        public const string MySlugcat_LH_FrameSkill_Enable_savefield = "MySlugcat_LH_FrameSkill_Enable";
        public const string MySlugcat_LH_DeflagrationSkill_Enable_savefield = "MySlugcat_LH_DeflagrationSkill_Enable";
        public const string MySlugcat_LH_KnitmeshSkill_Enable_savefield = "MySlugcat_LH_KnitmeshSkill_Enable";
        //public const string glacier2_iceshield_lock_savefield = "THEGLACIER2_ICESHIELD_LOCK";
        //public const string glacier2_iceshield_count_savefield = "THEGLACIER2_ICESHIELD_COUNT";
        //全局系统变量
        public static RainWorldGame? game = null;
        //最后一次的存档数据
        public static bool MySlugcat_LH_FrameSkill_Enable;
        public static bool MySlugcat_LH_DeflagrationSkill_Enable;
        public static bool MySlugcat_LH_KnitmeshSkill_Enable;
        //public static bool glacier2_iceshield_lock;
        //public static string savedata_glacier2_iceshield_count = "";
        //是否使用存档数据载入玩家数据
        public static bool enableLoadData = false;
        //冰盾技能按键
        //public static readonly PlayerKeybind iceshield_skill = PlayerKeybind.Register("glacier2:iceshield_skill", "theglacier2", "Shield", KeyCode.S, KeyCode.Joystick1Button4);
/*        public static bool IsPressedIceShield(Player player)
        {
            return CustomInputExt.IsPressed(player, iceshield_skill);
        }*/
/*        public static void NewGameGlobalVarSet()
        {
            //新剧情模式禁用冰盾能力
            glacier2_iceshield_lock = true;
        }*/
    }
}

