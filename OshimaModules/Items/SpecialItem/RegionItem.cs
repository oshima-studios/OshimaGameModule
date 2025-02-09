using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Items
{
    public class RegionItem : Item
    {
        public HashSet<Func<Region, bool>> GenerationPredicates { get; } = [];

        public RegionItem(long id, string name, string description, string story = "", QualityType quality = QualityType.White, params IEnumerable<Func<Region, bool>> predicates) : base(ItemType.SpecialItem)
        {
            Id = id;
            Name = name;
            Description = description;
            BackgroundStory = story;
            QualityType = quality;
            foreach (Func<Region, bool> predicate in predicates)
            {
                GenerationPredicates.Add(predicate);
            }
        }
    }
}
