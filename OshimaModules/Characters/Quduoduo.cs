using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class Quduoduo : Character
    {
        public Quduoduo() : base()
        {
            Id = 12;
            Name = "Qu";
            FirstName = "Duoduo";
            NickName = "趣多多";
            PrimaryAttribute = PrimaryAttribute.STR;
            InitialATK = 19;
            InitialHP = 90;
            InitialMP = 40;
            InitialSTR = 13;
            STRGrowth = 1.5;
            InitialAGI = 13;
            AGIGrowth = 1.2;
            InitialINT = 4;
            INTGrowth = 0.3;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
