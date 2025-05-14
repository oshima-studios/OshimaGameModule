using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class XinYin : Character
    {
        public XinYin() : base()
        {
            Id = 2;
            Name = "Xiyue";
            FirstName = "XinYin";
            NickName = "心音";
            PrimaryAttribute = PrimaryAttribute.AGI;
            InitialATK = 22;
            InitialHP = 85;
            InitialMP = 60;
            InitialSTR = 8;
            STRGrowth = 0.9;
            InitialAGI = 19;
            AGIGrowth = 1.7;
            InitialINT = 3;
            INTGrowth = 0.4;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;

        }
    }
}
