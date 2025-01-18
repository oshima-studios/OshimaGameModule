using Milimoe.FunGame.Core.Library.Common.Addon;
using Oshima.Core.Constant;

namespace Oshima.FunGame.OshimaMaps
{
    public class FastAutoMap : GameMap
    {
        public override string Name => OshimaGameModuleConstant.FastAutoMap;

        public override string Description => OshimaGameModuleConstant.Description;

        public override string Version => OshimaGameModuleConstant.Version;

        public override string Author => OshimaGameModuleConstant.Author;

        public override float Length => 6;

        public override float Width => 6;

        public override float Height => 3;

        public override float Size => 6;
    }
}
