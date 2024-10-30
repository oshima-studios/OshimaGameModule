using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
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

        public override Dictionary<string, Skill> Skills
        {
            get
            {
                return Factory.GetGameModuleInstances<Skill>(OshimaGameModuleConstant.General, OshimaGameModuleConstant.Skill);
            }
        }

        protected override Factory.EntityFactoryDelegate<Skill> SkillFactory()
        {
            return (id, name, args) =>
            {
                Skill skill = id switch
                {
                    (long)MagicID.冰霜攻击 => new 冰霜攻击(),
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
                    (long)PassiveID.玻璃大炮 => new 玻璃大炮(),
                    (long)PassiveID.累积之压 => new 累积之压(),
                    (long)PassiveID.敏捷之刃 => new 敏捷之刃(),
                    (long)PassiveID.弱者猎手 => new 弱者猎手(),
                    (long)ItemPassiveID.攻击之爪 => new 攻击之爪技能(),
                    _ => new OpenSkill(id, name, args)
                };

                if (skill is OpenSkill && args.TryGetValue("values", out object? value) && value is Dictionary<string, object> dict)
                {
                    foreach (string key in dict.Keys)
                    {
                        skill.Values[key] = dict[key];
                    }
                }

                return skill;
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
                        _ => null
                    };
                }
                return null;
            };
        }
    }
}
