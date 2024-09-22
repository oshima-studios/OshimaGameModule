using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
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

        public override List<Skill> Skills
        {
            get
            {
                EntityModuleConfig<Skill> config = new(OshimaGameModuleConstant.General, OshimaGameModuleConstant.Skill);
                config.LoadConfig();
                foreach (string key in config.Keys)
                {
                    Skill prev = config[key];
                    Skill? next = GetSkill(prev.Id, prev.Name, prev.SkillType);
                    if (next != null)
                    {
                        config[key] = next;
                    }
                }
                return [.. config.Values];
            }
        }

        public override Skill? GetSkill(long id, string name, SkillType type)
        {
            if (type == SkillType.Magic)
            {
                switch ((MagicID)id)
                {
                    case MagicID.冰霜攻击:
                        return new 冰霜攻击();
                }
            }

            if (type == SkillType.Skill)
            {
                switch ((SkillID)id)
                {
                    case SkillID.疾风步:
                        return new 疾风步();
                }
            }

            if (type == SkillType.SuperSkill)
            {
                switch ((SuperSkillID)id)
                {
                    case SuperSkillID.力量爆发:
                        return new 力量爆发();
                    case SuperSkillID.天赐之力:
                        return new 天赐之力();
                    case SuperSkillID.魔法涌流:
                        return new 魔法涌流();
                    case SuperSkillID.三重叠加:
                        return new 三重叠加();
                    case SuperSkillID.变幻之心:
                        return new 变幻之心();
                    case SuperSkillID.精准打击:
                        return new 精准打击();
                    case SuperSkillID.绝对领域:
                        return new 绝对领域();
                    case SuperSkillID.能量毁灭:
                        return new 能量毁灭();
                    case SuperSkillID.迅捷之势:
                        return new 迅捷之势();
                    case SuperSkillID.嗜血本能:
                        return new 嗜血本能();
                    case SuperSkillID.平衡强化:
                        return new 平衡强化();
                    case SuperSkillID.血之狂欢:
                        return new 血之狂欢();
                }
            }

            if (type == SkillType.Passive)
            {
                switch ((PassiveID)id)
                {
                    case PassiveID.META马:
                        return new META马();
                    case PassiveID.心灵之火:
                        return new 心灵之火();
                    case PassiveID.魔法震荡:
                        return new 魔法震荡();
                    case PassiveID.灵能反射:
                        return new 灵能反射();
                    case PassiveID.智慧与力量:
                        return new 智慧与力量();
                    case PassiveID.致命打击:
                        return new 致命打击();
                    case PassiveID.毁灭之势:
                        return new 毁灭之势();
                    case PassiveID.枯竭打击:
                        return new 枯竭打击();
                    case PassiveID.玻璃大炮:
                        return new 玻璃大炮();
                    case PassiveID.累积之压:
                        return new 累积之压();
                    case PassiveID.敏捷之刃:
                        return new 敏捷之刃();
                    case PassiveID.弱者猎手:
                        return new 弱者猎手();
                }
                switch ((ItemPassiveID)id)
                {
                    case ItemPassiveID.攻击之爪:
                        return new 攻击之爪技能();
                }
            }

            return null;
        }
    }
}
