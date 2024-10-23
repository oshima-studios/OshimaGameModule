using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

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
                    Skill? next = EntityFactory.GetSkill(prev.Id, prev.SkillType);
                    if (next != null)
                    {
                        config[key] = next;
                    }
                    Skill skill = config[key];
                    List<Effect> effects = [.. skill.Effects];
                    foreach (Effect effect in effects)
                    {
                        Effect? newEffect = EntityFactory.GetEffect(effect.Id, skill);
                        if (newEffect != null)
                        {
                            skill.Effects.Remove(effect);
                            skill.Effects.Add(newEffect);
                        }
                    }
                }
                return [.. config.Values];
            }
        }

        public override Skill? GetSkill(long id, string name, SkillType type) => EntityFactory.GetSkill(id, type);
    }
}
