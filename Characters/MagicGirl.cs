using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Milimoe.FunGame.GameMode.OfficialStandard.Characters
{
    public class MagicGirl : Character
    {
        public MagicGirl() : base()
        {
            Name = "Magic Girl";
            FirstName = "魔法少女";
            FirstRoleType = RoleType.Core;
            STR = 3;
            AGI = 8;
            INT = 4;
        }
    }
}
