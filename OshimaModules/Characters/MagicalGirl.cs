using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class MagicalGirl : Character
    {
        public MagicalGirl() : base()
        {
            Id = 7;
            Name = "Magical";
            FirstName = "Girl";
            NickName = "魔法少女";
            PrimaryAttribute = PrimaryAttribute.AGI;
            InitialATK = 20;
            InitialHP = 95;
            InitialMP = 35;
            InitialSTR = 7;
            STRGrowth = 0.3;
            InitialAGI = 15;
            AGIGrowth = 2.3;
            InitialINT = 8;
            INTGrowth = 0.4;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
