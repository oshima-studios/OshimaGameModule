using System.Text;
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

        public override bool Equals(IBaseEntity? other)
        {
            return other is OshimaRegion && other.GetIdName() == GetIdName();
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

            builder.AppendLine($"探索难度：{CharacterSet.GetRarityTypeName(Difficulty)}");

            return builder.ToString().Trim();
        }
    }
}
