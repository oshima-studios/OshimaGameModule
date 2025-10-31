using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core.Constant;
using Oshima.FunGame.OshimaModules.Effects.ItemEffects;
using Oshima.FunGame.OshimaModules.Effects.OpenEffects;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Skills;

namespace Oshima.FunGame.OshimaModules
{
    public class SkillModule : Milimoe.FunGame.Core.Library.Common.Addon.SkillModule
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
                    (long)SuperSkillID.力量爆发 => new 力量爆发(),
                    (long)SuperSkillID.天赐之力 => new 天赐之力(),
                    (long)SuperSkillID.魔法涌流 => new 魔法涌流(),
                    (long)SuperSkillID.三重叠加 => new 三重叠加(),
                    (long)SuperSkillID.变幻之心 => new 变幻之心(),
                    (long)SuperSkillID.精准打击 => new 精准打击(),
                    (long)SuperSkillID.绝对领域 => new 绝对领域(),
                    (long)SuperSkillID.能量毁灭 => new 能量毁灭(),
                    (long)SuperSkillID.迅捷之势 => new 迅捷之势(),
                    (long)SuperSkillID.嗜血本能 => new 嗜血本能(),
                    (long)SuperSkillID.平衡强化 => new 平衡强化(),
                    (long)SuperSkillID.血之狂欢 => new 血之狂欢(),
                    (long)PassiveID.META马 => new META马(),
                    (long)PassiveID.心灵之火 => new 心灵之火(),
                    (long)PassiveID.魔法震荡 => new 魔法震荡(),
                    (long)PassiveID.灵能反射 => new 灵能反射(),
                    (long)PassiveID.智慧与力量 => new 智慧与力量(),
                    (long)PassiveID.致命打击 => new 致命打击(),
                    (long)PassiveID.毁灭之势 => new 毁灭之势(),
                    (long)PassiveID.枯竭打击 => new 枯竭打击(),
                    (long)PassiveID.破釜沉舟 => new 破釜沉舟(),
                    (long)PassiveID.累积之压 => new 累积之压(),
                    (long)PassiveID.敏捷之刃 => new 敏捷之刃(),
                    (long)PassiveID.弱者猎手 => new 弱者猎手(),
                    (long)PassiveID.征服者 => new 征服者(),
                    (long)PassiveID.致命节奏 => new 致命节奏(),
                    (long)PassiveID.强攻 => new 强攻(),
                    (long)PassiveID.电刑 => new 电刑(),
                    (long)PassiveID.黑暗收割 => new 黑暗收割(),
                    (long)ItemPassiveID.攻击之爪 => new 攻击之爪技能(),
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
