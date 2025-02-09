using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Units
{
    public class RegionCharacter : Character
    {
        public HashSet<Func<Region, bool>> GenerationPredicates { get; } = [];

        public RegionCharacter(long id, string name, params IEnumerable<Func<Region, bool>> predicates)
        {
            Id = id;
            Name = name;
            NickName = name;
            PrimaryAttribute = (PrimaryAttribute)Random.Shared.Next(1, 4);
            InitialATK = Random.Shared.Next(55, 101);
            InitialHP = Random.Shared.Next(80, 201);
            InitialMP = Random.Shared.Next(50, 131);

            int value = 61;
            int valueGrowth = 61;
            for (int i = 0; i < 3; i++)
            {
                if (value == 0) break;
                int attribute = i < 2 ? Random.Shared.Next(value) : (value - 1);
                int growth = i < 2 ? Random.Shared.Next(0, valueGrowth) : (valueGrowth - 1);
                switch (i)
                {
                    case 1:
                        InitialAGI = attribute;
                        AGIGrowth = Calculation.Round(Convert.ToDouble(growth) / 10, 2);
                        break;
                    case 2:
                        InitialINT = attribute;
                        INTGrowth = Calculation.Round(Convert.ToDouble(growth) / 10, 2);
                        break;
                    case 0:
                    default:
                        InitialSTR = attribute;
                        STRGrowth = Calculation.Round(Convert.ToDouble(growth) / 10, 2);
                        break;
                }
                value -= attribute;
                valueGrowth -= growth;
            }
            InitialSPD = Random.Shared.Next(220, 451);
            InitialHR = Random.Shared.Next(3, 9);
            InitialMR = Random.Shared.Next(3, 9);
            foreach (Func<Region, bool> predicate in predicates)
            {
                GenerationPredicates.Add(predicate);
            }
        }
    }
}
