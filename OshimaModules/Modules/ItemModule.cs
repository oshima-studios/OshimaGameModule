using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules
{
    public class ItemModule : Milimoe.FunGame.Core.Library.Common.Addon.ItemModule
    {
        public override string Name => OshimaGameModuleConstant.Item;
        public override string Description => OshimaGameModuleConstant.Description;
        public override string Version => OshimaGameModuleConstant.Version;
        public override string Author => OshimaGameModuleConstant.Author;

        public override List<Item> Items
        {
            get
            {
                EntityModuleConfig<Item> config = new(OshimaGameModuleConstant.General, OshimaGameModuleConstant.Item);
                config.LoadConfig();
                foreach (string key in config.Keys)
                {
                    Item prev = config[key];
                    Item? next = GetItem(prev.Id, prev.Name, prev.ItemType);
                    if (next != null)
                    {
                        prev.SetPropertyToItemModuleNew(next);
                        config[key] = next;
                    }
                    Item item = config[key];
                    HashSet<Skill> skills = item.Skills.Passives;
                    if (item.Skills.Active != null) skills.Add(item.Skills.Active);
                    List<Skill> skilllist = [.. skills];
                    foreach (Skill skill in skilllist)
                    {
                        Skill? newSkill = EntityFactory.GetSkill(skill.Id, skill.SkillType);
                        if (newSkill != null)
                        {
                            if (newSkill.IsActive)
                            {
                                item.Skills.Active = newSkill;
                            }
                            else
                            {
                                item.Skills.Passives.Remove(skill);
                                item.Skills.Passives.Add(newSkill);
                            }
                        }
                        Skill s = newSkill ?? skill;
                        List<Effect> effects = [.. s.Effects];
                        foreach (Effect effect in effects)
                        {
                            skill.Effects.Remove(effect);
                            Effect? newEffect = EntityFactory.GetEffect(effect.Id, skill);
                            if (newEffect != null)
                            {
                                skill.Effects.Add(newEffect);
                            }
                        }
                    }
                }
                return [.. config.Values];
            }
        }

        protected override void AfterLoad()
        {
            Factory.OpenFactory.RegisterFactory(args =>
            {
                if (args.TryGetValue("id", out object? value) && value is long id && args.TryGetValue("itemtype", out value) && value is int type)
                {
                    Item? item = EntityFactory.GetItem(id, (ItemType)type);
                    if (item != null)
                    {
                        HashSet<Skill> skills = item.Skills.Passives;
                        if (item.Skills.Active != null) skills.Add(item.Skills.Active);
                        List<Skill> skilllist = [.. skills];
                        foreach (Skill skill in skilllist)
                        {
                            item.Skills.Passives.Remove(skill);
                            Skill newSkill = EntityFactory.GetSkill(skill.Id, skill.SkillType) ?? new OpenSkill(skill.Id, skill.Name);
                            if (newSkill != null)
                            {
                                if (newSkill.IsActive)
                                {
                                    item.Skills.Active = newSkill;
                                }
                                else
                                {
                                    item.Skills.Passives.Add(newSkill);
                                }
                            }
                            Skill s = newSkill ?? skill;
                            List<Effect> effects = [.. s.Effects];
                            foreach (Effect effect in effects)
                            {
                                skill.Effects.Remove(effect);
                                Effect? newEffect = EntityFactory.GetEffect(effect.Id, skill);
                                if (newEffect != null)
                                {
                                    skill.Effects.Add(newEffect);
                                }
                            }
                        }
                        return item;
                    }
                }
                return null;
            });
        }

        public override Item? GetItem(long id, string name, ItemType type) => EntityFactory.GetItem(id, type);
    }
}
