using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class ColdBlue : Character
    {
        public ColdBlue() : base()
        {
            Id = 10;
            Name = "Cold";
            FirstName = "Blue";
            NickName = "冷蓝";
            PrimaryAttribute = PrimaryAttribute.STR;
            InitialATK = 28;
            InitialHP = 95;
            InitialMP = 25;
            InitialSTR = 22;
            STRGrowth = 1.9;
            InitialAGI = 4;
            AGIGrowth = 0.6;
            InitialINT = 4;
            INTGrowth = 0.6;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
