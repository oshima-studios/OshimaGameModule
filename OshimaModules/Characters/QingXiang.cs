using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class QingXiang : Character
    {
        public QingXiang() : base()
        {
            Id = 8;
            Name = "Qing";
            FirstName = "Xiang";
            NickName = "清香";
            PrimaryAttribute = PrimaryAttribute.INT;
            InitialATK = 26;
            InitialHP = 110;
            InitialMP = 80;
            InitialSTR = 6;
            STRGrowth = 0.5;
            InitialAGI = 4;
            AGIGrowth = 0.5;
            InitialINT = 20;
            INTGrowth = 2;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
