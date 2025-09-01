using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Oshima.Core.Constant;

namespace Oshima.FunGame.OshimaMaps
{
    public class AnonymousMap : GameMap
    {
        public override string Name => OshimaGameModuleConstant.AnonymousMap;

        public override string Description => OshimaGameModuleConstant.Description;

        public override string Version => OshimaGameModuleConstant.Version;

        public override string Author => OshimaGameModuleConstant.Author;

        public override int Length => 6;

        public override int Width => 6;

        public override int Height => 3;

        public override float Size => 6;

        public override GameMap InitGamingQueue(IGamingQueue queue)
        {
            GameMap map = new AnonymousMap();
            map.Load();
            return map;
        }
    }
}
