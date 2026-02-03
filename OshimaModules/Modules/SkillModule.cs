using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base.Addons;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaModules.Effects.ItemEffects;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules
{
    public class SkillModule : Milimoe.FunGame.Core.Library.Common.Addon.SkillModule, IHotReloadAware
    {
        public override string Name => OshimaGameModuleConstant.Skill;
        public override string Description => OshimaGameModuleConstant.Description;
        public override string Version => OshimaGameModuleConstant.Version;
        public override string Author => OshimaGameModuleConstant.Author;
        public Dictionary<string, Skill> KnownSkills { get; } = [];

        public override Dictionary<string, Skill> Skills
        {
            get
            {
                Dictionary<string, Skill> skills = Factory.GetGameModuleInstances<Skill>(OshimaGameModuleConstant.General, OshimaGameModuleConstant.Skill);
                if (KnownSkills.Count == 0 && skills.Count > 0)
                {
                    foreach (string key in skills.Keys)
                    {
                        KnownSkills[key] = skills[key];
                    }
                }
                return skills;
            }
        }

        protected override void AfterLoad()
        {
            General.GameplayEquilibriumConstant.InGameTime = "秒";
            General.GameplayEquilibriumConstant.InGameMaterial = "钻石";
            General.GameplayEquilibriumConstant.UseMagicType = [MagicType.None];
        }

        public void OnBeforeUnload()
        {

        }

        protected override Factory.EntityFactoryDelegate<Skill> SkillFactory()
        {
            return (id, name, args) =>
            {
                return id switch
                {
                    (long)MagicID.冰霜攻击 => new 冰霜攻击(),
                    (long)MagicID.火之矢 => new 火之矢(),
                    (long)MagicID.水之矢 => new 水之矢(),
                    (long)MagicID.石之锤 => new 石之锤(),
                    (long)MagicID.风之轮 => new 风之轮(),
                    (long)MagicID.心灵之霞 => new 心灵之霞(),
                    (long)MagicID.次元上升 => new 次元上升(),
                    (long)MagicID.暗物质 => new 暗物质(),
                    (long)MagicID.回复术 => new 回复术(),
                    (long)MagicID.治愈术 => new 治愈术(),
                    (long)MagicID.复苏术 => new 复苏术(),
                    (long)MagicID.圣灵术 => new 圣灵术(),
                    (long)MagicID.时间加速 => new 时间加速(),
                    (long)MagicID.时间减速 => new 时间减速(),
                    (long)MagicID.反魔法领域 => new 反魔法领域(),
                    (long)MagicID.沉默十字 => new 沉默十字(),
                    (long)MagicID.虚弱领域 => new 虚弱领域(),
                    (long)MagicID.混沌烙印 => new 混沌烙印(),
                    (long)MagicID.凝胶稠絮 => new 凝胶稠絮(),
                    (long)MagicID.大地之墙 => new 大地之墙(),
                    (long)MagicID.盖亚之盾 => new 盖亚之盾(),
                    (long)MagicID.风之守护 => new 风之守护(),
                    (long)MagicID.结晶防护 => new 结晶防护(),
                    (long)MagicID.强音之力 => new 强音之力(),
                    (long)MagicID.神圣祝福 => new 神圣祝福(),
                    (long)MagicID.根源屏障 => new 根源屏障(),
                    (long)MagicID.灾难冲击波 => new 灾难冲击波(),
                    (long)MagicID.银色荆棘 => new 银色荆棘(),
                    (long)MagicID.等离子之波 => new 等离子之波(),
                    (long)MagicID.地狱之门 => new 地狱之门(),
                    (long)MagicID.钻石星尘 => new 钻石星尘(),
                    (long)MagicID.死亡咆哮 => new 死亡咆哮(),
                    (long)MagicID.鬼魅之痛 => new 鬼魅之痛(),
                    (long)MagicID.导力停止 => new 导力停止(),
                    (long)MagicID.冰狱冥嚎 => new 冰狱冥嚎(),
                    (long)MagicID.火山咆哮 => new 火山咆哮(),
                    (long)MagicID.水蓝轰炸 => new 水蓝轰炸(),
                    (long)MagicID.岩石之息 => new 岩石之息(),
                    (long)MagicID.弧形日珥 => new 弧形日珥(),
                    (long)MagicID.苍白地狱 => new 苍白地狱(),
                    (long)MagicID.破碎虚空 => new 破碎虚空(),
                    (long)MagicID.弧光消耗 => new 弧光消耗(),
                    (long)MagicID.回复术改 => new 回复术改(),
                    (long)MagicID.回复术复 => new 回复术复(),
                    (long)MagicID.治愈术复 => new 治愈术复(),
                    (long)MagicID.风之守护复 => new 风之守护复(),
                    (long)MagicID.强音之力复 => new 强音之力复(),
                    (long)MagicID.结晶防护复 => new 结晶防护复(),
                    (long)MagicID.神圣祝福复 => new 神圣祝福复(),
                    (long)MagicID.时间加速改 => new 时间加速改(),
                    (long)MagicID.时间减速改 => new 时间减速改(),
                    (long)MagicID.时间加速复 => new 时间加速复(),
                    (long)MagicID.时间减速复 => new 时间减速复(),
                    (long)SkillID.疾风步 => new 疾风步(),
                    (long)SkillID.助威 => new 助威(),
                    (long)SkillID.挑拨 => new 挑拨(),
                    (long)SkillID.绞丝棍 => new 绞丝棍(),
                    (long)SkillID.金刚击 => new 金刚击(),
                    (long)SkillID.旋风轮 => new 旋风轮(),
                    (long)SkillID.双连击 => new 双连击(),
                    (long)SkillID.绝影 => new 绝影(),
                    (long)SkillID.胧 => new 胧(),
                    (long)SkillID.魔眼 => new 魔眼(),
                    (long)SkillID.天堂之吻 => new 天堂之吻(),
                    (long)SkillID.回复弹 => new 回复弹(),
                    (long)SkillID.养命功 => new 养命功(),
                    (long)SkillID.镜花水月 => new 镜花水月(),
                    (long)SkillID.剑风闪 => new 剑风闪(),
                    (long)SkillID.鲨鱼锚击 => new 鲨鱼锚击(),
                    (long)SkillID.疾走 => new 疾走(),
                    (long)SkillID.闪现 => new 闪现(),
                    (long)SuperSkillID.熵灭极诣 => new 熵灭极诣(),
                    (long)SuperSkillID.千羽瞬华 => new 千羽瞬华(),
                    (long)SuperSkillID.咒怨洪流 => new 咒怨洪流(),
                    (long)SuperSkillID.三相灵枢 => new 三相灵枢(),
                    (long)SuperSkillID.变幻之心 => new 变幻之心(),
                    (long)SuperSkillID.零式灭杀 => new 零式灭杀(),
                    (long)SuperSkillID.绝对领域 => new 绝对领域(),
                    (long)SuperSkillID.残香凋零 => new 残香凋零(),
                    (long)SuperSkillID.宿命时律 => new 宿命时律(),
                    (long)SuperSkillID.极寒渴望 => new 极寒渴望(),
                    (long)SuperSkillID.身心一境 => new 身心一境(),
                    (long)SuperSkillID.饕餮盛宴 => new 饕餮盛宴(),
                    (long)SuperSkillID.放监 => new 放监(),
                    (long)SuperSkillID.归元环 => new 归元环(),
                    (long)SuperSkillID.海王星的野望 => new 海王星的野望(),
                    (long)SuperSkillID.全军出击 => new 全军出击(),
                    (long)SuperSkillID.宿命之潮 => new 宿命之潮(),
                    (long)SuperSkillID.神之因果 => new 神之因果(),
                    (long)PassiveID.META马 => new META马(),
                    (long)PassiveID.心灵之弦 => new 心灵之弦(),
                    (long)PassiveID.蚀魂震击 => new 蚀魂震击(),
                    (long)PassiveID.灵能反射 => new 灵能反射(),
                    (long)PassiveID.双生流转 => new 双生流转(),
                    (long)PassiveID.零式崩解 => new 零式崩解(),
                    (long)PassiveID.少女绮想 => new 少女绮想(),
                    (long)PassiveID.暗香疏影 => new 暗香疏影(),
                    (long)PassiveID.破釜沉舟 => new 破釜沉舟(),
                    (long)PassiveID.累积之压 => new 累积之压(),
                    (long)PassiveID.银隼之赐 => new 银隼之赐(),
                    (long)PassiveID.弱者猎手 => new 弱者猎手(),
                    (long)PassiveID.开宫 => new 开宫(),
                    (long)PassiveID.八卦阵 => new 八卦阵(),
                    (long)PassiveID.深海之戟 => new 深海之戟(),
                    (long)PassiveID.概念之骰 => new 概念之骰(),
                    (long)PassiveID.雇佣兵团 => new 雇佣兵团(),
                    (long)PassiveID.不息之流 => new 不息之流(),
                    (long)PassiveID.征服者 => new 征服者(),
                    (long)PassiveID.致命节奏 => new 致命节奏(),
                    (long)PassiveID.强攻 => new 强攻(),
                    (long)PassiveID.电刑 => new 电刑(),
                    (long)PassiveID.黑暗收割 => new 黑暗收割(),
                    (long)PassiveID.迅捷步法 => new 迅捷步法(),
                    (long)PassiveID.贪欲猎手 => new 贪欲猎手(),
                    (long)ItemPassiveID.攻击之爪 => new 攻击之爪技能(),
                    (long)ItemPassiveID.糖糖一周年纪念武器 => new 糖糖一周年纪念武器技能(),
                    (long)ItemPassiveID.糖糖一周年纪念防具 => new 糖糖一周年纪念防具技能(),
                    (long)ItemPassiveID.糖糖一周年纪念鞋子 => new 糖糖一周年纪念鞋子技能(),
                    (long)ItemPassiveID.糖糖一周年纪念饰品1 => new 糖糖一周年纪念饰品技能1(),
                    (long)ItemPassiveID.糖糖一周年纪念饰品2 => new 糖糖一周年纪念饰品技能2(),
                    (long)ItemActiveID.经验书 => new 经验书技能(),
                    (long)ItemActiveID.礼包 => new 礼包技能(),
                    _ => null
                };
            };
        }

        protected override Factory.EntityFactoryDelegate<Effect> EffectFactory()
        {
            return (id, name, args) =>
            {
                if (args.TryGetValue("skill", out object? value) && value is Skill skill && args.TryGetValue("values", out value) && value is Dictionary<string, object> dict)
                {
                    return (EffectID)id switch
                    {
                        EffectID.ExATK => new ExATK(skill, dict),
                        EffectID.ExDEF => new ExDEF(skill, dict),
                        EffectID.ExSTR => new ExSTR(skill, dict),
                        EffectID.ExAGI => new ExAGI(skill, dict),
                        EffectID.ExINT => new ExINT(skill, dict),
                        EffectID.SkillHardTimeReduce => new SkillHardTimeReduce(skill, dict),
                        EffectID.NormalAttackHardTimeReduce => new NormalAttackHardTimeReduce(skill, dict),
                        EffectID.AccelerationCoefficient => new AccelerationCoefficient(skill, dict),
                        EffectID.ExSPD => new ExSPD(skill, dict),
                        EffectID.ExActionCoefficient => new ExActionCoefficient(skill, dict),
                        EffectID.ExCDR => new ExCDR(skill, dict),
                        EffectID.ExMaxHP => new ExMaxHP(skill, dict),
                        EffectID.ExMaxMP => new ExMaxMP(skill, dict),
                        EffectID.ExCritRate => new ExCritRate(skill, dict),
                        EffectID.ExCritDMG => new ExCritDMG(skill, dict),
                        EffectID.ExEvadeRate => new ExEvadeRate(skill, dict),
                        EffectID.PhysicalPenetration => new PhysicalPenetration(skill, dict),
                        EffectID.MagicalPenetration => new MagicalPenetration(skill, dict),
                        EffectID.ExPDR => new ExPDR(skill, dict),
                        EffectID.ExMDF => new ExMDF(skill, dict),
                        EffectID.ExHR => new ExHR(skill, dict),
                        EffectID.ExMR => new ExMR(skill, dict),
                        EffectID.ExATK2 => new ExATK2(skill, dict),
                        EffectID.ExDEF2 => new ExDEF2(skill, dict),
                        EffectID.ExSTR2 => new ExSTR2(skill, dict),
                        EffectID.ExAGI2 => new ExAGI2(skill, dict),
                        EffectID.ExINT2 => new ExINT2(skill, dict),
                        EffectID.SkillHardTimeReduce2 => new SkillHardTimeReduce2(skill, dict),
                        EffectID.NormalAttackHardTimeReduce2 => new NormalAttackHardTimeReduce2(skill, dict),
                        EffectID.ExMaxHP2 => new ExMaxHP2(skill, dict),
                        EffectID.ExMaxMP2 => new ExMaxMP2(skill, dict),
                        EffectID.DynamicsEffect => new DynamicsEffect(skill, dict),
                        EffectID.IgnoreEvade => new IgnoreEvade(skill, dict),
                        EffectID.ExATR => new ExATR(skill, dict),
                        EffectID.ExMOV => new ExMOV(skill, dict),
                        EffectID.ExLifesteal => new ExLifesteal(skill, dict),
                        EffectID.RecoverHP => new RecoverHP(skill, dict),
                        EffectID.RecoverMP => new RecoverMP(skill, dict),
                        EffectID.RecoverHP2 => new RecoverHP2(skill, dict),
                        EffectID.RecoverMP2 => new RecoverMP2(skill, dict),
                        EffectID.GetEP => new GetEP(skill, dict),
                        EffectID.GetEXP => new GetEXP(skill, dict),
                        _ => null
                    };
                }
                return null;
            };
        }
    }
}
