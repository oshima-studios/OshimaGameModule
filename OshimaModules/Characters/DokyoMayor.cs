using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class DokyoMayor : Character
    {
        public DokyoMayor() : base()
        {
            Id = 6;
            Name = "Dokyo";
            FirstName = "Mayor";
            NickName = "铎京市长";
            PrimaryAttribute = PrimaryAttribute.AGI;
            InitialATK = 21;
            InitialHP = 120;
            InitialMP = 20;
            InitialSTR = 7;
            STRGrowth = 1;
            InitialAGI = 21;
            AGIGrowth = 1.8;
            InitialINT = 2;
            INTGrowth = 0.2;
            InitialSPD = 300;
            InitialHR = 4;
            InitialMR = 2;
        }
    }
}
