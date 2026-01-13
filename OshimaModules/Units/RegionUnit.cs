using Milimoe.FunGame.Core.Entity;

namespace Oshima.FunGame.OshimaModules.Units
{
    public class RegionUnit : Unit
    {
        public override bool IsUnit => false; // 不走单位判断
        public HashSet<Func<Region, bool>> GenerationPredicates { get; } = [];

        public RegionUnit(long id, string name, params IEnumerable<Func<Region, bool>> predicates)
        {
            Id = id;
            Name = name;
            InitialATK = Random.Shared.Next(25, 51);
            InitialHP = Random.Shared.Next(35, 91);
            InitialMP = Random.Shared.Next(20, 61);
            InitialSPD = Random.Shared.Next(155, 320);
            InitialHR = Random.Shared.Next(1, 6);
            InitialMR = Random.Shared.Next(1, 6);
            foreach (Func<Region, bool> predicate in predicates)
            {
                GenerationPredicates.Add(predicate);
            }
        }
    }
}
