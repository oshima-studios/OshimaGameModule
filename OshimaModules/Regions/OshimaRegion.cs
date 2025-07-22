using System.Text;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Items;
using Oshima.FunGame.OshimaModules.Units;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class OshimaRegion : Region
    {
        public new HashSet<RegionCharacter> Characters { get; } = [];
        public new HashSet<RegionUnit> Units { get; } = [];
        public new HashSet<RegionItem> Crops { get; } = [];
        public Dictionary<string, QuestExploration> ContinuousQuestList { get; set; } = [];
        public Dictionary<string, QuestExploration> ImmediateQuestList { get; set; } = [];
        public Dictionary<string, QuestExploration> ProgressiveQuestList { get; set; } = [];

        public override bool Equals(IBaseEntity? other)
        {
            return other is OshimaRegion && other.GetIdName() == GetIdName();
        }

        public virtual Store? VisitStore(EntityModuleConfig<Store> stores, User user, string storeName)
        {
            return null;
        }
        
        public virtual void SaveGlobalStore(Store store, string storeName)
        {

        }

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine($"☆--- {Name} ---☆");
            builder.AppendLine($"编号：{Id}");
            builder.AppendLine($"天气：{Weather}");
            builder.AppendLine($"温度：{Temperature} °C");
            builder.AppendLine($"{Description}");

            if (Characters.Count > 0)
            {
                builder.AppendLine($"== 头目 ==");
                builder.AppendLine(string.Join("，", Characters.Select(c => c.Name)));
            }

            if (Units.Count > 0)
            {
                builder.AppendLine($"== 生物 ==");
                builder.AppendLine(string.Join("，", Units.Select(u => u.Name)));
            }

            if (Crops.Count > 0)
            {
                builder.AppendLine($"== 作物 ==");
                builder.AppendLine(string.Join("，", Crops.Select(i => i.Name + "：" + i.Description + "\"" + i.BackgroundStory + "\"")));
            }

            if (Items.Count > 0)
            {
                builder.AppendLine($"== 掉落 ==");
                builder.AppendLine(string.Join("，", Items.Select(i =>
                {
                    string itemquality = ItemSet.GetQualityTypeName(i.QualityType);
                    string itemtype = ItemSet.GetItemTypeName(i.ItemType) + (i.ItemType == ItemType.Weapon && i.WeaponType != WeaponType.None ? "-" + ItemSet.GetWeaponTypeName(i.WeaponType) : "");
                    if (itemtype != "") itemtype = $"|{itemtype}";
                    return $"[{itemquality + itemtype}]{i.Name}";
                })));
            }

            builder.AppendLine($"探索难度：{CharacterSet.GetRarityTypeName(Difficulty)}");

            return builder.ToString().Trim();
        }
    }
}
