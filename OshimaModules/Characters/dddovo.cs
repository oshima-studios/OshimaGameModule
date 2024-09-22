using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class dddovo : Character
    {
        public dddovo() : base()
        {
            Id = 11;
            Name = "ddd";
            FirstName = "ovo";
            NickName = "绿拱门";
            PrimaryAttribute = PrimaryAttribute.AGI;
            InitialATK = 22;
            InitialHP = 65;
            InitialMP = 22;
            InitialSTR = 10;
            STRGrowth = 1;
            InitialAGI = 20;
            AGIGrowth = 2;
            InitialINT = 0;
            INTGrowth = 0;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
